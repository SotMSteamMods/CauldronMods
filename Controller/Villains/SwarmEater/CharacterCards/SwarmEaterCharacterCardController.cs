using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class SwarmEaterCharacterCardController : VillainCharacterCardController
    {
        private const string SingleMindedPursuitIdent = "SingleMindedPursuit";

        public SwarmEaterCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => !base.Card.IsFlipped, () => this.PursuedCard().AlternateTitleOrTitle + " is the pursued target.", () => "No one is being pursued");
            base.SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria((Card c) => c != base.Card));
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                //If Single-Minded Pursuit leaves play, flip {SwarmEater}'s villain character cards.
                base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.CardToDestroy.Card.Identifier == SingleMindedPursuitIdent && action.WasCardDestroyed, base.FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.After));

                //At the start of the villain turn, {SwarmEater} deals the pursued target 3 psychic damage.
                base.AddSideTrigger(base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => this.IsPursued(c), TargetType.All, 3, DamageType.Psychic));

                //Whenever a pursued hero deals damage to a target other than {SwarmEater}, you may move Single-Minded Pursuit next to that target.
                base.AddSideTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction action) => this.IsPursued(action.DamageSource.Card) && action.DamageSource.Card.IsHeroCharacterCard && action.Target != base.Card && action.Target != action.DamageSource.Card, this.ChangePursuedResponse, TriggerType.MoveCard, TriggerTiming.After));

                if (base.Game.IsAdvanced)
                {
                    //Increase damage dealt by {SwarmEater} to environment targets by 1.
                    base.AddSideTrigger(base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.Card && action.Target.IsEnvironment, 1));
                }
            }
            else
            {
                //At the start of the villain turn, {SwarmEater} deals the target other than itself with the lowest HP 2 melee damage.
                base.AddSideTrigger(base.AddDealDamageAtStartOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c != base.Card, TargetType.LowestHP, 2, DamageType.Melee));

                //Whenever Single-Minded Pursuit enters play, flip {SwarmEater}'s villain character cards.
                /**************Trigger added to Single-Minded Pursuit****************/

                //Whenever a villain card is played {SwarmEater} deals the non-hero target other than itself with the lowest HP 3 melee damage.
                base.AddSideTrigger(base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.CardToPlay.IsVillain && action.WasCardPlayed && !action.IsPutIntoPlay, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After));
                if (base.Game.IsAdvanced)
                {
                    //Whenever {SwarmEater} destroys a villain target, play the top card of the villain deck.
                    base.AddSideTrigger(base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.ResponsibleCard == base.Card && action.WasCardDestroyed && action.CardToDestroy.Card.IsVillain, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard, TriggerTiming.After));
                }
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        private bool IsPursued(Card card)
        {
            return card.NextToLocation.Cards.Any((Card c) => c.Identifier == SingleMindedPursuitIdent);
        }

        private Card PursuedCard()
        {
            return base.FindCardsWhere(new LinqCardCriteria(this.IsPursued)).FirstOrDefault();
        }

        private IEnumerator ChangePursuedResponse(DealDamageAction action)
        {
            Card pursuit = base.FindCard(SingleMindedPursuitIdent);
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(this.DecisionMaker, SelectionType.MoveCardNextToCard, pursuit, storedResults: storedResults, associatedCards: action.Target.ToEnumerable<Card>());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(storedResults))
            {
                coroutine = base.GameController.MoveCard(base.TurnTakerController, pursuit, action.Target.NextToLocation, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private IEnumerator DealDamageResponse(PlayCardAction action)
        {
            //...{SwarmEater} deals the non-hero target other than itself with the lowest HP 3 melee damage.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => c != base.Card && !c.IsHero, (Card c) => 3, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
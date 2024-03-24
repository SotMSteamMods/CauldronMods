using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class InsubstantialMatadorCardController : PhaseVillainCardController
    {
        public InsubstantialMatadorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = base.SpecialStringMaker.ShowIfElseSpecialString(() => this.DidDamageVillainThisTurn(),
                () => "Phase has been dealt damage by this hero this turn.",
                () => "Phase has not been dealt damage by this hero this turn.");
            ss.Condition = () => Card.IsInPlay;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, {Phase} deals the hero target with the second lowest HP {H - 1} radiant damage.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 2, (Card c) => IsHero(c), (Card c) => Game.H - 1, DamageType.Radiant);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //At the end of each hero's turn, if that hero dealt {Phase} no damage, that hero deals themselves 1 irreducible melee damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !this.DidDamageVillainThisTurn(), this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...that hero deals themselves 1 irreducible melee damage.
            IEnumerator coroutine = null;
            if (base.FindHeroTurnTakerController(base.Game.ActiveTurnTaker.ToHero()).HasMultipleCharacterCards)
            {
                coroutine = base.GameController.SelectTargetsToDealDamageToSelf(this.DecisionMaker, 1, DamageType.Melee, 1, false, 1, true, additionalCriteria: (Card c) =>  IsHeroCharacterCard(c) && c.Owner == base.Game.ActiveTurnTaker, cardSource: GetCardSource());
            }
            else
            {
                Card hero = base.Game.ActiveTurnTaker.CharacterCard;
                coroutine = base.DealDamage(hero, hero, 1, DamageType.Melee, true, cardSource: base.GetCardSource());
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private bool DidDamageVillainThisTurn()
        {
            return base.Journal.DealDamageEntriesThisTurnSinceCardWasPlayed(base.Card).Any((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard && entry.Amount > 0);
        }
    }
}
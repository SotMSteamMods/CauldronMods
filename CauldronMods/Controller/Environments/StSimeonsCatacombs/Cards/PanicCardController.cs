using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class PanicCardController : StSimeonsBaseCardController
    {
        public static readonly string Identifier = "Panic";

        public PanicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the highest HP
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredHighestHitPoints(c, (Card card) => card.IsHeroCharacterCard) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero target with the lowest HP"), storedResults, isPutIntoPlay, decisionSources);
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
            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo().Owner, this.StartOfHeroResponse, new TriggerType[]
            {
                TriggerType.UsePower,
                TriggerType.SkipTurn,
                TriggerType.DrawCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator StartOfHeroResponse(PhaseChangeAction pca)
        {
            HeroTurnTakerController httc = base.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());

            IEnumerator message = base.GameController.SendMessageAction(base.Card.Title + " causes " + httc.Name + " to use their innate power twice!", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
            }

            //that hero uses their innate power twice, 
            CardController cc = GameController.FindCardController(base.GetCardThisCardIsNextTo());

            if(cc.Card.IsIncapacitatedOrOutOfGame)
            {
                yield break;
            }

            IEnumerator power1 = base.UsePowerOnOtherCard(cc.Card);

            if (base.GetCardThisCardIsNextTo().NumberOfPowers > 1)
            {
                power1 = base.GameController.SelectAndUsePower(httc, false, p => p.CardController == httc.CharacterCardController,eliminateUsedPowers: false, cardSource: GetCardSource());

            }
            //then immediately end their turn, 
            IEnumerator endTurn = base.GameController.ImmediatelyEndTurn(httc, base.GetCardSource());
            //draw a card
            IEnumerator draw = base.DrawCard(httc.HeroTurnTaker);
            //destroy this card.
            IEnumerator destroy = base.DestroyThisCardResponse(pca);

            //execute power 1
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(power1);
                
            }
            else
            {
                base.GameController.ExhaustCoroutine(power1);
            }
            
            //kill sequence if this card has been destroyed
            if(!base.Card.IsInPlayAndHasGameText)
            {
                Log.Debug("No longer in play!");
                yield break;
            }

            if(cc.Card.IsIncapacitatedOrOutOfGame)
            {
                Log.Debug("Hero this card is next to is incapacitated or out of the game!");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(endTurn);
                    yield return base.GameController.StartCoroutine(destroy);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(endTurn);
                    base.GameController.ExhaustCoroutine(destroy);
                }

                yield break;
            }

            //we collect power2 after we've ran power1 basically for things like extremist skyscraper where the innate's have changed as a result of power 1
            IEnumerator power2 = base.UsePowerOnOtherCard(cc.Card);
            if (base.GetCardThisCardIsNextTo().NumberOfPowers > 1)
            {
                power2 = base.GameController.SelectAndUsePower(httc, false, p => p.CardController == httc.CharacterCardController, eliminateUsedPowers: false, cardSource: GetCardSource());

            }

            //execute power 2
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(power2);

            }
            else
            {
                base.GameController.ExhaustCoroutine(power2);
            }

            if (!base.Card.IsInPlayAndHasGameText)
            {
                Log.Debug("No longer in play!");
                yield break;
            }

            if (cc.Card.IsIncapacitatedOrOutOfGame)
            {
                Log.Debug("Hero this card is next to is incapacitated or out of the game!");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(endTurn);
                    yield return base.GameController.StartCoroutine(destroy);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(endTurn);
                    base.GameController.ExhaustCoroutine(destroy);
                }

                yield break;
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(endTurn);
                yield return base.GameController.StartCoroutine(draw);
                yield return base.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(endTurn);
                base.GameController.ExhaustCoroutine(draw);
                base.GameController.ExhaustCoroutine(destroy);
            }

            yield break;
        }
    }
}

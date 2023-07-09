using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class InjuredWorkerCardController : WindmillCityUtilityCardController
    {
        public InjuredWorkerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => IsResponder(c), "responder"));
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => GetCardThisCardIsNextTo() == null;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the Responder wth the lowest HP 2 melee damage. 
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => IsResponder(c), (Card c) => 2, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Then move this card next to the hero with the highest HP.           
            List<Card> storedResults = new List<Card>();
            coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame, storedResults,cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResults.FirstOrDefault();
            if (card == null)
            {

                string message = $"There are no heroes in play to put {Card.Title} next to.";

                coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), showCardSource: true);
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

            coroutine = base.GameController.MoveCard(TurnTakerController, Card, card.NextToLocation, cardSource: GetCardSource());
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
            //Increase the first damage dealt to that hero each turn by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => CheckCriteria(dd), 1);
        }

        private bool CheckCriteria(DealDamageAction dd)
        {
            return GetCardThisCardIsNextTo() != null && dd.Target != null && dd.Target == GetCardThisCardIsNextTo() && !HasBeenDealtDamageThisTurn(GetCardThisCardIsNextTo());
        }

    }
}

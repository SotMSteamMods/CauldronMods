using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{

    public class HullCladdingCardController : PyreUtilityCardController
    {

        public HullCladdingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => GameController.GetAllCards().Any((Card c) => c.IsInPlayAndHasGameText && c.Identifier == ContainmentBreachIdentifier), () => "Containment Breach is in play", () => "Containment Breach is not in play.");
        }

        public readonly string ContainmentBreachIdentifier = "ContainmentBreach";

        public override bool ShouldBeDestroyedNow()
        {
            return Card.IsInPlayAndHasGameText && FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == ContainmentBreachIdentifier).Any();
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to and by {Pyre} by 1.",
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);
            AddReduceDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(CharacterCard), dd => 1);

            //"If Containment Breach is ever in play, destroy it or destroy this card."
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay.Identifier == ContainmentBreachIdentifier || (cep.CardEnteringPlay == Card && GameController.GetAllCards().Any((Card c) => c.IsInPlayAndHasGameText && c.Identifier == ContainmentBreachIdentifier && GameController.IsCardVisibleToCardSource(c, GetCardSource()))), DestroyCladdingOrBreachResponse, TriggerType.DestroyCard, TriggerTiming.After);
        }

        private IEnumerator DestroyCladdingOrBreachResponse(CardEntersPlayAction cep)
        {
            return GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c == Card || c.Identifier == ContainmentBreachIdentifier), false, cardSource: GetCardSource());
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numDraw = GetPowerNumeral(0, 2);
            //"Draw 2 cards. 
            IEnumerator coroutine = DrawCards(DecisionMaker, numDraw);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Destroy this card."
            coroutine = DestroyThisCardResponse(null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

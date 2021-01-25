using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class DubiousEdiblesCardController : DungeonsOfTerrorUtilityCardController
    {
        public DubiousEdiblesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, check the top card of the environment trash.
            List<int> storedResults = new List<int>();
            Card cardToCheck = TurnTaker.Trash.TopCard;
            IEnumerator coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator message = DoNothing();
            IEnumerator effect = DoNothing();
            if (storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, this card deals each hero 1 toxic damage.
                message = GameController.SendMessageAction($"The top card of the environment trash is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = DealDamage(Card, (Card c) => c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame, 1, DamageType.Toxic);
            }
            else if(storedResults.Any() && storedResults.First() == 0)
            {
                //If it is not a fate card, 1 hero target regains 4HP. 
                message = GameController.SendMessageAction($"The top card of the environment trash is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = GameController.SelectAndGainHP(DecisionMaker, 4, additionalCriteria: (Card c) => c.IsHero && GameController.IsCardVisibleToCardSource(c, GetCardSource()), cardSource: GetCardSource());
            } else
            {
                message = GameController.SendMessageAction("There are no cards in the environment trash!", Priority.High, GetCardSource(), showCardSource: true);
            }
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

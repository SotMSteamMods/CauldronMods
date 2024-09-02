using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightOfAsheronCharacterCardController : MultiStarlightIndividualCardController
    {
        public StarlightOfAsheronCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
            if (base.TurnTaker.IsPlayer)
            {
                SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.Hand, new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {

            int targets = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 1);
            
            //Play a constellation.
            IEnumerator playRoutine = SelectAndPlayCardFromHand(HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => IsConstellation(c)));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playRoutine);
            }

            List<Card> storedResults = new List<Card> { };
            IEnumerator pickStarlight = SelectActiveCharacterCardToDealDamage(storedResults, amount, DamageType.Radiant);
            if (UseUnityCoroutines)
            {

                yield return GameController.StartCoroutine(pickStarlight);
            }
            else
            {

                GameController.ExhaustCoroutine(pickStarlight);
            }

            Card chosenStarlight = storedResults.FirstOrDefault();
            if (chosenStarlight == null)
            {
                IEnumerator message = GameController.SendMessageAction("No Starlight was picked!", Priority.High, GetCardSource());
                yield break;
            }

            //One Starlight deals 1 target 1 radiant damage.
            IEnumerator damageRoutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController,
                                                                        new DamageSource(GameController, chosenStarlight),
                                                                        amount,
                                                                        DamageType.Radiant,
                                                                        targets,
                                                                        false,
                                                                        targets,
                                                                        cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageRoutine);
            }
            yield break;
        }

    }
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightOfCryosFourCharacterCardController : StarlightSubCharacterCardController
    {
        public StarlightOfCryosFourCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DealDamageAction> damageActions = new List<DealDamageAction> { };
            int damAmount = GetPowerNumeral(0, 2);

            //"This Starlight deals herself 2 irreducible cold damage."
            IEnumerator damageRoutine = DealDamage(this.Card, this.Card, damAmount, DamageType.Cold, isIrreducible: true, storedResults: damageActions, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(damageRoutine);
            }

            // "If she takes damage this way, play 2 constellations from your trash."
            if (DidDealDamage(damageActions, this.Card))
            {
                IEnumerator playRoutine = SelectAndPlayConstellationsFromTrash();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(playRoutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(playRoutine);
                }
            }
            yield break;
        }

        private IEnumerator SelectAndPlayConstellationsFromTrash()
        {
            List<PlayCardAction> playedCards = new List<PlayCardAction> {};
            IEnumerator playRoutine = GameController.SelectAndPlayCard(HeroTurnTakerController,
                                                        (Card c) => c.Owner == TurnTaker && c.IsInTrash && IsConstellation(c),
                                                        cardSource: GetCardSource(),
                                                        noValidCardsMessage: "There were no playable constellations in the trash.",
                                                        storedResults: playedCards);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(playRoutine);
            }

            if(DidPlayCards(playedCards))
            {
                playRoutine = GameController.SelectAndPlayCard(HeroTurnTakerController,
                                                (Card c) => c.Owner == TurnTaker && c.IsInTrash && IsConstellation(c),
                                                cardSource: GetCardSource(),
                                                noValidCardsMessage: "There were no more playable constellations in the trash.");
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(playRoutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(playRoutine);
                }
            }
            yield break;
        }
    }
}
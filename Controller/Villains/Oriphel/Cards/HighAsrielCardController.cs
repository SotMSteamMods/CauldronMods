using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighAsrielCardController : OriphelGuardianCardController
    {
        public HighAsrielCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Guardian destroy tregger
            base.AddTriggers();

            //"When a hero card is played, this card deals the hero with the highest HP 2 psychic damage.",
            AddTrigger((PlayCardAction pca) => pca.WasCardPlayed == true && !pca.IsPutIntoPlay && pca.CardToPlay.IsHero,
                            DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(PlayCardAction pca)
        {
            IEnumerator coroutine = DealDamageToHighestHP(this.Card, 1, (Card c) => c.IsHeroCharacterCard, (c) => 2, DamageType.Psychic);
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
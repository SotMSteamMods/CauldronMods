using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class BarnacleHydraCardController : TheWanderingIsleCardController
    {
        public BarnacleHydraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //When this card is destroyed, it deals Teryx { H} toxic damage
            base.AddWhenDestroyedTrigger(this.DestroySelfResponse, TriggerType.DealDamage);
            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageReponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP });
        }

        private IEnumerator DealDamageReponse(PhaseChangeAction pca)
        {
            //this card deals the non-environment target with the lowest HP 2 projectile damage. 
            IEnumerator dealDamage = base.DealDamageToLowestHP(base.Card, 1, (Card c) => c.IsTarget && c.IsInPlayAndHasGameText && !c.IsEnvironment, (Card c) => 2, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamage);
            }

            //Then, if Submerge is in play, this card regains 6HP.
            if (GameController.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "Submerge").Any())
            {
                IEnumerator gainHp = base.GameController.GainHP(base.Card, 6);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(gainHp);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(gainHp);
                }
            }
            yield break;
        }

        private IEnumerator DestroySelfResponse(DestroyCardAction dca)
        {
            var teryx = base.FindTeryx();
            if (teryx != null)
            {
                //it deals Teryx { H} toxic damage
                IEnumerator dealDamage = base.DealDamage(base.Card, teryx, base.H, DamageType.Toxic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamage);
                }
            }
            yield break;
        }
    }
}

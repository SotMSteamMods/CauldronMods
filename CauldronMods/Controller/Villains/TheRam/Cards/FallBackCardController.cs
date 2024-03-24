using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class FallBackCardController : TheRamUtilityCardController
    {
        public FallBackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers();
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheRam} deals each Up Close hero target {H - 1} melee damage."
            if (RamIfInPlay != null)
            {
                IEnumerator damage = DealDamage(RamIfInPlay, (Card c) => c.IsInPlayAndHasGameText && IsHeroTarget(c) && IsUpClose(c), H - 1, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damage);
                }
            }
            else
            {
                IEnumerator message = MessageNoRamToAct(GetCardSource(), "deal damage");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
            }

            // "Destroy all copies of Up Close in play."
            IEnumerator destroyUpClose = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "UpClose"), true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyUpClose);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyUpClose);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"{TheRam} is immune to damage from heroes that are not Up Close."
            AddImmuneToDamageTrigger((DealDamageAction dda) => dda.Target == GetRam && dda.DamageSource != null && dda.DamageSource.Card != null && dda.DamageSource.IsHero && dda.DamageSource.IsTarget && !IsUpClose(dda.DamageSource.Card));
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class PersonalDefenseSpinesCardController : TheRamUtilityCardController
    {
        public PersonalDefenseSpinesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers();
        }

        public override IEnumerator Play()
        {
            //"{TheRam} deals each Up Close hero target {H + 2} melee damage. 
            IEnumerator coroutine = DealDamage(GetRam, (Card c) => c.IsHero && c.IsTarget && IsUpClose(c), H + 2, DamageType.Melee);
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Destroy all ongoing and equipment cards belonging to those heroes.
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsUpClose(c.Owner) && (c.IsOngoing || IsEquipment(c))));
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{TheRam} deals each other hero target 2 projectile damage."
            coroutine = DealDamage(GetRam, (Card c) => c.IsHero && c.IsTarget && !IsUpClose(c), 2, DamageType.Projectile);
            if (base.UseUnityCoroutines)
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
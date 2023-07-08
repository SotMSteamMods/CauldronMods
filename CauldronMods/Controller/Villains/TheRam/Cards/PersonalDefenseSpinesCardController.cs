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
            IEnumerator coroutine;
            //"{TheRam} deals each Up Close hero target {H + 2} melee damage. 
            if (RamIfInPlay != null)
            {
                coroutine = DealDamage(GetRam, (Card c) => IsHeroTarget(c) && IsUpClose(c), H + 2, DamageType.Melee);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
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

            //Destroy all ongoing and equipment cards belonging to those heroes.
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsUpClose(c.Owner) && (IsOngoing(c) || IsEquipment(c))));
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{TheRam} deals each other hero target 2 projectile damage."
            if (RamIfInPlay != null)
            {
                coroutine = DealDamage(GetRam, (Card c) => IsHeroTarget(c) && !IsUpClose(c), 2, DamageType.Projectile);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
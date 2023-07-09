using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /*
     * Destroy up to 3 hero ongoing or equipment cards belonging to other players.
     * {Gargoyle} deals 1 target X toxic damage, where X is 3 times the number of cards destroyed this way.
     */
    public class UltimatumCardController : GargoyleUtilityCardController
    {
        public UltimatumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            int valueOfX = 0;

            // Destroy up to 3 hero ongoing or equipment cards belonging to other players.
            coroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((card) => (base.GameController.IsEquipment(card) || IsOngoing(card)) && card.IsInPlay && IsHero(card.Owner) && card.Owner != DecisionMaker.TurnTaker && GameController.IsCardVisibleToCardSource(card, GetCardSource())), 3, false, 0, storedResultsAction: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null)
            {
                valueOfX = storedResults.Count((dca) => dca.WasCardDestroyed);

                // {Gargoyle} deals 1 target X toxic damage, where X is 3 times the number of cards destroyed this way.
                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), valueOfX * 3, DamageType.Toxic, 1, false, 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }

            yield break;
        }
    }
}

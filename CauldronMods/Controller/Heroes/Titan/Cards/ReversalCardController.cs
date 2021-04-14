using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class ReversalCardController : TitanCardController
    {
        public ReversalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<SelectCardDecision> target = new List<SelectCardDecision>();
            //{Titan} deals 1 target 1 infernal damage.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Infernal, 1, false, 1, storedResultsDecisions: target, addStatusEffect: AddRedirectStatusEffect, selectTargetsEvenIfCannotDealDamage: true, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If Titanform is in your trash, you may put it into play or into your hand.
            if (base.GetTitanform().Location.IsTrash)
            {
                IEnumerable<MoveCardDestination> locations = new MoveCardDestination[]
                {
                    new MoveCardDestination(base.TurnTaker.PlayArea),
                    new MoveCardDestination(base.HeroTurnTaker.Hand)
                };
                coroutine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, base.GetTitanform(), locations, true, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
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

        private IEnumerator AddRedirectStatusEffect(DealDamageAction action)
        {
            Card target = action.Target;
            RedirectDamageStatusEffect statusEffect = new RedirectDamageStatusEffect()
            {
                NumberOfUses = 1,
                RedirectTarget = target,
                SourceCriteria = { IsSpecificCard = target }
            };
            statusEffect.UntilCardLeavesPlay(target);
            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
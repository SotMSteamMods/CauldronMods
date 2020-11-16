using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class NightloreArmorCardController : StarlightCardController
    {
        public NightloreArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever damage would be dealt to another hero target, you may destroy a constellation card in play to prevent that damage."
            AddTrigger((DealDamageAction dd) => dd.Target.IsHero && !ListStarlights().Contains(dd.Target) && dd.Amount > 0,
                DestroyConstellationToPreventDamage,
                new TriggerType[3]
                    {
                        TriggerType.DestroyCard,
                        TriggerType.WouldBeDealtDamage,
                        TriggerType.CancelAction
                    },
                timing: TriggerTiming.Before);
        }

        private IEnumerator DestroyConstellationToPreventDamage(DealDamageAction dd)
        {
            Func<Card, bool> constellationInPlayCriteria = (Card c) => IsConstellation(c) && c.IsInPlayAndHasGameText;
            var constellationsInPlay = GameController.FindCardsWhere(constellationInPlayCriteria);
            if (constellationsInPlay.Count() == 0)
            {
                //don't bother player with trigger they can't do anything about
                yield break;
            }

            //"...you may..."
            List<YesNoCardDecision> yesNoDecision = new List<YesNoCardDecision> { };
            IEnumerator askPrevent = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.DestroyCard, base.Card, dd, yesNoDecision, constellationsInPlay, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(askPrevent);
            }
            else
            {
                base.GameController.ExhaustCoroutine(askPrevent);
            }
            if (!DidPlayerAnswerYes(yesNoDecision))
            {
                yield break;
            }

            //"...destroy a constellation in play..."
            IEnumerator destroyConstellation = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(constellationInPlayCriteria), optional:false, cardSource: GetCardSource());
            //"...to prevent that damage."
            IEnumerator preventDamage = GameController.CancelAction(dd, isPreventEffect: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyConstellation);
                yield return base.GameController.StartCoroutine(preventDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyConstellation);
                base.GameController.ExhaustCoroutine(preventDamage);
            }
            yield break;
        }
    }
}
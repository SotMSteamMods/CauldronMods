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
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
        }

        public override void AddTriggers()
        {
            //"Whenever damage would be dealt to another hero target, you may destroy a constellation card in play to prevent that damage."
            AddTrigger((DealDamageAction dd) => dd.Target.IsHero && !ListStarlights().Contains(dd.Target) && dd.Amount > 0,
                DestroyConstellationToPreventDamage,
                new TriggerType[]
                    {
                        TriggerType.DestroyCard,
                        TriggerType.WouldBeDealtDamage,
                        TriggerType.CancelAction
                    },
                isActionOptional: true,
                timing: TriggerTiming.Before);
        }

        private IEnumerator DestroyConstellationToPreventDamage(DealDamageAction dd)
        {
            var constellationsInPlay = FindCardsWhere(IsConstellationInPlay);
            if (constellationsInPlay.Count() == 0)
            {
                //don't bother player with trigger they can't do anything about
                yield break;
            }

            //"...you may..."
            List<YesNoCardDecision> yesNoDecision = new List<YesNoCardDecision> { };

            //What does the associatedCards argument actually do here? Should/should not be passing in the list of constellations?
            //Looks like it puts a constellation on the other side of the decision. Probably a good idea.
            IEnumerator askPrevent = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.PreventDamage, Card, dd, yesNoDecision, constellationsInPlay, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(askPrevent);
            }
            else
            {
                GameController.ExhaustCoroutine(askPrevent);
            }
            if (!DidPlayerAnswerYes(yesNoDecision))
            {
                yield break;
            }

            //"...destroy a constellation in play..."
            IEnumerator destroyConstellation = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(IsConstellationInPlay, "constellation"), optional: false, cardSource: GetCardSource());
            //"...to prevent that damage."
            IEnumerator preventDamage = CancelAction(dd, isPreventEffect: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(destroyConstellation);
                yield return GameController.StartCoroutine(preventDamage);
            }
            else
            {
                GameController.ExhaustCoroutine(destroyConstellation);
                GameController.ExhaustCoroutine(preventDamage);
            }
            yield break;
        }

        private bool IsConstellationInPlay(Card c)
        {
            return IsConstellation(c) && c.IsInPlayAndHasGameText;
        }
    }
}
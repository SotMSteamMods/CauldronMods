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

        public readonly string HasConstellationBeenDestroyed = "HasConstellationBeenDestroyed";
        public readonly string HasSaidNoToNightloreArmor = "HasSaidNoToNightloreArmor";
        public readonly string CurrentActionGUID = "CurrentActionGUID";

        private bool hasSaidNoToNightloreArmor = false;
        private int currentActionGUID = -1;

        public override void AddTriggers()
        {
            //"Whenever damage would be dealt to another hero target, you may destroy a constellation card in play to prevent that damage."
            AddTrigger((DealDamageAction dd) => IsHeroTarget(dd.Target) && !ListStarlights().Contains(dd.Target) && dd.Amount > 0,
                DestroyConstellationToPreventDamage,
                new TriggerType[]
                    {
                        TriggerType.DestroyCard,
                        TriggerType.WouldBeDealtDamage,
                        TriggerType.CancelAction
                    },
                timing: TriggerTiming.Before);
        }

        private IEnumerator DestroyConstellationToPreventDamage(DealDamageAction dd)
        {
            if(hasSaidNoToNightloreArmor == true && currentActionGUID == dd.InstanceIdentifier.GetHashCode() + dd.Target.GetHashCode())
            {
                if (!dd.IsPretend)
                {
                    hasSaidNoToNightloreArmor = false;
                    currentActionGUID = -1;
                }
                yield break;
            }
            if (!IsPropertyTrue(HasConstellationBeenDestroyed))
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
                    hasSaidNoToNightloreArmor = true;
                    currentActionGUID = dd.InstanceIdentifier.GetHashCode() + dd.Target.GetHashCode();
                    yield break;
                }

                //"...destroy a constellation in play..."
                List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
                IEnumerator destroyConstellation = GameController.SelectAndDestroyCard(HeroTurnTakerController, new LinqCardCriteria(IsConstellationInPlay, "constellation"), optional: false, storedResultsAction: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(destroyConstellation);
                }
                else
                {
                    GameController.ExhaustCoroutine(destroyConstellation);
                }
                if(DidDestroyCard(storedResults))
                {
                    SetCardProperty(HasConstellationBeenDestroyed, true);
                }
            }
            if (IsPropertyTrue(HasConstellationBeenDestroyed))
            {
               
                //"...to prevent that damage."
                IEnumerator preventDamage = CancelAction(dd, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(preventDamage);
                }
                else
                {
                    GameController.ExhaustCoroutine(preventDamage);
                }

                if (!dd.IsPretend)
                {
                    SetCardProperty(HasConstellationBeenDestroyed, false);
                    hasSaidNoToNightloreArmor = false;
                    currentActionGUID = -1;
                }

            }
            yield break;
        }

        private bool IsConstellationInPlay(Card c)
        {
            return IsConstellation(c) && c.IsInPlayAndHasGameText;
        }
    }
}

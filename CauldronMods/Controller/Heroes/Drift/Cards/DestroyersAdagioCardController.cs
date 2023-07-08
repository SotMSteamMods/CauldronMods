using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DestroyersAdagioCardController : DriftUtilityCardController
    {
        public DestroyersAdagioCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"));
        }

        public override IEnumerator Play()
        {
            //{DriftPast}
            if (base.IsTimeMatching(Past))
            {
                //You may play an ongoing card from your trash, or one player may play a card now.
                IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "You may play an ongoing from your trash", SelectionType.PlayCard, () => this.PlayTrashOngoingResponse(),
                     onlyDisplayIfTrue: base.GameController.GetAllCards().Any(c => IsOngoing(c) && TurnTaker.Trash.HasCard(c))),
                    new Function(base.HeroTurnTakerController, "One player may play a card now", SelectionType.PlayCard, () => this.PlayCardNowResponse())
                });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftR}
                coroutine = base.ShiftR();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }

            //{DriftFuture}
            if (base.IsTimeMatching(Future))
            {
                //{Drift} deals 1 target 2 radiant damage.
                IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), 2, DamageType.Radiant, 1, false, 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftLL}
                coroutine = base.ShiftLL();
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

        private IEnumerator PlayTrashOngoingResponse()
        {
            //You may play an ongoing from your trash
            IEnumerator coroutine = base.GameController.SelectAndPlayCard(base.HeroTurnTakerController, base.FindCardsWhere((Card c) => IsOngoing(c) && c.Location == base.TurnTaker.Trash), true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator PlayCardNowResponse()
        {
            //One player may play a card now
            IEnumerator coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

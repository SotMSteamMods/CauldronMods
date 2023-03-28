using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class BeneathTheFleshCardController : TheInfernalChoirUtilityCardController
    {
        public BeneathTheFleshCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddTrigger<DealDamageAction>(dda => IsHeroCharacterCard(dda.Target) && dda.Amount >= 3 && !dda.Target.IsIncapacitatedOrOutOfGame && dda.CardSource.Card != Card, dda => DiscardCardsAndOptionalDamageToPlayAndDestroyResponse(dda), new[] { TriggerType.DiscardCard, TriggerType.PutIntoPlay, TriggerType.DealDamage }, TriggerTiming.After);
        }

        private IEnumerator DiscardCardsAndOptionalDamageToPlayAndDestroyResponse(DealDamageAction dda)
        {
            IEnumerator coroutine;
            var httc = FindHeroTurnTakerController(dda.Target.Owner.ToHero());
            List<SelectNumberDecision> numberResult = new List<SelectNumberDecision>();
            coroutine = GameController.SelectNumber(httc, SelectionType.DiscardFromDeck, 1, 3, storedResults: numberResult, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            int selectedNumber = numberResult.First()?.SelectedNumber ?? 0;

            List<MoveCardAction> discardResult = new List<MoveCardAction>();
            var number = numberResult.First()?.SelectedNumber ?? 0;
            if (number > 0)
            {
                coroutine = DiscardCardsFromTopOfDeck(httc, number, storedResults: discardResult, showMessage: true, responsibleTurnTaker: TurnTaker);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (discardResult.Any())
            {
                int discards = GetNumberOfCardsMoved(discardResult);

                var fake = new DealDamageAction(GetCardSource(), new DamageSource(GameController, dda.Target), dda.Target, discards, DamageType.Infernal);
                var yesNo = new YesNoAmountDecision(GameController, httc, SelectionType.DealDamageSelf, discards, action: fake, cardSource: GetCardSource());

                coroutine = GameController.MakeDecisionAction(yesNo);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (yesNo.Answer == true)
                {
                    coroutine = GameController.DealDamageToSelf(httc, c => c == dda.Target, discards, DamageType.Infernal, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.SelectAndPlayCard(httc, discardResult.Where(r => r.WasCardMoved && !r.CardToMove.IsInPlay).Select(d => d.CardToMove), isPutIntoPlay: true, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = base.DestroyThisCardResponse(null);
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

            yield break;
        }
    }
}

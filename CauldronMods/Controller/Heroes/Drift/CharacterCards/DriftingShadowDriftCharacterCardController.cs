using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DriftingShadowDriftCharacterCardController : DriftSubCharacterCardController
    {
        public DriftingShadowDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //At the start of your next turn, shift {L} or {R}, then draw a card or use a power.
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(this.Card, nameof(this.StartOfTurnResponse), "At the start of your next turn, shift {ShiftL} or {ShiftR}, then draw a card or use a power", new TriggerType[] { TriggerType.ModifyTokens, TriggerType.DrawCard, TriggerType.UsePower }, this.Card);
            effect.UntilEndOfNextTurn(base.HeroTurnTaker);
            effect.TurnTakerCriteria.IsSpecificTurnTaker = base.HeroTurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            effect.CardSource = this.Card;
            effect.NumberOfUses = 1;
            effect.TurnIndexCriteria.GreaterThan = base.Game.TurnIndex;

            IEnumerator coroutine = base.AddStatusEffect(effect);
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

        public IEnumerator StartOfTurnResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect effect)
        {
            //...shift {DriftL} or {DriftR}...
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Shift {ShiftL}", SelectionType.RemoveTokens, () => base.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftR}", SelectionType.AddTokens, () => base.ShiftR())
            });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...then draw a card or use a power.
            coroutine = coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Draw Card", SelectionType.DrawCard, () => base.DrawCard()),
                    new Function(base.HeroTurnTakerController, "Use Power", SelectionType.UsePower, () => base.GameController.SelectAndUsePower(base.HeroTurnTakerController ,cardSource:base.GetCardSource()))
            });
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One hero may play a card now.
                        coroutine = base.SelectHeroToPlayCard(base.HeroTurnTakerController, heroCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitated));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //One hero may use a power now.
                        coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //Move up to 3 non-character hero cards from play to their owner' hands.
                        List<SelectCardsDecision> cardsDecision = new List<SelectCardsDecision>();
                        coroutine = base.GameController.SelectCardsAndStoreResults(base.HeroTurnTakerController, SelectionType.ReturnToHand, (Card c) => IsHero(c) && !c.IsCharacter && c.IsInPlayAndHasGameText, 3, cardsDecision, false, 0, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if(!DidSelectCards(cardsDecision))
                        {
                            yield break;
                        }
                        IEnumerable<Card> selectedCards = GetSelectedCards(cardsDecision);
                        foreach (Card card in selectedCards)
                        {
                            coroutine = base.GameController.MoveCard(base.TurnTakerController, card, card.Owner.ToHero().Hand, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}

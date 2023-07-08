using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class FutureDriftCharacterCardController : DualDriftSubCharacterCardController
    {
        public FutureDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Play an ongoing card. 
            List<PlayCardAction> playAction = new List<PlayCardAction>();
            IEnumerator coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, playAction, new LinqCardCriteria((Card c) => IsOngoing(c)));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(playAction.Any())
            {
                //At the end of your next turn, return it from play to your hand. 
                Card playedCard = playAction.FirstOrDefault().CardToPlay;
                OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(this.Card, nameof(this.EndOfTurnResponse), "At the end of your next turn, return " + playedCard.Title + " from play to your hand.", new TriggerType[] { TriggerType.MoveCard, TriggerType.AddTokensToPool }, this.Card);
                statusEffect.NumberOfUses = 1;
                statusEffect.BeforeOrAfter = BeforeOrAfter.Before;
                statusEffect.TurnPhaseCriteria.Phase = Phase.End;
                statusEffect.TurnPhaseCriteria.TurnTaker = base.TurnTaker;
                statusEffect.TurnIndexCriteria.GreaterThan = base.Game.TurnIndex;
                statusEffect.CardMovedExpiryCriteria.Card = playedCard;

                coroutine = base.AddStatusEffect(statusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Shift {RR}.
            coroutine = base.ShiftRR();
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

        public IEnumerator EndOfTurnResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect effect)
        {
            //...return it from play to your hand.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, effect.CardMovedExpiryCriteria.Card, base.TurnTaker.ToHero().Hand, cardSource: base.GetCardSource());
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
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && !tt.IsIncapacitated), cardSource: base.GetCardSource());
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
                        //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it and that hero uses that power.
                        List<SelectLocationDecision> selectedDeck = new List<SelectLocationDecision>();
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location loc) => loc.IsHero && loc.IsDeck && !loc.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && !loc.OwnerTurnTaker.IsIncapacitated, storedResults: selectedDeck, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (selectedDeck.Any())
                        {
                            //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it...
                            List<Card> playResult = new List<Card>();
                            coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, selectedDeck.FirstOrDefault().SelectedLocation.Location, true, false, false, new LinqCardCriteria((Card c) => c.HasPowers), null, 1, shuffleSourceAfterwards: false, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, storedPlayResults: playResult);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            //...and that hero uses that power.
                            if (playResult.Any())
                            {
                                for (int i = 0; i < playResult.FirstOrDefault().NumberOfPowers; i++)
                                {
                                    coroutine = UsePowerOnOtherCard(playResult.FirstOrDefault(), i);
                                    if (base.UseUnityCoroutines)
                                    {
                                        yield return base.GameController.StartCoroutine(coroutine);
                                    }
                                    else
                                    {
                                        base.GameController.ExhaustCoroutine(coroutine);
                                    }
                                }
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        //One target regains 2 HP.
                        coroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, 2, cardSource: base.GetCardSource());
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
            }
            yield break;
        }
    }
}

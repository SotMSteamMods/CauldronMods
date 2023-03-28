using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DriftCharacterCardController : DriftSubCharacterCardController
    {

        public DriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {

            int hpNumeral = base.GetPowerNumeral(0, 1);

            //Shift {DriftLL}, {DriftL}, {DriftR}, {DriftRR}. 
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Shift {ShiftLL}", SelectionType.RemoveTokens, () => base.ShiftLL()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftL}", SelectionType.RemoveTokens, () => base.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftR}", SelectionType.AddTokens, () => base.ShiftR()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftRR}", SelectionType.AddTokens, () => base.ShiftRR())
            }, associatedCards: GetShiftTrack().ToEnumerable());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Drift regains 1 HP.
            coroutine = base.GameController.GainHP(this.Card, hpNumeral, cardSource: GetCardSource());
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
                case 1:
                    {
                        //One target regains 1 HP and deals another target 1 radiant damage.
                        List<SelectTargetDecision> targetDecision = new List<SelectTargetDecision>();
                        coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, base.FindCardsWhere((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), storedResults: targetDecision, selectionType: SelectionType.GainHP, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        //One target regains 1 HP
                        Card hpGainer = targetDecision.FirstOrDefault().SelectedCard;
                        coroutine = base.GameController.GainHP(hpGainer, 1);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        //deals another target 1 radiant damage
                        coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, hpGainer), 1, DamageType.Radiant, 1, false, 1, cardSource: base.GetCardSource());
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
                        IEnumerable<TurnTaker> turnTakersWithOneShots = base.FindTurnTakersWhere((TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && !tt.IsIncapacitated && IsHero(tt) && base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location.OwnerTurnTaker == tt && c.Location.IsHand && c.IsOneShot && c.Location.IsHero)).Any());
                        List<SelectTurnTakerDecision> turnTakerDecisions = new List<SelectTurnTakerDecision>();
                        if (turnTakersWithOneShots.Any())
                        {
                            //One player may discard a one-shot. If they do, they may draw 2 cards.
                            coroutine = base.GameController.SelectHeroTurnTaker(base.HeroTurnTakerController, SelectionType.DiscardAndDrawCard, false, false, turnTakerDecisions,
                            new LinqTurnTakerCriteria((TurnTaker tt) => turnTakersWithOneShots.Contains(tt)), cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }

                        if (turnTakerDecisions.Any())
                        {
                            //One player may discard a one-shot.
                            List<DiscardCardAction> discardCard = new List<DiscardCardAction>();
                            coroutine = base.GameController.SelectAndDiscardCard(base.FindHeroTurnTakerController(turnTakerDecisions.FirstOrDefault().SelectedTurnTaker.ToHero()), true, (Card c) => c.IsOneShot, discardCard, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            if (discardCard.Any())
                            {
                                //If they do, they may draw 2 cards.
                                coroutine = base.DrawCards(discardCard.FirstOrDefault().HeroTurnTakerController, 2, true);
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
                        break;
                    }
            }
            yield break;
        }
    }
}

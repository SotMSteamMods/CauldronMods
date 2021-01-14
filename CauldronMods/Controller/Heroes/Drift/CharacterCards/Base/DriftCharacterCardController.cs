using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DriftCharacterCardController : HeroCharacterCardController
    {
        public DriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected const string HasShifted = "HasShifted";
        protected const string ShiftTrack = "ShiftTrack";

        private int totalShifts = 0;
        public int TotalShifts { get => totalShifts; set => totalShifts = value; }

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
                        //One player may discard a one-shot. If they do, they may draw 2 cards.
                        List<DiscardCardAction> discardCard = new List<DiscardCardAction>();
                        coroutine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, true, (Card c) => c.IsOneShot, discardCard, cardSource: base.GetCardSource());
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
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int hpNumeral = base.GetPowerNumeral(0, 1);

            //Shift {DriftLL}, {DriftL}, {DriftR}, {DriftRR}. 
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Drift Left Twice", SelectionType.RemoveTokens, () => this.ShiftLL()),
                    new Function(base.HeroTurnTakerController, "Drift Left", SelectionType.RemoveTokens, () => this.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Drift Right", SelectionType.AddTokens, () => this.ShiftR()),
                    new Function(base.HeroTurnTakerController, "Drift Right Twice", SelectionType.AddTokens, () => this.ShiftRR())
            });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Drift regains 1 HP.
            coroutine = base.GameController.GainHP(this.Card, hpNumeral);
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

        public int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        public TokenPool GetShiftPool()
        {
            return this.GetShiftTrack().FindTokenPool("ShiftPool");
        }

        public Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText).FirstOrDefault();
        }

        public IEnumerator ShiftL()
        {
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() > 1)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);
                IEnumerator coroutine = base.GameController.RemoveTokensFromPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Switch to the new card
                coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(ShiftTrack + this.CurrentShiftPosition(), false));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                totalShifts++;
            }
            else
            {
                IEnumerator coroutine = base.GameController.SendMessageAction("Drift has reached the end of the Shift Track", Priority.Medium, base.GetCardSource());
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

        public IEnumerator ShiftLL()
        {
            IEnumerator coroutine = this.ShiftL();
            IEnumerator coroutine2 = this.ShiftL();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public IEnumerator ShiftR()
        {
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() < 4)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);
                IEnumerator coroutine = base.GameController.AddTokensToPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Switch to the new card
                coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(ShiftTrack + this.CurrentShiftPosition(), false));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                totalShifts++;
            }
            else
            {
                IEnumerator coroutine = base.GameController.SendMessageAction("Drift has reached the end of the Shift Track", Priority.Medium, base.GetCardSource());
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

        public IEnumerator ShiftRR()
        {
            IEnumerator coroutine = this.ShiftR();
            IEnumerator coroutine2 = this.ShiftR();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}

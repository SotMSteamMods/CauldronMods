using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class WastelandRoninCricketCharacterCardController : HeroCharacterCardController
    {
        public WastelandRoninCricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine = null;
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
                        //Select a power on a card in play. The next time a hero uses it. They may immediately use it again.
                        List<SelectCardDecision> decision = new List<SelectCardDecision>();
                        //Select a power on a card in play. 
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.UsePowerTwice, new LinqCardCriteria((Card c) => c.HasPowers && c.IsInPlayAndHasGameText), decision, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectCard(decision))
                        {
                            int powerIndex = 0;
                            string powerString = "power on ";
                            Card selectedCard = decision.FirstOrDefault().SelectedCard;
                            if (selectedCard.NumberOfPowers > 1)
                            {
                                List<SelectNumberDecision> number = new List<SelectNumberDecision>();
                                coroutine = base.GameController.SelectNumber(base.HeroTurnTakerController, SelectionType.UsePowerTwice, 0, selectedCard.NumberOfPowers - 1, storedResults: number, cardSource: base.GetCardSource());
                                if (base.UseUnityCoroutines)
                                {
                                    yield return base.GameController.StartCoroutine(coroutine);
                                }
                                else
                                {
                                    base.GameController.ExhaustCoroutine(coroutine);
                                }
                                if (number != null && number.FirstOrDefault().SelectedNumber != null)
                                {
                                    powerIndex = number.FirstOrDefault().SelectedNumber ?? default;
                                    if (powerIndex == 0)
                                    {
                                        powerString = "first power on ";
                                    }
                                    else if (powerIndex == 1)
                                    {
                                        powerString = "second power on ";
                                    }
                                    else if (powerIndex == 2)
                                    {
                                        powerString = "third power on ";
                                    }
                                    else
                                    {
                                        powerString = "nth power on ";
                                    }
                                }
                            }
                            //The next time a hero uses it. They may immediately use it again.
                            OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(base.CharacterCard, "FakeStatusEffect", "The " + powerString + selectedCard.Title + " will be used twice next time it is used by a hero.", new TriggerType[] { TriggerType.UsePower }, base.CharacterCard);
                            statusEffect.CardDestroyedExpiryCriteria.Card = selectedCard;
                            statusEffect.NumberOfUses = powerIndex + 1; //cheat to store the powerIndex in the statusEffect
                            statusEffect.CanEffectStack = true;
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
                        break;
                    }
                case 2:
                    {
                        List<SelectLocationDecision> storedLocation = new List<SelectLocationDecision>();
                        //Move the bottom card of a deck to the top.
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => true, storedLocation, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectLocation(storedLocation))
                        {
                            Location selectedDeck = GetSelectedLocation(storedLocation);
                            coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedDeck.BottomCard, selectedDeck, showMessage: true, cardSource: base.GetCardSource());
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

        public override void AddSideTriggers()
        {
            if (base.CharacterCard.IsFlipped)
            {
                //This looks for when a power is used that has been setup to be duplicated as per Incap 2
                base.AddSideTrigger(base.AddTrigger<UsePowerAction>((UsePowerAction action) => base.Game.StatusEffects.Where((StatusEffect effect) => effect.CardSource == base.CharacterCard && effect.CardDestroyedExpiryCriteria.Card == action.Power.CardSource.Card).Any(), this.UsePowerAgainResponse, TriggerType.UsePower, TriggerTiming.After));
            }
        }

        public override bool AskIfCardMayPreventAction<UsePowerAction>(TurnTakerController ttc, CardController preventer)
        {
            return !base.Card.IsIncapacitated && base.AskIfCardMayPreventAction<UsePowerAction>(ttc, preventer);
        }

        private IEnumerator UsePowerAgainResponse(UsePowerAction action)
        {
            StatusEffect[] statusEffects = base.Game.StatusEffects.Where((StatusEffect effect) => effect.CardSource == base.CharacterCard && effect.CardDestroyedExpiryCriteria.Card == action.Power.CardSource.Card).ToArray();
            if (statusEffects.Any())
            {
                //If a power has been setup more than once, use it more than once
                foreach (StatusEffect statusEffect in statusEffects)
                {
                    //remove the fake status effect
                    base.GameController.StatusEffectManager.RemoveStatusEffect(statusEffect);

                        IEnumerator coroutine = UsePowerOnOtherCard(action.Power.CardSource.Card, action.Power.Index);
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
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Increase damage dealt by {Cricket} during your next turn by 1. {Cricket} may deal 1 target 1 sonic damage.
            int increaseNumeral = GetPowerNumeral(0, 1);
            int targetNumeral = GetPowerNumeral(1, 1);
            int damageNumeral = GetPowerNumeral(2, 1);

            //Increase damage dealt by {Cricket} during your next turn by 1.
            OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(base.Card, "IncreaseDamageResponse" + increaseNumeral, "Increase damage dealt by {Cricket} during your next turn by 1", new TriggerType[] { TriggerType.IncreaseDamage }, base.Card);
            statusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
            statusEffect.TurnIndexCriteria.GreaterThan = Game.TurnIndex;
            statusEffect.TurnPhaseCriteria.TurnTaker = base.TurnTaker;
            statusEffect.NumberOfUses = 1;
            statusEffect.CanEffectStack = true;
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);

            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cricket} may deal 1 target 1 sonic damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Sonic, targetNumeral, false, 0, cardSource: base.GetCardSource());
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

        private IEnumerator IncreaseDamageResponse(int increaseNumeral)
        {
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(increaseNumeral);
            statusEffect.UntilThisTurnIsOver(Game);
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
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

        public IEnumerator IncreaseDamageResponse1(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            IEnumerator coroutine = this.IncreaseDamageResponse(1);
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

        public IEnumerator IncreaseDamageResponse2(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            IEnumerator coroutine = this.IncreaseDamageResponse(2);
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
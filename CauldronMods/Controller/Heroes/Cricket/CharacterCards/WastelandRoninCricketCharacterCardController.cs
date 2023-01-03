using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class WastelandRoninCricketCharacterCardController : CricketCharacterSubCardController
    {
        public WastelandRoninCricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const string Incap2PropKey = "WastelandRoninCricketIncap2";

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

                        bool CardsWithPowers(Card c)
                        {
                            if (c.IsInPlayAndHasGameText)
                            {
                                if (c.HasPowers)
                                    return true;
                                return GameController.GetAllPowersForCardController(FindCardController(c)).Any();
                            }
                            return false;
                        }

                        //Select a power on a card in play. 
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.UsePowerTwice, new LinqCardCriteria(CardsWithPowers), decision, false, cardSource: base.GetCardSource());
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
                            Card selectedCard = decision.FirstOrDefault().SelectedCard;
                            var cc = FindCardController(selectedCard);
                            var httc = cc.HeroTurnTakerController;
                            Power selectedPower = null;
                            int powerIndex = 0;
                            var powers = GameController.GetAllPowersForCardController(cc, httc).ToList();
                            int numberOfPowers = powers.Count;
                            if (powers.Count > 1)
                            {
                                UsePowerDecision selectPower = new UsePowerDecision(GameController, DecisionMaker, powers, false, GetCardSource());
                                IEnumerator coroutine3 = GameController.MakeDecisionAction(selectPower);
                                if (UseUnityCoroutines)
                                {
                                    yield return GameController.StartCoroutine(coroutine3);
                                }
                                else
                                {
                                    GameController.ExhaustCoroutine(coroutine3);
                                }
                                if (selectPower.Completed && selectPower.SelectedPower != null)
                                {
                                    selectedPower = selectPower.SelectedPower;
                                    powerIndex = powers.IndexOf(selectedPower);
                                }
                            }
                            else
                            {
                                selectedPower = powers.FirstOrDefault();
                            }

                            if (selectedPower != null)
                            {
                                selectedPower.CardController.SetCardPropertyToTrueIfRealAction(Incap2PropKey);
                                string msg = "The power on ";
                                if (numberOfPowers > 1)
                                {
                                    msg = $"The {(powerIndex + 1).ToOrdinalString()} power on ";
                                }
                                msg += $"{selectedPower.CardController.Card.Title} will be used twice next time it is used by a hero.";

                                //The next time a hero uses it. They may immediately use it again.
                                OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), msg, new TriggerType[] { TriggerType.UsePower }, base.CharacterCard);
                                statusEffect.CardDestroyedExpiryCriteria.Card = selectedPower.CardController.Card;
                                statusEffect.NumberOfUses = 1 + powerIndex; //cheat to store the power index in the status effect
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
                base.AddSideTrigger(base.AddTrigger<UsePowerAction>(action => action.Power.CardController.GetCardPropertyJournalEntryBoolean(Incap2PropKey) == true, UsePowerAgainResponse, TriggerType.UsePower, TriggerTiming.After));
            }
        }

        public override bool AskIfCardMayPreventAction<UsePowerAction>(TurnTakerController ttc, CardController preventer)
        {
            return !base.Card.IsIncapacitated && base.AskIfCardMayPreventAction<UsePowerAction>(ttc, preventer);
        }

        private IEnumerator UsePowerAgainResponse(UsePowerAction action)
        {
            var power = action.Power;
            var keyCard = power.CardController.Card;

            //we know keyCard has it's card prop set to true
            //we use the number of status effect instances as the number of uses
            //we use the NumberOfUses in the status effect as the powerIndex, which has to match the used power above

            var statusEffects = Game.StatusEffects
                                    .OfType<OnPhaseChangeStatusEffect>()
                                    .Where(effect => (effect.CardSource == CharacterCard || (Game.IsOblivAeonMode && effect.CardSource.Location == CharacterCard.Location)) &&
                                            effect.CardDestroyedExpiryCriteria.Card == keyCard)
                                    .ToArray();

            //we have to deduce the correct powerIndex from the available powers on the cardController to account for granted powers.
            var powers = GameController.GetAllPowersForCardController(power.CardController, action.HeroUsingPower).ToList();
            int powerIndex = powers.FindIndex(p => p.Title == power.Title && p.Index == power.Index);

            //if there are no status effects, we just reset the flag and exit
            //if there are status effects, we see if all of them are for the current power index
            // if so, we can reset the flag, otherwise filter the status effect list smaller, and don't reset
            bool resetFlag = true;
            if (statusEffects.Any() && statusEffects.Count(effect => effect.NumberOfUses == (1 + powerIndex)) != statusEffects.Length)
            {
                resetFlag = false;
                statusEffects = statusEffects.Where(effect => effect.NumberOfUses == (1 + powerIndex)).ToArray();
            }

            //defer bookkeeping til real action
            if (IsRealAction(action))
            {
                //we clear the status effects and possibly the flag before using the power again to prevent
                //the trigger from firing again
                foreach (var effect in statusEffects)
                {
                    base.GameController.StatusEffectManager.RemoveStatusEffect(effect);
                }
                if (resetFlag)
                {
                    Journal.RecordCardProperties(keyCard, Incap2PropKey, (bool?)null);
                }
            }

            //last check that we are actually going to trigger the power again
            if (statusEffects.Any())
            {
                var coroutine = GameController.SendMessageAction($"{CharacterCard.Title} allows {action.Power.Title} to be used again!", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //If a power has been setup more than once, use it more than once
                foreach (var effect in statusEffects)
                {
                    coroutine = GameController.UsePower(power.CardController.Card, powerIndex, true, action.CardSource);
                    //coroutine = UsePowerOnOtherCard(power.CardController.Card, powerIndex);
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
            OnPhaseChangeStatusEffect statusEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(IncreaseDamageResponse), "Increase damage dealt by {Cricket} during your next turn by 1", new TriggerType[] { TriggerType.IncreaseDamage }, base.Card);
            statusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
            statusEffect.TurnIndexCriteria.GreaterThan = Game.TurnIndex;
            statusEffect.TurnPhaseCriteria.TurnTaker = base.TurnTaker;
            statusEffect.NumberOfUses = 1;
            statusEffect.CanEffectStack = true;
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            statusEffect.SetPowerNumeralsArray(new int[] { increaseNumeral });

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

        public IEnumerator IncreaseDamageResponse(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            int increaseNumeral = sourceEffect.PowerNumeralsToChange[0];

            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(increaseNumeral);
            statusEffect.UntilThisTurnIsOver(Game);
            statusEffect.UntilTargetLeavesPlay(base.CharacterCard);
            statusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;

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
    }
}
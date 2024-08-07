﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Vanish
{
    public class TombOfThievesVanishCharacterCardController : VanishSubCharacterCardController
    {

        public TombOfThievesVanishCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"The next time {Vanish} is dealt damage, draw or play a card."
            var effect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(DrawOrPlayResponse), $"The next time {CharacterCard.Title} is dealt damage, draw or play a card.", new[] { TriggerType.DrawCard, TriggerType.PlayCard }, TurnTaker, CharacterCard);
            // if NumberOfUses is set, stacking the effect will automatically increase numberofuses instead of having all the responses happen to the same damage
            effect.TargetCriteria.IsSpecificCard = CharacterCard;
            effect.CardSource = CharacterCard;
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.DamageAmountCriteria.GreaterThan = 0;
            effect.UntilTargetLeavesPlay(CharacterCard);

            var coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
        }

        public IEnumerator DrawOrPlayResponse(DealDamageAction _1, TurnTaker _2, StatusEffect _3, int[] _4 = null)
        {
            //avoid having the play-a-card effect indirectly trigger the same status effect
            if (_3.CardMovedExpiryCriteria.Card == null)
            {
                IEnumerator coroutine;
                _3.CardMovedExpiryCriteria.Card = this.Card;
                if (_2 != null && IsHero(_2))
                {
                    coroutine = DrawACardOrPlayACard(FindHeroTurnTakerController(_2.ToHero()), false);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                coroutine = GameController.ExpireStatusEffect(_3, null);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player may use a power now.",
             * "Destroy 1 ongoing card.",
             * "Select 1 environment target in play. Destroy it at the start of your next turn. Until then, it is immune to damage."
             */

            switch (index)
            {
                case 0:
                    {
                        //"One player may use a power now."
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: base.GetCardSource());
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
                        //"Destroy 1 ongoing card.",
                        var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => IsOngoing(c), "ongoing"), false, cardSource: GetCardSource());
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
                        //"Select 1 environment target in play. Destroy it at the start of your next turn. Until then, it is immune to damage."
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        var coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DestroyCard, new LinqCardCriteria(c => c.IsEnvironmentTarget, "environment target", false), storedResults, false,
                                            allowAutoDecide: true,
                                            cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectCard(storedResults))
                        {
                            var card = GetSelectedCard(storedResults);
                            if (IsRealAction())
                            {
                                Journal.RecordCardProperties(card, "MarkedForDestruction", true);
                            }
                            var immune = new ImmuneToDamageStatusEffect();
                            immune.CardSource = CharacterCard;
                            immune.TargetCriteria.IsSpecificCard = card;
                            immune.UntilCardLeavesPlay(card);

                            coroutine = base.AddStatusEffect(immune, true);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            var effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DestroyMarkedTarget), $"{card.Title} will be destroyed at the start of {CharacterCard.Title}'s next turn.", new[] { TriggerType.DestroyCard }, CharacterCard);
                            effect.TurnPhaseCriteria.TurnTaker = TurnTaker;
                            effect.TurnPhaseCriteria.Phase = Phase.Start;
                            effect.NumberOfUses = 1;
                            effect.CanEffectStack = true;
                            effect.CardSource = CharacterCard;
                            effect.UntilTargetLeavesPlay(card);

                            coroutine = base.AddStatusEffect(effect, true);
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

        public IEnumerator DestroyMarkedTarget(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            //the target to be destroyed will be in the target leaves play criteria, and have the marked card prop.
            var card = sourceEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards?.First() ?? sourceEffect.TargetLeavesPlayExpiryCriteria.Card;
            if (card != null && Journal.GetCardPropertiesBoolean(card, "MarkedForDestruction") == true)
            {
                if (IsRealAction())
                {
                    Journal.RecordCardProperties(card, "MarkedForDestruction", (bool?)null);
                }

                if (card.IsInPlay)
                {
                    var coroutine = GameController.DestroyCard(DecisionMaker, card,
                                        actionSource: action,
                                        cardSource: GetCardSource(sourceEffect));
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
    }
}
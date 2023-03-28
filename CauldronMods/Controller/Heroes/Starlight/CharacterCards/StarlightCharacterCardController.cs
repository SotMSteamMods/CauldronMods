using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Starlight
{
    public class StarlightCharacterCardController : StarlightSubCharacterCardController
    {

        private static readonly string PreventDamageViaIncapPropertyKey = "StarlightIncapPreventDamageToOrByLowest";

        private bool? PreventDamageViaIncap
        {
            get;
            set;
        }

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                bool? incapIsActive = base.GetCardPropertyJournalEntryBoolean(PreventDamageViaIncapPropertyKey);
                return !incapIsActive.HasValue || !incapIsActive.Value;
            }
        }

        public StarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
            AllowFastCoroutinesDuringPretend = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Draw a card, or play a Constellation from your trash
            IEnumerator coroutine = DrawACardOrPlayConstellationFromTrash();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"Until the start of your next turn, prevent all damage that would be dealt to or by the target with the lowest HP.",
                        //Secretly set a property on this card that is checked by triggers, in order to apply the effect.
                        base.AddCardPropertyJournalEntry(PreventDamageViaIncapPropertyKey, true);
                        //This status effect displays UI text but provides no behavior on its own.
                        //The method name is fake. Another trigger clears the above property when this status effect is removed.
                        OnPhaseChangeStatusEffect displayEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), "The target with the lowest HP is immune to damage and cannot deal damage.", new TriggerType[] { }, base.Card);
                        displayEffect.UntilStartOfNextTurn(base.TurnTaker);
                        displayEffect.CanEffectStack = true;
                        IEnumerator coroutine = base.AddStatusEffect(displayEffect);

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
                        //"1 player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        IEnumerator coroutine3 = GameController.SelectAndGainHP(HeroTurnTakerController, 2, optional: false, (Card c) => c.IsInPlay && IsHeroTarget(c), 1, null, allowAutoDecide: false, null, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }

            yield break;
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (base.Card.IsFlipped)
            {
                //Triggers for the damage prevention incap ability.
                //Uses Triggers, because StatusEffects have no parallel to AllowFastCoroutinesDuringPretend.
                base.AddSideTrigger(base.AddTrigger<DealDamageAction>(PreventDamageViaIncapCriteria, PreventDamageViaIncapResponse, TriggerType.CancelAction, TriggerTiming.Before));
                Func<ExpireStatusEffectAction, bool> clearIncapEffectCriteria = (ExpireStatusEffectAction action) =>
                {
                    if (action.StatusEffect is OnPhaseChangeStatusEffect effect)
                    {
                        return effect.CardWithMethod == CardWithoutReplacements && effect.MethodToExecute == nameof(DoNothing);
                    }
                    return false;
                };
                base.AddSideTrigger(base.AddTrigger<ExpireStatusEffectAction>(clearIncapEffectCriteria, ClearPreventDamageViaIncapResponse, TriggerType.Other, TriggerTiming.After));
            }
        }

        private IEnumerator DrawACardOrPlayConstellationFromTrash()
        {
            List<Function> list = new List<Function>();
            string forceDrawCardEnder = " cannot play a constellations from their trash, so they must draw a card.";
            string forcePlayConstellationEnder = " cannot draw any cards, so they must play a constellation from their trash.";
            string forceDoNothingEnder = " cannot draw nor play any cards, so the power has no effect.";
            string heroName = ((TurnTaker == null) ? Card.Title : TurnTaker.Name);

            list.Add(new Function(HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker, false), HeroTurnTakerController != null && CanDrawCards(HeroTurnTakerController), heroName + forceDrawCardEnder));

            list.Add(new Function(HeroTurnTakerController, "Play a constellation from your trash", SelectionType.PlayCard, () => SelectAndPlayConstellationFromTrash(HeroTurnTakerController), HeroTurnTakerController != null && GetPlayableConstellationsInTrash().Count() > 0, heroName + forcePlayConstellationEnder));

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, HeroTurnTakerController, list, false, null, heroName + forceDoNothingEnder, null, GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator SelectAndPlayConstellationFromTrash(HeroTurnTakerController hero)
        {
            IEnumerator coroutine = GameController.SelectAndPlayCard(hero, GetPlayableConstellationsInTrash(), isPutIntoPlay: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerable<Card> GetPlayableConstellationsInTrash()
        {
            return HeroTurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), false, null, false, true) == CanPlayCardResult.CanPlay);
        }

        private bool PreventDamageViaIncapCriteria(DealDamageAction dealDamage)
        {
            bool? incapIsActive = base.GetCardPropertyJournalEntryBoolean(PreventDamageViaIncapPropertyKey);
            return incapIsActive.HasValue && incapIsActive.Value;
        }

        private IEnumerator PreventDamageViaIncapResponse(DealDamageAction dealDamage)
        {
            if (base.GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();

                //Is the target of the damage the lowest HP target?
                IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.Target, highest: false, (Card card) => GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //If not, is the source of the damage the lowest HP target?
                if (!storedResults.First() && dealDamage.DamageSource.IsTarget)
                {
                    IEnumerator coroutine2 = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.DamageSource.Card, highest: false, (Card card) => GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine2);
                    }
                }

                //If we answered yes to either question, prevent the damage.
                this.PreventDamageViaIncap = storedResults.Contains(true);
            }

            if (this.PreventDamageViaIncap != null && this.PreventDamageViaIncap.Value)
            {
                IEnumerator coroutine3 = base.CancelAction(dealDamage, showOutput: true, cancelFutureRelatedDecisions: true, null, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine3);
                }
            }

            if (!base.GameController.PretendMode)
            {
                this.PreventDamageViaIncap = null;
            }
            yield break;
        }

        protected IEnumerator ClearPreventDamageViaIncapResponse(ExpireStatusEffectAction expireAction)
        {
            base.AddCardPropertyJournalEntry(PreventDamageViaIncapPropertyKey, (bool?)null);
            yield break;
        }
    }
}
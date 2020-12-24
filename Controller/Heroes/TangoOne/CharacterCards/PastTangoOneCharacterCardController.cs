using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PastTangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageToDeal = 3;
        private const int Incapacitate1CardsToDraw = 2;
        private const int Incapacitate2CardsToPlay = 2;
        private const int Incapacitate3CardsToDestroy = 1;

        public PastTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Select a target, at the start of your next turn,
            // {TangoOne} deals that target 3 projectile damage.
            //==============================================================

            IEnumerable<Card> targets = GameController.FindTargetsInPlay();
            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            IEnumerator coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, targets, storedResults,
                    damageSource: CharacterCard, damageAmount: card => 3, damageType: DamageType.Projectile,
                    selectionType: SelectionType.SelectTargetNoDamage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card target = storedResults.FirstOrDefault()?.SelectedCard;
            if (target != null)
            {
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(this.CardWithoutReplacements,
                    nameof(StartOfTurnDealDamageResponse),
                    $"{base.Card.Title} will deal {PowerDamageToDeal} projectile damage to {target.Title} at the start of her next turn",
                    new[] { TriggerType.DealDamage }, this.Card);
                effect.UntilEndOfNextTurn(base.HeroTurnTaker);
                effect.TurnTakerCriteria.IsSpecificTurnTaker = base.HeroTurnTaker;
                effect.UntilTargetLeavesPlay(target);
                effect.TurnPhaseCriteria.Phase = Phase.Start;
                effect.BeforeOrAfter = BeforeOrAfter.After;
                effect.CanEffectStack = true;
                effect.CardSource = this.Card;

                IEnumerator addStatusEffectRoutine = base.GameController.AddStatusEffect(effect, true, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(addStatusEffectRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(addStatusEffectRoutine);
                }
            }
        }

        public IEnumerator StartOfTurnDealDamageResponse(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            var target = sourceEffect.TargetLeavesPlayExpiryCriteria.IsOneOfTheseCards.FirstOrDefault();

            int powerNumeral = GetPowerNumeral(0, PowerDamageToDeal);

            if (!CharacterCard.IsIncapacitatedOrOutOfGame && target.IsTarget && target.IsInPlayAndNotUnderCard)
            {
                if (GameController.IsCardVisibleToCardSource(target, GetCardSource()) != true)
                {
                    var coroutine = GameController.SendMessageAction($"{CharacterCard.Title} misses! {target.Title} is no longer visible!", Priority.Medium, GetCardSource(), new[] { target });
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    IEnumerator dealDamageRoutine = this.DealDamage(CharacterCard, target, powerNumeral, DamageType.Projectile);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(dealDamageRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(dealDamageRoutine);
                    }
                }
            }
            GameController.StatusEffectManager.RemoveStatusEffect(sourceEffect);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            List<SelectTurnTakerDecision> result;
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may draw 2 cards.
                    //==============================================================

                    result = new List<SelectTurnTakerDecision>();
                    coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.DrawCard, false, true, result, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (DidSelectTurnTaker(result))
                    {
                        var tt = GetSelectedTurnTaker(result);
                        var effect = MakeTurntakerStartPhaseEffect(nameof(StartOfTurnDrawCardsResponse), tt, TriggerType.DrawCard,
                            $"{tt.NameRespectingVariant} may draw {Incapacitate1CardsToDraw} cards at the start of {this.Card.Title}'s next turn");

                        coroutine = AddStatusEffect(effect);
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

                case 1:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may play 2 cards.
                    //==============================================================

                    result = new List<SelectTurnTakerDecision>();
                    coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.DrawCard, false, true, result, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (DidSelectTurnTaker(result))
                    {
                        var tt = GetSelectedTurnTaker(result);
                        var effect = MakeTurntakerStartPhaseEffect(nameof(StartOfTurnPlayCardsResponse), tt, TriggerType.PlayCard,
                            $"{tt.NameRespectingVariant} may play {Incapacitate2CardsToPlay} cards at the start of {this.Card.Title}'s next turn");

                        coroutine = AddStatusEffect(effect);
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

                case 2:

                    //==============================================================
                    // Destroy an environment card.
                    //==============================================================

                    coroutine = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, new LinqCardCriteria(c => c.IsEnvironment, "environment"), Incapacitate3CardsToDestroy,
                        requiredDecisions: 0, cardSource: GetCardSource());
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

        public IEnumerator StartOfTurnDrawCardsResponse(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            var tt = GetCardPropertyJournalEntryTurnTaker(IncapTurnTakerKey);
            if (tt != null && !tt.IsIncapacitatedOrOutOfGame)
            {
                IEnumerator drawCardRoutine = base.DrawCards(FindHeroTurnTakerController(tt.ToHero()), Incapacitate1CardsToDraw, true, allowAutoDraw: false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(drawCardRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(drawCardRoutine);
                }
                //clear the last card prop
                GameController.AddCardPropertyJournalEntry(Card, IncapTurnTakerKey, (TurnTaker)null);
            }

            GameController.StatusEffectManager.RemoveStatusEffect(sourceEffect);
        }

        private static readonly string IncapTurnTakerKey = "PastTangoIncapTurnTaker";

        public IEnumerator StartOfTurnPlayCardsResponse(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            var tt = GetCardPropertyJournalEntryTurnTaker(IncapTurnTakerKey);
            if (tt != null && !tt.IsIncapacitatedOrOutOfGame)
            {
                var httc = FindHeroTurnTakerController(tt.ToHero());

                IEnumerator playCardsRoutine = base.GameController.SelectAndPlayCardsFromHand(httc, 2, true, requiredCards: 0, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playCardsRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playCardsRoutine);
                }


                //clear the last card prop
                GameController.AddCardPropertyJournalEntry(Card, IncapTurnTakerKey, (TurnTaker)null);
            }
            GameController.StatusEffectManager.RemoveStatusEffect(sourceEffect);
        }

        private OnPhaseChangeStatusEffect MakeTurntakerStartPhaseEffect(string methodToCall, TurnTaker target, TriggerType triggerType, string description)
        {
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(this.CardWithoutReplacements,
                methodToCall,
                description, new[] { triggerType }, this.Card);

            GameController.AddCardPropertyJournalEntry(Card, IncapTurnTakerKey, target);

            effect.UntilEndOfNextTurn(base.HeroTurnTaker);
            effect.TurnTakerCriteria.IsSpecificTurnTaker = base.HeroTurnTaker;
            effect.TurnPhaseCriteria.Phase = Phase.Start;
            effect.BeforeOrAfter = BeforeOrAfter.After;
            effect.CanEffectStack = true;
            effect.UntilCardLeavesPlay(base.Card);

            return effect;
        }
    }
}

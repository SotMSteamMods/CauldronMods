using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.Impact
{
    public class RenegadeImpactCharacterCardController : ImpactSubCharacterCardController
    {
        private readonly string renegadeKey = "RenegadeImpactIrreducibleTriggerKey";
        public RenegadeImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy 1 of your ongoing cards. If you do, play a card and draw a card."
            int numToDestroy = GetPowerNumeral(0, 1);
            var storedDestroy = new List<DestroyCardAction> { };
            IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.Owner == this.TurnTaker && IsOngoing(c) && !c.IsBeingDestroyed, "ongoing"),
                                                                                                        numToDestroy, false, numToDestroy, storedResultsAction: storedDestroy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (GetNumberOfCardsDestroyed(storedDestroy) >= numToDestroy)
            {
                coroutine = GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DrawCard();
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
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"Select a target. Damage dealt by that target is irreducible during its next turn.",
                        var selectTarget = new SelectCardDecision(GameController, DecisionMaker, SelectionType.SelectTargetFriendly, GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget, visibleToCard: GetCardSource()), cardSource: GetCardSource());
                        coroutine = GameController.SelectCardAndDoAction(selectTarget, MakeIrreducibleNextTurn);
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
                case 1:
                    {
                        //"One hero may use a power now.",
                        coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
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
                        //"The environment deals 1 target 2 projectile damage."
                        coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, FindEnvironment().TurnTaker), 2, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddTrigger((MoveCardAction mc) => mc.Origin.IsInPlay && !mc.Destination.IsInPlay && IsRenegadeMarked(mc.CardToMove), mc => ClearRenegadeKey(mc.CardToMove), TriggerType.Hidden, TriggerTiming.After);
        }

        private bool IsRenegadeMarked(Card card)
        {
            return GameController.GetCardPropertyJournalEntryBoolean(card, renegadeKey) == true;
        }
        private IEnumerator ClearRenegadeKey(Card c)
        {
            GameController.AddCardPropertyJournalEntry(c, renegadeKey, false);
            yield break;
        }
        private IEnumerator MakeIrreducibleNextTurn(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                var card = scd.SelectedCard;
                var holderEffect = IrreducibleNextTurnHolder(card);
                holderEffect.UntilCardLeavesPlay(card);
                holderEffect.CanEffectStack = true;
                holderEffect.TurnPhaseCriteria.IsEphemeral = false;
                holderEffect.TurnTakerCriteria.IsSpecificTurnTaker = card.Owner;
                holderEffect.NumberOfUses = 1;
                holderEffect.BeforeOrAfter = BeforeOrAfter.After;
                holderEffect.CardSource = CharacterCard;

                GameController.AddCardPropertyJournalEntry(card, renegadeKey, true);
                IEnumerator coroutine = GameController.AddStatusEffect(holderEffect, true, GetCardSource());
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

        private OnPhaseChangeStatusEffect IrreducibleNextTurnHolder(Card chosenCard)
        {
            return new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(ActivateThisTurnIrreducible), $"On {chosenCard.Owner.Name}'s next turn, damage dealt by {chosenCard.Title} is irreducible.", new TriggerType[] { TriggerType.Hidden }, this.Card);
        }

        public IEnumerator ActivateThisTurnIrreducible(PhaseChangeAction _, StatusEffect _2)
        {
            var needsIrreducibleEffect = GameController.FindCardsWhere(new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText && c.Owner == Game.ActiveTurnTaker && IsRenegadeMarked(c)));
            foreach (Card c in needsIrreducibleEffect)
            {
                var activeEffect = new MakeDamageIrreducibleStatusEffect();
                activeEffect.UntilThisTurnIsOver(Game);
                activeEffect.SourceCriteria.IsSpecificCard = c;
                activeEffect.CardSource = CharacterCard;
                activeEffect.UntilCardLeavesPlay(c);

                IEnumerator coroutine = AddStatusEffect(activeEffect, showMessage: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                ClearRenegadeKey(c);
            }
            yield break;
        }
    }
}
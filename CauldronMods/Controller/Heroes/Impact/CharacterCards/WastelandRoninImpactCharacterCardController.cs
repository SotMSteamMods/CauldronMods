using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Impact
{
    public class WastelandRoninImpactCharacterCardController : ImpactSubCharacterCardController
    {
        public WastelandRoninImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} regains 1 HP, or the next time one of your ongoing cards is destroyed, play it."
            int numHPGain = GetPowerNumeral(0, 1);
            var functions = new Function[]
            {
                new Function(DecisionMaker, $"Regain {numHPGain} HP", SelectionType.GainHP, () => GameController.GainHP(this.CharacterCard, numHPGain, cardSource: GetCardSource())),
                new Function(DecisionMaker, "The next time one of your ongoing cards is destroyed, play it.", SelectionType.PlayCard, AddOngoingDestructionTrigger)
            };
            var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, cardSource: GetCardSource()); ;
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator AddOngoingDestructionTrigger()
        {
            var statusEffect = new WhenCardIsDestroyedStatusEffect(CardWithoutReplacements, nameof(PlayDestroyedOngoing), $"The next time one of {this.TurnTaker.Name}'s ongoings is destroyed, they play it again.", new TriggerType[] { TriggerType.PlayCard }, DecisionMaker.HeroTurnTaker, this.Card);
            statusEffect.NumberOfUses = 1;
            statusEffect.CardDestroyedCriteria.OwnedBy = this.TurnTaker;
            statusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "ongoing" };
            statusEffect.CanEffectStack = true;
            statusEffect.CardSource = CharacterCard;

            IEnumerator coroutine = AddStatusEffect(statusEffect);
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

        public IEnumerator PlayDestroyedOngoing(DestroyCardAction dc, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            dc.AddAfterDestroyedAction(() => GameController.PlayCard(DecisionMaker, dc.CardToDestroy.Card, cardSource: GetCardSource()), this);
            dc.PostDestroyDestinationCanBeChanged = false;
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
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
                        yield break;
                    }
                case 1:
                    {
                        //"Environment cards cannot be played during the next environment turn.",
                        var holderEffect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(EnvironmentCannotPlayEffect), "During the next environment turn, environment cards cannot be played.", new TriggerType[] { TriggerType.Hidden }, this.Card);
                        holderEffect.NumberOfUses = 1;
                        holderEffect.TurnTakerCriteria.IsEnvironment = true;
                        holderEffect.TurnPhaseCriteria.IsEphemeral = false;
                        holderEffect.CardSource = CharacterCard;
                        holderEffect.CanEffectStack = true;

                        coroutine = AddStatusEffect(holderEffect);
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
                        //"Select a target. The next damage it deals is irreducible."
                        var storedTarget = new List<SelectCardDecision> { };
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && AskIfCardIsVisibleToCardSource(c, GetCardSource()) != false), storedTarget, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (storedTarget.FirstOrDefault() != null && storedTarget.FirstOrDefault().SelectedCard != null)
                        {
                            var target = storedTarget.FirstOrDefault().SelectedCard;
                            var irreducibleEffect = new MakeDamageIrreducibleStatusEffect();
                            irreducibleEffect.SourceCriteria.IsSpecificCard = target;
                            irreducibleEffect.NumberOfUses = 1;
                            irreducibleEffect.CardSource = CharacterCard;
                            irreducibleEffect.CreateImplicitExpiryConditions();

                            coroutine = AddStatusEffect(irreducibleEffect);
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

        public IEnumerator EnvironmentCannotPlayEffect(PhaseChangeAction pc, StatusEffect effect)
        {
            var newEffect = new CannotPlayCardsStatusEffect();
            newEffect.TurnTakerCriteria.IsEnvironment = true;
            newEffect.CardSource = CharacterCard;
            newEffect.UntilThisTurnIsOver(Game);

            IEnumerator coroutine = AddStatusEffect(newEffect);
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
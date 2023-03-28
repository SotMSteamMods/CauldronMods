using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MagnificentMaraCharacterCardController : MaraUtilityCharacterCardController
    {
        public MagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetAmount = GetPowerNumeral(0, 1);
            int firstTargetDamage = GetPowerNumeral(1, 1);
            int secondTargetDamage = GetPowerNumeral(2, 1);
            //"{MagnificentMara} deals 1 target 1 psychic damage. That target deals another target 1 melee damage."
            List<SelectCardDecision> firstTargetDecision = new List<SelectCardDecision> { };
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), firstTargetDamage, DamageType.Psychic, targetAmount, false, targetAmount, selectTargetsEvenIfCannotDealDamage: true, storedResultsDecisions: firstTargetDecision, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(firstTargetDecision))
            {
                yield break;
            }

            Card firstTarget = firstTargetDecision.FirstOrDefault()?.SelectedCard;

            if (firstTarget != null && firstTarget.IsInPlayAndHasGameText && firstTarget.IsTarget)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, firstTarget), secondTargetDamage, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => c != firstTarget, cardSource: GetCardSource());
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
                case (0):
                    {
                        //"One hero may put a card from their trash on top of their deck.",
                        var turnTakerDecision = new SelectTurnTakerDecision(GameController, DecisionMaker, GameController.FindTurnTakersWhere((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.MoveCardOnDeck, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(turnTakerDecision, (TurnTaker tt) => GameController.SelectAndMoveCard(FindHeroTurnTakerController(tt.ToHero()), (Card c) => c.Location == tt.Trash, tt.Deck, cardSource: GetCardSource()));
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
                case (1):
                    {
                        //"Destroy 1 ongoing card.",
                        coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"), false, cardSource: GetCardSource());
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
                case (2):
                    {
                        //"Select a target. Until the start of your next turn that target is immune to damage from environment cards."
                        var storedTarget = new List<SelectCardDecision> { };
                        coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.PreventDamage, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && AskIfCardIsVisibleToCardSource(c, GetCardSource()) != false && c.IsTarget), storedTarget, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        var card = storedTarget.FirstOrDefault()?.SelectedCard;
                        if (card != null)
                        {
                            var preventDamage = new ImmuneToDamageStatusEffect();
                            preventDamage.TargetCriteria.IsSpecificCard = card;
                            preventDamage.SourceCriteria.IsEnvironment = true;
                            preventDamage.UntilStartOfNextTurn(TurnTaker);
                            coroutine = AddStatusEffect(preventDamage);
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
    }
}
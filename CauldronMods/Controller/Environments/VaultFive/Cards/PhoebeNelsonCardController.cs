using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class PhoebeNelsonCardController : VaultFiveUtilityCardController
    {
        public PhoebeNelsonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each villain target 1 fire damage and each hero with an Artifact card in their hand 1 fire damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);

            //Whenever an Artifact card enters play, this card deals itself 2 psychic damage.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && IsArtifact(cpa.CardEnteringPlay), DealDamageSelfResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageSelfResponse(CardEntersPlayAction arg)
        {
            //this card deals itself 2 psychic damage.
            IEnumerator coroutine = DealDamage(Card, Card, 2, DamageType.Psychic, cardSource: GetCardSource());
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

        private IEnumerator DealDamageResponse(PhaseChangeAction arg)
        {
            //this card deals each villain target 1 fire damage and each hero with an Artifact card in their hand 1 fire damage.
            IEnumerator coroutine = DealDamage(Card, (Card c) => IsVillainTarget(c), 1, DamageType.Fire);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            IEnumerable<TurnTaker> heroesWithArtifactsInHand = GetPlayersWithArtifactsInHand();
            foreach (TurnTaker turnTaker in heroesWithArtifactsInHand)
            {
                if (Card.IsInPlayAndHasGameText)
                {
                    Card hero = null;
                    if (!turnTaker.HasMultipleCharacterCards)
                    {
                        hero = turnTaker.CharacterCard;
                    }
                    else
                    {
                        List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTarget, new LinqCardCriteria((Card c) => c.Owner == turnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active heroes"), storedDecision, false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
                        if (selectCardDecision != null)
                        {
                            hero = selectCardDecision.SelectedCard;
                        }
                    }

                    if (hero != null)
                    {
                        coroutine = DealDamage(Card, hero, 1, DamageType.Fire, cardSource: GetCardSource());
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

            yield break;
        }
    }
}

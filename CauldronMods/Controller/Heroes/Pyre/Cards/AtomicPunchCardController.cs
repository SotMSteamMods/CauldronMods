using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class AtomicPunchCardController : PyreUtilityCardController
    {
        public AtomicPunchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Select 2 non-{PyreIrradiate} cards in 1 player's hand. {PyreIrradiate} those cards until they leave that player's hand. 
            var irradiateStorage = new List<bool>();
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.CardFromHand, tt => IrradiateCardsInHand(tt, irradiateStorage), 1, false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //If 2 of your cards were {PyreIrradiate} this way, increase energy damage dealt by {Pyre} by 1 until the end of your turn.",
            if(irradiateStorage.Count() >= 2)
            {
                var effect = new IncreaseDamageStatusEffect(1);
                effect.UntilEndOfPhase(TurnTaker, Phase.End);
                effect.SourceCriteria.IsSpecificCard = CharacterCard;
                effect.DamageTypeCriteria.AddType(DamageType.Energy);
                effect.CreateImplicitExpiryConditions();
                effect.CardSource = Card;

                coroutine = AddStatusEffect(effect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //"{Pyre} deals 1 target 2 energy damage."
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
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

        private IEnumerator IrradiateCardsInHand(TurnTaker tt, List<bool> pyreCardsIrradiateCount)
        {
            var heroTT = tt.ToHero();
            var decision = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => c.Location == heroTT.Hand && !IsIrradiated(c), SelectionType.CardFromHand, 2, false, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardsAndDoAction(decision, (SelectCardDecision scd) => IrradiateAndStoreIfPyre(scd, pyreCardsIrradiateCount), cardSource: GetCardSource());
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

        private IEnumerator IrradiateAndStoreIfPyre(SelectCardDecision scd, List<bool> pyreCardsIrradiateCount)
        {
            var toIrradiate = scd.SelectedCard;
            if(toIrradiate != null)
            {
                IEnumerator coroutine = IrradiateCard(toIrradiate);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (toIrradiate.Owner == TurnTaker)
                {
                    pyreCardsIrradiateCount.Add(true);
                }
            }
            yield break;
        }
    }
}

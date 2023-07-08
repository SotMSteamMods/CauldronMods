using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class EtherealArmoryCardController : TerminusBaseCardController
    {
        /* 
         * Each player may play an ongoing or equipment card now.
         * At the start of your next turn, return each of those cards that is still in play to its player's hand.
         */
        public EtherealArmoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Each player may play an ongoing or equipment card now.
            coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                            new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())),
                            SelectionType.PlayCard,
                            PlayCardResponse,
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

            yield break;
        }

        private IEnumerator PlayCardResponse(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            List<PlayCardAction> playCardActions = new List<PlayCardAction>();
            IEnumerator coroutine = base.GameController.SelectAndPlayCardFromHand(heroTTC, true, storedResults: playCardActions, cardCriteria: new LinqCardCriteria((card) => IsOngoing(card) || base.IsEquipment(card)), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayCards(playCardActions))
            {
                var playedCard = playCardActions.FirstOrDefault().CardToPlay;
                string playerPossessive = DecisionMaker.Name;

                var onPhaseChangeStatusEffect = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements,
                    nameof(this.StartOfTurnResponse),
                    "At the start of " + DecisionMaker.Name + "'s next turn, return " + playedCard.Title + " from play to your hand.",
                    new TriggerType[] { TriggerType.MoveCard },
                    playedCard);
                onPhaseChangeStatusEffect.NumberOfUses = 1;
                onPhaseChangeStatusEffect.BeforeOrAfter = BeforeOrAfter.After;
                onPhaseChangeStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
                onPhaseChangeStatusEffect.TurnPhaseCriteria.Phase = Phase.Start;
                onPhaseChangeStatusEffect.TurnPhaseCriteria.TurnTaker = base.TurnTaker;
                onPhaseChangeStatusEffect.TurnIndexCriteria.GreaterThan = base.Game.TurnIndex;
                onPhaseChangeStatusEffect.UntilCardLeavesPlay(playedCard);

                coroutine = base.AddStatusEffect(onPhaseChangeStatusEffect);
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
       

        public IEnumerator StartOfTurnResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect effect)
        {
            //...return it from play to your hand.
            IEnumerator coroutine;
            TurnTakerController originalCardController = base.FindTurnTakerController(effect.CardMovedExpiryCriteria.Card.Owner);

            coroutine = base.GameController.MoveCard(originalCardController, effect.CardMovedExpiryCriteria.Card, originalCardController.TurnTaker.ToHero().Hand, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SendMessageAction($"{Card.Title} returns {effect.CardMovedExpiryCriteria.Card.Title} to {originalCardController.TurnTaker.ToHero().Hand.GetFriendlyName()}.", Priority.Medium, GetCardSource(), associatedCards: effect.CardMovedExpiryCriteria.Card.ToEnumerable(), showCardSource: true);
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

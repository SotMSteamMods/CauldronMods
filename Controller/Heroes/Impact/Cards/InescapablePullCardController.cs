using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class InescapablePullCardController : CardController
    {
        public InescapablePullCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TargetsDamagedThisTurn);
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw a card.",
            IEnumerator coroutine = DrawCard();
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

        public override IEnumerator UsePower(int index = 0)
        {
            int damages = GetPowerNumeral(0, 4);

            var selectedCard = new List<SelectCardDecision> { };
            //"Select a target that {Impact} damaged this turn. {Impact} deals that target 4 infernal damage."
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), damages, DamageType.Infernal, 1, false, 1,
                                                        additionalCriteria: (Card c) => HasDamagedTargetThisTurn(c), storedResultsDecisions: selectedCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(selectedCard))
            {
                coroutine = GameController.SendMessageAction($"{this.CharacterCard.Title} has not damaged any targets this turn, so no damage can be dealt.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private bool HasDamagedTargetThisTurn(Card c)
        {
            return Journal.DealDamageEntriesThisTurn().Any((DealDamageJournalEntry ddj) => ddj.SourceCard == this.CharacterCard && ddj.TargetCard == c);
        }

        private string TargetsDamagedThisTurn()
        {
            string start = $"Targets dealt damage by {this.TurnTaker.Name} this turn: ";
            var damagedTargets = GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && HasDamagedTargetThisTurn(c)), visibleToCard: GetCardSource());
            string end;
            if (damagedTargets.FirstOrDefault() == null)
            {
                end = "None";
            }
            else
            {
                end = string.Join(", ", damagedTargets.Select(c => c.Title).ToArray());
            }
            return start + end + ".";
        }
    }
}
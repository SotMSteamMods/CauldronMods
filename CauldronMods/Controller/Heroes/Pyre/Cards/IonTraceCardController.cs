using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class IonTraceCardController : PyreUtilityCardController
    {
        public IonTraceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIrradiatedCount();
        }

        public override IEnumerator Play()
        {
            //"Two players may each select a non-{PyreIrradiate} card in their hand and move a card that shares a keyword with it from their trash to their hand. {PyreIrradiate} any cards selected or moved this way until they leave their hands.",
            var selectHeroes = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && tt.ToHero().Hand.Cards.Any((card) => !IsIrradiated(card))), SelectionType.ReturnToHand, 2, false, 2, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(selectHeroes, RescueAndIrradiateCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"{Pyre} deals each non-hero target 0 energy damage."
            coroutine = DealDamage(CharacterCard, (Card c) => !IsHeroTarget(c), 0, DamageType.Energy);
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

        private IEnumerator RescueAndIrradiateCards(TurnTaker tt)
        {
            //...select a non-{PyreIrradiate} card in their hand...
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var storedCard = new List<SelectCardDecision>();
            IEnumerator coroutine = SelectAndIrradiateCardsInHand(heroTTC, tt, 1, 0, storedCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(storedCard))
            {
                yield break;
            }

            //and move a card that shares a keyword with it from their trash to their hand.
            var cardInHand = GetSelectedCard(storedCard);
            var handKeywords = GameController.GetAllKeywords(cardInHand);
            var storedTrashCard = new List<SelectCardDecision>();
            coroutine = GameController.SelectAndMoveCard(heroTTC, ((Card c) => c.Location == tt.Trash && c.DoKeywordsContain(handKeywords)), tt.ToHero().Hand, storedResults: storedTrashCard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{PyreIrradiate} any cards selected or moved this way until they leave their hands.",
            if(DidSelectCard(storedTrashCard))
            {
                coroutine = IrradiateCard(GetSelectedCard(storedTrashCard));
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
    }
}

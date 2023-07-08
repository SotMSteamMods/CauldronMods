using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class AtmosphereScrubbersCardController : PyreUtilityCardController
    {
        public AtmosphereScrubbersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIrradiatedCardsInHands();
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, you may use a power.",
            IEnumerator coroutine = GameController.SelectAndUsePower(DecisionMaker, cardSource: GetCardSource());
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
            int numHeroes = GetPowerNumeral(0, 2);
            int numHPGain = GetPowerNumeral(1, 2);

            //"2 heroes may each discard a card to regain 2 HP. Each hero who discards a {PyreIrradiate} card this way may draw a card."
            var validHeroes = new LinqTurnTakerCriteria(tt => IsHero(tt) && tt.ToHero().HasCardsInHand && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()));
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, validHeroes, SelectionType.DiscardCard, tt => DiscardHealAndDrawResponse(tt, numHPGain), numHeroes, false, 0, cardSource: GetCardSource());
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

        private IEnumerator DiscardHealAndDrawResponse(TurnTaker tt, int numHPGain = 2)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            if(heroTTC == null)
            {
                yield break;
            }

            //...may discard a card....
            var storedCard = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(heroTTC, SelectionType.DiscardCard, new LinqCardCriteria(c => c.Location == heroTTC.HeroTurnTaker.Hand), storedCard, true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(storedCard))
            {
                yield break;
            }

            var card = GetSelectedCard(storedCard);
            bool wasIrradiated = IsIrradiated(card);

            var storedDiscard = new List<DiscardCardAction>();
            coroutine = GameController.DiscardCard(heroTTC, card, new IDecision[] { storedCard.FirstOrDefault() }, TurnTaker, storedDiscard, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidDiscardCards(storedDiscard))
            {
                //...to regain 2 HP.
                var storedHero = new List<Card>();
                coroutine = FindCharacterCard(tt, SelectionType.GainHP, storedHero);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(storedHero.FirstOrDefault() != null)
                {
                    coroutine = GameController.GainHP(storedHero.FirstOrDefault(), numHPGain, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //Each hero who discards a PyreIrradiate card this way may draw a card.
                if(wasIrradiated)
                {
                    coroutine = DrawCard(heroTTC.HeroTurnTaker, optional: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            
            yield break;
        }
    }
}

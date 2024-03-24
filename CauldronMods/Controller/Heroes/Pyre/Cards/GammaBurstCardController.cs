using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class GammaBurstCardController : PyreUtilityCardController
    {
        public GammaBurstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIrradiatedCount(true);
        }
        public override IEnumerator Play()
        {
            //"Select up to 2 non-{PyreIrradiate} cards in 1 player's hand. {PyreIrradiate} those cards until they leave that player's hand.",
            var storedCards = new List<SelectCardDecision>();
            var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, GameController.AllHeroes.Where(htt => !htt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker), SelectionType.Custom, cardSource: GetCardSource());
            CurrentMode = CustomMode.PlayerToIrradiate;
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, tt => SelectAndIrradiateCardsInHand(FindHeroTurnTakerController(tt.ToHero()), tt, 2, 0, storedCards));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //"{Pyre} deals that hero and each non-hero target X energy damage, where X is the number of cards {PyreIrradiate} this way."
            if(selectHero.SelectedTurnTaker != null)
            {
                var numCardsIrradiated = storedCards.Where(scd => scd.SelectedCard != null).Count();
                var hero = selectHero.SelectedTurnTaker;
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numCardsIrradiated, DamageType.Energy, 1, false, 1, additionalCriteria: (Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && c.Owner == hero, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = DealDamage(CharacterCard, (Card c) => !IsHeroTarget(c), numCardsIrradiated, DamageType.Energy);
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

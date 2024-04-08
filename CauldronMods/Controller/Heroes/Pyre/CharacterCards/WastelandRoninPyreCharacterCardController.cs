using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class WastelandRoninPyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public WastelandRoninPyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numDraws = GetPowerNumeral(0, 2);
            //"The next time {Pyre} would deal damage to a hero target, you may redirect it to another target. 
            var redirectEffect = new RedirectDamageStatusEffect();
            redirectEffect.UntilTargetLeavesPlay(this.Card);
            redirectEffect.TargetCriteria.IsHero = true;
            redirectEffect.SourceCriteria.IsSpecificCard = this.Card;
            redirectEffect.NumberOfUses = 1;
            redirectEffect.RedirectableTargets.IsTarget = true;
            redirectEffect.IsOptional = true;

            IEnumerator coroutine = AddStatusEffect(redirectEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Draw 2 cards. 
            coroutine = DrawCards(DecisionMaker, numDraws);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Discard a card."
            coroutine = SelectAndDiscardCards(DecisionMaker, 1, false, 1);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, they deal 3 targets 3 energy damage each.",
                        var heroesWithIrradiated = GameController.AllHeroes.Where(htt => htt.Hand.Cards.Any(card => card.IsIrradiated()) && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesWithIrradiated, SelectionType.DiscardCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DiscardForDamage);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"Each target regains 1 HP.",
                        coroutine = GameController.GainHP(DecisionMaker, (Card c) => c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"One hero may deal themselves 2 energy damage and play 2 cards."
                        var heroesActive = GameController.AllHeroes.Where(htt => !htt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(htt, GetCardSource())).Select(htt => htt as TurnTaker);
                        var selectHero = new SelectTurnTakerDecision(GameController, DecisionMaker, heroesActive, SelectionType.PlayCard, true, cardSource: GetCardSource());
                        coroutine = GameController.SelectTurnTakerAndDoAction(selectHero, DamageSelfForPlays);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }

        private IEnumerator DiscardForDamage(TurnTaker tt)
        {
            //One player may discard a {PyreIrradiate} card...
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            var storedDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.SelectAndDiscardCard(heroTTC, true, card => card.IsIrradiated(), storedDiscard, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(storedDiscard))
            {
                List<Card> storedCharacter = new List<Card>();
                coroutine = FindCharacterCard(tt, SelectionType.HeroToDealDamage, storedCharacter);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                var damageDealer = storedCharacter.FirstOrDefault();
                if(damageDealer != null)
                {
                    coroutine = GameController.SelectTargetsAndDealDamage(heroTTC, new DamageSource(GameController, damageDealer), 3, DamageType.Energy, 3, false, 3, cardSource: GetCardSource());
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
        private IEnumerator DamageSelfForPlays(TurnTaker tt)
        {
            var heroTTC = FindHeroTurnTakerController(tt.ToHero());
            List<Card> storedCharacter = new List<Card>();
            IEnumerator coroutine = FindCharacterCard(tt, SelectionType.HeroToDealDamage, storedCharacter);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            var damageDealer = storedCharacter.FirstOrDefault();
            if(damageDealer == null)
            {
                yield break;
            }

            var damage = new DealDamageAction(GetCardSource(), new DamageSource(GameController, damageDealer), damageDealer, 2, DamageType.Energy);
            var decision = new YesNoCardDecision(GameController, heroTTC, SelectionType.DealDamageSelf, damageDealer, action: damage, cardSource: GetCardSource());
            coroutine = GameController.MakeDecisionAction(decision);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(decision))
            {
                coroutine = DealDamage(damageDealer, damageDealer, 2, DamageType.Energy, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectAndPlayCardsFromHand(heroTTC, 2, false, 2, cardSource: GetCardSource());
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

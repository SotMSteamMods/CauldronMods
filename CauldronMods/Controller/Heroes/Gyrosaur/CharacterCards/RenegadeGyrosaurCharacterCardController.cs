using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RenegadeGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public RenegadeGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            int numHPGain = GetPowerNumeral(0, 2);
            int numTargets = GetPowerNumeral(1, 1);
            int numDamage = GetPowerNumeral(2, 2);

            //"Discard a card.
            var storedDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = SelectAndDiscardCards(DecisionMaker, 1, false, 1, storedDiscard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(storedDiscard) && IsCrash(storedDiscard.FirstOrDefault().CardToDiscard))
            {
                //If it was a crash card, {Gyrosaur} regains 2 HP. 
                coroutine = GameController.GainHP(Card, numHPGain, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //Otherwise, she deals 1 target 2 melee damage. 
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), numDamage, DamageType.Melee, numTargets, false, numTargets, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Draw a card."
            coroutine = DrawCard();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, false, true, cardSource: GetCardSource());
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
                case 1:
                    {
                        //"Each hero with fewer than 2 non-character cards in play may use a power now.",
                        var viableHeroCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt)
                                                                                    && !tt.IsIncapacitatedOrOutOfGame
                                                                                    && tt.GetCardsWhere((Card c) => c.IsInPlay && !c.IsCharacter).Count() < 2
                                                                                    && GameController.CanPerformAction<UsePowerAction>(FindTurnTakerController(tt), GetCardSource())
                                                                                    && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), 
                                                                           "hero with fewer than 2 non-character cards in play");
                        coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                                                            viableHeroCriteria,
                                                            SelectionType.UsePower,
                                                            (TurnTaker tt) => GameController.SelectAndUsePower(FindHeroTurnTakerController(tt.ToHero()), cardSource: GetCardSource()),
                                                            cardSource: GetCardSource());
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
                        //"Targets cannot regain HP until the start of your next turn."
                        var noGainEffect = new CannotGainHPStatusEffect();
                        noGainEffect.TargetCriteria.IsTarget = true;
                        noGainEffect.UntilStartOfNextTurn(TurnTaker);
                        noGainEffect.CardSource = Card;

                        coroutine = AddStatusEffect(noGainEffect);
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
            }
            yield break;
        }
    }
}

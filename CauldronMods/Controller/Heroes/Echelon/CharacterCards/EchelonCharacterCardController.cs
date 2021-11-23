using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Echelon
{
    public class EchelonCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 1;
        private const int PowerTargetAmount = 1;
        private const int Incap2CardsToDraw = 2;

        public EchelonCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // {Echelon} deals 1 target 1 irreducible melee damage.
            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int targetsNumeral = base.GetPowerNumeral(0, PowerTargetAmount);
            int damageNumeral = base.GetPowerNumeral(1, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                damageNumeral, DamageType.Melee, targetsNumeral, false, 
                targetsNumeral, true, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator routine;
            switch (index)
            {
                case 0:
                    
                    // Destroy 1 ongoing card.
                    routine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController,
                        new LinqCardCriteria(card => card.IsOngoing && card.IsInPlay, "ongoing"),
                        true, cardSource: this.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;

                case 1:
                    // One player may draw 2 cards, then discard 2 cards.

                    List<TurnTaker> viableHeroes = GameController.AllHeroes.Where((HeroTurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())).Select(htt => htt as TurnTaker).ToList();
                    var selectTurnTaker = new SelectTurnTakerDecision(GameController, DecisionMaker, viableHeroes, SelectionType.DiscardAndDrawCard, numberOfCards: 2, cardSource: GetCardSource());

                    routine = GameController.SelectTurnTakerAndDoAction(selectTurnTaker, MayDrawTwoDiscardTwo);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                    break;

                case 2:

                    // Move the top card of the villain deck to the bottom of the villain deck.
                    List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                    routine = FindVillainDeck(this.HeroTurnTakerController, SelectionType.MoveCard, storedResults, loc => !loc.IsSubDeck);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                    if(!DidSelectLocation(storedResults))
                    {
                        yield break;
                    }
                    Location deck = GetSelectedLocation(storedResults);
                    routine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, deck.TopCard, new[]
                    {
                        new MoveCardDestination(deck, toBottom: true, showMessage: true)
                    }, cardSource:GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;
            }
        }

        private IEnumerator MayDrawTwoDiscardTwo(TurnTaker tt)
        {
            var player = FindHeroTurnTakerController(tt?.ToHero());
            if (player == null)
            {
                yield break;
            }

            var storedYesNo = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.DiscardAndDrawCard, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(storedYesNo))
            {
                coroutine = GameController.DrawCards(player, 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SelectAndDiscardCards(player, 2, false, 2, cardSource: GetCardSource());
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
    }
}
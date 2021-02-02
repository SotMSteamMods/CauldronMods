using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class MinistryOfStrategicScienceTerminusCharacterCardController : TerminusBaseCharacterCardController
    {
        // Power
        // Remove X tokens from your wrath pool (up to 2). {Terminus} deals 1 target X+1 cold damage and regains X+1 HP.
        private int UpToAmount => GetPowerNumeral(0, 2);
        private int TargetCount => GetPowerNumeral(1, 1);
        private int PlusColdDamage => GetPowerNumeral(2, 1);
        private int PlusHP => GetPowerNumeral(3, 1);

        public MinistryOfStrategicScienceTerminusCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.SetHP(base.CharacterCard, 0), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "TheLightAtTheEnd" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "CovenantOfWrath" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.AddTokensToPool(base.CharacterCard.FindTokenPool("TerminusWrathPool"), 3, base.GetCardSource()), TriggerType.PhaseChange, TriggerTiming.After);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<SelectNumberDecision> selectNumberDecisions = new List<SelectNumberDecision>();
            int valueOfX;

            // Remove X tokens from your wrath pool (up to 2). {Terminus} deals 1 target X+1 cold damage and regains X+1 HP.
            coroutine = base.GameController.SelectNumber(DecisionMaker, SelectionType.RemoveTokens, 0, UpToAmount, storedResults: selectNumberDecisions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectNumberDecisions != null && selectNumberDecisions.Count() > 0)
            {
                valueOfX = selectNumberDecisions.FirstOrDefault().SelectedNumber ?? 0;
                coroutine = TerminusWrathPoolUtility.RemoveWrathTokens<GameAction>(this, valueOfX);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), PlusColdDamage + valueOfX, DamageType.Cold, TargetCount, false, TargetCount, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.GameController.GainHP(base.CharacterCard, PlusHP + valueOfX, cardSource: base.GetCardSource());
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;

            switch (index)
            {
                // One hero may deal themselves 2 psychic damage to draw 2 cards.
                case 0:
                    coroutine = UseIncapacitatedAbility1(); 
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;
                // One non-hero target regains 3 HP.
                case 1:
                    coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 3, additionalCriteria: (card) => !card.IsHero, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;
                // Reveal the top 3 cards of a hero deck. Discard 2 of them and replace the other.
                case 2:
                    coroutine = UseIncapacitatedAbility3();
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

            yield break;
        }

        private IEnumerator UseIncapacitatedAbility1()
        {
            IEnumerator coroutine;
            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            List<DealDamageAction> dealDamageActions = new List<DealDamageAction>();
            Card hero;

            coroutine = base.GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.DealDamageSelf, selectCardDecisions, true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectCardDecisions != null && selectCardDecisions.Count() > 0)
            {
                hero = selectCardDecisions.FirstOrDefault().SelectedCard;
                coroutine = base.DealDamage(hero, hero, 2, DamageType.Psychic, isIrreducible: false, optional: false, isCounterDamage: false, null, dealDamageActions);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidDealDamage(dealDamageActions))
                {
                    coroutine = base.DrawCards(base.FindHeroTurnTakerController(hero.Owner.ToHero()), 2);
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

            yield break;
        }

        private IEnumerator UseIncapacitatedAbility3()
        {
            // Reveal the top 3 cards of a hero deck. Discard 2 of them and replace the other.
            IEnumerator coroutine;
            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            Card hero;
            MoveCardDestination topDeck;
            MoveCardDestination trash;

            coroutine = base.GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.RevealCardsFromDeck, selectCardDecisions, true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectCardDecisions != null && selectCardDecisions.Count() > 0)
            {
                hero = selectCardDecisions.FirstOrDefault().SelectedCard;
                topDeck = new MoveCardDestination(hero.Owner.Deck);
                trash = new MoveCardDestination(hero.Owner.Trash);

                coroutine = base.RevealCardsFromTopOfDeck_DetermineTheirLocation(DecisionMaker, base.FindHeroTurnTakerController(hero.Owner.ToHero()), hero.Owner.Deck, topDeck, trash, 3, 1, 0);
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

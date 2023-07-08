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
            TokenPool wrathPool = TerminusWrathPoolUtility.GetWrathPool(this);
            int realMax;

            if(wrathPool == null)
            {
                realMax = 0;
            }
            else if (wrathPool.CurrentValue < UpToAmount)
            {
                realMax = wrathPool.CurrentValue;
            }
            else
            {
                realMax = UpToAmount;
            }

            if (realMax == 0)
            {
                string message;
                if(wrathPool == null)
                {
                    message = "There is no wrath pool to remove tokens from.";
                }
                else
                {
                    message = $"There are no tokens in {wrathPool.Name} to remove.";
                }
                coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource());
                valueOfX = 0;
            }
            else
            {
                // Remove X tokens from your wrath pool (up to 2). 
                coroutine = base.GameController.SelectNumber(DecisionMaker, SelectionType.RemoveTokens, 0, realMax, storedResults: selectNumberDecisions, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                valueOfX = selectNumberDecisions.FirstOrDefault()?.SelectedNumber ?? 0;
            }

            if (valueOfX > 0)
            {
                coroutine = TerminusWrathPoolUtility.RemoveWrathTokens<GameAction>(this, valueOfX);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{Terminus} deals 1 target X+1 cold damage 
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), PlusColdDamage + valueOfX, DamageType.Cold, TargetCount, false, TargetCount, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and regains X+1 HP.
            coroutine = base.GameController.GainHP(base.CharacterCard, PlusHP + valueOfX, cardSource: base.GetCardSource());
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
                    coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 3, additionalCriteria: (card) => !IsHero(card), cardSource: base.GetCardSource());
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
                    Func<TurnTaker, List<LocationChoice>> AllDecksForTurnTaker = delegate (TurnTaker tt)
                    {
                        var decks = new List<LocationChoice>();
                        decks.Add(new LocationChoice(tt.Deck));
                        foreach(Location subdeck in tt.SubDecks)
                        {
                            if(subdeck.IsRealDeck)
                            {
                                decks.Add(new LocationChoice(subdeck));
                            }
                        }
                        return decks;
                    };
                    List<LocationChoice> heroDecks = GameController.AllTurnTakers.Where(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()))
                                                            .SelectMany(tt => AllDecksForTurnTaker(tt))
                                                            .ToList();
                    if(heroDecks.Count() == 0)
                    {
                        coroutine = GameController.SendMessageAction("There were no decks that could be chosen.", Priority.High, GetCardSource());
                    }
                    if(heroDecks.Count() == 1)
                    {
                        coroutine = Incap3RevealAndDiscard(heroDecks.FirstOrDefault().Location);
                    }
                    else
                    {
                        var locationDecision = new SelectLocationDecision(GameController, DecisionMaker, heroDecks, SelectionType.RevealCardsFromDeck, false, cardSource: GetCardSource());
                        coroutine = GameController.SelectLocationAndDoAction(locationDecision, Incap3RevealAndDiscard);
                    }
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

            coroutine = base.GameController.SelectHeroCharacterCard(DecisionMaker, SelectionType.DealDamageSelf, selectCardDecisions, false, cardSource: base.GetCardSource());
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
                if(hero == null)
                {
                    yield break;
                }
                var heroTTC = FindHeroTurnTakerController(hero.Owner.ToHero());
                var previewDamage = new DealDamageAction(GetCardSource(), new DamageSource(GameController, hero), hero, 2, DamageType.Psychic);
                var dealSelfDamage = new YesNoAmountDecision(GameController, heroTTC, SelectionType.DealDamageSelf, 2, action: previewDamage, associatedCards: new Card[] { hero }, cardSource: GetCardSource());
                coroutine = GameController.MakeDecisionEvent(dealSelfDamage);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayerAnswerYes(dealSelfDamage))
                {
                    coroutine = base.DealDamage(hero, hero, 2, DamageType.Psychic, isIrreducible: false, optional: false, isCounterDamage: false, null, dealDamageActions);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = base.DrawCards(heroTTC, 2);
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
        private IEnumerator Incap3RevealAndDiscard(Location deck)
        {
            var deckDestination = new MoveCardDestination(deck);
            MoveCardDestination trashDestination;
            if(deck.IsSubDeck)
            {
                var subTrash = deck.OwnerTurnTaker.FindSubTrash(deck.Identifier);
                trashDestination = new MoveCardDestination(subTrash);
            }
            else
            {
                trashDestination = new MoveCardDestination(deck.OwnerTurnTaker.Trash);
            }
            

            IEnumerator coroutine = base.RevealCardsFromTopOfDeck_DetermineTheirLocation(DecisionMaker, base.FindHeroTurnTakerController(deck.OwnerTurnTaker.ToHero()), deck, trashDestination, deckDestination, 3, 2, 1, responsibleTurnTaker: TurnTaker);
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

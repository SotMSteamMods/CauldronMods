using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{

    public class BaccaratCharacterCardController : HeroCharacterCardController
    {
        public BaccaratCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick") && c.IsInTrash, "trick"));
        }

        private List<Card> actedHeroes;

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        //Put 2 cards from a trash on the bottom of their deck.

                        List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
                        //Pick first card (and subsequentially the deck)
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardOnBottomOfDeck, new LinqCardCriteria((Card c) => c.IsInTrash && this.GameController.IsLocationVisibleToSource(c.Location, base.GetCardSource(null))), selectCardDecisions, false);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if(!DidSelectCard(selectCardDecisions))
                        {
                            yield break;
                        }

                        List<MoveCardDestination> list = new List<MoveCardDestination>
                        {
                            new MoveCardDestination(GetSelectedCard(selectCardDecisions).NativeDeck, true)
                        };
                        //Move first card
                        coroutine = base.GameController.MoveCard(this.TurnTakerController, selectCardDecisions.FirstOrDefault().SelectedCard, list.FirstOrDefault().Location, true, false, false, null, false, null, null, null, false, false, null, false, false, false, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //Pick second card
                        List<SelectCardDecision> selectCardDecisions2 = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.MoveCardOnBottomOfDeck, new LinqCardCriteria((Card c) => c.IsInTrash && c.NativeDeck == list.FirstOrDefault().Location && this.GameController.IsLocationVisibleToSource(c.Location, base.GetCardSource(null))), selectCardDecisions2, false);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if(!DidSelectCard(selectCardDecisions2))
                        {
                            yield break;
                        }

                        //Move second card
                        coroutine = base.GameController.MoveCard(this.TurnTakerController, GetSelectedCard(selectCardDecisions2), list.FirstOrDefault().Location, true, false, false, null, false, null, null, null, false, false, null, false, false, false, true, base.GetCardSource(null));
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
                        //Increase the next damage dealt by a hero target by 2.
                        IEnumerator coroutine2 = base.AddStatusEffect(new IncreaseDamageStatusEffect(2)
                        {
                            SourceCriteria =
                            {
                                IsHero = new bool?(true)
                            },
                            TargetCriteria =
                            {
                                IsTarget = new bool?(true)
                            },
                            NumberOfUses = new int?(1)
                        });
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //Each hero character may deal themselves 3 toxic damage to use a power now.
                        this.actedHeroes = new List<Card>();
                        IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
                        {
                            new Function(base.FindCardController(c).DecisionMaker, "Deal self 3 toxic damage and use a power", SelectionType.UsePower, () => this.DealDamageAndUsePowerResponse(c))
                        };
                        IEnumerator coroutine3 = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }
            yield break;
        }
        private IEnumerator DealDamageAndUsePowerResponse(Card card)
        {
            if (card != null)
            {
                CardController cc = base.FindCardController(card);
                IEnumerator coroutine = base.DealDamage(card, card, 3, DamageType.Toxic, false, false, false, null, null, null, false, null);
                IEnumerator e2 = base.SelectAndUsePower(cc, false, null, null, false, false, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(e2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(e2);
                }
                this.LogActedCard(card);
            }
            yield break;
        }

        private void LogActedCard(Card card)
        {
            if (card.SharedIdentifier != null)
            {
                IEnumerable<Card> collection = base.FindCardsWhere((Card c) => c.SharedIdentifier != null && c.SharedIdentifier == card.SharedIdentifier && c != card, false, null, false);
                this.actedHeroes.AddRange(collection);

            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<Function> list = new List<Function>();
            //Discard the top card of your deck...
            list.Add(new Function(this.DecisionMaker, "Discard the top card of your deck", SelectionType.DiscardFromDeck, () => base.GameController.DiscardTopCard(base.TurnTaker.Deck, null, (Card c) => true, cardSource: base.GetCardSource())));
            //...or put up to 2 trick cards with the same name from your trash into play.
            list.Add(new Function(this.DecisionMaker, "Put up to 2 trick cards with the same name from your trash into play", SelectionType.PlayCard, () => this.PlayTricksFromTrash()));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, null, null, null, base.GetCardSource(null));
            IEnumerator coroutine3 = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }

        private IEnumerator PlayTricksFromTrash()
        {
            //...or put up to 2 trick cards with the same name from your trash into play.
            List<SelectCardDecision> list = new List<SelectCardDecision>();
            int upTo = base.GetPowerNumeral(0, 2);
            IEnumerator coroutine = SelectAndMoveCardOptional(base.HeroTurnTakerController, (Card c) => c.IsInTrash && c.DoKeywordsContain("trick"), base.TurnTaker.PlayArea, false, true, true, true, list, base.GetCardSource(null));
            coroutine = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(0), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick"), "trick"), true, false, false, true, storedResults: list);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (upTo > 1)
            {
                //play second card
                if (list.FirstOrDefault() != null && list.FirstOrDefault().SelectedCard != null)
                {
                    coroutine = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(0), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick") && c.Identifier == list.FirstOrDefault().SelectedCard.Identifier && c.InstanceIndex != list.FirstOrDefault().SelectedCard.InstanceIndex, "trick"), true, false, false, true);
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
            if (upTo > 2)
            {
                //play third card if Guise uses Power Numerals
                if (list.FirstOrDefault() != null && list.FirstOrDefault().SelectedCard != null)
                {
                    coroutine = base.SearchForCards(base.HeroTurnTakerController, false, true, new int?(0), 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick") && c.Identifier == list.FirstOrDefault().SelectedCard.Identifier && c.InstanceIndex != list.FirstOrDefault().SelectedCard.InstanceIndex && c.InstanceIndex != list.LastOrDefault().SelectedCard.InstanceIndex, "trick"), true, false, false, true);
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
        public IEnumerator SelectAndMoveCardOptional(HeroTurnTakerController hero, Func<Card, bool> criteria, Location toLocation, bool toBottom = false, bool optional = false, bool isPutIntoPlay = false, bool playIfMovingToPlayArea = true, List<SelectCardDecision> storedResults = null, CardSource cardSource = null)
        {
            BattleZone battleZone = null;
            if (cardSource != null)
            {
                battleZone = cardSource.BattleZone;
            }
            SelectCardDecision selectCardDecision = new SelectCardDecision(this.GameController, hero, SelectionType.MoveCard, this.GameController.FindCardsWhere(criteria, true, null, battleZone), optional, false, null, null, null, null, null, false, true, cardSource, null);
            selectCardDecision.BattleZone = battleZone;
            if (storedResults != null)
            {
                storedResults.Add(selectCardDecision);
            }
            IEnumerator coroutine = this.GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision d) => this.GameController.MoveCard(hero, d.SelectedCard, toLocation, toBottom, isPutIntoPlay, playIfMovingToPlayArea, null, false, null, null, null, false, false, null, false, false, false, false, cardSource), true);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
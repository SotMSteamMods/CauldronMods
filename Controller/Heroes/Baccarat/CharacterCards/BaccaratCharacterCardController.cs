using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace Cauldron.Baccarat
{

    public class BaccaratCharacterCardController : HeroCharacterCardController
    {
        #region Constructors

        public BaccaratCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties

        private List<Card> actedHeroes;

        #endregion Properties

        #region Methods

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        //Put 2 cards from a trash on the bottom of their deck.
                        IEnumerator coroutine = base.GameController.SelectADeck(this.HeroTurnTakerController, SelectionType.MoveCardOnBottomOfDeck, null, storedResults, false, null, base.GetCardSource(null));
                        var a = storedResults.FirstOrDefault().SelectedTurnTaker.Trash;
                        CardSource source = base.GetCardSource(null);
                        coroutine = base.GameController.SelectAndPlayCard(this.DecisionMaker, (Card c) => c.Location.IsTrash && this.GameController.IsLocationVisibleToSource(c.Location, source) && c == c.Location.TopCard, false, true, base.GetCardSource(null), "There are no cards in any trashes.", null, false);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        List<MoveCardDestination> list = new List<MoveCardDestination>
                        {
                            new MoveCardDestination(storedResults.FirstOrDefault().SelectedTurnTaker.Deck, true, false, false)
                        };
                        coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController, null, new int?(2),2,new LinqCardCriteria((Card c) => c.Location.IsTrash && c.Location.OwnerTurnTaker.Identifier == storedResults.FirstOrDefault().SelectedTurnTaker.Identifier), list, true, false, false);
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
                            new Function(base.FindCardController(c).DecisionMaker, "Deal self 3 toxic damage and use a power", SelectionType.UsePower, () => this.DealDamageAndUsePowerResponse(c), null, null, null)
                        };
                        IEnumerator coroutine3 = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
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
                IEnumerator coroutine = base.DealDamage(card, card, 3, DamageType.Toxic, false, true, false, null, null, null, false, null);
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
                e2 = null;
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
            
            this.actedHeroes = new List<Card>();
            IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
            {
                //Discard the top card of your deck...
                new Function(base.FindCardController(c).DecisionMaker, "Discard the top card of your deck", SelectionType.DiscardFromDeck, () => base.DiscardCardsFromTopOfDeck(this.TurnTakerController, 1, false, null, false, this.TurnTaker), null, null, null),
                //...or put up to 2 trick cards with the same name from your trash into play.
                new Function(base.FindCardController(c).DecisionMaker, "put up to 2 trick cards with the same name from your trash into play", SelectionType.PlayCard, () => this.PlayTricksFromTrash(), null, null, null)
            };
            IEnumerator coroutine3 = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
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
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.SearchTrash, new LinqCardCriteria((Card c) => c.Location.IsTrash && TwoOrMoreCopiesInTrash(c) && c.DoKeywordsContain("trick"), "trick cards with two or more copies in the trash", false, false, null, null, false), list, false, false, null, true, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.GameController.PlayCards(this.HeroTurnTakerController, (Card c) => c.Identifier == list.FirstOrDefault<SelectCardDecision>().SelectedCard.Identifier, false, true, new int?(2), null, true, null, null, null, this.TurnTaker, base.GetCardSource(null));
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
        private bool TwoOrMoreCopiesInTrash(Card c)
        {
            int num = (from card in base.TurnTaker.Trash.Cards
                       where card.Identifier == c.Identifier
                       select card).Count<Card>();
            return num >= 2;

        }

        #endregion Methods
    }
}
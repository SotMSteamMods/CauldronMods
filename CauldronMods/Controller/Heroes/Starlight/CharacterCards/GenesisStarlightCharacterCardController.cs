using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Starlight
{
    public class GenesisStarlightCharacterCardController : StarlightSubCharacterCardController
    {
        public GenesisStarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int damageNumeral = base.GetPowerNumeral(0, 1);
            //"Starlight deals each hero target 1 energy damage."
            IEnumerator damageHeroes = DealDamage(this.Card, (Card c) => c.IsInPlayAndHasGameText && IsHero(c), damageNumeral, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageHeroes);
            }
            else
            {
                GameController.ExhaustCoroutine(damageHeroes);
            }

            int searchNumeral = base.GetPowerNumeral(1, 2);
            //"Search your deck for up to 2 constellation cards and play them. Shuffle your deck."
            List<MoveCardDestination> intoPlay = new List<MoveCardDestination> { new MoveCardDestination(TurnTaker.PlayArea) };
            IEnumerator searchAndPlay = GameController.SelectCardsFromLocationAndMoveThem(HeroTurnTakerController,
                                                                            TurnTaker.Deck,
                                                                            minNumberOfCards: 0, maxNumberOfCards: searchNumeral,
                                                                            new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"),
                                                                            intoPlay,
                                                                            shuffleAfterwards: true,
                                                                            cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(searchAndPlay);
            }
            else
            {
                GameController.ExhaustCoroutine(searchAndPlay);
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"One player may put an ongoing card from their trash into their hand.",
                        List<TurnTaker> usableHeroes = GameController.AllTurnTakers
                                                                     .Where((TurnTaker tt) => IsHero(tt) &&
                                                                                            !tt.IsIncapacitatedOrOutOfGame &&
                                                                                            GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                                                                                            tt.Trash.Cards.Where((Card c) => IsOngoing(c)).Count() > 0)
                                                                     .ToList();
                        SelectTurnTakerDecision whoGetsCard = new SelectTurnTakerDecision(GameController,
                                                                            HeroTurnTakerController,
                                                                            usableHeroes,
                                                                            SelectionType.MoveCardToHandFromTrash,
                                                                            isOptional: true,
                                                                            cardSource: GetCardSource());
                        Func<TurnTaker, IEnumerator> getOngoingFromTrash = (TurnTaker tt) => GameController.SelectAndMoveCard(GameController.FindHeroTurnTakerController(tt.ToHero()),
                                                                                                                        (Card c) => c.IsInTrash && IsOngoing(c) && c.Location == tt.Trash,
                                                                                                                        tt.ToHero().Hand,
                                                                                                                        optional: true,
                                                                                                                        cardSource: GetCardSource());
                        IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(whoGetsCard, getOngoingFromTrash);
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
                        //"One player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"Look at the top card of a deck, and replace it or discard it."
                        List<SelectLocationDecision> storedDeck = new List<SelectLocationDecision> { };
                        IEnumerator pickDeck = GameController.SelectADeck(HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location loc) => true, storedDeck, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(pickDeck);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(pickDeck);
                        }

                        if (!DidSelectDeck(storedDeck))
                        {
                            yield break;
                        }
                        Location chosenDeck = storedDeck.FirstOrDefault().SelectedLocation.Location;
                        IEnumerator revealTopCard = RevealCard_DiscardItOrPutItOnDeck(HeroTurnTakerController, HeroTurnTakerController, chosenDeck, false);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(revealTopCard);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(revealTopCard);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
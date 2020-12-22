using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class CommandAndConquerCardController : EchelonBaseCardController
    {
        //==============================================================
        // Reveal cards from the top of your deck until 2 Tactics are revealed.
        // Shuffle the other revealed cards into your deck.
        // Put 1 revealed Tactic into play and the other on the top or bottom of your deck.
        // {Echelon} deals 1 target 2 lightning or 2 melee damage.
        //==============================================================

        public static string Identifier = "CommandAndConquer";

        public CommandAndConquerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {

            // Reveal cards from the top of your deck until 2 Tactics are revealed.
            List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
            IEnumerator routine = base.GameController.RevealCards(base.HeroTurnTakerController, base.TurnTaker.Deck, IsTactic, 2, 
                revealedCardActions, RevealedCardDisplay.ShowMatchingCards, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            List<Card> tacticCards = GetRevealedCards(revealedCardActions).Where(IsTactic).Take(2).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardActions).Where(c => !tacticCards.Contains(c)).ToList();

            if (otherCards.Any())
            {
                // Shuffle the rest of the revealed cards into your deck.
                routine = base.GameController.MoveCards(this.DecisionMaker, otherCards, this.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(routine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(routine);
                }

                routine = this.ShuffleDeck(this.DecisionMaker, this.TurnTaker.Deck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(routine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(routine);
                }
            }

            List<PlayCardAction> playCardActions = new List<PlayCardAction>();
            
            if (tacticCards.Any())
            {
                routine = GameController.SelectAndPlayCard(DecisionMaker, tacticCards, false, true, playCardActions, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                if(DidPlayCards(playCardActions))
                {
                    tacticCards.Remove(playCardActions.FirstOrDefault().CardToPlay);
                }

                foreach(Card left in tacticCards)
                {
                    var destinations = new List<MoveCardDestination>
                    {
                        new MoveCardDestination(TurnTaker.Deck),
                        new MoveCardDestination(TurnTaker.Deck, toBottom: true)
                    };
                    routine = GameController.SelectLocationAndMoveCard(DecisionMaker, left, destinations, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                }
            }
            else
            {
                routine = GameController.SendMessageAction($"There were no Tactics in {TurnTaker.Deck.GetFriendlyName()}", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }


            // {Echelon} deals 1 target 2 lightning or 2 melee damage.
            var typeStorage = new List<SelectDamageTypeDecision>();
            routine = GameController.SelectDamageType(DecisionMaker, typeStorage, new DamageType[] { DamageType.Melee, DamageType.Lightning }, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            var selectedType = GetSelectedDamageType(typeStorage) ?? DamageType.Melee;
            
            routine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), 2, selectedType, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
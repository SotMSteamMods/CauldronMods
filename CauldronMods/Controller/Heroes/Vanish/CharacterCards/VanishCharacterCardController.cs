using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Vanish
{
    public class VanishCharacterCardController : VanishSubCharacterCardController
    {
        public VanishCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private int _customAmountText = 1;
        private int _customTargetsText = 1;
        public override IEnumerator UsePower(int index = 0)
        {
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 1);

            _customAmountText = damages;
            _customTargetsText = targets;

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.Custom, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget, "target to deal damage", false), storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCard(storedResults))
            {
                Card selectedCard = base.GetSelectedCard(storedResults);
                coroutine = GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, selectedCard), damages, DamageType.Projectile, targets, false, targets, additionalCriteria: c => c != selectedCard, cardSource: GetCardSource());
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
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: base.GetCardSource());
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
                        //Reveal the bottom card of a deck, then replace it or move it to the top.
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        IEnumerator coroutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealBottomCardOfDeck, (Location l) => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectLocation(storedResults))
                        {
                            var location = GetSelectedLocation(storedResults);
                            List<Card> revealedCards = new List<Card>(); //we already know this is location.BottomCard, but the function demands
                            coroutine = GameController.RevealCards(this.DecisionMaker, location, 1, revealedCards,
                                fromBottom: true,
                                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
                                cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            var card = revealedCards.First();
                            var locations = new[]
                            {
                                new MoveCardDestination(card.NativeDeck, true, false),
                                new MoveCardDestination(card.NativeDeck, false, true)
                            };

                            coroutine = GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, locations, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;

                    }
                case 2:
                    {
                        //Select a hero target. Reduce damage dealt to that target by 1 till the start of your next turn"
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHero, "hero target", false), storedResults, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card selectedCard = GetSelectedCard(storedResults);
                            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = selectedCard;
                            reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            reduceDamageStatusEffect.UntilTargetLeavesPlay(selectedCard);

                            coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText($"Select a target to deal {_customTargetsText} { (_customTargetsText == 1 ? "target" : "targets")} {_customAmountText} projectile damage.", $"Selecting a target to deal {_customTargetsText} { (_customTargetsText == 1 ? "target" : "targets")} {_customAmountText} projectile damage.", $"Vote for a target to deal { _customTargetsText } { (_customTargetsText == 1 ? "target" : "targets")} {_customAmountText} projectile damage.", "target to deal damage");

        }
    }
}

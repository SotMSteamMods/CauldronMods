using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Vanish
{
    public class FirstResponseVanishCharacterCardController : VanishSubCharacterCardController
    {
        public FirstResponseVanishCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Reduce the next damage that would be dealt to a hero target by 2."
            int reduction = GetPowerNumeral(0, 2);

            var effect = new ReduceDamageStatusEffect(reduction);
            effect.NumberOfUses = 1;
            effect.TargetCriteria.IsHero = true;
            effect.TargetCriteria.IsTarget = true;
            effect.CardSource = Card;

            var coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player use a power now.",
                "Reveal the top card of 2 decks, then replace those cards.",
                "The next time a hero would be dealt damage, redirect that damage to a non-villain target."
             */

            switch (index)
            {
                case 0:
                    {
                        //"One player use a power now."
                        IEnumerator coroutine = base.GameController.SelectHeroToUsePower(this.DecisionMaker, cardSource: base.GetCardSource());
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
                        //"Reveal the top card of 2 decks, then replace those cards.",
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        IEnumerator coroutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        var location = GetSelectedLocation(storedResults);
                        if (location != null)
                        {
                            storedResults.Clear();
                            coroutine = RevealAndReplace(location);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            coroutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && l != location, storedResults, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            location = GetSelectedLocation(storedResults);
                            if (location != null)
                            {
                                storedResults.Clear();
                                coroutine = RevealAndReplace(location);
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
                        break;
                    }
                case 2:
                    {
                        //"The next time a hero would be dealt damage, redirect that damage to a non-villain target."
                        var effect = new RedirectDamageStatusEffect();
                        effect.NumberOfUses = 1;
                        effect.RedirectableTargets.IsVillain = false;
                        effect.RedirectableTargets.IsTarget = true;
                        effect.TargetCriteria.IsHeroCharacterCard = true;
                        effect.CardSource = Card;

                        var coroutine = base.AddStatusEffect(effect, true);
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

        private IEnumerator RevealAndReplace(Location location)
        {
            List<Card> revealedCards = new List<Card>(); //we already know this is location.BottomCard, but the function demands
            var coroutine = GameController.RevealCards(this.DecisionMaker, location, 1, revealedCards,
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

            var card = revealedCards.FirstOrDefault();
            if (card != null)
            {
                coroutine = GameController.MoveCard(this.DecisionMaker, card, location, cardSource: GetCardSource());
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
    }
}

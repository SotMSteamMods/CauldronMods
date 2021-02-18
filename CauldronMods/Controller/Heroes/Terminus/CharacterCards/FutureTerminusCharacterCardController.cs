using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class FutureTerminusCharacterCardController : TerminusBaseCharacterCardController
    {
        // Power
        // Play a card. {Terminus} deals herself 2 cold damage.
        private int ColdDamage => GetPowerNumeral(0, 2);

        public FutureTerminusCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.SetHP(base.CharacterCard, 0), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "TheLightAtTheEnd" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.SearchForCards(this.DecisionMaker, true, false, 1, 1, cardCriteria: new LinqCardCriteria((card) => "CovenantOfWrath" == card.Identifier), false, true, false, autoDecideCard: true), TriggerType.PhaseChange, TriggerTiming.After);
            //base.AddTrigger<PhaseChangeAction>((pca) => pca.FromPhase == null, (pca) => base.GameController.AddTokensToPool(base.CharacterCard.FindTokenPool("TerminusWrathPool"), 3, base.GetCardSource()), TriggerType.PhaseChange, TriggerTiming.After);
        }

        // Play a card. {Terminus} deals herself 2 cold damage.
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // Play a card
            coroutine = base.GameController.SelectAndPlayCardFromHand(DecisionMaker, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // {Terminus} deals herself 2 cold damage
            coroutine = base.GameController.DealDamageToSelf(DecisionMaker, (card) => card == base.CharacterCard, 2, DamageType.Cold, cardSource: base.GetCardSource());
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
                // One hero may use a power now.
                case 0:
                    coroutine = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;
                // Each environment target deals itself 3 cold damage.
                case 1:
                    coroutine = base.GameController.DealDamageToSelf(DecisionMaker, (card) => card.IsEnvironmentTarget, 3, DamageType.Cold, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;
                // Reveal the top 2 cards of a non-villain deck and replace them in the same order.
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

        private IEnumerator UseIncapacitatedAbility3()
        {
            // Reveal the top 2 cards of a non-villain deck and replace them in the same order.
            IEnumerator coroutine;
            List<LocationChoice> locationChoices =  base.GameController.AllTurnTakers.Where((tt) => !tt.IsVillain && !tt.IsIncapacitatedOrOutOfGame).Select((tt) => new LocationChoice(tt.Deck)).ToList();
            List<SelectLocationDecision> selectedLocationChoices = new List<SelectLocationDecision>();
            List<Card> revealedCards = new List<Card>();
            Location location;

            coroutine = base.GameController.SelectLocation(DecisionMaker, locationChoices, SelectionType.RevealCardsFromDeck, selectedLocationChoices, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectedLocationChoices != null && selectedLocationChoices.Count() > 0)
            {
                location = selectedLocationChoices.FirstOrDefault().SelectedLocation.Location;
                coroutine = base.GameController.RevealCards(base.TurnTakerController, location, 2, revealedCards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.CleanupRevealedCards(location.OwnerTurnTaker.Revealed, location);
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

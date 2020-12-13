using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

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
                    // One player may draw 2 cards,
                    
                    routine = base.GameController.SelectHeroToDrawCards(this.HeroTurnTakerController, Incap2CardsToDraw, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    // .. then discard 2 cards.
                    // TODO:


                    break;

                case 2:

                    // Move the top card of the villain deck to the bottom of the villain deck.
                    List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                    routine = FindVillainDeck(this.HeroTurnTakerController, SelectionType.MoveCard, storedResults, null);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class SwarmingProtocolCypherCharacterCardController : CypherBaseCharacterCardController
    {
        private const int PowerCardsToDraw = 1;
        private const int Incapacitate2CardsToDiscard = 3;

        public SwarmingProtocolCypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Play a card face down next to a hero. Until leaving play it gains keyword 'Augment' and text 'This hero is augmented', 

            //base.GameController.SelectAndMoveCard(this.HeroTurnTakerController, c => c.Location == this.HeroTurnTaker.Hand, new Location())

            MoveCardDestination[] destinations = FindCardsWhere(c => c.IsHeroCharacterCard && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame)
                .Select(c => new MoveCardDestination(c.NextToLocation)).ToArray();

            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();
            IEnumerator routine = base.GameController.SelectCardFromLocationAndMoveIt(this.HeroTurnTakerController, this.HeroTurnTaker.Hand,
                new LinqCardCriteria(card => true), destinations, flipFaceDown: true, storedResults: selectCardDecisions);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }


            //IEnumerator routin2e = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(c => validTargets.Criteria(c) &&
            //(additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), validTargets.Description),
            //storedResults, isPutIntoPlay, decisionSources);


            // ... draw a card.
            int drawNumeral = base.GetPowerNumeral(0, PowerCardsToDraw);

            routine = base.GameController.DrawCards(this.HeroTurnTakerController, drawNumeral, cardSource: GetCardSource());

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

                    // One player may play a card now.
                    routine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());

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

                    // Discard the top 3 cards of 1 deck.
                    routine = base.DiscardCardsFromTopOfDeck(this.HeroTurnTakerController, Incapacitate2CardsToDiscard);
                    
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;

                case 2:

                    // Destroy an environment card.
                    LinqCardCriteria criteria = new LinqCardCriteria(c => c.IsEnvironment && c.IsInPlay, "environment");
                    routine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, criteria, false, cardSource: base.GetCardSource());

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
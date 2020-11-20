using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class WetWorkCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Shuffle 2 cards from each trash back into their decks.
        // You may deal 1 target 2 melee damage.
        //==============================================================
        
        public static string Identifier = "WetWork";

        private const int CardsToMoveFromTrash = 2;
        private const int DamageAmount = 2;

        public WetWorkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Shuffle 2 cards from each trash back into their decks
            IEnumerator shuffleRoutine 
                = base.DoActionToEachTurnTakerInTurnOrder(turnTakerController => true, MoveCardToDeckResponse);

            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();
            IEnumerator selectOwnCharacterRoutine = base.SelectOwnCharacterCard(selectCardResults, SelectionType.HeroToDealDamage);


            // You may deal 1 target 2 melee damage
            Card characterCard = GetSelectedCard(selectCardResults);
            IEnumerator dealDamageRoutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(base.GameController, characterCard), DamageAmount,
                DamageType.Melee, 1, false, 0,
                additionalCriteria: c => c.IsTarget && c.IsInPlay,
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleRoutine);
                yield return base.GameController.StartCoroutine(selectOwnCharacterRoutine);
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleRoutine);
                base.GameController.ExhaustCoroutine(selectOwnCharacterRoutine);
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }

        private IEnumerator MoveCardToDeckResponse(TurnTakerController turnTakerController)
        {
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            List<MoveCardDestination> list = new List<MoveCardDestination>
            {
                new MoveCardDestination(turnTaker.Deck)
            };

            HeroTurnTakerController decisionMaker = turnTaker.IsHero ? turnTakerController.ToHero() : this.DecisionMaker;

            IEnumerator selectCardsFromLocationRoutine = base.GameController.SelectCardsFromLocationAndMoveThem(decisionMaker, turnTaker.Trash, 
                    2, CardsToMoveFromTrash, 
                    new LinqCardCriteria(c => c.Location == turnTaker.Trash, "trash"), 
                    list, shuffleAfterwards: false, cardSource: base.GetCardSource());

            IEnumerator shuffleDeckRoutine = base.GameController.ShuffleLocation(turnTaker.Deck);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCardsFromLocationRoutine);
                yield return base.GameController.StartCoroutine(shuffleDeckRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCardsFromLocationRoutine);
                base.GameController.ExhaustCoroutine(shuffleDeckRoutine);
            }
        }
    }
}
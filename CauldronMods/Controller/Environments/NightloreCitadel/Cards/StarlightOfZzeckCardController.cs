using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class StarlightOfZzeckCardController : NightloreCitadelUtilityCardController
    {
        public StarlightOfZzeckCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the villain target with the lowest HP 3 toxic damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 3, DamageType.Toxic);
            //Then, if Aethium Cannon is in play, put the top card of 1 hero deck beneath it.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, CannonInPlayResponse, TriggerType.MoveCard, additionalCriteria: (PhaseChangeAction pca) => IsAethiumCannonInPlay());
        }

        private IEnumerator CannonInPlayResponse(PhaseChangeAction arg)
        {
            //put the top card of 1 hero deck beneath it.
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = SelectDecks(DecisionMaker, 1, SelectionType.MoveCardToUnderCard, (Location loc) => loc.IsHero && !loc.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && loc.NumberOfCards > 0 && GameController.IsLocationVisibleToSource(loc, GetCardSource()), storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectDeck(storedResults))
            {
                Location deck = GetSelectedLocation(storedResults);
                Card cannon = FindAethiumCannonInPlay();
                coroutine = GameController.MoveCard(TurnTakerController, deck.TopCard, cannon.UnderLocation, doesNotEnterPlay: true, cardSource: GetCardSource());
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

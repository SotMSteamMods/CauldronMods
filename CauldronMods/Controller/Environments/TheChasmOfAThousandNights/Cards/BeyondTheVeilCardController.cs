using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class BeyondTheVeilCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public BeyondTheVeilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, cardCriteria: new LinqCardCriteria(c => c.IsTarget, "target")).Condition = () => !Card.IsInPlay;
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeDiscard);
        }
        private static readonly string FirstTimeDiscard = "FirstTimeDiscard";

        public override IEnumerator Play()
        {
            //When this card enters play, shuffle all targets from the environment trash into the environment deck.
            IEnumerator coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, FindCardsWhere((Card c) => c.IsTarget && TurnTaker.Trash.HasCard(c)), TurnTaker.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //The first time a hero card is discarded each turn, that hero deals themselves 1 psychic damage.
            AddTrigger((MoveCardAction mca) => mca.IsDiscard && mca.IsSuccessful && !HasBeenSetToTrueThisTurn(FirstTimeDiscard) && mca.CardToMove != null && IsHero(mca.CardToMove), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(MoveCardAction mca)
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDiscard);
            List<Card> storedCharacter = new List<Card>();
            IEnumerator coroutine = FindCharacterCardToTakeDamage(mca.CardToMove.Owner.ToHero(), storedCharacter, base.CharacterCard, 1, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedCharacter.FirstOrDefault();
            if (card != null)
            {
                IEnumerator coroutine2 = DealDamage(card, card, 1, DamageType.Psychic, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }
    }
}

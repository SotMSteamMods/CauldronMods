using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.CatchwaterHarbor
{
    public class TransportCardController : CatchwaterHarborUtilityCardController
    {

        public TransportCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} will be destroyed at the start of the next environment turn.").Condition = () => Card.IsInPlayAndHasGameText && AllAboardDestructionCondition;
        }

        public static readonly string AllAboardIdentifier = "AllAboard";

        public bool AllAboardDestructionCondition 
        {
            get
            {
                return GetCardPropertyJournalEntryBoolean(DestroyNextTurnKey) ?? false;
            }
        }

        public override IEnumerator Play()
        {
            var locations = new Location[]
            {
                        base.TurnTaker.Deck,
                        base.TurnTaker.Trash
            };

            //When this card enters play, search the environment deck and trash for All Aboard and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocations(locations, AllAboardIdentifier, isPutIntoPlay: true, showMessageIfFailed: false, shuffleAfterwardsIfDeck: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            IEnumerator coroutine2 = UniqueOnPlayEffect();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine2);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }

        public virtual IEnumerator UniqueOnPlayEffect() { return null; }

        public override IEnumerator ActivateAbility(string abilityKey)
		{
			IEnumerator enumerator = null;
			if (abilityKey == "travel")
			{
				enumerator = ActivateTravel();
			}
			
			if (enumerator != null)
			{
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(enumerator);
				}
				else
				{
					base.GameController.ExhaustCoroutine(enumerator);
				}
			}
		}

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, StartOfTurnResponse, TriggerType.DestroySelf, additionalCriteria: (PhaseChangeAction pca) => AllAboardDestructionCondition);
            AddAfterLeavesPlayAction(() => ResetFlagAfterLeavesPlay(DestroyNextTurnKey));

        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            SetCardProperty(DestroyNextTurnKey, false);
            IEnumerator coroutine = DestroyThisCardResponse(pca);
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

        public virtual IEnumerator ActivateTravel()
		{
			yield return null;
		}
	}
}
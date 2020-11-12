using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HtAggressionStimulantCardController : ChemicalTriggerCardController
    {
        #region Constructors

        public HtAggressionStimulantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //This card is indestructible if at least 1 Test Subject is in play. At the start of the environment turn, destroy this card.
            //In ChemicalTriggerCardController
            base.AddTriggers();

            //Increase damage dealt by Test Subjects by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && base.IsTestSubject(dd.DamageSource.Card), 1);
        }

        public override IEnumerator Play()
        {

            //When this card enters play, search the environment deck and trash for Halberd-04: Alpha and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocations(new Location[]
            {
                base.TurnTaker.Deck,
                base.TurnTaker.Trash
            }, "HalberdAlpha");
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
        #endregion Methods
    }
}
using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalcyonCleanersCardController : TestSubjectCardController
    {
        #region Constructors

        public HalcyonCleanersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //If there are no Chemical Triggers in play, increase damage dealt by this card to Test Subjects by 5

            //criteria for damage dealt by this card to test subjects when no chemical trigger is in play
            Func<DealDamageAction, bool> increaseCriteria = (DealDamageAction dd) => !base.IsChemicalTriggerInPlay() && dd.DamageSource != null && dd.DamageSource.Card == base.Card && base.IsTestSubject(dd.Target);
            //increase damage dealt that matches the above criteria by 5
            base.AddIncreaseDamageTrigger(increaseCriteria, (DealDamageAction dd) => 5);

            //At the end of the environment turn, this card deals the target other than itself with the second lowest HP {H} fire damage. 
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsTarget && c != base.Card, TargetType.LowestHP, base.H, DamageType.Fire, highestLowestRanking: 2);
            
            //Whenever this card destroys a Test Subject, play the top card of the environment deck.
            //criteria for this card destroying a test subject
            Func<DestroyCardAction, bool> destroyCriteria = (DestroyCardAction destroy) => destroy.CardToDestroy != null && base.IsTestSubject(destroy.CardToDestroy.Card)&& destroy.CardSource != null && destroy.CardSource.Card == base.Card;
            //When a card is destroyed that matches the above criteria, play the top card of the environment deck
            base.AddTrigger<DestroyCardAction>(destroyCriteria, new Func<DestroyCardAction, IEnumerator>(base.PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse), TriggerType.PlayCard, TriggerTiming.After);
        }
        #endregion Methods
    }
}
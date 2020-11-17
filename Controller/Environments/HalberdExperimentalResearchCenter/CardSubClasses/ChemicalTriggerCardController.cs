using System;
using System.Collections;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class ChemicalTriggerCardController : CardController
    {
        #region Constructors

        public ChemicalTriggerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //this card is indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            //show the number of Test Subjects in play
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => this.IsTestSubject(c), "test subject"));
        }

        #endregion Constructors

        #region Methods
        public override bool AskIfCardIsIndestructible(Card card)
        {
            return base.FindCardsWhere((Card c) => this.IsTestSubject(c) && c.IsInPlayAndHasGameText).Count<Card>() > 0 && card == base.Card;
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf, null, false);
        }

        protected bool IsTestSubject(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "test subject", false, false);
        }
        #endregion Methods
    }
}
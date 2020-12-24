using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class TestSubjectCardController : CardController
    {
        #region Constructors

        public TestSubjectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => IsChemicalTriggerInPlay(), () => "A chemical trigger is in play.", () => "A chemical trigger is not in play.");
        }

        #endregion Constructors

        #region Methods
        protected bool IsChemicalTrigger(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "chemical trigger");
        }

        protected bool IsChemicalTriggerInPlay()
        {
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsChemicalTrigger(c)).Count<Card>() > 0;
        }

        protected bool IsTestSubject(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "test subject");
        }
        #endregion Methods
    }
}
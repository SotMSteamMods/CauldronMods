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

        }

        #endregion Constructors

        #region Methods
        protected bool IsChemicalTrigger(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "chemical trigger", false, false);
        }

        protected bool IsChemicalTriggerInPlay()
        {
            return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsChemicalTrigger(c), false, null, false).Count<Card>() > 0;
        }

        protected bool IsTestSubject(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "test subject", false, false);
        }
        #endregion Methods
    }
}
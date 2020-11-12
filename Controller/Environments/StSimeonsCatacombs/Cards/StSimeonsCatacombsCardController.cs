using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class StSimeonsCatacombsCardController : CardController
    {
        #region Constructors

        public StSimeonsCatacombsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsUnderCard && c.Location == base.Card.UnderLocation, "under this card", false, true));
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

		#endregion Constructors

		#region Methods
		public override void AddTriggers()
		{
			this.AddSideTriggers();
		}

		public void AddSideTriggers()
		{
			//on  front side
			if (!base.Card.IsFlipped)
			{
					
			}
			else
			{
				//remove the environment cards can't be played status effect
				((StSimeonsCatacombsTurnTakerController)base.TurnTakerController).RemoveSideEffects();
			}
		}


		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}
		#region SideTrigger Functions
		//taken from CharacterCardController
		public override IEnumerator AfterFlipCardImmediateResponse()
		{
			this.RemoveSideTriggers();
			this.AddSideTriggers();
			yield return null;
			yield break;
		}


		public void RemoveSideTriggers()
		{
			foreach (ITrigger trigger in this.SideTriggers)
			{
				base.RemoveTrigger(trigger);
			}
			this.SideTriggers.Clear();
		}

		public override bool HasFlippedSide
		{
			get
			{
				return true;
			}
		}

		protected void AddSideTrigger(ITrigger trigger)
		{
			this.SideTriggers.Add(trigger);
		}

		protected void AddSideTriggers(IEnumerable<ITrigger> triggers)
		{
			foreach (ITrigger trigger in triggers)
			{
				this.AddSideTrigger(trigger);
			}
		}

		protected List<ITrigger> SideTriggers = new List<ITrigger>();
        #endregion
        #endregion Methods
    }
}
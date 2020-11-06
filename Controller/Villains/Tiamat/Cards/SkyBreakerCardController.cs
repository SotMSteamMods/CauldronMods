using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class SkyBreakerCardController : CardController
    {
        #region Constructors

        public SkyBreakerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods
		public override void AddTriggers()
		{
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine;
            IEnumerable<Card> heads = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsTarget));
            Card highestHPHead = heads.FirstOrDefault();
            int highestHP = Convert.ToInt32(heads.FirstOrDefault().HitPoints);

            //The Head with the highest HP...
            if (base.IsLowestHitPointsUnique((Card c) => c.DoKeywordsContain("head") && c.IsTarget))
            {
                foreach (Card head in heads)
                {
                    if (head.HitPoints <= highestHPHead.HitPoints)
                    {
                        highestHPHead = head;
                    }
                }
            }
            else
            {
                foreach (Card head in heads)
                {
                    if (head.HitPoints <= highestHPHead.HitPoints)
                    {
                        highestHP = Convert.ToInt32(head.HitPoints);
                    }
                }
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTarget, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsTarget && c.HitPoints == highestHP), storedResults, false, false, null, true, null);
                highestHPHead = storedResults.FirstOrDefault().SelectedCard;
            }

            //...deals each hero target {H + 2} infernal damage.
            coroutine = base.DealDamage(base.Card, (Card card) => card.IsHero, base.Game.H + 2, DamageType.Infernal, false, false, null, null, null, false, null, null, false, false);
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
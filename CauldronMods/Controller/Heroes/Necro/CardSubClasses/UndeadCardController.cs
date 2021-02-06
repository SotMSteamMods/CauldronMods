using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using Handelabra;

namespace Cauldron.Necro
{
    public abstract class UndeadCardController : NecroCardController
    {
        protected UndeadCardController(Card card, TurnTakerController turnTakerController, int baseHP) : base(card, turnTakerController)
        {
            this.BaseHP = baseHP;

            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsRitual(c) && c.IsInPlayAndHasGameText, "ritual"), null, new[] { TurnTaker });
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine = base.GameController.ChangeMaximumHP(base.Card, BaseHP + GetNumberOfRitualsInPlay(), true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            coroutine = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected int BaseHP { get; }

        protected string BuildUndeadSpecialString(bool highestOrLowest, int ranking = 1, Func<int> numberOfTargets = null, LinqCardCriteria cardCriteria = null)
        {
             
            return HighestOrLowest(highest: highestOrLowest, ranking, numberOfTargets, cardCriteria, null, false);

        }

		private string HighestOrLowest(bool highest, int ranking = 1, Func<int> numberOfTargets = null, LinqCardCriteria cardCriteria = null, BattleZone battleZone = null, bool ignoreBattleZone = false)
		{
			string result = null;
			int num = 1;
			if (numberOfTargets != null)
			{
				num = numberOfTargets();
			}
			if (num > 0)
			{
				string text = (highest ? "highest" : "lowest");
				Func<Card, bool> additionalCriteria = null;
				string text2 = "";
				if (battleZone != null)
				{
					ignoreBattleZone = true;
					additionalCriteria = ((cardCriteria == null) ? ((Func<Card, bool>)((Card c) => c.BattleZone == battleZone)) : ((Func<Card, bool>)((Card c) => cardCriteria.Criteria(c) && c.BattleZone == battleZone)));
					text2 = " in the " + battleZone.Name;
				}
				else if (cardCriteria != null)
				{
					additionalCriteria = cardCriteria.Criteria;
				}
				IEnumerable<Card> source = ((!highest) ? GameController.FindAllTargetsWithLowestHitPoints(ranking, additionalCriteria, GetCardSource(), num, ignoreBattleZone) : GameController.FindAllTargetsWithHighestHitPoints(ranking, additionalCriteria, GetCardSource(), num, ignoreBattleZone));
				string text3 = "";
				if (ranking > 1)
				{
					text3 = ranking.ToOrdinalString().ToLower() + " ";
				}
				if (source.Count() > 0)
				{
					string text4 = source.Count().ToString_TargetOrTargets();
					if (cardCriteria != null)
					{
						text4 = cardCriteria.GetDescription(source.Count());
					}
					string text5 = "";
					if (num > 1)
					{
						text5 = num + " ";
					}
					else
					{
						text4 = text4.Capitalize();
					}
					result = string.Format("{4}{0} with the {1}{2} HP{5}: {3}.", text4, text3, text, source.Select((Card c) => c.AlternateTitleOrTitle).ToRecursiveString(), text5, text2);
				}
				else
				{
					string text6 = num.ToString_TargetOrTargets();
					string text7 = num.ToString_IsOrAre();
					if (cardCriteria != null)
					{
						text6 = cardCriteria.GetDescription(source.Count());
						text7 = source.Count().ToString_IsOrAre();
					}
					result = $"There {text7} no {text6} with the {text3}{text} HP{text2}.";
				}
			}
			return result;
		}

	}
}

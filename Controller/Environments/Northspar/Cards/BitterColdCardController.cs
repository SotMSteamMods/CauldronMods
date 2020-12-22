using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class BitterColdCardController : NorthsparCardController
    {

        public BitterColdCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(ranking: 3);
            SpecialStringMaker.ShowSpecialString(() => BuildTargetsWhoWereDealtColdDamageThisTurnSpecialString());
        }

        public override void AddTriggers()
        {
            //Whenever an environment card enters play, this card deals the target with the third highest HP 2 cold damage.
            Func<CardEntersPlayAction, bool> criteria = (CardEntersPlayAction cp) => cp.CardEnteringPlay != null && cp.CardEnteringPlay.IsEnvironment;

            base.AddTrigger<CardEntersPlayAction>(criteria, this.EnterPlayResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of each turn, all targets that were dealt cold damage during that turn deal themselves 1 psychic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => true, this.EndOfTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //all targets that were dealt cold damage during that turn deal themselves 1 psychic damage
            List<Card> targets = this.GetTargetsWhoWereDealtColdDamageThisTurn();
            IEnumerator coroutine;
            foreach (Card target in targets)
            {
                coroutine = base.DealDamage(target, target, 1, DamageType.Psychic, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        private IEnumerator EnterPlayResponse(CardEntersPlayAction cp)
        {
            //this card deals the target with the third highest HP 2 cold damage

            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 3, (Card c) => c.IsTarget, (Card c) => 2, DamageType.Cold);
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

        private List<Card> GetTargetsWhoWereDealtColdDamageThisTurn()
        {
            return base.GameController.Game.Journal.DealDamageEntriesThisTurn()
                        .Where(e => e.TargetCard.IsTarget && e.DamageType == DamageType.Cold && e.TargetCard.IsInPlayAndHasGameText)
                        .Select(e => e.TargetCard)
                        .ToList();
        }

        private string BuildTargetsWhoWereDealtColdDamageThisTurnSpecialString()
        {
            var targetList = GetTargetsWhoWereDealtColdDamageThisTurn();
            string targetListSpecial = "Targets who were dealt cold damage this turn: ";
            if (targetList.Any())
            {
                targetListSpecial += string.Join(", ", targetList.Select(c => c.AlternateTitleOrTitle).ToArray());
            }
            else
            {
                targetListSpecial += "None";
            }
            return targetListSpecial;
        }
    }
}
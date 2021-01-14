using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class TheAncientsStirCardController : VaultFiveUtilityCardController
    {
        public TheAncientsStirCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"Damage dealt by {Card.Title} is increased by {GetCardPropertyJournalEntryInteger(DamageIncreasedBy) ?? 0}.").Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHighestHP(ranking: 2);
        }

        private static readonly string DamageIncreasedBy = "DamageIncreasedBy";

        public override void AddTriggers()
        {
            //Whenever an Artifact card enters play, increase damage dealt by this card by 1.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && IsArtifact(cpa.CardEnteringPlay), IncreaseDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.After);

            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null & dd.DamageSource.IsSameCard(Card), (DealDamageAction dd) => GetCardPropertyJournalEntryInteger(DamageIncreasedBy) ?? 0);

            //At the end of the environment turn, this card deals the target with the second-highest HP 1 infernal damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 1, DamageType.Infernal, highestLowestRanking: 2);
        }

        private IEnumerator IncreaseDamageResponse(CardEntersPlayAction arg)
        {
            this.IncrementCardProperty(DamageIncreasedBy);
            IEnumerator coroutine = GameController.SendMessageAction($"An artifact has entered play! Increasing damage dealt by {Card.Title} by 1.", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}

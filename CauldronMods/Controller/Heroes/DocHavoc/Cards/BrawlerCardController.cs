using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class BrawlerCardController : CardController
    {
        //==============================================================
        // One non-hero target deals {DocHavoc} 4 melee damage.
        // Then {DocHavoc} deals that target X melee damage,
        // where X is the amount of damage that target dealt {DocHavoc} this turn.
        //==============================================================

        public static readonly string Identifier = "Brawler";

        private const int DamageAmountToNonHeroTarget = 4;

        public BrawlerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

            IEnumerator selectCardRoutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.SelectTargetNoDamage,
                new LinqCardCriteria((Card c) => c.IsTarget && !IsHeroTarget(c) && c.IsInPlayAndHasGameText, "non-hero targets in play"),
                storedResults, false, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectCardRoutine);
            }

            Card selectedCard = (from d in storedResults where d.Completed select d.SelectedCard).FirstOrDefault<Card>();
            if (selectedCard == null)
            {
                yield break;
            }


            int damage = this.GetPowerNumeral(0, DamageAmountToNonHeroTarget);

            IEnumerator dealDamageRoutine = base.GameController.DealDamageToTarget(new DamageSource(this.GameController, selectedCard), base.CharacterCard,
                damage, DamageType.Melee, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }

            // Get damage amount dealt to Doc Havoc by selected target this turn
            int damageDealtToDocHavocByTargetThisTurn = GetDamageDealtToDocHavocByTargetThisTurn(selectedCard, base.CharacterCard);


            Console.WriteLine($"Damage dealt to Doc Havoc this turn by {selectedCard.Identifier}: {damageDealtToDocHavocByTargetThisTurn}");

            IEnumerator dealDamageRoutine2 = base.GameController.DealDamageToTarget(
                new DamageSource(this.GameController, this.CharacterCard), selectedCard,
                damageDealtToDocHavocByTargetThisTurn, DamageType.Melee, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine2);
            }
        }

        private int GetDamageDealtToDocHavocByTargetThisTurn(Card source, Card target)
        {
            int result = 0;
            try
            {
                result = base.GameController.Game.Journal.DealDamageEntriesFromCardToCardThisTurn(
                    source, target).Select(d => d.Amount).Sum();
            }
            catch (OverflowException ex)
            {
                Log.Warning("GetDamageDealtToDocHavocByTargetThisTurn overflowed: " + ex.Message);
                result = int.MaxValue;
            }
            return result;
        }
    }
}

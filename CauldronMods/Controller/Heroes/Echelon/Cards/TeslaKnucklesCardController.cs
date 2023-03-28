using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class TeslaKnucklesCardController : EchelonBaseCardController
    {
        //==============================================================
        //"At the end of your turn, {Echelon} may deal 1 target X lightning damage, where X is the number of Tactics destroyed during your turn."
        //Power: "{Echelon} deals each non-hero target 1 lightning damage."
        //==============================================================

        public static string Identifier = "TeslaKnuckles";

        public TeslaKnucklesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsDestroyedThisTurnEx(new LinqCardCriteria(IsTactic, "tactic")).Condition = () => GameController.ActiveTurnTaker == TurnTaker;
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, {Echelon} may deal 1 target X lightning damage, where X is the number of Tactics destroyed during your turn."
            Func<Card, int?> dynamicAmount = (Card c) => GetNumberOfTacticsDestroyedThisTurn();
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, (Card c) => true, TargetType.SelectTarget, 0, DamageType.Lightning, isIrreducible: false, optional: true, numberOfTargets: 1, dynamicAmount: dynamicAmount);
        }

        private int GetNumberOfTacticsDestroyedThisTurn()
        {
            return Journal.DestroyCardEntriesThisTurn().Where((DestroyCardJournalEntry dcj) => dcj.DidCardHaveKeyword("tactic")).Count();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numDamage = GetPowerNumeral(0, 1);

            //Power: "{Echelon} deals each non-hero target 1 lightning damage."
            IEnumerator coroutine = DealDamage(CharacterCard, (Card c) => !IsHero(c), numDamage, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
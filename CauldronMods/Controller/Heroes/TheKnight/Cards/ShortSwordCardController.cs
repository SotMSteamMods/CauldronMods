using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class ShortSwordCardController : SingleHandEquipmentCardController
    {
        public ShortSwordCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this._useSpecialAssignment = true;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine;
            if (IsMultiCharPromo())
            {
                List<SelectCardDecision> selectedKnight = new List<SelectCardDecision> { };
                coroutine = SelectOwnCharacterCard(selectedKnight, SelectionType.HeroCharacterCard);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidSelectCard(selectedKnight))
                {
                    AddCardPropertyJournalEntry(RoninKey, selectedKnight.FirstOrDefault().SelectedCard);
                }
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
            
            yield break;
        }
        public override void AddTriggers()
        {
            //"Increase damage dealt by {TheKnight} by 1."
            base.AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.Card != null && IsEquipmentEffectingCard(dd.DamageSource.Card), 1);
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheKnight} deals 1 target 2 melee damage."
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 2);
            IEnumerator coroutine;

            //List<SelectCardDecision> results = new List<SelectCardDecision>();
            //coroutine = base.SelectOwnCharacterCard(results, SelectionType.HeroToDealDamage);
            //if (base.UseUnityCoroutines)
            //{
            //    yield return base.GameController.StartCoroutine(coroutine);
            //}
            //else
            //{
            //    base.GameController.ExhaustCoroutine(coroutine);
            //}
            //Card card = GetSelectedCard(results);
            Card card = GetKnightCardUser(this.Card);
            coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, card), damages, DamageType.Melee, targets, false, targets, cardSource: base.GetCardSource());
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
    }
}

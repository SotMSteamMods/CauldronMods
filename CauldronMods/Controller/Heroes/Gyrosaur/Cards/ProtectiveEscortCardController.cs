using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class ProtectiveEscortCardController : GyrosaurUtilityCardController
    {
        public ProtectiveEscortCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Draw 2 cards.",
            IEnumerator coroutine = DrawCards(DecisionMaker, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"Select a target and a damage type. 
            var storedCard = new List<SelectCardDecision>();
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedCard, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedCard))
            {
                var storedType = new List<SelectDamageTypeDecision>();
                coroutine = GameController.SelectDamageType(DecisionMaker, storedType, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(storedType.FirstOrDefault()?.SelectedDamageType != null)
                {
                    //That target is immune to that damage type until the start of your next turn.

                    Card chosenCard = GetSelectedCard(storedCard);
                    DamageType chosenType = storedType.FirstOrDefault().SelectedDamageType ?? DamageType.Melee;

                    var immuneEffect = new ImmuneToDamageStatusEffect();
                    immuneEffect.TargetCriteria.IsSpecificCard = chosenCard;
                    immuneEffect.DamageTypeCriteria.AddType(chosenType);
                    immuneEffect.UntilTargetLeavesPlay(chosenCard);
                    immuneEffect.UntilStartOfNextTurn(TurnTaker);
                    immuneEffect.CardSource = this.Card;

                    coroutine = AddStatusEffect(immuneEffect);
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
            yield break;
        }
    }
}

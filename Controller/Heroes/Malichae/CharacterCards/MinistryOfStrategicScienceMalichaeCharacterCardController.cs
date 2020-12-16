using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class MinistryOfStrategicScienceMalichaeCharacterCardController : HeroCharacterCardController
    {
        public MinistryOfStrategicScienceMalichaeCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DiscardCardAction> results = new List<DiscardCardAction>();
            //"Discard a card. Use that card's power, replacing any djinn's name with {Malichae}. Draw a card."
            var coroutine = base.SelectAndDiscardCards(DecisionMaker, 1,
                                requiredDecisions: 1,
                                storedResults: results,
                                selectionType: SelectionType.DiscardCard);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(results, 1))
            {
                var discarded = results.First().CardToDiscard;
                //cases:
                //1: Card is DjinnOnging, get GrantedPower, execute
                //2: Card has powers and I'm the owner, UsePowerOnOtherCard
                //3: Card has no powers, proceed

                var cc = FindCardController(discarded);
                if (cc is DjinnOngoingController djinn)
                {
                    var pwr = djinn.GetGrantedPower(this);
                    coroutine = GameController.UsePower(pwr, true, DecisionMaker, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else if (discarded.HasPowers && discarded.Owner == TurnTaker)
                {
                    coroutine = base.UsePowerOnOtherCard(discarded);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else if (discarded.HasPowers)
                {
                    coroutine = GameController.SendMessageAction("Congratulations! You've found a way to discard a card with a power that you don't own! You should open a issue for this!", Priority.High, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    coroutine = GameController.SendMessageAction($"{discarded.Title} does not have a power that can be used.", Priority.Medium, GetCardSource());
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

            coroutine = DrawCard(DecisionMaker.HeroTurnTaker);
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player may use a power now.",
			 * "2 targets regain 1 HP each.",
			 * "Until the start of your next turn, whenever a hero would be dealt exactly 1 damage, prevent that damage."
             */

            switch (index)
            {
                case 0:
                    {
                        IEnumerator drawCardRoutine = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(drawCardRoutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(drawCardRoutine);
                        }
                        break;
                    }
                case 1:
                    {
                        var coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 1, false, numberOfTargets: 2, requiredDecisions: 2, allowAutoDecide: true, cardSource: GetCardSource());
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
                case 2:
                    {
                        var effect = new ImmuneToDamageStatusEffect();
                        effect.DamageAmountCriteria.EqualTo = 1;
                        effect.TargetCriteria.IsHero = true;
                        effect.UntilStartOfNextTurn(TurnTaker);

                        var coroutine = AddStatusEffect(effect, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}

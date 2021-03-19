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

        private Card CardUsingPower;
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
                //1: Card is DjinnOnging, get GrantedPower, add associated source, execute
                //2: Card has powers and I'm the owner, UsePowerOnOtherCard
                //3: Card has no powers, proceed

                var cc = FindCardController(discarded);
                if (cc is DjinnOngoingController djinn)
                {
                    var pwr = djinn.GetGrantedPower(this);
                    var cs = GetCardSource();
                    cc.AddAssociatedCardSource(cs);
                    coroutine = GameController.UsePower(pwr, true, DecisionMaker, cs);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    cc.RemoveAssociatedCardSource(cs);
                }
                else if (discarded.HasPowers && discarded.Owner == TurnTaker)
                {
                    //stolen from cherenkov drive
                    //has the same limitations as cherenkov drive
                    HeroTurnTakerController heroTTC = FindHeroTurnTakerController(discarded.Owner.ToHero());
                    TurnTaker tt = discarded.Owner;
                    if (!GameController.CanUsePowers(heroTTC, GetCardSource()))
                    {
                        coroutine = GameController.SendMessageAction($"{Card.Title} would allow {tt.Name} to use a power on {discarded.Title}, but they cannot currently use powers.", Priority.High, GetCardSource());
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

                    var controller = FindCardController(discarded);

                    var powersOnCard = new List<Power>();
                    for (int i = 0; i < discarded.NumberOfPowers; i++)
                    {
                        powersOnCard.Add(new Power(heroTTC, controller, controller.Card.CurrentPowers.ElementAt(i), controller.UsePower(i), i, null, controller.GetCardSource()));
                    }

                    //select the power
                    var powerDecision = new UsePowerDecision(GameController, heroTTC, powersOnCard, true, cardSource: GetCardSource());
                    coroutine = GameController.MakeDecisionAction(powerDecision);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    if (powerDecision.SelectedPower == null)
                    {
                        yield break;
                    }

                    //if they picked one, prep the card to do stuff
                    bool wasOnList = GameController.IsInCardControllerList(discarded, CardControllerListType.CanCauseDamageOutOfPlay);
                    GameController.AddInhibitorException(controller, (GameAction ga) => ga != null && ga.CardSource != null && ga.CardSource.Card == discarded);
                    if (!wasOnList)
                    {
                        GameController.AddCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay, controller);
                    }
                    CardUsingPower = discarded;


                    //use the power
                    coroutine = GameController.UsePower(powerDecision.SelectedPower, heroUsingPower: heroTTC, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    //clean up the prepwork
                    if (!wasOnList)
                    {
                        GameController.RemoveCardControllerFromList(CardControllerListType.CanCauseDamageOutOfPlay, controller);
                    }
                    GameController.RemoveInhibitorException(controller);
                    CardUsingPower = null;

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

            coroutine = DrawCard();
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
                        effect.TargetCriteria.IsHeroCharacterCard = true;
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

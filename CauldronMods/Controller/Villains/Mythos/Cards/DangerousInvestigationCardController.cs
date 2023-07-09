using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class DangerousInvestigationCardController : MythosUtilityCardController
    {
        public DangerousInvestigationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowTokenPool(DangerousInvestigationPool);
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: base.Game.H - this.NumberOfCardsChosenThisTurn());

        }

        private TokenPool DangerousInvestigationPool
        {
            get
            {
                return this.Card.FindTokenPool(DangerousInvestigationPoolName);
            }
        }

        private const string FirstCardPlay = "FirstCardPlay";
        private const string SecondCardPlay = "SecondCardPlay";
        private const string ThirdCardPlay = "ThirdCardPlay";
        private const string FourthCardPlay = "FourthCardPlay";
        private const string FifthCardPlay = "FifthCardPlay";
        private const string DangerousInvestigationPoolName = "DangerousInvestigationPool";

        public override void AddTriggers()
        {
            //{MythosClue} At the end of the villain turn, the players may play the top card of the villain deck to add a token to this card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.ClueResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.AddTokensToPool }, (PhaseChangeAction action) => base.IsTopCardMatching(MythosClueDeckIdentifier));
            //At the end of the villain turn, {Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
            base.AddTrigger<MakeDecisionAction>((MakeDecisionAction action) => IsPlayTopCardDecision(action), this.DecisionResponse, TriggerType.AddTokensToPool, TriggerTiming.After);
        }

        private bool IsPlayTopCardDecision(MakeDecisionAction action)
        {
            if(action.Decision.SelectionType == SelectionType.PlayTopCardOfVillainDeck)
            {
                return action.Decision is YesNoDecision yesNo && yesNo.Answer == true;
            }
            if(action.Decision.SelectionType == SelectionType.Custom && action.CardSource != null && (action.CardSource.Card == this.Card || action.CardSource.Card == this.CharacterCard))
            {
                return action.Decision is YesNoDecision yesNo && yesNo.Answer == true;
            }
            return false;
        }

        private IEnumerator DecisionResponse(MakeDecisionAction action)
        {
            //First Card Played
            if (!base.HasBeenSetToTrueThisTurn(FirstCardPlay))
            {
                base.SetCardPropertyToTrueIfRealAction(FirstCardPlay);
            } //Second
            else if (!base.HasBeenSetToTrueThisTurn(SecondCardPlay))
            {
                base.SetCardPropertyToTrueIfRealAction(SecondCardPlay);
            } //Third
            else if (!base.HasBeenSetToTrueThisTurn(ThirdCardPlay))
            {
                base.SetCardPropertyToTrueIfRealAction(ThirdCardPlay);
            } //Fourth
            else if (!base.HasBeenSetToTrueThisTurn(FourthCardPlay))
            {
                base.SetCardPropertyToTrueIfRealAction(FourthCardPlay);
            } //Fifth
            else if (!base.HasBeenSetToTrueThisTurn(FifthCardPlay))
            {
                base.SetCardPropertyToTrueIfRealAction(FifthCardPlay);
            }

            yield break;
        }

        private IEnumerator ClueResponse(PhaseChangeAction action)
        {
            //...the players may play the top card of the villain deck to add a token to this card.
            List<Card> playedCards = new List<Card>();
            var yesNo = new YesNoAmountDecision(GameController, DecisionMaker, SelectionType.Custom, 1, requireUnanimous: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.MakeDecisionAction(yesNo);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (DidPlayerAnswerYes(yesNo))
            {
                coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, false, 1, playedCards: playedCards, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (playedCards.Any())
            {
                coroutine = base.GameController.AddTokensToPool(this.DangerousInvestigationPool, 1, base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Mythos} deals the X hero targets with the highest HP 3 infernal damage each, where X is {H} minus the number of villain cards the players chose to play this turn.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHero(c) && c.IsInPlayAndHasGameText, (Card c) => 3, DamageType.Infernal, numberOfTargets: () => base.Game.H - this.NumberOfCardsChosenThisTurn());
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

        private int NumberOfCardsChosenThisTurn()
        {
            if (base.HasBeenSetToTrueThisTurn(FifthCardPlay))
            {
                return 5;
            }
            else if (base.HasBeenSetToTrueThisTurn(FourthCardPlay))
            {
                return 4;
            }
            else if (base.HasBeenSetToTrueThisTurn(ThirdCardPlay))
            {
                return 3;
            }
            else if (base.HasBeenSetToTrueThisTurn(SecondCardPlay))
            {
                return 2;
            }
            else if (base.HasBeenSetToTrueThisTurn(FirstCardPlay))
            {
                return 1;
            }
            return 0;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("Do you want to add a token to Dangerous Investigation by playing the top card of the villain deck?",
                            "Selecting whether to play the top card of the villain deck",
                            "Vote for whether to add a token to Dangerous Investigation by playing the top card of the villain deck.",
                            "Add a token to Dangerous Investigation");
        }
    }
}

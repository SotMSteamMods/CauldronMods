using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
    public class SeasonsOfChangeLadyOfTheWoodCharacterCardController : LadyOfTheWoodUtilityCharacterCardController
    {
        public SeasonsOfChangeLadyOfTheWoodCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private const int CHOOSE_TURNTAKER = 0;
        private const int DISCARD_HAND_TO_DRAW = 1;
        private int customDecisionFlag;

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard a card. 
            IEnumerator coroutine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, responsibleTurnTaker: base.TurnTaker, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may play a card.

            coroutine = SelectAndPlayCardFromHand(base.HeroTurnTakerController);
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
            switch (index)
            {
                case 0:
                    {
                        //One target regains 1 HP.
                        IEnumerator coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 1, cardSource: GetCardSource());
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
                case 1:
                    {
                        //One player may discard their hand and draw the same number of cards.
                        customDecisionFlag = CHOOSE_TURNTAKER;
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, false, false, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        TurnTaker turnTaker = (from d in storedResults
                                               where d.Completed
                                               select d.SelectedTurnTaker).FirstOrDefault();
                        if (turnTaker != null && turnTaker.IsHero)
                        {
                            coroutine2 = DiscardAndDrawResponse(turnTaker);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                            }
                        }
                        yield break;
                    }
                case 2:
                    {
                        //All damage dealt is irreducible until the start of your turn."
                        MakeDamageIrreducibleStatusEffect effect = new MakeDamageIrreducibleStatusEffect();
                        effect.UntilStartOfNextTurn(base.TurnTaker);
                        IEnumerator coroutine3 = AddStatusEffect(effect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        yield break;
                    }
            }

            yield break;
        }

        private IEnumerator DiscardAndDrawResponse(TurnTaker turnTaker)
        {
            HeroTurnTakerController heroTurnTakerController = base.FindHeroTurnTakerController(turnTaker.ToHero());

            //ask the selected player if they want to discard and draw
            customDecisionFlag = DISCARD_HAND_TO_DRAW;
            YesNoDecision yesNo = new YesNoDecision(base.GameController, heroTurnTakerController, SelectionType.Custom, cardSource: GetCardSource());
            IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (!DidPlayerAnswerYes(yesNo))
            {
                yield break;
            }

            //discard hand
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            coroutine = base.GameController.DiscardHand(heroTurnTakerController, false, storedResults, turnTaker, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //draw same number of cards
            int numberOfCardsDiscarded = base.GetNumberOfCardsDiscarded(storedResults);
            if (numberOfCardsDiscarded > 0)
            {
                coroutine = base.DrawCards(heroTurnTakerController, numberOfCardsDiscarded);
            }
            else
            {
                coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard any cards, so no cards will be drawn.", Priority.High, base.GetCardSource());
            }
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (customDecisionFlag == CHOOSE_TURNTAKER)
            {
                return new CustomDecisionText("Select a player to discard their hand and draw the same number of cards.", 
                    "selecting a player to discard their hand and draw the same number of cards...", "Vote for a player to discard their hand and draw the same number of cards.", "player to discard hand and draw that many cards");

            }

            if (customDecisionFlag == DISCARD_HAND_TO_DRAW)
            {
                return new CustomDecisionText("Do you want to discard your hand and draw the same number of cards?", "choosing whether to discard their hand and draw the same number of cards...", "Vote for whether {0} should discard their hand and draw the same number of cards?", "discard hand and draw the same number of cards");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;

namespace Cauldron.Necro
{
    public class PastNecroCharacterCardController : HeroCharacterCardController
    {
        public static readonly string PastNecroPowerKey = "HeroVillainFlipped";

        private const int CHOOSE_TURNTAKER = 0;
        private const int DISCARD_HAND_TO_DRAW = 1;
        private int customDecisionFlag;

        public PastNecroCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialString ss = base.SpecialStringMaker.ShowIfElseSpecialString(() => true, () => "Replacing the word “Hero” on " + base.Card.Title + "'s cards with “Villain”, and vice versa", () => "", showInEffectsList: () => true);
            ss.Condition = () => GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) == true;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            SetCardProperty(PastNecroPowerKey, true);
            IEnumerator coroutine = GameController.SendMessageAction("Applying ongoing effect: Replace the word “Hero” on " + base.Card.Title + "'s cards with “Villain” and vice versa.", Priority.High, GetCardSource(), showCardSource: true);
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
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, TurnOffPowerResponse, TriggerType.Hidden, additionalCriteria: (PhaseChangeAction pca) => GameController.GetCardPropertyJournalEntryBoolean(this.CharacterCard, PastNecroPowerKey) == true);
        }

        private IEnumerator TurnOffPowerResponse(PhaseChangeAction arg)
        {
            SetCardProperty(PastNecroPowerKey, false);
            IEnumerator coroutine = GameController.SendMessageAction("Expiring: Replace the word “Hero” on " + base.Card.Title + "'s cards with “Villain” and vice versa.", Priority.High, GetCardSource(), showCardSource: true);
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
                        //One hero target deals itself 3 toxic damage.
                        IEnumerable<Card> choices = FindCardsWhere((Card c) => c.IsInPlayAndNotUnderCard && IsHeroTarget(c));
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        var damageInfo = new DealDamageAction(GetCardSource(), new DamageSource(GameController, TurnTaker), null, 3, DamageType.Toxic);
                        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DealDamageSelf, choices, storedResults,
                            optional: false,
                            dealDamageInfo: new[] { damageInfo },
                            cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card card = GetSelectedCard(storedResults);
                            coroutine = DealDamage(card, card, 3, DamageType.Toxic);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        //One hero may use their innate power, then draw a card.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.UsePowerOnCard, false, false, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        if (DidSelectTurnTaker(storedResults))
                        {
                            TurnTaker tt = GetSelectedTurnTaker(storedResults);
                            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
                            coroutine2 = base.GameController.SelectAndUsePower(httc, powerCriteria: (Power p) => p.CardController == httc.CharacterCardController, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                            }

                            coroutine2 = base.DrawCards(httc, 1);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                            }
                        }

                        break;
                    }
                case 2:
                    {
                        customDecisionFlag = CHOOSE_TURNTAKER;
                        //One player may discard their hand and draw that many cards.
                        List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                        IEnumerator coroutine3 = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.Custom, false, false, storedResults, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        TurnTaker turnTaker = (from d in storedResults
                                               where d.Completed
                                               select d.SelectedTurnTaker).FirstOrDefault();
                        if (turnTaker != null && IsHero(turnTaker))
                        {
                            coroutine3 = DiscardAndDrawResponse(turnTaker);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine3);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine3);
                            }
                        }
                        break;
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
            if(customDecisionFlag == CHOOSE_TURNTAKER)
            {
                return new CustomDecisionText("Select a player to discard their hand and draw the same number of cards.", "selecting a player to discard their hand and draw the same number of cards...", "Vote for a player to discard their hand and draw the same number of cards.", "player to discard hand and draw that many cards");

            }

            if (customDecisionFlag == DISCARD_HAND_TO_DRAW)
            {
                return new CustomDecisionText("Do you want to discard your hand and draw the same number of cards?", "choosing whether to discard their hand and draw the same number of cards...", "Vote for whether {0} should discard their hand and draw the same number of cards.", "discard hand and draw the same number of cards");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}

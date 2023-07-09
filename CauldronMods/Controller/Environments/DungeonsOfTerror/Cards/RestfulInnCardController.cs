using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class RestfulInnCardController : DungeonsOfTerrorUtilityCardController
    {
        public RestfulInnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildTopCardOfLocationSpecialString(TurnTaker.Trash));
            SpecialStringMaker.ShowIfElseSpecialString(() => HasDrawnCardThisTurn(Game.ActiveTurnTaker.ToHero(), greaterThan: 0), () => $"{Game.ActiveTurnTaker.Name} has already drawn a card this turn.", () => $"{Game.ActiveTurnTaker.Name} has not yet drawn a card this turn.").Condition = () => Card.IsInPlayAndHasGameText && IsHero(Game.ActiveTurnTaker);
        }

        public override void AddTriggers()
        {
            //The first time a hero draws a card during their turn, they may discard it. If they do, that hero regains 2HP. Increase HP regained this way by 1 if the top card of the environment trash is a fate card.
            AddTrigger((DrawCardAction dca) =>dca.DidDrawCard && IsHero(Game.ActiveTurnTaker) && Game.ActiveTurnTaker.ToHero() == dca.HeroTurnTaker && !HasDrawnCardThisTurn(dca.HeroTurnTaker), FirstHeroDrawResponse, new TriggerType[]
                {
                    TriggerType.DiscardCard,
                    TriggerType.GainHP
                }, TriggerTiming.After);

            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator FirstHeroDrawResponse(DrawCardAction dca)
        {
            //they may discard it. 
            Card drawnCard = dca.DrawnCard;
            HeroTurnTakerController httc = FindHeroTurnTakerController(dca.HeroTurnTaker);
            DiscardCardAction fakeAction = new DiscardCardAction(GetCardSource(), httc, drawnCard, TurnTaker);
            List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(httc, SelectionType.DiscardCard, drawnCard, fakeAction, storedResults: storedYesNo, associatedCards: drawnCard.ToEnumerable(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If they do, that hero regains 2HP.Increase HP regained this way by 1 if the top card of the environment trash is a fate card.
            if(DidPlayerAnswerYes(storedYesNo))
            {
                List<DiscardCardAction> storedDiscard = new List<DiscardCardAction>();
                coroutine = GameController.DiscardCards(httc, drawnCard.ToEnumerable(), storedResults: storedDiscard, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(DidDiscardCards(storedDiscard))
                {
                    Card hpGainer = null;
                    if (httc.TurnTaker.HasMultipleCharacterCards)
                    {
                        List<SelectCardDecision> storedSelection = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(httc, SelectionType.CharacterCard, httc.CharacterCards.Where(c => !c.IsIncapacitatedOrOutOfGame && c.IsRealCard), storedSelection);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        SelectCardDecision selectCardDecision = storedSelection.FirstOrDefault();
                        if (selectCardDecision != null)
                        {
                            hpGainer = selectCardDecision.SelectedCard;
                        }
                    }
                    else
                    {
                        hpGainer = httc.CharacterCard;
                    }
                    if(hpGainer != null)
                    {
                        GainHPAction gainHPAction = new GainHPAction(GetCardSource(), hpGainer, 2, () => 2);
                        if(IsTopCardOfLocationFate(TurnTaker.Trash))
                        {
                            IEnumerator message = GameController.SendMessageAction("The top card of the environment trash is a fate card! Increasing HP recovery by 1!", Priority.Medium, GetCardSource(), showCardSource: true);
                            coroutine = GameController.IncreaseHPGain(gainHPAction, 1, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(message);
                                yield return base.GameController.StartCoroutine(coroutine);

                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(message);
                                base.GameController.ExhaustCoroutine(coroutine);

                            }
                        }
                        coroutine = GameController.DoAction(gainHPAction);
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
            yield break;
        }

        private bool HasDrawnCardThisTurn(HeroTurnTaker htt, int greaterThan = 1)
        {
           
                return (from e in Game.Journal.DrawCardEntriesThisTurn()
                        where htt == e.Hero
                        select e).Count() > greaterThan;
            
        }
    }
}

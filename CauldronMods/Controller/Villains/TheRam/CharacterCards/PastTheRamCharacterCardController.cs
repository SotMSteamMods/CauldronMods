using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace Cauldron.TheRam
{
    public class PastTheRamCharacterCardController : TheRamUtilityCharacterCardController
    {
        public PastTheRamCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card).Condition = () => !base.CharacterCard.IsFlipped;
        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            AddTrigger((GameAction ga) => TurnTakerController is TheRamTurnTakerController tttc && !tttc.IsAdmiralWintersHandled, ga => (TurnTakerController as TheRamTurnTakerController).HandleWintersEarly(false), TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);

        }

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                Card.UnderLocation.OverrideIsInPlay = false;

                //"If {TheRam} is destroyed remove this card from the game.",
                AddSideTrigger(AddWhenDestroyedTrigger(SetPostDestroyOutOfGame, TriggerType.ChangePostDestroyDestination));
                //"Whenever a villain one-shot would enter play, put it beneath this card instard. Cards beneath this one are not considered in play.",
                AddSideTrigger(AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay.IsVillain && cep.CardEnteringPlay.IsOneShot, MoveOneShotUnderResponse, TriggerType.CancelAction, TriggerTiming.Before));

                //"At the start of the villain turn, if there are 3 or more cards beneath this one, flip {TheRam}'s character cards."
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker && this.Card.UnderLocation.NumberOfCards >= 3, FlipThisCharacterCardResponse, TriggerType.FlipCard));

                if (IsGameAdvanced)
                {
                    //"Reduce damage dealt to {TheRam} by 1",
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c == this.Card, 1));
                }
            }
            else
            {
                //"If {TheRam} is destroyed remove this card from the game.",
                AddSideTrigger(AddWhenDestroyedTrigger(SetPostDestroyOutOfGame, TriggerType.ChangePostDestroyDestination));

                //"When {TheRam} flips to this side, it regains {H + 2} HP. Then, put all cards beneath this one into play in any order.",
                //see AfterFlipCardImmediateResponse

                //"At the start of the villain turn, if {TheRam} did not flip this turn, flip {TheRam}'s villain character cards.",
                AddSideTrigger(AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, FlipIfWasNotFlippedThisTurn, TriggerType.FlipCard));

                //"Increase projectile damage dealt by villain targets by 1."
                AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dda) => dda.DamageSource.IsVillainTarget && dda.DamageType == DamageType.Projectile, 1));

                if (IsGameAdvanced)
                {
                    //"At the end of the villain turn, play the top card of the villain deck.",
                    AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard));
                }
            }

            if(Game.IsChallenge)
            {
                //At the end of the villain turn, if this card is in play, destroy an equipment card, then put the top card of the villain deck under {TheRam}.
                AddSideTrigger(AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, ChallengeEndOfTurnResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.MoveCard }));
            }
        }

        private IEnumerator ChallengeEndOfTurnResponse(PhaseChangeAction pca)
        {
            //At the end of the villain turn, if this card is in play, destroy an equipment card
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsEquipment(c), "equipment"), optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (TurnTaker.Deck.IsEmpty) yield break;

            //then put the top card of the villain deck under {TheRam}.
            coroutine = GameController.MoveCard(TurnTakerController, TurnTaker.Deck.TopCard, Card.UnderLocation, cardSource: GetCardSource());
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

        private IEnumerator FlipIfWasNotFlippedThisTurn(PhaseChangeAction pca)
        {
            if (!Journal.WasCardFlippedThisTurn(this.Card))
            {
                IEnumerator coroutine = FlipThisCharacterCardResponse(pca);
                if (base.UseUnityCoroutines)
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

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (base.Card.IsFlipped)
            {
                //"When {TheRam} flips to this side, it regains {H + 2} HP. 
                coroutine = GameController.GainHP(this.Card, H + 2, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Then, put all cards beneath this one into play in any order.",
                coroutine = GameController.PlayCards(DecisionMaker, (Card c) => c.Location == this.Card.UnderLocation, false, true, responsibleTurnTaker: this.TurnTaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
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

        private IEnumerator SetPostDestroyOutOfGame(DestroyCardAction dc)
        {
            dc.SetPostDestroyDestination(TurnTaker.OutOfGame, showMessage: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SendMessageAction("The Ram 1929 is removed from the game!", Priority.High, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield return null;
            yield break;
        }

        private IEnumerator MoveOneShotUnderResponse(CardEntersPlayAction mc)
        {
            IEnumerator coroutine = GameController.CancelAction(mc);
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SendMessageAction($"The Ram stored {mc.CardEnteringPlay.Title} under itself!", Priority.Medium, GetCardSource(), new Card[] { mc.CardEnteringPlay });
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.MoveCard(TurnTakerController, mc.CardEnteringPlay, this.Card.UnderLocation, showMessage: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator AskIfMoveUpCloseResponse(PhaseChangeAction pc)
        {
            Card upClose = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash && c.Identifier == "UpClose")).FirstOrDefault();
            if (upClose == null)
            {
                IEnumerator message = GameController.SendMessageAction("There were no copies of Up Close in the trash to take.", Priority.High, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            TurnTaker activeHero = pc.ToPhase.TurnTaker;
            HeroTurnTakerController player = FindHeroTurnTakerController(activeHero.ToHero());

            List<YesNoCardDecision> storedResult = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.PutIntoPlay, upClose, pc, storedResult, new Card[] { this.Card }, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidPlayerAnswerYes(storedResult))
            {
                yield break;
            }
            
            List<Card> heroResult = new List<Card> { };
            coroutine = GameController.FindCharacterCard(player, activeHero, SelectionType.HeroCharacterCard, heroResult, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            Card hero = heroResult.FirstOrDefault();

            if (hero != null)
            {
                coroutine = (FindCardController(upClose) as UpCloseCardController).PlayBySpecifiedHero(hero, true, GetCardSource());
                if (base.UseUnityCoroutines)
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

        private IEnumerator FlipToBack(GameAction ga)
        {
            //"Whenever all active heroes are Up Close, flip {TheRam}'s character cards..."

            IEnumerator coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //...and destroy all copies of Fall Back"
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c != null && c.IsInPlayAndHasGameText && c.Identifier == "FallBack"), autoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //"When {TheRam}'s character cards are flipped to this side, search the villain trash for Grappling claw and put it into play.",
            coroutine = GameController.SelectAndMoveCard(DecisionMaker, (Card c) => c != null && c.Location == TurnTaker.Trash && c.Identifier == "GrapplingClaw", TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
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

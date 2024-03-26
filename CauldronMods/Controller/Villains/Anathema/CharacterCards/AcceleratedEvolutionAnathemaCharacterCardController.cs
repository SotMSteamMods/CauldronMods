using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Anathema
{
    public class AcceleratedEvolutionAnathemaCharacterCardController : VillainCharacterCardController
    {

        private const string ExplosiveTransformationKeyword = "ExplosiveTransformation";

        public AcceleratedEvolutionAnathemaCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialString ss = base.SpecialStringMaker.ShowNumberOfCardsUnderCard(base.Card);
            ss.Condition = () => !base.CharacterCard.IsFlipped;
        }

        private bool IsArm(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "arm");
        }

        private bool IsHead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "head");
        }

        private bool IsBody(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "body");
        }

        private bool IsArmOrHead(Card c)
        {
            return IsArm(c) || IsHead(c);
        }

        public override void AddSideTriggers()
        {
            //on his front side
            if (!base.Card.IsFlipped)
            {
                //Whenever {Anathema} destroys an arm or head card, put that under {Anathema}'s villain character card.
                //Front Challenge: Replace both instances of “arm or head card” with “villain target”.
                this.SideTriggers.Add(AddTrigger((DestroyCardAction destroy) => destroy.CardSource != null && destroy.CardToDestroy.CanBeDestroyed && destroy.WasCardDestroyed && destroy.CardSource.Card.Owner == base.TurnTaker && (IsArmOrHead(destroy.CardToDestroy.Card) || (base.Game.IsChallenge && IsVillainTarget(destroy.CardToDestroy.Card))) && destroy.PostDestroyDestinationCanBeChanged, PutUnderThisCardResponse, new TriggerType[2]
                        {
                            TriggerType.MoveCard,
                            TriggerType.ChangePostDestroyDestination
                        }, TriggerTiming.After));

                //At the end of the villain turn, reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
                //Then if there are {H} or more cards under {Anathema}, flip his villain character card.
                this.SideTriggers.Add(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EndOfTurnFrontResponse, new TriggerType[]
                {
                    TriggerType.RevealCard,
                    TriggerType.MoveCard,
                    TriggerType.FlipCard
                }));

                if (base.IsGameAdvanced)
                {
                    //At the end of the villain turn, {Anathema} regains {H - 2} HP.
                    this.SideTriggers.Add(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AdvancedEndOfTurnFrontResponse, TriggerType.GainHP));
                }
            }
            else
            {
                //When explosive transformation enters play, flip {Anathema}'s character cards.
                this.SideTriggers.Add(AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpe) => cpe.CardEnteringPlay != null && cpe.CardEnteringPlay.Identifier == ExplosiveTransformationKeyword && cpe.IsSuccessful, FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.After));

                if (base.IsGameAdvanced)
                {
                    //At the end of the villain turn {Anathema} regains 1 HP for each villain target in play.
                    this.SideTriggers.Add(AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, AdvancedEndOfTurnBackResponse, TriggerType.GainHP));
                }
            }

            base.AddDefeatedIfDestroyedTriggers();
            AddTrigger<PhaseChangeAction>(pca => true, pca =>  GameController.DestroyAnyCardsThatShouldBeDestroyed(cardSource: GetCardSource()), TriggerType.Hidden, TriggerTiming.After);
        }

        private IEnumerator AdvancedEndOfTurnFrontResponse(PhaseChangeAction arg)
        {
            //At the end of the villain turn, {Anathema} regains {H - 2} HP.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, Game.H - 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator AdvancedEndOfTurnBackResponse(PhaseChangeAction arg)
        {
            //Anathema} regains 1 HP for each villain target in play.
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, null, () => FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && IsVillainTarget(c)).Count(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator EndOfTurnFrontResponse(PhaseChangeAction arg)
        {
            //reveal the top card of the villain deck. If an arm or head card is revealed, put it under {Anathema}'s character card, otherwise discard it. 
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, 1, storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Count > 0)
            {
                //Front Challenge: Replace both instances of “arm or head card” with “villain target”.
                Card cardToMove = storedResults.First();
                Location location = IsArmOrHead(cardToMove) || (base.Game.IsChallenge && IsVillainTarget(cardToMove)) ? base.CharacterCard.UnderLocation : base.TurnTaker.Trash;
                coroutine = GameController.MoveCard(base.TurnTakerController, cardToMove, location, showMessage: true, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Then if there are {H} or more cards under {Anathema}, flip his villain character card.
            int numCardsUnderAnathema = base.CharacterCard.UnderLocation.NumberOfCards;
            if (numCardsUnderAnathema < Game.H)
            {
                yield break;
            }

            // Flip
            coroutine = base.GameController.FlipCard(this, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            //Remove and Add existing Side Triggers
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!base.CharacterCard.IsFlipped)
            {
                yield break;
            }

            //When {Anathema} flips to this side, put all cards from underneath him into play. 
            IEnumerable<Card> cardsToMove = base.CharacterCard.UnderLocation.Cards;
            coroutine = GameController.MoveCards(base.TurnTakerController, cardsToMove, (Card c) => new MoveCardDestination(c.Owner.PlayArea), isPutIntoPlay: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle all copies of explosive transformation from the villain trash into the villain deck.
            IEnumerable<Card> cardsToShuffle = FindCardsWhere((Card c) => c.Identifier == ExplosiveTransformationKeyword && base.TurnTaker.Trash.HasCard(c));
            coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, cardsToShuffle, base.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator PutUnderThisCardResponse(DestroyCardAction destroyCard)
        {
            destroyCard.SetPostDestroyDestination(base.Card.UnderLocation, cardSource: GetCardSource());
            yield return null;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Flipped: Arm and head cards are indestructible during the villain turn.
            return base.CharacterCard.IsFlipped && Game.ActiveTurnTaker.IsVillain && IsPotentialIndestructibleBodyPart(card);
        }

        private bool IsPotentialIndestructibleBodyPart(Card card)
        {
            bool bodyPartCheck = IsArmOrHead(card);
            if (base.Game.IsChallenge)
            {
                //Challenge Flipped: Body cards are indestructible during the villain turn.
                bodyPartCheck = bodyPartCheck || IsBody(card);
            }

            return bodyPartCheck;
        }
    }
}

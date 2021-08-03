using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class ConvincingDoubleCardController : CardController
    {
        public ConvincingDoubleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //SpecialStringMaker.ShowSpecialString(() => "Passing to or from multiple-character heroes is likely to be buggy. Don't say we didn't warn you.");
        }

        public override bool AllowFastCoroutinesDuringPretend => false;
        public override bool UseDecisionMakerAsCardOwner => true;

        private Card _passedCard = null;
        private HeroTurnTakerController _giverController = null;
        private HeroTurnTakerController _receiverController = null;
        private Card _receiverCharacterCard = null;

        private enum CustomMode
        {
            CardToPass,
            PlayerPassingCard,
            PlayerReceivingCard
        }
        private CustomMode CurrentMode;

        public override IEnumerator Play()
        {
            //"Select a player."
            List<SelectTurnTakerDecision> storedTurnTakerDecisions = new List<SelectTurnTakerDecision> { };
            IEnumerator msgRoutine;
            var validPassers = GameController.AllTurnTakers.Where((TurnTaker tt) => tt.IsHero &&
                                                                   GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                                                                   tt.ToHero().HasCardsWhere((Card c) => c.IsInHand && c.IsOneShot));
            if(validPassers.Count() == 0)
            {
                msgRoutine = GameController.SendMessageAction("No players can pass a one-shot.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msgRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msgRoutine);
                }
                yield break;
            }
            if (validPassers.Count() == 1)
            {
                msgRoutine = GameController.SendMessageAction($"{validPassers.FirstOrDefault().Name} is the only player who can pass a one-shot.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msgRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msgRoutine);
                }
            }
            CurrentMode = CustomMode.PlayerPassingCard;
            IEnumerator chooseGivingPlayer = GameController.SelectTurnTaker(HeroTurnTakerController, SelectionType.Custom, storedTurnTakerDecisions,
                                                                                additionalCriteria: (TurnTaker tt) => tt.IsHero &&
                                                                                                    GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                                                                                                    tt.ToHero().HasCardsWhere((Card c) => c.IsInHand && c.IsOneShot),
                                                                                cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseGivingPlayer);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseGivingPlayer);
            }

            if (!DidSelectTurnTaker(storedTurnTakerDecisions))
            {
                yield break;
            }

            var decision = storedTurnTakerDecisions.FirstOrDefault();
            storedTurnTakerDecisions.Remove(decision);

            TurnTaker giver = decision.SelectedTurnTaker;

            //"That player passes a one-shot card..."
            List<SelectCardDecision> storedCardDecision = new List<SelectCardDecision> { };

            var validCards = giver.ToHero().Hand.Cards.Where(c => c.IsOneShot);
            if(validCards.Count() == 1)
            {
                msgRoutine = GameController.SendMessageAction($"{validCards.FirstOrDefault().Title} is the only one-shot in {giver.ToHero().Hand.GetFriendlyName()}.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msgRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msgRoutine);
                }
            }
            CurrentMode = CustomMode.CardToPass;
            IEnumerator chooseOneShot = GameController.SelectCardAndStoreResults(FindHeroTurnTakerController(giver.ToHero()),
                                                                                SelectionType.Custom,
                                                                                new LinqCardCriteria((Card c) => giver.ToHero().Hand.Cards.Contains(c) && c.IsOneShot),
                                                                                storedCardDecision,
                                                                                false,
                                                                                cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseOneShot);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseOneShot);
            }


            if (!DidSelectCard(storedCardDecision))
            {
                yield break;
            }
            Card passedCard = storedCardDecision.FirstOrDefault().SelectedCard;

            //"..to another player..."
            var validReceivers = GameController.AllTurnTakers.Where((TurnTaker tt) => tt.IsHero &&
                                                                     !tt.IsIncapacitatedOrOutOfGame &&
                                                                     GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                                                                     tt != giver);
            if(validReceivers.Count() == 0)
            {
                msgRoutine = GameController.SendMessageAction($"No player can be passed {passedCard.Title}.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msgRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msgRoutine);
                }
                yield break;
            }
            if(validReceivers.Count() == 1)
            {
                msgRoutine = GameController.SendMessageAction($"{validReceivers.FirstOrDefault().Name} is the only player who can be passed {passedCard.Title}.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(msgRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(msgRoutine);
                }
            }
            /*
            IEnumerator chooseReceivingPlayer = GameController.SelectTurnTaker(FindHeroTurnTakerController(giver.ToHero()), SelectionType.MoveCard, storedTurnTakerDecisions,
                                                                                additionalCriteria: (TurnTaker tt) => tt.IsHero &&
                                                                                                    !tt.IsIncapacitatedOrOutOfGame &&
                                                                                                    GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                                                                                                    tt != giver,
                                                                                cardSource: GetCardSource());
            */
            CurrentMode = CustomMode.PlayerReceivingCard;
            var passToDecision = new SelectTurnTakerDecision(GameController, FindHeroTurnTakerController(giver.ToHero()), validReceivers, SelectionType.Custom, cardSource: GetCardSource(), associatedCards: new Card[] { passedCard });
            if (validReceivers.Count() == 1)
            {
                passToDecision.SelectedTurnTaker = validReceivers.FirstOrDefault();
            }
            else
            {
                IEnumerator chooseReceivingPlayer = GameController.MakeDecisionAction(passToDecision);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(chooseReceivingPlayer);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(chooseReceivingPlayer);
                }
            }

            storedTurnTakerDecisions.Add(passToDecision);

            if (!DidSelectTurnTaker(storedTurnTakerDecisions))
            {
                yield break;
            }

            decision = storedTurnTakerDecisions.FirstOrDefault();
            TurnTaker receiver = decision.SelectedTurnTaker;

            _passedCard = passedCard;
            _giverController = FindHeroTurnTakerController(giver.ToHero());
            _receiverController = FindHeroTurnTakerController(receiver.ToHero());

            //just in case we pass to Sentinels, we make them pick one character card to be "their" hero
            var storedResults = new List<Card> { };
            var selectReceiverCharacterCard = FindCharacterCard(receiver, SelectionType.CharacterCard, storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectReceiverCharacterCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectReceiverCharacterCard);
            }
            _receiverCharacterCard = storedResults.FirstOrDefault();

            AddThisCardControllerToList(CardControllerListType.ReplacesCards);
            AddThisCardControllerToList(CardControllerListType.ReplacesTurnTakerController);
            ITrigger associatedCardSourceTrigger = AddTrigger((GameAction ga) => ga.CardSource != null && ga.CardSource.Card == _passedCard, AddThisAsAssociatedCardSource, TriggerType.Hidden, TriggerTiming.Before);

            CardController passedController = FindCardController(_passedCard);
            passedController.AddAssociatedCardSource(GetCardSource());

            //send a message to tell the player what is happening
            IEnumerator sendMessage = GameController.SendMessageAction(receiver.CharacterCard.Title + " puts " + passedCard.Title + " into play!", Priority.High, cardSource: GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(sendMessage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(sendMessage);
            }

            //"who then puts that card into play as if it were their card, treating any hero name on the card as the name of their hero instead."
            IEnumerator swappedPlay = GameController.PlayCard(_receiverController, passedCard, true, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(swappedPlay);
            }
            else
            {
                base.GameController.ExhaustCoroutine(swappedPlay);
            }

            RemoveThisCardControllerFromList(CardControllerListType.ReplacesCards);
            RemoveThisCardControllerFromList(CardControllerListType.ReplacesTurnTakerController);
            RemoveTrigger(associatedCardSourceTrigger);
            passedController.RemoveAssociatedCardSource(GetCardSource());

            _passedCard = null;
            _giverController = null;
            _receiverController = null;
            _receiverCharacterCard = null;

            yield break;
        }

        public override Card AskIfCardIsReplaced(Card card, CardSource cardSource)
        {
            //this is what handles the "treating any hero name on that card as the name of their hero instead" part
            if (cardSource != null && _passedCard != null && cardSource.Card == _passedCard)
            {
                //Most cards, code-wise, refer to their hero with simply "this.CharacterCard" which is automatically
                //swapped out when the TurnTakerController is. For those few that call out a specific hero some other 
                //way, this swaps it for the receiver's character.
                if (card != null && _receiverCharacterCard != null && cardSource != null && card.IsHeroCharacterCard && cardSource.AllowReplacements)
                {
                    //Log.Debug($"Card-to-possibly-replace is {card.Title}");
                    if (_passedCard != null && _giverController != null)
                    {
                        Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
                        IEnumerable<CardController> sources = cardSource.CardSourceChain.Select((CardSource cs) => cs.CardController);

                        if (sources.Contains(FindCardController(_passedCard)) && sources.Contains(this) && _passedCard == cardSource.Card && _giverController.CharacterCards.Contains(card)) // && sources.Contains(this)
                        {
                            return _receiverCharacterCard;
                        }
                    }
                }

                //Alternatively, if we pass *to* a multi-character hero all of those "this.CharacterCard" references
                //will be to their instruction card, which is not supposed to act. Here's where we swap it for the one 
                //they picked.
                if (card == null || !card.IsRealCard)
                {
                    //Log.Debug("might try to replace a null character card, if we're lucky");
                    if (card != null && cardSource.AllowReplacements && cardSource.Card == _passedCard)
                    {
                        //Log.Debug("Fake card detected");
                        if (_receiverController != null && _receiverController.HasMultipleCharacterCards && _passedCard != null)
                        {
                            //Log.Debug("And it should!");
                            Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
                            IEnumerable<CardController> sources = cardSource.CardSourceChain.Select((CardSource cs) => cs.CardController);

                            if (sources.Contains(FindCardController(_passedCard)) && sources.Contains(this) && _passedCard == cardSource.Card) //&& sources.Contains(this) && _giverController.CharacterCards.Contains(card))
                            {
                                //Log.Debug($"Returning {_receiverCharacterCard.Title}");
                                return _receiverCharacterCard;
                            }
                        }

                    }
                    else
                    {
                        //Log.Debug("Null card detected");
                        if (_receiverController != null && _receiverController.HasMultipleCharacterCards && _passedCard != null)
                        {
                            //Log.Debug("And it did!");
                            Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
                            IEnumerable<CardController> sources = cardSource.CardSourceChain.Select((CardSource cs) => cs.CardController);

                            if (sources.Contains(FindCardController(_passedCard)) && sources.Contains(this) && _passedCard == cardSource.Card && _giverController.CharacterCards.Contains(card))
                            {
                                return _receiverCharacterCard;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public override TurnTakerController AskIfTurnTakerControllerIsReplaced(TurnTakerController ttc, CardSource cardSource)
        {
            //This is what handles the "as if it were their card" part of the instructions
            //Since CardController.CharacterCard ultimately goes through TurnTakerController.CharacterCard,
            //this usually handles swapping out hero names too.

            HeroTurnTakerController receiverTTC = _receiverController;
            HeroTurnTaker receiver = receiverTTC.HeroTurnTaker;
            if (cardSource != null && cardSource.Card.Owner != receiver && cardSource.AllowReplacements)
            {
                Card cardWithoutReplacements = cardSource.CardController.CardWithoutReplacements;
                TurnTakerController ttcWithoutReplacements = FindCardController(_passedCard).TurnTakerControllerWithoutReplacements;

                if (ttc == ttcWithoutReplacements)
                {

                    if (cardWithoutReplacements == _passedCard)
                    {
                        return receiverTTC;
                    }
                    if (cardSource.CardSourceChain.Any((CardSource cs) => cs.CardController == this))
                    {
                        return receiverTTC;
                    }

                }
            }
            return null;
        }

        private IEnumerator AddThisAsAssociatedCardSource(GameAction ga)
        {
            ga.CardSource.AddAssociatedCardSource(GetCardSource());
            yield return null;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (CurrentMode == CustomMode.CardToPass)
            {
                return new CustomDecisionText("Select a card to pass to another player.", $"{decision.DecisionMaker.Name} is deciding which card to pass...", $"Which card should {decision.DecisionMaker.Name} pass?", "card to pass");
            }
            else if (CurrentMode == CustomMode.PlayerPassingCard)
            {
                return new CustomDecisionText("Select a player to pass a one-shot from their hand.", $"{decision.DecisionMaker.Name} is deciding who should pass a one-shot...", $"Who should pass a one-shot to a different player?", "player to pass a one-shot");
            }
            else if (CurrentMode == CustomMode.PlayerReceivingCard)
            {
                var passedTitle = "the passed card";
                if(decision.AssociatedCards.Any())
                {
                    passedTitle = decision.AssociatedCards.FirstOrDefault().Title;
                }
                return new CustomDecisionText($"Select a player to put {passedTitle} into play.", $"{decision.DecisionMaker.Name} is deciding who to pass {passedTitle} to...", $"Who should {passedTitle} be passed to?", "player to pass a one-shot to");
            }

            return base.GetCustomDecisionText(decision);

        }
    }
}
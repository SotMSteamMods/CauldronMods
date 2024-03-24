using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class MythosCharacterCardController : VillainCharacterCardController
    {
        public MythosCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            base.SpecialStringMaker.ShowTokenPool(DangerousInvestigationIdentifier, DangerousInvestigationPool);
            base.SpecialStringMaker.ShowSpecialString(() => this.DeckIconList());
        }


        protected const string MythosClueDeckIdentifier = "{Clue}";
        protected const string MythosDangerDeckIdentifier = "{Danger}";
        protected const string MythosMadnessDeckIdentifier = "{Madness}";
        protected const string DangerousInvestigationPool = "DangerousInvestigationPool";
        protected const string DangerousInvestigationIdentifier = "DangerousInvestigation";

        public override void AddSideTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                //Activate any {MythosDanger}, {MythosMadness}, {MythosClue} effects that match the icon on top of the villain deck.
                /**Added to MythosUtilityCardController**/

                //{MythosDanger} {Mythos} is immune to damage.
                base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == this.Card && this.IsTopCardMatching(MythosDangerDeckIdentifier)));

                //At the end of the villain turn, the players may play up to 5 cards from the top of the villain deck. Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.FrontEndOfTurnResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.FlipCard }));

                if (base.Game.IsAdvanced)
                {
                    //Reduce damage dealt to {Mythos} by 2.
                    base.AddSideTrigger(base.AddReduceDamageTrigger((Card c) => c == this.Card, 2));
                }
            }
            else
            {
                //Activate any {MythosDanger}, {MythosMadness}, {MythosClue} effects that match the icon on top of the villain deck.
                /**Added to MythosUtilityCardController**/

                //{MythosDanger} Reduce damage dealt to villain targets by 1.
                base.AddSideTrigger(base.AddReduceDamageTrigger((Card c) => base.IsVillain(c) && this.IsTopCardMatching(MythosDangerDeckIdentifier), 1));

                //At the end of the villain turn, the players may play the top card of the villain deck. Then:
                //--{MythosClue} Play the top card of the villain deck.
                //--{MythosMadness} {Mythos} deals each hero target {H} infernal damage.
                base.AddSideTrigger(base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.BackEndOfTurnResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.DealDamage }));

                if (base.Game.IsAdvanced)
                {
                    //Advanced: Activate all {MythosDanger} effects.
                    /**Added to MythosUtilityCardController**/
                }
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            //When {Mythos} flips to [the back], remove Dangerous Investigation from the game.
            IEnumerator coroutine = base.GameController.MoveCard(this.TurnTakerController, this.DangerousInvestigationCard, base.TurnTaker.OutOfGame);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            
            coroutine = GameController.DestroyAnyCardsThatShouldBeDestroyed(cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            
            base.RemoveSideTriggers();
            this.AddSideTriggers();
            yield return null;
            yield break;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Front: {Mythos} and Dangerous Investigation are indestructible. 
            return (this.Card == card || this.DangerousInvestigationCard == card) && !this.Card.IsFlipped;
        }

        private Card DangerousInvestigationCard
        {
            get
            {
                return base.FindCard(DangerousInvestigationIdentifier);
            }
        }

        private IEnumerator BackEndOfTurnResponse(PhaseChangeAction action)
        {
            //...the players may play the top card of the villain deck. Then:
            IEnumerator coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, true, 1, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (this.IsTopCardMatching((MythosClueDeckIdentifier)))
            {
                //{MythosClue} Play the top card of the villain deck.
                coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(action);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (this.IsTopCardMatching((MythosMadnessDeckIdentifier)))
            {
                //{MythosMadness} {Mythos} deals each hero target {H} infernal damage.
                coroutine = base.DealDamage(this.Card, (Card c) => IsHeroTarget(c), base.Game.H, DamageType.Infernal);
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

        private string DeckIconList()
        {
            //For special string describing the order of icons in the deck top(1) to bottom
            string output = null;
            int place = 0;
            if (IsGameChallenge)
            {
                if(TurnTaker.Deck.HasCards)
                {
                    output = "The icon on top of the deck is:{BR}";
                    switch (this.GetIconIdentifier(TurnTaker.Deck.TopCard))
                    {
                        case MythosClueDeckIdentifier:
                            output += "{Clue}";
                            break;

                        case MythosDangerDeckIdentifier:
                            output += "{Danger}";
                            break;

                        case MythosMadnessDeckIdentifier:
                            output += "{Madness}";
                            break;
                    }
                }
            }
            else
            {
                foreach (Card c in base.TurnTaker.Deck.Cards.ToArray().Reverse())
                {
                    place++;
                    if (output == null)
                    {
                        output = "Starting at the top, the order of the deck icons is:{BR}";
                    }
                    switch (this.GetIconIdentifier(c))
                    {
                        case MythosClueDeckIdentifier:
                            output += place + ": {Clue}";
                            break;

                        case MythosDangerDeckIdentifier:
                            output += place + ": {Danger}";
                            break;

                        case MythosMadnessDeckIdentifier:
                            output += place + ": {Madness}";
                            break;
                    }
                    if (base.TurnTaker.Deck.Cards.Count() != place)
                    {
                        output += ",{BR}";
                    }
                }
            }

            if (output == null)
            {
                output = "There are no cards in the deck.";
            }
            else
            {
                output += ".";
            }
            return output;
        }

        private IEnumerator FrontEndOfTurnResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine;
            int cardCount = 0;
            //...the players may play up to 5 cards from the top of the villain deck. 
            while (cardCount < 5)
            {
                List<Card> playedCards = new List<Card>();
                if (IsTopCardMatching(MythosClueDeckIdentifier))
                {
                    var yesNo = new YesNoAmountDecision(GameController, DecisionMaker, SelectionType.Custom, 1, requireUnanimous: true, cardSource: GetCardSource());
                    coroutine = GameController.MakeDecisionAction(yesNo);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    if(DidPlayerAnswerYes(yesNo))
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
                }
                else
                {
                    coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, true, 1, playedCards: playedCards, cardSource: base.GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                cardCount++;
                if (!playedCards.Any())
                {
                    break;
                }
            }

            //Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            if (this.DangerousInvestigationCard.FindTokenPool(DangerousInvestigationPool).CurrentValue == base.Game.H)
            {
                coroutine = base.FlipThisCharacterCardResponse(action);
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

        private string GetIconIdentifier(Card c)
        {
            //Temporary method to get the icon of a card until Subdecks are implemented
            string[] clueIdentifiers = { "DangerousInvestigation", "PallidAcademic", "Revelations", "RitualSite", "RustedArtifact", "TornPage" };
            string[] dangerIdentifiers = { "AclastyphWhoPeers", "FaithfulProselyte", "OtherworldlyAlignment", "PreyUponTheMind" };
            string[] madnessIdentifiers = { "ClockworkRevenant", "DoktorVonFaust", "HallucinatedHorror", "WhispersAndLies", "YourDarkestSecrets" };

            string identifier = null;
            if (clueIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosClueDeckIdentifier;
            }
            if (dangerIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosDangerDeckIdentifier;
            }
            if (madnessIdentifiers.Contains(c.Identifier))
            {
                identifier = MythosMadnessDeckIdentifier;
            }
            return identifier;
            /**TODO: Remove above when Subdecks are implemented**/
            //return c.ParentDeck.Identifier;
        }

        private bool IsTopCardMatching(string type)
        {
            if(TurnTaker.Deck.NumberOfCards == 0)
            {
                return false;
            }

            //Advanced - Back: Activate all {MythosDanger} effects.
            if (base.Game.IsAdvanced && base.CharacterCard.IsFlipped && type == MythosDangerDeckIdentifier)
            {
                return true;
            }
            return this.GetIconIdentifier(base.TurnTaker.Deck.TopCard) == type;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            return new CustomDecisionText("There is a {{Clue}} card on top of the villain deck, but playing it this way will not add a token to Dangerous Investigation. Play the top card of the villain deck anyway?",
                            "Selecting whether to play the top card of the villain deck",
                            "Vote for whether to play the top card of the villain deck without adding a token to Dangerous Investigation.",
                            "Play {{Clue}} cards from villain deck without adding tokens");
        }
    }
}

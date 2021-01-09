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
            base.SpecialStringMaker.ShowSpecialString(() => this.DeckIconList());
        }


        protected const string MythosClueDeckIdentifier = "MythosClue";
        protected const string MythosDangerDeckIdentifier = "MythosDanger";
        protected const string MythosMadnessDeckIdentifier = "MythosMadness";

        public override void AddTriggers()
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
                base.AddSideTrigger(base.AddReduceDamageTrigger((Card c) => base.IsVillain(c), 1));

                //At the end of the villain turn, the players may play the top card of the villain deck. Then:
                //--{MythosClue} Play the top card of the villain deck.
                //--{MythosDanger} {Mythos} deals each hero target {H} infernal damage.
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
            yield return base.AfterFlipCardImmediateResponse();
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
                return base.FindCard("DangerousInvestigation");
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
                coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, true, 1, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (this.IsTopCardMatching((MythosDangerDeckIdentifier)))
            {
                //{MythosDanger} {Mythos} deals each hero target {H} infernal damage.
                coroutine = base.DealDamage(this.Card, (Card c) => c.IsHero, base.Game.H, DamageType.Infernal);
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
            string output = null;
            int place = 0;
            foreach (Card c in base.TurnTaker.Deck.Cards)
            {
                place++;
                if (output == null)
                {
                    output = "The order of the deck icons is: ";
                }
                switch (this.GetIconIdentifier(c))
                {
                    case MythosClueDeckIdentifier:
                        output += place + " is a Blue Clue Icon, ";
                        break;

                    case MythosDangerDeckIdentifier:
                        output += place + " is a Red Danger Icon, ";
                        break;

                    case MythosMadnessDeckIdentifier:
                        output += place + " is a Green Madness Icon, ";
                        break;
                }
            }

            if (output == null)
            {
                output = "There are no cards in the deck.";
            }
            else
            {
                output.Trim(new char[] { ',', ' ' });
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
                coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, base.TurnTaker.Deck, true, 1, playedCards: playedCards, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                cardCount++;
                if (!playedCards.Any())
                {
                    break;
                }
            }

            //Then if there are {H} tokens on Dangerous Investigation, flip {Mythos}' villain character cards.
            if (this.DangerousInvestigationCard.FindTokenPool("DangerousInvestigationPool").CurrentValue == 5)
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
            /**Remove above when Subdecks are implemented**/
            return c.ParentDeck.Identifier;
        }

        private bool IsTopCardMatching(string type)
        {
            //Advanced - Back: Activate all {MythosDanger} effects.
            if (base.Game.IsAdvanced && base.CharacterCard.IsFlipped && type == MythosDangerDeckIdentifier)
            {
                return true;
            }
            return this.GetIconIdentifier(base.TurnTaker.Deck.TopCard) == type;
        }
    }
}

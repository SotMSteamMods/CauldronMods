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

        }

        protected const string MythosEyeDeckIdentifier = "MythosEye";
        protected const string MythosFearDeckIdentifier = "MythosFear";
        protected const string MythosMindDeckIdentifier = "MythosMind";

        public override void AddTriggers()
        {
            if (!this.Card.IsFlipped)
            {
                //Activate any {MythosDanger}, {MythosMadness}, {MythosClue} effects that match the icon on top of the villain deck.
                /**Added to MythosUtilityCardController**/

                //{MythosDanger} {Mythos} is immune to damage.
                base.AddSideTrigger(base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target == this.Card && this.IsTopCardMatching(MythosFearDeckIdentifier)));

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

        private bool IsTopCardMatching(string type)
        {
            //Advanced - Back: Activate all {MythosDanger} effects.
            if (base.Game.IsAdvanced && base.CharacterCard.IsFlipped && type == MythosFearDeckIdentifier)
            {
                return true;
            }

            string[] eyeIdentifiers = { "DangerousInvestigation", "PallidAcademic", "Revelations", "RitualSite", "RustedArtifact", "TornPage" };
            string[] fearIdentifiers = { "AclastyphWhoPeers", "FaithfulProselyte", "OtherworldlyAlignment", "PreyUponTheMind" };
            string[] mindIdentifiers = { "ClockworkRevenant", "DoktorVonFaust", "HallucinatedHorror", "WhispersAndLies", "YourDarkestSecrets" };
            string topIdentifier = null;
            if (eyeIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosEyeDeckIdentifier;
            }
            if (fearIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosFearDeckIdentifier;
            }
            if (mindIdentifiers.Contains(base.TurnTaker.Deck.TopCard.Identifier))
            {
                topIdentifier = MythosMindDeckIdentifier;
            }
            return topIdentifier == type;
            //Once the UI allows for this, remove above.
            return base.TurnTaker.Deck.TopCard.ParentDeck.Identifier == type;
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

            if (this.IsTopCardMatching((MythosEyeDeckIdentifier)))
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

            if (this.IsTopCardMatching((MythosFearDeckIdentifier)))
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
    }
}

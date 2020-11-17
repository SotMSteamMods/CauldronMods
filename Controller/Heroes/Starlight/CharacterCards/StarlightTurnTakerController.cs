using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{ 
    public class StarlightTurnTakerController : HeroTurnTakerController
    {
        private CharacterCardController _instructions = null;
        private bool? hasInstructionCard = null;
        private bool _inSetup = true;

        public StarlightTurnTakerController(TurnTaker tt, GameController gc) : base(tt, gc)
        {
   
        }
        private List<string> nightloreCouncilIdentifiers
        {
            get
            {
                return new List<string> { "StarlightOfTerraCharacter", "StarlightOfAsheronCharacter", "StarlightOfCryosFourCharacter" };
            }
        }

        public IEnumerator LoadSubCharacters(bool isGameStart)
        {
            Log.Debug(TurnTaker == null ? "No TurnTaker yet" : "TurnTaker found...");
            DeckDefinition starlightDeck = TurnTaker.DeckDefinition;
            foreach (string charID in nightloreCouncilIdentifiers)
            {
                Log.Debug("Trying to load " + charID + "...");
                CardDefinition definer = starlightDeck.PromoCardDefinitions.Where((CardDefinition cd) => cd.Identifier == charID).FirstOrDefault();
                if (definer != null)
                {
                    Card newCard = new Card(definer, TurnTaker, 0, InstructionCardController.Card.IsFoilVersion);
                    TurnTaker.AddOwnedCard(newCard);
                    if (isGameStart)
                    {
                        TurnTaker.OffToTheSide.AddCard(newCard);
                    }
                    else
                    {
                        TurnTaker.PlayArea.AddCard(newCard);
                    }
                    CardController newCC = CardControllerFactory.CreateInstance(newCard, this, "Cauldron.Starlight");
                    AddCardController(newCC);
                    if (isGameStart)
                    { 
                        IEnumerator putInPlay = GameController.PlayCard(this, newCard, isPutIntoPlay: true, canBeCancelled: false);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(putInPlay);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(putInPlay);
                        }
                    }
                }
                else
                {
                    Log.Warning("Failed to load a Nightlore Council sub-starlight: " + charID + "!");
                }
            }
            yield break;
        }

        public void LoadSubCharactersNoEnumerator()
        {
            Log.Debug(TurnTaker == null ? "No TurnTaker yet" : "TurnTaker found...");
            DeckDefinition starlightDeck = TurnTaker.DeckDefinition;
            foreach (string charID in nightloreCouncilIdentifiers)
            {
                Log.Debug("Trying to load " + charID + "...");
                CardDefinition definer = starlightDeck.PromoCardDefinitions.Where((CardDefinition cd) => cd.Identifier == charID).FirstOrDefault();
                if (definer != null)
                {
                    Card newCard = new Card(definer, TurnTaker, 0, InstructionCardController.Card.IsFoilVersion);
                    TurnTaker.AddOwnedCard(newCard);
                    TurnTaker.PlayArea.AddCard(newCard);
                    CardController newCC = CardControllerFactory.CreateInstance(newCard, this, "Cauldron.Starlight");
                    AddCardController(newCC);
                    
                }
                else
                {
                    Log.Warning("Failed to load a Nightlore Council sub-starlight: " + charID + "!");
                }
            }
        }
        private CharacterCardController InstructionCardController
        {
            get
            {
                if (hasInstructionCard == null)
                {
                    Card card = base.TurnTaker.FindCard("StarlightCharacter", realCardsOnly: false);
                    Log.Debug("Card found, promo-ID is " + card.PromoIdentifierOrIdentifier);
                    if (card != null && card.PromoIdentifierOrIdentifier == "NightloreCouncilStarlightCharacter")
                    {
                        Log.Debug("Have found instruction card");

                            _instructions = (CharacterCardController)FindCardController(card);
                            hasInstructionCard = true;

                    }
                    else
                    {
                        Log.Debug("No instruction card found");
                        hasInstructionCard = false;
                    }
                }
                return (bool)hasInstructionCard ? _instructions : null;
            }
        }
        public override IEnumerator StartGame()
        {
            _inSetup = false;
            if ((bool)hasInstructionCard || CharacterCardController is NightloreCouncilStarlightCharacterCardController)
            {
                IEnumerator loadCharacters = null;
                //loadCharacters = LoadSubCharacters(true);
                if (loadCharacters != null)
                {
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(loadCharacters);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(loadCharacters);
                    }
                }
            }
            yield return base.StartGame();
            yield break;
        }
        public override Card CharacterCard
        {
            get
            {
                if (InstructionCardController != null)
                {
                    Log.Warning("There was a request for Nightlore Council Starlight's character card, which should be null.");
                    return null;
                }
                return base.CharacterCard;
            }
        }

        public override CharacterCardController IncapacitationCardController
        {
            get
            {
                if (InstructionCardController == null)
                {
                    return base.IncapacitationCardController;
                }
                else
                {
                    return InstructionCardController;
                }
            }
        }

        public override bool IsIncapacitated
        {
            get
            {
                if (InstructionCardController == null)
                {
                    return base.IsIncapacitated;
                }
                else 
                {
                    if (FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c.IsActive && c.IsInPlayAndHasGameText).Count() != 0)
                    {
                        return IncapacitationCardController.Card.IsFlipped;
                    }
                    //if (this.TurnTaker.GetAllCards(false).Where((Card c) => c.IsCharacter && c.IsInPlay).Count() <= 1)
                    //{
                    //    return false;
                    //}
                    return true;
                }
            }
        }
        public override bool IsIncapacitatedOrOutOfGame
        {
            get
            {
                if (!IsIncapacitated)
                {
                    return IncapacitationCardController.Card.IsOutOfGame;
                }
                return true;
            }
        }

    }
}
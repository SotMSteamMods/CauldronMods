using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Vanish;

namespace CauldronTests
{
    [TestFixture()]
    public class VanishTests : CauldronBaseTest
    {
        #region HelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(vanish.CharacterCard, 1);
            DealDamage(villain, vanish, 2, DamageType.Melee);
        }

        #endregion HelperFunctions

        [Test()]
        [Order(0)]
        public void VanishLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vanish);
            Assert.IsInstanceOf(typeof(VanishCharacterCardController), vanish.CharacterCardController);

            foreach (var card in vanish.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(26, vanish.CharacterCard.HitPoints);
        }

        [Test()]
        public void VanishDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("ongoing", new[]
            {
                "ConcussiveBurst",
                "JauntingReflex",
                "Elusive",
                "TeleportBarrage",
                "Forewarned",
                "BlindsideJump",
            });

            AssertHasKeyword("equipment", new[]
            {
                "FocusingGauntlet",
                "TranslocationAccelerator",
            });

            AssertHasKeyword("limited", new[]
            {
                "ConcussiveBurst",
                "FocusingGauntlet",
                "JauntingReflex",
                "TranslocationAccelerator",
                "TeleportBarrage",
                "BlindsideJump",
            });

            AssertHasKeyword("one-shot", new[]
            {
                "FlickeringStrike",
                "FlashRecon",
                "TacticalRelocation",
                "Blink",
                "AbductAndAbandon",
            });
        }

        [Test]
        public void VanishInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            var mode = PlayCard("TurretMode");
            AssertInPlayArea(bunker, mode);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, minion);
            DecisionSelectCards = new Card[] { haka.CharacterCard, minion, bunker.CharacterCard, minion };
            UsePower(vanish);

            QuickHPCheck(0, 0, 0, 0, 0, -1);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, minion);
            UsePower(vanish);
            //check that bunker dealt the damage, if he did it will be increased by 1
            QuickHPCheck(0, 0, 0, 0, 0, -2);
        }


        [Test]
        public void VanishInnatePower_CantHitSelf()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SelectCardsForNextDecision(legacy.CharacterCard, legacy.CharacterCard);
            Assert.Throws<AssertionException>(() => UsePower(vanish), "SelectCardDecision selected \"Legacy\", which is not one of the decision options");
            
        }

        [Test]
        public void VanishIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);

            QuickHandStorage(haka, bunker, scholar);
            DecisionSelectTurnTaker = haka.TurnTaker;
            UseIncapacitatedAbility(vanish, 0);

            QuickHandCheck(1, 0, 0);
        }

        [Test]
        public void VanishIncap2_Replace()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);

            var topCard = baron.TurnTaker.Deck.TopCard;
            var bottomCard = baron.TurnTaker.Deck.BottomCard;
            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(baron.TurnTaker.Deck, true);

            UseIncapacitatedAbility(vanish, 1);

            AssertNumberOfCardsInRevealed(baron, 0);

            AssertOnTopOfDeck(topCard);
            AssertOnBottomOfDeck(bottomCard);
        }


        [Test]
        public void VanishIncap2_MoveToTop()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);

            var topCard = haka.TurnTaker.Deck.TopCard;
            var bottomCard = haka.TurnTaker.Deck.BottomCard;
            DecisionSelectLocation = new LocationChoice(haka.TurnTaker.Deck);
            DecisionMoveCardDestination = new MoveCardDestination(haka.TurnTaker.Deck, false);

            UseIncapacitatedAbility(vanish, 1);

            AssertNumberOfCardsInRevealed(haka, 0);
            AssertOnTopOfDeck(bottomCard);
            AssertOnTopOfDeck(topCard, 1);

        }

        [Test]
        public void VanishIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);

            AssertNumberOfStatusEffectsInPlay(0);

            DecisionSelectCard = wraith.CharacterCard;
            UseIncapacitatedAbility(vanish, 2);

            string messageText = $"Reduce damage dealt to {wraith.Name} by 1.";

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Test that the reducing effect works as expected
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have been reduced by 1
            QuickHPCheck(-2);

            PrintSeparator("Change turns");
            GoToEndOfTurn(wraith);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the reducing effect works as expected
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have been reduced by 1
            QuickHPCheck(-2);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(vanish);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the reducing effect has disappeared
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have not have been reduced
            QuickHPCheck(-3);
        }


        [Test]
        public void ConcussiveBurst_EffectApplied()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            GoToPlayCardPhase(vanish);
            var card = PlayCard("ConcussiveBurst");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);

            DealDamage(vanish, baron.CharacterCard, 1, DamageType.Melee);

            string messageText = $"Reduce damage dealt by {baron.Name} by 1.";

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, baron.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Test that the reducing effect works as expected
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have been reduced by 1
            QuickHPCheck(-2);

            PrintSeparator("Change turns");
            GoToEndOfTurn(wraith);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, baron.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the reducing effect works as expected
            QuickHPStorage(ra);
            DealDamage(baron, ra, 3, DamageType.Melee);
            //should have been reduced by 1
            QuickHPCheck(-2);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(vanish);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the reducing effect has disappeared
            QuickHPStorage(vanish);
            DealDamage(baron, vanish, 3, DamageType.Melee);
            //should have not have been reduced
            QuickHPCheck(-3);
        }

        [Test]
        public void ConcussiveBurst_OnlyFirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(vanish);
            var card = PlayCard("ConcussiveBurst");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);

            DealDamage(vanish, baron.CharacterCard, 1, DamageType.Melee);

            string messageText = $"Reduce damage dealt by {baron.Name} by 1.";

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, baron.TurnTaker);
            AssertStatusEffectsContains(messageText);

            DealDamage(vanish, minion, 1, DamageType.Melee);

            //no status effect applied
            AssertNumberOfStatusEffectsInPlay(1);
        }

        [Test]
        public void ConcussiveBurst_NotAppliedOnNoDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(vanish);
            var card = PlayCard("ConcussiveBurst");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);

            DealDamage(vanish, baron.CharacterCard, 1, DamageType.Melee);

            //baron immune to damage, no status effect
            AssertNumberOfStatusEffectsInPlay(0);

            DestroyCard("MobileDefensePlatform");

            //first damage dealt, apply
            DealDamage(vanish, baron.CharacterCard, 1, DamageType.Melee);

            string messageText = $"Reduce damage dealt by {baron.Name} by 1.";

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, baron.TurnTaker);
            AssertStatusEffectsContains(messageText);
        }

        [Test]
        public void ConcussiveBurst_NotAppliedToHeroTargets()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(vanish);
            var card = PlayCard("ConcussiveBurst");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);

            DealDamage(vanish, ra.CharacterCard, 1, DamageType.Melee);

            //baron immune to damage, no status effect
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void FlickeringStrike_Discard0()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            var mdp = GetCardInPlay("MobileDefensePlatform");

            GoToPlayCardPhase(vanish);


            DecisionSelectWordSkip = true;
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            QuickHandStorage(vanish, ra, wraith);
            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            var card = PlayCard("FlickeringStrike");
            AssertInTrash(vanish, card);

            //two cards drawn
            QuickHandCheck(2, 0, 0);
            //no damage dealt
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        [Test]
        public void FlickeringStrike_Discard1()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            var mdp = GetCardInPlay("MobileDefensePlatform");

            GoToPlayCardPhase(vanish);

            DecisionSelectCards = new Card[] { vanish.HeroTurnTaker.Hand.Cards.First(), mdp, null };
            QuickHandStorage(vanish, ra, wraith);
            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            var card = PlayCard("FlickeringStrike");
            AssertInTrash(vanish, card);

            //two cards drawn, 1 discarded
            QuickHandCheck(1, 0, 0);
            //1 unit of damage dealt
            QuickHPCheck(0, 0, 0, 0, -1);
        }

        [Test]
        public void FlickeringStrike_DiscardAll()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            var mdp = GetCardInPlay("MobileDefensePlatform");

            //stack deck & hand
            var card = GetCard("FlickeringStrike");
            MoveCard(vanish, card, vanish.HeroTurnTaker.Deck, true);
            AssertInDeck(card); //just to ensure our setup is working

            var sequence = new Card[]
            {
                GetTopCardOfDeck(vanish, 0),
                ra.CharacterCard,
                GetTopCardOfDeck(vanish, 1),
                wraith.CharacterCard,
                GetCardFromHand(vanish, 0),
                mdp,
                GetCardFromHand(vanish, 1),
                mdp,
                GetCardFromHand(vanish, 2),
                mdp,
                GetCardFromHand(vanish, 3),
                mdp,
                null
            };

            GoToPlayCardPhase(vanish);

            DecisionSelectCards = sequence;
            QuickHandStorage(vanish, ra, wraith);
            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            PlayCard(card);
            AssertInTrash(vanish, card);

            //two cards drawn, 6 discarded
            QuickHandCheck(-4, 0, 0);
            //1 unit of damage dealt
            QuickHPCheck(0, 0, -1, -1, -4);
        }

        [Test]
        public void FocusingGauntlet()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            var mdp = GetCardInPlay("MobileDefensePlatform");

            var card = PlayCard("FocusingGauntlet");
            AssertInPlayArea(vanish, card);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            DealDamage(vanish, mdp, 1, DamageType.Energy);
            QuickHPCheck(0, 0, 0, 0, -2);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            DealDamage(vanish, mdp, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, -1);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, mdp);
            DealDamage(baron, vanish, 1, DamageType.Energy);
            QuickHPCheck(0, -1, 0, 0, 0);
        }

        #region Test Flash Recon
        [Test]
        public void FlashRecon()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            //stack decks with harmless cards
            var played = StackDeck(baron, "MobileDefensePlatform");
            StackDeck(vanish, "FocusingGauntlet");
            StackDeck(ra, "FleshOfTheSunGod");
            StackDeck(wraith, "StunBolt");
            StackDeck(env, "PoliceBackup");

            DecisionSelectLocations = new[]
            {
                new LocationChoice(baron.TurnTaker.Deck),
                new LocationChoice(vanish.TurnTaker.Deck),
                new LocationChoice(ra.TurnTaker.Deck),
                new LocationChoice(wraith.TurnTaker.Deck)
                //new LocationChoice(env.TurnTaker.Deck), env deck is selected automatically since it's the last selection, yuck
            };
            DecisionSelectCard = played;

            var card = PlayCard("FlashRecon");
            AssertInTrash(vanish, card);

            AssertInPlayArea(baron, played);
            AssertNumberOfCardsInRevealed(baron, 0);
            AssertNumberOfCardsInRevealed(vanish, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(wraith, 0);
            AssertNumberOfCardsInRevealed(env, 0);
        }

        [Test]
        public void FlashReconOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Vanish", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            GoToStartOfTurn(vanish);

            Card aeonWarrior = GetCard("AeonWarrior");
            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: vanish.BattleZone.FindScion().PlayArea);
            PlayCard(oblivaeon, borrScion, overridePlayLocation: vanish.BattleZone.FindScion().PlayArea);


            AssertNumberOfChoicesInNextDecision(7, SelectionType.RevealTopCardOfDeck);
            AssertNumberOfChoicesInNextDecision(7, SelectionType.PutIntoPlay);
            PlayCard("FlashRecon");
           
        }

        [Test]
        public void FlashReconOblivAeonOtherBattleZone()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Vanish", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            GoToStartOfTurn(vanish);

            SwitchBattleZone(vanish);

            Card aeonWarrior = GetCard("AeonWarrior");
            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: vanish.BattleZone.FindScion().PlayArea);
            PlayCard(oblivaeon, borrScion, overridePlayLocation: vanish.BattleZone.FindScion().PlayArea);

            //vanish, scion deck, aeon man deck, environment
            AssertNumberOfChoicesInNextDecision(4, SelectionType.RevealTopCardOfDeck);
            AssertNumberOfChoicesInNextDecision(4, SelectionType.PutIntoPlay);
            PlayCard("FlashRecon");

        }

        [Test]
        public void FlashReconDarkMind()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Vanish", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective", scionIdentifiers: new List<string>() { "DarkMindCharacter" }) ;
            StartGame();

            SwitchBattleZone(vanish);
            SwitchBattleZone(legacy);
            AssertBattleZone(mindScion, bzTwo);
            AssertBattleZone(vanish, bzTwo);
            AssertBattleZone(legacy, bzTwo);

            PutOnDeck("TacticalRelocation");
            PutOnDeck("Elusive");

            //the test doesn't show the cards to choose from as flipped, but they are in the UI
            GoToStartOfTurn(vanish);
            PlayCard("FlashRecon");

        }

        [Test]
        public void FlashRecon_OnlyCleanUpOwnRevealed()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "TheArgentAdept/DarkConductorArgentAdept", "Megalopolis");
            StartGame();

            //stack decks with harmless cards
            var played = StackDeck(baron, "MobileDefensePlatform");
            StackDeck(vanish, "FocusingGauntlet");
            StackDeck(ra, "FleshOfTheSunGod");
            StackDeck(wraith, "StunBolt");
            StackDeck(env, "PoliceBackup");

            Card adeptDiscard = PutOnDeck("TheStaffOfRa");
            Card adeptPlay = PutOnDeck("FlashRecon");

            DecisionSelectLocations = new[]
            {
                new LocationChoice(vanish.TurnTaker.Deck),
                new LocationChoice(ra.TurnTaker.Deck),
                new LocationChoice(baron.TurnTaker.Deck),
                new LocationChoice(vanish.TurnTaker.Deck),
                new LocationChoice(ra.TurnTaker.Deck),
                new LocationChoice(wraith.TurnTaker.Deck),
                new LocationChoice(adept.TurnTaker.Deck)
                //new LocationChoice(env.TurnTaker.Deck), env deck is selected automatically since it's the last selection, yuck
            };
            DecisionSelectCards = new Card[] { adeptPlay, played };

            UsePower(adept);
            AssertInTrash(vanish, adeptPlay);
            AssertInTrash(ra, adeptDiscard);

            AssertInPlayArea(baron, played);
            AssertNumberOfCardsInRevealed(baron, 0);
            AssertNumberOfCardsInRevealed(vanish, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(wraith, 0);
            AssertNumberOfCardsInRevealed(env, 0);
        }

        [Test]
        public void FlashRecon_Fix942()
        {
            Card mobileDefensePlatform;
            Card focusingGauntlet;
            Card fleshOfTheSunGod;
            Card stunBolt;
            Card policeBackup;

            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            //stack decks with harmless cards
            mobileDefensePlatform = StackDeck(baron, "MobileDefensePlatform");
            focusingGauntlet = StackDeck(vanish, "FocusingGauntlet");
            fleshOfTheSunGod = StackDeck(ra, "FleshOfTheSunGod");
            stunBolt = StackDeck(wraith, "StunBolt");
            policeBackup = StackDeck(env, "PoliceBackup");

            DecisionSelectLocations = new[]
            {
                new LocationChoice(baron.TurnTaker.Deck),
                new LocationChoice(vanish.TurnTaker.Deck),
                new LocationChoice(ra.TurnTaker.Deck),
                new LocationChoice(wraith.TurnTaker.Deck)
                //new LocationChoice(env.TurnTaker.Deck), env deck is selected automatically since it's the last selection, yuck
            };
            AssertNextDecisionChoices(DecisionSelectLocations, null);
            AssertNextDecisionChoices(new Card[] { mobileDefensePlatform, focusingGauntlet, fleshOfTheSunGod, stunBolt, policeBackup }, new Card[] { vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard });
            PlayCard("FlashRecon");

            AssertInPlayArea(baron, mobileDefensePlatform);
            AssertNumberOfCardsInRevealed(baron, 0);
            AssertNumberOfCardsInRevealed(vanish, 0);
            AssertNumberOfCardsInRevealed(ra, 0);
            AssertNumberOfCardsInRevealed(wraith, 0);
            AssertNumberOfCardsInRevealed(env, 0);
        }
        #endregion Test Flash Recon

        public void TacticalRelocation_()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, wraith.TurnTaker };
            QuickHPStorage(baron, vanish, ra, wraith);
            //will use the base power and deal some damage or something, don't matter. we just check it was used.
            var card = PlayCard("TacticalRelocation");
            AssertInTrash(vanish, card);

            QuickHPCheck(0, 0, -3, -3);

            Assert.Fail("Test not implemented");
        }

        [Test]
        public void Blink()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            var card = GetCard("Blink");
            PutInHand(card);
            var drawn = vanish.TurnTaker.Deck.TopCard;
            var played = PutInHand("ConcussiveBurst");

            DecisionSelectCardToPlay = played;

            //will use the base power and deal some damage or something, don't matter. we just check it was used.
            PlayCard(card);
            AssertInTrash(vanish, card);

            AssertNotUsablePower(vanish, vanish.CharacterCard);
            AssertInHand(drawn);
            AssertInPlayArea(vanish, played);
        }

        [Test]
        public void JauntingReflex_Play()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);

        }


        [Test]
        public void JauntingReflex_ReactionPowerDeclined()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();


            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);
            AssertNumberOfUsablePowers(vanish, 2); //base + reflex

            DecisionDoNotSelectCard = SelectionType.DiscardCard;

            DealDamage(baron, ra, 1, DamageType.Energy);

            //no power used
            AssertNumberOfUsablePowers(vanish, 2); //base + reflex
        }

        [Test]
        public void JauntingReflex_ReactionPowerAccepted()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();


            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);
            AssertNumberOfUsablePowers(vanish, 2); //base + reflex

            DealDamage(baron, ra, 1, DamageType.Energy);

            AssertNumberOfUsablePowers(vanish, 1);
        }

        [Test]
        public void JauntingReflex_ReactionPowerOncePerTurn()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();


            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);
            AssertNumberOfUsablePowers(vanish, 2); //base + reflex

            DealDamage(baron, ra, 1, DamageType.Energy);

            AssertNumberOfUsablePowers(vanish, 1);

            DealDamage(baron, ra, 1, DamageType.Energy);

            AssertNumberOfUsablePowers(vanish, 1);

            GoToStartOfTurn(vanish);

            AssertNumberOfUsablePowers(vanish, 2); //base + reflex

            DealDamage(baron, ra, 1, DamageType.Energy);

            AssertNumberOfUsablePowers(vanish, 1);

        }

        [Test]
        public void JauntingReflex_ReactionPowerNotHeroDamageNotOthers()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();


            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);
            AssertNumberOfUsablePowers(vanish, 2); //base + reflex
            AssertNumberOfUsablePowers(ra, 1);
            AssertNumberOfUsablePowers(wraith, 1);

            DealDamage(wraith, ra, 1, DamageType.Energy);

            AssertNumberOfUsablePowers(vanish, 2); //base + reflex
            AssertNumberOfUsablePowers(ra, 1);
            AssertNumberOfUsablePowers(wraith, 1);
        }


        [Test]
        public void JauntingReflex_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(vanish);

            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);

            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, ra, wraith);
            UsePower(card);
            AssertInTrash(card);
            QuickHandCheck(2, 0, 0);


        }

        [Test]
        public void JauntingReflex_UsePowerIndestructible()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            var card = PlayCard("JauntingReflex");
            AssertInPlayArea(vanish, card);

            var effect = new MakeIndestructibleStatusEffect();
            effect.CardsToMakeIndestructible.IsSpecificCard = card;
            effect.UntilThisTurnIsOver(base.GameController.Game);
            base.RunCoroutine(GameController.AddStatusEffect(effect, false, base.GetCardController(card).GetCardSource()));

            QuickHandStorage(vanish, ra, wraith);
            UsePower(card);
            //card not destroyed, no cards drawn
            AssertInPlayArea(vanish, card);
            QuickHandCheck(0, 0, 0);
        }

        [Test]
        public void AbductAndAbandon_Play()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            var target = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCard = target;

            var card = PlayCard("AbductAndAbandon");
            AssertInTrash(vanish, card);
            AssertOnTopOfDeck(target);
        }

        [Test]
        public void AbductAndAbandon_IndestructibleCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "TimeCataclysm");
            StartGame();

            var target = GetCardInPlay("MobileDefensePlatform");

            PlayCard("FixedPoint");

            DecisionSelectCard = target;

            var card = PlayCard("AbductAndAbandon");

            //AssertInTrash(vanish, card);
            //AssertOnTopOfDeck(target);

            AssertInTrash(vanish, card);
            AssertInPlayArea(baron, target);
        }

        [Test]
        public void AbductAndAbandon_TestTitleReturn()
        {
            SetupGameController("KaargraWarfang", "Cauldron.Vanish", "Ra", "TheWraith", "TimeCataclysm");
            StartGame();

            var title = FindCard(c => c.IsTitle && c.IsInPlay);


            DecisionSelectCard = title;

            var card = PlayCard("AbductAndAbandon");

            //AssertOnTopOfDeck(title);
            AssertAtLocation(title, title.NativeDeck);

            //AssertInTrash(vanish, card);
            //AssertInPlayArea(baron, target);
        }


        [Test]
        public void TranslocationAccelerator()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "TimeCataclysm");
            StartGame();

            var target = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = target;

            var card = PlayCard("TranslocationAccelerator");
            AssertInPlayArea(vanish, card);
            Assert.IsTrue(card.IsLimited);

            var power = PlayCard("JauntingReflex"); //use this power since it doesn't deal damage itself

            DecisionSelectTarget = target;
            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard, target);
            UsePower(power);

            QuickHPCheck(0, 0, 0, 0, -1);

            //check that ra's power useage doesn't trigger the effect
            GoToUsePowerPhase(ra);

            QuickHPUpdate();
            UsePower(ra);
            QuickHPCheck(0, 0, 0, 0, -2);
        }

        [Test]
        public void Elusive_Play()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            var card = PlayCard("Elusive");
            AssertInPlayArea(vanish, card);
        }

        [Test]
        public void Elusive_PowerDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("Elusive");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHandStorage(vanish, ra, wraith);
            DecisionSelectFunction = 1;
            UsePower(card);

            AssertNumberOfStatusEffectsInPlay(1);
            QuickHandCheck(1, 0, 0);

            QuickHPStorage(baron, vanish, ra, wraith);
            DealDamage(baron.CharacterCard, new Card[] { baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard }, 1, DamageType.Cold);
            QuickHPCheck(-1, 0, -1, -1);

            AssertNumberOfStatusEffectsInPlay(0);

            QuickHPUpdate();
            DealDamage(baron.CharacterCard, new Card[] { baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard }, 1, DamageType.Cold);
            QuickHPCheck(-1, -1, -1, -1);

        }

        [Test]
        public void Elusive_PowerPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            //preload a harmless card to play
            var play = PutInHand(vanish, "Forewarned");

            var card = PlayCard("Elusive");
            AssertInPlayArea(vanish, card);

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHandStorage(vanish, ra, wraith);
            DecisionSelectFunction = 0;
            DecisionSelectCard = play;
            UsePower(card);
            AssertInPlayArea(vanish, play);

            AssertNumberOfStatusEffectsInPlay(1);
            QuickHandCheck(-1, 0, 0);

            QuickHPStorage(baron, vanish, ra, wraith);
            DealDamage(baron.CharacterCard, new Card[] { baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard }, 1, DamageType.Cold);
            QuickHPCheck(-1, 0, -1, -1);

            AssertNumberOfStatusEffectsInPlay(0);

            QuickHPUpdate();
            DealDamage(baron.CharacterCard, new Card[] { baron.CharacterCard, vanish.CharacterCard, ra.CharacterCard, wraith.CharacterCard }, 1, DamageType.Cold);
            QuickHPCheck(-1, -1, -1, -1);

        }

        [Test]
        public void TeleportBarrage_PowerDiscard0()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("TeleportBarrage");
            AssertInPlayArea(vanish, card);

            QuickHPStorage(baron, vanish, ra, wraith);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(card);

            QuickHPCheck(-2, 0, 0, 0);
        }

        [Test]
        public void TeleportBarrage_PowerDiscard3()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("TeleportBarrage");
            AssertInPlayArea(vanish, card);

            var sequence = new Card[]
            {
                baron.CharacterCard,
                GetCardFromHand(vanish, 0),
                baron.CharacterCard,
                GetCardFromHand(vanish, 1),
                baron.CharacterCard,
                GetCardFromHand(vanish, 2),
                baron.CharacterCard,
                GetCardFromHand(vanish, 3),
                baron.CharacterCard,
                null
            };

            QuickHPStorage(baron, vanish, ra, wraith);
            DecisionSelectCards = sequence;

            UsePower(card);

            QuickHPCheck(-5, 0, 0, 0);
        }

        [Test]
        public void Forewarned_Yes()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            DiscardAllCards(vanish, ra, wraith);

            var card = PlayCard("Forewarned");
            AssertInPlayArea(vanish, card);

            DecisionYesNo = true;

            QuickHandStorage(vanish, ra, wraith);
            GoToStartOfTurn(vanish);

            QuickHandCheck(3, 3, 3);
            AssertInTrash(vanish, card);
        }

        [Test]
        public void Forewarned_No()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            DiscardAllCards(vanish, ra, wraith);

            var card = PlayCard("Forewarned");
            AssertInPlayArea(vanish, card);

            DecisionYesNo = false;

            QuickHandStorage(vanish, ra, wraith);
            GoToStartOfTurn(vanish);
            QuickHandCheck(0, 0, 0);
            AssertInPlayArea(vanish, card);

            //check that the decision doesn't trigger on other starts
            DecisionYesNo = true;
            GoToStartOfTurn(ra);
            QuickHandCheck(0, 0, 0);

            AssertInPlayArea(vanish, card);
        }

        [Test]
        public void Forewarned_YesIndestructible()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            DiscardAllCards(vanish, ra, wraith);

            var card = PlayCard("Forewarned");
            AssertInPlayArea(vanish, card);

            DecisionYesNo = true;

            var effect = new MakeIndestructibleStatusEffect();
            effect.UntilCardLeavesPlay(card);
            effect.CardsToMakeIndestructible.IsSpecificCard = card;
            effect.CardSource = card;
            var cardSource = GetCardController(card).GetCardSource();
            RunCoroutine(GameController.AddStatusEffect(effect, true, cardSource));

            QuickHandStorage(vanish, ra, wraith);
            GoToStartOfTurn(vanish);

            QuickHandCheck(0, 0, 0);
            AssertInPlayArea(vanish, card);
        }

        [Test]
        public void ForewarnedOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Vanish", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();


            //prep - all heroes now have 0 cards in hand
            DiscardAllCards(vanish, legacy, haka);

            //move legacy to other battlezone
            SwitchBattleZone(legacy);

            PlayCard("Forewarned");

            //go to start of turn to trigger draws
            DecisionYesNo = true;
            GoToStartOfTurn(vanish);

            AssertNumberOfCardsInHand(vanish, 3);
            AssertNumberOfCardsInHand(legacy, 0);
            AssertNumberOfCardsInHand(haka, 3);
        }

        [Test]
        public void BlindsideJump()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            var card = PlayCard("BlindsideJump");
            AssertInPlayArea(vanish, card);

            GoToStartOfTurn(vanish);

            QuickHPStorage(baron, vanish, ra, wraith);
            DecisionSelectTarget = baron.CharacterCard;

            UsePower(card);

            QuickHPCheck(-1, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(vanish, baron, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(ra, baron, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);

            QuickHPUpdate();
            DealDamage(wraith, ra, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0);

        }

        [Test]
        public void BlindsideJump_Redirection()
        {
            SetupGameController("KaargraWarfang", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            //Playing Tarnis when he's already in play messes up his redirection, so let's avoid that
            PutOnDeck("ProvocatorTarnis");

            var target1 = PlayCard("IdesaTheAdroit");
            var target2 = PlayCard("ProvocatorTarnis");
            DestroyCards(FindCardsWhere(c => c.IsTitle && c.IsInPlay));

            var card = PlayCard("BlindsideJump");
            AssertInPlayArea(vanish, card);

            QuickHPStorage(target1, target2);
            AssertNumberOfStatusEffectsInPlay(0);
            DecisionSelectCard = target1;
            UsePower(card);
            QuickHPCheck(0, -1);

            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectsDoesNotContain(target1.Title);
            AssertStatusEffectsContains(target2.Title);
        }

        [Test]
        public void TacticalRelocation()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis");
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Vanish", "Ra", "TheWraith", "Megalopolis" }, randomSeed: -1689251121);
            StartGame();

            RemoveMobileDefensePlatform();

            //wraith should play stunbolt, set that in trash
            //record hard as it should not change
            var t1 = GetCard("StunBolt");
            PutInTrash(wraith, t1);
            var hand1 = wraith.HeroTurnTaker.Hand.Cards.ToList();

            //vanish takes no damage, so draws and discards, set those up
            var t2 = GetCardFromHand(vanish);
            var card = PutInHand("TacticalRelocation");

            var f1 = GetCard("FocusingGauntlet");
            PutInTrash(vanish, f1); //should not be played

            var t3 = GetTopCardOfDeck(vanish);

            //ra takes damage, but has no valid cards, nothing should happen
            var hand2 = ra.HeroTurnTaker.Hand.Cards.ToList();

            DecisionSelectCards = new Card[]
            {
                //heros to damage
                ra.CharacterCard,
                wraith.CharacterCard,
                null,

                //cards to put into play
                t1,

                //card to discard,
                t2
            };
            DecisionYesNo = true;

            QuickHPStorage(baron, vanish, ra, wraith);
            PlayCard(card);
            AssertInTrash(vanish, card);

            QuickHPCheck(0, 0, -3, -3);

            AssertInPlayArea(wraith, t1);
            AssertInTrash(vanish, t2);
            AssertInHand(vanish, t3);
            AssertInTrash(vanish, f1);

            AssertInHand(hand1);
            AssertInHand(hand2);
        }
        [Test]
        public void TacticalRelocation_RecoveryRequiresDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "TheScholar", "TheWraith", "Megalopolis");
            StartGame();

            RemoveMobileDefensePlatform();

            PutOnDeck(scholar, scholar.HeroTurnTaker.Hand.Cards);

            Card liveIron = PlayCard("FleshToIron");
            Card deadIron = PutInTrash("FleshToIron");
            Card liquid = PutInTrash("SolidToLiquid");

            AssertIsInPlay(liveIron);
            AssertInTrash(deadIron);

            DecisionYesNo = true;

            PlayCard("TacticalRelocation");
            AssertIsInPlay(deadIron);

            PlayCard("TacticalRelocation");
            AssertNotInPlay(liquid);
        }
        [Test]
        public void TacticalRelocation_HeroTargetsDamaged()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Tachyon", "TheWraith", "Megalopolis");
            StartGame();

            Card bolt = PutInTrash("StunBolt");
            Card synaptic = PlayCard("SynapticInterruption");
            DecisionSelectCards = new Card[] { tachyon.CharacterCard, wraith.CharacterCard, null, bolt, null };

            QuickHPStorage(vanish, tachyon, wraith);
            DecisionsYesNo = new bool[] { true, false, false, false, false, false, };
            PlayCard("TacticalRelocation");

            QuickHPCheck(0, 0, -3);
            AssertInTrash(synaptic);
            AssertIsInPlay(bolt);
        }
        [Test]
        public void TacticalRelocation_DrawAndDiscardOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Tachyon", "TheWraith", "Megalopolis");
            StartGame();

            DecisionSelectTargets = new Card[] { null };

            DecisionYesNo = false;
            PlayCard("TacticalRelocation");
            AssertNumberOfCardsInTrash(vanish, 1);
            AssertNumberOfCardsInTrash(tachyon, 0);
            AssertNumberOfCardsInTrash(wraith, 0);
        }
    }
}

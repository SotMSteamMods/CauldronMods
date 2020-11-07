﻿using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheKnight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheKnightTests : BaseTest
    {
        #region HelperFunctions
        protected string HeroNamespace = "Cauldron.TheKnight";

        protected HeroTurnTakerController HeroController { get { return FindHero("TheKnight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(HeroController, 1);
            DealDamage(villain, HeroController, 2, DamageType.Melee);
        }
        
        #endregion

        [Test]
        [Order(1)]
        public void TheKnightCharacter_Load()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(HeroController);
            Assert.IsInstanceOf(typeof(TheKnightCharacterCardController), HeroController.CharacterCardController);

            Assert.AreEqual(32, HeroController.CharacterCard.HitPoints);
        }

        [Test]
        [Order(1)]
        public void TheKnightCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            PrintSeparator("Test");
            Assert.IsTrue(HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).All(c => c is TheKnightCardController), "Not all cards are derived from " + nameof(Cauldron.TheKnight.TheKnightCardController));

            var cc = (TheKnightCardController)HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).First();

            List<SelectCardDecision> results = new List<SelectCardDecision>();

            RunCoroutine(cc.SelectOwnCharacterCard(results, SelectionType.None));

            Assert.IsTrue(results.Count == 1 && results[0].SelectedCard == HeroController.CharacterCard, "Own CharacterCard was not selected");
        }

        [Test]
        [Order(1)]
        public void SingleHandEquipmentCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            PrintSeparator("Test");
            var ccs = HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).Cast<TheKnightCardController>().ToList();

            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => cc.IsSingleHandCard(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the single hand keyword");
            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => base.IsEquipment(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the equipment keyword");
            Assert.IsTrue(ccs.Where(cc => !(cc is SingleHandEquipmentCardController)).All(cc => !cc.IsSingleHandCard(cc.Card)), $"A card has the single hand keyword but does not derive from {nameof(SingleHandEquipmentCardController)}");

            PrintSeparator("put all Single Hand cards into hand");
            foreach(var card in ccs.OfType<SingleHandEquipmentCardController>().Select(cc => cc.Card))
            {
                PutInHand(card);
            }

            PrintSeparator("play them all, first two normal");
            PlayCardFromHand(HeroController, HeroController.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(HeroController, 2);
            AssertNumberOfCardsInTrash(HeroController, 0);

            PlayCardFromHand(HeroController, HeroController.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(HeroController, 3);
            AssertNumberOfCardsInTrash(HeroController, 0);

            PrintSeparator("subsequent cards should destroy a previous card");
            int cardsInTrash = 0;
            while (HeroController.HeroTurnTaker.Hand.HasCards)
            {
                DecisionSelectCard = HeroController.TurnTaker.PlayArea.Cards.First(c => !c.IsHeroCharacterCard);
                PlayCardFromHand(HeroController, HeroController.HeroTurnTaker.Hand.Cards.First().Identifier);
                AssertNumberOfCardsInPlay(HeroController, 3);
                AssertNumberOfCardsInTrash(HeroController, ++cardsInTrash);
            }
        }


        [Test]
        [Description("TheKnight Innate - {TheKnight} deals 1 target 1 melee damage.")]
        public void TheKnightCharacter_InnatePower()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PrintSeparator("Test");
            GoToUsePowerPhase(HeroController);
            DecisionSelectFunction = 0;
            DecisionSelectTarget = base.FindCardInPlay("BaronBladeCharacter");
            QuickHPStorage(DecisionSelectTarget);
            UsePower(HeroController.CharacterCard);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight Incap 1 - One player may play a card now.")]
        public void TheKnightCharacter_Incap1()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            //set up legacy
            PutInHand(legacy, "NextEvolution");
            QuickHandStorage(legacy);

            PrintSeparator("Test");
            GoToUseIncapacitatedAbilityPhase(HeroController);

            PrintSeparator("use incap");
            DecisionSelectCard = legacy.GetCardsAtLocation(legacy.HeroTurnTaker.Hand).First(c => c.Identifier == "NextEvolution");
            UseIncapacitatedAbility(HeroController, 0);

            PrintSeparator("assert card was played");
            QuickHandCheck(-1);
            Assert.IsTrue(legacy.GetCardsAtLocation(legacy.HeroTurnTaker.PlayArea).Count(c => c.Identifier == "NextEvolution") == 1);
        }

        [Test]
        [Description("TheKnight Incap 2 - Reduce damage dealt to 1 target by 1 until the start of your next turn.")]
        public void TheKnightCharacter_Incap2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(HeroController, 1);

            string messageText = $"Reduce damage dealt to {legacy.Name} by 1.";

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Change turns");
            GoToEndOfTurn(legacy);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        [Description("TheKnight Incap 3 - Increase damage dealt by 1 target by 1 until the start of your next turn.")]
        public void TheKnightCharacter_Incap3()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(HeroController, 2);

            string messageText = $"Increase damage dealt by {legacy.Name} by 1.";

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Change turns");
            GoToEndOfTurn(legacy);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - No Cards to Select")]
        public void ArmYourself_NoCards()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            
            PutInHand(HeroController, "ArmYourself");

            //Put some random cards non-Equipment in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(10));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            AssertNoDecision();
            PlayCardFromHand(HeroController, "ArmYourself");
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Choose skip")]
        public void ArmYourself_Skip()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            DecisionSelectCards = new Card[] { null, null };
            PlayCardFromHand(HeroController, "ArmYourself");
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Choose 2")]
        public void ArmYourself_Choose2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            //exclude PlateHelm since it draws a card
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) && c.Identifier != "PlateHelm").Take(5));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            DecisionSelectCards = HeroController.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(2);
            DecisionSelectLocation = new LocationChoice(HeroController.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(HeroController, "ArmYourself");
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(HeroController, 2);
        }

        [Test]
        [Description("TheKnight - Battlefield Scavenger - Draw Cards")]
        public void BattlefieldScavenger_Draw()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));

            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => c.IsOngoing).Take(5));
            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOngoing).Take(5));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController, legacy);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 0, 0 };
            PlayCardFromHand(HeroController, "BattlefieldScavenger");
            QuickHandCheck(0, 1);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Battlefield Scavenger - Top Deck Cards")]
        public void BattlefieldScavenger_TopDeck()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));

            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => c.IsOngoing).Take(5));
            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOngoing).Take(5));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController, legacy);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 1, 1 };
            var cards = HeroController.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1).Concat(
                legacy.HeroTurnTaker.Trash.Cards.Where(c => c.IsOngoing).Take(1)).ToList();
            DecisionSelectCards = cards;

            PlayCardFromHand(HeroController, "BattlefieldScavenger");
            AssertOnTopOfDeck(HeroController, cards[0]);
            AssertOnTopOfDeck(legacy, cards[1]);
            QuickHandCheck(-1, 0);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Catch Your Breath")]
        public void CatchYourBreath()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            DealDamage(baron, HeroController.CharacterCard, 5, DamageType.Psychic);
            PutInHand(HeroController, "CatchYourBreath");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            QuickHPStorage(HeroController);

            PlayCardFromHand(HeroController, "CatchYourBreath");

            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandCheck(0);
            QuickHPCheck(2);
        }


        [Test]
        [Description("TheKnight - ChampionOfTheRealm")]
        public void ChampionOfTheRealm()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "ChampionOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "ShortSword");
            string messageText = $"Increase damage dealt by {legacy.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(legacy.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(HeroController);
            PlayCardFromHand(HeroController, "ChampionOfTheRealm");

            PrintSeparator("Assert Played");
            AssertInPlayArea(HeroController, card);

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Change turns");
            GoToEndOfTurn(legacy);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        [Description("TheKnight - DefenderOfTheRealm")]
        public void DefenderOfTheRealm()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "DefenderOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "PlateMail");
            string messageText = $"Reduce damage dealt to {legacy.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(legacy.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(HeroController);
            PlayCardFromHand(HeroController, "DefenderOfTheRealm");

            PrintSeparator("Assert Played");
            AssertInPlayArea(HeroController, card);

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Change turns");
            GoToEndOfTurn(legacy);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        [Description("TheKnight - HeavySwing")]
        public void HeavySwing()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "HeavySwing");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCardFromHand(HeroController, "HeavySwing");
            QuickHPCheck(-3);
        }

        [Test]
        [Description("TheKnight - KnightsHonor")]
        public void KnightsHonor()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Ra", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            PutInHand(HeroController, "KnightsHonor");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            DecisionNextSelectionType = SelectionType.MoveCardNextToCard;
            DecisionNextToCard = legacy.CharacterCard;
            PlayCardFromHand(HeroController, "KnightsHonor");

            PrintSeparator("check redirect from legacy to knight");
            QuickHPStorage(HeroController, legacy, ra);
            DealDamage(baron, legacy, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("check no redirect from ra to knight");
            QuickHPStorage(HeroController, legacy, ra);
            DealDamage(baron, ra, 4, DamageType.Psychic);
            QuickHPCheck(0, 0, -4);

            PrintSeparator("check redirect damage is irreducible");
            PlayCard(HeroController, "StalwartShield");
            QuickHPStorage(HeroController, legacy, ra);
            DealDamage(baron, legacy, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("Test Power");
            UsePower("KnightsHonor");
            AssertInTrash("KnightsHonor");

            PrintSeparator("no redirect redirect after card leaves play");
            QuickHPStorage(HeroController, legacy, ra);
            DealDamage(baron, legacy, 4, DamageType.Psychic);
            QuickHPCheck(0, -5, 0);
        }

        [Test]
        [Description("TheKnight - MaidensBlessing")]
        public void MaidensBlessing()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            //prime a hand and top of deck
            var testCard = PutInHand(HeroController, "MaidensBlessing");
            var equipCard = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "Whetstone");
            PutInHand(equipCard);
            PutInHand(HeroController, HeroController.HeroTurnTaker.Deck.Cards.First(c => !IsEquipment(c)));
            PutOnDeck(HeroController, HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(3));

            PrintSeparator("Play the Card");           
            GoToPlayCardPhase(HeroController);
            QuickHandStorage(HeroController);
            AssertNumberOfCardsInPlay(HeroController, 1);
            PlayCardFromHand(HeroController, "MaidensBlessing");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);
            GoToUsePowerPhase(HeroController);

            PrintSeparator("Use power expect equipCard to be played, and a card draw");
            DecisionSelectCard = equipCard;
            QuickHandStorage(HeroController);
            UsePower(testCard);
            QuickHandCheck(0); //plays a card, draws a card
            AssertNumberOfCardsInPlay(HeroController, 3);
            AssertInPlayArea(HeroController, equipCard);

            PrintSeparator("Use power, no card to play, expect card draw");
            DecisionSelectCard = null;
            QuickHandStorage(HeroController);
            UsePower(testCard);
            AssertNoDecision(SelectionType.PlayCard);
            QuickHandCheck(1);
            AssertNumberOfCardsInPlay(HeroController, 3);
        }

        [Test]
        [Description("TheKnight - PlateHelm")]
        public void PlateHelm()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            var testCard = PutInHand(HeroController, "PlateHelm");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");

            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, testCard.Identifier);
            QuickHandCheck(0);
            AssertInPlayArea(HeroController, testCard);
            AssertIsTarget(testCard, 3);

            GoToStartOfTurn(baron);

            PrintSeparator("check redirect from knight to helm");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from legacy");
            DecisionYesNo = true;
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, legacy.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -2); //nemesis

            PrintSeparator("check no redirect from helm");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after helm leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(HeroController, testCard);
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);
        }

        [Test]
        [Description("TheKnight - PlateMail")]
        public void PlateMail()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            var testCard = PutInHand(HeroController, "PlateMail");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");

            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, testCard.Identifier);
            QuickHandCheck(-1);
            AssertInPlayArea(HeroController, testCard);
            AssertIsTarget(testCard, 5);

            GoToStartOfTurn(baron);

            PrintSeparator("check redirect from knight to mail");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from legacy");
            DecisionYesNo = true;
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, legacy.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -2); //nemesis

            PrintSeparator("check no redirect from helm");
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after mail leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(HeroController, testCard);
            QuickHPStorage(HeroController.CharacterCard, testCard, legacy.CharacterCard);
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);
        }

        [Test]
        [Description("TheKnight - Short Sword - Increase Damage Triggers")]
        public void ShortSword_Trigger()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            PutInHand(HeroController, "ShortSword");
            PutInHand(HeroController, "ShortSword");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "ShortSword");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 1");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-2); //increased by 1

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(baron);
            DealDamage(legacy, baron, 1, DamageType.Radiant);
            QuickHPCheck(-2);

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "ShortSword");
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 2");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-3); //increased by 1

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard("ShortSword");
            DestroyCard("ShortSword");

            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight - Short Sword - Power")]
        public void ShortSword_Power()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            var testCard = PutInHand(HeroController, "ShortSword");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "ShortSword");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(testCard);
            QuickHPCheck(-3); 
        }

        [Test]
        [Description("TheKnight - StalwartShield")]
        public void StalwartShield()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            PutInHand(HeroController, "StalwartShield");
            PutInHand(HeroController, "StalwartShield");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "StalwartShield");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 1");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 2, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 1

            PrintSeparator("Irreducible not effected");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 2, DamageType.Energy, true);
            QuickHPCheck(-2);

            PrintSeparator("Other Damage not reduced");
            QuickHPStorage(baron);
            DealDamage(legacy, baron, 1, DamageType.Radiant);
            QuickHPCheck(-2);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "StalwartShield");
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 2");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 3, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard("StalwartShield");
            DestroyCard("StalwartShield");

            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 1, DamageType.Energy);
            QuickHPCheck(-1);
        }


        [Test]
        [Description("TheKnight - SureFooting")]
        public void SureFooting()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            //prime a hand and top of deck
            var testCard = PutInHand(HeroController, "SureFooting");
            var oneshotCard = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "HeavySwing");
            PutInHand(oneshotCard);
            PutInHand(HeroController, HeroController.HeroTurnTaker.Deck.Cards.First(c => !c.IsOneShot));
            PutOnDeck(HeroController, HeroController.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOneShot).Take(3));

            PrintSeparator("Play the Card");
            GoToPlayCardPhase(HeroController);
            QuickHandStorage(HeroController);
            AssertNumberOfCardsInPlay(HeroController, 1);
            PlayCardFromHand(HeroController, "SureFooting");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);
            GoToUsePowerPhase(HeroController);

            PrintSeparator("Use power expect oneshotCard to be played, and a card draw");
            DecisionSelectCard = oneshotCard;
            QuickHandStorage(HeroController);
            UsePower(testCard);
            QuickHandCheck(0); //plays a card, draws a card
            AssertNumberOfCardsInPlay(HeroController, 2);
            AssertInTrash(oneshotCard);

            PrintSeparator("Use power, no card to play, expect card draw");
            DecisionSelectCard = null;
            QuickHandStorage(HeroController);
            UsePower(testCard);
            AssertNoDecision(SelectionType.PlayCard);
            QuickHandCheck(1);
            AssertNumberOfCardsInPlay(HeroController, 2);
        }

        [Test]
        [Description("TheKnight - SwiftStrikes")]
        public void SwiftStrikes()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("SwiftStrikes");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron, legacy);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, legacy.CharacterCard };
            PlayCardFromHand(HeroController, "SwiftStrikes");
            QuickHPCheck(-1, -1);
        }

        [Test]
        [Description("TheKnight - Whetstone")]
        public void Whetstone()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            PutInHand(HeroController, "Whetstone");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCardFromHand(HeroController, "Whetstone");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            PrintSeparator("Melee Damage increased by 1");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Melee);
            QuickHPCheck(-2); //increased by 1

            PrintSeparator("Non-Melee Damage not increased");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Fire);
            QuickHPCheck(-1);

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(baron);
            DealDamage(legacy, baron, 1, DamageType.Melee);
            QuickHPCheck(-2);

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard("Whetstone");

            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}

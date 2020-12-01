using Handelabra;
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(HeroController);
            Assert.IsInstanceOf(typeof(TheKnightCharacterCardController), HeroController.CharacterCardController);

            Assert.AreEqual(32, HeroController.CharacterCard.HitPoints);
        }

        [Test]
        [Order(1)]
        public void TheKnightCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            //set up wraith
            var card = wraith.GetCardsAtLocation(wraith.HeroTurnTaker.Deck).First(c => c.IsOngoing || IsEquipment(c));
            PutInHand(wraith, card);
            QuickHandStorage(wraith);

            PrintSeparator("Test");
            GoToUseIncapacitatedAbilityPhase(HeroController);

            PrintSeparator("use incap");
            DecisionSelectTurnTaker = wraith.TurnTaker;
            DecisionSelectCard = card;
            UseIncapacitatedAbility(HeroController, 0);

            PrintSeparator("assert card was played");
            QuickHandCheck(-1);
            AssertInPlayArea(wraith, card);
        }

        [Test]
        [Description("TheKnight Incap 2 - Reduce damage dealt to 1 target by 1 until the start of your next turn.")]
        public void TheKnightCharacter_Incap2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = wraith.CharacterCard;
            UseIncapacitatedAbility(HeroController, 1);

            string messageText = $"Reduce damage dealt to {wraith.Name} by 1.";

            PrintSeparator("Assert Applied");
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
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the reducing effect has disappeared
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have not have been reduced
            QuickHPCheck(-3);
        }

        [Test]
        [Description("TheKnight Incap 3 - Increase damage dealt by 1 target by 1 until the start of your next turn.")]
        public void TheKnightCharacter_Incap3()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = wraith.CharacterCard;
            UseIncapacitatedAbility(HeroController, 2);

            string messageText = $"Increase damage dealt by {wraith.Name} by 1.";

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the increasing effect works as expected
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have been increased by 1
            QuickHPCheck(-4);

            PrintSeparator("Change turns");
            GoToEndOfTurn(wraith);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the increasing effect works as expected
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have been increased by 1
            QuickHPCheck(-4);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the increasing effect has expired
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have not been increased
            QuickHPCheck(-3);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - No Cards to Select")]
        public void ArmYourself_NoCards()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
        [Description("TheKnight - Arm Yourself - Choose 1 Play")]
        public void ArmYourself_Choose1Play()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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

            //get 1 equipment from the trash, then append null at the end to force only 1 equipment taken
            List<Card> selectedCards = (HeroController.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1)).ToList();
            selectedCards.Add(null);
            DecisionSelectCards = selectedCards;

            DecisionMoveCardDestination = new MoveCardDestination(HeroController.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(HeroController, "ArmYourself");
            //no cards should have returned to hand for net -1
            //1 card should have been played, so 2 cards now in play
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(HeroController, 2);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Choose 1 Play")]
        public void ArmYourself_Choose1Hand()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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

            //get 1 equipment from the trash, then append null at the end to force only 1 equipment taken
            List<Card> selectedCards = (HeroController.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1)).ToList();
            selectedCards.Add(null);
            DecisionSelectCards = selectedCards;

            DecisionMoveCardDestination = new MoveCardDestination(HeroController.HeroTurnTaker.Hand); //Select the non-default decision just because
            PlayCardFromHand(HeroController, "ArmYourself");
            //1 card should have returned to hand for net 0
            //no card should have been played, so only the character card is still in play
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Choose 2")]
        public void ArmYourself_Choose2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            DecisionMoveCardDestination = new MoveCardDestination(HeroController.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(HeroController, "ArmYourself");
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(HeroController, 2);
        }

        [Test]
        [Description("TheKnight - Battlefield Scavenger - Draw Cards")]
        public void BattlefieldScavenger_Draw()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            PutInTrash(wraith.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController, ra, wraith);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 0, 0, 0 };
            PlayCardFromHand(HeroController, "BattlefieldScavenger");
            QuickHandCheck(0, 1, 1);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Battlefield Scavenger - Top Deck Cards")]
        public void BattlefieldScavenger_TopDeck()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(HeroController, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            PutInTrash(wraith.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController, ra, wraith);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 1, 0, 1 };
            var cards = new[] {
                HeroController.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c) || c.IsOngoing).First(),
                wraith.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c) || c.IsOngoing).First(),
            };
            DecisionSelectCards = cards;
            PlayCardFromHand(HeroController, "BattlefieldScavenger");
            AssertOnTopOfDeck(HeroController, cards[0]);
            AssertOnTopOfDeck(wraith, cards[1]);
            QuickHandCheck(-1, 1, 0);
            AssertNumberOfCardsInPlay(HeroController, 1);
        }

        [Test]
        [Description("TheKnight - Catch Your Breath")]
        public void CatchYourBreath()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            //set hitpoints to give room for gaining hp
            SetHitPoints(HeroController.CharacterCard, 20);

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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            var realm = PutInHand(HeroController, "ChampionOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "ShortSword");
            string messageText = $"Increase damage dealt by {wraith.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(wraith.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(HeroController);
            QuickShuffleStorage(HeroController.TurnTaker.Deck);
            PlayCard(HeroController, realm);
            AssertInTrash(HeroController, realm);
            
            QuickShuffleCheck(1);
            AssertInPlayArea(HeroController, card);

            PrintSeparator("Assert Applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the increasing effect works as expected
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have been increased by 1
            QuickHPCheck(-4);

            PrintSeparator("Change turns");
            GoToEndOfTurn(wraith);

            PrintSeparator("Effect still applied");
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, wraith.TurnTaker);
            AssertStatusEffectsContains(messageText);
            //Test that the increasing effect works as expected
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have been increased by 1
            QuickHPCheck(-4);

            PrintSeparator("Effect expires");
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the increasing effect expired
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 3, DamageType.Melee);
            //should have not been increased
            QuickHPCheck(-3);
        }

        [Test]
        [Description("TheKnight - DefenderOfTheRealm")]
        public void DefenderOfTheRealm()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Unity", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            var realm = PutInHand(HeroController, "DefenderOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "PlateMail");
            string messageText = $"Reduce damage dealt to {wraith.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(wraith.CharacterCard.ToEnumerable());
            
            GoToPlayCardPhase(HeroController);
            QuickShuffleStorage(HeroController.TurnTaker.Deck);
            PlayCard(HeroController, realm);
            AssertInTrash(realm);

            QuickShuffleCheck(1);
            AssertInPlayArea(HeroController, card);

            PrintSeparator("Assert Applied");
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
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
            //Test that the reducing effect has expired
            QuickHPStorage(wraith);
            DealDamage(baron, wraith, 3, DamageType.Melee);
            //should have not been reduced
            QuickHPCheck(-3);
        }

        [Test]
        [Description("TheKnight - HeavySwing")]
        public void HeavySwing()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            Card heavy = PutInHand(HeroController, "HeavySwing");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(HeroController, heavy);
            QuickHPCheck(-3);
            AssertInTrash(heavy);
        }

        [Test]
        [Description("TheKnight - KnightsHonor")]
        public void KnightsHonor_TestRedirect()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            Card honor = PutInHand(HeroController, "KnightsHonor");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            DecisionNextSelectionType = SelectionType.MoveCardNextToCard;
            DecisionNextToCard = wraith.CharacterCard;
            PlayCard(HeroController, honor);

            PrintSeparator("check redirect from wraith to knight");
            QuickHPStorage(HeroController, wraith, ra);
            DealDamage(baron, wraith, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("check no redirect from ra to knight");
            QuickHPStorage(HeroController, wraith, ra);
            DealDamage(baron, ra, 4, DamageType.Psychic);
            QuickHPCheck(0, 0, -4);

            PrintSeparator("check redirect damage is irreducible");
            PlayCard(HeroController, "StalwartShield");
            QuickHPStorage(HeroController, wraith, ra);
            DealDamage(baron, wraith, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("Test Power");
            UsePower("KnightsHonor");
            AssertInTrash("KnightsHonor");

            PrintSeparator("no redirect redirect after card leaves play");
            QuickHPStorage(HeroController, wraith, ra);
            DealDamage(baron, wraith, 4, DamageType.Psychic);
            QuickHPCheck(0, -4, 0);
        }

        [Test]
        [Description("TheKnight - KnightsHonor")]
        public void KnightsHonor_TestTargetLeavesPlay()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);
            var target = PutIntoPlay("BladeBattalion");

            var card = PutInHand(HeroController, "KnightsHonor");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            DecisionNextSelectionType = SelectionType.MoveCardNextToCard;
            DecisionNextToCard = target;
            PlayCardFromHand(HeroController, "KnightsHonor");

            PrintSeparator("Destroy Target");
            DestroyCard(target, baron.CharacterCard);
            
            PrintSeparator("Assert");
            AssertInPlayArea(baron, card);
            AssertNotNextToCard(card, target);
        }

        [Test]
        [Description("TheKnight - MaidensBlessing")]
        public void MaidensBlessing()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            PlayCard(HeroController, testCard);
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
        public void Armor_ImbuedVitality([Values("PlateHelm", "PlateMail")] string armor)
        {
            SetupGameController("GrandWarlordVoss", HeroNamespace, "RealmOfDiscord");
            StartGame();
                        
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            var target = PutInHand(HeroController, armor);
            int targetHP = armor == "PlateHelm" ? 3 : 5;
            var equip = PutInHand("Whetstone");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");

            PlayCard(target);
            AssertIsTarget(target, targetHP);
            AssertInPlayArea(HeroController, target);
            PlayCard(equip);
            AssertInPlayArea(HeroController, equip);

            var imbue = PlayCard("ImbuedVitality");
            AssertIsTarget(target, 6);
            AssertIsTarget(equip, 6);

            DestroyCard(imbue);
            AssertIsTarget(target, targetHP);
            AssertNotTarget(equip);
        }


        [Test]
        [Description("TheKnight - PlateHelm")]
        public void PlateHelm()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from wraith");
            DecisionYesNo = true;
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, wraith.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -1);

            PrintSeparator("check no redirect from helm");
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after helm leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(HeroController, testCard);
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);
        }

        [Test]
        [Description("TheKnight - PlateMail")]
        public void PlateMail()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            var testCard = PutInHand(HeroController, "PlateMail");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");

            QuickHandStorage(HeroController);
            PlayCard(HeroController, testCard);
            QuickHandCheck(-1);
            AssertInPlayArea(HeroController, testCard);
            AssertIsTarget(testCard, 5);

            GoToStartOfTurn(baron);

            PrintSeparator("check redirect from knight to mail");
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from wraith");
            DecisionYesNo = true;
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, wraith.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -1);

            PrintSeparator("check no redirect from mail");
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after mail leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(HeroController, testCard);
            QuickHPStorage(HeroController.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, HeroController.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);
        }

        [Test]
        [Description("TheKnight - Short Sword - Increase Damage Triggers")]
        public void ShortSword_Trigger()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            Card sword1 = PutInHand(HeroController, "ShortSword");
            Card sword2 = PutInHand(HeroController, "ShortSword");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, sword1);
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 1");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-2); //increased by 1

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, sword2);
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 2");
            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-3); //increased by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(sword1);
            DestroyCard(sword2);

            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight - Short Sword - Power")]
        public void ShortSword_Power()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            PlayCard(HeroController, testCard);
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandCheck(-1);

            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(testCard);
            //power deals 2 damage, +1 buff from itself = 3 total
            QuickHPCheck(-3); 
        }

        [Test]
        [Description("TheKnight - StalwartShield - Character Targeted")]
        public void StalwartShield_CharacterTargeted()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            Card shield1 = PutInHand(HeroController, "StalwartShield");
            Card shield2 = PutInHand(HeroController, "StalwartShield");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, shield1);
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
            DealDamage(wraith, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, shield2);
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 2");
            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 3, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(shield1);
            DestroyCard(shield2);

            QuickHPStorage(HeroController);
            DealDamage(baron, HeroController, 1, DamageType.Energy);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight - StalwartShield - Equipment Targeted")]
        public void StalwartShield_EquipmentTargeted()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            Card shield1 = PutInHand(HeroController, "StalwartShield");
            Card shield2 = PutInHand(HeroController, "StalwartShield");
            
            //put an equipment in play to test equipment reduction works
            //put a wraith equipment in play to make sure only applies to the knight's equipment
            Card equipment = PutInHand(HeroController, "PlateMail");
            Card wraithEquipment = PutInHand(wraith, "InfraredEyepiece");
            PlayCard(equipment);
            PlayCard(wraithEquipment);

            //really janky code to make infrared eyepiece have hp
            RunCoroutine(base.GameController.MakeTargettable(wraithEquipment, 6, 6, base.GetCardController(wraith.CharacterCard).GetCardSource()));

            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 2);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, shield1);
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 1");
            QuickHPStorage(equipment);
            DealDamage(baron.CharacterCard, equipment, 2, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 1

            PrintSeparator("Irreducible not effected");
            QuickHPStorage(equipment);
            DealDamage(baron.CharacterCard, equipment, 2, DamageType.Energy, true);
            QuickHPCheck(-2);

            //reset to max health
            SetHitPoints(equipment, 5);

            PrintSeparator("Other Damage not reduced");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Other Equipment not reduced");
            QuickHPStorage(wraithEquipment);
            DealDamage(baron.CharacterCard, wraithEquipment, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(HeroController, 3);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, shield2);
            AssertNumberOfCardsInPlay(HeroController, 4);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 2");
            QuickHPStorage(equipment);
            DealDamage(baron.CharacterCard, equipment, 3, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(shield1);
            DestroyCard(shield2);

            QuickHPStorage(equipment);
            DealDamage(baron.CharacterCard, equipment, 1, DamageType.Energy);
            QuickHPCheck(-1);
        }


        [Test]
        [Description("TheKnight - SureFooting")]
        public void SureFooting()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
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
            PlayCard(HeroController, testCard);
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
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            Card strikes = PutInHand("SwiftStrikes");

            PrintSeparator("Test");
            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron, wraith);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, wraith.CharacterCard };
            PlayCard(HeroController, strikes);
            QuickHPCheck(-1, -1);
        }

        [Test]
        [Description("TheKnight - Whetstone")]
        public void Whetstone()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(HeroController);

            Card whetstone = PutInHand(HeroController, "Whetstone");
            GoToPlayCardPhase(HeroController);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(HeroController, 1);
            QuickHandStorage(HeroController);
            PlayCard(HeroController, whetstone);
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

            PrintSeparator("Other's Melee Damage not increased");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Melee);
            QuickHPCheck(-1);

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(whetstone);

            QuickHPStorage(baron);
            DealDamage(HeroController, baron, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}

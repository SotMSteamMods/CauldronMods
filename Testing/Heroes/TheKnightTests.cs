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
    public class TheKnightTests : CauldronBaseTest
    {
        #region HelperFunctions
        protected string HeroNamespace = "Cauldron.TheKnight";

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(knight, 1);
            DealDamage(villain, knight, 2, DamageType.Melee);
        }
        
        #endregion

        [Test]
        [Order(1)]
        public void TheKnightCharacter_Load()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(knight);
            Assert.IsInstanceOf(typeof(TheKnightCharacterCardController), knight.CharacterCardController);

            Assert.AreEqual(32, knight.CharacterCard.HitPoints);
        }

        [Test]
        [Order(1)]
        public void TheKnightCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            DiscardAllCards(knight);
            ShuffleTrashIntoDeck(knight);

            PrintSeparator("Test");
            Assert.IsTrue(knight.GetCardControllersAtLocation(knight.TurnTaker.Deck).All(c => c is TheKnightCardController), "Not all cards are derived from " + nameof(Cauldron.TheKnight.TheKnightCardController));

            var cc = (TheKnightCardController)knight.GetCardControllersAtLocation(knight.TurnTaker.Deck).First();

            List<SelectCardDecision> results = new List<SelectCardDecision>();

            RunCoroutine(cc.SelectOwnCharacterCard(results, SelectionType.None));

            Assert.IsTrue(results.Count == 1 && results[0].SelectedCard == knight.CharacterCard, "Own CharacterCard was not selected");
            AssertNumberOfCardsInPlay(knight, 1);
        }

        [Test]
        [Order(1)]
        public void SingleHandEquipmentCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            DiscardAllCards(knight);
            ShuffleTrashIntoDeck(knight);

            PrintSeparator("Test");
            var ccs = knight.GetCardControllersAtLocation(knight.TurnTaker.Deck).Cast<TheKnightCardController>().ToList();

            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => cc.IsSingleHandCard(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the single hand keyword");
            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => base.IsEquipment(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the equipment keyword");
            Assert.IsTrue(ccs.Where(cc => !(cc is SingleHandEquipmentCardController)).All(cc => !cc.IsSingleHandCard(cc.Card)), $"A card has the single hand keyword but does not derive from {nameof(SingleHandEquipmentCardController)}");

            PrintSeparator("put all Single Hand cards into hand");
            foreach(var card in ccs.OfType<SingleHandEquipmentCardController>().Select(cc => cc.Card))
            {
                PutInHand(card);
            }

            PrintSeparator("play them all, first two normal");
            PlayCardFromHand(knight, knight.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(knight, 2);
            AssertNumberOfCardsInTrash(knight, 0);

            PlayCardFromHand(knight, knight.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(knight, 3);
            AssertNumberOfCardsInTrash(knight, 0);

            PrintSeparator("subsequent cards should destroy a previous card");
            int cardsInTrash = 0;
            while (knight.HeroTurnTaker.Hand.HasCards)
            {
                DecisionSelectCard = knight.TurnTaker.PlayArea.Cards.First(c => !c.IsHeroCharacterCard);
                PlayCardFromHand(knight, knight.HeroTurnTaker.Hand.Cards.First().Identifier);
                AssertNumberOfCardsInPlay(knight, 3);
                AssertNumberOfCardsInTrash(knight, ++cardsInTrash);
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
            GoToUsePowerPhase(knight);
            DecisionSelectFunction = 0;
            DecisionSelectTarget = base.FindCardInPlay("BaronBladeCharacter");
            QuickHPStorage(DecisionSelectTarget);
            UsePower(knight.CharacterCard);
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
            AssertIncapacitated(knight);

            //set up wraith
            var card = wraith.GetCardsAtLocation(wraith.HeroTurnTaker.Deck).First(c => c.IsOngoing || IsEquipment(c));
            PutInHand(wraith, card);
            QuickHandStorage(wraith);

            PrintSeparator("Test");
            GoToUseIncapacitatedAbilityPhase(knight);

            PrintSeparator("use incap");
            DecisionSelectTurnTaker = wraith.TurnTaker;
            DecisionSelectCard = card;
            UseIncapacitatedAbility(knight, 0);

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
            AssertIncapacitated(knight);

            GoToUseIncapacitatedAbilityPhase(knight);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = wraith.CharacterCard;
            UseIncapacitatedAbility(knight, 1);

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
            GoToStartOfTurn(knight);
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
            AssertIncapacitated(knight);

            GoToUseIncapacitatedAbilityPhase(knight);

            AssertNumberOfStatusEffectsInPlay(0);

            PrintSeparator("Test");
            DecisionSelectCard = wraith.CharacterCard;
            UseIncapacitatedAbility(knight, 2);

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
            GoToStartOfTurn(knight);
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
            
            PutInHand(knight, "ArmYourself");

            //Put some random cards non-Equipment in the trash
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(10));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            AssertNoDecision();
            PlayCardFromHand(knight, "ArmYourself");
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(knight, 1);
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

            PutInHand(knight, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            DecisionSelectCards = new Card[] { null, null };
            PlayCardFromHand(knight, "ArmYourself");
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(knight, 1);
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

            PutInHand(knight, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            //exclude PlateHelm since it draws a card
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) && c.Identifier != "PlateHelm").Take(5));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);

            //get 1 equipment from the trash, then append null at the end to force only 1 equipment taken
            List<Card> selectedCards = (knight.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1)).ToList();
            selectedCards.Add(null);
            DecisionSelectCards = selectedCards;

            DecisionMoveCardDestination = new MoveCardDestination(knight.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(knight, "ArmYourself");
            //no cards should have returned to hand for net -1
            //1 card should have been played, so 2 cards now in play
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(knight, 2);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Single Card")]
        public void ArmYourself_SingleCard()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(knight, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            //exclude PlateHelm since it draws a card
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) && c.Identifier != "PlateHelm").Take(1));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);

            //get 1 equipment from the trash, then append null at the end to force only 1 equipment taken
            List<Card> selectedCards = (knight.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1)).ToList();
            selectedCards.Add(null);
            DecisionSelectCards = selectedCards;

            DecisionMoveCardDestination = new MoveCardDestination(knight.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(knight, "ArmYourself");
            //no cards should have returned to hand for net -1
            //1 card should have been played, so 2 cards now in play
            QuickHandCheck(-1);
            AssertNumberOfCardsInPlay(knight, 2);
        }

        [Test]
        [Description("TheKnight - Arm Yourself - Choose 1 Hand")]
        public void ArmYourself_Choose1Hand()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Ra", "TheWraith", "Megalopolis");
            StartGame();

            PrintSeparator("Setup");
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand(knight, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            //exclude PlateHelm since it draws a card
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) && c.Identifier != "PlateHelm").Take(5));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);

            //get 1 equipment from the trash, then append null at the end to force only 1 equipment taken
            List<Card> selectedCards = (knight.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(1)).ToList();
            selectedCards.Add(null);
            DecisionSelectCards = selectedCards;

            DecisionMoveCardDestination = new MoveCardDestination(knight.HeroTurnTaker.Hand); //Select the non-default decision just because
            PlayCardFromHand(knight, "ArmYourself");
            //1 card should have returned to hand for net 0
            //no card should have been played, so only the character card is still in play
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(knight, 1);
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

            PutInHand(knight, "ArmYourself");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            //exclude PlateHelm since it draws a card
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) && c.Identifier != "PlateHelm").Take(5));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            DecisionSelectCards = knight.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c)).Take(2);
            DecisionMoveCardDestination = new MoveCardDestination(knight.HeroTurnTaker.PlayArea); //Select the non-default decision just because
            PlayCardFromHand(knight, "ArmYourself");
            QuickHandCheck(0);
            AssertNumberOfCardsInPlay(knight, 2);
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

            PutInHand(knight, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            PutInTrash(wraith.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight, ra, wraith);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 0, 0, 0 };
            PlayCardFromHand(knight, "BattlefieldScavenger");
            QuickHandCheck(0, 1, 1);
            AssertNumberOfCardsInPlay(knight, 1);
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

            PutInHand(knight, "BattlefieldScavenger");

            PrintSeparator("Put some random cards in the trash");
            PutInTrash(knight.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            PutInTrash(wraith.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c) || c.IsOngoing).Take(2));
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight, ra, wraith);
            DecisionAutoDecideIfAble = true;
            DecisionSelectFunctions = new int?[] { 1, 0, 1 };
            var cards = new[] {
                knight.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c) || c.IsOngoing).First(),
                wraith.HeroTurnTaker.Trash.Cards.Where(c => IsEquipment(c) || c.IsOngoing).First(),
            };
            DecisionSelectCards = cards;
            PlayCardFromHand(knight, "BattlefieldScavenger");
            AssertOnTopOfDeck(knight, cards[0]);
            AssertOnTopOfDeck(wraith, cards[1]);
            QuickHandCheck(-1, 1, 0);
            AssertNumberOfCardsInPlay(knight, 1);
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
            SetHitPoints(knight.CharacterCard, 20);

            PutInHand(knight, "CatchYourBreath");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            QuickHPStorage(knight);

            PlayCardFromHand(knight, "CatchYourBreath");

            AssertNumberOfCardsInPlay(knight, 1);
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

            var realm = PutInHand(knight, "ChampionOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = knight.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "ShortSword");
            string messageText = $"Increase damage dealt by {wraith.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(wraith.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(knight);
            QuickShuffleStorage(knight.TurnTaker.Deck);
            PlayCard(knight, realm);
            AssertInTrash(knight, realm);
            
            QuickShuffleCheck(1);
            AssertInPlayArea(knight, card);

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
            GoToStartOfTurn(knight);
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

            var realm = PutInHand(knight, "DefenderOfTheRealm");

            PrintSeparator("Test");
            AssertNumberOfStatusEffectsInPlay(0);

            var card = knight.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "PlateMail");
            string messageText = $"Reduce damage dealt to {wraith.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(wraith.CharacterCard.ToEnumerable());
            
            GoToPlayCardPhase(knight);
            QuickShuffleStorage(knight.TurnTaker.Deck);
            PlayCard(knight, realm);
            AssertInTrash(realm);

            QuickShuffleCheck(1);
            AssertInPlayArea(knight, card);

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
            GoToStartOfTurn(knight);
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

            Card heavy = PutInHand(knight, "HeavySwing");

            PrintSeparator("Test");
            GoToPlayCardPhase(knight);
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCard(knight, heavy);
            QuickHPCheck(-3);
            AssertInTrash(heavy);
        }

        [Test]
        [Description("TheKnight - HeavySwing in Oblivaeon")]
        public void HeavySwing_OblivaeonSelectingWhoDealsDamage()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon, ra, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TheKnight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.TheKnight";
            RunActiveTurnPhase();
           

            Card heavy = PutInHand(knight, "HeavySwing");

            PrintSeparator("Test");
            GoToPlayCardPhase(knight);
            QuickHPStorage(oblivaeon);
            DecisionSelectTarget = oblivaeon.CharacterCard;
            AssertNoDecision(SelectionType.HeroToDealDamage);
            PlayCard(knight, heavy);
            QuickHPCheck(-3);
            AssertInTrash(heavy);
        }

        [Test]
        [Description("TheKnight - HeavySwing in Oblivaeon")]
        public void HeavySwing_OblivaeonSelectingWhoDealsDamage_IncappedRonins()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon.CharacterCard, youngKnight, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            DealDamage(oblivaeon.CharacterCard, oldKnight, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TheKnight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.TheKnight";
            RunActiveTurnPhase();


            Card heavy = PutInHand(knight, "HeavySwing");

            PrintSeparator("Test");
            GoToPlayCardPhase(knight);
            QuickHPStorage(oblivaeon);
            DecisionSelectTarget = oblivaeon.CharacterCard;
            AssertNoDecision(SelectionType.HeroToDealDamage);
            PlayCard(knight, heavy);
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
            DiscardAllCards(knight);
            ShuffleTrashIntoDeck(knight);

            Card honor = PutInHand(knight, "KnightsHonor");

            PrintSeparator("Test");
            GoToPlayCardPhase(knight);
            DecisionNextSelectionType = SelectionType.MoveCardNextToCard;
            DecisionNextToCard = wraith.CharacterCard;
            PlayCard(knight, honor);

            PrintSeparator("check redirect from wraith to knight");
            QuickHPStorage(knight, wraith, ra);
            DealDamage(baron, wraith, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("check no redirect from ra to knight");
            QuickHPStorage(knight, wraith, ra);
            DealDamage(baron, ra, 4, DamageType.Psychic);
            QuickHPCheck(0, 0, -4);

            PrintSeparator("check redirect damage is irreducible");
            PlayCard(knight, "StalwartShield");
            QuickHPStorage(knight, wraith, ra);
            DealDamage(baron, wraith, 4, DamageType.Psychic);
            QuickHPCheck(-4, 0, 0);

            PrintSeparator("Test Power");
            UsePower(knight.CharacterCard, 1);
            AssertInTrash("KnightsHonor");

            PrintSeparator("no redirect redirect after card leaves play");
            QuickHPStorage(knight, wraith, ra);
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
            DiscardAllCards(knight);
            ShuffleTrashIntoDeck(knight);
            var target = PutIntoPlay("BladeBattalion");

            var card = PutInHand(knight, "KnightsHonor");

            PrintSeparator("Test");
            GoToPlayCardPhase(knight);
            DecisionNextSelectionType = SelectionType.MoveCardNextToCard;
            DecisionNextToCard = target;
            PlayCardFromHand(knight, "KnightsHonor");

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
            DiscardAllCards(knight);

            //prime a hand and top of deck
            var testCard = PutInHand(knight, "MaidensBlessing");
            var equipCard = knight.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "Whetstone");
            PutInHand(equipCard);
            PutInHand(knight, knight.HeroTurnTaker.Deck.Cards.First(c => !IsEquipment(c)));
            PutOnDeck(knight, knight.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(3));

            PrintSeparator("Play the Card");           
            GoToPlayCardPhase(knight);
            QuickHandStorage(knight);
            AssertNumberOfCardsInPlay(knight, 1);
            PlayCard(knight, testCard);
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandCheck(-1);
            GoToUsePowerPhase(knight);

            PrintSeparator("Use power expect equipCard to be played, and a card draw");
            DecisionSelectCard = equipCard;
            QuickHandStorage(knight);
            UsePower(testCard);
            QuickHandCheck(0); //plays a card, draws a card
            AssertNumberOfCardsInPlay(knight, 3);
            AssertInPlayArea(knight, equipCard);

            PrintSeparator("Use power, no card to play, expect card draw");
            DecisionSelectCard = null;
            QuickHandStorage(knight);
            UsePower(testCard);
            AssertNoDecision(SelectionType.PlayCard);
            QuickHandCheck(1);
            AssertNumberOfCardsInPlay(knight, 3);
        }


        [Test]
        public void Armor_ImbuedVitality([Values("PlateHelm", "PlateMail")] string armor)
        {
            SetupGameController("GrandWarlordVoss", HeroNamespace, "RealmOfDiscord");
            StartGame();
                        
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(knight);

            var target = PutInHand(knight, armor);
            int targetHP = armor == "PlateHelm" ? 3 : 5;
            var equip = PutInHand("Whetstone");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            PlayCard(target);
            AssertIsTarget(target, targetHP);
            AssertInPlayArea(knight, target);
            PlayCard(equip);
            AssertInPlayArea(knight, equip);

            var imbue = PlayCard("ImbuedVitality");
            AssertIsTarget(target, 6);
            AssertIsTarget(equip, 6);

            DestroyCard(imbue);
            AssertIsTarget(target, targetHP);
            AssertNotTarget(equip);
        }

        [Test]
        public void Armor_ImbuedVitalityOutOfPlay([Values("PlateHelm", "PlateMail")] string armor)
        {
            SetupGameController("GrandWarlordVoss", HeroNamespace, "RealmOfDiscord");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(knight);

            var target = PutInHand(knight, armor);
            int targetHP = armor == "PlateHelm" ? 3 : 5;
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            var imbue = PlayCard("ImbuedVitality");
            AssertIsTarget(target, 6);

            DestroyCard(imbue);
            AssertIsTarget(target, targetHP);
        }

        [Test]
        public void Armor_ImbuedVitalityOutOfPlay_ThenEntersPlay([Values("PlateHelm", "PlateMail")] string armor)
        {
            SetupGameController("GrandWarlordVoss", HeroNamespace, "RealmOfDiscord");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(knight);

            var target = PutInHand(knight, armor);
            int targetHP = armor == "PlateHelm" ? 3 : 5;
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            var imbue = PlayCard("ImbuedVitality");
            AssertIsTarget(target, 6);

            PlayCard(target);
            AssertIsTarget(target, 6);

            DestroyCard(imbue);
            AssertIsTarget(target, targetHP);
        }

        [Test]
        public void Armor_BounceArmourThenImbuedVitality([Values("PlateHelm", "PlateMail")] string armor)
        {
            SetupGameController("GrandWarlordVoss", HeroNamespace, "RealmOfDiscord");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            DiscardAllCards(knight);

            var target = PutInHand(knight, armor);
            int targetHP = armor == "PlateHelm" ? 3 : 5;
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            PlayCard(target);
            AssertIsTarget(target, targetHP);
            DestroyCard(target);

            var imbue = PlayCard("ImbuedVitality");
            AssertIsTarget(target, 6);

            DestroyCard(imbue);
            AssertIsTarget(target, targetHP);
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
            DiscardAllCards(knight);

            var testCard = PutInHand(knight, "PlateHelm");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            QuickHandStorage(knight);
            PlayCardFromHand(knight, testCard.Identifier);
            QuickHandCheck(0);
            AssertInPlayArea(knight, testCard);
            AssertIsTarget(testCard, 3);

            GoToStartOfTurn(baron);

            PrintSeparator("check redirect from knight to helm");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from wraith");
            DecisionYesNo = true;
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, wraith.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -1);

            PrintSeparator("check no redirect from helm");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after helm leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(knight, testCard);
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
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
            DiscardAllCards(knight);

            var testCard = PutInHand(knight, "PlateMail");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");

            QuickHandStorage(knight);
            PlayCard(knight, testCard);
            QuickHandCheck(-1);
            AssertInPlayArea(knight, testCard);
            AssertIsTarget(testCard, 5);

            GoToStartOfTurn(baron);

            PrintSeparator("check redirect from knight to mail");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = true;
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check redirect is optional");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DecisionYesNo = false;
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(-1, 0, 0);

            PrintSeparator("check no redirect from wraith");
            DecisionYesNo = true;
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, wraith.CharacterCard, 1, DamageType.Psychic);
            QuickHPCheck(0, 0, -1);

            PrintSeparator("check no redirect from mail");
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, testCard, 1, DamageType.Psychic);
            QuickHPCheck(0, -1, 0);

            PrintSeparator("check no redirect after mail leaves play");
            DealDamage(baron, testCard, 3, DamageType.Psychic);
            AssertInTrash(knight, testCard);
            QuickHPStorage(knight.CharacterCard, testCard, wraith.CharacterCard);
            DealDamage(baron, knight.CharacterCard, 1, DamageType.Psychic);
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
            DiscardAllCards(knight);

            Card sword1 = PutInHand(knight, "ShortSword");
            Card sword2 = PutInHand(knight, "ShortSword");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            PlayCard(knight, sword1);
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 1");
            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Radiant);
            QuickHPCheck(-2); //increased by 1

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Other Damage not increased");
            QuickHPStorage(knight);
            DealDamage(baron, knight, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandStorage(knight);
            PlayCard(knight, sword2);
            AssertNumberOfCardsInPlay(knight, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage increased by 2");
            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Radiant);
            QuickHPCheck(-3); //increased by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(sword1);
            DestroyCard(sword2);

            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Radiant);
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
            DiscardAllCards(knight);

            var testCard = PutInHand(knight, "ShortSword");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            PlayCard(knight, testCard);
            AssertNumberOfCardsInPlay(knight, 2);
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
            DiscardAllCards(knight);

            Card shield1 = PutInHand(knight, "StalwartShield");
            Card shield2 = PutInHand(knight, "StalwartShield");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            PlayCard(knight, shield1);
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 1");
            QuickHPStorage(knight);
            DealDamage(baron, knight, 2, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 1

            PrintSeparator("Irreducible not effected");
            QuickHPStorage(knight);
            DealDamage(baron, knight, 2, DamageType.Energy, true);
            QuickHPCheck(-2);

            PrintSeparator("Other Damage not reduced");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Radiant);
            QuickHPCheck(-1);

            PrintSeparator("Multiples stack correctly");
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandStorage(knight);
            PlayCard(knight, shield2);
            AssertNumberOfCardsInPlay(knight, 3);
            QuickHandCheck(-1);

            PrintSeparator("Damage reduced by 2");
            QuickHPStorage(knight);
            DealDamage(baron, knight, 3, DamageType.Energy);
            QuickHPCheck(-1); //reduced by 2

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(shield1);
            DestroyCard(shield2);

            QuickHPStorage(knight);
            DealDamage(baron, knight, 1, DamageType.Energy);
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
            DiscardAllCards(knight);

            Card shield1 = PutInHand(knight, "StalwartShield");
            Card shield2 = PutInHand(knight, "StalwartShield");
            
            //put an equipment in play to test equipment reduction works
            //put a wraith equipment in play to make sure only applies to the knight's equipment
            Card equipment = PutInHand(knight, "PlateMail");
            Card wraithEquipment = PutInHand(wraith, "InfraredEyepiece");
            PlayCard(equipment);
            PlayCard(wraithEquipment);

            //really janky code to make infrared eyepiece have hp
            RunCoroutine(base.GameController.MakeTargettable(wraithEquipment, 6, 6, base.GetCardController(wraith.CharacterCard).GetCardSource()));

            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandStorage(knight);
            PlayCard(knight, shield1);
            AssertNumberOfCardsInPlay(knight, 3);
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
            AssertNumberOfCardsInPlay(knight, 3);
            QuickHandStorage(knight);
            PlayCard(knight, shield2);
            AssertNumberOfCardsInPlay(knight, 4);
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
            DiscardAllCards(knight);

            //prime a hand and top of deck
            var testCard = PutInHand(knight, "SureFooting");
            var oneshotCard = knight.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "HeavySwing");
            PutInHand(oneshotCard);
            PutInHand(knight, knight.HeroTurnTaker.Deck.Cards.First(c => !c.IsOneShot));
            PutOnDeck(knight, knight.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOneShot).Take(3));

            PrintSeparator("Play the Card");
            GoToPlayCardPhase(knight);
            QuickHandStorage(knight);
            AssertNumberOfCardsInPlay(knight, 1);
            PlayCard(knight, testCard);
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandCheck(-1);
            GoToUsePowerPhase(knight);

            PrintSeparator("Use power expect oneshotCard to be played, and a card draw");
            DecisionSelectCard = oneshotCard;
            QuickHandStorage(knight);
            UsePower(testCard);
            QuickHandCheck(0); //plays a card, draws a card
            AssertNumberOfCardsInPlay(knight, 2);
            AssertInTrash(oneshotCard);

            PrintSeparator("Use power, no card to play, expect card draw");
            DecisionSelectCard = null;
            QuickHandStorage(knight);
            UsePower(testCard);
            AssertNoDecision(SelectionType.PlayCard);
            QuickHandCheck(1);
            AssertNumberOfCardsInPlay(knight, 2);
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
            GoToPlayCardPhase(knight);
            QuickHPStorage(baron, wraith);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, wraith.CharacterCard };
            PlayCard(knight, strikes);
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
            DiscardAllCards(knight);

            Card whetstone = PutInHand(knight, "Whetstone");
            GoToPlayCardPhase(knight);

            PrintSeparator("Test");
            AssertNumberOfCardsInPlay(knight, 1);
            QuickHandStorage(knight);
            PlayCard(knight, whetstone);
            AssertNumberOfCardsInPlay(knight, 2);
            QuickHandCheck(-1);

            PrintSeparator("Melee Damage increased by 1");
            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Melee);
            QuickHPCheck(-2); //increased by 1

            PrintSeparator("Non-Melee Damage not increased");
            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Fire);
            QuickHPCheck(-1);

            PrintSeparator("Other's Melee Damage not increased");
            QuickHPStorage(baron);
            DealDamage(wraith, baron, 1, DamageType.Melee);
            QuickHPCheck(-1);

            PrintSeparator("Effect removed when cards leave play");
            DestroyCard(whetstone);

            QuickHPStorage(baron);
            DealDamage(knight, baron, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight - Whetstone in Oblivaeon")]
        public void Whetstone_OblivaeonPlayingInPlayArea()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon, ra, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TheKnight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.TheKnight";
            RunActiveTurnPhase();

            GoToPlayCardPhase(knight);

            Card stone = PlayCard("Whetstone");
            AssertNotNextToCard(stone, knight.CharacterCard);
            AssertInPlayArea(knight, stone);
            
        }

        [Test]
        [Description("TheKnight - Whetstone in Oblivaeon")]
        public void Whetstone_OblivaeonPlayInPlayArea_IncappedRonins()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon.CharacterCard, youngKnight, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            DealDamage(oblivaeon.CharacterCard, oldKnight, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);

            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TheKnight" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.TheKnight";
            RunActiveTurnPhase();


            GoToPlayCardPhase(knight);

            Card stone = PlayCard("Whetstone");
            AssertNotNextToCard(stone, knight.CharacterCard);
            AssertInPlayArea(knight, stone);
        }
    }
}

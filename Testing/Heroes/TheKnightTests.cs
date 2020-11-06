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
        public void TestCharacterLoad()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(HeroController);
            Assert.IsInstanceOf(typeof(TheKnightCharacterCardController), HeroController.CharacterCardController);

            Assert.AreEqual(32, HeroController.CharacterCard.HitPoints);
        }

        [Test]
        [Order(1)]
        public void TestKnightCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            Assert.IsTrue(HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).All(c => c is TheKnightCardController), "Not all cards are derived from " + nameof(TheKnightCardController));

            var cc = (TheKnightCardController)HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).First();

            List<SelectCardDecision> results = new List<SelectCardDecision>();

            RunCoroutine(cc.SelectOwnCharacterCard(results, SelectionType.None));

            Assert.IsTrue(results.Count == 1 && results[0].SelectedCard == HeroController.CharacterCard, "Own CharacterCard was not selected");
        }

        [Test]
        [Order(1)]
        public void TestSingleHandEquipmentCardController()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            DiscardAllCards(HeroController);
            ShuffleTrashIntoDeck(HeroController);

            //Get all card controllers
            var ccs = HeroController.GetCardControllersAtLocation(HeroController.TurnTaker.Deck).Cast<TheKnightCardController>().ToList();

            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => cc.IsSingleHandCard(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the single hand keyword");
            Assert.IsTrue(ccs.OfType<SingleHandEquipmentCardController>().All(cc => base.IsEquipment(cc.Card)), $"not all {nameof(SingleHandEquipmentCardController)} have the equipment keyword");
            Assert.IsTrue(ccs.Where(cc => !(cc is SingleHandEquipmentCardController)).All(cc => !cc.IsSingleHandCard(cc.Card)), $"A card has the single hand keyword but does not derive from {nameof(SingleHandEquipmentCardController)}");

            //put all Single Hand cards into hand
            foreach(var card in ccs.OfType<SingleHandEquipmentCardController>().Select(cc => cc.Card))
            {
                PutInHand(card);
            }

            //play them all, first two normal
            PlayCardFromHand(HeroController, HeroController.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(HeroController, 2);
            AssertNumberOfCardsInTrash(HeroController, 0);

            PlayCardFromHand(HeroController, HeroController.HeroTurnTaker.Hand.Cards.First().Identifier);
            AssertNumberOfCardsInPlay(HeroController, 3);
            AssertNumberOfCardsInTrash(HeroController, 0);

            //subsequent cards should destroy a previous card
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
        public void InnatePower()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            GoToUsePowerPhase(HeroController);
            DecisionSelectFunction = 0;
            DecisionSelectTarget = base.FindCardInPlay("BaronBladeCharacter");
            QuickHPStorage(DecisionSelectTarget);
            UsePower(HeroController.CharacterCard);
            QuickHPCheck(-1);
        }

        [Test]
        [Description("TheKnight Incap 1 - One player may play a card now.")]
        public void TestIncap1()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            //set up legacy
            PutInHand(legacy, "NextEvolution");
            QuickHandStorage(legacy);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            //use incap
            DecisionSelectCard = legacy.GetCardsAtLocation(legacy.HeroTurnTaker.Hand).First(c => c.Identifier == "NextEvolution");
            UseIncapacitatedAbility(HeroController, 0);

            //assert card was played
            QuickHandCheck(-1);
            Assert.IsTrue(legacy.GetCardsAtLocation(legacy.HeroTurnTaker.PlayArea).Count(c => c.Identifier == "NextEvolution") == 1);
        }

        [Test]
        [Description("TheKnight Incap 2 - Reduce damage dealt to 1 target by 1 until the start of your next turn.")]
        public void TestIncap2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            //Apply Status effect
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(HeroController, 1);

            string messageText = $"Reduce damage dealt to {legacy.Name} by 1.";

            //Assert Applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Change turns
            GoToEndOfTurn(legacy);

            //Effect still applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Effect expires
            AssertNextMessageContains(messageText);
            GoToStartOfTurn(HeroController);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        [Description("TheKnight Incap 3 - Increase damage dealt by 1 target by 1 until the start of your next turn.")]
        public void TestIncap3()
        {
            //"Increase damage dealt by 1 target by 1 until the start of your next turn."
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            GoToUseIncapacitatedAbilityPhase(HeroController);

            AssertNumberOfStatusEffectsInPlay(0);

            //Apply Status effect
            DecisionSelectCard = legacy.CharacterCard;
            UseIncapacitatedAbility(HeroController, 2);

            string messageText = $"Increase damage dealt by {legacy.Name} by 1.";

            //Assert Applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Change turns
            GoToEndOfTurn(legacy);

            //Effect still applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Effect expires
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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);
            
            PutInHand("ArmYourself");

            //Put some random cards non-Equipment in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(10));
            GoToPlayCardPhase(HeroController);

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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("ArmYourself");

            //Put some random cards in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));
            GoToPlayCardPhase(HeroController);

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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("ArmYourself");

            //Put some random cards in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));
            GoToPlayCardPhase(HeroController);

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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("BattlefieldScavenger");

            //Put some random cards in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));

            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => c.IsOngoing).Take(5));
            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOngoing).Take(5));
            GoToPlayCardPhase(HeroController);

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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("BattlefieldScavenger");

            //Put some random cards in the trash
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => !IsEquipment(c)).Take(5));
            PutInTrash(HeroController.HeroTurnTaker.Deck.Cards.Where(c => IsEquipment(c)).Take(5));

            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => c.IsOngoing).Take(5));
            PutInTrash(legacy.HeroTurnTaker.Deck.Cards.Where(c => !c.IsOngoing).Take(5));
            GoToPlayCardPhase(HeroController);

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
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            //setup
            DealDamage(baron, HeroController.CharacterCard, 5, DamageType.Psychic);
            PutInHand("CatchYourBreath");
            GoToPlayCardPhase(HeroController);

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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("ChampionOfTheRealm");

            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "ShortSword");
            string messageText = $"Increase damage dealt by {legacy.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(legacy.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(HeroController);
            PlayCardFromHand(HeroController, "ChampionOfTheRealm");

            //Assert Played
            AssertInPlayArea(HeroController, card);

            //Assert Applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Change turns
            GoToEndOfTurn(legacy);

            //Effect still applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Effect expires
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

            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("DefenderOfTheRealm");

            AssertNumberOfStatusEffectsInPlay(0);

            var card = HeroController.HeroTurnTaker.Deck.Cards.First(c => c.Identifier == "PlateMail");
            string messageText = $"Reduce damage dealt to {legacy.Name} by 1.";

            DecisionSelectCards = card.ToEnumerable().Concat(legacy.CharacterCard.ToEnumerable());

            GoToPlayCardPhase(HeroController);
            PlayCardFromHand(HeroController, "DefenderOfTheRealm");

            //Assert Played
            AssertInPlayArea(HeroController, card);

            //Assert Applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Change turns
            GoToEndOfTurn(legacy);

            //Effect still applied
            AssertNumberOfStatusEffectsInPlay(1);
            AssertStatusEffectAssociatedTurnTaker(0, legacy.TurnTaker);
            AssertStatusEffectsContains(messageText);

            //Effect expires
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
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("HeavySwing");

            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron);
            DecisionSelectTarget = baron.CharacterCard;
            PlayCardFromHand(HeroController, "HeavySwing");
            QuickHPCheck(-3);
        }


        [Test]
        [Description("TheKnight - SwiftStrikes")]
        public void SwiftStrikes()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();
            //nuke all baron blades cards so his ongoings don't break tests
            DestroyCards((Card c) => c.IsVillain && c.IsInPlayAndHasGameText && !c.IsCharacter);

            PutInHand("SwiftStrikes");

            GoToPlayCardPhase(HeroController);
            QuickHPStorage(baron, legacy);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, legacy.CharacterCard };
            PlayCardFromHand(HeroController, "SwiftStrikes");
            QuickHPCheck(-1, -1);
        }
    }
}

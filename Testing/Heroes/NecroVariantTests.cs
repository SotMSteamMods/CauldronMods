using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Necro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NecroVariantTests : CauldronBaseTest
    {
        #region NecroHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(necro.CharacterCard, 1);
            DealDamage(villain, necro, 2, DamageType.Melee);
        }
        protected void AssertNumberOfUndeadInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsUndead(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfRitualInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsRitual(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }
        private bool IsUndead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
        }

        private bool IsRitual(Card card)
        {
            return card != null && this.GameController.DoesCardContainKeyword(card, "ritual", false, false);
        }

        #endregion

        [Test()]
        public void TestPastNecroLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(necro);
            Assert.IsInstanceOf(typeof(PastNecroCharacterCardController), necro.CharacterCardController);

            Assert.AreEqual(25, necro.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestPastNecroInnatePowerAbomination()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");
            Card abomination = PlayCard("Abomination");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, battalion, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2, -2, 0, 0, 0);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            QuickHPStorage(baron.CharacterCard, battalion, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(0, 0, -2, -2, -2);
        }

        [Test()]
        public void TestPastNecroInnatePowerOnReload()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");
            Card ghoul = PlayCard("Ghoul");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            SaveAndLoad();
            QuickHPStorage(baron.CharacterCard, battalion, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }


        [Test()]
        public void TestPastNecroInnatePowerBloodRite()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card bloodRite = PlayCard("BloodRite");
            Card zombie = PlayCard("NecroZombie");
            SetHitPoints(new Card[] { baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard }, 2);

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(2, 2, 0, 0, 0);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            PlayCard(zombie);
            QuickHPUpdate();
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(0, 0, 2, 2, 2);
        }

        [Test()]
        public void TestPastNecroInnatePowerBloodRite_RedMenace()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Card red = MoveCard(oblivaeon, "TheRedMenace", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            GoToBeforeStartOfTurn(necro);
            RunActiveTurnPhase();

            GoToPlayCardPhase(necro);
            SetHitPoints(red, 5);
            Card ghoul = PlayCard("Ghoul");
            Card blood = PlayCard("BloodRite");

            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(red);
            DestroyCard(ghoul, necro.CharacterCard);
            QuickHPCheck(2);

        }



        [Test()]
        public void TestPastNecroInnatePowerCorpseExplosion()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card explosion = PlayCard("CorpseExplosion");
            Card zombie = PlayCard("NecroZombie");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(0, 0, -2, -2, -2);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            PlayCard(zombie);
            QuickHPUpdate();
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(-2, -2, 0, 0, 0);
        }

        [Test()]
        public void TestPastNecroInnatePowerCorpseExplosion_RedMenaceFlipped()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Card red = MoveCard(oblivaeon, "TheRedMenace", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));
            GoToBeforeStartOfTurn(necro);
            RunActiveTurnPhase();

            GoToPlayCardPhase(necro);
            SetHitPoints(red, 5);
            Card ghoul = PlayCard("Ghoul");
            Card explosion = PlayCard("CorpseExplosion");

            DestroyCard(red);
            SetHitPoints(red, 5);

            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(red);
            DestroyCard(ghoul, necro.CharacterCard);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPastNecroInnatePowerDemonicImp()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card field = PlayCard("LivingForceField");
            Card mere = PlayCard("Mere");
            Card imp = PlayCard("DemonicImp");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            AssertInPlayArea(baron, field);
            AssertInPlayArea(haka, mere);
            GoToEndOfTurn(necro);
            AssertInTrash(field);
            AssertInPlayArea(haka, mere);

            PlayCard(field);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            AssertInPlayArea(baron, field);
            AssertInPlayArea(haka, mere);
            GoToEndOfTurn(necro);
            AssertInTrash(mere);
            AssertInPlayArea(baron, field);
        }

        [Test()]
        public void TestPastNecroInnatePowerGhoul()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card ghoul = PlayCard("Ghoul");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2, 0, 0, 0, 0);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(0, 0, 0, -2, 0);
        }

        [Test()]
        public void TestPastNecroInnatePowerHellfire()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card hellfire = PlayCard("Hellfire");
            Card zombie = PlayCard("NecroZombie");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            DecisionSelectTarget = haka.CharacterCard;
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(0, 0, 0, 0, -3);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            PlayCard(zombie);
            QuickHPUpdate();
            DecisionSelectTarget = baron.CharacterCard;
            DealDamage(necro, zombie, 50, DamageType.Infernal);
            QuickHPCheck(-3, 0, 0, 0, 0);
        }

        [Test()]
        public void TestPastNecroInnatePowerZombie()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card zombie = PlayCard("NecroZombie");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2, 0, 0, 0, 0);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(0, 0, 0, 0, -2);
        }

        [Test()]
        public void TestPastNecroInnatePowerPossessedCorpse()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Fanatic", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card elemental = PlayCard("ElementalRedistributor");
            Card corpse = PlayCard("PossessedCorpse");

            //Until the start of your next turn, replace the word “hero” on your cards with “villain”, and vice versa.
            GoToUsePowerPhase(necro);
            UsePower(necro.CharacterCard);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(0, -2, 0, 0, 0);

            //it should expire at the start of your next turn
            GoToUsePowerPhase(necro);
            QuickHPStorage(baron.CharacterCard, elemental, necro.CharacterCard, fanatic.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(necro);
            QuickHPCheck(0, 0, -2, 0, 0);
        }

        [Test()]
        public void TestPastNecroIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(necro);
            GoToUseIncapacitatedAbilityPhase(necro);

            //One hero target deals itself 3 toxic damage.
            DecisionSelectTarget = fanatic.CharacterCard;
            QuickHPStorage(fanatic);
            UseIncapacitatedAbility(necro, 0);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestPastNecroIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(necro);
            GoToUseIncapacitatedAbilityPhase(necro);
            PlayCard("BlazingTornado");

            //One hero may use their innate power, then draw a card.
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectTarget = legacy.CharacterCard;
            QuickHPStorage(legacy);
            QuickHandStorage(ra);
            UseIncapacitatedAbility(necro, 1);
            QuickHPCheck(-2);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestPastNecroIncap2_MultiplePowers()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Guise/SantaGuise", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(necro);
            GoToUseIncapacitatedAbilityPhase(necro);

            //One hero may use their innate power, then draw a card.
            DecisionSelectTurnTaker = guise.TurnTaker;
            QuickHandStorage(guise);
            UseIncapacitatedAbility(necro, 1);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestPastNecroIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/PastNecroCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(necro);

            IEnumerable<Card> cardsInHand = FindCardsWhere((Card c) => ra.HeroTurnTaker.Hand.HasCard(c));
            IEnumerable<Card> cardsOnDeck = GetTopCardsOfDeck(ra, cardsInHand.Count());
            //One player may discard their hand and draw that many cards.
            GoToUseIncapacitatedAbilityPhase(necro);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionYesNo = true;
            UseIncapacitatedAbility(necro, 2);
            AssertInTrash(cardsInHand);
            AssertInHand(cardsOnDeck);

        }

        [Test()]
        public void TestWardenOfChaosNecroLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(necro);
            Assert.IsInstanceOf(typeof(WardenOfChaosNecroCharacterCardController), necro.CharacterCardController);

            Assert.AreEqual(25, necro.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestWardenOfChaosNecroInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Megalopolis");
            StartGame();

            GoToStartOfTurn(necro);
            Card abomination = PutOnDeck("Abomination");
            AssertInDeck(abomination);
            //Put the top card of your deck into play.
            GoToUsePowerPhase(necro);
            UsePower(necro);
            AssertInPlayArea(necro, abomination);

        }

        [Test()]
        public void TestWardenOfChaosNecroInnatePower_NoDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Megalopolis");
            StartGame();

            GoToStartOfTurn(necro);
            DiscardTopCards(necro.TurnTaker.Deck, 36);
            //Put the top card of your deck into play.
            GoToUsePowerPhase(necro);
            QuickShuffleStorage(necro);
            UsePower(necro);
            QuickShuffleCheck(0);

        }

        [Test()]
        public void TestWardenOfChaosNecroIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);
            GoToUseIncapacitatedAbilityPhase(necro);

            IEnumerable<Card> toTrash = FindCardsWhere((Card c) => legacy.TurnTaker.Deck.HasCard(c) && (c.IsOngoing || IsEquipment(c))).Take(5);
            PutInTrash(toTrash);
            //One hero may put a random card from their trash into play.
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionYesNo = true;
            AssertNumberOfCardsInTrash(legacy, 5);
            AssertNumberOfCardsInPlay(legacy, 1);
            UseIncapacitatedAbility(necro, 0);
            AssertNumberOfCardsInTrash(legacy, 4);
            AssertNumberOfCardsInPlay(legacy, 2);
            AssertNumberOfCardsInRevealed(legacy, 0);
        }

        [Test()]
        public void TestWardenOfChaosNecroIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            Card field = PlayCard("LivingForceField");
            Card fire = PlayCard("ImbuedFire");

            //Destroy 1 ongoing card
            GoToUseIncapacitatedAbilityPhase(necro);
            DecisionSelectCard = field;
            AssertInPlayArea(baron, field);
            AssertInPlayArea(ra, fire);
            UseIncapacitatedAbility(necro, 1);
            AssertInTrash(field);
            AssertInPlayArea(ra, fire);
        }

        [Test()]
        public void TestWardenOfChaosNecroIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Legacy", "Unity", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            Card raptor = PlayCard("RaptorBot");
            Card mdp = GetCardInPlay("MobileDefensePlatform");


            //The next time a target is destroyed, 1 player may draw 2 cards.
            GoToUseIncapacitatedAbilityPhase(necro);
            UseIncapacitatedAbility(necro, 2);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);
            DealDamage(baron, raptor, 50, DamageType.Melee);
            QuickHandCheck(2);

            //only 1 use
            PlayCard(raptor);
            QuickHandStorage(legacy);
            DealDamage(baron, raptor, 50, DamageType.Melee);
            QuickHandCheck(0);

            //check villain targets
            UseIncapacitatedAbility(necro, 2);
            QuickHandStorage(legacy);
            DealDamage(baron, mdp, 50, DamageType.Melee);
            QuickHandCheck(2);

            //only 1 use
            PlayCard(mdp);
            QuickHandStorage(legacy);
            DealDamage(baron, mdp, 50, DamageType.Melee);
            QuickHandCheck(0);

        }


        [Test()]
        public void TestWardenOfChaosNecroIncap3Stacking()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/WardenOfChaosNecroCharacter", "Legacy", "Unity", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            Card raptor = PlayCard("RaptorBot");
            Card mdp = GetCardInPlay("MobileDefensePlatform");


            //The next time a target is destroyed, 1 player may draw 2 cards.
            GoToUseIncapacitatedAbilityPhase(necro);
            UseIncapacitatedAbility(necro, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            UseIncapacitatedAbility(necro, 2);
            AssertNumberOfStatusEffectsInPlay(2);

            DecisionSelectTurnTaker = legacy.TurnTaker;
            QuickHandStorage(legacy);
            DealDamage(baron, raptor, 50, DamageType.Melee);
            QuickHandCheck(4);

            //only 1 use
            PlayCard(raptor);
            QuickHandStorage(legacy);
            DealDamage(baron, raptor, 50, DamageType.Melee);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestLastOfTheForgottenOrderNecroLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(necro);
            Assert.IsInstanceOf(typeof(LastOfTheForgottenOrderNecroCharacterCardController), necro.CharacterCardController);

            Assert.AreEqual(26, necro.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestLastOfTheForgottenOrderNecroInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToStartOfTurn(necro);

            Card zombie = PlayCard("NecroZombie");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //The next time an undead target is destroyed, 1 hero deals a target 1 fire damage and draws a card.
            GoToUsePowerPhase(necro);
            UsePower(necro);

            DecisionSelectCard = ra.CharacterCard;
            DecisionSelectTarget = haka.CharacterCard;

            QuickHPStorage(ra, haka);
            QuickHandStorage(ra, haka);
            DealDamage(baron, zombie, 50, DamageType.Melee);
            QuickHPCheck(0, -1);
            QuickHandCheck(1, 0);

            //check only 1 time
            PlayCard(zombie);
            QuickHPUpdate();
            QuickHandUpdate();
            DealDamage(baron, zombie, 50, DamageType.Melee);
            QuickHPCheck(0, 0);
            QuickHandCheck(0, 0);

            UsePower(necro);

            //only undead targets
            QuickHPUpdate();
            QuickHandUpdate();
            DealDamage(baron, mdp, 50, DamageType.Melee);
            QuickHPCheck(0, 0);
            QuickHandCheck(0, 0);

        }


        [Test()]
        public void TestLastOfTheForgottenOrderNecroInnatePower_EffectStacks()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToStartOfTurn(necro);

            Card zombie = PlayCard("NecroZombie");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //use the power twice
            UsePower(necro);

            //The next time an undead target is destroyed, 1 hero deals a target 1 fire damage and draws a card.
            GoToUsePowerPhase(necro);
            UsePower(necro);

            DecisionSelectCard = ra.CharacterCard;
            DecisionSelectTarget = haka.CharacterCard;

            QuickHPStorage(ra, haka);
            DealDamage(baron, zombie, 50, DamageType.Melee);

            //should trigger twice
            QuickHPCheck(0, -2);
        }



        [Test()]
        public void TestLastOfTheForgottenOrderNecroInnatePower_NoChainKills()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            DiscardAllCards(necro);
            PutInHand("Ghoul");

            GoToStartOfTurn(necro);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card z1 = PlayCard("NecroZombie", 0);
            Card z2 = PlayCard("NecroZombie", 1);
            Card z3 = PlayCard("NecroZombie", 2);

            SetHitPoints(z1, 1);
            SetHitPoints(z2, 1);
            SetHitPoints(z3, 1);

            //The next time an undead target is destroyed, 1 hero deals a target 1 fire damage and draws a card.
            GoToUsePowerPhase(necro);
            UsePower(necro);

            DecisionSelectCards = new Card[] { ra.CharacterCard, z2, ra.CharacterCard, z3, ra.CharacterCard, mdp };

            DealDamage(baron, z1, 50, DamageType.Melee);

            //ra should kill z2, but not z3
            AssertInTrash(z2);
            AssertIsInPlay(z3);
        }
        [Test()]
        public void TestLastOfTheForgottenOrderNecroInnatePower_PowerModifiersTrack()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Unity", "Haka", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card z1 = PlayCard("NecroZombie");

            QuickHPStorage(baron);
            DecisionSelectCards = new Card[] { haka.CharacterCard, baron.CharacterCard };

            PlayCard("HastyAugmentation");

            DestroyCard(z1);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestLastOfTheForgottenOrderNecroIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            //One player may draw a card now.
            GoToUseIncapacitatedAbilityPhase(necro);
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHandStorage(haka);
            UseIncapacitatedAbility(necro, 0);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestLastOfTheForgottenOrderNecroIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            Card police = PlayCard("PoliceBackup");
            //Destroy an environment card.
            GoToUseIncapacitatedAbilityPhase(necro);
            DecisionSelectCard = police;
            AssertInPlayArea(env, police);
            UseIncapacitatedAbility(necro, 1);
            AssertInTrash(police);

        }

        [Test()]
        public void TestLastOfTheForgottenOrderNecroIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(necro);

            DiscardAllCards(haka);
            IEnumerable<Card> toHand = FindCardsWhere((Card c) => haka.TurnTaker.Deck.HasCard(c) && (c.IsOngoing || IsEquipment(c)) && !c.IsLimited).Take(4);
            PutInHand(toHand);

            //One player may play 2 random cards from their hand now.
            GoToUseIncapacitatedAbilityPhase(necro);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionYesNo = true;
            AssertNumberOfCardsInHand(haka, 4);
            AssertNumberOfCardsInPlay(haka, 1);
            UseIncapacitatedAbility(necro, 2);
            AssertNumberOfCardsInHand(haka, 2);
            AssertNumberOfCardsInPlay(haka, 3);
            AssertNumberOfCardsInRevealed(haka, 0);

        }
    }
}

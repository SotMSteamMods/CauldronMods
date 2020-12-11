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
    public class NecroVariantTests : BaseTest
    {
        #region NecroHelperFunctions
        protected HeroTurnTakerController necro { get { return FindHero("Necro"); } }
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

    }
}

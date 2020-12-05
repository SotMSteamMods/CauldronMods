using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.Celadroch;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class CeladrochTests : BaseTest
    {
        #region HelperFunctions

        protected TokenPool stormPool => celadroch.CharacterCard.FindTokenPool(CeladrochCharacterCardController.StormPoolIdentifier);
        protected TurnTakerController celadroch { get { return FindVillain("Celadroch"); } }

        private void AssertCard(string identifier, string[] keywords = null, int hitpoints = 0)
        {
            Card card = GetCard(identifier);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    AssertCardHasKeyword(card, keyword, false);
                }
            }
            if (hitpoints > 0)
            {
                AssertMaximumHitPoints(card, hitpoints);
            }
        }

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AssertDamageTypeChanged(HeroTurnTakerController httc, Card source, Card target, int amount, DamageType initialDamageType, DamageType expectedDamageType)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            this.RunCoroutine(this.GameController.DealDamage(httc, source, (Card c) => c == target, amount, initialDamageType, false, false, storedResults, null, null, false, null, null, false, false, new CardSource(GetCardController(source))));

            if (storedResults != null)
            {
                DealDamageAction dd = storedResults.FirstOrDefault<DealDamageAction>();
                DamageType actualDamageType = dd.DamageType;
                Assert.AreEqual(expectedDamageType, actualDamageType, $"Expected damage type: {expectedDamageType}. Actual damage type: {actualDamageType}");
            }
            else
            {
                Assert.Fail("storedResults was null");
            }
        }
        #endregion

        [Test()]
        [Order(0)]
        public void TestCeladroch_LoadedProperly()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(celadroch);
            Assert.IsInstanceOf(typeof(CeladrochTurnTakerController), celadroch);
            Assert.IsInstanceOf(typeof(CeladrochCharacterCardController), celadroch.CharacterCardController);
        }

        [Test()]
        public void TestCeladroch_StartGame()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            AssertInPlayArea(celadroch, celadroch.CharacterCard);
            AssertNotTarget(celadroch.CharacterCard);
            AssertNotFlipped(celadroch);

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            AssertInPlayArea(celadroch, p1);
            AssertInPlayArea(celadroch, p2);
            AssertInPlayArea(celadroch, p3);

            var topCard = celadroch.TurnTaker.Deck.TopCard;
            AssertCardSpecialString(celadroch.CharacterCard, 0, $"Celadroch's top card is {topCard.Title}");

            AssertNumberOfCardsInRevealed(celadroch, 0);
        }

        [Test()]
        public void TestCeladroch_DeckList()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertCardHasKeyword(celadroch.CharacterCard, "villain", false);

            AssertCard("PillarOfNight", new string[] { "relic" }, 25);
            AssertCard("PillarOfSky", new string[] { "relic" }, 25);
            AssertCard("PillarOfStorms", new string[] { "relic" }, 25);

            AssertCard("AvatarOfDeath", new string[] { "avatar" }, 20);

            AssertCard("SummersWrath", new string[] { "elemental" }, 5);
            AssertCard("WintersBane", new string[] { "elemental" }, 5);
            AssertCard("SpringsAtrophy", new string[] { "elemental" }, 5);
            AssertCard("AutumnsTorment", new string[] { "elemental" }, 5);

            AssertCard("TatteredDevil", new string[] { "demon" }, 10);
            AssertCard("HollowAngel", new string[] { "demon" }, 10);

            AssertCard("WhisperingBreath", new string[] { "zombie" }, 6);
            AssertCard("GraspingBreath", new string[] { "zombie" }, 6);
            AssertCard("LeechingBreath", new string[] { "zombie" }, 6);

            AssertCard("ForsakenCrusader", new string[] { "chosen" }, 3);
            AssertCard("LordOfTheMidnightRevel", new string[] { "chosen" }, 12);
            AssertCard("LaughingHag", new string[] { "chosen" }, 5);

            AssertCard("ScreamingGale", new string[] { "ongoing" });
            AssertCard("HoursTilDawn", new string[] { "ongoing" });
            AssertCard("RattlingWind", new string[] { "ongoing" });
            AssertCard("NightUnderTheMountain", new string[] { "ongoing" });
            AssertCard("LingeringExhalation", new string[] { "ongoing" });

            AssertCard("GallowsBlast", new string[] { "one-shot" });
            AssertCard("TheMountainsMadness", new string[] { "one-shot" });
        }

        [Test()]
        public void TestCeladroch_FrontCannotPlayCards()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            var card = PlayCard("AvatarOfDeath");

            AssertInDeck(card);
        }

        [Test()]
        public void TestCeladroch_FrontTestTokenGain()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");
            StartGame();

            AssertTokenPoolCount(stormPool, 1);
            GoToEndOfTurn(celadroch);

            GoToStartOfTurn(celadroch);
            AssertTokenPoolCount(stormPool, 2);
            GoToEndOfTurn(celadroch);
        }

        [Test()]
        public void TestCeladroch_FrontFlipOnTokens()
        {
            SetupGameController("Cauldron.Celadroch", "Legacy", "Megalopolis");

            stormPool.AddTokens(2);

            StartGame();

            AssertTokenPoolCount(stormPool, 3);
            GoToEndOfTurn(celadroch);

            GoToStartOfTurn(celadroch);
            AssertFlipped(celadroch);
            AssertTokenPoolCount(stormPool, 4); //flipped side token gain
        }

        [Test()]
        public void TestCeladroch_NormalFrontNoRelicDr()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Legacy", "Megalopolis" }, advanced: false);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            QuickHPStorage(p1, p2, p3);
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning);
            QuickHPCheck(-1, -1, -1);

            QuickHPUpdate();
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning, true);
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestCeladroch_AdvancedFrontRelicDr()
        {
            SetupGameController(new[] { "Cauldron.Celadroch", "Legacy", "Megalopolis" }, advanced: true);
            StartGame();

            var p1 = GetCard("PillarOfNight");
            var p2 = GetCard("PillarOfSky");
            var p3 = GetCard("PillarOfStorms");

            QuickHPStorage(p1, p2, p3);
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning);
            QuickHPCheckZero();

            QuickHPUpdate();
            DealDamage(legacy.CharacterCard, p1, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p2, 1, DamageType.Lightning, true);
            DealDamage(legacy.CharacterCard, p3, 1, DamageType.Lightning, true);

            QuickHPCheck(-1, -1, -1);
        }

    }
}

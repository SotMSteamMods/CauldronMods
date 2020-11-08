using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Instrumentation;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatTests : BaseTest
    {
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }
        protected Card inferno { get { return GetCardInPlay("InfernoTiamatCharacter"); } }
        protected Card storm { get { return GetCardInPlay("StormTiamatCharacter"); } }
        protected Card winter { get { return GetCardInPlay("WinterTiamatCharacter"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test()]
        public void TestTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(tiamat);
            Assert.IsInstanceOf(typeof(WinterTiamatCharacterCardController), tiamat.CharacterCardController);
            AssertNumberOfCardsInPlay(tiamat, 3);
            Assert.AreEqual(40, inferno.HitPoints);
            Assert.AreEqual(40, storm.HitPoints);
            Assert.AreEqual(40, winter.HitPoints);
        }

        [Test()]
        public void TestInfernoFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            Assert.IsTrue(inferno.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnInfernoIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            AssertNotGameOver();
        }

        [Test()]
        public void TestStormFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            Assert.IsTrue(storm.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnStormIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            AssertNotGameOver();
        }

        [Test()]
        public void TestWinterFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            Assert.IsTrue(winter.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnWinterIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            AssertNotGameOver();
        }

        [Test()]
        public void TestDecapitatedHeadCannotDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Cold);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestWinCondition()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            AssertNotGameOver();
            SetupIncap(legacy, inferno);
            AssertNotGameOver();
            SetupIncap(legacy, storm);
            AssertGameOver();
        }

        [Test()]
        public void TestInfernoImmuneToFire()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Fire);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestStormImmuneToLightning()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Lightning);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestWinterImmuneToCold()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Cold);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestInfernoEndOfTurnDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Tachyon", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            SetupIncap(legacy, storm);
            QuickHPStorage(legacy);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestWinterEndOfTurnDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Tachyon", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            SetupIncap(legacy, storm);
            QuickHPStorage(legacy);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestStormEndOfTurnDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Tachyon", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            SetupIncap(legacy, storm);
            QuickHPStorage(legacy);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAllHeadsEndOfTurnDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Haka", "Guise", "Parse", "Megalopolis");
            StartGame();
            QuickHPStorage(haka);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestInfernoAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestStormAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestWinterAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestInfernoIncapAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            SetupIncap(legacy, inferno);
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestStormIncapAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            SetupIncap(legacy, storm);
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestWinterIncapAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            SetupIncap(legacy, winter);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAcidBreathDestroy2HeroOngoing2HeroEquipment()
        {
            SetupGameController("Cauldron.Tiamat", "AbsoluteZero", "Ra", "Haka", "TheBlock");
            StartGame();
            Card glacial = GetCard("GlacialStructure");
            PlayCards(new Card[] {
                //2 Hero Equipment
                GetCard("CryoChamber"),
                GetCard("FocusedApertures"),
                //3 Hero Ongoing
                GetCard("ColdSnap"),
                GetCard("CoolantBlast"),
                glacial,
                //1 Villain Ongoing
                GetCard("AncientWard"),
                //1 Environment
                GetCard("BlockGuard")
            });

            PlayCard(GetCard("AcidBreath"));
            //Without choics cards are selected alphabetically. So this should be skipped
            AssertInPlayArea(az, glacial);
            //Environment shouldn't be destroyed
            AssertNumberOfCardsInPlay(env, 1);
            //Villain cards should be skipped
            AssertNumberOfCardsInPlay(tiamat, 4);
        }

        [Test()]
        public void TestAcidBreathDamage()
        {
            SetupGameController("Cauldron.Tiamat", "AbsoluteZero", "Ra", "Haka", "TheBlock");
            StartGame();
            Card glacial = GetCard("GlacialStructure");
            PlayCards(new Card[] {
                //2 Hero Equipment
                GetCard("CryoChamber"),
                GetCard("FocusedApertures"),
                //3 Hero Ongoing
                GetCard("ColdSnap"),
                GetCard("CoolantBlast"),
                glacial,
                //1 Villain Ongoing
                GetCard("AncientWard"),
                //1 Environment
                GetCard("BlockGuard")
            });

            QuickHPStorage(ra.CharacterCard, haka.CharacterCard);
            PlayCard(GetCard("AcidBreath"));
            QuickHPCheck(-3, -3);
        }

        [Test()]
        public void TestAlteration()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of decks
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, ward);
            Card traffic = GetCard("TrafficPileup");
            PutOnDeck(env, traffic);
            //Assert nothing in play
            AssertNumberOfCardsInPlay(tiamat, 3);
            AssertNumberOfCardsInPlay(env, 0);
            //Play card and make sure top of env and villain played
            PlayCard(tiamat, "Alteration");
            AssertInPlayArea(tiamat, ward);
            AssertInPlayArea(env, traffic);
        }

        [Test()]
        public void TestAncientWard()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "AncientWard");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDragonsWrath()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "DragonsWrath");

            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(legacy);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestElementalFormSameTypes()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-2);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(0);

            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-2);
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(0);

            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-2);
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestElementalFormDifferentTypes()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(0);

            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Projectile);
            QuickHPCheck(-2);
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Projectile);
            QuickHPCheck(0);

            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Radiant);
            QuickHPCheck(-2);
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Radiant);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestElementalFormUntilStart()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
            GoToStartOfTurn(tiamat);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestElementalFormMultipleImmune()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            DealDamage(legacy, inferno, 2, DamageType.Projectile);
            DealDamage(legacy, inferno, 2, DamageType.Radiant);

            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            DealDamage(legacy, inferno, 2, DamageType.Radiant);
            DealDamage(legacy, inferno, 2, DamageType.Projectile);

            QuickHPCheck(-6);
        }

        [Test()]
        public void TestElementOfFireDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, "ElementOfFire");
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestElementOfFireDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfFire", 1));
            PutInTrash(GetCard("ElementOfFire", 2));
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfFire"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfFireCantPlayCards()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard("OmniCannon");
            PlayCard(tiamat, GetCard("ElementOfFire"));
            AssertCanPlayCards(legacy);
            AssertCanPlayCards(haka);
            AssertCannotPlayCards(bunker);
        }

        [Test()]
        public void TestElementOfIceDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, "ElementOfIce");
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestElementOfIceDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfIce", 1));
            PutInTrash(GetCard("ElementOfIce", 2));
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfIce"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfIceCantUsePowers()
        {
            SetupGameController("Cauldron.Tiamat", "Parse", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, GetCard("ElementOfIce"));
            GoToUsePowerPhase(parse);
            AssertNumberOfUsablePowers(parse, 1);
            GoToUsePowerPhase(haka);
            AssertNumberOfUsablePowers(haka, 0);
            GoToUsePowerPhase(bunker);
            AssertNumberOfUsablePowers(bunker, 1);
        }

        [Test()]
        public void TestElementOfLightningDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, "ElementOfLightning");
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestElementOfLightningDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfLightning", 1));
            PutInTrash(GetCard("ElementOfLightning", 2));
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfLightningCantDrawCards()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            DrawCard(bunker);
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToDrawCardPhase(legacy);
            AssertCanPerformPhaseAction();
            GoToDrawCardPhase(bunker);
            AssertCannotPerformPhaseAction();
            GoToDrawCardPhase(haka);
            AssertCanPerformPhaseAction();
        }

        [Test()]
        public void TestInfernoIncapEffect0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestInfernoIncapEffect2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfFire", 1));
            PutInTrash(GetCard("ElementOfFire", 2));
            SetupIncap(legacy, inferno);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestStormIncapEffect0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfIce"));
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestStormIncapEffect2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" );
            StartGame();
            PutInTrash(GetCard("ElementOfLightning", 1));
            PutInTrash(GetCard("ElementOfLightning", 2));
            SetupIncap(legacy, storm);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfIce"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestWinterIncapEffect0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfFire"));
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestWinterIncapEffect2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfIce", 1));
            PutInTrash(GetCard("ElementOfIce", 2));
            SetupIncap(legacy, winter);
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfFire"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementalFrenzyPutUnder()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ward = GetCard("AncientWard");
            Card frenzy = GetCard("ElementalFrenzy");
            Card fire = GetCard("ElementOfFire", 1);
            PutInTrash(fire);
            PutInTrash(GetCard("ElementOfFire", 2));
            PutInTrash(GetCard("ElementOfIce", 1));
            PutInTrash(GetCard("ElementOfIce", 2));
            PutInTrash(GetCard("ElementOfLightning", 1));
            PutInTrash(GetCard("ElementOfLightning", 2));
            PutInTrash(ward);

            PlayCard(frenzy);

            AssertNumberOfCardsUnderCard(frenzy, 6);
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertInTrash(ward);
            Assert.IsTrue(!fire.IsFaceUp);
        }

        [Test()]
        public void TestElementalFrenzyPlayUnder()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");
            PutInTrash(GetCard("ElementOfFire", 1));
            PutInTrash(GetCard("ElementOfFire", 2));

            PlayCard(frenzy);

            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(tiamat);
            //End of Turn effects of heads deal 2 to Haka and 1 to Legacy
            QuickHPCheck(-3, -2, -4);

            AssertNumberOfCardsInTrash(tiamat, 1);
        }

        [Test()]
        public void TestElementalFrenzyDestroySelf()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");
            PutInTrash(GetCard("ElementOfFire", 1));

            PlayCard(frenzy);
            GoToEndOfTurn(tiamat);

            AssertInTrash(tiamat, frenzy);
        }

        [Test()]
        public void TestElementalFrenzy0CardInTrashDestroySelf()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");

            PlayCard(frenzy);

            AssertInTrash(tiamat, frenzy);
        }

        [Test()]
        public void TestHealingMagic0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of deck
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, ward);
            DealDamage(legacy, inferno, 10, DamageType.Psychic);
            //Checks
            QuickHPStorage(inferno);
            PlayCard(tiamat, "HealingMagic");
            QuickHPCheck(3);
            AssertInPlayArea(tiamat, ward);
        }

        [Test()]
        public void TestManaChargeDiscard()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card charge = GetCard("ManaCharge");
            Card fire = GetCard("ElementOfFire");
            Card ice = GetCard("ElementOfIce");
            Card lightning = GetCard("ElementOfLightning");
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, new Card[] { ice, ward, fire, lightning });
            AssertNumberOfCardsInTrash(tiamat, 0);

            QuickShuffleStorage(tiamat);
            PlayCard(tiamat, charge);
            AssertNumberOfCardsInTrash(tiamat, 4);
            AssertInTrash(tiamat, new Card[] { ice, charge, fire, lightning });
            AssertNotInTrash(tiamat, ward.Identifier);
            QuickShuffleCheck(1);
            AssertNumberOfCardsInRevealed(tiamat, 0);
        }

        [Test()]
        public void TestManaChargeElementalFrenzyShuffle()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(tiamat, new Card[] { GetCard("ElementalFrenzy"), GetCard("ElementalFrenzy", 1) });

            QuickShuffleStorage(tiamat);
            PlayCard(tiamat, "ManaCharge");
            AssertNumberOfCardsInTrash(tiamat, 4);
            QuickShuffleCheck(2);
            AssertNumberOfCardsInRevealed(tiamat, 0);
        }

        [Test()]
        public void TestReptiliatnAspect()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            DealDamage(legacy, inferno, 5, DamageType.Melee);
            DealDamage(legacy, storm, 5, DamageType.Melee);
            DealDamage(legacy, winter, 5, DamageType.Melee);
            DealDamage(legacy, haka, 5, DamageType.Melee);
            QuickHPStorage(new Card[] { inferno, storm, winter, haka.CharacterCard });
            PlayCard(tiamat, "ReptilianAspect");
            GoToEndOfTurn(tiamat);
            QuickHPCheck(1, 1, 1, 0);
        }

        [Test()]
        public void TestSkyBreaker()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(new Card[] { inferno, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard });
            PlayCard(tiamat, "SkyBreaker");
            QuickHPCheck(0, -5, -5, -5);
        }
    }
}

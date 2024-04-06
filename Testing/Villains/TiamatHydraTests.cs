using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatHydraTests : CauldronBaseTest
    {
        protected Card inferno { get { return GetCardInPlay("HydraInfernoTiamatCharacter"); } }
        protected Card storm { get { return GetCardInPlay("HydraStormTiamatCharacter"); } }
        protected Card winter { get { return GetCardInPlay("WinterTiamatCharacter"); } }
        protected Card decay { get { return GetCardInPlay("HydraDecayTiamatCharacter"); } }
        protected Card wind { get { return GetCardInPlay("HydraWindTiamatCharacter"); } }
        protected Card earth { get { return GetCardInPlay("HydraEarthTiamatCharacter"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Radiant);
        }

        private void SetupHead(TurnTakerController source, Card secondHead)
        {
            Card target = null;
            if (secondHead == decay)
            {
                target = inferno;
            }
            else if (secondHead == wind)
            {
                target = storm;
            }
            else if (secondHead == earth)
            {
                target = winter;
            }
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Radiant);
            GoToStartOfTurn(tiamat);
        }

        private void AddShuffleTrashCounterAttackTrigger(TurnTakerController ttc, TurnTaker turnTakerToReshuffleTrash)
        {
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.Target == ttc.CharacterCard;
            Func<DealDamageAction, IEnumerator> response = (DealDamageAction dd) => this.GameController.ShuffleTrashIntoDeck(this.GameController.FindTurnTakerController(turnTakerToReshuffleTrash));
            this.GameController.AddTrigger<DealDamageAction>(new Trigger<DealDamageAction>(this.GameController, criteria, response, new TriggerType[] { TriggerType.ShuffleTrashIntoDeck }, TriggerTiming.After, this.GameController.FindCardController(turnTakerToReshuffleTrash.CharacterCard).GetCardSource()));
        }

        private bool IsSpell(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "spell");
        }

        [Test()]
        public void TestHydraTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            var a = tiamat.CharacterCardController;
            Assert.IsNotNull(tiamat);
            //Assert.IsInstanceOf(typeof(WinterTiamatCharacterCardController), tiamat.CharacterCardController);
            AssertNumberOfCardsInPlay(tiamat, 6);
            Assert.AreEqual(15, inferno.HitPoints);
            Assert.AreEqual(15, storm.HitPoints);
            Assert.AreEqual(15, winter.HitPoints);
        }

        [Test()]
        public void TestInfernoFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            //Head flips on incap
            Assert.IsTrue(inferno.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnInfernoIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestStormFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            //Head flips on incap
            Assert.IsTrue(storm.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnStormIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestWinterFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Head flips on incap
            Assert.IsTrue(winter.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnWinterIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestDidNotWinOnWindIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupHead(haka, wind);
            SetupIncap(legacy, wind);
            //Head is flipped
            Assert.IsTrue(wind.IsFlipped);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestDidNotWinOnEarthIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupHead(haka, earth);
            SetupIncap(legacy, earth);
            //Head is flipped
            Assert.IsTrue(earth.IsFlipped);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestDidNotWinOnDecayIncap()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupHead(haka, decay);
            SetupIncap(legacy, decay);
            //Head is flipped
            Assert.IsTrue(decay.IsFlipped);
            //Should only win when all 6 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestDecapitatedHeadCannotDealDamage()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(haka, inferno);
            //Flipped heads cannot deal damage
            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Cold);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestDecapitatedHeadNotATarget()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Flipped heads are not targets
            AssertNotTarget(winter);
        }

        [Test()]
        public void TestWinCondition()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, storm);
            SetupIncap(legacy, winter);
            SetupIncap(legacy, inferno);
            AssertNotGameOver();
            GoToStartOfTurn(tiamat);
            //Reincap Inferno after regrowing at start of turn
            SetupIncap(legacy, inferno);
            SetupIncap(legacy, decay);
            AssertNotGameOver();
            SetupIncap(legacy, earth);
            AssertNotGameOver();
            SetupIncap(legacy, wind);
            //Win if all heads are flipped
            AssertGameOver();
        }

        [Test()]
        public void TestInfernoImmuneToFire()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(inferno);
            //Inferno Tiamat should be immune to fire damage
            DealDamage(legacy, inferno, 2, DamageType.Fire);
            QuickHPCheck(0);

            //test only immune to fire
            QuickHPUpdate();
            DealDamage(legacy, inferno, 2, DamageType.Infernal);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestStormImmuneToLightning()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(storm);
            //Storm Tiamat should be immune to lightning damage
            DealDamage(legacy, storm, 2, DamageType.Lightning);
            QuickHPCheck(0);

            //test only immune to lightning
            QuickHPUpdate();
            DealDamage(legacy, storm, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestWinterImmuneToCold()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(winter);
            //Inferno Tiamat should be immune to cold damage
            DealDamage(legacy, winter, 2, DamageType.Cold);
            QuickHPCheck(0);

            //test only immune to cold
            QuickHPUpdate();
            DealDamage(legacy, winter, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestWindImmuneToProjectile()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupHead(haka, wind);

            QuickHPStorage(wind);
            //Inferno Tiamat should be immune to cold damage
            DealDamage(legacy, wind, 2, DamageType.Projectile);
            QuickHPCheck(0);

            //test only immune to projectile
            QuickHPUpdate();
            DealDamage(legacy, wind, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestEarthImmuneToMelee()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupHead(haka, earth);

            QuickHPStorage(earth);
            //Inferno Tiamat should be immune to melee damage
            DealDamage(legacy, earth, 2, DamageType.Melee);
            QuickHPCheck(0);

            //test only immune to melee
            QuickHPUpdate();
            DealDamage(legacy, earth, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDecayImmuneToToxic()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupHead(haka, decay);

            QuickHPStorage(decay);
            //Inferno Tiamat should be immune to toxic damage
            DealDamage(legacy, decay, 2, DamageType.Toxic);
            QuickHPCheck(0);

            //test only immune to toxic
            QuickHPUpdate();
            DealDamage(legacy, decay, 2, DamageType.Infernal);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFrontInstructionsEndOfTurnFront()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Haka", "Ra", "Parse", "Megalopolis");
            StartGame();
            QuickHPStorage(haka, ra, parse);
            QuickHandStorage(ra);
            //End of turn damage checking happens per head
            DecisionSelectTurnTaker = ra.TurnTaker;
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-3, -1, -1);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestAcidBreathDestroy2HeroOngoing2HeroEquipment()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "AbsoluteZero", "Ra", "Haka", "TheBlock");
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
            //These should be destroyed
            AssertInTrash(az, new Card[] { GetCard("CryoChamber"), GetCard("FocusedApertures"), GetCard("ColdSnap"), GetCard("CoolantBlast") });
            //Without choics cards are selected alphabetically. So this should be skipped
            AssertInPlayArea(az, glacial);
            //Environment shouldn't be destroyed
            AssertNumberOfCardsInPlay(env, 1);
            //Villain cards should be skipped
            AssertNumberOfCardsInPlay(tiamat, 7);
        }

        [Test()]
        public void TestAcidBreathDamage()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "AbsoluteZero", "Ra", "Haka", "TheBlock");
            StartGame();
            GoToPlayCardPhase(tiamat);

            //set hp of heads
            SetHitPoints(inferno, 5);
            SetHitPoints(storm, 4);
            SetHitPoints(winter, 6);

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

            //Heroes that don't destroy a card get H damage
            QuickHPStorage(ra.CharacterCard, haka.CharacterCard, az.CharacterCard);

            //damage should be dealt by highest hp, which is winter
            //adding cannot deal damage status effects to storm and inferno
            AddCannotDealNextDamageTrigger(ra, inferno);
            AddCannotDealNextDamageTrigger(ra, storm);
            PlayCard(GetCard("AcidBreath"));
            QuickHPCheck(-3, -3, 0);
        }

        [Test()]
        public void TestAcidBreathNoDamageIfNoActiveHeads()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "AbsoluteZero", "Ra", "Haka", "TheBlock");
            StartGame();
            GoToPlayCardPhase(tiamat);

            SetupIncap(haka, storm);
            SetupIncap(haka, winter);
            SetupIncap(haka, inferno);

            PlayCards(new Card[] {
                //2 Hero Equipment
                GetCard("CryoChamber"),
                GetCard("FocusedApertures"),
                //3 Hero Ongoing
                GetCard("ColdSnap"),
                GetCard("CoolantBlast"),
                GetCard("GlacialStructure"),
                //1 Villain Ongoing
                GetCard("AncientWard"),
                //1 Environment
                GetCard("BlockGuard")
            });

            //Heroes that don't destroy a card get H damage
            QuickHPStorage(ra.CharacterCard, haka.CharacterCard, az.CharacterCard);

            PlayCard(GetCard("AcidBreath"));

            // No one damaged, because no head to deal the damage
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestAlteration()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of decks
            Card ward = PutOnDeck("AncientWard");
            Card traffic = PutOnDeck("TrafficPileup");
            //Assert nothing in play
            AssertNumberOfCardsInPlay(tiamat, 6);
            AssertNumberOfCardsInPlay(env, 0);
            //Play card and make sure top of env and villain played
            PlayCard(tiamat, "Alteration");
            AssertInPlayArea(tiamat, ward);
            AssertInPlayArea(env, traffic);
        }

        [Test()]
        public void TestAncientWard()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ward = PlayCard(tiamat, "AncientWard");

            //Heads take 1 less damage
            PrintSeparator("check all heads reduced by 1");
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //non-heads take damage normally
            PrintSeparator("check non-heads normal");
            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //after destroying ancient ward, should be normal
            PrintSeparator("check damage to heads normal after ancient ward is gone");
            DestroyCard(ward, bunker.CharacterCard);

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestDragonsWrath()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card wrath = PlayCard(tiamat, "DragonsWrath");
            //Heads deal 1 extra damage
            PrintSeparator("check all heads increased by 1");

            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(legacy);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //non-heads take damage normally
            PrintSeparator("check non-heads normal");
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //after destroying dragon's wrath, should be normal
            PrintSeparator("check damage dealt by heads normal after dragon's wrath is gone");
            DestroyCard(wrath, bunker.CharacterCard);
            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(legacy);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestElementalFormSameTypes()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            //Until start of next turn heads become immune to a damage type when they take that damage
            PrintSeparator("check for inferno");
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(0);

            PrintSeparator("check for storm");
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(0);

            PrintSeparator("check for winter");
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestElementalFormZeroDamageDealt()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard("ElementalForm");

            //Until start of next turn heads become immune to a damage type when they take that damage
            PrintSeparator("check for inferno");
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 0, DamageType.Melee);
            QuickHPCheck(0);
            //Should not be immune now
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestElementalFormDifferentTypes()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard("ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(0);
            //Should only be immune to Psychic
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Projectile);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(storm);
            DealDamage(legacy, storm, 2, DamageType.Projectile);
            QuickHPCheck(0);
            //Should only be immune to Psychic and Radiant
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Radiant);
            QuickHPCheck(-2);
            //Should be immune now
            QuickHPStorage(winter);
            DealDamage(legacy, winter, 2, DamageType.Radiant);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestElementalFormUntilStart()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
            //Immunity only lasts until start of turn
            GoToStartOfTurn(tiamat);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestElementalFormRemainsAfterDestruction()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card form = PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);

            DestroyCard(form, bunker.CharacterCard);

            //should still be immune even with elemental form gone
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(0);

            //Immunity should go away at next start of turn
            GoToStartOfTurn(tiamat);
            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestElementalFormMultipleImmune()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

            QuickHPStorage(inferno);
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            DealDamage(legacy, inferno, 2, DamageType.Projectile);
            DealDamage(legacy, inferno, 2, DamageType.Radiant);
            //Can be immune to multiple types of damage
            DealDamage(legacy, inferno, 2, DamageType.Psychic);
            DealDamage(legacy, inferno, 2, DamageType.Radiant);
            DealDamage(legacy, inferno, 2, DamageType.Projectile);

            QuickHPCheck(-6);
        }

        [Test()]
        public void TestElementOfFireDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
            QuickHPStorage(legacy, bunker, haka);
            //mouth of inferno should be the one dealing this damage
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, winter);
            PlayCard(tiamat, "ElementOfFire");
            QuickHPCheck(-2, -2, -2);
        }



        [Test()]
        public void TestElementOfFireDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card fire = GetCard("ElementOfFire");

            //put the other two copies of element of fire in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfFire" && c != fire).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, fire);
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, winter);
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfFireDamage2InTrash_DynamicValues()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card fire = GetCard("ElementOfFire");

            //put the other two copies of element of fire in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfFire" && c != fire).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
            QuickHPStorage(legacy, bunker, haka);
            AddShuffleTrashCounterAttackTrigger(legacy, tiamat.TurnTaker);
            PlayCard(tiamat, fire);
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, winter);
            //after damage is dealt to legacy, tiamat's trash should have been shuffled into the deck
            //this means that the subsequent damage will only be 2
            QuickHPCheck(-4, -2, -2);
        }

        [Test()]
        public void TestElementOfFireCantPlayCards()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard("OmniCannon");
            PlayCard(tiamat, GetCard("ElementOfFire"));
            //The hero with most cards in play cannot play cards until start of next villain turn
            AssertCanPlayCards(legacy);
            AssertCanPlayCards(haka);
            AssertCannotPlayCards(bunker);
        }

        [Test()]
        public void TestElementOfFireCanPlayCardsNextTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard("OmniCannon");
            //The hero with most cards in play cannot play cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfFire"));
            GoToStartOfTurn(tiamat);
            AssertCanPlayCards(bunker);
        }

        [Test()]
        public void TestElementOfIceDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Winter deals 2+X damage to each hero where X is the number of Element of Ices in trash
            QuickHPStorage(legacy, bunker, haka);
            //winter is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, "ElementOfIce");

            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestElementOfIceDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ice = GetCard("ElementOfIce");

            //put the other two copies of element of ice in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfIce" && c != ice).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of ices in trash
            QuickHPStorage(legacy, bunker, haka);
            //winter is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, ice);
            //since there are 2 cards in trash, should deal 4 damage per hero
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfIceDamage2InTrash_DynamicValues()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card ice = GetCard("ElementOfIce");

            //put the other two copies of element of ice in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfIce" && c != ice).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of ices in trash
            QuickHPStorage(legacy, bunker, haka);
            AddShuffleTrashCounterAttackTrigger(legacy, tiamat.TurnTaker);
            //winter is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, ice);
            //after damage is dealt to legacy, tiamat's trash should have been shuffled into the deck
            //this means that the subsequent damage will only be 2
            QuickHPCheck(-4, -2, -2);
        }

        [Test()]
        public void TestElementOfIceCantUsePowers()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Highest HP cannot use powers until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfIce"));
            GoToUsePowerPhase(parse);
            AssertNumberOfUsablePowers(parse, 1);
            GoToUsePowerPhase(haka);
            AssertNumberOfUsablePowers(haka, 0);
            GoToUsePowerPhase(bunker);
            AssertNumberOfUsablePowers(bunker, 1);
        }

        [Test()]
        public void TestElementOfIceCanUsePowersNextTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Highest HP cannot use powers until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfIce"));
            GoToUsePowerPhase(haka);
            GoToUsePowerPhase(haka);
            AssertNumberOfUsablePowers(haka, 1);
        }

        [Test()]
        public void TestElementOfLightningDamage0InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Storm deals 2+X damage to each hero where X is the number of Element of Lightnings in trash
            QuickHPStorage(legacy, bunker, haka);
            //storm is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, winter);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, "ElementOfLightning");
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestElementOfLightningDamage2InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card lightning = GetCard("ElementOfLightning");

            //put the other two copies of element of lightning in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfLightning" && c != lightning).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Lightnings in trash
            QuickHPStorage(legacy, bunker, haka);
            //storm is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, winter);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, lightning);
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfLightningDamage2InTrash_DynamicValues()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card lightning = GetCard("ElementOfLightning");

            //put the other two copies of element of lightning in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfLightning" && c != lightning).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Lightning in trash
            QuickHPStorage(legacy, bunker, haka);
            AddShuffleTrashCounterAttackTrigger(legacy, tiamat.TurnTaker);
            //storm is the one who should be dealing the damage
            AddCannotDealNextDamageTrigger(tiamat, winter);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, lightning);
            //after damage is dealt to legacy, tiamat's trash should have been shuffled into the deck
            //this means that the subsequent damage will only be 2
            QuickHPCheck(-4, -2, -2);
        }

        [Test()]
        public void TestElementOfLightningCantDrawCards()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            DrawCard(bunker);
            //The hero with most cards in hand cannot draw cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToDrawCardPhase(legacy);
            AssertCanPerformPhaseAction();
            GoToDrawCardPhase(bunker);
            AssertCannotPerformPhaseAction();
            GoToDrawCardPhase(haka);
            AssertCanPerformPhaseAction();
        }

        [Test()]
        public void TestElementOfLightningCantDrawCards_MultiChar()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "TheSentinels", "Megalopolis");
            StartGame();
            DrawCard(sentinels); ;
            //The hero with most cards in hand cannot draw cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToStartOfTurn(legacy);
            QuickHandStorage(legacy, bunker, haka, sentinels);
            DrawCard(legacy);
            DrawCard(bunker);
            DrawCard(haka);
            DrawCard(sentinels);
            QuickHandCheck(1, 1, 1, 0);
            GoToStartOfTurn(tiamat);
            QuickHandStorage(sentinels);
            DrawCard(sentinels);
            QuickHandCheck(1);

            PrintJournal();

        }

        [Test()]
        public void TestElementOfLightningCantDrawCards_CompletionistGuise()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Guise/CompletionistGuiseCharacter", "Megalopolis");
            StartGame();
            DrawCard(bunker); ;
            //The hero with most cards in hand cannot draw cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToStartOfTurn(legacy);
            DecisionSelectCard = bunker.CharacterCard;
            UsePower(guise);
            QuickHandStorage(legacy, bunker, haka, guise);
            DrawCard(legacy);
            DrawCard(bunker);
            DrawCard(haka);
            DrawCard(guise);
            QuickHandCheck(1, 0, 1, 1);
            GoToStartOfTurn(tiamat);
            QuickHandStorage(bunker);
            DrawCard(bunker);
            QuickHandCheck(1);

            PrintJournal();
        }

        [Test()]
        public void TestElementOfLightningCanDrawCardsNextTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            DrawCard(bunker);
            //The hero with most cards in hand cannot draw cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToDrawCardPhase(bunker);
            GoToDrawCardPhase(bunker);
            AssertCanPerformPhaseAction();
        }

        [Test()]
        public void TestElementalFrenzyPutUnder()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ward = PutInTrash("AncientWard");

            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => this.IsSpell(c)).Take(6)).ToList();
            PutInTrash(spellCards);

            //Elemental Frenzy puts all spells in trash face down under it
            Card frenzy = PlayCard("ElementalFrenzy");
            AssertNumberOfCardsUnderCard(frenzy, 6);
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertInTrash(ward);
            foreach (Card card in spellCards)
            {
                Assert.IsTrue(!card.IsFaceUp, $"{card.Title} was face up.");
                AssertUnderCard(frenzy, card);
            }

        }

        [Test()]
        public void TestElementalFrenzyPutUnder_OnlyOwnTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ward = PutInTrash("AncientWard");

            //put in the wrong trash
            Card otherSpell = GetCard("ElementOfIce");
            PutInTrash(legacy, otherSpell);

            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => this.IsSpell(c)).Take(6)).ToList();
            PutInTrash(spellCards);

            //Elemental Frenzy puts all spells in trash face down under it
            Card frenzy = PlayCard("ElementalFrenzy");
            AssertNumberOfCardsUnderCard(frenzy, 6);
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertInTrash(ward);
            foreach (Card card in spellCards)
            {
                AssertUnderCard(frenzy, card);
            }

            //confirm other spell is still in the wrong trash
            AssertInTrash(legacy, otherSpell);

        }

        [Test()]
        public void TestElementalFrenzyPlayUnder()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            List<Card> elementOfFires = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfFire").Take(2)).ToList();
            PutInTrash(elementOfFires);

            //At end of turn play the top card of the pile under Elemental Frenzy
            Card frenzy = PlayCard("ElementalFrenzy");

            //check the trash and under card values before end of turn
            AssertNumberOfCardsUnderCard(frenzy, 2);
            AssertNumberOfCardsInTrash(tiamat, 0);

            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(tiamat);
            //End of Turn effects of heads deal 3 to Haka and 1 to Legacy and 1 to Bunker
            //The spell played deals 2 to each hero
            QuickHPCheck(-3, -3, -5);

            //check the trash and under card values after end of turn
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertNumberOfCardsUnderCard(frenzy, 1);
        }

        [Test()]
        public void TestElementalFrenzyDestroySelf()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");

            PutInTrash(GetCard("ElementOfFire"));

            //Elemental Frenzy destroys self when no more cards underneath it
            PlayCard(frenzy);
            AssertInPlayArea(tiamat, frenzy);
            GoToEndOfTurn(tiamat);
            AssertInTrash(tiamat, frenzy);
        }

        [Test()]
        public void TestElementalFrenzyPlayOnDestroy()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");
            Card ward = PutOnDeck("AncientWard");
            PutInTrash(GetCard("ElementOfFire"));

            //when this card is destroyed, play the top card of the villain deck
            PlayCard(frenzy);
            DestroyCard(frenzy, bunker.CharacterCard);
            //top card of deck should have been played, which was ward
            AssertInPlayArea(tiamat, ward);
        }

        [Test()]
        public void TestElementalFrenzy0CardInTrashDestroySelf()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");

            //Elemental Frenzy destroys self when no cards underneath it
            PlayCard(frenzy);
            AssertInTrash(tiamat, frenzy);
        }

        [Test()]
        public void TestHealingMagic0InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of deck
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, ward);
            SetHitPoints(inferno, 3);
            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            QuickHPStorage(inferno);
            PlayCard(tiamat, "HealingMagic");
            QuickHPCheck(3);
            //The top card of the villain deck is played
            AssertInPlayArea(tiamat, ward);
        }

        [Test()]
        public void TestHealingMagic2InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of deck
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, ward);
            SetHitPoints(inferno, 3);

            Card magic = GetCard("HealingMagic");

            //put the other two copies of element of lightning in the trash
            List<Card> magicCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "HealingMagic" && c != magic).Take(2)).ToList();
            PutInTrash(magicCards);

            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            QuickHPStorage(inferno);
            PlayCard(tiamat, magic);
            QuickHPCheck(5);
            //The top card of the villain deck is played
            AssertInPlayArea(tiamat, ward);
        }

        [Test]
        public void TestHeadlingMagicNoActiveHeads()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(haka, winter);
            SetupIncap(haka, storm);
            SetupIncap(haka, inferno);

            //Setup top of deck
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, ward);

            Card magic = GetCard("HealingMagic");

            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            PlayCard(tiamat, magic);

            //The top card of the villain deck is played
            AssertInPlayArea(tiamat, ward);

            //No active heads, so no heads healed
            AssertNumberOfCardsAtLocation(tiamat.TurnTaker.PlayArea, 0, (Card c) => c.IsTarget);
        }

        [Test()]
        public void TestManaChargeDiscard()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card charge = GetCard("ManaCharge");
            Card fire = GetCard("ElementOfFire");
            Card ice = GetCard("ElementOfIce");
            Card lightning = GetCard("ElementOfLightning");
            Card ward = GetCard("AncientWard");
            PutOnDeck(tiamat, new Card[] { ice, ward, fire, lightning });

            AssertNumberOfCardsInTrash(tiamat, 0);

            //Mana Charge discards the first 3 spells from the deck and returns all other revealed cards
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
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();


            List<Card> frenzyCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementalFrenzy").Take(2)).ToList();
            PutInTrash(tiamat, frenzyCards);

            //Mana Charge shuffles all copies of Elemental Frenzy in the trash to the deck
            QuickShuffleStorage(tiamat);
            PlayCard(tiamat, "ManaCharge");
            AssertNumberOfCardsInTrash(tiamat, 4);
            QuickShuffleCheck(2);
            AssertNumberOfCardsInRevealed(tiamat, 0);
            AssertInDeck(tiamat, frenzyCards);
        }

        [Test()]
        public void TestManaChargeElementalFrenzyShuffle_OnlyOwnDeck()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();


            List<Card> frenzyCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementalFrenzy").Take(1)).ToList();
            PutInTrash(tiamat, frenzyCards);

            List<Card> frenzyCardsOther = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementalFrenzy" && !frenzyCards.Contains(c)).Take(1)).ToList();
            PutInTrash(legacy, frenzyCardsOther);

            //Mana Charge shuffles all copies of Elemental Frenzy in the trash to the deck
            QuickShuffleStorage(tiamat);
            PlayCard(tiamat, "ManaCharge");
            AssertNumberOfCardsInTrash(tiamat, 4);
            QuickShuffleCheck(2);
            AssertNumberOfCardsInRevealed(tiamat, 0);
            AssertInDeck(tiamat, frenzyCards);
            AssertInTrash(legacy, frenzyCardsOther);
        }

        [Test()]
        public void TestReptilianAspect()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetHitPoints(inferno, 10);
            SetHitPoints(storm, 10);
            SetHitPoints(winter, 10);
            SetHitPoints(haka.CharacterCard, 20);
            QuickHPStorage(inferno, storm, winter, haka.CharacterCard);
            //Reptilian Aspect heals all heads for H - 2 at end of turn
            PlayCard(tiamat, "ReptilianAspect");
            GoToEndOfTurn(tiamat);
            //Haka takes 1 at end of turn from Storm
            QuickHPCheck(1, 1, 1, -1);
        }

        [Test()]
        public void TestSkyBreaker()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetHitPoints(storm, 8);
            SetHitPoints(inferno, 8);
            SetHitPoints(winter, 10);
            //Sky Breaker deals H + 2 to all heroes
            QuickHPStorage(new Card[] { inferno, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard });
            //winter has highest HP, so should deal the damamage
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            PlayCard(tiamat, "SkyBreaker");
            QuickHPCheck(0, -5, -5, -5);
        }

        [Test]
        public void TestSkyBreakerNoActiveHeads()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(haka, winter);
            SetupIncap(haka, storm);
            SetupIncap(haka, inferno);

            Card skyBreaker = GetCard("SkyBreaker");

            QuickHPStorage(legacy, bunker, haka);

            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            PlayCard(tiamat, skyBreaker);

            //No active heads, so no heads deal damage
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Head_IsHead([Values("HydraInfernoTiamatCharacter", "HydraStormTiamatCharacter", "WinterTiamatCharacter", "HydraEarthTiamatCharacter", "HydraDecayTiamatCharacter", "HydraWindTiamatCharacter")] string head)
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Ra", "Fanatic", "Megalopolis");
            //StartGame();

            Card card = GetCard(head);
            //AssertInPlayArea(tiamat, card);
            AssertCardHasKeyword(card, "head", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Villain_IsVillain([Values("HydraInfernoTiamatCharacter", "HydraStormTiamatCharacter", "WinterTiamatCharacter", "HydraEarthTiamatCharacter", "HydraDecayTiamatCharacter", "HydraWindTiamatCharacter")] string villain)
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Ra", "Fanatic", "Megalopolis");
            //StartGame();

            Card card = GetCard(villain);
            //AssertInPlayArea(tiamat, card);
            AssertCardHasKeyword(card, "villain", false);
        }

        [Test()]
        public void TestGrowEarth()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, winter);

            //Start of Turn if Winter is decap put Earth into play with 15 HP
            GoToStartOfTurn(tiamat);
            Assert.IsTrue(!earth.IsFlipped);
            AssertHitPoints(earth, 15);
            //Storm regrows 1 head at start of turn
            AssertHitPoints(winter, 6);
        }

        [Test()]
        public void TestGrowDecay()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, inferno);

            //Start of Turn if Winter is decap put Earth into play with 15 HP
            GoToStartOfTurn(tiamat);
            Assert.IsTrue(!decay.IsFlipped);
            AssertHitPoints(decay, 15);
            //Storm regrows 1 head at start of turn
            AssertHitPoints(inferno, 6);
        }

        [Test()]
        public void TestGrowWind()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, storm);

            //Start of Turn if Winter is decap put Earth into play with 15 HP
            GoToStartOfTurn(tiamat);
            Assert.IsTrue(!wind.IsFlipped);
            AssertHitPoints(wind, 15);
            //Thunderous Gale regrows 1 head at start of turn
            AssertHitPoints(storm, 6);
        }

        [Test()]
        public void TestRegrowHeadFront()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, inferno);
            //At start of turn Storm regrows 1 head at start of turn with H * 2 HP
            GoToStartOfTurn(tiamat);
            AssertHitPoints(inferno, 6);
        }

        [Test()]
        public void TestInstructionsFrontAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            //First time Storm or Inferno deal damage each turn increase that damage by 2
            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-8);
            //Only first time
            QuickHPStorage(legacy);
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-4);

            //Winter makes players discard a second card
            QuickHandStorage(legacy);
            GoToEndOfTurn(tiamat);
            QuickHandCheck(-2);
        }

        [Test()]
        public void TestRegrowHeadBack()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, storm);
            GoToStartOfTurn(tiamat);
            //At start of turn where Thunderous Gale side up regrow 1 head at start of turn with H * 2 HP
            AssertHitPoints(storm, 6);
        }

        [Test()]
        public void TestDecayEffect()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);

            QuickHPStorage(legacy, bunker, haka);
            DealDamage(decay, legacy.CharacterCard, 2, DamageType.Melee);
            DealDamage(storm, bunker.CharacterCard, 2, DamageType.Melee);
            DealDamage(winter, haka.CharacterCard, 2, DamageType.Melee);
            //Breath of Decay makes all heads do +1 damage
            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestEarthEffect()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, winter);
            GoToStartOfTurn(tiamat);

            QuickHPStorage(earth, storm, inferno);
            DealDamage(legacy.CharacterCard, earth, 2, DamageType.Radiant);
            DealDamage(haka.CharacterCard, storm, 2, DamageType.Radiant);
            DealDamage(bunker.CharacterCard, inferno, 2, DamageType.Radiant);
            //Earth makes all other heads take -1 damage
            QuickHPCheck(-2, -1, -1);
        }

        [Test()]
        public void TestWindHealEndOfTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, storm);
            GoToStartOfTurn(tiamat);

            DealDamage(legacy.CharacterCard, wind, 10, DamageType.Radiant);
            DealDamage(haka.CharacterCard, winter, 10, DamageType.Radiant);
            DealDamage(bunker.CharacterCard, inferno, 10, DamageType.Radiant);

            QuickHPStorage(wind, winter, inferno);
            //Wind makes all heads regain 2 HP at end of turn
            GoToEndOfTurn(tiamat);
            QuickHPCheck(2, 2, 2);
        }

        [Test()]
        public void TestNoxiousFireBackEndOfTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, storm);
            //At the end of the villain turn, if {InfernoTiamatCharacter} is active, she deals the hero target with the second highest HP 1 fire damage.
            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(tiamat);
            //Breath of Decay makes all heads do +1 damage
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestThunderousGaleBackEndOfTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, storm);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, inferno);
            //At the end of the villain turn, if {StormTiamatCharacter} is active, she deals the hero target with the highest HP 1 lightning damage.
            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-1, 0, 0);
        }

        [Test()]
        public void TestFrigidEarthBackEndOfTurn()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(legacy, winter);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, inferno);
            SetupIncap(legacy, storm);
            //At the end of the villain turn, if {WinterTiamatCharacter} is active, she deals the hero target with the lowest HP 1 cold damage.
            QuickHPStorage(legacy, bunker, haka);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(0, -1, 0);
        }

        [Test()]
        public void TestAdvanced1FlippedRegrow()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            SetupIncap(legacy, winter);
            GoToStartOfTurn(tiamat);
            //Advanced - Decapitated heads are restored to {H * 3} HP when they become active.
            AssertHitPoints(winter, 9);
        }

        [Test()]
        public void TestAdvanced2FlippedRegrow()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            SetupIncap(legacy, winter);
            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);
            //Advanced - Decapitated heads are restored to {H * 3} HP when they become active.
            AssertHitPoints(inferno, 9);
        }

        [Test()]
        public void TestAdvanced3FlippedRegrow()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            SetupIncap(legacy, winter);
            SetupIncap(legacy, inferno);
            SetupIncap(legacy, storm);
            GoToStartOfTurn(tiamat);
            //Advanced - Decapitated heads are restored to {H * 3} HP when they become active.
            AssertHitPoints(inferno, 9);
        }

        [Test()]
        public void TestAlternateElementOfFire0InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, inferno);
            //Whenever Element of Fire enters play and {InfernoTiamatCharacter} is decapitated, if {HydraDecayTiamatCharacter} is active she deals each hero target X toxic damage, where X = 2 plus the number of Acid Breaths in the villain trash.
            QuickHPStorage(legacy, haka, bunker);
            PlayCard("ElementOfFire");
            //Breath of Decay increase damage dealt by heads by 1
            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestAlternateElementOfFire1InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, inferno);

            PutInTrash("AcidBreath", 0);
            //Whenever Element of Fire enters play and {InfernoTiamatCharacter} is decapitated, if {HydraDecayTiamatCharacter} is active she deals each hero target X toxic damage, where X = 2 plus the number of Acid Breaths in the villain trash.
            QuickHPStorage(legacy, haka, bunker);
            PlayCard("ElementOfFire");
            //Breath of Decay increase damage dealt by heads by 1
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestAlternateElementOfFire2InTrash()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            GoToStartOfTurn(tiamat);
            SetupIncap(legacy, inferno);

            PutInTrash("AcidBreath", 0);
            PutInTrash("AcidBreath", 1);
            //Whenever Element of Fire enters play and {InfernoTiamatCharacter} is decapitated, if {HydraDecayTiamatCharacter} is active she deals each hero target X toxic damage, where X = 2 plus the number of Acid Breaths in the villain trash.
            QuickHPStorage(legacy, haka, bunker);
            PlayCard("ElementOfFire");
            //Breath of Decay increase damage dealt by heads by 1
            QuickHPCheck(-5, -5, -5);
        }

        [Test()]
        public void TestAlternateElementOfIce0InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Guise", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, winter);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, winter);
            //Whenever Element of Ice enters play and {WinterTiamatCharacter} is decapitated, if {HydraEarthTiamatCharacter} is active she deals the hero target with the highest HP X melee damage, where X = {H} plus the number of Sky Breaker cards in the villain trash.
            QuickHPStorage(parse, haka, guise);
            PlayCard("ElementOfIce");
            QuickHPCheck(0, -3, 0);
        }

        [Test()]
        public void TestAlternateElementOfIce1InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Guise", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, winter);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, winter);

            PutInTrash("SkyBreaker", 0);
            //Whenever Element of Ice enters play and {WinterTiamatCharacter} is decapitated, if {HydraEarthTiamatCharacter} is active she deals the hero target with the highest HP X melee damage, where X = {H} plus the number of Sky Breaker cards in the villain trash.
            QuickHPStorage(parse, haka, guise);
            PlayCard("ElementOfIce");
            QuickHPCheck(0, -4, 0);
        }

        [Test()]
        public void TestAlternateElementOfIce2InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Guise", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, winter);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, winter);

            PutInTrash("SkyBreaker", 0);
            PutInTrash("SkyBreaker", 1);
            //Whenever Element of Ice enters play and {WinterTiamatCharacter} is decapitated, if {HydraEarthTiamatCharacter} is active she deals the hero target with the highest HP X melee damage, where X = {H} plus the number of Sky Breaker cards in the villain trash.
            QuickHPStorage(parse, haka, guise);
            PlayCard("ElementOfIce");
            QuickHPCheck(0, -5, 0);
        }

        [Test()]
        public void TestAlternateElementOfLightning0InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, storm);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, storm);
            //Whenever Element of Lightning enters play and {StormTiamatCharacter} is decapitated, if {HydraWindTiamatCharacter} is active she deals the X hero targets with the Highest HP {H - 1} projectile damage each, where X = 1 plus the number of ongoing cards in the villain trash.
            QuickHPStorage(parse, haka, bunker);
            PlayCard("ElementOfLightning");
            QuickHPCheck(0, -2, 0);
        }

        [Test()]
        public void TestAlternateElementOfLightning1InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, storm);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, storm);
            PutInTrash("AncientWard", 0);
            //Whenever Element of Lightning enters play and {StormTiamatCharacter} is decapitated, if {HydraWindTiamatCharacter} is active she deals the X hero targets with the Highest HP {H - 1} projectile damage each, where X = 1 plus the number of ongoing cards in the villain trash.
            QuickHPStorage(parse, haka, bunker);
            PlayCard("ElementOfLightning");
            QuickHPCheck(0, -2, -2);
        }

        [Test()]
        public void TestAlternateElementOfLightning2InTrash()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" });
            StartGame();
            SetupIncap(haka, storm);
            GoToStartOfTurn(tiamat);
            SetupIncap(haka, storm);
            PutInTrash("AncientWard", 0);
            PutInTrash("ReptilianAspect", 0);
            //Whenever Element of Lightning enters play and {StormTiamatCharacter} is decapitated, if {HydraWindTiamatCharacter} is active she deals the X hero targets with the Highest HP {H - 1} projectile damage each, where X = 1 plus the number of ongoing cards in the villain trash.
            QuickHPStorage(parse, haka, bunker);
            PlayCard("ElementOfLightning");
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestNoWinConditionWhenInfernoMouthInstructionsFrontOut()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "WagerMaster", "Parse", "Bunker", "Haka", "Megalopolis" });

            //losingtothe odds causes a game over mid test, banish it.
            PutInTrash("LosingToTheOdds");

            //Wagelings can cause a game over immediately, so banish them
            MoveCards(wager, FindCardsWhere((Card c) => c.Identifier == "Wagelings"), wager.TurnTaker.Trash);

            StartGame();
            IEnumerable<Card> wagerCardsToDestroy = FindCardsWhere((Card c) => c.Owner == wager.TurnTaker && c.IsInPlay);
            DestroyCards(wagerCardsToDestroy);
            GoToPlayCardPhase(wager);
            SetHitPoints(parse, 10);
            SetHitPoints(bunker, 10);
            SetHitPoints(haka, 10);

            PlayCard("LosingToTheOdds");
            GoToEndOfTurn(wager);

            AssertNotGameOver();
        }

        [Test()]
        public void TestHydraTiamatMustTakeDownAllHeads()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" });
            StartGame();

            DestroyCard(inferno);
            DestroyCard(winter);

            GoToStartOfTurn(tiamat);
            foreach(Card head in new Card[] { inferno, winter, storm, decay, wind, earth})
            {
                if(head.IsInPlayAndHasGameText && !head.IsFlipped)
                {
                    DestroyCard(head);
                }
            }
            GoToStartOfTurn(tiamat);
            AssertNotGameOver();
        }



        [Test()]
        public void TestReloadNotLosingInformation()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" });
            StartGame();
            SaveAndLoad();

            SetupIncap(haka, storm);
            GoToStartOfTurn(tiamat);
            AssertIsInPlayAndNotUnderCard(wind);
        }

        [Test()]
        public void TestHydraTiamatChallenge()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            Card aspect = PlayCard("ReptilianAspect");
            DestroyCard(aspect);
            AssertIsInPlay(aspect);

            DestroyCard(inferno);
            DestroyCard(aspect);
            AssertIsInPlay(aspect);

            DestroyCard(winter);
            DestroyCard(aspect);
            AssertInTrash(aspect);

        }
        [Test()]
        public void TestElementalFrenzyIndestructible()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            Card fire = PutInTrash("ElementOfFire");
            Card frenzy = PlayCard("ElementalFrenzy");


            GoToEndOfTurn(tiamat);
            AssertIsInPlay(frenzy);

            Card tamoko = PlayCard("TaMoko");
            DiscardCard(parse);
            DestroyCard(tamoko);
            MoveCard(haka, tamoko, haka.HeroTurnTaker.Hand);

            Card aspect = PutOnDeck("ReptilianAspect");
            DestroyCard(winter);
            DestroyCard(inferno);
            AssertInTrash(frenzy);
            AssertIsInPlay(aspect);
        }
        [Test()]
        public void TestElementalFrenzyIndestructible_PickingUpExtraCards()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            Card frenzy = PlayCard("ElementalFrenzy");

            PlayCard("SkyBreaker");

            //if bug is occurring, at EoT, frenzy will pick up skybreaker and play it immediately
            //this would do 5 damage to all heroes
            //assert only end of turn head damage
            QuickHPStorage(parse, bunker, haka);
            GoToEndOfTurn(tiamat);
            AssertIsInPlay(frenzy);
            QuickHPCheck(-1,-1,-3);

         }
        [Test()]
        public void TestHydraTiamatHeadsRemovedFromGame()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Parse", "Bunker", "Haka", "TheFinalWasteland");
            StartGame();

            DestroyCard(inferno);
            DestroyCard(winter);
            DestroyCard(storm);
            GoToStartOfTurn(tiamat);
            GoToStartOfTurn(tiamat);
            GoToStartOfTurn(tiamat);

            PlayCard("UnforgivingWasteland");
            Card skunk = PlayCard("HorridSkunkApe");
            DealDamage(skunk, storm, 20, DamageType.Melee);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("HydraStormTiamatCharacter"));

            if(GameController.IsGameOver)
            {
                Assert.Pass();
            }

            SaveAndLoad();
            Card rfgStorm = tiamat.TurnTaker.FindCard("HydraStormTiamatCharacter");
            Log.Debug($"Storm location: {rfgStorm.Location.GetFriendlyName()}");
            AssertOutOfGame(rfgStorm);

            skunk = GetCardInPlay("HorridSkunkApe");
            DealDamage(skunk, inferno, 20, DamageType.Melee);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("HydraInfernoTiamatCharacter"));

            DealDamage(skunk, winter, 20, DamageType.Melee);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("WinterTiamatCharacter"));

            DealDamage(skunk, wind, 20, DamageType.Melee);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("HydraWindTiamatCharacter"));
            DealDamage(skunk, decay, 20, DamageType.Melee);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("HydraDecayTiamatCharacter"));
            DealDamage(skunk, earth, 20, DamageType.Fire);
            AssertOutOfGame(tiamat.TurnTaker.FindCard("HydraEarthTiamatCharacter"));

            AssertGameOver();
        }

        [Test()]
        public void TestFlipAndThenRewind ()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            
            // Get rid of all tiamat's cards so she can't interrupt us
            MoveAllCards(tiamat, tiamat.TurnTaker.Deck, tiamat.TurnTaker.OutOfGame);

            StartGame();

            GoToPlayCardPhase(legacy);

            SetupIncap(legacy, inferno);
            Assert.IsTrue(inferno.IsFlipped);

            GoToStartOfTurn(tiamat);

            // Inferno got flipped face up
            Assert.IsFalse(inferno.IsFlipped);

            // Decay entered play
            AssertIsInPlayAndNotUnderCard(decay);

            GoToNextTurn();

            SaveAndLoad(GameController);

            // These should still be true
            Assert.IsFalse(inferno.IsFlipped);
            AssertIsInPlayAndNotUnderCard(decay);

            GoToNextTurn();

            // These should still be true
            Assert.IsFalse(inferno.IsFlipped);
            AssertIsInPlayAndNotUnderCard(decay);
        }

        [Test()]
        public void TestRewindDoesntFlipUnderCards()
        {
            SetupGameController("Cauldron.Tiamat/HydraWinterTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");

            // Get rid of all tiamat's cards so she can't interrupt us
            MoveAllCards(tiamat, tiamat.TurnTaker.Deck, tiamat.TurnTaker.OutOfGame);

            StartGame();

            AssertUnderCard(inferno, decay);
            AssertUnderCard(winter, earth);
            AssertUnderCard(storm, wind);
            AssertFlipped(decay, wind, earth);

            GoToPlayCardPhase(legacy);

            SaveAndLoad(GameController);

            AssertUnderCard(inferno, decay);
            AssertUnderCard(winter, earth);
            AssertUnderCard(storm, wind);
            AssertFlipped(decay, wind, earth);

            GoToNextTurn();

            AssertUnderCard(inferno, decay);
            AssertUnderCard(winter, earth);
            AssertUnderCard(storm, wind);
            AssertFlipped(decay, wind, earth);
        }
    }
}

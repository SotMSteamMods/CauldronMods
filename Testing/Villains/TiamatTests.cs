using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.AbsoluteZero;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections;
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

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card; 
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
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
            //Head flips on incap
            Assert.IsTrue(inferno.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnInfernoIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            //Should only win when all 3 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestStormFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            //Head flips on incap
            Assert.IsTrue(storm.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnStormIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, storm);
            //Should only win when all 3 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestWinterFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Head flips on incap
            Assert.IsTrue(winter.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnWinterIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Should only win when all 3 heads are destroyed
            AssertNotGameOver();
        }

        [Test()]
        public void TestDecapitatedHeadCannotDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, winter);
            //Flipped heads cannot deal damage
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
            //Win if all heads are flipped
            AssertGameOver();
        }

        [Test()]
        public void TestInfernoImmuneToFire()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(inferno);
            //Inferno Tiamat should be immune to fire damage
            DealDamage(legacy, inferno, 2, DamageType.Fire);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestStormImmuneToLightning()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(storm);
            //Storm Tiamat should be immune to lightning damage
            DealDamage(legacy, storm, 2, DamageType.Lightning);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestWinterImmuneToCold()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            QuickHPStorage(winter);
            //Inferno Tiamat should be immune to cold damage
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
            //Inferno deals damage at the end of turn if she did not already deal damage this turn
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
            //Winter deals damage at the end of turn if she did not already deal damage this turn
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
            //Storm deals damage at the end of turn if she did not already deal damage this turn
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAllHeadsEndOfTurnDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Haka", "Guise", "Parse", "Megalopolis");
            StartGame();
            QuickHPStorage(haka);
            //End of turn damage checking happens per head
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestInfernoAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            //Advanced heads deal 1 extra damage
            DealDamage(inferno, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestStormAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            //Advanced heads deal 1 extra damage
            DealDamage(storm, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestWinterAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Guise", "Parse", "Megalopolis" }, true);
            StartGame();
            QuickHPStorage(legacy);
            //Advanced heads deal 1 extra damage
            DealDamage(winter, legacy, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestInfernoIncapAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            SetupIncap(legacy, inferno);
            QuickHPStorage(storm);
            //Advanced heads make the other heads take 1 less damage
            DealDamage(legacy, storm, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestStormIncapAdvancedEffect()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();
            SetupIncap(legacy, storm);
            QuickHPStorage(winter);
            //Advanced heads make the other heads take 1 less damage
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
            //Advanced heads make the other heads take 1 less damage
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
            //These should be destroyed
            AssertInTrash(az, new Card[] { GetCard("CryoChamber"), GetCard("FocusedApertures"), GetCard("ColdSnap"), GetCard("CoolantBlast") });
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

            //set hp of heads
            SetHitPoints(inferno, 20);
            SetHitPoints(storm, 15);
            SetHitPoints(winter, 25);

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
            AddCannotDealNextDamageTrigger(tiamat, inferno);
            AddCannotDealNextDamageTrigger(tiamat, storm);
            PlayCard(GetCard("AcidBreath"));
            QuickHPCheck(-3, -3, 0);
        }

        [Test()]
        public void TestAlteration()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Setup top of decks
            Card ward = PutOnDeck("AncientWard");
            Card traffic = PutOnDeck("TrafficPileup");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PlayCard(tiamat, "ElementalForm");

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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card fire = GetCard("ElementOfFire");

            //put the other two copies of element of fire in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfFire" && c != fire).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat,fire);
            AddCannotDealNextDamageTrigger(tiamat, storm);
            AddCannotDealNextDamageTrigger(tiamat, winter);
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementOfFireDamage2InTrash_DynamicValues()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ice = GetCard("ElementOfIce");

            //put the other two copies of element of fire in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfIce" && c != ice).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Card ice = GetCard("ElementOfIce");

            //put the other two copies of element of fire in the trash
            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => c.Identifier == "ElementOfIce" && c != ice).Take(2)).ToList();
            PutInTrash(spellCards);

            //Inferno deals 2+X damage to each hero where X is the number of Element of Fires in trash
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
            SetupGameController("Cauldron.Tiamat", "Parse", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Parse", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Storm deals 2+X damage to each hero where X is the number of Element of Lightning in trash
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
            //Storm deals 2+X damage to each hero where X is the number of Element of Lightning in trash
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
        public void TestElementOfLightningCanDrawCardsNextTurn()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            DrawCard(bunker);
            //The hero with most cards in hand cannot draw cards until start of next villain turn
            PlayCard(tiamat, GetCard("ElementOfLightning"));
            GoToDrawCardPhase(bunker);
            GoToDrawCardPhase(bunker);
            AssertCanPerformPhaseAction();
        }

        [Test()]
        public void TestInfernoIncapEffect0InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            SetupIncap(legacy, inferno);
            //Inferno increases Spell damage dealt by heads when Incapped for number of Element of Fires in trash
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
            //Inferno increases Spell damage dealt by heads when Incapped for number of Element of Fires in trash
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
            //Storm increases Spell damage dealt by heads when Incapped for number of Element of Lightning in trash
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfIce"));
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestStormIncapEffect2InTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(GetCard("ElementOfLightning", 1));
            PutInTrash(GetCard("ElementOfLightning", 2));
            SetupIncap(legacy, storm);
            //Storm increases Spell damage dealt by heads when Incapped for number of Element of Lightning in trash
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
            //Winter increases Spell damage dealt by heads when Incapped for number of Element of Ice in trash
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
            //Winter increases Spell damage dealt by heads when Incapped for number of Element of Ice in trash
            QuickHPStorage(legacy, bunker, haka);
            PlayCard(tiamat, GetCard("ElementOfFire"));
            QuickHPCheck(-4, -4, -4);
        }

        [Test()]
        public void TestElementalFrenzyPutUnder()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card ward = PutInTrash("AncientWard");

            List<Card> spellCards = (tiamat.TurnTaker.Deck.Cards.Where(c => this.IsSpell(c)).Take(6)).ToList();
            PutInTrash(spellCards);

            //Elemental Frenzy puts all spells in trash face down under it
            Card frenzy = PlayCard("ElementalFrenzy");
            AssertNumberOfCardsUnderCard(frenzy, 6);
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertInTrash(ward);
            foreach(Card card in spellCards)
            {
                Assert.IsTrue(!card.IsFaceUp, $"{card.Title} was face up.");
                AssertUnderCard(frenzy, card);
            }
            
        }

        [Test()]
        public void TestElementalFrenzyPutUnder_OnlyOwnTrash()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            //End of Turn effects of heads deal 2 to Haka and 1 to Legacy
            //The spell played deals 2 to each hero
            QuickHPCheck(-3, -2, -4);

            //check the trash and under card values after end of turn
            AssertNumberOfCardsInTrash(tiamat, 1);
            AssertNumberOfCardsUnderCard(frenzy, 1);
        }

        [Test()]
        public void TestElementalFrenzyDestroySelf()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            Card frenzy = GetCard("ElementalFrenzy");

            //Elemental Frenzy destroys self when no cards underneath it
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
            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            QuickHPStorage(inferno);
            PlayCard(tiamat, "HealingMagic");
            QuickHPCheck(3);
            //The top card of the villain deck is played
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
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            PutInTrash(tiamat, new Card[] { GetCard("ElementalFrenzy"), GetCard("ElementalFrenzy", 1) });

            //Mana Charge shuffles all copies of Elemental Frenzy in the trash to the deck
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
            //Reptilian Aspect heals all heads for H - 2 at end of turn
            PlayCard(tiamat, "ReptilianAspect");
            GoToEndOfTurn(tiamat);
            QuickHPCheck(1, 1, 1, 0);
        }

        [Test()]
        public void TestSkyBreaker()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();
            //Sky Breaker deals H + 2 to all heroes
            QuickHPStorage(new Card[] { inferno, legacy.CharacterCard, bunker.CharacterCard, haka.CharacterCard });
            PlayCard(tiamat, "SkyBreaker");
            QuickHPCheck(0, -5, -5, -5);
        }


        [Test()]
        [Sequential]
        public void DecklistTest_OneShot_IsOneShot([Values("ElementOfFire", "ElementOfIce", "ElementOfLightning", "AcidBreath", "ManaCharge", "SkyBreaker", "HealingMagic", "Alteration")] string oneshot)
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tiamat);

            Card card = PlayCard(oneshot);
            AssertInTrash(tiamat, card);
            AssertCardHasKeyword(card, "one-shot", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Spell_IsSpell([Values("ElementOfFire", "ElementOfIce", "ElementOfLightning")] string spell)
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(tiamat);

            Card card = PlayCard(spell);
            AssertCardHasKeyword(card, "spell", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Ongoing_IsOngoing([Values("ElementalForm", "AncientWard", "DragonsWrath", "ReptilianAspect", "ElementalFrenzy")] string ongoing)
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            //put a spell in trash so ElementalFrenzy doesn't nuke itself
            PutInTrash("ElementOfFire");
            GoToPlayCardPhase(tiamat);

            Card card = PlayCard(ongoing);
            AssertInPlayArea(tiamat, card);
            AssertCardHasKeyword(card, "ongoing", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Head_IsHead([Values("InfernoTiamatCharacter", "StormTiamatCharacter", "WinterTiamatCharacter")] string head)
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            Card card = GetCard(head);
            AssertInPlayArea(tiamat, card);
            AssertCardHasKeyword(card, "head", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Villain_IsVillain([Values("InfernoTiamatCharacter", "StormTiamatCharacter", "WinterTiamatCharacter")] string villain)
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            Card card = GetCard(villain);
            AssertInPlayArea(tiamat, card);
            AssertCardHasKeyword(card, "villain", false);
        }
    }
}

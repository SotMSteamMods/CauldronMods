using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Titan;

namespace CauldronTests
{
    [TestFixture()]
    public class TitanTests : BaseTest
    {

        protected HeroTurnTakerController titan { get { return FindHero("Titan"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(titan.CharacterCard, 1);
            DealDamage(villain, titan, 2, DamageType.Melee);
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        [Test()]
        [Order(0)]
        public void TestTitanLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(titan);
            Assert.IsInstanceOf(typeof(TitanCharacterCardController), titan.CharacterCardController);

            foreach (var card in titan.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(32, titan.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestTitanDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("ongoing", new[]
            {
                "CombatPragmatism",
                "Immolate",
                "PaybackTime",
                "StubbornGoliath",
                "Titanform",
                "Unbreakable",
                "VulcansJudgment"
            });

            AssertHasKeyword("equipment", new[]
            {
                "TheChaplain"
            });

            AssertHasKeyword("limited", new[]
            {
                "StubbornGoliath",
                "TheChaplain",
                "PaybackTime"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "ForbiddenArchives",
                "HaplessShield",
                "JuggernautStrike",
                "Misbehavior",
                "MoltenVeins",
                "Ms5DemolitionCharge",
                "ObsidianGrasp",
                "Reversal"
            });
        }

        [Test()]
        public void TestTitanInnatePower()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //{Titan} deals 1 target 2 infernal damage.
            QuickHPStorage(apostate);
            UsePower(titan);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestTitanIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, bunker.TurnTaker, scholar.TurnTaker };

            //One player may use a power now.s
            SetHitPoints(scholar, 17);
            QuickHandStorage(bunker);
            QuickHPStorage(apostate, scholar);
            UseIncapacitatedAbility(titan, 0);
            UseIncapacitatedAbility(titan, 0);
            UseIncapacitatedAbility(titan, 0);
            QuickHandCheck(1);
            QuickHPCheck(-2, 1);
        }

        [Test()]
        public void TestTitanIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One player may play a card now.
            Card moko = PutInHand("TaMoko");
            DecisionSelectCard = moko;
            UseIncapacitatedAbility(titan, 1);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestTitanIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            UseIncapacitatedAbility(titan, 2);

            //Increase the next damage dealt by a hero target by 1.
            QuickHPStorage(apostate);
            DealDamage(apostate, apostate, 2, DamageType.Melee);
            QuickHPCheck(-2);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestCombatPragmatismYes()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card prag = PlayCard("CombatPragmatism");
            DecisionYesNo = true;

            //When a non-hero card enters play, you may destroy this card. If you do, you may use a power now.
            QuickHPStorage(apostate);
            PlayCard("ImpPilferer");
            QuickHPCheck(-2);
            AssertInTrash(prag);
        }

        [Test()]
        public void TestCombatPragmatismNo()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //When a non-hero card enters play, you may destroy this card. If you do, you may use a power now.
            Card prag = PlayCard("CombatPragmatism");
            DecisionYesNo = false;

            QuickHPStorage(apostate);
            PlayCard("ImpPilferer");
            QuickHPCheck(0);
            AssertIsInPlay(prag);
        }

        [Test()]
        public void TestCombatPragmatismPower()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(titan, 17);
            Card prag = PlayCard("CombatPragmatism");

            //{Titan} regains 3HP.
            QuickHPStorage(titan);
            UsePower(prag);
            QuickHPCheck(3);
        }

        [Test()]
        public void TestForbiddenArchivesAllDraw()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card arch = PutInHand("ForbiddenArchives");
            DecisionsYesNo = new Boolean[] { true, true, true, true };

            //Each player may draw 2 cards now.
            QuickHandStorage(titan, haka, bunker, scholar);
            QuickHPStorage(titan);
            PlayCard(arch);
            QuickHandCheck(1, 2, 2, 2);
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestForbiddenArchives2Draw()
        {
            SetupGameController("Omnitron", "Haka", "Bunker", "TheScholar", "Cauldron.Titan", "Megalopolis");
            StartGame();

            Card arch = PutInHand("ForbiddenArchives");
            DecisionsYesNo = new Boolean[] { true, true, false, false };

            //Each player may draw 2 cards now.
            QuickHandStorage(titan, haka, bunker, scholar);
            QuickHPStorage(titan);
            PlayCard(arch);
            QuickHandCheck(-1, 2, 2, 0);
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestForbiddenArchives0Draw()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card arch = PutInHand("ForbiddenArchives");
            DecisionYesNo = false;

            //Each player may draw 2 cards now.
            QuickHandStorage(titan, haka, bunker, scholar);
            QuickHPStorage(titan);
            PlayCard(arch);
            QuickHandCheck(-1, 0, 0, 0);
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            QuickHPCheckZero();
        }

        [Test()]
        public void TestHaplessShield()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("PlummetingMonorail");
            PlayCard("ElectroPulseExplosive");

            QuickHPStorage(omnitron);
            PlayCard("HaplessShield");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestImmolate()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("S83AssaultDrone");
            Card imm = PlayCard("Immolate");


            //Play this card next to a target.
            AssertNextToCard(imm, omnitron.CharacterCard);

            //The first time that target deals damage each turn, it deals itself 1 fire damage.
            QuickHPStorage(omnitron);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            QuickHPCheck(-1);
            //Only first time
            QuickHPStorage(omnitron);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //If that target leaves play, destroy this card.
            DestroyCard(omnitron.CharacterCard);
            AssertInTrash(imm);
        }

        [Test()]
        public void TestJuggernautStrike()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card drone0 = PlayCard("S83AssaultDrone", 0);
            Card drone1 = PlayCard("S83AssaultDrone", 1);
            Card tform = PutInHand("Titanform");
            DecisionsYesNo = new Boolean[] { true, false, false, false, };


            //If Titanform is in your hand you may play it now.
            //{Titan} deals 1 target 4 infernal damage and each other target from that deck 1 projectile damage.
            QuickHPStorage(omnitron.CharacterCard, drone0, drone1, haka.CharacterCard);
            PlayCard("JuggernautStrike");
            AssertIsInPlay(tform);
            QuickHPCheck(-4, -1, -1, 0);
        }

        [Test()]
        public void TestTitanform()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck("Terraforming");

            Card tform = PlayCard("Titanform");

            //Whenever {Titan} is dealt damage by another target, reduce damage dealt to {Titan} by 1 until the start of your next turn.
            QuickHPStorage(titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Second time damage dealt to Titan
            QuickHPStorage(titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //First time in a new turn
            GoToStartOfTurn(titan);
            QuickHPStorage(titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //When {Titan} would deal damage, you may destroy this card to increase that damage by 2.
            //saying no - not destroyed no increase
            DecisionYesNo = false;
            QuickHPStorage(omnitron);
            DealDamage(titan, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-2);
            AssertIsInPlay(tform);
            //selecting yes this time
            DecisionYesNo = true;
            QuickHPStorage(omnitron);
            DealDamage(titan, omnitron, 3, DamageType.Melee);
            QuickHPCheck(-5);
            AssertInTrash(tform);
        }
    }
}

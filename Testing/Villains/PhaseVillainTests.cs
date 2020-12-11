using NUnit.Framework;
using Cauldron.PhaseVillain;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class PhaseVillainTests : BaseTest
    {
        protected TurnTakerController phase { get { return FindVillain("PhaseVillain"); } }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private void DestroyWall()
        {
            Card wall = GetCardInPlay("ReinforcedWall");
            SetHitPoints(wall, 0);
            DestroyCard(wall);
        }

        [Test()]
        public void TestPhaseLoad()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(phase);
            Assert.IsInstanceOf(typeof(PhaseVillainCharacterCardController), phase.CharacterCardController);

            foreach (Card card in phase.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(30, phase.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestPhaseVillainDecklist()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("device", new string[] { "DensityRegulator", "DistortionGrenade", "DistortionNet" });

            AssertHasKeyword("obstacle", new string[] { "BlockedSightline", "PrecariousRubble", "ReinforcedWall", "VaultDoor" });

            AssertHasKeyword("one-shot", new string[] { "FrequencyShift", "NowhereToGoButDown" });

            AssertHasKeyword("ongoing", new string[] { "AlmostGotHer", "AroundTheCorner", "InsubstantialMatador", "ResidualDesynchronization", "UnimpededProgress" });

            AssertMaximumHitPoints(GetCard("BlockedSightline"), 5);
            AssertMaximumHitPoints(GetCard("DensityRegulator"), 10);
            AssertMaximumHitPoints(GetCard("DistortionGrenade"), 3);
            AssertMaximumHitPoints(GetCard("DistortionNet"), 4);
            AssertMaximumHitPoints(GetCard("PrecariousRubble"), 3);
            AssertMaximumHitPoints(GetCard("ReinforcedWall"), 10);
            AssertMaximumHitPoints(GetCard("VaultDoor"), 6);
        }

        [Test()]
        public void TestPhaseStartGame()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertIsInPlay("ReinforcedWall");
        }

        [Test()]
        public void TestPhaseFront()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Phase is immune to damage dealt by environment cards.
            Card popo = PlayCard("PoliceBackup");
            QuickHPStorage(phase);
            DealDamage(popo, phase, 2, DamageType.Melee);
            QuickHPCheck(0);

            //At the end of the villain turn, play the top card of the villain deck.
            Card vault = PutOnDeck("VaultDoor");
            GoToEndOfTurn(phase);
            AssertIsInPlay(vault);

            //At the start of the villain turn, if there are 3 or more obstacles in play, flip {PhaseVillain}'s villain character cards.
            PlayCard("BlockedSightline");
            GoToStartOfTurn(phase);
            AssertFlipped(phase);
        }

        [Test()]
        public void TestPhaseFrontIncreaseDamage()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt by {PhaseVillain} by 1 for each obstacle that has been removed from the game.
            //0 out of game
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //1 out of game
            MoveCard(phase, "VaultDoor", phase.TurnTaker.OutOfGame);
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //2 out of game
            Card wall = GetCard("ReinforcedWall");
            SetHitPoints(wall, 0);
            MoveCard(phase, wall, phase.TurnTaker.OutOfGame);
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-4);

            //3 out of game
            MoveCard(phase, "BlockedSightline", phase.TurnTaker.OutOfGame);
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-5);

            //4 out of game
            MoveCard(phase, "PrecariousRubble", phase.TurnTaker.OutOfGame);
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestPhaseFrontAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis" }, true);
            StartGame();

            PutOnDeck("AroundTheCorner");
            DestroyWall();

            //When {Phase} is damaged, she becomes immune to damage until the end of the turn.
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //After taking damage is immune
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Next turn not immune
            GoToStartOfTurn(haka);
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestPhaseBack()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mat = PlayCard("InsubstantialMatador");
            Card door = GetCard("VaultDoor", 0);
            Card rub = GetCard("PrecariousRubble");
            Card door2 = GetCard("VaultDoor", 1);
            PutOnDeck(phase, door2);
            PutOnDeck(phase, door);
            PutOnDeck(phase, rub);
            Card wall = GetCardInPlay("ReinforcedWall");
            //When {Phase} flips to this side, destroy the obstacle with the lowest HP and remove it from the game. If the card Insubstantial Matador is in play, destroy it.
            GoToStartOfTurn(phase);
            AssertOutOfGame(rub);
            AssertInTrash(mat);
            AssertIsInPlay(new Card[] { wall, door });

            //At the end of the villain turn, {Phase} deals each hero target {H} radiant damage. Then, flip {Phase}'s villain character cards.
            QuickHPStorage(haka, bunker, scholar);
            GoToEndOfTurn(phase);
            QuickHPCheck(-3, -3, -3);
            AssertNotFlipped(phase);
        }

        [Test()]
        public void TestPhaseBackAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis" }, true);
            StartGame();

            //When {PhaseVillain} flips to this side, destroy {H - 2} hero ongoing cards.
            Card moko = PlayCard("TaMoko");
            Card iron = PlayCard("FleshToIron");

            FlipCard(phase);

            AssertIsInPlay(iron);
            AssertInTrash(moko);
        }

        [Test()]
        public void TestAlmostGotHerDamageEffects()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("AlmostGotHer");
            PlayCard("TaMoko");
            Card wall = GetCard("ReinforcedWall");
            Card block = PutOnDeck("BlockedSightline");

            //Increase damage dealt to Obstacles by 1 by the first hero target damaged by {PhaseVillain} each round.
            //no one's beem hit yet
            QuickHPStorage(wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Damage dealt by {PhaseVillain} is irreducible.
            QuickHPStorage(haka);
            DealDamage(phase, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Haka was first hit this round
            QuickHPStorage(wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(-3);


            DealDamage(phase, bunker, 2, DamageType.Melee);
            //Bunker was not first hit
            QuickHPStorage(wall);
            DealDamage(bunker, wall, 2, DamageType.Melee);
            QuickHPCheck(-2);

            SetHitPoints(wall, 0);
            DestroyCard(wall);

            GoToStartOfTurn(env);
            //haka is still first
            QuickHPStorage(block);
            DealDamage(haka, block, 1, DamageType.Melee);
            QuickHPCheck(-2);
            //bunker still not first
            QuickHPStorage(block);
            DealDamage(bunker, block, 1, DamageType.Melee);
            QuickHPCheck(-1);

            //New round
            GoToStartOfTurn(phase);
            QuickHPStorage(block);
            DealDamage(haka, block, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAlmostGotHerNotDestroySelf()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card almost = PlayCard("AlmostGotHer");
            PutOnDeck("VaultDoor");
            //At the start of the villain turn, if there are 1 or 0 Obstacles in play, each player must discard a card. Then, this card is destroyed.
            GoToStartOfTurn(phase);
            AssertIsInPlay(almost);
        }

        [Test()]
        public void TestAlmostGotHerDestroySelf()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card almost = PutOnDeck("AlmostGotHer");
            //At the start of the villain turn, if there are 1 or 0 Obstacles in play, each player must discard a card. Then, this card is destroyed.
            QuickHandStorage(haka, scholar, bunker);
            GoToStartOfTurn(phase);
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(almost);
        }

        [Test()]
        public void TestAroundTheCorner()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //The first time an Obstacle is destroyed each turn, {PhaseVillain} deals each hero target 2 radiant damage.
            PlayCard("AroundTheCorner");
            QuickHPStorage(haka, bunker, scholar);
            DestroyWall();
            QuickHPCheck(-2, -2, -2);
            //only first time
            Card door = PlayCard("VaultDoor");
            QuickHPStorage(haka, bunker, scholar);
            DestroyCard(door);
            QuickHPCheck(0, 0, 0);

            //New turn new damage
            PutOnDeck("ReinforcedWall");
            GoToNextTurn();
            QuickHPStorage(haka, bunker, scholar);
            DestroyWall();
            QuickHPCheck(-2, -2, -2);
        }

        [Test()]
        public void TestBlockedSightline()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            Card block = PlayCard("BlockedSightline");
            DestroyWall();

            //Whenever {PhaseVillain} would be dealt damage, reduce that damage to 0.
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheckZero();

            //Reduces damage to 0, irreducible means we can deal damage
            PlayCard("RevealTheFlaws");
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheck(-2);

            PlayCard("TaMoko");
            //When this card is destroyed, {PhaseVillain} deals the 2 hero targets with the highest HP {H} irreducible radiant damage each.
            QuickHPStorage(haka, parse, scholar);
            DestroyCard(block);
            QuickHPCheck(-3, 0, -3);
        }

        [Test()]
        public void TestDistortionGrenade()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            //Increase damage dealt to non-villain targets by 1.
            //When this card enters play, it deals the {H - 1} hero targets with the lowest HP 2 lightning damage each.
            QuickHPStorage(haka, parse, scholar);
            PlayCard("DistortionGrenade");
            QuickHPCheck(0, -3, -3);

            Card wall = GetCard("ReinforcedWall");
            //Only non-villains take more damge
            QuickHPStorage(wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //environment targets are non-villain
            Card rail = PlayCard("PlummetingMonorail");
        }

        [Test()]
        public void TestDistortionNet()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            //When this card enters play, place it next to the hero with the highest HP.
            Card net = PlayCard("DistortionNet");
            AssertNextToCard(net, haka.CharacterCard);

            //Reduce damage dealt by that hero by 2.
            QuickHPStorage(scholar);
            DealDamage(haka, scholar, 3, DamageType.Melee);
            QuickHPCheck(-1);

            //At the start of that hero's turn, this card deals them {H} toxic damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(haka);
            QuickHPCheck(-3);
        }
    }
}

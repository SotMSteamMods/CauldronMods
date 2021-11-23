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
    class PhaseVillainTests : CauldronBaseTest
    {

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
        public void TestPhaseChallenge()
        {
            SetupGameController(new string[] { "Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis" }, challenge: true);
            StartGame();

            //Whenever an Obstacle is destroyed, Obstacles become immune to damage until the end of the turn.
            Card wall = GetCardInPlay("ReinforcedWall");
            Card sightline = PlayCard("BlockedSightline");
            Card vault = PlayCard("VaultDoor");


            DealDamage(haka.CharacterCard, wall, 15, DamageType.Fire, isIrreducible: true);

            //now all obstacles should be immune to damage

            QuickHPStorage(sightline, vault);
            DealDamage(haka.CharacterCard, sightline, 15, DamageType.Melee, isIrreducible: true);
            DealDamage(haka.CharacterCard, vault, 15, DamageType.Melee, isIrreducible: true);
            QuickHPCheckZero();

            //should expire by next turn

            GoToNextTurn();

            QuickHPUpdate();
            DealDamage(haka.CharacterCard, sightline, 4, DamageType.Melee, isIrreducible: true);
            DealDamage(haka.CharacterCard, vault, 4, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(-4,-4);

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
        public void TestPhaseFlipNotBreakTriggers()
        {
            SetupGameController(new string[] { "Cauldron.PhaseVillain", "Cauldron.Impact", "Legacy", "VoidGuardWrithe", "OmnitronX", "Megalopolis" });
            StartGame();

            MoveCard(phase, "VaultDoor", phase.TurnTaker.OutOfGame);
            PlayCard("BlockedSightline");

            QuickHPStorage(impact, legacy);
            FlipCard(phase);

            //H damage each, plus nemesis bonus from Impact. Should not have the "boost damage" effect.
            QuickHPCheck(-5, -4);

            PlayCard("TakeDown");
            GoToStartOfTurn(impact);

            QuickHPUpdate();
            DealDamage(phase, impact, 1, DamageType.Melee);
            DealDamage(phase, legacy, 1, DamageType.Melee);
            //1 from base damage, 2 from RFG'd cards, Impact also takes 1 from nemesis
            QuickHPCheck(-4, -3);
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
        public void TestDensityRegulator()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();
            DestroyWall();

            //When this card enters play, place it next to the hero with the highest HP.
            Card reg = PlayCard("DensityRegulator");
            AssertInPlayArea(phase, reg);
            
            //Reduce damage dealt to phase by 1.
            QuickHPStorage(phase);
            DealDamage(haka, phase, 3, DamageType.Melee);
            QuickHPCheck(-2);

            SetHitPoints(legacy, 20);
            SetHitPoints(scholar, 20);

            //prevent Phase's EOT play from messing up test
            PlayCard("TakeDown");

            //deal all hero targets but the lowest 2 damage
            QuickHPStorage(haka, legacy, scholar);
            DecisionLowestHP = legacy.CharacterCard;
            DecisionAutoDecide = SelectionType.SelectTarget;
            GoToEndOfTurn(phase);
            QuickHPCheck(-2, 0, -2);
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
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck("AroundTheCorner");

            //When this card enters play, place it next to the hero with the highest HP.
            Card net = PlayCard("DistortionNet");
            AssertNextToCard(net, haka.CharacterCard);


            //Reduce damage dealt by that hero by 2.
            QuickHPStorage(scholar);
            DealDamage(haka, scholar, 3, DamageType.Melee);
            QuickHPCheck(-1);

            //prevent Phase's EOT play from messing up test
            PlayCard("TakeDown");

            //At the start of that hero's turn, this card deals them {H} toxic damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(haka);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestFrequencyShift1Obstacle()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(phase, 17);
            Card flak = PlayCard("FlakCannon");
            Card iron = PlayCard("FleshToIron");
            Card punish = PlayCard("PunishTheWeak");
            Card moko = PlayCard("TaMoko");
            Card mere = PlayCard("Mere");

            //{PhaseVillain} regains X HP, where X is the number of Obstacle cards in play.
            //{PhaseVillain} deals the hero target with the highest HP {H} irreducible radiant damage and destroys 1 ongoing and 1 equipment card belonging to that hero.
            QuickHPStorage(phase, haka);
            PlayCard("FrequencyShift");
            QuickHPCheck(1, -3);
            AssertInTrash(new Card[] { mere, punish });
            AssertIsInPlay(new Card[] { flak, iron, moko });
        }

        [Test()]
        public void TestFrequencyShift3Obstacle()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(phase, 17);
            PlayCards((Card c) => c.DoKeywordsContain("obstacle"), 2);

            //{PhaseVillain} regains X HP, where X is the number of Obstacle cards in play.
            QuickHPStorage(phase);
            PlayCard("FrequencyShift");
            QuickHPCheck(3);
        }

        [Test()]
        public void TestInsubstantialMatador()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            DestroyWall();
            PutOnDeck("AroundTheCorner");
            var mat = PutOnDeck("InsubstantialMatador");

            //When this card enters play, {PhaseVillain} deals the hero target with the second lowest HP {H - 1} radiant damage.
            GoToPlayCardPhase(phase);
            QuickHPStorage(scholar);
            PlayTopCard(phase);
            QuickHPCheck(-2);

            //At the end of each hero's turn, if that hero dealt {PhaseVillain} no damage, that hero deals themselves 1 irreducible melee damage.
            GoToStartOfTurn(haka);
            GoToPlayCardPhase(haka);
            PlayCard("TaMoko");
            QuickHPStorage(haka);
            GoToEndOfTurn(haka);
            QuickHPCheck(-1);

            //Did deal damage to Phase
            GoToStartOfTurn(parse);
            QuickHPStorage(parse);
            DealDamage(parse, phase, 2, DamageType.Melee);
            GoToEndOfTurn(parse);
            QuickHPCheckZero();
        }

        [Test()]
        public void TestInsubstantialMatador_Sentinels()
        {
            SetupGameController("Cauldron.PhaseVillain", "TheSentinels", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            DestroyWall();
            PutOnDeck("AroundTheCorner");
            var mat = PutOnDeck("InsubstantialMatador");

            //At the end of each hero's turn, if that hero dealt {PhaseVillain} no damage, that hero deals themselves 1 irreducible melee damage.
            GoToStartOfTurn(sentinels);
            DecisionSelectCards = new Card[] { mainstay };
            QuickHPStorage(writhe, mainstay, medico, idealist);
            GoToEndOfTurn(sentinels);
            QuickHPCheck(0,-1,0,0);
        }

        [Test()]
        public void TestNowhereToGoButDown()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            //Reveal cards from the top of the villain deck until {H - 1} Obstacle cards are revealed. Shuffle the other revealed cards back into the villain deck.
            Card mat = GetCard("InsubstantialMatador");
            Card door0 = GetCard("VaultDoor", 0);
            Card door1 = GetCard("VaultDoor", 1);
            PutOnDeck(phase, new Card[] { door0, mat, door1 });
            //Put the Obstacles into play in the order they were revealed.
            PlayCard("NowhereToGoButDown");
            AssertIsInPlay(new Card[] { door0, door1 });
            AssertInDeck(mat);
            //Just Nowhere to go but down
            AssertNumberOfCardsInTrash(phase, 1);
        }

        [Test()]
        public void TestPrecariousRubble()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            DestroyWall();
            Card door = PutOnDeck("VaultDoor");
            //When this card enters play, it deals the hero target with the lowest HP {H - 1} projectile damage. 
            QuickHPStorage(parse);
            PlayCard("PrecariousRubble");
            QuickHPCheck(-2);
            //Then, play the top card of the villain deck.
            AssertIsInPlay(door);
            //{PhaseVillain} is immune to damage.
            DestroyCard(door);
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheckZero();
        }

        [Test()]
        public void TestReinforcedWall()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Parse", "TheScholar", "Megalopolis");
            StartGame();

            Card wall = GetCardInPlay("ReinforcedWall");

            //{PhaseVillain} is immune to damage.
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheck(0);

            //This card is indestructible while it has more than 0 HP.
            SetHitPoints(wall, 1);
            DestroyCard(wall);
            AssertIsInPlay(wall);

            //Other Obstacles that enter play are immune to damage while this card is in play.
            Card door = PlayCard("VaultDoor");
            QuickHPStorage(door);
            DealDamage(haka, door, 3, DamageType.Melee);
            QuickHPCheck(0);

            //This card is indestructible while it has more than 0 HP.
            //0 hp = destroyable
            SetHitPoints(wall, 0);
            DestroyCard(wall);
            AssertInTrash(wall);

            //Other Obstacles that enter play are immune to damage while this card is in play.
            //wall renentered play after door already was in play
            PlayCard(wall);
            QuickHPStorage(door);
            DealDamage(haka, door, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestResidualDesynchronization()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            Card wall = GetCardInPlay("ReinforcedWall");
            Card door = PlayCard("VaultDoor");
            PlayCard("ResidualDesynchronization");

            //Reduce damage dealt to Obstacles by 1.
            //The first time a villain target is dealt damage each turn, it deals the source of that damage 2 energy damage.
            QuickHPStorage(haka.CharacterCard, wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(-2, -1);
            //Only first time is there retaliation
            QuickHPStorage(haka.CharacterCard, wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(0, -1);
            DestroyWall();
            //First time each turn - not per target
            QuickHPStorage(haka.CharacterCard, door);
            DealDamage(haka, door, 4, DamageType.Melee);
            QuickHPCheck(0, -3);
            PlayCard(wall);

            //prevent Phase's EOT play from messing up test
            PlayCard("TakeDown");

            //New turn
            GoToStartOfTurn(haka);
            QuickHPStorage(haka.CharacterCard, wall);
            DealDamage(haka, wall, 2, DamageType.Melee);
            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestResidualDesynchronization_DestroysTarget()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            Card wall = GetCardInPlay("ReinforcedWall");
            PlayCard("ResidualDesynchronization");

            //Reduce damage dealt to Obstacles by 1.
            //The first time a villain target is dealt damage each turn, it deals the source of that damage 2 energy damage.
            //if the target is destroyed no damage should be dealt
            QuickHPStorage(haka.CharacterCard);
            DealDamage(haka.CharacterCard, wall, 20, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(0);
            AssertInTrash(wall);
        }


        [Test()]
        public void TestResidualDesynchronization_NoDamageIfTargetDestroyed()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Megalopolis");
            StartGame();

            Card wall = GetCardInPlay("ReinforcedWall");
            PlayCard("ResidualDesynchronization");

            //The first time a villain target is dealt damage each turn, it deals the source of that damage 2 energy damage.
            //Damage fizzles since wall is destroyed and can't deal damage
            QuickHPStorage(haka.CharacterCard);
            DealDamage(haka, wall, 99, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestUnimpededProgress()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Parse", "Megalopolis");
            StartGame();

            Card prog = PlayCard("UnimpededProgress");
            Card mat = PutOnDeck("InsubstantialMatador");
            Card door = PutOnDeck("VaultDoor");

            //At the end of the villain turn, play the top card of the villain deck.
            GoToEndOfTurn(phase);
            //Phase Character Card plays a card at end of turn
            AssertIsInPlay(new Card[] { mat, door });

            //Destroy this card when {PhaseVillain} is dealt {H * 2} or more damage in 1 round.
            //H * 2 = 8
            DestroyCards(new Card[] { mat, door });
            DestroyWall();
            DealDamage(haka, phase, 6, DamageType.Melee);
            //6 total
            AssertIsInPlay(prog);

            GoToEndOfTurn(legacy);
            DealDamage(phase, phase, 1, DamageType.Melee);
            //7 total
            AssertIsInPlay(prog);

            PlayCard("TakeDown");

            GoToStartOfTurn(phase);
            DealDamage(haka, phase, 1, DamageType.Melee);
            //1 total
            AssertIsInPlay(prog);

            GoToStartOfTurn(haka);
            DealDamage(phase, phase, 1, DamageType.Melee);
            //2 total
            AssertIsInPlay(prog);

            GoToStartOfTurn(env);
            DealDamage(haka, phase, 6, DamageType.Melee);
            //8 total
            AssertInTrash(prog);
        }

        [Test()]
        public void TestVaultDoor()
        {
            SetupGameController("Cauldron.PhaseVillain", "Haka", "Legacy", "TheScholar", "Parse", "Megalopolis");
            StartGame();

            DestroyWall();
            Card door = PlayCard("VaultDoor");

            //{PhaseVillain} is immune to damage.
            QuickHPStorage(phase);
            DealDamage(haka, phase, 2, DamageType.Melee);
            QuickHPCheckZero();

            //When this card would be dealt 2 or less damage, prevent that damage.
            QuickHPStorage(door);
            DealDamage(haka, door, 2, DamageType.Melee);
            QuickHPCheckZero();
            //more than 2
            QuickHPStorage(door);
            DealDamage(haka, door, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}

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
    public class TitanTests : CauldronBaseTest
    {

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(titan.CharacterCard, 1);
            DealDamage(villain, titan, 2, DamageType.Melee);
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
        public void TestTitanInnatePowerWithCutoutSwapVariants([Values("/FutureTitanCharacter", "/MinistryOfStrategicScienceTitanCharacter", "/OniTitanCharacter")] string promoVersion)
        {
            SetupGameController("Apostate", "Cauldron.Titan" + promoVersion, "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card form = MoveCard(titan, "Titanform", titan.TurnTaker.OutOfGame);
            DiscardAllCards(titan);
            PutOnDeck("HaplessShield");
            UsePower(titan);
            AssertNumberOfUsablePowers(titan, 0);
            PlayCard("Titanform");
            AssertNumberOfUsablePowers(titan, 0);


            DecisionYesNo = false;
            GoToStartOfTurn(titan);
            AssertNumberOfUsablePowers(titan, 1);
            DiscardAllCards(titan);
            PutOnDeck("HaplessShield");
            UsePower(titan);
            AssertNumberOfUsablePowers(titan, 0);
            MoveCard(titan, "Titanform", titan.TurnTaker.OutOfGame);
            AssertNumberOfUsablePowers(titan, 0);
        }
        [Test()]
        public void TestTitanInnatePowerWithCutoutSwap()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            UsePower(titan);
            AssertNumberOfUsablePowers(titan, 0);
            PlayCard("Titanform");
            AssertNumberOfUsablePowers(titan, 0);


            DecisionYesNo = false;
            GoToStartOfTurn(titan);
            AssertNumberOfUsablePowers(titan, 1);
            UsePower(titan);
            AssertNumberOfUsablePowers(titan, 0);
            MoveCard(titan, "Titanform", titan.TurnTaker.OutOfGame);
            AssertNumberOfUsablePowers(titan, 0);
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
        public void TestCombatPragmatismNotWhenDestroyed()
        {
            SetupGameController("Apostate", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card prag = PlayCard("CombatPragmatism");
            DecisionYesNo = true;
            DecisionSelectPower = titan.CharacterCard;
            DecisionSelectTarget = apostate.CharacterCard;
            //When a non-hero card enters play, you may destroy this card. If you do, you may use a power now.
            QuickHPStorage(apostate);
            DestroyCard(prag);
            //nothing should happen
            QuickHPCheck(0);
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
            DecisionsYesNo = new Boolean[] { true, true, true};

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
            DecisionsYesNo = new Boolean[] { true, true, false};

            //Each player may draw 2 cards now.
            QuickHandStorage(titan, haka, bunker, scholar);
            QuickHPStorage(titan);
            PlayCard(arch);
            QuickHandCheck(1, 2, 2, 0);
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
            QuickHandCheck(1, 0, 0, 0);
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            QuickHPCheckZero();
        }
        [Test]
        public void TestForbiddenArchivesNotRetainInfo()
        {
            SetupGameController("Omnitron", "Haka", "Bunker", "TheScholar", "Cauldron.Titan", "Megalopolis");
            StartGame();

            Card arch = PutInHand("ForbiddenArchives");
            DecisionsYesNo = new Boolean[] { true, true, false };

            //Each player may draw 2 cards now.
            QuickHandStorage(titan, haka, bunker, scholar);
            QuickHPStorage(titan);
            PlayCard(arch);
            QuickHandCheck(1, 2, 2, 0);
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            QuickHPCheck(-4);

            DecisionsYesNoIndex = 0;

            PlayCard(arch);
            QuickHandCheck(2, 2, 2, 0);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestHaplessShield()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("PlummetingMonorail");
            Card epe = PlayCard("ElectroPulseExplosive");

            QuickHPStorage(omnitron);
            PlayCard("HaplessShield");
            QuickHPCheck(-2);

            //ensure it is the nonhero targets doing damage
            DecisionSelectTarget = epe;
            PlayCard("OffensiveTransmutation");
            DecisionSelectTarget = null;
            PlayCard("HaplessShield");
            QuickHPCheck(-1);
        }
        [Test]
        public void TestHaplessShieldOnlyPicksTargets()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card train = PlayCard("PlummetingMonorail");
            Card epe = PlayCard("ElectroPulseExplosive");
            Card police = PlayCard("PoliceBackup");
            Card beam = PlayCard("InterpolationBeam");

            AssertNextDecisionChoices(included: new Card[] { train, epe, omnitron.CharacterCard }, notIncluded: new Card[] { police, beam });
            PlayCard("HaplessShield");

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

            //test it resets each turn
            PutOnDeck("S83AssaultDrone");
            GoToStartOfTurn(titan);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //If that target leaves play, destroy this card.
            DestroyCard(omnitron.CharacterCard);
            AssertInTrash(imm);
        }
        [Test()]
        public void TestImmolateNoDamageDealt()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Immolate");
            PlayCard("TaMoko");

            QuickHPStorage(omnitron, titan, haka);
            DealDamage(omnitron, haka, 1, DamageType.Melee);
            QuickHPCheckZero();
            DealDamage(omnitron, titan, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, 0);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -1);
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
        public void TestJuggernautStrikePickAnyDeck()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "CaptainCosmic", "Megalopolis");
            StartGame();

            Card drone0 = PlayCard("S83AssaultDrone", 0);
            Card drone1 = PlayCard("S83AssaultDrone", 1);

            //{Titan} deals 1 target 4 infernal damage and each other target from that deck 1 projectile damage.
            QuickHPStorage(omnitron.CharacterCard, drone0, drone1, haka.CharacterCard);
            PlayCard("JuggernautStrike");
            QuickHPCheck(-4, -1, -1, 0);

            //check it can pick non-villain decks

            Card traffic = PlayCard("TrafficPileup");
            Card targeting = PlayCard("TargetingInnocents");

            QuickHPStorage(omnitron.CharacterCard, drone0, traffic, targeting, haka.CharacterCard);

            DecisionSelectTarget = targeting;
            PlayCard("JuggernautStrike");
            QuickHPCheck(0, 0, -1, -4, 0);
            DecisionSelectTarget = null;

            Card weapon = PlayCard("CosmicWeapon");
            Card crest = PlayCard("CosmicCrest");
            DecisionSelectTargets = new Card[] { cosmic.CharacterCard, weapon };
            QuickHPStorage(cosmic.CharacterCard, weapon, crest, traffic, drone0);
            PlayCard("JuggernautStrike");
            QuickHPCheck(-4, -1, -1, 0, 0);
        }

        [Test()]
        public void TestTitanformDamageBoost()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck("Terraforming");

            Card tform = PlayCard("Titanform");

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


        [Test()]
        public void TestTitanformDR()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck("Terraforming");

            Card tform = PlayCard("Titanform");
            DecisionYesNo = false;

            //Whenever {Titan} is dealt damage by another target, reduce damage dealt to {Titan} by 1 until the start of your next turn.
            QuickHPStorage(titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Second time damage dealt to Titan
            QuickHPStorage(titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-1);
            //Third time - having been hit twice, he should have accumulated 2 DR
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Should wear off on his turn
            GoToStartOfTurn(titan);
            DealDamage(titan, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //And not trigger on self-damage
            DealDamage(titan, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);

            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2);

            DealDamage(titan, titan, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //Should keep his DR until his turn starts
            GoToStartOfTurn(haka);
            DealDamage(titan, titan, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestMisbehaviorReveal0()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutOnDeck("Titanform");
            Card chap = PutOnDeck("TheChaplain");
            Card prag = PutOnDeck("CombatPragmatism");
            SetHitPoints(titan, 17);

            QuickHPStorage(titan);
            PlayCard("Misbehavior");
            //Reveal up to 3 cards from the top of your deck. Put 1 of them into play or into your hand. Put the rest into your trash.
            AssertInDeck(new Card[] { tform, chap, prag });
            //{Titan} regains X HP, where X is 3 minus the number of cards revealed this way.
            QuickHPCheck(3);
        }

        [Test()]
        public void TestMisbehaviorReveal1ToHand()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutOnDeck("Titanform");
            Card chap = PutOnDeck("TheChaplain");
            Card prag = PutOnDeck("CombatPragmatism");
            SetHitPoints(titan, 17);

            DecisionSelectNumber = 1;
            DecisionMoveCardDestination = new MoveCardDestination(titan.HeroTurnTaker.Hand);

            QuickHPStorage(titan);
            PlayCard("Misbehavior");
            //Reveal up to 3 cards from the top of your deck. Put 1 of them into play or into your hand. Put the rest into your trash.
            AssertInDeck(new Card[] { tform, chap });
            AssertInHand(prag);
            //{Titan} regains X HP, where X is 3 minus the number of cards revealed this way.
            QuickHPCheck(2);
        }

        [Test()]
        public void TestMisbehaviorReveal2ToPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutOnDeck("Titanform");
            Card chap = PutOnDeck("TheChaplain");
            Card prag = PutOnDeck("CombatPragmatism");
            SetHitPoints(titan, 17);

            DecisionSelectNumber = 2;
            DecisionMoveCardDestination = new MoveCardDestination(titan.HeroTurnTaker.PlayArea);
            DecisionSelectCard = chap;

            QuickHPStorage(titan);
            PlayCard("Misbehavior");
            //Reveal up to 3 cards from the top of your deck. Put 1 of them into play or into your hand. Put the rest into your trash.
            AssertInDeck(tform);
            AssertInTrash(prag);
            AssertIsInPlay(chap);
            //{Titan} regains X HP, where X is 3 minus the number of cards revealed this way.
            QuickHPCheck(1);
        }

        [Test()]
        public void TestMisbehaviorReveal3ToPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutOnDeck("Titanform");
            Card chap = PutOnDeck("TheChaplain");
            Card prag = PutOnDeck("CombatPragmatism");
            SetHitPoints(titan, 17);

            DecisionSelectNumber = 3;
            DecisionMoveCardDestination = new MoveCardDestination(titan.HeroTurnTaker.PlayArea);
            DecisionSelectCard = chap;

            QuickHPStorage(titan);
            PlayCard("Misbehavior");
            //Reveal up to 3 cards from the top of your deck. Put 1 of them into play or into your hand. Put the rest into your trash.
            AssertInTrash(new Card[] { prag, tform });
            AssertIsInPlay(chap);
            //{Titan} regains X HP, where X is 3 minus the number of cards revealed this way.
            QuickHPCheck(0);
        }

        [Test()]
        public void TestMoltenViensSearchDeck()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = GetCard("Titanform");
            Card chap = PutInHand("TheChaplain");
            SetHitPoints(titan, 17);
            DecisionYesNo = true;
            DecisionSelectCard = chap;

            QuickHPStorage(titan);
            PlayCard("MoltenVeins");
            //{Titan} regains 2HP.
            QuickHPCheck(2);
            //You may search your deck and trash for a copy of the card Titanform and put it into your hand. If you searched your deck, shuffle it.
            AssertInHand(tform);
            //You may play a card.
            AssertIsInPlay(chap);
        }

        [Test()]
        public void TestMoltenViensSearchTrash()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutInTrash("Titanform");
            Card chap = PutInHand("TheChaplain");
            SetHitPoints(titan, 17);
            DecisionYesNo = true;
            DecisionSelectCard = chap;

            QuickHPStorage(titan);
            PlayCard("MoltenVeins");
            //{Titan} regains 2HP.
            QuickHPCheck(2);
            //You may search your deck and trash for a copy of the card Titanform and put it into your hand. If you searched your deck, shuffle it.
            AssertInHand(tform);
            //You may play a card.
            AssertIsInPlay(chap);
        }

        [Test()]
        public void TestMoltenViensDontSearchOrPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutInTrash("Titanform");
            Card veins = PutOnDeck("MoltenVeins");
            SetHitPoints(titan, 17);
            DecisionYesNo = false;
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            QuickHPStorage(titan);
            QuickHandStorage(titan);
            PlayCard(veins);
            //{Titan} regains 2HP.
            QuickHPCheck(2);
            QuickHandCheck(0);
        }
        [Test()]
        public void TestMoltenVeinsNotAskForSearchIfAlreadyInPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Titanform");
            DiscardAllCards(titan);
            MoveCard(titan, "PaybackTime", titan.HeroTurnTaker.Hand);
            AssertMaxNumberOfDecisions(1);

            PlayCard("MoltenVeins");
        }
        [Test()]
        public void TestMoltenVeinsNotAskForSearchIfAlreadyInHand()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DiscardAllCards(titan);
            MoveCard(titan, "PaybackTime", titan.HeroTurnTaker.Hand);
            MoveCard(titan, "Titanform", titan.HeroTurnTaker.Hand);
            AssertMaxNumberOfDecisions(1);

            PlayCard("MoltenVeins");
        }

        [Test()]
        public void TestMs5DemolitionCharge0Environment()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHPStorage(titan);
            PlayCard("Ms5DemolitionCharge");
            //Destroy all environment cards.
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            QuickHPCheckZero();
        }

        [Test()]
        public void TestMs5DemolitionCharge1Environment()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mono = PlayCard("PlummetingMonorail");

            QuickHPStorage(titan);
            PlayCard("Ms5DemolitionCharge");
            //Destroy all environment cards.
            AssertInTrash(new Card[] { mono });
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestMs5DemolitionCharge2Environment()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mono0 = PlayCard("PlummetingMonorail", 0);
            Card mono1 = PlayCard("PlummetingMonorail", 1);

            QuickHPStorage(titan);
            PlayCard("Ms5DemolitionCharge");
            //Destroy all environment cards.
            AssertInTrash(new Card[] { mono0, mono1 });
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestMs5DemolitionCharge3Environment()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mono0 = PlayCard("PlummetingMonorail", 0);
            Card mono1 = PlayCard("PlummetingMonorail", 1);
            Card popo = PlayCard("PoliceBackup");

            QuickHPStorage(titan);
            PlayCard("Ms5DemolitionCharge");
            //Destroy all environment cards.
            AssertInTrash(new Card[] { mono0, mono1, popo });
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            QuickHPCheck(-3);
        }
        [Test()]
        public void TestMs5DemolitionChargeIndividualHits()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "CaptainCosmic", "Megalopolis");
            StartGame();

            Card mono0 = PlayCard("PlummetingMonorail", 0);
            Card mono1 = PlayCard("PlummetingMonorail", 1);
            Card popo = PlayCard("PoliceBackup");

            QuickHPStorage(titan);
            PlayCard("EnergyBracer");
            PlayCard("Ms5DemolitionCharge");
            //Destroy all environment cards.
            AssertInTrash(new Card[] { mono0, mono1, popo });
            //For each environment card destroyed this way, {Titan} deals himself 1 fire damage.
            QuickHPCheck(0);
        }

        [Test()]
        public void TestObsidianGraspNotTitanform()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card imm = PutOnDeck("Immolate");

            QuickHPStorage(omnitron);
            PlayCard("ObsidianGrasp");
            //{Titan} deals 1 target 3 melee damage.
            QuickHPCheck(-3);
            //If Titanform is in play, you may discard a card to search your deck and trash for a copy of the card Immolate and play it next to that target. If you searched your deck, shuffle it.
            AssertInDeck(imm);
        }

        [Test()]
        public void TestObsidianGraspYesTitanformSearchDeck()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Titanform");
            Card imm = PutOnDeck("Immolate");
            Card veins = PutInHand("MoltenVeins");
            DecisionsYesNo = new bool[] { false, true };
            DecisionSelectCards = new Card[] { omnitron.CharacterCard, veins, imm };

            DecisionSelectFunction = 0;

            QuickShuffleStorage(titan);
            QuickHPStorage(omnitron);
            PlayCard("ObsidianGrasp");
            //{Titan} deals 1 target 3 melee damage.
            QuickHPCheck(-3);
            //If Titanform is in play, you may discard a card to search your deck and trash for a copy of the card Immolate and play it next to that target. If you searched your deck, shuffle it.
            AssertNextToCard(imm, omnitron.CharacterCard);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestObsidianGraspYesTitanformSearchTrash()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Titanform");
            Card imm = PutInTrash("Immolate");
            Card veins = PutInHand("MoltenVeins");
            DecisionsYesNo = new bool[] { false, true };
            DecisionSelectCards = new Card[] { omnitron.CharacterCard, veins, imm };

            DecisionSelectFunction = 1;

            QuickShuffleStorage(titan);
            QuickHPStorage(omnitron);
            PlayCard("ObsidianGrasp");
            //{Titan} deals 1 target 3 melee damage.
            QuickHPCheck(-3);
            //If Titanform is in play, you may discard a card to search your deck and trash for a copy of the card Immolate and play it next to that target. If you searched your deck, shuffle it.
            AssertNextToCard(imm, omnitron.CharacterCard);
            QuickShuffleCheck(0);
        }

        [Test()]
        public void TestObsidianGraspDestroysTarget()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Titanform");
            Card veins = PutInHand("MoltenVeins");
            Card imm = PutOnDeck("Immolate");
            Card s84 = PlayCard("S84AutomatonDrone");
            DecisionsYesNo = new bool[] { false, true };
            DecisionSelectCards = new Card[] { s84, veins, imm };
            DecisionSelectFunction = 0;

            PlayCard("ObsidianGrasp");
            AssertInTrash(s84);
            AssertOnTopOfDeck(imm);
        }

        [Test()]
        public void TestPaybackTime()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck(omnitron, GetCard("Terraforming", 0));
            PutOnDeck(omnitron, GetCard("Terraforming", 1));
            PlayCard("PaybackTime");
            GoToStartOfTurn(omnitron);
            //Increase damage dealt by {Titan} by 1 to non-hero targets that have dealt him damage since the end of his last turn.

            //Hasn't been hit by Omnitron yet
            QuickHPStorage(omnitron);
            DealDamage(titan, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //getting hit by Omnitron
            DealDamage(omnitron, titan, 2, DamageType.Melee);

            //+1 for having been hit
            QuickHPStorage(omnitron);
            DealDamage(titan, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //hasn't been hit by Haka yet
            QuickHPStorage(haka);
            DealDamage(titan, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //shouldn't trigger against heroes
            DealDamage(haka, titan, 2, DamageType.Melee);
            DealDamage(titan, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //shouldn't trigger against things that haven't hit him
            Card epe = PlayCard("ElectroPulseExplosive");
            QuickHPStorage(epe);
            DealDamage(titan, epe, 2, DamageType.Melee);
            QuickHPCheck(-2);

            GoToPlayCardPhase(titan);
            //At the end of your turn {Titan} regains 1HP.
            QuickHPStorage(titan);
            GoToEndOfTurn(titan);
            QuickHPCheck(1);

            GoToStartOfTurn(haka);
            //reset list of people who have hit Titan
            QuickHPStorage(omnitron);
            DealDamage(titan, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestReversalToPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutInTrash("Titanform");

            QuickHPStorage(omnitron);
            PlayCard("Reversal");
            //{Titan} deals 1 target 1 infernal damage.
            QuickHPCheck(-1);
            //Redirect the next damage dealt by that target back to itself.
            QuickHPStorage(omnitron, titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);
            //If Titanform is in your trash, you may put it into play or into your hand.
            AssertIsInPlay(tform);
        }

        [Test()]
        public void TestReversal_SecondEffectTriggering_ReduceTo0()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //reduce damage being dealt to omni to 0
            AddReduceDamageTrigger(omnitron, false, true, 1);
            QuickHPStorage(omnitron);
            PlayCard("Reversal");
            QuickHPCheck(0);

            //should still be redirected back
            QuickHPStorage(omnitron, titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(-2, 0);

        }

        [Test()]
        public void TestReversal_SecondEffectTriggering_CannotDealDamage()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //titan can't deal damage
            AddCannotDealNextDamageTrigger(titan, titan.CharacterCard);
            QuickHPStorage(omnitron);
            PlayCard("Reversal");
            QuickHPCheck(0);

            //should still be redirected back
            QuickHPStorage(omnitron, titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee, isIrreducible: true);
            QuickHPCheck(-2, 0);
        }

            [Test()]
        public void TestReversalToHand()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutInTrash("Titanform");
            DecisionMoveCardDestination = new MoveCardDestination(titan.HeroTurnTaker.Hand);

            QuickHPStorage(omnitron);
            PlayCard("Reversal");
            //{Titan} deals 1 target 1 infernal damage.
            QuickHPCheck(-1);
            //Redirect the next damage dealt by that target back to itself.
            QuickHPStorage(omnitron, titan);
            DealDamage(omnitron, titan, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);
            //If Titanform is in your trash, you may put it into play or into your hand.
            AssertInHand(tform);
        }

        [Test()]
        public void TestReversalRedirectTiming_Issue1136()
        {
            SetupGameController("Ambuscade", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mine = PlayCard("SonicMine");

            QuickHPStorage(ambuscade, titan, haka, bunker, scholar);
            DecisionSelectTarget = mine;
            DecisionAutoDecideIfAble = true;
            PlayCard("Reversal");
            QuickHPCheckZero();


        }

        [Test()]
        public void TestStubbornGoliath()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck(omnitron, GetCard("Terraforming", 0));
            Card epe = PlayCard("ElectroPulseExplosive");
            DecisionYesNo = true;
            Card gol = PlayCard("StubbornGoliath");
            UsePower(gol);
            //{Titan} deals up to 2 non-hero targets 2 infernal damage each.{BR}Until the start of your next turn, when those targets would deal damage, you may redirect that damage to {Titan}.
            QuickHPStorage(titan, haka);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            DealDamage(epe, haka, 2, DamageType.Melee);
            QuickHPCheck(-4, 0);

            //no longer works at start of new turn
            GoToStartOfTurn(titan);
            QuickHPStorage(titan, haka);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            DealDamage(epe, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -4);
        }
        [Test()]
        public void TestStubbornGoliathNotFollowOutOfPlay()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck(omnitron, GetCard("Terraforming", 0));
            Card epe = PlayCard("ElectroPulseExplosive");
            DecisionYesNo = true;
            Card gol = PlayCard("StubbornGoliath");
            UsePower(gol);
            //{Titan} deals up to 2 non-hero targets 2 infernal damage each.{BR}Until the start of your next turn, when those targets would deal damage, you may redirect that damage to {Titan}.
            DestroyCard(epe);
            PlayCard(epe);
            QuickHPStorage(titan, haka);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            DealDamage(epe, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);
        }
        [Test]
        public void TestStubbornGoliathFewerTargetsThanAllowed()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck(omnitron, GetCard("Terraforming", 0));
            Card epe = PlayCard("ElectroPulseExplosive");
            DecisionYesNo = true;
            Card gol = PlayCard("StubbornGoliath");
            DecisionSelectTargets = new Card[] { epe, null };
            UsePower(gol);
            QuickHPStorage(haka, titan);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            DealDamage(epe, haka, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);
        }
        [Test]
        public void TestStubbornGoliathRedirectOptional()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PutOnDeck(omnitron, GetCard("Terraforming", 0));
            Card epe = PlayCard("ElectroPulseExplosive");
            DecisionYesNo = false;
            Card gol = PlayCard("StubbornGoliath");
            DecisionSelectTargets = new Card[] { epe, omnitron.CharacterCard };
            UsePower(gol);
            QuickHPStorage(haka, titan);
            DealDamage(omnitron, haka, 2, DamageType.Melee);
            DealDamage(epe, haka, 2, DamageType.Melee);
            QuickHPCheck(-4, 0);
        }
        [Test()]
        public void TestTheChaplianPower1()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");
            PlayCard("Titanform");
            Card chap = PlayCard("TheChaplain");
            //{Titan} deals 1 target 3 projectile damage.
            QuickHPStorage(omnitron);
            UsePower(chap, 0);
            QuickHPCheck(-3);
            //first power does not destroy ongoings
            AssertIsInPlay(moko);
        }
        [Test()]
        public void TestTheChaplianPower2NoTitan()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");

            Card chap = PlayCard("TheChaplain");
            //{Titan} deals 1 target 3 projectile damage.
            QuickHPStorage(omnitron);
            UsePower(chap, 1);
            //If Titanform is in play, destroy 1 ongoing card.
            AssertIsInPlay(moko);
            //this power does not deal damage
            QuickHPCheck(0);
        }

        [Test()]
        public void TestTheChaplianPower2YesTitan()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");
            PlayCard("Titanform");
            DecisionYesNo = false;
            DecisionSelectCard = moko;

            Card chap = PlayCard("TheChaplain");
            QuickHPStorage(omnitron);
            UsePower(chap, 1);
            //If Titanform is in play, destroy 1 ongoing card.
            AssertInTrash(moko);
            //this power does not deal damage
            QuickHPCheck(0);
        }

        [Test()]
        public void TestVulcansJudgmentNoTitan()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card vulc = PlayCard("VulcansJudgment");
            //When this card is destroyed, {Titan} deals 1 villain target 5 infernal damage. If Titanform is in play, {Titan} also deals that target 2 fire damage.
            QuickHPStorage(omnitron);
            UsePower(vulc);
            QuickHPCheck(-5);
            AssertInTrash(vulc);
        }

        [Test()]
        public void TestVulcansJudgmentYesTitan()
        {
            SetupGameController("Omnitron", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("Titanform");
            DecisionYesNo = false;

            Card vulc = PlayCard("VulcansJudgment");
            //When this card is destroyed, {Titan} deals 1 villain target 5 infernal damage. If Titanform is in play, {Titan} also deals that target 2 fire damage.
            QuickHPStorage(omnitron);
            UsePower(vulc);
            QuickHPCheck(-7);
            AssertInTrash(vulc);
        }

        [Test()]
        public void TestUnbreakable()
        {
            SetupGameController("Omnitron", "CaptainCosmic", "Cauldron.Titan", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card epe = PutOnDeck("ElectroPulseExplosive");
            Card unb = PlayCard("Unbreakable");

            //Skip any effects which would act at the end of the villain turn.
            //Skip play top card of deck
            GoToEndOfTurn(omnitron);
            AssertInDeck(epe);

            //Don't skip Hero End of Turn
            GoToStartOfTurn(cosmic);
            PlayCard("UnflaggingAnimation");
            QuickHPStorage(cosmic);
            GoToEndOfTurn(cosmic);
            QuickHPCheck(-1);

            //You may not use Powers.
            QuickHPStorage(omnitron);
            UsePower(titan);
            QuickHPCheckZero();

            //You may not draw cards.
            QuickHandStorage(titan);
            DrawCard(titan);
            QuickHandCheckZero();

            //At the start of your turn, destroy this card.
            GoToStartOfTurn(titan);
            AssertInTrash(unb);
        }

        [Test()]
        public void TestUnbreakableOblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Titan", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Card unb = PlayCard("Unbreakable");
            SwitchBattleZone(oblivaeon);
            AssertBattleZone(oblivaeon.CharacterCard, bzTwo);
            AssertBattleZone(titan.CharacterCard, bzOne);

            GoToEndOfTurn(oblivaeon);
            AssertNumberOfCardsInPlay((Card c) => c.IsAeonMan, 3);
            

           
        }

        [Test()]
        public void TestUnbreakableVillainEndOfHeroTurnEffects()
        {
            SetupGameController("Chokepoint", "Haka", "Cauldron.Titan", "TheScholar", "VoidGuardDrMedico", "Megalopolis");
            StartGame();

            PlayCard("Unbreakable");

            PlayCard("Mere");
            PlayCard("ShockingAnimation");
            //Skip any effects which would act at the end of the villain turn.

            //Villain End of Turn effects trigger on other turns as appropriate
            //Shocking Animation deals 2 to all hero targets
            GoToStartOfTurn(haka);
            QuickHPStorage(haka, scholar, voidMedico, titan);
            GoToEndOfTurn(haka);
            QuickHPCheck(-2, -2, -2, -2);
        }

        [Test()]
        public void TestUnbreakableHeroEndOfVillainTurnEffects()
        {
            SetupGameController("Chokepoint", "Haka", "Cauldron.Titan", "TheScholar", "VoidGuardDrMedico", "Megalopolis");
            StartGame();

            PlayCard("Unbreakable");
            //Skip any effects which would act at the end of the villain turn.

            //Hero end of turn needs to be canceled
            //Regeneration heals Medico 2
            PlayCard("Regeneration");
            SetHitPoints(voidMedico, 10);
            QuickHPStorage(voidMedico);
            GoToEndOfTurn(choke);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestUnbreakableDestroySelfAndRestoreEndOfTurnEffects()
        {
            SetupGameController("Omnitron", "Haka", "Cauldron.Titan", "TheScholar", "VoidGuardDrMedico", "Megalopolis");
            StartGame();

            Card drone = PutInTrash("S83AssaultDrone");
            Card unb = PlayCard("Unbreakable");
            //At the start of your turn, destroy this card.
            GoToStartOfTurn(titan);
            AssertInTrash(unb);

            GoToEndOfTurn(omnitron);
            AssertIsInPlay(drone);
        }

        [Test()]
        public void TestUnbreakableVillainTeamMode()
        {
            Game game = new Game(new string[] { "BugbearTeam", "Cauldron.Titan", "ErmineTeam", "Bunker", "FrictionTeam", "Lifeline", "GreazerTeam", "VoidGuardDrMedico", "Megalopolis" });
            SetupGameController(game);
            StartGame();

            GoToStartOfTurn(ermineTeam);

            PlayCard("Unbreakable");
            //Skip any effects which would act at the end of the villain turn.

            //Hero end of turn needs to be canceled
            //Regeneration heals Medico 2
            PlayCard("Regeneration");
            SetHitPoints(voidMedico, 10);
            QuickHPStorage(voidMedico);
            GoToStartOfTurn(voidMedico);
            //+2 each for Bunker and Scholar's turns
            QuickHPCheck(4);
        }

        [Test()]
        public void TestTitanAndProgeny()
        {
            SetupGameController("Progeny", "Cauldron.Titan", "Haka", "Bunker", "TheScholar", "Megalopolis");
            SetHitPoints(titan, 8);
            Card tform = PlayCard("Titanform");
            StartGame();

            DecisionYesNo = true;
            DealDamage(titan, progeny, 3, DamageType.Infernal);
            DealDamage(progeny, titan, 8, DamageType.Radiant);
            AssertIncapacitated(titan);

            GoToStartOfTurn(progeny);

            AssertNotFlipped(progeny);


            
        }
    }
}

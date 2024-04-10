using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheCybersphereTests : CauldronBaseTest
    {

        #region CybersphereHelperFunctions

        protected TurnTakerController cybersphere { get { return FindEnvironment(); } }
        protected bool IsGridVirus(Card card)
        {
            return card != null && card.DoKeywordsContain("grid virus");
        }
        #endregion

        [Test()]
        public void TestCybersphereLoads()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTestGridVirus_IsGridVirus([Values("B3h3mth", "Dr3Dnt", "Fo551l", "Gho5t", "Glitch", "H3l1x", "InfectedFirewall", "InfectedHoloweapon", "N1nj4", "Sp4rk")] string gridVirus)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            GoToPlayCardPhase(cybersphere);

            Card card = PlayCard(gridVirus);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "grid virus", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestGridProgram_IsGridProgram([Values("HologameArena", "HolocycleRace")] string gridProgram)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            GoToPlayCardPhase(cybersphere);

            Card card = PlayCard(gridProgram);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "grid program", false);
        }

        [Test()]
        public void TestB3h3mth()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            SetHitPoints(haka, 15);

            GoToPlayCardPhase(cybersphere);
            Card b3h3mth = PlayCard("B3h3mth");

            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP 4 fire damage.
            //2nd lowest is Haka

            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, b3h3mth);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(0, 0, 0, -4, 0);

        }

        [Test()]
        public void TestDr3Dnt()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card dr3Dnt = PlayCard("Dr3Dnt");

            //At the end of the environment turn, this card deals each other target 2 projectile damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, dr3Dnt);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(-2, -2, -2, -2, 0);

        }

        [Test()]
        public void TestF0551l()
        {
            SetupGameController("BaronBlade", "Ra", "Stuntman", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //this will result in stuntman being the second highest
            SetHitPoints(ra, 20);
            SetHitPoints(haka, 15);
            SetHitPoints(stunt, 25);
            SetHitPoints(baron, 25);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            //Play out CouteQueCoute to verify there were two instances of damage
            PlayCard("CouteQueCoute");

            GoToPlayCardPhase(cybersphere);
            Card f0551l = PlayCard("Fo551l");

            //At the end of the environment turn, this card deals the non-environment target with the second highest HP 3 melee damage and 1 lightning damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, stunt.CharacterCard, haka.CharacterCard, f0551l);
            //should allow you to select if there is a tie
            DecisionSelectTarget = stunt.CharacterCard;
            GoToEndOfTurn(cybersphere);
            //3 +1, 1+1 -> 6 damage total
            QuickHPCheck(0, 0, -6, 0, 0);

        }

        [Test()]
        public void TestGho5t_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            string[] inPlay = new string[] { "TheStaffOfRa", "Dominion" };
            IEnumerable<Card> inPlayCards = PlayCards(inPlay);

            string[] inTrash = new string[] { "FlameBarrier", "Mere" };
            IEnumerable<Card> inTrashCards = PlayCards(inTrash);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card gho5t = PlayCard("Gho5t");

            //At the end of the environment turn, each hero must destroy 1 of their ongoing or equipment cards.
            DecisionSelectCards = inTrashCards;
            GoToEndOfTurn(cybersphere);
            AssertInTrash(inTrashCards);
            AssertIsInPlay(inPlayCards);

        }

        [Test()]
        public void TestGho5t_Destroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card gho5t = PlayCard("Gho5t");

            //When this card is destroyed, each player may draw a card.
            QuickHandStorage(ra, legacy, haka);
            DestroyCard(gho5t, baron.CharacterCard);
            QuickHandCheck(1, 1, 1);


        }


        [Test()]
        public void TestGho5t_Destroyed_OptionalDraw()
        {
            SetupGameController("Omnitron", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            PutOnDeck("InterpolationBeam");

            GoToPlayCardPhase(cybersphere);
            Card gho5t = PlayCard("Gho5t");

            //When this card is destroyed, each player *may* draw a card.

            DecisionsYesNo = new bool[] { false, false, true };
            QuickHandStorage(ra, legacy, haka);
            QuickHPStorage(ra, legacy, haka);
            DestroyCard(gho5t, omnitron.CharacterCard);
            QuickHPCheck(0, 0, -1);
            QuickHandCheck(0, 0, 1);


        }

        [Test()]
        public void TestGlitch_Destroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            SetHitPoints(ra, 20);
            SetHitPoints(haka, 15);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card glitch = PlayCard("Glitch");

            //When this card is destroyed, it deals the non-environment target with the second highest HP 5 lightning damage.
            //legacy is second highest
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DestroyCard(glitch, baron.CharacterCard);
            QuickHPCheck(0, 0, -5, 0);


        }

        [Test()]
        public void TestH3l1x()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            SetHitPoints(ra, 10);
            SetHitPoints(legacy, 15);
            SetHitPoints(haka, 20);
            SetHitPoints(tachyon, 11);
            GoToEndOfTurn(haka);
            Card h3l1x = PlayCard("H3l1x");

            //At the start of the environment turn, this card deals the hero target with the lowest HP 1 melee damage, the hero target with the second highest HP 3 melee damage, and the hero target with the highest HP 5 melee damage.
            //lowest hp ra, second highest is legacy, highest is haka
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToStartOfTurn(cybersphere);
            QuickHPCheck(0, -1, -3, -5, 0);

        }

        [Test()]
        public void TestHolocycleRace_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card h3l1x = PlayCard("H3l1x");


            //Increase all damage dealt by 1.
            Card holocycleRace = PlayCard("HolocycleRace");

            //hero damage
            QuickHPStorage(baron);
            DealDamage(ra, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);

            //villain damage
            QuickHPStorage(ra);
            DealDamage(baron, ra, 2, DamageType.Fire);
            QuickHPCheck(-3);

            //env damage
            QuickHPStorage(baron);
            DealDamage(h3l1x, baron, 2, DamageType.Fire);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestHolocycleRace_StartOfTurn_ThresholdMet()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToEndOfTurn(haka);
            Card holocycleRace = PlayCard("HolocycleRace");

            //At the start of the environment turn, each player may discard up to 2 cards. Then, unless {H} cards were discarded this way, destroy this card.
            QuickHandStorage(ra, legacy, haka, tachyon);
            DecisionSelectCards = new Card[] {ra.HeroTurnTaker.Hand.Cards.ElementAt(0), null,
                                            legacy.HeroTurnTaker.Hand.Cards.ElementAt(0), legacy.HeroTurnTaker.Hand.Cards.ElementAt(1),
                                            null,
                                            tachyon.HeroTurnTaker.Hand.Cards.ElementAt(0), null};

            GoToStartOfTurn(cybersphere);
            QuickHandCheck(-1, -2, 0, -1);
            AssertIsInPlay(holocycleRace);
            
        }

        [Test()]
        public void TestHolocycleRace_StartOfTurn_ThresholdExceeded()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card holocycleRace = PlayCard("HolocycleRace");

            //At the start of the environment turn, each player may discard up to 2 cards. Then, unless {H} cards were discarded this way, destroy this card.
            QuickHandStorage(ra, legacy, haka, tachyon);
            DecisionSelectCards = new Card[] {ra.HeroTurnTaker.Hand.Cards.ElementAt(0), ra.HeroTurnTaker.Hand.Cards.ElementAt(1),
                                            legacy.HeroTurnTaker.Hand.Cards.ElementAt(0), legacy.HeroTurnTaker.Hand.Cards.ElementAt(1),
                                            haka.HeroTurnTaker.Hand.Cards.ElementAt(0), haka.HeroTurnTaker.Hand.Cards.ElementAt(1),
                                            tachyon.HeroTurnTaker.Hand.Cards.ElementAt(0), null};

            GoToStartOfTurn(cybersphere);
            QuickHandCheck(-2, -2, -2, -1);
            AssertInTrash(holocycleRace);

        }

        [Test()]
        public void TestHolocycleRace_StartOfTurn_ThresholdNotMet()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card holocycleRace = PlayCard("HolocycleRace");

            //At the start of the environment turn, each player may discard up to 2 cards. Then, unless {H} cards were discarded this way, destroy this card.
            QuickHandStorage(ra, legacy, haka, tachyon);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            GoToStartOfTurn(cybersphere);
            QuickHandCheck(0, 0, 0, 0);
            AssertInTrash(holocycleRace);

        }

        [Test()]
        public void TestHologameArena_DestroyCardDrawCard()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card hologameArena = PlayCard("HologameArena");

            //Whenever a hero card destroys an environment target, that hero draws a card.
            QuickHandStorage(ra, legacy, haka, tachyon);
            Card dr3Dnt = PlayCard("Dr3Dnt");
            DestroyCard(dr3Dnt, ra.CharacterCard);
            QuickHandCheck(1, 0, 0, 0);

            Card blinding = PutInTrash("BlindingSpeed");
            QuickHandUpdate();
            dr3Dnt = PlayCard("Dr3Dnt");
            DecisionSelectCard = dr3Dnt;
            PlayCard(blinding);
            QuickHandCheck(0, 0, 0, 1);

            QuickHandUpdate();
            dr3Dnt = PlayCard("Dr3Dnt");
            DestroyCard(dr3Dnt, baron.CharacterCard);
            QuickHandCheck(0, 0, 0, 0);


        }

        [Test()]
        public void TestHologameArena_StartOfTurn_Destroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card hologameArena = PlayCard("HologameArena");
            Card glitch = PutOnDeck("Glitch");

            //At the end of the Environment turn, either destroy this card or play the top card of the Environment deck.
            DecisionYesNo = true;
            GoToStartOfTurn(cybersphere);
            AssertInTrash(cybersphere, hologameArena);

        }

        [Test()]
        public void TestHologameArena_StartOfTurn_DestroyOptional()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card hologameArena = PlayCard("HologameArena");
            Card glitch = PutOnDeck("Glitch");

            //At the end of the Environment turn, either destroy this card or play the top card of the Environment deck.
            DecisionYesNo = false;
            GoToStartOfTurn(cybersphere);
            AssertInPlayArea(cybersphere, hologameArena);

        }

        [Test()]
        public void TestHologameArena_EndOfTurn_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card hologameArena = PlayCard("HologameArena");

            Card glitch = PutOnDeck("Glitch");

            //At the end of the Environment turn, either destroy this card or play the top card of the Environment deck.
            GoToEndOfTurn(cybersphere);
            AssertInPlayArea(cybersphere, glitch);
        }

        [Test()]
        public void TestInfectedFirewall_Reduce()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card firewall = PlayCard("InfectedFirewall");
            Card dr3Dnt = PlayCard("Dr3Dnt");

            //Reduce damage dealt to environment targets by 1.
            QuickHPStorage(dr3Dnt);
            DealDamage(baron.CharacterCard, dr3Dnt, 2, DamageType.Fire);
            QuickHPCheck(-1);

            //only environment
            QuickHPStorage(ra, baron);
            DealDamage(baron, ra, 2, DamageType.Fire);
            DealDamage(ra, baron, 2, DamageType.Fire);
            QuickHPCheck(-2, -2);
            

        }

        [Test()]
        public void TestInfectedFirewall_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            GoToEndOfTurn(haka);
            Card firewall = PlayCard("InfectedFirewall");
            Card dr3Dnt = PutOnDeck("Dr3Dnt");

            //At the end of the environment turn, play the top card of the environment deck.
            GoToEndOfTurn(cybersphere);
            AssertInPlayArea(cybersphere, dr3Dnt);

        }

        [Test()]
        public void TestInfectedHoloWeapon()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();


            GoToEndOfTurn(haka);
            Card holoweapon = PlayCard("InfectedHoloweapon");

            //At the end of the environment turn, this card deals the 2 non-environment targets with the highest HP 2 irreducible energy damage each.
            //Damage dealt by those targets is irreducible until the start of the environment turn.
            //mdp makes baron immune to the damage, but should still get the irreducible buff

            //baron and haka are the highest HP
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(0, 0, 0, -2, 0);

            //damage dealt by each of them is irreducible
            PlayCard("TaMoko");
            QuickHPStorage(haka);
            DealDamage(baron, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPUpdate();
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //only those targets
            QuickHPUpdate();
            DealDamage(ra, haka, 3, DamageType.Melee);
            QuickHPCheck(-2);

            //Resets at start of the env turn
            GoToStartOfTurn(cybersphere);
            QuickHPUpdate();
            DealDamage(baron, haka, 3, DamageType.Melee);
            QuickHPCheck(-2);
            QuickHPUpdate();
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestN1nj4_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToEndOfTurn(haka);
            Card n1nj4 = PlayCard("N1nj4");

            //At the end of the environment turn, this card deals the target other than itself with the lowest HP 3 energy damage.
            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(0, -3, 0, 0, 0, 0);

        }

        [Test()]
        public void TestN1nj4_DamageDestroysTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToEndOfTurn(haka);
            Card n1nj4 = PlayCard("N1nj4");
            Card glitch = PutOnDeck("Glitch");
            AssertInDeck(glitch);

            //Whenever damage dealt by this card destroys a target, play the top card of the environment deck.
            DealDamage(n1nj4, mdp, 10, DamageType.Melee);
            AssertInTrash(mdp);
            AssertInPlayArea(cybersphere, glitch);

        }

        [Test()]
        public void TestReplication_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToEndOfTurn(haka);

            //don't want this coming out and blowing up any viruses
            PutInTrash("YourMindMakesItReal");

            int numVirus = GameController.Game.RNG.Next(0, 5);
            IEnumerable<Card> virusToPlay = FindCardsWhere((Card c) => IsGridVirus(c) && cybersphere.TurnTaker.Deck.Cards.Contains(c)).Take(numVirus);

            foreach(Card virus in virusToPlay)
            {
                PlayCard(virus);
            }

            //When this card enters play, play the top X cards of the environment deck, where X is 1 plus the number of Grid Virus cards currently in play.
            Card replication = PlayCard("Replication");
            if (numVirus > 0)
            {
                AssertInPlayArea(cybersphere, virusToPlay);
            }
            //X original viruses, 1 from Replication, X+1 plays, for a total of (X+1) * 2 cards
            AssertNumberOfCardsInPlay(cybersphere, (numVirus + 1) * 2);

        }

        [Test()]
        public void TestReplication_Destroy()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere" });
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToEndOfTurn(haka);

            IEnumerable<Card> nonVirusToPlay = FindCardsWhere((Card c) => !IsGridVirus(c) && c.Identifier != "SystemCrash" && cybersphere.TurnTaker.Deck.Cards.Contains(c)).Take(2);
            PutOnDeck(cybersphere, nonVirusToPlay);

            //At the end of the environment turn, destroy this card.
            Card replication = PlayCard("Replication");
            AssertInPlayArea(cybersphere, replication);
            GoToEndOfTurn(cybersphere);
            AssertInTrash(replication);



        }

        [Test()]
        public void TestSp4rk()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToEndOfTurn(haka);


            //At the end of the environment turn, this card deals the non-environment target with the second highest HP 3 toxic damage.
            //second highest is haka
            Card sp4rk = PlayCard("Sp4rk");
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(0, 0, 0, -3, 0);
   
        }

        [Test()]
        public void TestSystemCrash_LessThan4()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            GoToEndOfTurn(haka);

            IEnumerable<Card> virusToPlay = FindCardsWhere((Card c) => IsGridVirus(c) && cybersphere.TurnTaker.Deck.Cards.Contains(c)).Take(3);
            PlayCards(virusToPlay);
            PlayCard("SystemCrash");
            //At the start of the environment turn, if there are at least 4 Grid Virus cards in play, everyone is deleted. [b] Game Over.[/ b]
            GoToStartOfTurn(cybersphere);
            AssertNotGameOver();
        }

        [Test()]
        public void TestSystemCrash_MoreThan4()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();

            GoToEndOfTurn(haka);

            IEnumerable<Card> virusToPlay = FindCardsWhere((Card c) => IsGridVirus(c) && cybersphere.TurnTaker.Deck.Cards.Contains(c)).Take(4);
            PlayCards(virusToPlay);
            PlayCard("SystemCrash");
            //At the start of the environment turn, if there are at least 4 Grid Virus cards in play, everyone is deleted. [b] Game Over.[/ b]
            GoToStartOfTurn(cybersphere);
            AssertGameOver(EndingResult.EnvironmentDefeat);
        }

        [Test()]
        public void TestYourMindMakesItReal_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            GoToEndOfTurn(haka);
            //When this card enters play, it deals each target 3 lightning damage.
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            Card yourMindMakesItReal = PlayCard("YourMindMakesItReal");
            QuickHPCheck(-3, -3, -3, -3, -3);
        }

        [Test()]
        public void TestYourMindMakesItReal_EndOFTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.TheCybersphere");
            StartGame();
            GoToEndOfTurn(haka);
            //At the end of the environment turn, destroy this card.
            Card yourMindMakesItReal = PlayCard("YourMindMakesItReal");
            AssertInPlayArea(cybersphere, yourMindMakesItReal);
            GoToEndOfTurn(cybersphere);
            AssertInTrash(yourMindMakesItReal);
        }

    }
}

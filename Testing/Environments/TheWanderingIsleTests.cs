using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace MyModTest
{
    [TestFixture()]
    public class TheWanderingIsleTests : BaseTest
    {

        #region TheWanderingIsleHelperFunctions

        protected TurnTakerController isle { get { return FindEnvironment(); } }

        private bool IsTeryxInPlay(TurnTakerController ttc)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsTeryx(c));
            var numCardsInPlay = cardsInPlay.Count();

            return numCardsInPlay > 0;
        }
        private bool IsTeryx(Card card)
        {
            return card.Identifier == "Teryx";
        }

        #endregion

        [Test()]
        public void TestTheWanderingIsleWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            
        }

        [Test()]
        public void TestTeryxIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //teryx is indestructible, so shouldn't be destroyed
            int numCardsInPlayBefore = GetNumberOfCardsInPlay(isle);
            DestroyCard(teryx, baron.CharacterCard);
            int numCardsInPlayAfter = GetNumberOfCardsInPlay(isle);
            Assert.AreEqual(numCardsInPlayBefore, numCardsInPlayAfter, $"Teryx was destroyed when they shouldn't have. Expected {numCardsInPlayBefore}, actual {numCardsInPlayAfter}");

        }

        [Test()]
        public void TestTeryxGameOver()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //If this card reaches 0HP, the heroes lose.
            DealDamage(baron.CharacterCard, teryx, 50, DamageType.Melee, true);
            
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestTeryxVillainDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //At the end of the environment turn, the villain target with the highest HP deals Teryx {H + 2} energy damage.
            //H = 3, should be dealt 5 damage
            QuickHPStorage(teryx);
            GoToEndOfTurn(isle);
            QuickHPCheck(-5);

        }

        [Test()]
        public void TestTeryxHeroDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //set hp lower so something to gain
            SetHitPoints(teryx, 30);

            //Whenever a hero target would deal damage to Teryx, Teryx Instead regains that much HP.
            QuickHPStorage(teryx);
            DealDamage(ra, teryx, 5, DamageType.Fire);
            QuickHPCheck(5);

        }

        [Test()]
        public void TestAmphibiousAssaultPlayWith2Villains()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            List<int?> hpBefore = new List<int?>() { ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };
            //When this card enters play, the {H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            //H = 3, so 2 villain targets hould deal damage
            //since we aren't selecting targets, it should default to ra, then fanatic being dealt damage
            PutIntoPlay("AmphibiousAssault");
            List<int?> hpAfter = new List<int?>() { ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };

            Assert.AreEqual(hpBefore[0] - 3, hpAfter[0], "Ra's hitpoints doin't match.");
            Assert.AreEqual(hpBefore[1] - 3, hpAfter[1], "Fanatic's hitpoints doin't match.");
            Assert.AreEqual(hpBefore[2], hpAfter[2], "Haka's hitpoints doin't match.");

        }

        [Test()]
        public void TestAmphibiousAssaultPlayWith1Villains()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            //destroy mdp so there is only baron blade in play
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            List<int?> hpBefore = new List<int?>() { ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };
            //When this card enters play, the {H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            //H = 3, so 2 villain targets should deal damage
            //however, there is only 1 villain target in play, so only 1 instance of damage
            //since we aren't selecting targets, it should default to ra
            PutIntoPlay("AmphibiousAssault");
            List<int?> hpAfter = new List<int?>() { ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, haka.CharacterCard.HitPoints };

            Assert.AreEqual(hpBefore[0] - 3, hpAfter[0], "Ra's hitpoints doin't match.");
            Assert.AreEqual(hpBefore[1], hpAfter[1], "Fanatic's hitpoints doin't match.");
            Assert.AreEqual(hpBefore[2], hpAfter[2], "Haka's hitpoints doin't match.");

        }

        [Test()]
        public void TestAmphibiousAssaultStartOfTurnHeroCardsPlayed()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack villain deck to not play hasten doom
            PutOnDeck("MobileDefensePlatform");

            //play a hero card
            GoToPlayCardPhase(ra);
            PlayCard(GetCardFromHand(ra, 0));
            GoToPlayCardPhase(haka);

            PutIntoPlay("AmphibiousAssault");
            int numCardsInVillainDeckBefore = GetNumberOfCardsInDeck(baron);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            GoToStartOfTurn(isle);

            int numCardsInVillainDeckAfter = GetNumberOfCardsInDeck(baron);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //should be 1 fewer card in villain deck, and amphibious assult should have been destroyed
            Assert.AreEqual(numCardsInVillainDeckBefore - 1, numCardsInVillainDeckAfter, "Number of cards in Baron Blade's deck don't match.");
            Assert.AreEqual(numCardsInEnvironmentPlayBefore - 1, numCardsInEnvironmentPlayAfter, "Number of environment cards in play don't match.");
        }

        [Test()]
        public void TestAmphibiousAssaultStartOfTurnNoHeroCardsPlayed()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("AmphibiousAssault");
            int numCardsInVillainDeckBefore = GetNumberOfCardsInDeck(baron);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            //no hero cards were played, so no villain cards played and this card not discarded
            GoToStartOfTurn(isle);

            int numCardsInVillainDeckAfter = GetNumberOfCardsInDeck(baron);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //should be the same for both
            Assert.AreEqual(numCardsInVillainDeckBefore, numCardsInVillainDeckAfter, "Number of cards in Baron Blade's deck don't match.");
            Assert.AreEqual(numCardsInEnvironmentPlayBefore, numCardsInEnvironmentPlayAfter, "Number of environment cards in play don't match.");
        }

        [Test()]
        public void TestAncientParasiteHeroDamageMoveCard()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("AncientParasite");
            Card parasite = GetCardInPlay("AncientParasite");
            //Whenever this card is dealt damage by a hero target, move it next to that target.
            int numCardsNextToRaBefore = GetNumberOfCardsNextToCard(ra.CharacterCard);
            DealDamage(ra, GetCardInPlay("AncientParasite"), 5, DamageType.Fire);
            int numCardsNextToRaAfter = GetNumberOfCardsNextToCard(ra.CharacterCard);

            Assert.AreEqual(numCardsNextToRaBefore + 1, numCardsNextToRaAfter, "Number of cards next to Ra don't match");
            AssertNextToCard(parasite, ra.CharacterCard);
        }

        [Test()]
        public void TestAncientParasiteVillainDamageDontMoveCard()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("AncientParasite");

            //Whenever this card is dealt damage by a hero target, move it next to that target.
            int numCardsNextToBaronBladeBefore = GetNumberOfCardsNextToCard(baron.CharacterCard);
            DealDamage(baron, GetCardInPlay("AncientParasite"), 5, DamageType.Fire);
            int numCardsNextToBaronBladeAfter = GetNumberOfCardsNextToCard(baron.CharacterCard);

            Assert.AreEqual(numCardsNextToBaronBladeBefore, numCardsNextToBaronBladeAfter, "Number of cards next to Baron Blade don't match");

        }

        [Test()]
        public void TestAncientParasiteStartOfTurnNextToTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("AncientParasite");
            Card parasite = GetCardInPlay("AncientParasite");
            //move next to ra
            DealDamage(ra, GetCardInPlay("AncientParasite"), 5, DamageType.Fire);

            //At the start of the environment turn, if this card is next to a target, it deals that target {H} toxic damage and moves back to the environment play area. 
            //H is 3, so 3 damage should be dealt
            QuickHPStorage(ra);

            GoToStartOfTurn(isle);
            int numCardsInIslePlayAfter = GetNumberOfCardsInPlay(isle);
            QuickHPCheck(-3);
            AssertNotNextToCard(parasite, ra.CharacterCard);

           
        }

        [Test()]
        public void TestAncientParasiteStartOfTurnNotNextToTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("AncientParasite");
            PutIntoPlay("Teryx");

            Card teryx = GetCardInPlay("Teryx");


            //Otherwise it deals Teryx {H + 2} toxic damage.
            //H is 3, so 5 damage should be dealt
            QuickHPStorage(teryx);

            GoToStartOfTurn(isle);
            QuickHPCheck(-5);

        }

        [Test()]
        public void TestAncientParasiteCardNextToDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("DecoyProjection");
            Card decoy = GetCardInPlay("DecoyProjection");

            PutIntoPlay("AncientParasite");
            DealDamage(decoy, GetCardInPlay("AncientParasite"), 5, DamageType.Fire);

            int numCardsInVisionaryPlayBefore = GetNumberOfCardsInPlay(visionary);
            DestroyCard(decoy);
            int numCardsInVisionaryPlayAfter = GetNumberOfCardsInPlay(visionary);

            //visionary should have 1 less card in play, not two
            Assert.AreEqual(numCardsInVisionaryPlayBefore - 1, numCardsInVisionaryPlayAfter, "Visionary cards in play don't match");


        }

        [Test()]
        public void TestExposedLifeforcePlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("ExposedLifeforce");

            //teryx should now be in play
            Assert.IsTrue(this.IsTeryxInPlay(isle), "Teryx is not in play");

        }

        [Test()]
        public void TestExposedLifeforceIncreaseDamageVillain()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //Increase damage dealt by villain cards by 1.
            PutIntoPlay("ExposedLifeforce");

            QuickHPStorage(ra);
            DealDamage(baron, ra, 4, DamageType.Melee);

            //damge should be increase, so will be 5
            QuickHPCheck(-5);


        }

        [Test()]
        public void TestExposedLifeforceDontIncreaseDamageHero()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //Increase damage dealt by villain cards by 1.
            PutIntoPlay("ExposedLifeforce");

            QuickHPStorage(haka);
            DealDamage(ra, haka, 4, DamageType.Fire);

            //damge should be not increase, so will be 4
            QuickHPCheck(-4);


        }

        [Test()]
        public void TestExposedLifeforceDestroyWhen10()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            PutIntoPlay("ExposedLifeforce");


            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);

            //Destroy this card if Teryx regains 10HP in a single round.
            GainHP(teryx, 10);

            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            Assert.AreEqual(numCardsInEnvironmentPlayBefore - 1, numCardsInEnvironmentPlayAfter, "Number of environment cards in play don't match");


        }

        [Test()]
        public void TestExposedLifeforceNoDestroyWhenLessThan10()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            PutIntoPlay("ExposedLifeforce");


            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);

            //Destroy this card if Teryx regains 10HP in a single round.
            GainHP(teryx, 9);

            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //Since teryx only gained 9 hp should not destroy
            Assert.AreEqual(numCardsInEnvironmentPlayBefore, numCardsInEnvironmentPlayAfter, "Number of environment cards in play don't match");


        }

        [Test()]
        public void TestBarnacleHydraDestroyedTeryxInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //put teryx into play
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            PutIntoPlay("BarnacleHydra");
            Card hydra = GetCardInPlay("BarnacleHydra");
            //When this card is destroyed, it deals Teryx {H} toxic damage.
            //H=3, so 3 damage should be dealt
            QuickHPStorage(teryx);
            DestroyCard(hydra, baron.CharacterCard);
            QuickHPCheck(-3);


        }

        [Test()]
        public void TestBarnacleHydraDestroyedNoTeryxInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();


            PutIntoPlay("BarnacleHydra");
            Card hydra = GetCardInPlay("BarnacleHydra");
            //When this card is destroyed, it deals Teryx {H} toxic damage.
            //teryx is not in play so damage should be dealt, essentially checking for no crash

            DestroyCard(hydra, baron.CharacterCard);

            AssertNotGameOver();


        }

        [Test()]
        public void TestBarnacleHydraEndOfTurnNoSubmerge()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            SetHitPoints(ra.CharacterCard, 5);
            SetHitPoints(visionary.CharacterCard, 7);
            SetHitPoints(haka.CharacterCard, 9);


            PutIntoPlay("BarnacleHydra");
            Card hydra = GetCardInPlay("BarnacleHydra");

            //set hitpoints so have room to gain things
            //this also checks to make sure that damage being dealt ignores hydra
            SetHitPoints(hydra, 1);

            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP
            //lowest HP is ra
            //Submerge is not in play
            QuickHPStorage(ra.CharacterCard, hydra);
            GoToEndOfTurn(isle);
            QuickHPCheck(-2, 0);


        }

        [Test()]
        public void TestBarnacleHydraEndOfTurnSubmerge()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

       

            SetHitPoints(ra.CharacterCard, 5);
            SetHitPoints(visionary.CharacterCard, 7);
            SetHitPoints(haka.CharacterCard, 9);


            PutIntoPlay("BarnacleHydra");
            Card hydra = GetCardInPlay("BarnacleHydra");

            //set hitpoints so have room to gain things
            //this also checks to make sure that damage being dealt ignores hydra
            SetHitPoints(hydra, 1);

            GoToPlayCardPhase(isle);
            //put submerge into play
            PutIntoPlay("Submerge");

            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP
            //lowest HP is ra
            //Submerge is in play
            //submerge reduces damage by 2 so 0 damage should be dealt
            QuickHPStorage(ra);
            GoToEndOfTurn(isle);
            QuickHPCheck(0);

            //hydra max Hp is 6, so should be at max Hp now
            AssertIsAtMaxHP(hydra);

        }

        [Test()]
        public void TestIslandquakeDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            PutIntoPlay("Islandquake");

            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);

            //Then, this card is destroyed.
            GoToStartOfTurn(isle);

            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //this card should have destroyed itself
            Assert.AreEqual(numCardsInEnvironmentPlayBefore - 1, numCardsInEnvironmentPlayAfter, "Number of environment cards in play don't match");
        }

        [Test()]
        public void TestIslandquakeStartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //destroy mdp so baron blade is not immune to damage
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            GoToPlayCardPhase(ra);
            //ra deals damage to teryx to cause hp gain and to be immune to islandquake damage
            DealDamage(ra.CharacterCard, teryx, 5, DamageType.Fire);

            PutIntoPlay("Islandquake");


            //At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, teryx);
            GoToStartOfTurn(isle);
            QuickHPCheck(-4, 0, -4, -4, 0);
        }

        [Test()]
        public void TestSongOfTheDeepPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck to reduce variance
            PutOnDeck("Teryx");

            //When this card enters play, play the top card of the environment deck.

            int numCardsInEnvironmentDeckBefore = GetNumberOfCardsInDeck(isle);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //Play song of the deep
            PutIntoPlay("SongOfTheDeep");

            int numCardsInEnvironmentDeckAfter = GetNumberOfCardsInDeck(isle);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);


            //should be 2 fewer cards in the deck, one for song of the deep, 1 for top card of the deck
            Assert.AreEqual(numCardsInEnvironmentDeckBefore - 2, numCardsInEnvironmentDeckAfter, "The number of cards in the environment deck don't match.");
            //should be 2 more cards in play, one for song of the deep, 1 for top card of deck
            Assert.AreEqual(numCardsInEnvironmentPlayBefore + 2, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play area don't match.");

        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnNoDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutOnDeck("AncientParasite");
            PutIntoPlay("Teryx");
            //Play song of the deep
            PutIntoPlay("SongOfTheDeep");

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            QuickHandCheck(1, 1, 1);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //since no creatures in play, should not be destroyed
            Assert.AreEqual(numCardsInEnvironmentPlayBefore, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play area don't match.");
        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnDestroyWithTeryx()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            PutOnDeck("BarnacleHydra");

            //Play song of the deep
            PutIntoPlay("SongOfTheDeep");
            PutIntoPlay("BarnacleHydra");
            PutIntoPlay("AncientParasite");
            PutIntoPlay("Teryx");

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            QuickHandCheck(1, 1, 1);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //since 2 creatures in play, 1 destroyed
            Assert.AreEqual(numCardsInEnvironmentPlayBefore - 1, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play area don't match.");
        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnWithoutTeryx()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutOnDeck("AncientParasite");
            //Play song of the deep
            PutIntoPlay("SongOfTheDeep");
            PutIntoPlay("BarnacleHydra");
            PutIntoPlay("AncientParasite");

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            //since teryx is not in play, nothing will be drawn
            QuickHandCheck(0, 0, 0);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //since no creatures in play, should not be destroyed
            Assert.AreEqual(numCardsInEnvironmentPlayBefore, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play area don't match.");
        }

        [Test()]
        public void TestSubmergePlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("Submerge");

            //teryx should now be in play
            Assert.IsTrue(this.IsTeryxInPlay(isle), "Teryx is not in play");

        }

        [Test()]
        public void TestSubmergeReduceDamage()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("Submerge");
            //Reduce all damage dealt by 2

            QuickHPStorage(ra);
            DealDamage(baron, ra, 5, DamageType.Lightning);
            //since 5 damage dealt - 2 = should be 3 less now
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestSubmergeStartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutIntoPlay("Submerge");
            //At the start of the environment turn, this card is destroyed.

            int numCardsInPlayBefore = GetNumberOfCardsInPlay(isle);
            GoToStartOfTurn(isle);
            int numCardsInPlayAfter = GetNumberOfCardsInPlay(isle);

            //Submerge should have destroyed itself so 1 fewer env cards in play
            Assert.AreEqual(numCardsInPlayBefore - 1, numCardsInPlayAfter, "The number of environment cards in play don't match");
        }
        [Test()]
        public void TestThroughTheHurricaneTargetIsPlayed()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(visionary.CharacterCard, 18);
            SetHitPoints(haka.CharacterCard, 28);

           // Whenever a target enters play, this card deals { H - 1}lightning damage to the target with the third highest HP.
           // 3rd highest hp is ra
           //H =3, 3-1 = 2
            PutIntoPlay("ThroughTheHurricane");

            QuickHPStorage(ra);
            PlayCard("DecoyProjection");
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestThroughTheHurricaneStartOfTurnPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            PutOnDeck("Teryx");
            PutOnDeck("AncientParasite");
            
            PutIntoPlay("ThroughTheHurricane");

            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            DecisionYesNo = true;
            int numCardsInEnvironmentDeckBefore = GetNumberOfCardsInDeck(isle);
            GoToStartOfTurn(isle);
            int numCardsInEnvironmentDeckAfter = GetNumberOfCardsInDeck(isle);

            //should be 2 cards played, so 2 fewer cards in deck
            Assert.AreEqual(numCardsInEnvironmentDeckBefore - 2, numCardsInEnvironmentDeckAfter, "The number of cards in the environment deck do not match.");
        }

        [Test()]
        public void TestThroughTheHurricaneStartOfTurnDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            PutOnDeck("Teryx");
            PutOnDeck("AncientParasite");

            PutIntoPlay("ThroughTheHurricane");

            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            DecisionYesNo = true;
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            GoToStartOfTurn(isle);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //should be 2 cards played, but this card destroyed, so 1 more
            Assert.AreEqual(numCardsInEnvironmentPlayBefore + 1, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play do not match.");
        }

        [Test()]
        public void TestTimedDetonatorPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck to reduce variance
            PutOnDeck("Teryx");

            //When this card enters play, play the top card of the environment deck.

            int numCardsInEnvironmentDeckBefore = GetNumberOfCardsInDeck(isle);
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            //Play timed detonator
            PutIntoPlay("TimedDetonator");

            int numCardsInEnvironmentDeckAfter = GetNumberOfCardsInDeck(isle);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);


            //should be 2 fewer cards in the deck, one for timed detonator, 1 for top card of the deck
            Assert.AreEqual(numCardsInEnvironmentDeckBefore - 2, numCardsInEnvironmentDeckAfter, "The number of cards in the environment deck don't match.");
            //should be 2 more cards in play, one for 1 for timed detonator, 1 for top card of deck
            Assert.AreEqual(numCardsInEnvironmentPlayBefore + 2, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play area don't match.");

        }
        [Test()]
        public void TestTimedDetonatorStartofTurnDamage()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            PutOnDeck("BarnacleHydra");

            //play teryx
            PutIntoPlay("Teryx");
            Card teryx = GetCardInPlay("Teryx");

            //Play timed detonator
            PutIntoPlay("TimedDetonator");

            //pause before environment to collect effects
            GoToEndOfTurn(haka);

            //At the start of the environment turn, this card deals Teryx 10 fire damage and each hero target {H - 2} fire damage. Then, this card is destroyed.
            //H = 3, so H-2 = 1
            QuickHPStorage(teryx, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard);
            GoToStartOfTurn(isle);
            QuickHPCheck(-10, -1, -1, -1);
        }

        [Test()]
        public void TestTimedDetonatorStartOfTurnDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            //stack deck for less variance
            PutOnDeck("BarnacleHydra");

            //play timed detonator
            PutIntoPlay("TimedDetonator");

            //At the start of the environment turn, this card deals Teryx 10 fire damage and each hero target {H - 2} fire damage. Then, this card is destroyed.
           
            int numCardsInEnvironmentPlayBefore = GetNumberOfCardsInPlay(isle);
            GoToStartOfTurn(isle);
            int numCardsInEnvironmentPlayAfter = GetNumberOfCardsInPlay(isle);

            //this card should be destroyed, so 1 less
            Assert.AreEqual(numCardsInEnvironmentPlayBefore - 1, numCardsInEnvironmentPlayAfter, "The number of cards in the environment play do not match.");
        }

    }
}

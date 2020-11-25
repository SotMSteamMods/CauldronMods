using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheWanderingIsleTests : BaseTest
    {

        #region TheWanderingIsleHelperFunctions

        protected TurnTakerController isle { get { return FindEnvironment(); } }
        protected HeroTurnTakerController stranger { get { return FindHero("TheStranger"); } }

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
        public void TestDecklist()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card card = GetCard("Teryx");
            AssertIsTarget(card, 50);
            AssertCardHasKeyword(card, "living island", false);

            card = GetCard("AncientParasite");
            AssertIsTarget(card, 20);
            AssertCardHasKeyword(card, "creature", false);

            card = GetCard("BarnacleHydra");
            AssertIsTarget(card, 6);
            AssertCardHasKeyword(card, "creature", false);

            card = GetCard("TimedDetonator");
            AssertIsTarget(card, 8);
            AssertCardHasKeyword(card, "device", false);

        }

        [Test()]
        public void TestTeryxIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            Card teryx = PutIntoPlay("Teryx");
            AssertInPlayArea(env, teryx);

            //teryx is indestructible, so shouldn't be destroyed
            DestroyCard(teryx, baron.CharacterCard);
            AssertInPlayArea(env, teryx);

            //but not immune to damage
            QuickHPStorage(teryx);
            DealDamage(baron.CharacterCard, teryx, 3, DamageType.Cold);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestTeryxGameOver()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            Card teryx = PutIntoPlay("Teryx");

            //If this card reaches 0HP, the heroes lose.
            DealDamage(baron.CharacterCard, teryx, 50, DamageType.Melee, true);

            AssertGameOver(EndingResult.EnvironmentDefeat);
        }

        [Test()]
        public void TestTeryxVillainDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            Card teryx = PutIntoPlay("Teryx");

            //At the end of the environment turn, the villain target with the highest HP deals Teryx {H + 2} energy damage.
            //H = 3, should be dealt 5 damage
            GoToStartOfTurn(isle);
            QuickHPStorage(teryx, baron.CharacterCard, ra.CharacterCard);
            GoToEndOfTurn(isle);
            QuickHPCheck(-5, 0, 0);
        }

        [Test()]
        public void TestTeryxHeroDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            Card teryx = PutIntoPlay("Teryx");

            //set hp lower so something to gain
            SetHitPoints(teryx, 30);

            //Whenever a hero target would deal damage to Teryx, Teryx Instead regains that much HP.
            QuickHPStorage(teryx, baron.CharacterCard, ra.CharacterCard);
            DealDamage(ra, teryx, 5, DamageType.Fire);
            QuickHPCheck(5, 0, 0);
        }

        [Test()]
        public void TestAmphibiousAssaultPlayWith2Villains()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            QuickHPStorage(baron, ra, fanatic, haka);

            //When this card enters play, the {H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            //H = 3, so 2 villain targets should deal damage
            //since we aren't selecting targets, it should default to ra, then fanatic being dealt damage
            var card = PutIntoPlay("AmphibiousAssault");
            AssertInPlayArea(isle, card);

            QuickHPCheck(0, -3, -3, 0);
        }

        [Test]
        public void TestAmphibiousAssaultPlayWith1Villains()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            //destroy mdp so there is only baron blade in play
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            QuickHPStorage(baron, ra, fanatic, haka);

            //however, there is only 1 villain target in play, so only 1 instance of damage
            //since we aren't selecting targets, it should default to ra
            var card = PutIntoPlay("AmphibiousAssault");
            AssertInPlayArea(isle, card);

            QuickHPCheck(0, -3, 0, 0);
        }

        [Test()]
        public void TestAmphibiousAssaultPlayWithNotEnoughHeros()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //incap fanatic and haka so there's not enough hero targets
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 99, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, haka.CharacterCard, 99, DamageType.Cold, true);

            QuickHPStorage(baron, ra);

            //When this card enters play, the {H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            //H = 3, so 2 villain targets should deal damage
            //since we aren't selecting targets, it should default to ra (only one available)
            var card = PutIntoPlay("AmphibiousAssault");
            AssertInPlayArea(isle, card);

            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestAmphibiousAssaultStartOfTurnNoHeroCardsPlayed()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack villain deck to not play hasten doom
            var topCard = PutOnDeck("MobileDefensePlatform");

            //don't play a hero card
            GoToPlayCardPhase(ra);

            GoToPlayCardPhase(haka);

            var card = PutIntoPlay("AmphibiousAssault");
            AssertInPlayArea(isle, card);
            AssertNotInPlay(topCard);

            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            GoToStartOfTurn(isle);

            AssertNotInPlay(topCard);
            AssertInTrash(card);
        }

        [Test()]
        public void TestAmphibiousAssaultStartOfTurnHeroCardsPlayed()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack villain deck to not play hasten doom
            var topCard = PutOnDeck("MobileDefensePlatform");

            //play a hero card
            GoToPlayCardPhase(ra);
            var random = GetCardFromHand(ra);
            PlayCard(random);
            GoToPlayCardPhase(haka);

            var card = PutIntoPlay("AmphibiousAssault");
            AssertInPlayArea(isle, card);
            AssertNotInPlay(topCard);

            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            GoToStartOfTurn(isle);

            AssertInPlayArea(baron, topCard);
            AssertInTrash(card);
        }

        [Test()]
        public void TestAmphibiousAssaultNotInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack villain deck to not play hasten doom
            var topCard = PutOnDeck("MobileDefensePlatform");

            //don't play a hero card
            GoToPlayCardPhase(ra);

            GoToPlayCardPhase(haka);
                        
            //Issue #109 - villain deck is playing a card at the beginning of the turn
            AssertNotInPlay(topCard);
            //AmphibiousAssult isn't in play, so the top card of the villain shouldn't be played
            
            GoToStartOfTurn(isle);

            //and still no
            AssertNotInPlay(topCard);
        }


        [Test()]
        public void TestAncientParasiteHeroDamageMoveCard()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            var mdp = GetCardInPlay("MobileDefensePlatform");

            Card parasite = PutIntoPlay("AncientParasite");
            AssertInPlayArea(isle, parasite);

            //Whenever this card is dealt damage by a hero target, move it next to that target.
            DealDamage(ra, parasite, 5, DamageType.Fire);
            AssertNextToCard(parasite, ra.CharacterCard);

            //still next too
            DealDamage(ra, parasite, 1, DamageType.Fire);
            AssertNextToCard(parasite, ra.CharacterCard);

            //different hero
            DealDamage(fanatic, parasite, 1, DamageType.Fire);
            AssertNextToCard(parasite, fanatic.CharacterCard);

            //doesn't move next to haka, still next to fanatic
            DealDamage(haka, mdp, 1, DamageType.Melee);
            AssertNextToCard(parasite, fanatic.CharacterCard);
        }

        [Test()]
        public void TestAncientParasiteVillainDamageDontMoveCard()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card parasite = PutIntoPlay("AncientParasite");
            AssertInPlayArea(isle, parasite);

            //Whenever this card is dealt damage by a hero target, move it next to that target.
            DealDamage(baron, parasite, 5, DamageType.Fire);
            //didn't move
            AssertInPlayArea(isle, parasite);
        }

        [Test()]
        public void TestAncientParasiteStartOfTurnNextToTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");
            Card parasite = PutIntoPlay("AncientParasite");

            //move next to ra
            DealDamage(ra, parasite, 5, DamageType.Fire);
            AssertNextToCard(parasite, ra.CharacterCard);

            //At the start of the environment turn, if this card is next to a target, it deals that target {H} toxic damage and moves back to the environment play area. 
            //H is 3, so 3 damage should be dealt
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, haka.CharacterCard, parasite, teryx);

            GoToStartOfTurn(isle);

            QuickHPCheck(0, -3, 0, 0, 0, 0);
            AssertNotNextToCard(parasite, ra.CharacterCard);
            AssertInPlayArea(isle, parasite);
        }

        [Test()]
        public void TestAncientParasiteStartOfTurnNotNextToTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Fanatic", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");
            Card parasite = PutIntoPlay("AncientParasite");
            AssertInPlayArea(isle, teryx);
            AssertInPlayArea(isle, parasite);

            //Otherwise it deals Teryx {H + 2} toxic damage.
            //H is 3, so 5 damage should be dealt
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, haka.CharacterCard, parasite, teryx);

            GoToStartOfTurn(isle);
            QuickHPCheck(0, 0, 0, 0, 0, -5);

            AssertInPlayArea(isle, teryx);
            AssertInPlayArea(isle, parasite);
        }

        [Test()]
        public void TestAncientParasiteCardNextToDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card decoy = PutIntoPlay("DecoyProjection");
            Card parasite = PutIntoPlay("AncientParasite");

            DealDamage(decoy, parasite, 5, DamageType.Fire);
            AssertNextToCard(parasite, decoy);

            DestroyCard(decoy);

            AssertInPlayArea(visionary, parasite);
        }

        [Test()]
        public void TestExposedLifeforcePlay_SearchFromDeck()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            QuickShuffleStorage(isle);
            //start with teryx in the deck
            Card teryx = PutOnDeck("Teryx");
            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("ExposedLifeforce");

            //teryx should now be in play
            AssertIsInPlay(teryx);

            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestExposedLifeforcePlay_SearchFromTrash()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            QuickShuffleStorage(isle);
            //start with teryx in trash
            Card teryx = PutInTrash("Teryx");
            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("ExposedLifeforce");

            //teryx should now be in play
            AssertIsInPlay(teryx);

            QuickShuffleCheck(1);
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

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            Card card = PutIntoPlay("ExposedLifeforce");
            AssertInPlayArea(isle, card);

            //Destroy this card if Teryx regains 10HP in a single round.
            GainHP(teryx, 10);

            AssertInTrash(card);
        }

        [Test()]
        public void TestExposedLifeforceDestroyWhen10_OverRound()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            Card card = PutIntoPlay("ExposedLifeforce");
            AssertInPlayArea(isle, card);

            //Destroy this card if Teryx regains 10HP in a single round.
            GoToStartOfTurn(ra);
            //5 now
            GainHP(teryx, 5);
            GoToNextTurn();
            //5 later
            GainHP(teryx, 5);

            AssertInTrash(card);
        }

        [Test()]
        public void TestExposedLifeforceDestroyWhen10_ResetsOverRoundBreak()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            Card card = PutIntoPlay("ExposedLifeforce");
            AssertInPlayArea(isle, card);

            //Destroy this card if Teryx regains 10HP in a single round.
            GoToStartOfTurn(ra);
            //5 now
            GainHP(teryx, 5);
            GoToStartOfTurn(baron);
            //5 later
            GainHP(teryx, 5);
            //because split over a round break, should not be destroyed
            AssertInPlayArea(isle, card);
        }

        [Test()]
        public void TestExposedLifeforceNoDestroyWhenLessThan10()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            Card card = PutIntoPlay("ExposedLifeforce");
            AssertInPlayArea(isle, card);

            //Destroy this card if Teryx regains 10HP in a single round.
            GainHP(teryx, 9);

            AssertInPlayArea(isle, card);
        }

        [Test()]
        public void TestBarnacleHydraDestroyedTeryxInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //put teryx into play
            Card teryx = PutIntoPlay("Teryx");
            AssertInPlayArea(isle, teryx);

            Card hydra = PutIntoPlay("BarnacleHydra");
            AssertInPlayArea(isle, hydra);

            //When this card is destroyed, it deals Teryx {H} toxic damage.
            //H=3, so 3 damage should be dealt
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, teryx);
            DestroyCard(hydra, baron.CharacterCard);
            QuickHPCheck(0, 0, 0, 0, -3);
        }

        [Test()]
        public void TestBarnacleHydraDestroyedNoTeryxInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card hydra = PutIntoPlay("BarnacleHydra");
            AssertInPlayArea(isle, hydra);

            //When this card is destroyed, it deals Teryx {H} toxic damage.
            //teryx is not in play so no damage should be dealt, essentially checking for no crash
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard);
            DestroyCard(hydra, baron.CharacterCard);
            QuickHPCheck(0, 0, 0, 0);

            AssertNotGameOver();
        }

        [Test()]
        public void TestBarnacleHydraEndOfTurnNoSubmerge()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card hydra = PutIntoPlay("BarnacleHydra");

            //set hitpoints so have room to gain things
            //this also checks to make sure that damage being dealt ignores hydra
            SetHitPoints(hydra, 1);
            SetHitPoints(haka, 5); //ensure haka is the lowest target

            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP
            //Submerge is not in play
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, hydra);
            GoToEndOfTurn(isle);
            QuickHPCheck(0, 0, 0, -2, 0);
        }

        [Test()]
        public void TestBarnacleHydraEndOfTurnSubmerge()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            GoToStartOfTurn(isle);

            Card hydra = PutIntoPlay("BarnacleHydra");
            //submerge reduces all damage by 2
            PutIntoPlay("Submerge");

            //set hitpoints so have room to gain things
            //this also checks to make sure that damage being dealt ignores hydra
            SetHitPoints(hydra, 1);
            SetHitPoints(haka, 5); //ensure haka is the lowest target

            //At the end of the environment turn, this card deals the non-environment target with the lowest HP 2 projectile damage. Then, if Submerge is in play, this card regains 6HP
            //Submerge is in play
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, hydra);
            GoToEndOfTurn(isle);
            //haka should not be dealt any damage because of submerge reduction
            QuickHPCheck(0, 0, 0, 0, 5);
            AssertIsAtMaxHP(hydra);
        }

        [Test()]
        public void TestIslandquakeDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            var card = PutIntoPlay("Islandquake");
            AssertInPlayArea(isle, card);

            //Then, this card is destroyed.
            GoToStartOfTurn(isle);
            AssertInTrash(card);
        }

        [Test()]
        public void TestIslandquakeStartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //destroy mdp so baron blade is not immune to damage
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            GoToPlayCardPhase(ra);
            //ra deals damage to teryx to cause hp gain and to be immune to islandquake damage
            DealDamage(ra.CharacterCard, teryx, 5, DamageType.Fire);

            var card = PutIntoPlay("Islandquake");
            AssertInPlayArea(isle, card);

            //At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, teryx);
            GoToStartOfTurn(isle);
            QuickHPCheck(-4, 0, -4, -4, 0);
        }

        [Test()]
        public void TestIslandquakeStartOfTurnRedirectForMultipleHits()
        {
            SetupGameController("BaronBlade", "Ra", "Tachyon", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //destroy mdp so baron blade is not immune to damage
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            GoToPlayCardPhase(ra);
            //ra deals damage to teryx to cause hp gain and to be immune to islandquake damage
            DealDamage(ra.CharacterCard, teryx, 5, DamageType.Fire);

            //tachyon has synaptic interruption in play
            PutIntoPlay("SynapticInterruption");

            var card = PutIntoPlay("Islandquake");
            AssertInPlayArea(isle, card);

            //tachyon uses synaptic interruption to redirect damage to ra, who is immune to islandquake damage
            DecisionRedirectTarget = ra.CharacterCard;

            //At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, tachyon.CharacterCard, haka.CharacterCard, teryx);
            GoToStartOfTurn(isle);

            //ra was immune to damage and tachyon redirected damage to ra
            //blade and haka were hit
            QuickHPCheck(-4, 0, 0, -4, 0);
        }

        [Test()]
        public void TestIslandquakeStartOfTurnHealTeryxDuringStartOfTurnTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Ra", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //destroy mdp so baron blade is not immune to damage
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            var card = PutIntoPlay("Islandquake");
            AssertInPlayArea(isle, card);

            //both the stranger and ra have a mark of the blood thorn attached
            GoToPlayCardPhase(stranger);
            DecisionSelectCard = ra.CharacterCard;
            Card rune1 = PutIntoPlay("MarkOfTheBloodThorn");
            AssertNextToCard(rune1, ra.CharacterCard);
            DecisionSelectCard = stranger.CharacterCard;
            Card rune2 = PutIntoPlay("MarkOfTheBloodThorn");
            AssertNextToCard(rune2, stranger.CharacterCard);

            //the stranger uses 2x mark of the blood thorn to heal teryx when ra is damaged
            base.ResetDecisions();
            DecisionSelectTargets = new[] {
                //islandquake targets ra
                ra.CharacterCard,
                //ra targets the stranger, via blood thorn
                stranger.CharacterCard,
                //the stranger targets teryx, via blood thorn
                teryx,
                //islandquake targets the stranger
                stranger.CharacterCard,
                //islandquake targets the remaining targets
                baron.CharacterCard,
                haka.CharacterCard
            };

            //At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            QuickHPStorage(baron.CharacterCard, stranger.CharacterCard, ra.CharacterCard, haka.CharacterCard, teryx);
            GoToStartOfTurn(isle);

            //stranger was immune, but took 1 damage from blood thorn
            //blade ra, and haka were hit
            //teryx healed from mainstay
            QuickHPCheck(-4, -1, -4, -4, 1);
        }

        [Test()]
        [Ignore("TODO:issues#199")]
        public void TestIslandquakeStartOfTurnHealTeryxDuringDamage()
        {
            SetupGameController("BaronBlade", "VoidGuardMainstay", "Ra", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //destroy mdp so baron blade is not immune to damage
            DestroyCard(GetCardInPlay("MobileDefensePlatform"));

            Card teryx = PutIntoPlay("Teryx");

            //set hitpoints so there is room to gain
            SetHitPoints(teryx, 30);

            //mainstay has preemptive payback in play
            PutIntoPlay("PreemptivePayback");

            var card = PutIntoPlay("Islandquake");
            AssertInPlayArea(isle, card);

            //mainstay uses preemptive payback to heal teryx before being hit
            //yes, destroy preemptive payback
            DecisionYesNo = true;
            DecisionSelectTargets = new[] {
                //islandquake targets mainstay
                voidMainstay.CharacterCard,
                //mainstay preemptively targets teryx, via preemptive payback
                teryx,
                //islandquake targets the remaining targets
                baron.CharacterCard,
                ra.CharacterCard,
                haka.CharacterCard
            };

            //At the start of the environment turn, this card deals each target other than Teryx 4 sonic damage. Hero targets which caused Teryx to regain HP since the end of the last environment turn are immune to this damage.
            QuickHPStorage(baron.CharacterCard, voidMainstay.CharacterCard, ra.CharacterCard, haka.CharacterCard, teryx);
            GoToStartOfTurn(isle);

            //mainstay was immune
            //blade ra, and haka were hit
            //teryx healed from mainstay
            QuickHPCheck(-4, 0, -4, -4, 3);
        }

        [Test()]
        public void TestSongOfTheDeepPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck to reduce variance
            var teryx = PutOnDeck("Teryx");

            //When this card enters play, play the top card of the environment deck.

            //Play song of the deep
            var card = PutIntoPlay("SongOfTheDeep");
            AssertInPlayArea(isle, card);
            AssertInPlayArea(isle, teryx);
        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnNoDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutOnDeck("AncientParasite");
            PutIntoPlay("Teryx");
            //Play song of the deep
            var card = PutIntoPlay("SongOfTheDeep");
            AssertInPlayArea(isle, card);

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);

            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            QuickHandCheck(1, 1, 1);

            //since only 1 creature in play, should not be destroyed
            AssertInPlayArea(isle, card);
        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnDestroyWithTeryx()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            PutOnDeck("BarnacleHydra");

            //Play song of the deep
            var card = PutIntoPlay("SongOfTheDeep");
            PutIntoPlay("BarnacleHydra");
            PutIntoPlay("AncientParasite");
            PutIntoPlay("Teryx");
            AssertInPlayArea(isle, card);

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);            
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);

            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            //since 2 creatures in play, song of the deep destroyed
            QuickHandCheck(1, 1, 1);
            AssertNotInPlay(card);
            AssertInTrash(card);
        }

        [Test()]
        public void TestSongOfTheDeepStartOfTurnWithoutTeryx()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            PutOnDeck("AncientParasite");
            //Play song of the deep
            var card = PutIntoPlay("SongOfTheDeep");
            PutIntoPlay("BarnacleHydra");
            PutIntoPlay("AncientParasite");
            AssertInPlayArea(isle, card);

            //collect the appropriate values for all hands
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Teryx is in play, each player may draw a card. Then, if there are at least 2 creatures in play, destroy this card.
            QuickHandStorage(ra, visionary, haka);

            //setting all players to draw a card
            DecisionYesNo = true;
            GoToStartOfTurn(isle);
            //since teryx is not in play, nothing will be drawn
            QuickHandCheck(0, 0, 0);

            //since no creatures in play, should not be destroyed
            AssertInPlayArea(isle, card);
        }

        [Test()]
        public void TestSubmergePlay_SearchFromDeck()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            QuickShuffleStorage(isle);
            Card teryx = PutOnDeck("Teryx");

            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("Submerge");

            //teryx should now be in play
            AssertIsInPlay(teryx);

            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestSubmergePlay_SearchFromTrash()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            QuickShuffleStorage(isle);
            Card teryx = PutInTrash("Teryx");

            // When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            PutIntoPlay("Submerge");

            //teryx should now be in play
            AssertIsInPlay(teryx);

            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestSubmergeReduceDamage()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            var card = PutIntoPlay("Submerge");
            AssertInPlayArea(isle, card);
            //Reduce all damage dealt by 2

            //check villain damage
            QuickHPStorage(ra);
            DealDamage(baron, ra, 5, DamageType.Lightning);
            //since 5 damage dealt - 2 = should be 3 less now
            QuickHPCheck(-3);

            //check hero damage
            QuickHPStorage(ra);
            DealDamage(haka, ra, 5, DamageType.Melee);
            //since 5 damage dealt - 2 = should be 3 less now
            QuickHPCheck(-3);


        }

        [Test()]
        public void TestSubmergeStartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            var card = PutIntoPlay("Submerge");
            AssertInPlayArea(isle, card);

            //At the start of the environment turn, this card is destroyed.

            GoToStartOfTurn(isle);

            //Submerge should have destroyed itself so 1 fewer env cards in play
            AssertInTrash(card);
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
            var card = PutIntoPlay("ThroughTheHurricane");
            AssertInPlayArea(isle, card);

            QuickHPStorage(baron, ra, visionary, haka);
            PlayCard("DecoyProjection");
            QuickHPCheck(0, -2, 0, 0);
        }

        [Test()]
        public void TestThroughTheHurricaneStartOfTurnPlayCards()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            var c1 = PutOnDeck("Teryx");
            var c2 = PutOnDeck("AncientParasite");

            var card = PutIntoPlay("ThroughTheHurricane");
            AssertInPlayArea(isle, card);

            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            DecisionYesNo = true;

            GoToStartOfTurn(isle);
            //assert stacked cards got played
            AssertInPlayArea(isle, c1);
            AssertInPlayArea(isle, c2);

            //and then this card was destroyed
            AssertInTrash(card);
        }

        [Test()]
        public void TestThroughTheHurricaneStartOfTurnNoPlayCard()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            var c1 = PutOnDeck("Teryx");
            var c2 = PutOnDeck("AncientParasite");

            var card = PutIntoPlay("ThroughTheHurricane");
            AssertInPlayArea(isle, card);

            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            DecisionYesNo = false;

            GoToStartOfTurn(isle);

            //stacked cards are not played
            AssertInDeck(c1);
            AssertInDeck(c2);

            //card not destroyed
            AssertInPlayArea(isle, card);
        }

        [Test()]
        public void TestTimedDetonatorPlay()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck to reduce variance
            var c1 = PutOnDeck("Teryx");

            //Play timed detonator
            var card = PutIntoPlay("TimedDetonator");
            AssertInPlayArea(isle, card);

            //When this card enters play, play the top card of the environment deck.
            AssertInPlayArea(isle, c1);
        }

        [Test()]
        public void TestTimedDetonatorStartofTurnDamage()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();

            //stack deck for less variance
            var c1 = PutOnDeck("BarnacleHydra");

            //play teryx
            var teryx = PutIntoPlay("Teryx");

            //Play timed detonator
            var card = PutIntoPlay("TimedDetonator");
            AssertInPlayArea(isle, card);
            AssertInPlayArea(isle, c1);

            //pause before environment to collect effects
            GoToEndOfTurn(haka);

            //At the start of the environment turn, this card deals Teryx 10 fire damage and each hero target {H - 2} fire damage. Then, this card is destroyed.
            //H = 3, so H-2 = 1
            QuickHPStorage(teryx, ra.CharacterCard, visionary.CharacterCard, haka.CharacterCard, c1);
            GoToStartOfTurn(isle);
            QuickHPCheck(-10, -1, -1, -1, 0);
        }

        [Test()]
        public void TestTimedDetonatorStartOfTurnDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "TheVisionary", "Haka", "Cauldron.TheWanderingIsle");
            StartGame();
            //stack deck for less variance
            var c1 = PutOnDeck("BarnacleHydra");

            //play timed detonator
            var card = PutIntoPlay("TimedDetonator");

            //At the start of the environment turn, this card deals Teryx 10 fire damage and each hero target {H - 2} fire damage. Then, this card is destroyed.

            GoToStartOfTurn(isle);

            AssertInTrash(card);
        }
    }
}

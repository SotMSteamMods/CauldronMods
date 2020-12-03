using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gray;
using System.Linq;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    class GrayTests : BaseTest
    {
        protected TurnTakerController gray { get { return FindVillain("Gray"); } }

        private void AssertCard(string identifier, string[] keywords = null, int hitpoints = 0)
        {
            Card card = GetCard(identifier);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    AssertCardHasKeyword(card, keyword, false);
                }
            }
            if (hitpoints > 0)
            {
                AssertMaximumHitPoints(card, hitpoints);
            }
        }

        protected void AddCannotDealTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        [Test()]
        public void TestGrayLoads()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(gray);

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gray);
            Assert.IsInstanceOf(typeof(GrayCharacterCardController), gray.CharacterCardController);

            Assert.AreEqual(75, gray.CharacterCard.HitPoints);
            AssertInPlayArea(gray, GetCardInPlay("ChainReaction"));
        }

        [Test()]
        public void TestGrayDeckList()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");

            AssertCardHasKeyword(gray.CharacterCard, "villain", false);

            AssertCard("AlistairWinters", new string[] { "minion" }, 5);
            AssertCard("BlightTheLand", new string[] { "radiation" }, 8);
            AssertCard("ChainReaction", new string[] { "radiation" }, 3);
            AssertCard("Contamination", new string[] { "ongoing" });
            AssertCard("CriticalMass", new string[] { "one-shot" });
            AssertCard("HeavyRadiation", new string[] { "ongoing" });
            AssertCard("IrradiatedTouch", new string[] { "radiation" }, 6);
            AssertCard("LivingReactor", new string[] { "ongoing" });
            AssertCard("MutatedWildlife", new string[] { "radiation" }, 6);
            AssertCard("NuclearFire", new string[] { "one-shot" });
            AssertCard("RadioactiveCascade", new string[] { "radiation" });
            AssertCard("UnstableIsotope", new string[] { "one-shot" });
            AssertCard("UnwittingHenchmen", new string[] { "minion" }, 5);
        }

        [Test()]
        public void TestGrayFrontFlip0Environment()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            //Flip on 3 Radiation cards in play at end of turn
            //Ensuring flipping with no environment to destroy doesn't fail
            PlayCards(GetCard("BlightTheLand", 0), GetCard("BlightTheLand", 1));
            AssertNotFlipped(gray);
            GoToEndOfTurn(gray);
            AssertFlipped(gray);
        }

        [Test()]
        public void TestGrayFrontFlip1Environment()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            //Flip on 3 Radiation cards in play at end of turn
            //Ensuring an environemnt card is destroyed
            PlayCard("PoliceBackup");
            PlayCards(GetCard("BlightTheLand", 0), GetCard("BlightTheLand", 1));
            AssertNotFlipped(gray);
            GoToEndOfTurn(gray);
            AssertFlipped(gray);
            AssertInTrash("PoliceBackup");
        }

        [Test()]
        public void TestGrayFrontFlip2Environment()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            //Flip on 3 Radiation cards in play at end of turn
            //Ensuring 1 environemnt card is destroyed
            PlayCards(new string[] { "PoliceBackup", "PlummetingMonorail" });
            PlayCards(GetCard("BlightTheLand", 0), GetCard("BlightTheLand", 1));
            AssertNotFlipped(gray);
            GoToEndOfTurn(gray);
            AssertFlipped(gray);
            AssertIsInPlay("PoliceBackup");
            AssertInTrash("PlummetingMonorail");
        }

        [Test()]
        public void TestGrayBackFlip()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            //Flip on 1 or fewer radiation cards in play at start of turn
            GoToEndOfTurn(env);
            FlipCard(gray);
            AssertFlipped(gray);
            GoToStartOfTurn(gray);
            AssertNotFlipped(gray);
        }

        [Test()]
        public void TestGrayFrontEndOfTurn()
        {
            SetupGameController("Cauldron.Gray", "Parse", "Haka", "Guise", "Megalopolis");
            StartGame();
            //At the end of the villain turn, {Gray} deals the hero target with the highest HP {H - 1} energy damage.
            QuickHPStorage(gray, parse, haka, guise);
            GoToEndOfTurn(gray);
            QuickHPCheck(0, 0, -2, 0);
        }

        [Test()]
        public void TestGrayFrontRadiationNoCardsToDestroy()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);
            //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail);
            DestroyCard("ChainReaction");
            QuickHPCheck(-2, -2, -2, -2);

        }

        [Test()]
        public void TestGrayFrontRadiationDestroy1Card()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);
            PlayCards(new string[] { "TaMoko", "Mere" });
            //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
            QuickHPStorage(gray.CharacterCard, legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail);
            DestroyCard("ChainReaction");
            //Ta Moko is in play so Haka takes 1 less
            QuickHPCheck(0, -2, -1, -2, -2);
            AssertInTrash("Mere");
            AssertIsInPlay("TaMoko");
        }

        [Test()]
        public void TestGrayFrontRadiationDestroy1Ongoing()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);
            PlayCards(new string[] { "TaMoko", "Mere" });
            //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
            QuickHPStorage(gray.CharacterCard, legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail);
            DecisionSelectCards = new Card[] { GetCardInPlay("TaMoko"), legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail };
            DestroyCard("ChainReaction");
            QuickHPCheck(0, -2, -2, -2, -2);
            AssertInTrash("TaMoko");
            AssertIsInPlay("Mere");
        }

        [Test()]
        public void TestGrayFrontAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            StartGame();
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);
            PlayCards(new string[] { "TaMoko", "Mere" });
            //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail);
            DestroyCard("ChainReaction");
            QuickHPCheck(-2, -2, -2, -2);
            AssertInTrash("TaMoko");
            AssertInTrash("Mere");
        }

        [Test()]
        public void TestGrayFrontAdvanced1CardToDestroy()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            StartGame();
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);
            PlayCard("TaMoko");
            //Whenever a radiation card is destroyed, destroy 1 hero ongoing or equipment card and gray deals each non-villain target {H - 1} energy damage.
            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, monorail);
            DestroyCard("ChainReaction");
            QuickHPCheck(-2, -2, -2, -2);
            AssertInTrash("TaMoko");
        }

        [Test()]
        public void TestGrayBackStartOfTurn()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card ali = GetCard("AlistairWinters");
            PutOnDeck(gray, ali);
            PlayCards(GetCard("BlightTheLand", 0), GetCard("BlightTheLand", 1));
            GoToEndOfTurn(env);
            DealDamage(legacy, GetCardInPlay("ChainReaction"), 3, DamageType.Energy);
            IEnumerable<Card> trash = GetCards("Fortitude", "Mere");
            IEnumerable<Card> play = GetCards("TaMoko", "BlazingTornado");
            PlayCards(play);
            PlayCards(trash);
            //At the start of the villain turn, destroy all but 2 hero ongoing or equipment cards. {Gray} deals each hero target {H x 2} energy damage. Play the top card of the villain deck.
            QuickHPStorage(legacy, haka, ra);
            GoToStartOfTurn(gray);
            //Ta Moko is in play for -1 damage
            QuickHPCheck(-6, -5, -6);
            AssertInTrash(trash);
            AssertIsInPlay(play);
            AssertIsInPlay(ali);
        }

        [Test()]
        public void TestGrayBackDestroyRadiationEnvironmentDestroys()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            FlipCard(gray);
            Card roach = GetCard("GiantMutatedSpecimen");
            PlayCard(roach);
            //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
            //Not a hero source, no damage
            QuickHPStorage(legacy, ra, haka);
            DealDamage(roach, GetCardInPlay("ChainReaction"), 3, DamageType.Melee);
            AssertInTrash("ChainReaction");
            QuickHPCheckZero();
        }

        [Test()]
        public void TestGrayBackDestroyRadiationHeroDestroys()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            FlipCard(gray);
            //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
            QuickHPStorage(legacy, ra, haka);
            DealDamage(legacy, GetCardInPlay("ChainReaction"), 3, DamageType.Melee);
            AssertInTrash("ChainReaction");
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestGrayBackDestroyRadiationHeroCardDestroys()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Unity", "Ra", "TimeCataclysm" });
            StartGame();
            var reaction = GetCardInPlay("ChainReaction");
            FlipCard(gray);
            Card bot = GetCard("RaptorBot");
            PlayCard(bot);
            //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
            QuickHPStorage(legacy, ra, unity);
            DealDamage(bot, reaction, 3, DamageType.Melee);
            AssertInTrash(reaction);
            QuickHPCheck(0, 0, -2);

            //reset and test a destroy effect
            PlayCard(reaction);
            QuickHPStorage(legacy, ra, unity);
            DestroyCard(reaction, bot);
            AssertInTrash(reaction);
            QuickHPCheck(0, 0, -2);
        }

        [Test()]
        public void TestGrayBackDestroyRadioactiveCascade()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            FlipCard(gray);
            Card roach = GetCard("GiantMutatedSpecimen");
            PlayCard(roach);
            PlayCard("RadioactiveCascade");
            //Whenever a radiation card is destroyed by a hero card, {Gray} deals that hero {H - 1} energy damage.
            //Not a hero source, no damage
            QuickHPStorage(legacy, ra, haka);
            DealDamage(roach, GetCardInPlay("ChainReaction"), 3, DamageType.Melee);
            AssertInTrash("ChainReaction");
            AssertInTrash("RadioactiveCascade");
            QuickHPCheck(-2, 0, 0);
        }

        [Test()]
        public void TestGrayBackAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis" }, true);
            StartGame();
            FlipCard(gray);
            Card ali = GetCard("AlistairWinters");
            PlayCard(ali);
            //Advanced - Reduce damage dealt to villain targets by 1.
            QuickHPStorage(gray.CharacterCard, ali, haka.CharacterCard);
            DealDamage(legacy, gray, 2, DamageType.Melee);
            DealDamage(ra, ali, 2, DamageType.Melee);
            DealDamage(ra, haka, 2, DamageType.Fire);
            QuickHPCheck(-1, -1, -3);

        }

        [Test()]
        public void TestAlistairWintersHeroTargetEffects()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card roach = GetCard("GiantMutatedSpecimen");
            Card ali = GetCard("AlistairWinters");
            PlayCards(ali, roach);
            QuickHPStorage(legacy, haka, ra);
            //Increase damage dealt to hero targets by 1.
            DealDamage(gray, legacy, 2, DamageType.Melee);
            DealDamage(ali, haka, 2, DamageType.Melee);
            DealDamage(roach, ra, 2, DamageType.Melee);
            //Hero tagets cannot gain HP.
            PlayCard("InspiringPresence");
            QuickHPCheck(-3, -3, -3);
        }

        [Test()]
        public void TestAlistairWintersNonHeroTargetEffects()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card roach = GetCard("GiantMutatedSpecimen");
            Card ali = GetCard("AlistairWinters");
            Card chain = GetCardInPlay("ChainReaction");
            PlayCards(ali, roach);
            QuickHPStorage(gray.CharacterCard, chain, roach);
            //Increase damage dealt to hero targets by 1.
            DealDamage(legacy, gray, 2, DamageType.Melee);
            DealDamage(haka, chain, 2, DamageType.Melee);
            DealDamage(ra, roach, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, -2);
            //Hero tagets cannot gain HP.
            QuickHPUpdate();
            PlayCard("BlightTheLand");
            GoToEndOfTurn(gray);
            //BlightTheLand deals 2 to non-villain but heals radiation cards 2
            QuickHPCheck(0, 2, -2);
        }

        [Test()]
        public void TestBlightTheLand()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card blight = GetCard("BlightTheLand");
            Card chain = GetCardInPlay("ChainReaction");
            AddCannotDealTrigger(gray, chain);
            AddCannotDealTrigger(gray, gray.CharacterCard);
            PlayCard(blight);
            DealDamage(legacy, blight, 2, DamageType.Melee);
            DealDamage(haka, chain, 2, DamageType.Melee);
            //At the end of the villain turn, this card deals each non-villain target 2 toxic damage and each Radiation card regains 2HP.
            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, blight, chain);
            GoToEndOfTurn(gray);
            QuickHPCheck(-2, -2, -2, 2, 2);
        }

        [Test()]
        public void TestChainReaction() //This does not affect a dynamic number of targets because the DealDamageToLowestHP does not accept a dynamic number of targets
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Parse", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            //The game starts with a Chain Reaction in play
            Card touch = GetCard("IrradiatedTouch");
            PutInTrash(touch);
            //At the end of the villain turn, put a random Radiation card from the villain trash into play.
            GoToEndOfTurn(gray);
            AssertIsInPlay(touch);
            //At the start of the villain turn, this card deals the X hero targets with the lowest HP 1 energy damage each, where X is the number of Radiation cards in play.
            //Chain Reaction and Irradiated Touch makes 2
            AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("radiation"), 2);
            QuickHPStorage(parse, ra, haka);
            GoToStartOfTurn(gray);
            QuickHPCheck(-1, -1, 0);
        }

        [Test()]
        public void TestContamination()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card contamination = GetCard("Contamination");
            Card blight = GetCard("BlightTheLand");
            Card ring = GetCard("TheLegacyRing");
            Card moko = GetCard("TaMoko");
            Card mere = GetCard("Mere");
            //Whenever a hero deals damage to a villain target, that hero must destroy 1 of their ongoing or equipment cards.
            PlayCards(contamination, blight, ring, mere, moko);
            DealDamage(legacy, gray, 2, DamageType.Melee);
            AssertInTrash(ring);
            AssertIsInPlay(contamination);
            //Ra has none, so none are destroyed
            DealDamage(ra, gray, 2, DamageType.Melee);
            AssertIsInPlay(mere, moko, contamination);
            //check that it is to all villain targets
            DealDamage(haka.CharacterCard, blight, 2, DamageType.Melee);
            AssertInTrash(mere);
            AssertIsInPlay(contamination);
            DealDamage(haka, gray, 2, DamageType.Melee);
            //Destroy this card when {H} hero cards are destroyed this way in one round.
            AssertInTrash(mere, contamination);
        }

        [Test()]
        public void TestCriticalMass()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PutInTrash("ChainReaction");
            Card iso = GetCard("UnstableIsotope");
            PutInTrash(iso);
            //Search the villain deck and trash for all copies of Chain Reaction and put them into play. Move 1 copy of Unstable Isotope from the villain trash to the villain deck. Shuffle the villain deck.
            //{Gray} deals himself 2 energy damage.
            QuickHPStorage(gray, legacy, haka, ra);
            QuickShuffleStorage(gray);
            PlayCard("CriticalMass");
            QuickHPCheck(-2, 0, 0, 0);
            QuickShuffleCheck(1);
            AssertIsInPlay("ChainReaction", 3);
            AssertInDeck(iso);
        }

        [Test()]
        public void TestHeavyRadiation1RadCard()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PutOnDeck("Contamination");
            PlayCard("HeavyRadiation");
            //Reduce damage dealt to {Gray} by 1 for each Radiation card in play.
            QuickHPStorage(gray);
            DealDamage(ra, gray, 2, DamageType.Melee);
            QuickHPCheck(-1);
            //Ensure no Radiations in play
            DealDamage(ra, GetCardInPlay("ChainReaction"), 3, DamageType.Melee);
            //At the end of the villain turn, if there are no Radiation cards in play, play the top card of the villain deck.
            GoToEndOfTurn(gray);
            AssertIsInPlay("Contamination");
        }

        [Test()]
        public void TestHeavyRadiation3RadCard()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCards((Card c) => c.Identifier == "ChainReaction");
            PlayCard("HeavyRadiation");
            //Reduce damage dealt to {Gray} by 1 for each Radiation card in play.
            QuickHPStorage(gray);
            DealDamage(ra, gray, 4, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIrradiatedTouch()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("IrradiatedTouch");
            DealDamage(ra, GetCardInPlay("ChainReaction"), 4, DamageType.Melee);
            //At the end of the villain turn, {Gray} deals the hero target with the second highest HP {H - 2} melee and {H - 2} energy damage.
            //Gray deals highest target (Haka) H-1 damage
            QuickHPStorage(legacy, gray, haka, ra);
            GoToEndOfTurn(gray);
            QuickHPCheck(-2, 0, -2, 0);
            //When this card is destroyed, {Gray} deals the hero target with the highest HP 2 energy damage.
            QuickHPUpdate();
            DestroyCard("IrradiatedTouch");
            //Gray deals everyone 2 energy damage when a radiation card is destroyed
            QuickHPCheck(-2, 0, -4, -2);
        }

        [Test()]
        public void TestLivingReactorCardsInHandIncrease()
        {
            //Whenever {Gray} deals damage to a hero target, either increase that damage by 1 or that player must discard a card.
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("LivingReactor");
            //Increase damage
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            QuickHPStorage(ra);
            QuickHandStorage(ra);
            DealDamage(gray, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestLivingReactorCardsInHandDiscard()
        {
            //Whenever {Gray} deals damage to a hero target, either increase that damage by 1 or that player must discard a card.
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("LivingReactor");
            //Discard card
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            DealDamage(gray, ra, 2, DamageType.Melee);
            QuickHPCheck(-2);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestLivingReactorEmptyHand()
        {
            //Whenever {Gray} deals damage to a hero target, either increase that damage by 1 or that player must discard a card.
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("LivingReactor");
            DiscardAllCards(ra);
            //Increase damage
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            DealDamage(gray, ra, 2, DamageType.Melee);
            PrintJournal();
            QuickHPCheck(-3);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestLivingReactorImmuneToDamage()
        {
            //Whenever {Gray} deals damage to a hero target, either increase that damage by 1 or that player must discard a card.
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("LivingReactor");
            Card evo = PlayCard("NextEvolution");
            UsePower(evo);

            //Since Legacy is immune to the damage he should not have to discard a card or increase the damage
            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            DealDamage(gray, legacy, 2, DamageType.Cold);
            QuickHPCheck(0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestMutatedWildlife()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Parse", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card roach = GetCard("GiantMutatedSpecimen");
            PutOnDeck(env, roach);
            PlayCard("MutatedWildlife");
            //At the end of the villain turn, play the top card of the environment deck.
            GoToEndOfTurn(gray);
            AssertIsInPlay(roach);
            //Increase damage dealt by environment cards by 1. 
            QuickHPStorage(ra);
            DealDamage(roach, ra, 2, DamageType.Melee);
            QuickHPCheck(-3);
            //Whenever a villain target would be dealt damage by an environment card, redirect that damage to the hero target with the highest HP.
            QuickHPStorage(haka, gray);
            DealDamage(roach, gray, 2, DamageType.Melee);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestNuclearFire()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            //{Gray} deals the 2 hero targets with the highest HP {H - 1} energy damage each.
            //{Gray} deals the 2 hero targets with the lowest HP {H - 2} fire damage each.
            QuickHPStorage(gray, legacy, haka, ra);
            PlayCard("NuclearFire");
            QuickHPCheck(0, -3, -2, -1);
        }

        [Test()]
        public void TestRadioactiveCascade()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            //When this card enters play, {Gray} deals the hero with the highest HP X energy damage, where X is 2 plus the number of Radiation cards in play.",
            //X=2, radioactive cascade and chain reaction
            QuickHPStorage(haka);
            PlayCard("RadioactiveCascade");
            QuickHPCheck(-4);
            //When another villain card is destroyed, destroy this card.
            DealDamage(haka, GetCardInPlay("ChainReaction"), 3, DamageType.Cold);
            AssertInTrash("RadioactiveCascade");
        }

        [Test()]
        public void TestUnstableIsotope()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            Card mutie = GetCard("MutatedWildlife", 0);
            Card mutie2 = GetCard("MutatedWildlife", 1);
            Card fire = GetCard("NuclearFire");
            PutOnDeck(gray, mutie);
            PutOnDeck(gray, mutie2);
            PutOnDeck(gray, fire);
            //Reveal cards from the top of the villain deck until 2 Radiation cards are revealed. Put those cards into play and discard the rest.
            PlayCard("UnstableIsotope");
            AssertIsInPlay(mutie, mutie2);
            AssertInTrash(fire);
            AssertNumberOfCardsInRevealed(gray, 0);
        }

        [Test()]
        public void TestUnwittingHenchmenDestroyEquipment()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCards("Mere", "TheLegacyRing", "UnwittingHenchmen");
            DealDamage(legacy, gray, 5, DamageType.Cold);
            AddCannotDealTrigger(gray, gray.CharacterCard);
            //At the end of the villain turn, destroy 1 equipment card.
            //If a card is destroyed this way, {Gray} regains 3 HP. Otherwise this card deals the hero target with the highest HP 1 melee damage.
            QuickHPStorage(gray, haka);
            GoToEndOfTurn(gray);
            QuickHPCheck(3, 0);
            AssertInTrash("TheLegacyRing");
            AssertIsInPlay("Mere");
        }

        [Test()]
        public void TestUnwittingHenchmenNoDestroyEquipment()
        {
            SetupGameController(new string[] { "Cauldron.Gray", "Legacy", "Haka", "Ra", "TimeCataclysm" });
            StartGame();
            PlayCard("UnwittingHenchmen");
            DealDamage(legacy, gray, 5, DamageType.Cold);
            AddCannotDealTrigger(gray, gray.CharacterCard);
            //At the end of the villain turn, destroy 1 equipment card.
            //If a card is destroyed this way, {Gray} regains 3 HP. Otherwise this card deals the hero target with the highest HP 1 melee damage.
            QuickHPStorage(gray, haka);
            GoToEndOfTurn(gray);
            QuickHPCheck(0, -1);
        }
    }
}

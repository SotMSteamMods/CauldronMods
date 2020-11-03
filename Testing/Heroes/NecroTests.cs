using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Necro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyModTest
{
    [TestFixture()]
    public class NecroTests : BaseTest
    {
        #region NecroHelperFunctions
        protected HeroTurnTakerController necro { get { return FindHero("Necro"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(necro.CharacterCard, 1);
            DealDamage(villain, necro, 2, DamageType.Melee);
        }
        protected void AssertNumberOfUndeadInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsUndead(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfRitualInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsRitual(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }
        private bool IsUndead(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
        }

        private bool IsRitual(Card card)
        {
            return card != null && this.GameController.DoesCardContainKeyword(card, "ritual", false, false);
        }

        #endregion

        [Test()]
        public void TestNecroLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(necro);
            Assert.IsInstanceOf(typeof(NecroCharacterCardController), necro.CharacterCardController);

            Assert.AreEqual(24, necro.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestNecroInnatePowerOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");

            PlayCard(abomination);
            GoToUsePowerPhase(necro);
            DecisionSelectFunction = 0;
            DecisionSelectTarget = abomination;
            QuickHPStorage(abomination);
            UsePower(necro.CharacterCard);
            QuickHPCheck(-1);

        }
        
        [Test()]
        public void TestNecroInnatePowerOption2()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");


            PlayCard(abomination);

            GoToUsePowerPhase(necro);
            DecisionSelectFunction = 1;
            DecisionSelectTarget = abomination;
            QuickHPStorage(abomination);
            UsePower(necro.CharacterCard);
            QuickHPCheck(-2);

        }
        [Test()]
        public void TestNecroIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Legacy", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(necro);

            //One hero may deal himself 2 toxic damage to draw 2 cards now.
            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            //using incap ability on legacy
            DecisionSelectTarget = legacy.CharacterCard;
            //set to true so legacy will deal himself the damage
            DecisionsYesNo = new bool[] { true };

            GoToUseIncapacitatedAbilityPhase(necro);
            UseIncapacitatedAbility(necro, 0);

            //verify damage was dealt and cards were drawn
            QuickHPCheck(-2);
            QuickHandCheck(2);
        }

        [Test()]
        public void TestNecroIncap2_3CardDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Legacy", "Megalopolis");
            StartGame();
            SetHitPoints(legacy.CharacterCard, 20);

            SetupIncap(baron);
            AssertIncapacitated(necro);

            //Grab the first three cards to discard
            //One hero may discard up to 3 cards, then regain 2 HP for each card discarded.
            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            //using incap ability on legacy
            DecisionSelectTarget = legacy.CharacterCard;

            GoToUseIncapacitatedAbilityPhase(necro);
            UseIncapacitatedAbility(necro, 1);

            //verify damage was dealt and cards were drawn
            QuickHPCheck(6);
            QuickHandCheck(-3);
        }

           //once figure out a way to choose only 1 card or 2 cards to discard, add test cases for that

        [Test()]
        public void TestNecroIncap2_0CardDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Legacy", "Megalopolis");
            StartGame();
            SetHitPoints(legacy.CharacterCard, 20);

            SetupIncap(baron);
            AssertIncapacitated(necro);

            //One hero may discard up to 3 cards, then regain 2 HP for each card discarded.
            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            //using incap ability on legacy
            DecisionSelectTarget = legacy.CharacterCard;
            //setting legacy to discard 0 cards
            DecisionDoNotSelectCard = SelectionType.DiscardCard;

            GoToUseIncapacitatedAbilityPhase(necro);
            UseIncapacitatedAbility(necro, 1);

            //verify damage was dealt and cards were drawn
            QuickHPCheck(0);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestNecroIncap3()
        {
            SetupGameController("Omnitron", "Cauldron.Necro", "Legacy", "Megalopolis");
            StartGame();
            SetHitPoints(legacy.CharacterCard, 20);
            SetupIncap(omnitron);
            AssertIncapacitated(necro);

            //Destroy all non-character cards in play to reduce variance
            GoToStartOfTurn(necro);
            DestroyCards((Card c) => c.IsInPlayAndHasGameText && !c.IsCharacter);



            //Select a hero target. Increase damage dealt by that target by 3 and increase damage dealt to that target by 2 until the start of your next turn.
            //using incap ability on legacy
            DecisionSelectTarget = legacy.CharacterCard;
            GoToUseIncapacitatedAbilityPhase(necro);
            DestroyCards((Card c) => c.IsInPlayAndHasGameText && !c.IsCharacter);
            UseIncapacitatedAbility(necro, 2);

            GoToPlayCardPhase(legacy);

            //try legacy dealing damage to omnitron, should be +3
            QuickHPStorage(omnitron);
            DealDamage(legacy, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-5);

            //try omnitron dealing damage to legacy, should be +2
            QuickHPStorage(legacy);
            DealDamage(omnitron, legacy, 2, DamageType.Projectile);
            QuickHPCheck(-4);

            //go to necro's next turn, should be normal damage
            GoToUseIncapacitatedAbilityPhase(necro);

            QuickHPStorage(omnitron);
            DealDamage(legacy, omnitron, 2, DamageType.Melee);
            QuickHPCheck(-2);

            QuickHPStorage(legacy);
            DealDamage(omnitron, legacy, 2, DamageType.Projectile);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestDarkPactPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card darkPact = GetCard("DarkPact");
            PutInHand("Ghoul");
            GoToPlayCardPhase(necro);
            PutIntoPlay("DarkPact");
            GoToUsePowerPhase(necro);

            QuickHandStorage(necro);
            //Put an undead card from hand into play.
            UsePower(darkPact);
            QuickHandCheck(-1);

            AssertNumberOfUndeadInPlay(necro, 1);
            
        }

        [Test()]
        public void TestDarkPactDestroyResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card ghoul = GetCard("Ghoul");
            PlayCard(ghoul,true);
            GoToPlayCardPhase(necro);
            PutIntoPlay("DarkPact");
            GoToUsePowerPhase(necro);

            //Whenever an undead target is destroyed, draw a card."
            QuickHandStorage(necro);
            DestroyCard(ghoul);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestBookOfTheDeadSearchDeckAndPutIntoPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(necro);

            //Search your deck or trash for a ritual and put it into play or into your hand. If you searched your deck, shuffle your deck.
            //since no decisions specified, searching deck and putting in play
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(necro);
            PutIntoPlay("BookOfTheDead");
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(necro);

            AssertNumberOfRitualInPlay(necro, 1);
            Assert.IsTrue(numCardsInDeckBefore > numCardsInDeckAfter, "There was not fewer cards in deck after playing BookOfTheDead");
        }

        //once figure out a way to choose location of deck or trash followed by decision of in play or hand, add those test cases

        [Test()]
        public void TestBookOfTheDeadDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(necro);

            //You may draw a card.
            QuickHandStorage(necro);
            //tell the game that necro wants to draw a card
            DecisionYesNo = true;
            PutIntoPlay("BookOfTheDead");
            //we expect to have 1 more card in hand
            QuickHandCheck(1);
     
        }

        [Test()]
        public void TestGhoulHPSetsCorrectlyWith0Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card ghoul = GetCard("Ghoul");
            GoToPlayCardPhase(necro);

            //play ghoul with 0 rituals in play
            PlayCard(ghoul, true);

            //hp should be 2
            Assert.AreEqual(2, ghoul.HitPoints, $"Expected hitpoints to be 2, actual hitpoints is {ghoul.HitPoints}");
        }

        [Test()]
        public void TestGhoulHPSetsCorrectlyWith1Ritual()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card ghoul = GetCard("Ghoul");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");

            //play ghoul with 1 ritual in play
            PlayCard(ghoul, true);

            //hp should be 3
            Assert.AreEqual(3, ghoul.HitPoints, $"Expected hitpoints to be 3, actual hitpoints is {ghoul.HitPoints}");
        }

        [Test()]
        public void TestGhoulHPSetsCorrectlyWith2Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card ghoul = GetCard("Ghoul");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");
            PutIntoPlay("BloodRite");

            //play ghoul with 2 rituals in play
            PlayCard(ghoul, true);

            //hp should be 4
            Assert.AreEqual(4, ghoul.HitPoints, $"Expected hitpoints to be 4, actual hitpoints is {ghoul.HitPoints}");
        }

        [Test()]
        public void TestGhoulDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card ghoul = GetCard("Ghoul");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 25);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(ghoul, true);

            //At the end of your turn, this card deals the non-undead target with the second lowest HP 2 toxic damage.
            //second lowest non-undead should be ra
            QuickHPStorage(ra);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestChaoticSummon()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card chaotic = GetCard("ChaoticSummon");

            //stack deck so that we know that 2 cards should enter play
            PutOnDeck("Ghoul");
            PutOnDeck("DemonicImp");

            GoToPlayCardPhase(necro);
            //Put the top 2 cards of your deck into play.
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(necro);
            PlayCard(chaotic, true);
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(necro);

            //should be at 2 cards fewer, 2 cards played + chaotic summon
            Assert.AreEqual(numCardsInDeckBefore - 3, numCardsInDeckAfter);
            
            //based on the stacking of the deck, should be 2 undead now in play
            AssertNumberOfUndeadInPlay(necro, 2);
        }

        [Test()]
        public void TestBackfireHexDestructionEffect()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card backfire = GetCard("BackfireHex");

            PutIntoPlay("BacklashField");

            GoToPlayCardPhase(necro);

            //You may destroy an ongoing card.
            int numCardsInPlayBefore = GetNumberOfCardsInPlay(baron);
            PlayCard(backfire, true);
            int numCardsInPlayAfter = GetNumberOfCardsInPlay(baron);

            //should be at 1 card fewer, since backfire hex destroyed backlash field
            Assert.AreEqual(numCardsInPlayBefore - 1, numCardsInPlayAfter);
        }

        [Test()]
        public void TestBackfireHexPutUndeadInPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card backfire = GetCard("BackfireHex");

            PutInTrash("Ghoul");

            GoToPlayCardPhase(necro);

            //Put an undead card from the trash into play.
            PlayCard(backfire, true);

            //should be at 1 undead in play now
            AssertNumberOfUndeadInPlay(necro, 1);
        }

        [Test()]
        public void TestBloodRiteGainHP()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card bloodRite = GetCard("BloodRite");
            Card ghoul = GetCard("Ghoul");
            Card imp = GetCard("DemonicImp");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(fanatic.CharacterCard, 25);

            PlayCard(ghoul, true);
            PlayCard(imp, true);
            SetHitPoints(imp, 1);
            


            GoToPlayCardPhase(necro);

            PlayCard(bloodRite, true);

            //When an Undead target is destroyed, all non-undead hero targets regain 2 HP.

            List<int?> heroHpBefore = new List<int?>() { necro.CharacterCard.HitPoints, ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, imp.HitPoints };
            DestroyCard(ghoul, baron.CharacterCard);
            List<int?> heroHpAfter = new List<int?>() { necro.CharacterCard.HitPoints, ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, imp.HitPoints };

            //check each hero's hp
            Assert.AreEqual(heroHpBefore[0] + 2, heroHpAfter[0]);
            Assert.AreEqual(heroHpBefore[1] + 2, heroHpAfter[1]);
            Assert.AreEqual(heroHpBefore[2] + 2, heroHpAfter[2]);
            //check imp hp hasn't changed
            Assert.AreEqual(heroHpBefore[3], heroHpAfter[3]);

        }

        [Test()]
        public void TestHellfireDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card hellfire = GetCard("Hellfire");
            Card ghoul = GetCard("Ghoul");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PlayCard(ghoul, true);

            GoToPlayCardPhase(necro);
            
            PlayCard(hellfire, true);

            //Whenever an Undead target is destroyed, Necro deals 1 non-hero target 3 infernal damage.
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            DestroyCard(ghoul, baron.CharacterCard);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestCorpseExplosionDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card explosion = GetCard("CorpseExplosion");
            Card ghoul = GetCard("Ghoul");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card elemental = GetCard("ElementalRedistributor");

            //set up game for effect testing
            PlayCard(ghoul, true);
            PlayCard(elemental, true);

            GoToPlayCardPhase(necro);

            PlayCard(explosion, true);

            //Whenever an Undead target is destroyed, Necro deals 2 toxic damage to all villain targets.
            List<int?> HpBefore = new List<int?>() { baron.CharacterCard.HitPoints, mdp.HitPoints, elemental.HitPoints };
            DestroyCard(ghoul, baron.CharacterCard);
            List<int?> HpAfter = new List<int?>() { baron.CharacterCard.HitPoints, mdp.HitPoints, elemental.HitPoints };

            //baron blade is immune to damage, so HP should be the same
            Assert.AreEqual(HpBefore[0], HpAfter[0], "Baron Blade's hitpoints don't match.");

            //other villain targets should have 2 fewer HP
            Assert.AreEqual(HpBefore[1] - 2, HpAfter[1], "Mobile Defense Platform's hitpoints don't match");
            Assert.AreEqual(HpBefore[2] - 2, HpAfter[2], "Elemental Redistributor's hitpoints don't match");


        }

        [Test()]
        public void TestFinalRitualPutInPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card final = GetCard("FinalRitual");


            //put in play a ritual to boost HP of undead by 1
            PutIntoPlay("DarkPact");


            //set up game for effect testing
            PutInTrash("Ghoul");
            PutInTrash("Abomination");

            GoToPlayCardPhase(necro);

            //Search your trash for up to 2 Undead and put them into play. {Necro} deals each of those cards 2 toxic damage.
            PlayCard(final, true);

            //check that there are 2 in play
            AssertNumberOfUndeadInPlay(necro, 2);
            Card abomination = GetCardInPlay("Abomination");
            Card ghoul = GetCardInPlay("Ghoul");

            //check the health of each
            Assert.AreEqual(abomination.MaximumHitPoints - 2, abomination.HitPoints);
            Assert.AreEqual(ghoul.MaximumHitPoints - 2, ghoul.HitPoints);


        }

        [Test()]
        public void TestFinalRitualDealDamageMax()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card final = GetCard("FinalRitual");


            //put in play a ritual to boost HP of undead by 1
            PutIntoPlay("DarkPact");


            //set up game for effect testing
            PutInTrash("Ghoul");
            PutInTrash("Abomination");

            GoToPlayCardPhase(necro);

            QuickHPStorage(necro);
            //Necro deals himself X toxic damage, where X is 2 times the number of cards put into play this way.
            PlayCard(final, true);

            //2 cards were put into play, so 4 damage should be dealt
            QuickHPCheck(-4);


        }

        [Test()]
        public void TestFinalRitualDealDamageNone()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card final = GetCard("FinalRitual");


            //put in play a ritual to boost HP of undead by 1
            PutIntoPlay("DarkPact");


            //set up game for effect testing
            PutInTrash("Ghoul");
            PutInTrash("Abomination");

            GoToPlayCardPhase(necro);

            QuickHPStorage(necro);
            DecisionDoNotSelectCard = SelectionType.PutIntoPlay;
            //Necro deals himself X toxic damage, where X is 2 times the number of cards put into play this way.
            PlayCard(final, true);

            //no cards were put into play so no damage should be dealt
            QuickHPCheck(0);


        }

        [Test()]
        public void TestGrandSummonPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            //get GrandSummon from hand for consistent behavior
            PutInHand("GrandSummon");
            Card grand = GetCardFromHand("GrandSummon");

            GoToPlayCardPhase(necro);

            int numCardsInDeckBefore = GetNumberOfCardsInDeck(necro);
            int numCardsInPlayBefore = GetNumberOfCardsInPlay(necro);
            int numCardsInTrashBefore = GetNumberOfCardsInTrash(necro);
            //Reveal cards from the top of your deck until you reveal 2 Undead cards. Put 1 into play and 1 into the trash.
            PlayCard(grand, true);
            int numCardsInDeckAfter = GetNumberOfCardsInDeck(necro);
            int numCardsInPlayAfter = GetNumberOfCardsInPlay(necro);
            int numCardsInTrashAfter = GetNumberOfCardsInTrash(necro);

            //should be 3 fewer cards in deck, 2 for revealed undeads
            Assert.AreEqual(numCardsInDeckBefore - 2, numCardsInDeckAfter, "The number of cards in the deck don't match.");
            //should be 1 more card in play, check that it is undead
            Assert.AreEqual(numCardsInPlayBefore + 1, numCardsInPlayAfter, "The number of cards in play don't match.");
            AssertNumberOfUndeadInPlay(necro, 1);
            //should be 2 more cards in trash, undead trashed and grand summon
            Assert.AreEqual(numCardsInTrashBefore + 2, numCardsInTrashAfter, "The number of cards in the trash don't match.");

        }

        [Test()]
        public void TestPossessedCorpseHPSetsCorrectlyWith0Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");
            GoToPlayCardPhase(necro);

            //play corpse with 0 rituals in play
            PlayCard(corpse, true);

            //hp should be 2
            Assert.AreEqual(2, corpse.HitPoints, $"Expected hitpoints to be 2, actual hitpoints is {corpse.HitPoints}");
        }

        [Test()]
        public void TestPossessedCorpseHPSetsCorrectlyWith1Ritual()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");

            //play corpse with 1 ritual in play
            PlayCard(corpse, true);

            //hp should be 3
            Assert.AreEqual(3, corpse.HitPoints, $"Expected hitpoints to be 3, actual hitpoints is {corpse.HitPoints}");
        }

        [Test()]
        public void TestPossessedCorpseHPSetsCorrectlyWith2Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");
            PutIntoPlay("BloodRite");

            //play corpse with 2 rituals in play
            PlayCard(corpse, true);

            //hp should be 4
            Assert.AreEqual(4, corpse.HitPoints, $"Expected hitpoints to be 4, actual hitpoints is {corpse.HitPoints}");
        }

        [Test()]
        public void TestPossessedCorpseDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 3);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(corpse, true);

            //At the end of your turn, this card deals the non-undead hero target with the lowest HP 2 infernal damage
            // lowest non-undead should be fanatic
            QuickHPStorage(fanatic);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDemonicImpHPSetsCorrectlyWith0Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");
            GoToPlayCardPhase(necro);

            //play imp with 0 rituals in play
            PlayCard(imp, true);

            //hp should be 2
            Assert.AreEqual(2, imp.HitPoints, $"Expected hitpoints to be 2, actual hitpoints is {imp.HitPoints}");
        }

        [Test()]
        public void TestDemonicImpHPSetsCorrectlyWith1Ritual()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");

            //play imp with 1 ritual in play
            PlayCard(imp, true);

            //hp should be 3
            Assert.AreEqual(3, imp.HitPoints, $"Expected hitpoints to be 3, actual hitpoints is {imp.HitPoints}");
        }

        [Test()]
        public void TestDemonicImpHPSetsCorrectlyWith2Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");
            PutIntoPlay("BloodRite");

            //play imp with 2 rituals in play
            PlayCard(imp, true);

            //hp should be 4
            Assert.AreEqual(4, imp.HitPoints, $"Expected hitpoints to be 4, actual hitpoints is {imp.HitPoints}");
        }

        [Test()]
        public void TestDemonicImpDestroyEquipment()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");

            //prep an equipment to destroy
            PutIntoPlay("TheStaffOfRa");
            GoToPlayCardPhase(necro);

            PlayCard(imp, true);

            //At the end of your turn, destroy 1 hero equipment or ongoing card."
            GoToEndOfTurn(necro);

            //there should be no more equipment in play
            AssertNumberOfCardsInPlay((Card c) => GameController.IsEquipment(c) && c.IsHero, 0);

        }

        [Test()]
        public void TestDemonicImpDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");

            //prep an ongoing to destroy
            PutIntoPlay("FleshOfTheSunGod");
            GoToPlayCardPhase(necro);

            PlayCard(imp, true);

            //At the end of your turn, destroy 1 hero equipment or ongoing card."
            GoToEndOfTurn(necro);

            //there should be no more ongoings in play
            AssertNumberOfCardsInPlay((Card c) => c.IsOngoing && c.IsHero, 0);

        }

        [Test()]
        public void TestDemonicImpWhenDestroyed()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card imp = GetCard("DemonicImp");
            PutInHand(ra, "TheStaffOfRa");
            Card staff = GetCardFromHand(ra, "TheStaffOfRa");
            

            PlayCard(imp, true);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCard = staff;
            //When this card is destroyed, one player may play a card."
            QuickHandStorage(ra);
            DestroyCard(imp, baron.CharacterCard);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestTaintedBloodDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            //put ritual in play to prevent undead from being destroyed
            PutIntoPlay("DarkPact");

            //put some undead in play
            PutIntoPlay("Ghoul");
            PutIntoPlay("Abomination");

            Card ghoul = GetCardInPlay("Ghoul");
            Card abomination = GetCardInPlay("Abomination");

            GoToPlayCardPhase(necro);

            PutIntoPlay("TaintedBlood");

            //At the end of your draw phase, Necro deals the undead target with the lowest HP 2 irreducible toxic damage.
            //lowest hp undead is ghoul

            QuickHPStorage(ghoul);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestTalismanImmuneToUndead()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 3);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(corpse, true);

            //put the talisman in play on fanatic
            DecisionSelectCard = fanatic.CharacterCard;
            PutIntoPlay("Talisman");

            //At the end of your turn, possessed card deals the non-undead hero target with the lowest HP 2 infernal damage
            // lowest non-undead should be fanatic, but since talisman is on her, she should be immune
            QuickHPStorage(fanatic);
            GoToEndOfTurn(necro);
            QuickHPCheck(0);
        }
        [Test()]
        public void TestTalismanNotImmuneToNotUndead()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card corpse = GetCard("PossessedCorpse");


            GoToPlayCardPhase(necro);


            //put the talisman in play on fanatic
            DecisionSelectCard = fanatic.CharacterCard;
            PutIntoPlay("Talisman");

            //Damage is being dealt by not an undead target, should be dealt normally
            QuickHPStorage(fanatic);
            DealDamage(baron, fanatic, 5, DamageType.Fire);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestAbominationHPSetsCorrectlyWith0Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");
            GoToPlayCardPhase(necro);

            //play abomination with 0 rituals in play
            PlayCard(abomination, true);

            //hp should be 6
            Assert.AreEqual(6, abomination.HitPoints, $"Expected hitpoints to be 6, actual hitpoints is {abomination.HitPoints}");
        }

        [Test()]
        public void TestAbominationHPSetsCorrectlyWith1Ritual()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");

            //play abomination with 1 ritual in play
            PlayCard(abomination, true);

            //hp should be 7
            Assert.AreEqual(7, abomination.HitPoints, $"Expected hitpoints to be 7, actual hitpoints is {abomination.HitPoints}");
        }

        [Test()]
        public void TestAbominationHPSetsCorrectlyWith2Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");
            PutIntoPlay("BloodRite");

            //play abomination with 2 rituals in play
            PlayCard(abomination, true);

            //hp should be 8
            Assert.AreEqual(8, abomination.HitPoints, $"Expected hitpoints to be 8, actual hitpoints is {abomination.HitPoints}");
        }

        [Test()]
        public void TestAbominationDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");
            PutIntoPlay("DemonicImp");
            Card imp = GetCardInPlay("DemonicImp");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 3);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(abomination, true);

            //At the end of your turn, this card deals all non-Undead hero targets 2 toxic damage.

            List<int?> heroHpBefore = new List<int?>() { necro.CharacterCard.HitPoints, ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, imp.HitPoints };
            GoToEndOfTurn(necro);
            List<int?> heroHpAfter = new List<int?>() { necro.CharacterCard.HitPoints, ra.CharacterCard.HitPoints, fanatic.CharacterCard.HitPoints, imp.HitPoints };

            //check each hero's hp
            Assert.AreEqual(heroHpBefore[0] - 2, heroHpAfter[0]);
            Assert.AreEqual(heroHpBefore[1] - 2, heroHpAfter[1]);
            Assert.AreEqual(heroHpBefore[2] - 2, heroHpAfter[2]);
            //check imp hp hasn't changed
            Assert.AreEqual(heroHpBefore[3], heroHpAfter[3]);
        }

        [Test()]
        public void TestAbominationWhenDestroyed()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card abomination = GetCard("Abomination");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 3);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(abomination, true);

            // When this card is destroyed, all players draw a card.

             List<int?> numCardsInHandBefore = new List<int?>() { GetNumberOfCardsInHand(necro), GetNumberOfCardsInHand(ra), GetNumberOfCardsInHand(fanatic)};
            DestroyCard(abomination, necro.CharacterCard);
            List<int?> numCardsInHandAfter = new List<int?>() { GetNumberOfCardsInHand(necro), GetNumberOfCardsInHand(ra), GetNumberOfCardsInHand(fanatic) };

            //check each hero's hand size, should be +1
            Assert.AreEqual(numCardsInHandBefore[0] + 1, numCardsInHandAfter[0]);
            Assert.AreEqual(numCardsInHandBefore[1] + 1, numCardsInHandAfter[1]);
            Assert.AreEqual(numCardsInHandBefore[2] + 1, numCardsInHandAfter[2]);
 
        }


        [Test()]
        public void TestZombieSetsCorrectlyWith0Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card zombie = GetCard("NecroZombie");
            GoToPlayCardPhase(necro);

            //play zombie with 0 rituals in play
            PlayCard(zombie, true);

            //hp should be 2
            Assert.AreEqual(2, zombie.HitPoints, $"Expected hitpoints to be 2, actual hitpoints is {zombie.HitPoints}");
        }

        [Test()]
        public void TestZombieHPSetsCorrectlyWith1Ritual()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card zombie = GetCard("NecroZombie");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");

            //play zombie with 1 ritual in play
            PlayCard(zombie, true);

            //hp should be 3
            Assert.AreEqual(3, zombie.HitPoints, $"Expected hitpoints to be 3, actual hitpoints is {zombie.HitPoints}");
        }

        [Test()]
        public void TestZombieHPSetsCorrectlyWith2Rituals()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Megalopolis");
            StartGame();
            Card zombie = GetCard("NecroZombie");
            GoToPlayCardPhase(necro);

            PutIntoPlay("DarkPact");
            PutIntoPlay("BloodRite");

            //play zombie with 2 rituals in play
            PlayCard(zombie, true);

            //hp should be 4
            Assert.AreEqual(4, zombie.HitPoints, $"Expected hitpoints to be 4, actual hitpoints is {zombie.HitPoints}");
        }

        [Test()]
        public void TestZombieDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Necro", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            Card zombie = GetCard("NecroZombie");

            SetHitPoints(necro.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 12);
            SetHitPoints(baron.CharacterCard, 23);
            SetHitPoints(fanatic.CharacterCard, 3);
            SetHitPoints(GetCard("MobileDefensePlatform"), 6);


            GoToPlayCardPhase(necro);

            PlayCard(zombie, true);

            //At the end of your turn, this card deals the non-Undead hero target with the highest HP 2 toxic damage.
            // highest HP should be necro
            QuickHPStorage(necro);
            GoToEndOfTurn(necro);
            QuickHPCheck(-2);
        }

    }
}

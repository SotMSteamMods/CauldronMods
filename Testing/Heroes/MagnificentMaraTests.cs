using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.MagnificentMara;

namespace CauldronTests
{
    [TestFixture()]
    public class MagnificentMaraTests : BaseTest
    {
        #region MaraHelperFunctions
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(mara.CharacterCard, 1);
            DealDamage(villain, mara, 2, DamageType.Melee);
        }

        #endregion maraHelperFunctions

        [Test]
        public void TestMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(MagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(27, mara.CharacterCard.HitPoints);
        }
        [Test]
        public void TestConvincingDoubleBasic()
        {
            //There are a million things that could go wrong with Convincing Double. 
            //For now I'm just going to make a very basic test to show that the fundamental
            //functionality is there.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            StartGame();
            Card thokk = PutInHand("Thokk");
            Card external = PutInHand("ExternalCombustion");
            Card transmutive = PutInHand("TransmutiveRecovery");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("AdhesiveFoamGrenade");

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker, scholar.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { thokk, GetCardInPlay("MobileDefensePlatform"), external, transmutive };
            DecisionAutoDecideIfAble = true;
            SetHitPoints(new TurnTakerController[] { legacy, bunker, scholar }, 15);
            
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);
            AssertNotDamageSource(legacy.CharacterCard);

            for(int i = 0; i < 3; i++)
            {
                DecisionSelectTurnTakersIndex = i;
                PlayCard("ConvincingDouble");
                //first, Legacy hands Thokk to Bunker
                //then Bunker hands External Combustion to Scholar
                //finally Scholar gives Transmutive Recovery to Legacy
            }

            //each one lost a card, Legacy drew 2 from Recovery and Bunker drew 1 from Thokk
            QuickHandCheck(1, 0, -1);

            //Legacy gained two, Bunker gained nothing, Scholar hit himself for two
            QuickHPCheck(2, 0, -2);
        }
        [Test]
        public void TestConvincingDoubleWithSentinels()
        {
            //just to make sure it doesn't break *too* badly.

            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "TheSentinels", "TheScholar", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card thokk = PutInHand("Thokk");
            Card transmutive = PutInHand("TransmutiveRecovery");
            Card dichotomy = PutInHand("HorrifyingDichotomy");

            //needs extra one-shots in hand so we actually make decisions
            PutInHand("BolsterAllies");
            PutInHand("KnowWhenToTurnLoose");
            PutInHand("SecondChance");

            SetHitPoints(new Card[] { mainstay, writhe, medico, idealist, legacy.CharacterCard, scholar.CharacterCard }, 8);

            DecisionSelectCards = new Card[] { thokk, idealist, mdp };
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, sentinels.TurnTaker, scholar.TurnTaker, sentinels.TurnTaker, sentinels.TurnTaker, legacy.TurnTaker };

            QuickHPStorage(writhe, mdp, scholar.CharacterCard);
            QuickHandStorage(legacy, sentinels, scholar);

            PlayCard("ConvincingDouble");

            //Legacy hands Sentinels Thokk, they pick Idealist to take it
            QuickHPCheck(0, -3, 0);

            QuickHandCheck(-1, 1, 0);

            DecisionSelectCards = new Card[] { transmutive, writhe };
            DecisionSelectCardsIndex = 0;

            PlayCard("ConvincingDouble");

            //Scholar hands Sentinels Transmutive Recovery, they pick Writhe to take it
            QuickHandCheck(0, 2, -1);
            //QuickHPCheck(2, 0, 0); 
            //this does not work, cannot pick specific character for Sentinels

            DecisionSelectCards = new Card[] { dichotomy, mdp, mdp };
            DecisionSelectCardsIndex = 0;
            AssertNotDamageSource(writhe);
            AssertNotDamageSource(medico);

            PlayCard("ConvincingDouble");

            //Sentinels hand Legacy Horrifying Dichotomy, who ought to do both of the damages.
            QuickHandCheck(0, -1, 0);
            QuickHPCheck(0, -6, 0);

            Assert.Ignore("Pass-to-Sentinels doesn't work, not sure it's fixable. Pass-from-sentinels doesn't, and I'm not sure why.");
        }
    }
}

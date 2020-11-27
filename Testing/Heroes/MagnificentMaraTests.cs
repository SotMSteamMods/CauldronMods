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
            Card thokk =PutInHand("Thokk");
            Card external = PutInHand("ExternalCombustion");
            Card transmutive = PutInHand("TransmutiveRecovery");

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
    }
}

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
    public class TitanVariantTests : BaseTest
    {
        protected HeroTurnTakerController titan { get { return FindHero("Titan"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(titan.CharacterCard, 1);
            DealDamage(villain, titan, 2, DamageType.Melee);
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        [Test()]
        [Order(0)]
        public void TestMOSSTitanLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(titan);
            Assert.IsInstanceOf(typeof(MinistryOfStrategicScienceTitanCharacterCardController), titan.CharacterCardController);

            foreach (var card in titan.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(29, titan.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestMOSSTitanInnatePower()
        {
            SetupGameController("Apostate", "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card tform = PutInHand("Titanform");
            DecisionSelectCard = tform;

            //You may play an ongoing card. Draw a card.
            QuickHandStorage(titan);
            UsePower(titan);
            QuickHandCheck(0);
            AssertIsInPlay(tform);
        }

        [Test()]
        public void TestMOSSTitanIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);


            //Each hero may deal themselves 1 psychic damage to draw a card.
            QuickHandStorage(haka, bunker, scholar);
            QuickHPStorage(haka, bunker, scholar);
            UseIncapacitatedAbility(titan, 0);
            QuickHandCheck(1, 1, 1);
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestMOSSTitanIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //Destroy an environment card.
            Card mono = PlayCard("PlummetingMonorail");
            UseIncapacitatedAbility(titan, 1);
            AssertInTrash(mono);
        }

        [Test()]
        public void TestMOSSTitanIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            UseIncapacitatedAbility(titan, 2);

            //Prevent the next damage that would be dealt to a hero target.
            QuickHPStorage(haka);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(0);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}

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
    public class TitanVariantTests : CauldronBaseTest
    {

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(titan.CharacterCard, 1);
            DealDamage(villain, titan, 2, DamageType.Melee);
        }

        [Test()]
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

        [Test()]
        public void TestFutureTitanLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Titan/FutureTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(titan);
            Assert.IsInstanceOf(typeof(FutureTitanCharacterCardController), titan.CharacterCardController);

            foreach (var card in titan.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(33, titan.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestFutureTitanInnatePower()
        {
            SetupGameController("Apostate", "Cauldron.Titan/FutureTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //If Titanform is in play, 1 target deals {Titan} 1 melee damage. Otherwise, play the top card of your deck.
            Card tform = PutOnDeck("Titanform");
            //Otherwise, play the top card of your deck.
            QuickHPStorage(titan, apostate);
            UsePower(titan);
            QuickHPCheck(0, 0);
            AssertIsInPlay(tform);
            //If Titanform is in play, 1 target deals {Titan} 1 melee damage. 
            QuickHPStorage(titan, apostate);
            UsePower(titan);
            QuickHPCheck(-1, 0);
        }

        [Test()]
        public void TestFutureTitanIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Titan/FutureTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One player may draw a card now,
            QuickHandStorage(haka);
            UseIncapacitatedAbility(titan, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestFutureTitanIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Titan/FutureTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            SetHitPoints(apostate, 27);
            Card sword = GetCardInPlay("Condemnation");
            SetHitPoints(sword, 6);

            //Two targets regain 1 HP each.
            QuickHPStorage(apostate.CharacterCard, sword);
            UseIncapacitatedAbility(titan, 1);
            QuickHPCheck(1, 1);
        }

        [Test()]
        public void TestFutureTitanIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Titan/FutureTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            Card apoc = PlayCard("Apocalypse");

            //Destroy an ongoing card.
            UseIncapacitatedAbility(titan, 2);
            AssertInTrash(apoc);
        }

        [Test()]
        public void TestOniTitanLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(titan);
            Assert.IsInstanceOf(typeof(OniTitanCharacterCardController), titan.CharacterCardController);

            foreach (var card in titan.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(27, titan.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestOniTitanInnatePowerDeck()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(titan, 17);
            Card tform = PutOnDeck("Titanform");

            //Titan regains 1HP. Search your deck, trash, and hand for Titanform and play it. Shuffle your deck.
            QuickShuffleStorage(titan);
            QuickHPStorage(titan);
            UsePower(titan);
            QuickHPCheck(1);
            AssertIsInPlay(tform);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestOniTitanInnatePowerTrash()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(titan, 17);
            Card tform = PutInTrash("Titanform");

            //Titan regains 1HP. Search your deck, trash, and hand for Titanform and play it. Shuffle your deck.
            QuickShuffleStorage(titan);
            QuickHPStorage(titan);
            UsePower(titan);
            QuickHPCheck(1);
            AssertIsInPlay(tform);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestOniTitanInnatePowerHand()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(titan, 17);
            Card tform = PutInHand("Titanform");

            //Titan regains 1HP. Search your deck, trash, and hand for Titanform and play it. Shuffle your deck.
            QuickShuffleStorage(titan);
            QuickHPStorage(titan);
            UsePower(titan);
            QuickHPCheck(1);
            AssertIsInPlay(tform);
            QuickShuffleCheck(1);
        }

        [Test()]
        public void TestOniTitanIncap1()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One player may draw a card now,
            QuickHandStorage(haka);
            UseIncapacitatedAbility(titan, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestOniTitanIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One Hero may use a power now.
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(titan, 1);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestOniTitanIncap3()
        {
            SetupGameController("Apostate", "Cauldron.Titan/OniTitanCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(apostate);

            //One hero may discard a card to reduce damage dealt to them by 1 until the start of your turn.
            QuickHandStorage(haka);
            UseIncapacitatedAbility(titan, 2);
            QuickHandCheck(-1);

            //reduce damge to target by 1
            QuickHPStorage(haka);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(-1);
            //until start of your turn
            GoToStartOfTurn(titan);
            QuickHPStorage(haka);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
    }
}

using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatFutureTests : CauldronBaseTest
    {
        private bool IsSpell(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "spell");
        }

        [Test()]
        public void TestFutureTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tiamat);
            AssertNumberOfCardsInPlay(tiamat, 3);
            AssertHitPoints(tiamat, 120);
            //Dragonscales have X HP, where X = {H - 1}.
            AssertMaximumHitPoints(GetCard("NeoscaleCharacter"), 2);
            AssertMaximumHitPoints(GetCard("ExoscaleCharacter"), 2);
            //At the start of the villain turn, flip {Tiamat}'s villain character cards.
            AssertFlipped(tiamat);
        }

        [Test()]
        public void TestScaleHP_5H()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Ra", "Unity", "Megalopolis");
            StartGame();

            //Dragonscales have X HP, where X = {H - 1}.
            AssertMaximumHitPoints(GetCard("NeoscaleCharacter"), 4);
            AssertMaximumHitPoints(GetCard("ExoscaleCharacter"), 4);
        }

        [Test()]
        public void TestScaleHP_3H_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            //Advanced: X = {H + 1} instead.
            AssertMaximumHitPoints(GetCard("NeoscaleCharacter"), 4);
            AssertMaximumHitPoints(GetCard("ExoscaleCharacter"), 4);
        }

        [Test()]
        public void TestScaleHP_5H_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Ra", "Unity", "Megalopolis" }, true);
            StartGame();

            //Advanced: X = {H + 1} instead.
            AssertMaximumHitPoints(GetCard("NeoscaleCharacter"), 6);
            AssertMaximumHitPoints(GetCard("ExoscaleCharacter"), 6);
        }

        [Test()]
        public void TestFutureTiamatAdvancedStartOfGame()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            AssertNumberOfCardsInTrash(tiamat, 2);
            foreach (Card c in tiamat.TurnTaker.Trash.Cards)
            {
                Assert.IsTrue(IsSpell(c));
            }
        }
        [Test()]
        public void TestFutureTiamatAdvancedStartOfGame_OnlyHappensOnce()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis" }, true);
            StartGame();

            AssertNumberOfCardsInTrash(tiamat, 2);
            GoToStartOfTurn(tiamat);
            AssertNumberOfCardsInTrash(tiamat, 2);
        }

        [Test()]
        public void TestFutureTiamatEndOfTurn()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            //At the end of the villain turn, {Tiamat} deals the hero target with the highest HP {H} energy damage. 
            QuickHPStorage(haka, legacy, bunker);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-3, 0, 0);

            AddCannotDealNextDamageTrigger(tiamat, tiamat.CharacterCard);
            PlayCard("TaMoko");
            PlayCard("SurgeOfStrength");
            //Then, if {Tiamat} deals no damage this turn, each hero target deals itself 3 projectile damage.
            QuickHPStorage(haka, legacy, bunker);
            GoToEndOfTurn(tiamat);
            QuickHPCheck(-2, -4, -3);
        }

        [Test()]
        public void TestExoscale()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "TheScholar", "Bunker", "Haka", "Unity", "Megalopolis");
            StartGame();

            Card exo = GetCardInPlay("ExoscaleCharacter");
            SetHitPoints(scholar, 17);

            //When this card is destroyed, 1 hero may draw a card...
            QuickHandStorage(scholar);
            DealDamage(bunker, exo, 3, DamageType.Melee);
            QuickHandCheck(1);
            AssertIsInPlay(exo);
            //Then, flip this card.
            AssertFlipped(exo);

            //At the end of the villain turn...Then flip all ruined scales and restore them to their max HP.
            GoToEndOfTurn(tiamat);
            AssertNotFlipped(exo);
            AssertHitPoints(exo, 3);

            DecisionSelectFunction = 1;

            //When this card is destroyed, 1 hero may...use a power.
            QuickHPStorage(scholar);
            QuickHandStorage(scholar);
            DealDamage(bunker, exo, 3, DamageType.Melee);
            QuickHandCheck(0);
            QuickHPCheck(1);
            //Then, flip this card.
            AssertFlipped(exo);
        }

        [Test()]
        public void TestNeoscale()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Unity", "Megalopolis");
            StartGame();

            Card neo = GetCardInPlay("NeoscaleCharacter");
            Card surge = PutInHand("SurgeOfStrength");

            //When this card is destroyed, 1 hero may draw a card...
            QuickHandStorage(legacy);
            DealDamage(bunker, neo, 3, DamageType.Melee);
            QuickHandCheck(1);
            AssertIsInPlay(neo);
            //Then, flip this card.
            AssertFlipped(neo);

            //At the end of the villain turn...Then flip all ruined scales and restore them to their max HP.
            GoToEndOfTurn(tiamat);
            AssertNotFlipped(neo);
            AssertHitPoints(neo, 3);

            DecisionSelectFunction = 1;
            DecisionSelectCard = surge;

            //When this card is destroyed, 1 hero may...play a card. 
            QuickHandStorage(legacy);
            DealDamage(bunker, neo, 3, DamageType.Melee);
            QuickHandCheck(-1);
            AssertIsInPlay(surge);
            //Then, flip this card.
            AssertFlipped(neo);
        }

        [Test()]
        public void TestTiamatFutureBackEffects()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Ra", "Megalopolis");
            StartGame();

            //{Tiamat} counts as The Jaws of Winter, The Mouth of the Inferno, and The Eye of the Storm.
            AddCannotDealNextDamageTrigger(tiamat, tiamat.CharacterCard);
            QuickHPStorage(legacy, bunker, haka, ra);
            Card ice = PlayCard("ElementOfIce");
            PrintSpecialStringsForCard(ice);
            //Preventing damage by Tiamat once shows the damage is coming from her
            QuickHPCheck(0, -2, -2, -2);

            //Each spell card in the villain trash counts as Element of Ice, Element of Fire, and Element of Lightining.
            AddCannotDealNextDamageTrigger(tiamat, tiamat.CharacterCard);
            QuickHPStorage(legacy, bunker, haka, ra);
            Card fire = PlayCard("ElementOfFire");
            PrintSpecialStringsForCard(fire);
            //Preventing damage by Tiamat once shows the damage is coming from her
            //Only card in trash is Element of Ice which means Ice increased Fire
            QuickHPCheck(0, -3, -3, -3);


            AddCannotDealNextDamageTrigger(tiamat, tiamat.CharacterCard);
            QuickHPStorage(legacy, bunker, haka, ra);
            Card lightning = PlayCard("ElementOfLightning");
            PrintSpecialStringsForCard(lightning);
            //Preventing damage by Tiamat once shows the damage is coming from her
            //Only card in trash is Element of Ice which means Ice increased Fire
            QuickHPCheck(0, -4, -4, -4);
        }

        [Test()]
        public void TestTiamatFutureChallenge()
        {
            SetupGameController(new string[] { "Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Ra", "Megalopolis" }, challenge: true);
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");
            Card neo = GetCardInPlay("NeoscaleCharacter");
            Card exo = GetCardInPlay("ExoscaleCharacter");

            QuickHPStorage(tiamat.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, traffic);

            //"Whenever a non-villain target deals damage to a Dragonscale, {Tiamat} deals that target {H - 1} projectile damage.",
            DealDamage(legacy, neo, 1, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0);
            DealDamage(traffic, exo, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -3);

            //does not react to Tiamat damage
            DealDamage(bunker, tiamat, 4, DamageType.Melee);
            QuickHPCheck(-4, 0, 0, 0);

            //properly handles non-target and non-card damage sources
            DealDamage(police, neo, 1, DamageType.Melee);
            var envDamage = GameController.DealDamageToTarget(new DamageSource(GameController, base.env.TurnTaker), exo, 1, DamageType.Melee);
            GameController.ExhaustCoroutine(envDamage);
            QuickHPCheckZero();

            //also reacts if it flips the scale
            DealDamage(legacy, exo, 4, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0);


            //does not react to villain damage
            PlayCard("HostageSituation");
            DealDamage(tiamat, neo, 4, DamageType.Melee);
            QuickHPCheckZero();
        }
    }
}

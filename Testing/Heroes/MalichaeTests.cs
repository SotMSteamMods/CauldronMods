using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Malichae;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.LadyOfTheWood;

namespace CauldronTests
{
    [TestFixture()]
    public class MalichaeTests : BaseTest
    {
        #region MalichaeTestsHelperFunctions
        protected HeroTurnTakerController Malichae { get { return FindHero("Malichae"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(Malichae.CharacterCard, 1);
            DealDamage(villain, Malichae, 2, DamageType.Melee);
            AssertIncapacitated(Malichae);
        }

        #endregion

        [Test()]
        public void TestMalichaeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Malichae);
            Assert.IsInstanceOf(typeof(MalichaeCharacterCardController), Malichae.CharacterCardController);

            Assert.AreEqual(27, Malichae.CharacterCard.HitPoints);
        }

        [Test]
        public void InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);
            var discard = GetCardFromHand(Malichae);
            DecisionDiscardCard = discard;
            UsePower(Malichae);

            QuickHandCheck(1, 0, 0);
            AssertInTrash(Malichae, discard);
        }

        [Test]
        public void Incap1_DiscardDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            var discard = GetCardFromHand(ra);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionDiscardCard = discard;

            UseIncapacitatedAbility(Malichae, 0);
            QuickHandCheck(0, 2, 0);
            AssertInTrash(ra, discard);
        }

        [Test]
        public void Incap1_NoDiscardNoDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCards = new Card[1];

            UseIncapacitatedAbility(Malichae, 0);
            QuickHandCheck(0, 0, 0);
            AssertNumberOfCardsInTrash(Malichae, 0);
        }

        [Test]
        public void Incap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(env, "TrafficPileup");
            AssertInPlayArea(env, card);

            DecisionSelectCard = card;

            UseIncapacitatedAbility(Malichae, 1);

            AssertNumberOfCardsInTrash(env, 1);
        }

        [Test]
        public void Incap3_DamageTypeChanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(ra, "FleshOfTheSunGod");
            AssertInPlayArea(ra, card);

            DecisionSelectDamageType = DamageType.Fire;

            UseIncapacitatedAbility(Malichae, 2);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(fanatic, ra, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0);

            QuickHPStorage(baron, ra, fanatic);
            //villian damage is uneffected
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void Incap3_EffectExpires()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(ra, "FleshOfTheSunGod");
            AssertInPlayArea(ra, card);

            DecisionSelectDamageType = DamageType.Fire;

            UseIncapacitatedAbility(Malichae, 2);

            GoToEndOfTurn(Malichae);

            GoToStartOfTurn(Malichae);

            //effect is now expired

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(fanatic, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            QuickHPStorage(baron, ra, fanatic);
            //villian damage is uneffected
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void BaitAndSwitch_Discard_PlayDjinn()
        {
            SetupGameController(new[] { "BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis" }, randomSeed: 1480459545);
            StartGame();

            //prevent power usage
            PutIntoPlay("PaparazziOnTheScene");

            var discard1 = PutInHand(Malichae, "GrandBathiel");
            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");

            //DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);
            DecisionDiscardCard = discard1;
            DecisionMoveCard = trash;
            //DecisionSelectCards = new Card[] { discard1, trash };
            DecisionSelectFunction = 0; //put djinn into play
            DecisionSelectPower = Malichae.CharacterCard;

            AssertNextMessage($"{Malichae.Name} cannot currently use powers.");
            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInTrash(Malichae, discard1);
            AssertInPlayArea(Malichae, trash);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 1);
            //played a card, discarded a card
            //-1 + -1  = -2
            QuickHandCheck(-2, 0, 0);
        }

        [Test]
        public void BaitAndSwitch_Discard_DrawCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            PutIntoPlay("PaparazziOnTheScene");

            //2nd discard for the UsePower innate
            var discard1 = PutInHand(Malichae, "GrandBathiel");
            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");

            DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionSelectCards = new Card[] { discard1, trash };
            DecisionSelectFunction = 1; //draw cards
            DecisionSelectPower = Malichae.CharacterCard;

            AssertNextMessage($"{Malichae.Name} cannot currently use powers.");
            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInTrash(Malichae, discard1);
            AssertInTrash(Malichae, trash);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 2);
            //played a card, discarded a card, draw 2,
            //-1 + -1 + 2 = 0
            QuickHandCheck(0, 0, 0);
        }


        [Test]
        public void BaitAndSwitch_NoDiscard(
            [Values(588433992, null)] int? badSeeds
            )
        {
            SetupGameController(new[] { "BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis" }, randomSeed: badSeeds);
            StartGame();

            PutIntoPlay("PaparazziOnTheScene");

            var discard1 = PutInHand(Malichae, "GrandBathiel");
            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");
            DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            //DecisionDiscardCard = null;
            //DecisionSelectCards = new Card[3];
            DecisionSelectPower = Malichae.CharacterCard;

            AssertNextMessage($"{Malichae.Name} cannot currently use powers.");
            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInHand(Malichae, discard1);
            AssertInTrash(Malichae, trash);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 1);
            QuickHandCheck(-1, 0, 0);
        }

        [Test]
        [Sequential]
        public void DjinnTarget_IsTarget(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);
            AssertIsTarget(card, 5);
            AssertCardHasKeyword(card, "djinn", false);
        }

        [Test]
        [Sequential]
        public void DjinnTarget_IsImmuneToDamageType(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn,
            [Values(DamageType.Energy, DamageType.Radiant, DamageType.Projectile, DamageType.Sonic)] DamageType type
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, type, true); //damage is irreducible to ensure it's only the immunity that blocks the damage
            QuickHPCheck(0, 0, 0, 0, 0); //no damage

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Melee, true); //damage is irreducible to ensure it's only the immunity that blocks the damage
            QuickHPCheck(0, 0, 0, 0, -1); //no damage

            //check only djinn is effected
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, type, true); //damage is irreducible to ensure it's only the immunity that blocks the damage
            QuickHPCheck(0, -1, 0, 0, 0); //no damage
        }

        [Test]
        public void DjinnTarget_ReturnsToHandIfDestroyed(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            PrintSeparator("Destroy Action");
            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DestroyCard(card, baron.CharacterCard);
            AssertInHand(Malichae, card);

            PrintSeparator("Damage Action");
            PlayCard(card);
            AssertInPlayArea(Malichae, card);

            DealDamage(baron.CharacterCard, card, 99, DamageType.Melee, true);
            AssertInHand(Malichae, card);
        }

        [Test]
        public void DjinnTarget_DestroyPreventedWithHighOngoing(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            PrintSeparator("Setup");
            var card = PlayCard(djinn);
            DecisionNextToCard = card;
            var high = PlayCard("High" + djinn);
            SetHitPoints(card, 3); //low Hp to confirm the heal
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);

            PrintSeparator("Destroy Action");
            //Destroy on djinn should destroy ongoing instead
            DestroyCard(card, baron.CharacterCard);
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
            AssertIsAtMaxHP(card);

            PrintSeparator("Setup");
            DecisionNextToCard = card;
            PlayCard(high);
            SetHitPoints(card, 3); //low Hp to confirm the heal
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);

            PrintSeparator("Damage Action");
            DealDamage(baron.CharacterCard, card, 99, DamageType.Melee, true);
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
            AssertIsAtMaxHP(card);
        }

        [Test]
        public void DjinnTarget_DestroyPreventedWithGrandOngoing(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            PrintSeparator("Setup");
            var card = PlayCard(djinn);
            DecisionNextToCard = card;
            var high = PlayCard("High" + djinn);
            DecisionNextToCard = card;
            var grand = PlayCard("Grand" + djinn);
            SetHitPoints(card, 3); //low Hp to confirm the heal
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);

            PrintSeparator("Destroy Action");
            //Destroy on djinn should destroy ongoing instead
            DecisionSelectCard = grand;
            DestroyCard(card, baron.CharacterCard);
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertInTrash(Malichae, grand);
            AssertIsAtMaxHP(card);

            PrintSeparator("Setup");
            DecisionNextToCard = card;
            PlayCard(grand);
            SetHitPoints(card, 3); //low Hp to confirm the heal
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);

            PrintSeparator("Damage Action");
            DecisionSelectCard = grand;
            DealDamage(baron.CharacterCard, card, 99, DamageType.Melee, true);
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertInTrash(Malichae, grand);
            AssertIsAtMaxHP(card);
        }

        [Test]
        public void Djinn_Bathiel()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, -2, 0, 0);
            AssertInPlayArea(Malichae, card);
        }

        [Test]
        public void Djinn_Somael()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Somael";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DecisionSelectCard = ra.CharacterCard;

            GoToEndOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0); //damage reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0, -1, 0, 0); //damage not reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, -1, 0); //damage not reduced

            //expire effect
            GoToStartOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, -1, 0, 0); //damage not reduced
        }

        [Test]
        public void Djinn_Ezael()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Ezael";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);
            SetHitPoints(ra.CharacterCard, 10);

            DecisionSelectCard = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, 2, 0, 0);
            AssertInPlayArea(Malichae, card);
        }

        [Test]
        public void Djinn_Reshiel()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Reshiel";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DecisionSelectCards = new Card[] { ra.CharacterCard, fanatic.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, -1, -1, 0);
            AssertInPlayArea(Malichae, card);
        }

        [Test]
        public void HighDjinnOngoing_PlayWithoutTarget(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var high = GetCard("High" + djinn);

            AssertNextMessage($"There are no {djinn} cards in play to put High {djinn} next to. Moving it to {Malichae.Name}'s trash.");
            PlayCard(high);
            AssertNotInPlay(high);
            AssertInTrash(Malichae, high);
        }

        [Test]
        public void HighDjinnOngoing_PlayWithTarget(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);
            AssertCardHasKeyword(high, "ongoing", false);
            AssertCardHasKeyword(high, "limited", false);
            AssertCardHasKeyword(high, "djinn", false);
        }

        [Test]
        public void HighDjinnOngoing_NoEffectIfOngoingDestroy(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            SetHitPoints(card, 2); //to check we don't heal
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DestroyCard(high, baron.CharacterCard);

            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
            QuickHPCheck(0, 0, 0, 0, 0);
        }


        [Test]
        public void Djinn_HighBathiel_Passive()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, -3, 0, 0);
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
        }

        [Test]
        public void Djinn_HighBathiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToUsePowerPhase(Malichae);
            UsePower(high);
            QuickHPCheck(0, 0, -5, 0, 0); //4 + 1
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
        }


        [Test]
        public void Djinn_HighSomael_Passive()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Somael";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectCard = ra.CharacterCard;

            GoToEndOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0); //damage reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0, -1, 0, 0); //damage not reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, -1, 0); //damage not reduced

            //damage to djinns
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0); //damage reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0, 0, 0, -1); //damage not reduced to zero

            //expire effect
            GoToStartOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            QuickHPCheck(0, 0, -1, 0, 0); //damage not reduced

            //damage to djinns should still be protected
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0); //damage reduced to zero

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold, true);
            QuickHPCheck(0, 0, 0, 0, -1); //damage not reduced to zero
        }


        [Test]
        public void Djinn_HighSomael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Somael";
            var blade = PlayCard("BladeBattalion");
            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            GoToUsePowerPhase(Malichae);
            UsePower(high);
            AssertInTrash(high);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, -1); //damage reduced to zero except for villain

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold, true);
            QuickHPCheck(0, -1, -1, -1, -1, -1); //damage not reduced to zero

            DecisionSelectCard = ra.CharacterCard;

            GoToEndOfTurn(Malichae);

            //expire effect
            GoToStartOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold);
            QuickHPCheck(0, -1, -1, -1, -1, -1); //damage not reduced
        }

        [Test]
        public void Djinn_HighReshiel_Passive()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Reshiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectCards = new Card[] { ra.CharacterCard, fanatic.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, -2, -2, 0);
            AssertInPlayArea(Malichae, card);
        }


        [Test]
        public void Djinn_HighReshiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Reshiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectTargets = new Card[] { ra.CharacterCard, fanatic.CharacterCard, Malichae.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToUsePowerPhase(Malichae);
            UsePower(high);
            QuickHPCheck(0, -3, -3, -3, 0); //4 + 1
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
        }


        [Test]
        public void Djinn_HighEzael_Passive()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Ezael";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);
            var otherCard = PlayCard("Bathiel");
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(otherCard, 3);
            SetHitPoints(card, 3);

            PlayCard(high);
            AssertNextToCard(high, card);

            DecisionSelectCard = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, otherCard);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, 2, 0, 1, 1);
            AssertInPlayArea(Malichae, card);
        }

        [Test]
        public void Djinn_HighEzael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Ezael";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            AssertInPlayArea(Malichae, card);
            var otherCard = PlayCard("Bathiel");
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(otherCard, 3);
            SetHitPoints(card, 3);
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(fanatic.CharacterCard, 20);
            SetHitPoints(Malichae.CharacterCard, 20);

            PlayCard(high);
            AssertNextToCard(high, card);

            GoToUsePowerPhase(Malichae);
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, otherCard);

            UsePower(high);

            QuickHPCheck(0, 1, 1, 1, 2, 2);
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
        }


        [Test]
        public void GrandDjinnOngoing_PlayWithoutTarget(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var ongoing = GetCard("Grand" + djinn);

            AssertNextMessage($"There are no {djinn} cards in play to put Grand {djinn} next to. Moving it to {Malichae.Name}'s trash.");
            PlayCard(ongoing);
            AssertNotInPlay(ongoing);
            AssertInTrash(Malichae, ongoing);
        }

        [Test]
        public void GrandDjinnOngoing_PlayWithTargetNoHighOngoing(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(grand);

            //card should destroy itself
            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, grand);
        }

        [Test]
        public void GrandDjinnOngoing_PlayWithHighOngoing(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            PlayCard(grand);
            AssertNextToCard(grand, card);
            AssertCardHasKeyword(grand, "ongoing", false);
            AssertCardHasKeyword(grand, "djinn", false);
        }


        [Test]
        public void GrandDjinnOngoing_NoEffectIfOngoingDestroy(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            SetHitPoints(card, 2); //to check we don't heal
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            PlayCard(grand);
            AssertNextToCard(grand, card);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DestroyCard(grand, baron.CharacterCard);

            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, grand);
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        [Test]
        public void GrandDjinnOngoing_DestroyIfHighIsDestroyed(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            PlayCard(grand);
            AssertNextToCard(grand, card);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DestroyCard(high, baron.CharacterCard);

            AssertInPlayArea(Malichae, card);
            AssertInTrash(Malichae, high);
            AssertInTrash(Malichae, grand);
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        [Test]
        public void GrandDjinnOngoing_DestroyAtEndOfTurn(
            [Values("Bathiel", "Ezael", "Somael", "Reshiel")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);

            PlayCard(grand);
            AssertNextToCard(grand, card);

            GoToEndOfTurn(Malichae);

            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertInTrash(Malichae, grand);
        }

        [Test]
        public void Djinn_GrandBathiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);
            PlayCard(grand);
            AssertNextToCard(grand, card);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            GoToUsePowerPhase(Malichae);
            UsePower(grand);
            QuickHPCheck(0, 0, -7, 0, 0); //6 + 1
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);
        }


        [Test]
        public void Djinn_GrandEzael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Ezael";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);
            var otherCard = PlayCard("Bathiel");
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(otherCard, 1);
            SetHitPoints(card, 1);
            SetHitPoints(ra.CharacterCard, 10);
            SetHitPoints(baron.CharacterCard, 10);
            SetHitPoints(fanatic.CharacterCard, 10);
            SetHitPoints(Malichae.CharacterCard, 10);

            PlayCard(high);
            AssertNextToCard(high, card);
            PlayCard(grand);
            AssertNextToCard(grand, card);

            GoToUsePowerPhase(Malichae);
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, otherCard);

            UsePower(grand);

            QuickHPCheck(0, 3, 3, 3, 3, 3);
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);
        }

        [Test]
        public void Djinn_GrandReshiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var mdp = GetCardInPlay("MobileDefensePlatform");

            GoToPlayCardPhase(Malichae);

            var blade = PlayCard("BladeBattalion");

            string djinn = "Reshiel";

            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);
            PlayCard(grand);
            AssertNextToCard(grand, card);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade, mdp);
            GoToUsePowerPhase(Malichae);

            UsePower(grand);
            QuickHPCheck(0, 0, 0, 0, 0, -3, -3); //2 + 1
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);
        }


        [Test]
        public void Djinn_GrandSomael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Somael";
            var blade = PlayCard("BladeBattalion");
            var card = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);
            AssertInPlayArea(Malichae, card);

            PlayCard(high);
            AssertNextToCard(high, card);
            PlayCard(grand);
            AssertNextToCard(grand, card);

            GoToUsePowerPhase(Malichae);
            UsePower(grand);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, card, 3, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 2, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, -2); //damage reduced to zero except for villain
            SetHitPoints(blade, 5);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, card, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, blade, 2, DamageType.Cold, true);
            QuickHPCheck(0, -2, -2, -2, -2, -2); //damage not reduced to zero
            SetHitPoints(blade, 5);

            DecisionSelectCard = ra.CharacterCard;

            AssertInPlayArea(Malichae, card);
            AssertNextToCard(high, card);
            AssertNextToCard(grand, card);

            GoToEndOfTurn(Malichae);

            //expire effect4
            GoToStartOfTurn(Malichae);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold);
            QuickHPCheck(0, -1, -1, -1, 0, -1); //damage not reduced, djinn still have a global reducer
        }

        [Test]
        public void SummoningCrystal()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var card = PutInHand("SummoningCrystal");
            var target = PutInHand("Bathiel");
            AssertCardHasKeyword(card, "equipment", false);

            PlayCard(card);
            AssertInPlayArea(Malichae, card);

            GoToUsePowerPhase(Malichae);

            DecisionSelectCard = target;
            UsePower(card);

            AssertInPlayArea(Malichae, target);
        }

        [Test]
        public void Unshackled()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard("Unshackled");
            var target = PlayCard(djinn);
            var high = GetCard("High" + djinn);
            var grand = GetCard("Grand" + djinn);

            AssertInPlayArea(Malichae, card);
            AssertInPlayArea(Malichae, target);

            PlayCard(high);
            AssertNextToCard(high, target);
            PlayCard(grand);
            AssertNextToCard(grand, target);

            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, target);
            GoToUsePowerPhase(Malichae);

            AssertPhaseActionCount(2);

            UsePower(grand);
            UsePower(high);
            QuickHPCheck(0, 0, -12, 0, 0); //6 + 1
            AssertInPlayArea(Malichae, card);
            AssertInPlayArea(Malichae, target);
            AssertInTrash(high);
            AssertInTrash(grand);
        }

        [Test]
        public void FriendlyAdvice()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            DiscardAllCards(Malichae, ra, fanatic);
            ShuffleTrashIntoDeck(Malichae);
            ShuffleTrashIntoDeck(ra);
            ShuffleTrashIntoDeck(fanatic);

            SetHitPoints(Malichae, 17);
            SetHitPoints(fanatic, 17);
            SetHitPoints(ra, 17);

            DiscardTopCards(ra, 1);
            var moved = DiscardTopCards(ra, 1).First();
            AssertNumberOfCardsInTrash(ra, 2);

            var card = GetCard("FriendlyAdvice");
            PutInHand(card);

            GoToPlayCardPhase(Malichae);

            DecisionGainHP = fanatic.CharacterCard;
            DecisionSelectTurnTakers = new TurnTaker[] { Malichae.TurnTaker, ra.TurnTaker };
            DecisionMoveCard = moved;

            QuickHPStorage(baron, Malichae, ra, fanatic);
            QuickHandStorage(Malichae, ra, fanatic);
            PlayCard(card);
            AssertInTrash(card);

            QuickHPCheck(0, 0, 0, 2);
            QuickHandCheck(0, 0, 0);

            AssertNumberOfCardsInTrash(ra, 1);
            AssertOnTopOfDeck(moved);
        }


        [Test]
        public void SolomonsFire()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            var mdp = GetMobileDefensePlatform().Card;
            AssertInPlayArea(baron, mdp);

            GoToPlayCardPhase(Malichae);

            string djinn = "Reshiel";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DecisionNextToCard = mdp;

            var fire = PlayCard("SolomonsFire");
            AssertNextToCard(fire, mdp);

            DecisionSelectCards = new Card[] { mdp, fanatic.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card, mdp);
            GoToEndOfTurn(Malichae);
            QuickHPCheck(0, 0, 0, -1, 0, -2);
            AssertInPlayArea(Malichae, card);
            AssertNextToCard(fire, mdp);

            DestroyCard(mdp);
            AssertInTrash(Malichae, fire);
        }

        [Test]
        public void ShadowCatch()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            var moved = PutInDeck("Reshiel");

            QuickHandStorage(Malichae, ra, fanatic);
            QuickShuffleStorage(Malichae, ra, fanatic);

            DecisionSelectCards = new Card[] { moved, moved };

            var card = PutIntoPlay("ShadowCatch");
            AssertInTrash(card);

            QuickHandCheck(1, 0, 0);
            QuickShuffleCheck(1, 0, 0);
        }

        [Test]
        public void ZephaerensCompass()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var ongoing = PlayCard("BacklashField");
            var envCard = PlayCard("PlummetingMonorail");

            GoToPlayCardPhase(Malichae);

            var card = PlayCard("ZephaerensCompass");
            AssertInPlayArea(Malichae, card);

            string djinn = "Reshiel";

            var target = PlayCard(djinn);
            DecisionMoveCard = target;
            var high = GetCard("High" + djinn);
            PlayCard(high);
            AssertNextToCard(high, target);

            GoToUsePowerPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionMoveCard = target;
            DecisionDestroyCards = new Card[] { ongoing, envCard };

            UsePower(card);

            QuickHandCheck(1, 0, 0);
            AssertInHand(target);
            AssertInTrash(high);
            AssertInTrash(ongoing);
            AssertInTrash(envCard);
        }


        [Test]
        public void PrismaticVision()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            DiscardAllCards(Malichae);

            var cards = StackDeck(Malichae, new[] { "Bathiel", "Unshackled", "BaitAndSwitch" }).ToList();
            var card = PutInHand("PrismaticVision");

            var toPlay = cards.First(c => c.Identifier == "Bathiel");
            var toHand = cards.First(c => c.Identifier == "Unshackled");
            var toTrash = cards.First(c => c.Identifier == "BaitAndSwitch");

            DecisionSelectCards = new Card[] { toPlay, toHand, toPlay };
            QuickHandStorage(Malichae, ra, fanatic);
            PlayCard(card);
            AssertInTrash(card);
            AssertInPlayArea(Malichae, toPlay);
            AssertInHand(Malichae, toHand);
            AssertInTrash(Malichae, toTrash);
            QuickHandCheck(0, 0, 0);
        }

    }
}

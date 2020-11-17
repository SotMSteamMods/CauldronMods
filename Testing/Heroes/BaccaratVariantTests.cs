using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Baccarat;

namespace CauldronTests
{
    [TestFixture()]
    public class BaccaratVariantTests : BaseTest
    {
        #region BaccaratHelperFunctions
        protected HeroTurnTakerController baccarat { get { return FindHero("Baccarat"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(baccarat.CharacterCard, 1);
            DealDamage(villain, baccarat, 2, DamageType.Melee);
        }

        #endregion BaccaratHelperFunctions

        [Test()]
        public void TestLoadBaccaratAceOfSwords()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(AceOfSwordsBaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(30, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower1EuchreInTrash()
        {
            SetupGameController("Spite", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            GoToUsePowerPhase(baccarat);
            //Pick deal damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            QuickHPStorage(spite);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-2);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower2EuchreInTrash()
        {
            SetupGameController("Spite", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            GoToUsePowerPhase(baccarat);
            //Pick deal damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            QuickHPStorage(spite);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-4);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower3EuchreInTrash()
        {
            SetupGameController("Spite", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            GoToUsePowerPhase(baccarat);
            //Pick deal 2 damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            PutInTrash("AfterlifeEuchre", 2);
            QuickHPStorage(spite);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-6);
            AssertNumberOfCardsInTrash(baccarat, 4);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower4EuchreInTrash()
        {
            SetupGameController("Spite", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            GoToUsePowerPhase(baccarat);
            //Pick deal 2 damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            PutInTrash("AfterlifeEuchre", 2);
            PutInTrash("AfterlifeEuchre", 3);
            QuickHPStorage(spite);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-6);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePowerSelect0Euchre()
        {
            SetupGameController("Spite", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            GoToUsePowerPhase(baccarat);
            //Pick deal 2 damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            PutInTrash("AfterlifeEuchre", 2);
            PutInTrash("AfterlifeEuchre", 3);
            //SelectAndMoveACard's optional paramter does not work
            DecisionDoNotSelectCard = SelectionType.MoveCard;
            QuickHPStorage(spite);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(0);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card bot1 = GetCard("RaptorBot", 0);
            Card bot2 = GetCard("RaptorBot", 1);
            PlayCard(bot1);
            PutInTrash(bot2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(bot1, bot2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1Villain()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card mdp = GetCardInPlay("MobileDefensePlatform", 0);
            Card mdp2 = GetCard("MobileDefensePlatform", 1);
            PlayCard(mdp);
            PutInTrash(mdp2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(mdp, mdp2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1EnvNonTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card throng = GetCard("MagmarianThrong", 0);
            Card throng2 = GetCard("MagmarianThrong", 1);
            PlayCard(env, throng);
            PutInTrash(env, throng2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(throng, throng2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1NotFailOnNoDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap2Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            //One target deals itself 1 energy damage.
            DecisionSelectCard = bunker.CharacterCard;
            QuickHPStorage(bunker);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap2Villain()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToUseIncapacitatedAbilityPhase(baccarat);
            //One target deals itself 1 energy damage.
            DecisionSelectCard = mdp;
            QuickHPStorage(mdp);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap3Yes()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Each hero character may deal themselves 1 toxic damage to draw a card now.
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            QuickHandCheck(1, 1, 1);
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap3No()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            DecisionDoNotSelectFunction = true;

            //Each hero character may deal themselves 1 toxic damage to draw a card now.
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            QuickHandCheck(0, 0, 0);
            QuickHPCheck(0, 0, 0);
        }
    }
}

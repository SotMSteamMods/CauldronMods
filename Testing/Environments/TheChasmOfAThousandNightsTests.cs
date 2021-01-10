using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheChasmOfAThousandNightsTests : BaseTest
    {

        #region ChasmrHelperFunctions

        protected TurnTakerController chasm { get { return FindEnvironment(); } }
        protected bool IsDjinn(Card card)
        {
            return card.DoKeywordsContain("djinn");
        }
        protected bool IsNature(Card card)
        {
            return card.DoKeywordsContain("nature");
        }

        protected Card chasmCard { get { return FindCardsWhere(c => c.Identifier == "TheChasmOfAThousandNights", realCardsOnly: false).First(); } }

        #endregion

        [Test()]
        public void TestChasmWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Nature_IsNature([Values("Noxious", "Vicious", "SurprisinglyAgile", "Thieving", "RatherFriendly", "Malevolent")] string nature)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();

            GoToPlayCardPhase(chasm);
            //play a djinn so we have something to go to
            Card djinn = PlayCard("HighTemoq");
            Card card = GetCard(nature);
            if (!card.IsInPlayAndHasGameText)
            {
                FlipCard(card);
                MoveCard(chasm, card, djinn.NextToLocation);
            }
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nature", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_Djinn_IsDjinn([Values("GrandAmaraqiel", "HighMhegas", "HighTemoq", "Axion", "Tevael", "Gul")] string djinn
            )
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();

            GoToPlayCardPhase(chasm);

            Card card = PlayCard(djinn);

            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "djinn", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("BeyondTheVeil", "IreOfTheDjinn", "CrumblingRuins")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();

            GoToPlayCardPhase(chasm);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }
        [Test()]
        public void TestNaturesReturnWhenAbandoned_NextToDestroyed()
        {
            //NOTE: This test fails, but the effect happens as expected in UI
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();

            Card djinn = PlayCard("HighTemoq");
            Card nature = djinn.NextToLocation.TopCard;
            AssertNotFlipped(nature);
            AssertNextToCard(nature, djinn);
            DestroyCard(djinn, baron.CharacterCard);
            AssertUnderCard(chasmCard, nature);
            AssertFlipped(nature);

        }

        [Test()]
        public void TestNaturesReturnWhenAbandoned_NextToReturn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy/FreedomFiveLegacyCharacter", "Haka", "Cauldron.TheChasmOfAThousandNights");
            StartGame();

            Card djinn = PlayCard("HighTemoq");
            Card nature = djinn.NextToLocation.TopCard;
            AssertNotFlipped(nature);
            AssertNextToCard(nature, djinn);
            DecisionSelectFunction = 0;
            DecisionSelectCard = djinn;
            UsePower(legacy);
            AssertUnderCard(chasmCard, nature);
            AssertFlipped(nature);

        }
    }
}

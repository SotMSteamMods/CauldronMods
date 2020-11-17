using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Starlight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NightloreCouncilStarlightTests : BaseTest
    {
        #region StarlightHelperFunctions
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }

        protected Card terra { get { return GetCard("StarlightOfTerraCharacter"); } }
        protected Card asheron { get { return GetCard("StarlightOfAsheronCharacter"); } }
        protected Card cryos { get { return GetCard("StarlightOfCryosFourCharacter"); } }
        #endregion
        [Test()]
        public void TestGenesisLoads()
        {
            var promoDict = new Dictionary<string, string> { };
            promoDict["Cauldron.Starlight"] = "GenesisStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "Megalopolis" }, false, promoDict);

            Assert.AreEqual(4, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(GenesisStarlightCharacterCardController), starlight.CharacterCardController);

            Assert.AreEqual(29, starlight.CharacterCard.HitPoints);
        }
        [Test()]
        public void TestNightloreCouncilLoads()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "Megalopolis" }, false, nightloreDict);

            Assert.AreEqual(4, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(NightloreCouncilStarlightCharacterCardController), starlight.CharacterCardController);



            Log.Debug("But before all that, let's see if we can make a base Starlight!");
            var baseType = ModHelper.GetTypeForCardController("Starlight", "StarlightCharacter");
            var baseDefinition = starlight.TurnTaker.DeckDefinition.CardDefinitions.Where((CardDefinition cd) => cd.Identifier == "StarlightCharacter").FirstOrDefault();
            var baseCard = new Card(baseDefinition, starlight.TurnTaker, 0);
            var acceptableCard = Activator.CreateInstance(baseType, baseCard, starlight);


            var terraType = ModHelper.GetTypeForCardController("Starlight", "StarlightOfTerraCharacter");
            Log.Debug(terraType == null ? "Couldn't find Terra type" : "Found Terra type");
            Log.Debug("Deck Identifier: " + starlight.TurnTaker.DeckDefinition.Identifier);
            Log.Debug(typeof(StarlightOfTerraCharacterCardController).Namespace);

            string charID = "StarlightOfTerraCharacter";

            Log.Debug("Trying to load " + charID + "...");
            CardDefinition definer = starlight.TurnTaker.DeckDefinition.PromoCardDefinitions.Where((CardDefinition cd) => cd.Identifier == charID).FirstOrDefault();
            Log.Debug(definer == null ? "Couldn't find definition" : "Found definition");

            Card newCard = new Card(definer, starlight.TurnTaker, 0, false);
            Log.Debug(newCard == null ? "Card creation failed" : "Card creation successful, title is " + newCard.Title);

            string defaultSearchSpace = String.Format("Handelabra.Sentinels.Engine.Controller.{0}.{1}CardController", starlight.TurnTaker.DeckDefinition.Identifier, newCard.Definition.PromoIdentifier);
            string overrideSearchSpace = String.Format("Handelabra.Sentinels.Engine.Controller.{0}.{1}CardController", "Cauldron.Starlight", newCard.Definition.PromoIdentifier);
            Log.Debug("CardControllerFactory would look in " + defaultSearchSpace);
            var soughtType = Type.GetType(defaultSearchSpace);

            Log.Debug(soughtType == null ? "Couldn't find it there.": "Something was found, though");

            Log.Debug("So let's try " + overrideSearchSpace);

            soughtType = Type.GetType(overrideSearchSpace);

            Log.Debug(soughtType == null ? "That didn't work either" : "And that's where it is.");

            Log.Debug("ModHelper can find it, though. Look! It's in " + terraType.Namespace);

            Log.Debug("So then it goes into Activator.CreateInstance...");
            object obj = Activator.CreateInstance(terraType, newCard, starlight);
            Log.Debug(obj == null ? "That fails, theough" : "Which we get something from");

        }
        [Test()]
        public void TestNightloreCouncilIndividualsLoad()
        {
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Legacy", "Megalopolis" }, false, nightloreDict);
            StartGame();

            Assert.AreEqual(13, terra.HitPoints);
            Assert.AreEqual(12, asheron.HitPoints);
            Assert.AreEqual(15, cryos.HitPoints);

            Assert.IsNull(starlight.CharacterCard);
        }

    }
}
using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.ScreaMachine;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class ScreaMachineTestsTests : BaseTest
    {
        #region ScreaMachineTestsHelperFunctions

        protected TurnTakerController scream { get { return FindVillain("ScreaMachine"); } }

        protected Card slice { get { return FindCardInPlay("SliceCharacter"); } }
        protected Card valentine { get { return FindCardInPlay("ValentineCharacter"); } }
        protected Card bloodlace { get { return FindCardInPlay("BloodlaceCharacter"); } }
        protected Card rickyg { get { return FindCardInPlay("RickyGCharacter"); } }
        protected Card setlist { get { return GetCard("TheSetList"); } }

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private void AssertHasAbility(string abilityKey, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                int number = card.GetNumberOfActivatableAbilities(abilityKey);
                Assert.GreaterOrEqual(number, 1);
            }
        }

        #endregion

        [Test()]
        public void TestScreaMachineLoadedProperly()
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(scream);
            Assert.IsInstanceOf(typeof(ScreaMachineTurnTakerController), scream);

            Assert.IsNotNull(slice);
            Assert.IsInstanceOf(typeof(SliceCharacterCardController), FindCardController(slice));
            Assert.AreEqual(28, slice.HitPoints);

            Assert.IsNotNull(valentine);
            Assert.IsInstanceOf(typeof(ValentineCharacterCardController), FindCardController(valentine));
            Assert.AreEqual(31, valentine.HitPoints);

            Assert.IsNotNull(bloodlace);
            Assert.IsInstanceOf(typeof(BloodlaceCharacterCardController), FindCardController(bloodlace));
            Assert.AreEqual(26, bloodlace.HitPoints);

            Assert.IsNotNull(rickyg);
            Assert.IsInstanceOf(typeof(RickyGCharacterCardController), FindCardController(rickyg));
            Assert.AreEqual(35, rickyg.HitPoints);

            Assert.IsNotNull(setlist);
            Assert.IsInstanceOf(typeof(TheSetListCardController), FindCardController(setlist));
        }

        [Test()]
        public void TestScreaMachineDeckList()
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Megalopolis");

            AssertHasKeyword("guitarist", new string[] {
                "ShredZone",
                "SlicesAxe"
            });
            AssertHasAbility("{Guitar}", new string[] {
                "ShredZone",
                "SlicesAxe"
            });
            AssertHasKeyword("bassist", new string[] {
                "Biosurge",
                "CantStopTheMusic"
            });
            AssertHasAbility("{Bass}", new string[] {
                "Biosurge",
                "CantStopTheMusic"
            });
            AssertHasKeyword("drummer", new string[] {
                "PoundingRhythm",
                "TectonicBeat"
            });
            AssertHasAbility("{Drum}", new string[] {
                "PoundingRhythm",
                "TectonicBeat"
            });
            AssertHasKeyword("vocalist", new string[] {
                "MentalLink",
                "IrresistibleVoice",
                "HypnotizeTheCrowd"
            });
            AssertHasAbility("{Vocal}", new string[] {
                "MentalLink",
                "IrresistibleVoice",
                "HypnotizeTheCrowd"
            });
            AssertHasKeyword("one-shot", new string[] {
                "HarshNote",
                "UpToEleven",
                "DeathMetal",
                "NothingButHits",
                "ScreamAndShout",
                "LiveInConcert"
            });
            AssertHasKeyword("ongoing", new string[] {
                "PercussiveWave"
            });

        }


        [Test()]
        public void TestScreaMachineGameStart()
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis");
            AssertInPlayArea(scream, setlist);
            AssertNotFlipped(setlist);
            QuickShuffleStorage(scream);
            StartGame();

            QuickShuffleCheck(1);
            AssertInPlayArea(scream, slice);
            AssertNotFlipped(slice);
            AssertInPlayArea(scream, bloodlace);
            AssertNotFlipped(bloodlace);
            AssertInPlayArea(scream, valentine);
            AssertNotFlipped(valentine);
            AssertInPlayArea(scream, rickyg);
            AssertNotFlipped(rickyg);
            AssertInPlayArea(scream, setlist);
            AssertFlipped(setlist);

            HashSet<string> _bandKeywords = new HashSet<string>(StringComparer.Ordinal)
            {
                "vocalist",
                "guitarist",
                "bassist",
                "drummer"
            };

            int inPlay = 0;
            foreach (var card in FindCardsWhere(c => GameController.GetAllKeywords(c, true, true).Any(str => _bandKeywords.Contains(str))))
            {
                if (card.Location != setlist.UnderLocation)
                {
                    AssertAtLocation(card, scream.TurnTaker.PlayArea);
                    inPlay++;
                }
            }

            Assert.AreEqual(1, inPlay, $"Should have 1 band cards in play");
        }

        [Test()]
        public void TestScreaMachineAdvancedGameStart()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: true);
            AssertInPlayArea(scream, setlist);
            AssertNotFlipped(setlist);

            QuickShuffleStorage(scream.TurnTaker.Deck, setlist.UnderLocation);
            StartGame();
            QuickShuffleCheck(1, 1);

            AssertInPlayArea(scream, slice);
            AssertNotFlipped(slice);
            AssertInPlayArea(scream, bloodlace);
            AssertNotFlipped(bloodlace);
            AssertInPlayArea(scream, valentine);
            AssertNotFlipped(valentine);
            AssertInPlayArea(scream, rickyg);
            AssertNotFlipped(rickyg);
            AssertInPlayArea(scream, setlist);
            AssertFlipped(setlist);

            HashSet<string> _bandKeywords = new HashSet<string>(StringComparer.Ordinal)
            {
                "vocalist",
                "guitarist",
                "bassist",
                "drummer"
            };

            int inPlay = 0;
            foreach (var card in FindCardsWhere(c => GameController.GetAllKeywords(c, true, true).Any(str => _bandKeywords.Contains(str))))
            {
                if (card.Location != setlist.UnderLocation)
                {
                    AssertAtLocation(card, scream.TurnTaker.PlayArea);
                    inPlay++;
                }
            }

            Assert.AreEqual(this.GameController.Game.H - 2 + 1, inPlay, $"Should have {GameController.Game.H - 2 + 1} band cards in play");
        }
    }
}

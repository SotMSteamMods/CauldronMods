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
    public class ScreaMachineTests : CauldronBaseTest
    {
        #region ScreaMachineTestsHelperFunctions

        protected Card slice { get { return FindCardInPlay("SliceCharacter"); } }
        protected Card valentine { get { return FindCardInPlay("ValentineCharacter"); } }
        protected Card bloodlace { get { return FindCardInPlay("BloodlaceCharacter"); } }
        protected Card rickyg { get { return FindCardInPlay("RickyGCharacter"); } }
        protected Card setlist { get { return GetCard("TheSetList"); } }

        protected void AddPlayCardWhenVillainCardPlayed(TurnTakerController ttc, Location deck, CardSource cardSource)
        {
            Trigger<CardEntersPlayAction> trigger = new Trigger<CardEntersPlayAction>(GameController, (CardEntersPlayAction pca) =>  GameController.Game.Journal.CardEntersPlayEntriesThisTurn().Count() < 4 && pca.CardEnteringPlay != null && pca.CardEnteringPlay.IsVillain,
                (CardEntersPlayAction pca) => GameController.PlayTopCardOfLocation(ttc, deck, cardSource: cardSource, showMessage: true), new TriggerType[] { TriggerType.PlayCard }, TriggerTiming.After, cardSource: cardSource);
            this.GameController.AddTrigger(trigger);
        }

        private Card SetupBandCard(string identifier)
        {
            var card = FindCard(c => c.Identifier == identifier);
            if (card.Location == setlist.UnderLocation)
            {
                FlipCard(card);
                PlayCard(card);
            }
            return card;
        }

        private void AssertNumberOfActivatableAbility(Card card, string key, int number)
        {
            var cc = FindCardController(card);
            var abilities = cc.GetActivatableAbilities(key).Count();
            Assert.AreEqual(number, abilities, $"{card.Title} does not have the correct number of {(key is null ? "" : key)} abilities");
        }

        private void RemoveAllBandMateCards()
        {
            foreach (var card in FindCardsWhere(c => c.DoKeywordsContain(ScreaMachineBandmate.Keywords, true, true)))
            {
                MoveCard(scream, card, scream.TurnTaker.OutOfGame, false, false, true);
                AssertOutOfGame(card);
            }
        }

        private void CleanUpSetup()
        {
            foreach (var card in FindCardsWhere(c => c.Location == scream.TurnTaker.PlayArea && !c.IsCharacter))
            {
                MoveCard(scream, card, setlist.UnderLocation, true, false, true);
                if (!card.IsFlipped)
                {
                    FlipCard(card);
                }
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
        [Sequential]
        public void TestBandCardIndestructible([Values("ShredZone", "SlicesAxe", "Biosurge", "CantStopTheMusic", "PoundingRhythm", "TectonicBeat", "MentalLink", "IrresistibleVoice", "HypnotizeTheCrowd")] string bandId)
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card band = GetCard(bandId);
            if(!band.IsInPlayAndHasGameText)
            {
                PlayCard(band);
            }

            AssertInPlayArea(scream, band);
            DestroyCard(band, haka.CharacterCard);
            AssertInPlayArea(scream, band);
        }

        [Test()]
        public void TestCardsUnderSetlistAreSafeFromActions()
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            foreach(Card c in setlist.UnderLocation.Cards)
            {
                DestroyCard(c, haka.CharacterCard);
                AssertUnderCard(setlist, c);
            }
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
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Tachyon", "Bunker", "Megalopolis" }, advanced: true);
            AssertInPlayArea(scream, setlist);
            AssertNotFlipped(setlist);

            QuickShuffleStorage(scream.TurnTaker.Deck, scream.TurnTaker.Revealed);
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
            AssertNumberOfCardsInRevealed(scream, 0);
            AssertFlipped(setlist.UnderLocation.Cards.ToArray());
        }

        [Test()]
        public void TestScreaMachineCharacterFlip([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var memberCard = GetCard(member.GetIdentifier());
            AssertNotFlipped(memberCard);
            AssertHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
            AssertMaximumHitPoints(memberCard, memberCard.Definition.HitPoints.Value);

            var cards = FindCardsWhere(c => c.DoKeywordsContain(member.GetKeyword(), true, true));
            foreach (var card in cards)
            {
                if (card.Location == setlist.UnderLocation)
                {
                    FlipCard(card);
                    PlayCard(card);
                }

                AssertNotFlipped(card);
                AssertInPlayArea(scream, card);
            }

            AssertFlipped(memberCard);
            AssertHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
            AssertMaximumHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
        }

        [Test()]
        public void TestScreaMachineCharacterFlip_Challenge([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            var memberCard = GetCard(member.GetIdentifier());
            AssertNotFlipped(memberCard);
            AssertHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
            AssertMaximumHitPoints(memberCard, memberCard.Definition.HitPoints.Value);

            //should flip after 2 cards rather than 3
            var cards = FindCardsWhere(c => c.DoKeywordsContain(member.GetKeyword(), true, true)).Take(2);
            foreach (var card in cards)
            {
                if (card.Location == setlist.UnderLocation)
                {
                    FlipCard(card);
                    PlayCard(card);
                }

                AssertNotFlipped(card);
                AssertInPlayArea(scream, card);
            }

            AssertFlipped(memberCard);
            AssertHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
            AssertMaximumHitPoints(memberCard, memberCard.Definition.HitPoints.Value);
        }

        [Test()]
        public void TestSetListCharacterRemovedFromGameDestroy([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var memberCard = GetCard(member.GetIdentifier());

            DestroyCard(memberCard);

            AssertAtLocation(memberCard, scream.TurnTaker.OutOfGame);
        }

        [Test()]
        public void TestSetListCharacterRemovedFromGameDamage([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();

            var memberCard = GetCard(member.GetIdentifier());

            DealDamage(legacy, memberCard, 99, DamageType.Cold);

            AssertAtLocation(memberCard, scream.TurnTaker.OutOfGame);
        }

        [Test()]
        public void TestSetListRevealCard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: false);
            StartGame();
            GoToPlayCardPhase(scream);

            PrintSeparator("StartTest");

            List<Card> revealed = new List<Card>();
            //I use legacy as the revealer to confirm it's ANY reveal
            var cards = setlist.UnderLocation.GetTopCards(2).ToList();
            var c1 = cards[0];
            var c2 = cards[1];
            Console.WriteLine("Card to Reveal: " + c1.Title);

            RunCoroutine(GameController.RevealCards(legacy, setlist.UnderLocation, 1, revealed, cardSource: legacy.CharacterCardController.GetCardSource()));

            AssertNumberOfCardsInRevealed(scream, 0);

            if (c1.Location == setlist.UnderLocation)
            {
                Console.WriteLine($"{c1.Title} is under the setList");
                AssertOnBottomOfLocation(c1, setlist.UnderLocation);
                AssertInPlayArea(scream, c2);
            }
            else
            {
                Console.WriteLine($"{c1.Title} not under the setList");
                AssertInPlayArea(scream, c1);
                AssertOnTopOfLocation(c2, setlist.UnderLocation);
            }
        }

        [Test()]
        public void TestSetListEndOfTurn()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            //prevent card plays
            PlayCard("TakeDown");

            GoToPlayCardPhase(scream);
            DestroyCard(slice);
            DestroyCard(valentine);

            AssertInPlayArea(scream, rickyg);
            AssertInPlayArea(scream, bloodlace);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, rickyg, bloodlace);

            AssertNextMessage("Take Down prevented ScreaMachine from playing cards.");
            GoToEndOfTurn(scream);
            QuickHPCheck(-2, -2, -2, -2, 0, 0);

            //There's an end of turn card play here
        }

        [Test]
        public void TestSetListAdvancedDestroyOngoing([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: true);
            StartGame();

            var bandmate = GetCard(member.GetIdentifier());

            var card = PlayCard("Dominion");
            AssertInPlayArea(haka, card);

            FlipCard(bandmate);

            AssertInTrash(card);
        }

        [Test]
        public void TestSetListAdvancedDestroyEquipment([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Megalopolis" }, advanced: true);
            StartGame();

            var bandmate = GetCard(member.GetIdentifier());

            var card = PlayCard("TheStaffOfRa");
            AssertInPlayArea(ra, card);

            FlipCard(bandmate);

            AssertInTrash(card);
        }


        [Test()]
        public void TestSliceAbility()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Slice);
            AssertNumberOfActivatableAbility(slice, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, slice);
            QuickHPCheck(0, -3, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestSliceUltimate()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Slice);

            FlipCard(slice);
            AssertNumberOfActivatableAbility(slice, key, 0);

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            GoToEndOfTurn(scream);

            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(ra.CharacterCard, haka.CharacterCard, 2, DamageType.Cold); //cannot deal damage
            QuickHPCheck(0, -3, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestSliceUltimate_Freeze()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "TheVisionary", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TelekineticCocoon");
            DealDamage(slice, c => c.IsHeroCharacterCard, 999, DamageType.Energy);

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Slice);

            FlipCard(slice);

            GoToEndOfTurn(scream);

        }


        [Test()]
        public void TestValentineAbility()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(valentine, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, valentine);
            QuickHPCheck(-2, 0, -2, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestValentineUltimate()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);

            FlipCard(valentine);
            AssertNumberOfActivatableAbility(valentine, key, 0);
            //villain damage increased by 1

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            DealDamage(bloodlace, ra, 1, DamageType.Energy);
            QuickHPCheck(0, -2, 0, 0, 0, 0, 0, 0);

            QuickHPUpdate();
            GoToEndOfTurn(scream);
            //H -1 sonic + 1 & H - 2 + 1
            //3 + 1 & 2 + 1 = 7
            QuickHPCheck(0, 0, -7, 0, 0, 0, 0, 0);
        }


        [Test()]
        public void TestBloodlaceAbility()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Bloodlace);
            AssertNumberOfActivatableAbility(bloodlace, key, 1);

            SetHitPoints(slice, 10);
            SetHitPoints(bloodlace, 10);
            SetHitPoints(valentine, 10);
            SetHitPoints(rickyg, 10);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, bloodlace);
            QuickHPCheck(0, 0, 0, 0, 2, 0, 2, 2);
        }

        [Test()]
        public void TestBloodlaceUltimate()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Bloodlace);

            FlipCard(bloodlace);
            AssertNumberOfActivatableAbility(bloodlace, key, 0);

            //ensure ricky is the lowest
            SetHitPoints(rickyg, 10);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            GoToEndOfTurn(scream);

            //H damage, H healing
            QuickHPCheck(0, 0, -4, 0, 0, 0, 0, 4);
        }


        [Test()]
        public void TestRickyGAbility()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.RickyG);
            AssertNumberOfActivatableAbility(rickyg, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            AssertNumberOfStatusEffectsInPlay(0);
            ActivateAbility(key, rickyg);
            AssertNumberOfStatusEffectsInPlay(1);

            DealDamage(ra, bloodlace, 2, DamageType.Radiant);
            QuickHPCheck(0, 0, 0, 0, 0, -1, 0, 0);
        }

        [Test()]
        public void TestRickyGUltimateImmunity()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.RickyG);

            FlipCard(rickyg);
            AssertNumberOfActivatableAbility(rickyg, key, 0);


            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            DealDamage(legacy, slice, 2, DamageType.Radiant, true);
            DealDamage(legacy, rickyg, 2, DamageType.Radiant);
            //slice immune to damage, rickyg is not
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, -2);

            //indestructible
            DestroyCard(slice);
            AssertInPlayArea(scream, slice);

            DestroyCard(rickyg);

            //immunity ends once ricky leaves play
            var s = slice; //FindCard fails once slice is destoryed, so buffer locally
            DestroyCard(s);
            AssertOutOfGame(s);

            QuickHPStorage(bloodlace);
            DealDamage(legacy, bloodlace, 5, DamageType.Cold);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestRickyGUltimateDrumDiscard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            FlipCard(rickyg);
            var card = PutOnDeck("UpToEleven");
            AssertInDeck(card);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            GoToEndOfTurn(scream);

            AssertInTrash(card);

            QuickHPCheck(-3, -3, -3, -3, 0, 0, 0, 0);
        }

        [Test()]
        public void TestRickyGUltimateNotDrumDiscard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            PlayCard("TakeDown");

            FlipCard(rickyg);
            var card = PutOnDeck("DeathMetal");
            AssertInDeck(card);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            GoToEndOfTurn(scream);

            AssertInTrash(card);

            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
        }

        [Test]
        public void TestBandCardRevealsIfBandMateMissing([Values(ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Bloodlace, ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.RickyG)] ScreaMachineBandmate.Value member)
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false, randomSeed: -1112568770);
            StartGame();

            //reset the band cards
            foreach (var card in FindCardsWhere(c => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain(ScreaMachineBandmate.Keywords, true, true)))
            {
                if (!card.IsFlipped)
                    FlipCard(card);
                MoveCard(scream, card, setlist.UnderLocation);
            }

            //destroy all other characters
            foreach (var id in ScreaMachineBandmate.Identifiers.Where(s => s != member.GetIdentifier()))
            {
                DestroyCard(GetCard(id));
            }

            int count = GetNumberOfCardsInPlay(c => c.Location == scream.TurnTaker.PlayArea && c.IsInPlayAndNotUnderCard);

            //we are not going to reveal cards undersetlist, just flip and play specific cards.
            //first we hard play the cards for the active member leaving only the deadies cards under.
            //Then we play the top card of the under, and reset of the under playitself out
            foreach (var card in FindCardsWhere(c => c.Location == setlist.UnderLocation && c.DoKeywordsContain(member.GetKeyword(), true, true)))
            {
                if (card.IsFlipped)
                    FlipCard(card);
                PlayCard(card, true);
            }

            PrintCardsInPlayWithGameText(c => c.Location == scream.TurnTaker.PlayArea && c.IsInPlayAndNotUnderCard);
            AssertNumberOfCardsInPlay(c => c.Location == scream.TurnTaker.PlayArea && c.IsInPlayAndNotUnderCard, count + 3);

            var c1 = setlist.UnderLocation.TopCard;
            if (c1.IsFlipped)
                FlipCard(c1);
            PlayCard(c1, true);

            //The rest of the under cards should be chain played out
            AssertNumberOfCardsInPlay(c => c.Location == scream.TurnTaker.PlayArea && c.IsInPlayAndNotUnderCard, count + 12);
        }


        [Test()]
        public void TestShredZone()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("ShredZone");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Slice);
            AssertNumberOfActivatableAbility(card, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(0, -1, 0, -1, 0, 0, 0, 0);
        }

        [Test()]
        public void TestSlicesAxe()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("SlicesAxe");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Slice);
            AssertNumberOfActivatableAbility(card, key, 1);

            DrawCard(legacy); //make legacy the one with the most cards in hand

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(-3, 0, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestCantStopTheMusic()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("CantStopTheMusic");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Bloodlace);
            AssertNumberOfActivatableAbility(card, key, 1);

            SetHitPoints(rickyg, 10);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 2);
        }


        [Test()]
        public void TestBiosurge()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("Biosurge");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Bloodlace);
            AssertNumberOfActivatableAbility(card, key, 1);

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            AssertNumberOfStatusEffectsInPlay(1);

            //next bloodlace damage is increased, so not slice
            DealDamage(slice, ra, 1, DamageType.Lightning);
            DealDamage(bloodlace, legacy, 1, DamageType.Lightning);
            QuickHPCheck(-3, -1, 0, 0, 0, 0, 0, 0);

            //only next damage
            AssertNumberOfStatusEffectsInPlay(0);
        }



        [Test()]
        public void TestPoundingRhythm()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("PoundingRhythm");
            PlayCard("TaMoko"); //to check damage is irreducible

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.RickyG);
            AssertNumberOfActivatableAbility(card, key, 1);

            AssertNumberOfStatusEffectsInPlay(0);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);

            //1 irreducible melee damage and reduce the next damage dealt by 2.
            QuickHPCheck(0, 0, -1, 0, 0, 0, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPUpdate();
            DealDamage(haka, slice, 3, DamageType.Lightning);
            QuickHPCheck(0, 0, 0, 0, -1, 0, 0, 0);
            //only next damage
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test()]
        public void TestTectonicBeat()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("TectonicBeat");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.RickyG);
            AssertNumberOfActivatableAbility(card, key, 1);

            AssertNumberOfStatusEffectsInPlay(0);

            ActivateAbility(key, card);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            DealDamage(ra, rickyg, 2, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, -1);
        }

        [Test()]
        public void TestMentalLink()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("MentalLink");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            DecisionAutoDecideIfAble = true;
            int deck = GetNumberOfCardsInDeck(scream);
            ActivateAbility(key, card);
            AssertNumberOfCardsInDeck(scream, deck - 1);
        }

        [Test()]
        public void TestMentalLink_PlayCardWhenVillainCardIsPlayed()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("MentalLink");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            //make haka the highest to reduce desicion spam
            SetHitPoints(legacy, 18);
            SetHitPoints(ra, 13);
            SetHitPoints(bunker, 10);
            //some room for healing, but main hp positions
            SetHitPoints(slice, 23);
            SetHitPoints(bloodlace, 26);
            SetHitPoints(valentine, 21);
            SetHitPoints(rickyg, 30);

            PutOnDeck("NothingButHits");
            PutOnDeck("PercussiveWave");
            PrintSeparator("START TEST");

            DecisionAutoDecideIfAble = true;
            int deck = GetNumberOfCardsInDeck(scream);
            AddPlayCardWhenVillainCardPlayed(scream, scream.TurnTaker.Deck, FindCardController(valentine).GetCardSource());
            ActivateAbility(key, card);
            AssertNumberOfCardsInDeck(scream, deck - 3);
        }

        [Test()]
        public void TestMentalLink_PlayCardWhenVillainDeckIsEmpty()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            DestroyNonCharacterVillainCards();
            MoveAllCards(scream, scream.TurnTaker.Deck, scream.TurnTaker.Trash);

            var card = SetupBandCard("MentalLink");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            StackAfterShuffle(scream.TurnTaker.Deck, new string[] { "Biosurge" });
            QuickShuffleStorage(scream.TurnTaker.Deck);
            ActivateAbility(key, card);
            QuickShuffleCheck(1);
            AssertIsInPlay("Biosurge");
        }
        [Test()]
        public void TestMentalLink_OtherVillainCardPlayDuringMentalLinkPlay()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Stuntman", "Mordengrad" }, advanced: false);
            StartGame();

            var card = SetupBandCard("MentalLink");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            //prevent 
            SetHitPoints(valentine, 20);
            SetHitPoints(bloodlace, 10);

            QuickHPStorage(valentine, bloodlace);
            Card tank = PlayCard("RemoteWalkingTank");
            SetHitPoints(tank, 3);
            Card steal = PlayCard("StealTheScene");
            PlayCard("Fortitude");
            PlayCard("TheStaffOfRa");
            PlayCard("TaMoko");
            DecisionAutoDecideIfAble = true;
            DecisionSelectTarget = tank;
            Card wave = PutOnDeck("PercussiveWave");
            Card metal = PutOnDeck("DeathMetal");
            int deck = GetNumberOfCardsInDeck(scream);
            ActivateAbility(key, card);

            AssertIsInPlay(wave);
            QuickHPCheck(0, 0);
            DealDamage(legacy, bloodlace, 2, DamageType.Melee);
            QuickHPCheck(0, -1);
        }
        [Test()]
        public void TestIrresistibleVoice()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("IrresistibleVoice");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(0, 0, -2, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestHypnotizeTheCrowd_NoEnvironment()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("HypnotizeTheCrowd");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestHypnotizeTheCrowd_SelectEnvironment()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("HypnotizeTheCrowd");
            var e1 = PlayCard("PlummetingMonorail");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            DecisionSelectCard = e1;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            ActivateAbility(key, card);
            QuickHPCheck(0, 0, -3, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestHypnotizeTheCrowd_SelectEnvironment_NonTarget()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("HypnotizeTheCrowd");
            var e1 = PlayCard("PlummetingMonorail");
            var e2 = PlayCard("TrafficPileup");
            var e3 = PlayCard("PoliceBackup");

            string key = ScreaMachineBandmate.GetAbilityKey(ScreaMachineBandmate.Value.Valentine);
            AssertNumberOfActivatableAbility(card, key, 1);

            DecisionSelectCard = e3;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            AssertNextDecisionChoices(included: new List<Card>() { e1, e2, e3 });
            ActivateAbility(key, card);
            QuickHPCheck(0, 0, -3, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestHypnotizeTheCrowd_ImmuneToEnviroment()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            var card = SetupBandCard("HypnotizeTheCrowd");
            var e1 = PlayCard("ImpendingCasualty");

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            DealDamage(e1, valentine, 5, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
        }

        //green - second lowest H -1
        //blue - 2 highest, H - 2
        //pink - each other regains
        //orange - status effect

        [Test()]
        public void TestHarshNote()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("HarshNote");

            //green, blue
            //green - second lowest H -1
            //blue - 2 highest, H - 2

            QuickHPCheck(-2, -7, -2, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestUpToEleven()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            var band = MoveCard(scream, "SlicesAxe", setlist.UnderLocation);
            PrintSeparator("TEST");

            SetHitPoints(slice, 15);
            SetHitPoints(bloodlace, 15);
            SetHitPoints(valentine, 15);
            SetHitPoints(rickyg, 10);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("UpToEleven");

            //pink, orange
            //pink - each other regains 2
            //orange - status effect
            //card - lowest regains 3
            QuickHPCheck(-1, -1, -1, -1, 2, 0, 2, 5);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertInPlayArea(scream, band);
        }


        [Test()]
        public void TestPercussiveWave()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");
            PlayCard("TakeDown");

            SetHitPoints(slice, 22);
            SetHitPoints(bloodlace, 22);
            SetHitPoints(valentine, 22);
            SetHitPoints(rickyg, 20);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("PercussiveWave", 0, true);
            AssertInPlayArea(scream, card);

            //orange, green
            //orange - status effect
            //green - second lowest H -1
            //card - lowest regains 3
            QuickHPCheck(0, -3, 0, 0, 0, 0, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);
            //RickyG gets the status effect

            QuickHPUpdate();
            DealDamage(legacy, slice, 5, DamageType.Cold); //5 reduced to 3
            DealDamage(legacy, bloodlace, 4, DamageType.Cold); //4 reduced to 2
            DealDamage(legacy, valentine, 3, DamageType.Cold); //3 reduced to 2
            QuickHPCheck(0, 0, 0, 0, -3, -2, -2, 0);

            QuickHPUpdate();
            DealDamage(legacy, slice, 2, DamageType.Cold); //2 not reduced to 2
            DealDamage(legacy, bloodlace, 1, DamageType.Cold); //1 not reduced to 1
            QuickHPCheck(0, 0, 0, 0, -2, -1, 0, 0);

            GoToEndOfTurn(env);
            PlayCard("TakeDown");

            //destroyed at the start of turn
            AssertInPlayArea(scream, card);
            GoToStartOfTurn(scream);
            AssertInTrash(card);

        }


        [Test()]
        public void TestDeathMetal()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");

            SetHitPoints(slice, 15);
            SetHitPoints(bloodlace, 15);
            SetHitPoints(valentine, 15);
            SetHitPoints(rickyg, 15);

            var c1 = PlayCard("DangerSense");
            var c2 = PlayCard("TheStaffOfRa");
            var c3 = PlayCard("Dominion");

            DecisionSelectCards = new[] { c1, c2, null, slice, haka.CharacterCard, /*bunker,*/ legacy.CharacterCard, /* haka, */ };
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);

            var card = PlayCard("DeathMetal");
            AssertInTrash(card);

            //blue, pink
            //blue - 2 highest, H - 2
            //pink - each other regains 2
            //bunker and haka take 3
            QuickHPCheck(-2, 0, -5, -3, 2, 0, 2, 2);
            AssertInTrash(c1);
            AssertInTrash(c2);
            AssertInPlayArea(haka, c3);
        }

        [Test()]
        public void TestNothingButHits()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("NothingButHits");

            //blue, orange
            //blue - 2 highest, H - 2
            //orange - status effect

            QuickHPCheck(-4, -2, -6, 0, 0, 0, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);
        }

        [Test()]
        public void TestScreamAndShout()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");

            SetHitPoints(slice, 15);
            SetHitPoints(bloodlace, 15);
            SetHitPoints(valentine, 15);
            SetHitPoints(rickyg, 15);

            DecisionAutoDecideIfAble = true;
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("ScreamAndShout");

            //green, pink
            //green - second lowest H -1
            //pink - each other regains 2

            QuickHPCheck(-4, 0, -4, 0, 2, 0, 2, 2);
        }

        [Test()]
        public void TestLiveInConcert()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            RemoveAllBandMateCards();
            PrintSeparator("TEST");

            SetHitPoints(slice, 17);
            SetHitPoints(bloodlace, 15);
            SetHitPoints(valentine, 14);
            SetHitPoints(rickyg, 16);

            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 25);
            SetHitPoints(bunker, 15);

            var d1 = GetRandomCardFromHand(legacy);
            var d2 = GetRandomCardFromHand(ra);
            var d3 = GetRandomCardFromHand(haka);
            var d4 = GetRandomCardFromHand(bunker);

            DecisionAutoDecideIfAble = true;
            DecisionSelectTurnTakers = new[] { legacy.TurnTaker, ra.TurnTaker, haka.TurnTaker, bunker.TurnTaker };
            DecisionSelectCards = new[] { d1, d2, d3, d4 };
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, bunker.CharacterCard, slice, bloodlace, valentine, rickyg);
            var card = PlayCard("LiveInConcert");

            //green, blue, pink, orange
            //green - second lowest H -1
            //blue - 2 highest, H - 2
            //pink - each other regains
            //orange - status effect

            QuickHPCheck(-2, -3, -6, 0, 2, 0, 2, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            AssertInTrash(d1, d2, d3, d4);
        }

        [Test()]
        public void TestScreaMachineDefeatDamage()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            DealDamage(legacy, slice, 99, DamageType.Cold);
            DealDamage(legacy, bloodlace, 99, DamageType.Cold);
            DealDamage(legacy, valentine, 99, DamageType.Cold);
            DealDamage(legacy, rickyg, 99, DamageType.Cold);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test()]
        public void TestScreaMachineDefeatDestroy()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();

            DestroyCard(slice, legacy.CharacterCard);
            DestroyCard(bloodlace, legacy.CharacterCard);
            DestroyCard(valentine, legacy.CharacterCard);
            DestroyCard(rickyg, legacy.CharacterCard);

            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }
        [Test()]
        public void TestMusicianCardMessages_NonChallenge_SecondCard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            CleanUpSetup();

            var vocal1 = GetCard("IrresistibleVoice");
            var vocal2 = GetCard("HypnotizeTheCrowd");
            var vocal3 = GetCard("MentalLink");
            var axe = GetCard("SlicesAxe");

            var sliceMessage = $"Slice is keeping it mellow since there aren't any guitarist cards in play!";
            var flipMessage = "[b]Valentine - The Set List[/b]                                               \n[i]“The world's a stage, and I want the brightest spot.”[/i]";

            var messages = new string[] {
                sliceMessage,
               $"[b]Valentine[/b] steps into the limelight and plays a vocalist card!",
               sliceMessage,
               $"[b]Valentine[/b] is ramping it up and plays a vocalist card!",
               sliceMessage,
               $"The music [b]surges[/b] and a vocalist card is played! This is [b]Valentine[/b] moment!",
               flipMessage
            };

            AssertNextMessages(messages);

            MoveCard(scream, vocal1, setlist.UnderLocation);
            MoveCard(scream, vocal2, setlist.UnderLocation);
            MoveCard(scream, vocal3, setlist.UnderLocation);

            for (int i = 0; i < 3; i++)
            {
                MoveCard(scream, "SlicesAxe", setlist.UnderLocation);
                var reveal = GameController.RevealCards(scream, setlist.UnderLocation, 1, new List<Card> { });
                GameController.ExhaustCoroutine(reveal);
            }
        }
        [Test()]
        public void TestMusicianCardMessages_NonChallenge_FirstCard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, advanced: false);
            StartGame();
            CleanUpSetup();

            var vocal1 = GetCard("IrresistibleVoice");
            var vocal2 = GetCard("HypnotizeTheCrowd");
            var vocal3 = GetCard("MentalLink");
            var axe = GetCard("SlicesAxe");

            var sliceMessage = $"Slice is keeping it mellow since there aren't any guitarist cards in play!";
            var flipMessage = "[b]Valentine - The Set List[/b]                                               \n[i]“The world's a stage, and I want the brightest spot.”[/i]";

            var messages = new string[] {
               $"[b]Valentine[/b] is starting to feel it and plays a vocalist card!",
               $"[b]Valentine[/b] is ramping it up and plays a vocalist card!",
               $"The music [b]surges[/b] and a vocalist card is played! This is [b]Valentine[/b] moment!",
               flipMessage
            };

            AssertNextMessages(messages);

            MoveCard(scream, vocal2, setlist.UnderLocation);
            MoveCard(scream, vocal1, setlist.UnderLocation);
            MoveCard(scream, vocal3, setlist.UnderLocation);

            for (int i = 0; i < 3; i++)
            {
                var reveal = GameController.RevealCards(scream, setlist.UnderLocation, 1, new List<Card> { });
                GameController.ExhaustCoroutine(reveal);
                if(i == 0)
                {
                    MoveCard(scream, vocal2, setlist.UnderLocation);
                }
                if(i == 1)
                {
                    MoveCard(scream, vocal3, setlist.UnderLocation);
                }
            }
        }
        [Test()]
        public void TestMusicianCardMessages_Challenge_SecondCard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, challenge: true);
            StartGame();
            CleanUpSetup();

            var vocal1 = GetCard("IrresistibleVoice");
            var vocal2 = GetCard("HypnotizeTheCrowd");
            var vocal3 = GetCard("MentalLink");
            var axe = GetCard("SlicesAxe");

            var sliceMessage = $"Slice is keeping it mellow since there aren't any guitarist cards in play!";
            var flipMessage = "[b]Valentine - The Set List[/b]                                               \n[i]“The world's a stage, and I want the brightest spot.”[/i]";

            var messages = new string[] {
                sliceMessage,
               $"[b]Valentine[/b] steps into the limelight and plays a vocalist card!",
               sliceMessage,
               $"The music [b]surges[/b] and a vocalist card is played! This is [b]Valentine[/b] moment!",
               flipMessage,
               sliceMessage,
               $"The music [b]rages[/b] and a vocalist card is played! [b]Valentine[/b] is going into overdrive!"
            };

            AssertNextMessages(messages);

            MoveCard(scream, vocal1, setlist.UnderLocation);
            MoveCard(scream, vocal2, setlist.UnderLocation);
            MoveCard(scream, vocal3, setlist.UnderLocation);

            for (int i = 0; i < 3; i++)
            {
                MoveCard(scream, "SlicesAxe", setlist.UnderLocation);
                var reveal = GameController.RevealCards(scream, setlist.UnderLocation, 1, new List<Card> { });
                GameController.ExhaustCoroutine(reveal);
            }
        }
        [Test()]
        public void TestMusicianCardMessages_Challenge_FirstCard()
        {
            SetupGameController(new[] { "Cauldron.ScreaMachine", "Legacy", "Ra", "Haka", "Bunker", "Megalopolis" }, challenge: true);
            StartGame();
            CleanUpSetup();

            var vocal1 = GetCard("IrresistibleVoice");
            var vocal2 = GetCard("HypnotizeTheCrowd");
            var vocal3 = GetCard("MentalLink");
            var axe = GetCard("SlicesAxe");

            var sliceMessage = $"Slice is keeping it mellow since there aren't any guitarist cards in play!";
            var flipMessage = "[b]Valentine - The Set List[/b]                                               \n[i]“The world's a stage, and I want the brightest spot.”[/i]";

            var messages = new string[] {
               $"[b]Valentine[/b] is starting to feel it and plays a vocalist card!",
               $"The music [b]surges[/b] and a vocalist card is played! This is [b]Valentine[/b] moment!",
               flipMessage,
               $"The music [b]rages[/b] and a vocalist card is played! [b]Valentine[/b] is going into overdrive!"
            };

            AssertNextMessages(messages);

            MoveCard(scream, vocal2, setlist.UnderLocation);
            MoveCard(scream, vocal1, setlist.UnderLocation);
            MoveCard(scream, vocal3, setlist.UnderLocation);

            for (int i = 0; i < 3; i++)
            {
                var reveal = GameController.RevealCards(scream, setlist.UnderLocation, 1, new List<Card> { });
                GameController.ExhaustCoroutine(reveal);
                if (i == 0)
                {
                    MoveCard(scream, vocal2, setlist.UnderLocation);
                }
                if (i == 1)
                {
                    MoveCard(scream, vocal3, setlist.UnderLocation);
                }
            }
        }
    }
}

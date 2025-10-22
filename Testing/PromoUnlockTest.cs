using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using System.IO;
using Handelabra;
using CauldronTests;

namespace CauldronTests.PromoUnlock
{
    [TestFixture()]
    public class PromoUnlockTest : CauldronBaseTest
    {
        protected Card inferno { get { return GetCardInPlay("InfernoTiamatCharacter"); } }
        protected Card storm { get { return GetCardInPlay("StormTiamatCharacter"); } }
        protected Card winter { get { return GetCardInPlay("WinterTiamatCharacter"); } }

        [Test()]
        public void TestHydraTiamatUnlock()
        {

            // Win a game against Tiamat with The Knight, Echelon, and Necro.
            // In that game, The Knight must deal the final damage to Inferno Head of Tiamat;
            // Echelon must deal the final damage to the Storm Head of Tiamat;
            // and Necro must deal the final damage to the Winter Head of Tiamat. 
            SetupGameController("Cauldron.Tiamat", "Cauldron.TheKnight", "Cauldron.Echelon", "Cauldron.Necro", "Megalopolis");
            StartGame();

            DestroyNonCharacterVillainCards();

            DealDamage(echelon.CharacterCard, storm, 1000, DamageType.Projectile, isIrreducible: true);
            DealDamage(necro.CharacterCard, winter, 1000, DamageType.Infernal, isIrreducible: true);

            SetHitPoints(inferno, 2);

            DecisionSelectTarget = inferno;
            PlayCard("HeavySwing");

        }

        [Test()]
        public void TestWardenOfChaosNecroUnlock()
        {

            // Play a game with Necro where Chaotic Summon is played off of another Chaotic Summon twice in one turn.
            SetupGameController("BaronBlade", "Cauldron.Necro", "Legacy", "Haka", "Megalopolis");
            StartGame();

            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(necro);
            Card chaoticSummon1 = GetCard("ChaoticSummon", 0);
            Card chaoticSummon2 = GetCard("ChaoticSummon", 1);
            Card chaoticSummon3 = GetCard("ChaoticSummon", 2);
            Card demonicImp = GetCard("DemonicImp");
            Card ghoul = GetCard("Ghoul");

            PutInHand(necro, chaoticSummon1);

            StackDeck(necro, new List<Card>()
            {
                chaoticSummon2,
                demonicImp,
                chaoticSummon3,
                ghoul
            });

            GoToPlayCardPhase(necro);

            PrintSeparator("PLAYING CHAOTIC SUMMON");
            PlayCard(chaoticSummon1);


        }

        [Test()]
        public void TestUncannyQuicksilver()
        {

            // Win a game with Quicksilver in Halberd Experimental Research Center.
            // Quicksilver must create a combo chain of at least 5 cards, ending with a finisher.
                // This is defined as at least 4 combo cards and 1 finisher.
            // Quicksilver must destroy Halcyon Cleaners in the game.
            // Quicksilver must not be incapicitated during the game.

            SetupGameController("AkashBhuta", "Cauldron.Quicksilver", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            DestroyNonCharacterVillainCards();
            AddCannotPlayCardsStatusEffect(legacy, heroesCannotPlay: false, villainsCannotPlay: true);

            MoveAllCardsFromHandToDeck(quicksilver);


            Card alloyStorm1 = GetCard("AlloyStorm", 0);
            Card alloyStorm2 = GetCard("AlloyStorm", 1);
            Card alloyStorm3 = GetCard("AlloyStorm", 2);
            Card coalescingSpear1 = GetCard("CoalescingSpear", 0);
            Card coalescingSpear2 = GetCard("CoalescingSpear", 1);
            Card coalescingSpear3 = GetCard("CoalescingSpear", 2);

            Card guardBreaker1 = GetCard("GuardBreaker", 0);
            Card guardBreaker2 = GetCard("GuardBreaker", 1);
            Card forestOfNeedles1 = GetCard("ForestOfNeedles", 0);
            Card halcyonCleaners = GetCard("HalcyonCleaners");


            PutInHand(new List<Card>()
            {
                alloyStorm1,
                alloyStorm2,
                alloyStorm3,
                coalescingSpear1,
                coalescingSpear2,
                coalescingSpear3,
                guardBreaker1,
                guardBreaker2,
                forestOfNeedles1
            });

            GoToPlayCardPhase(quicksilver);

            PrintSeparator("STARTING COMBO CHAIN");

            DecisionSelectFunctions = new int?[] { 1, 1, 1, 0, 1, 0, 1 };
            DecisionSelectCards = new List<Card>()
            {
                alloyStorm2,
                alloyStorm3,
                coalescingSpear1,
                akash.CharacterCard,
                guardBreaker1,
                akash.CharacterCard,
                halcyonCleaners,
                guardBreaker2,
                halcyonCleaners
            };

            PlayCard(alloyStorm1);

            PlayCard(halcyonCleaners);
            PlayCard(coalescingSpear2);

            DealDamage(quicksilver.CharacterCard, akash.CharacterCard, 300, DamageType.Melee, isIrreducible: true);
            

        }
    }
}
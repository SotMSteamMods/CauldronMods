using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace CauldronTests
{
    public class CauldronBaseTest : BaseTest
    {
        //Heroes
        protected HeroTurnTakerController baccarat { get { return FindHero("Baccarat"); } }
        protected HeroTurnTakerController cricket { get { return FindHero("Cricket"); } }
        protected HeroTurnTakerController cypher { get { return FindHero("Cypher"); } }
        protected HeroTurnTakerController doc { get { return FindHero("DocHavoc"); } }
        protected HeroTurnTakerController drift { get { return FindHero("Drift"); } }
        protected Card futureDrift { get { return GetCard("FutureDriftCharacter"); } }
        protected Card pastDrift { get { return GetCard("PastDriftCharacter"); } }
        protected HeroTurnTakerController echelon { get { return FindHero("Echelon"); } }
        protected HeroTurnTakerController gargoyle { get { return FindHero("Gargoyle"); } }
        protected HeroTurnTakerController gyrosaur { get { return FindHero("Gyrosaur"); } }
        protected HeroTurnTakerController impact { get { return FindHero("Impact"); } }
        protected HeroTurnTakerController ladyWood { get { return FindHero("LadyOfTheWood"); } }
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        protected HeroTurnTakerController malichae { get { return FindHero("Malichae"); } }
        protected HeroTurnTakerController necro { get { return FindHero("Necro"); } }
        protected HeroTurnTakerController pyre { get { return FindHero("Pyre"); } }
        protected HeroTurnTakerController quicksilver { get { return FindHero("Quicksilver"); } }
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        protected Card terra { get { return GetCard("StarlightOfTerraCharacter"); } }
        protected Card asheron { get { return GetCard("StarlightOfAsheronCharacter"); } }
        protected Card cryos { get { return GetCard("StarlightOfCryosFourCharacter"); } }
        protected HeroTurnTakerController tango { get { return FindHero("TangoOne"); } }
        protected HeroTurnTakerController terminus { get { return FindHero("Terminus"); } }
        protected HeroTurnTakerController knight { get { return FindHero("TheKnight"); } }

        protected Card youngKnight { get { return GetCard("TheYoungKnightCharacter"); } }
        protected Card oldKnight { get { return GetCard("TheOldKnightCharacter"); } }

        protected HeroTurnTakerController stranger { get { return FindHero("TheStranger"); } }
        protected HeroTurnTakerController titan { get { return FindHero("Titan"); } }
        protected HeroTurnTakerController vanish { get { return FindHero("Vanish"); } }

        //Villains
        protected TurnTakerController anathema { get { return FindVillain("Anathema"); } }
        protected TurnTakerController celadroch { get { return FindVillain("Celadroch"); } }
        protected TurnTakerController dendron { get { return FindVillain("Dendron"); } }
        protected TurnTakerController dynamo { get { return FindVillain("Dynamo"); } }
        protected TurnTakerController gray { get { return FindVillain("Gray"); } }
        protected TurnTakerController menagerie { get { return FindVillain("Menagerie"); } }
        protected TurnTakerController mythos { get { return FindVillain("Mythos"); } }
        protected TurnTakerController oriphel { get { return FindVillain("Oriphel"); } }
        protected Cauldron.Outlander.OutlanderTurnTakerController outlander { get { return (Cauldron.Outlander.OutlanderTurnTakerController)FindVillain("Outlander"); } }
        protected TurnTakerController phase { get { return FindVillain("PhaseVillain"); } }
        protected TurnTakerController scream { get { return FindVillain("ScreaMachine"); } }
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }
        protected Cauldron.TheInfernalChoir.TheInfernalChoirTurnTakerController choir { get { return FindVillain("TheInfernalChoir") as Cauldron.TheInfernalChoir.TheInfernalChoirTurnTakerController; ; } }
        protected TurnTakerController fate { get { return FindVillain("TheMistressOfFate"); } }
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }
        protected TurnTakerController vector { get { return FindVillain("Vector"); } }

        public static string[] CauldronHeroes = { "Cauldron.Baccarat", "Cauldron.Cricket", "Cauldron.Cypher", "Cauldron.DocHavoc", "Cauldron.Drift", "Cauldron.Echelon", "Cauldron.Gargoyle", "Cauldron.Gyrosaur", "Cauldron.Impact", "Cauldron.LadyOfTheWood", "Cauldron.MagnificentMara", "Cauldron.Malichae", "Cauldron.Necro", "Cauldron.Pyre", "Cauldron.Quicksilver", "Cauldron.Starlight", "Cauldron.TangoOne", "Cauldron.Terminus", "Cauldron.TheKnight", "Cauldron.TheStranger", "Cauldron.Titan", "Cauldron.Vanish" };
        public static string[] CauldronVillains = { "Cauldron.Anathema", "Cauldron.Celadroch", "Cauldron.Dendron", "Cauldron.Dynamo", "Cauldron.Gray", "Cauldron.Menagerie", "Cauldron.Mythos", "Cauldron.Oriphel", "Cauldron.Outlander", "Cauldron.PhaseVillain", "Cauldron.ScreaMachine", "Cauldron.SwarmEater", "Cauldron.TheInfernalChoir", "Cauldron.TheMistressOfFate", "Cauldron.Tiamat", "Cauldron.Vector" };
        public static string[] CauldronEnvironments = { "Cauldron.BlackwoodForest", "Cauldron.CatchwaterHarbor", "Cauldron.DungeonsOfTerror", "Cauldron.FSCContinuanceWanderer", "Cauldron.HalberdExperimentalResearchCenter", "Cauldron.NightloreCitadel", "Cauldron.Northspar", "Cauldron.OblaskCrater", "Cauldron.StSimeonsCatacombs", "Cauldron.SuperstormAkela", "Cauldron.TheChasmOfAThousandNights", "Cauldron.TheCybersphere", "Cauldron.TheWanderingIsle", "Cauldron.VaultFive", "Cauldron.WindmillCity" };

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune, CardSource cardSource = null )
        {
            if(cardSource is null)
            {
                cardSource = new CardSource(ttc.CharacterCardController);
            }
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, cardSource));
        }

        protected void AddCannotDealDamageTrigger(TurnTakerController ttc, Card specificCard, bool untilEnd = false)
        {
            CannotDealDamageStatusEffect cannotDealDamageEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageEffect.SourceCriteria.IsSpecificCard = specificCard;
            if(!untilEnd)
            {
                cannotDealDamageEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            }
            else
            {
                cannotDealDamageEffect.UntilEndOfNextTurn(ttc.TurnTaker);
            }
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotPlayCardsStatusEffect(TurnTakerController source, bool heroesCannotPlay, bool villainsCannotPlay, bool envCardsCannotPlay = false, bool untilEnd = false)
        {
            CannotPlayCardsStatusEffect effect = new CannotPlayCardsStatusEffect();
            if (heroesCannotPlay)
                effect.CardCriteria.IsHero = true;
            if (villainsCannotPlay)
                effect.CardCriteria.IsVillain = true;
            if (envCardsCannotPlay)
                effect.CardCriteria.IsEnvironment = true;

            if(!untilEnd)
            {
                effect.UntilStartOfNextTurn(source.TurnTaker);

            } else
            {
                effect.UntilEndOfNextTurn(source.TurnTaker);
            }
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(source.CharacterCardController)));
        }

        protected void AddImmuneToNextDamageEffect(TurnTakerController ttc, bool villains, bool heroes)
        {
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.TargetCriteria.IsVillain = villains;
            effect.TargetCriteria.IsHero = heroes;
            effect.NumberOfUses = 1;
            RunCoroutine(GameController.AddStatusEffect(effect, true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void AddReduceDamageTrigger(TurnTakerController ttc, bool heroesReduce, bool villainsReduce, int amount)
        {
            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(amount);
            effect.TargetCriteria.IsHero = new bool?(heroesReduce);
            effect.TargetCriteria.IsVillain = new bool?(villainsReduce);
            effect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddReduceNextDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            reduceDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(reduceDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddIncreaseNextDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(amount);
            increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            increaseDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(increaseDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddIncreaseNextHealingTrigger(HeroTurnTakerController httc,  int amount)
        {
            IncreaseGainHPStatusEffect increaseGainHPStatusEffect = new IncreaseGainHPStatusEffect(amount);
            increaseGainHPStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(increaseGainHPStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddDamageCannotBeIncreasedTrigger(Func<DealDamageAction, bool> criteria, CardSource cardSource)
        {
            Trigger<DealDamageAction> unincreasableDamageTrigger = new Trigger<DealDamageAction>( GameController, criteria, (DealDamageAction dd) => base.GameController.MakeDamageUnincreasable(dd, cardSource), TriggerType.MakeDamageUnincreasable.ToEnumerable(), TriggerTiming.Before, cardSource);
            this.GameController.AddTrigger(unincreasableDamageTrigger); 
        }

        protected void AddDamageCannotBeRedirectedTrigger(Func<DealDamageAction, bool> criteria, CardSource cardSource)
        {
            Trigger<DealDamageAction> unredirectableTrigger = new Trigger<DealDamageAction>(GameController, criteria, (DealDamageAction dd) => base.GameController.MakeDamageNotRedirectable(dd, cardSource), TriggerType.MakeDamageNotRedirectable.ToEnumerable(), TriggerTiming.Before, cardSource);
            this.GameController.AddTrigger(unredirectableTrigger);
        }

        protected void PreventEndOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
        {
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect();
            preventPhaseEffectStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
            RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void PreventStartOfTurnEffects(TurnTakerController ttc, Card cardToPrevent)
        {
            PreventPhaseEffectStatusEffect preventPhaseEffectStatusEffect = new PreventPhaseEffectStatusEffect(Phase.Start);
            preventPhaseEffectStatusEffect.UntilEndOfNextTurn(ttc.TurnTaker);
            preventPhaseEffectStatusEffect.CardCriteria.IsSpecificCard = cardToPrevent;
            RunCoroutine(base.GameController.AddStatusEffect(preventPhaseEffectStatusEffect, showMessage: true, ttc.CharacterCardController.GetCardSource()));
        }

        protected void AssertCardConfiguration(string identifier, string[] keywords = null, int hitpoints = 0)
        {
            Card card = GetCard(identifier);
            if (keywords != null)
            {
                foreach (string keyword in keywords)
                {
                    AssertCardHasKeyword(card, keyword, false);
                }
            }
            if (hitpoints > 0)
            {
                AssertMaximumHitPoints(card, hitpoints);
            }
        }

        protected void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
        }

        protected void AssertHasKeywordEvenIfUnderOrFaceDown(Card card, string keyword)
        {
            Assert.IsTrue(this.GameController.DoesCardContainKeyword(card, keyword, true, true), "{0} should have keyword: {1}", card.Identifier, keyword);
        }

        protected void AssertHasAbility(string abilityKey, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                int number = card.GetNumberOfActivatableAbilities(abilityKey);
                Assert.GreaterOrEqual(number, 1);
            }
        }


        protected void AssertDamageTypeChanged(HeroTurnTakerController httc, Card source, Card target, int amount, DamageType initialDamageType, DamageType expectedDamageType)
        {
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            this.RunCoroutine(this.GameController.DealDamage(httc, source, (Card c) => c == target, amount, initialDamageType, false, false, storedResults, null, null, false, null, null, false, false, new CardSource(GetCardController(source))));

            if (storedResults != null)
            {
                DealDamageAction dd = storedResults.FirstOrDefault<DealDamageAction>();
                DamageType actualDamageType = dd.DamageType;
                Assert.AreEqual(expectedDamageType, actualDamageType, $"Expected damage type: {expectedDamageType}. Actual damage type: {actualDamageType}");
            }
            else
            {
                Assert.Fail("storedResults was null");
            }

        }

        protected void QuickHPCheckZeroThroughFormChange()
        {
            var change = new int?[_quickHPStorage.Count];
            for (int i = 0; i < _quickHPStorage.Count; i++)
            {
                change[i] = 0;
            }

            QuickHPCheckThroughFormChange(change);
        }
        protected void QuickHPCheckThroughFormChange(params int?[] hpChange)
        {
            Assert.AreEqual(_quickHPStorage.Count, hpChange.Length, "QuickHPCheck passed {0} values but {1} are stored.", hpChange.Length, _quickHPStorage.Count);

            for (int i = 0; i < hpChange.Count(); i++)
            {
                var id = _quickHPStorage.Keys.ElementAt(i).SharedIdentifier;

                var card = id is null ? _quickHPStorage.Keys.ElementAt(i) : FindCardsWhere(c => c.IsInPlayAndHasGameText && c.SharedIdentifier == id).First();
                var previousHP = _quickHPStorage.Values.ElementAt(i);
                var changeHP = hpChange.ElementAt(i);
                if (changeHP.HasValue)
                {
                    var expected = previousHP + changeHP.Value;
                    if (card.HitPoints.HasValue)
                    {
                        var actual = card.HitPoints.Value;
                        Assert.AreEqual(expected, actual, "Expected " + card.Title + "'s HP to be " + expected + ", but it was " + actual + ".");
                        _quickHPStorage[_quickHPStorage.Keys.ElementAt(i)] = actual;
                    }
                    else
                    {
                        Assert.Fail("QuickHPCheck: " + card.Title + " isn't a target!");
                    }
                }
                else if (card.IsTarget)
                {
                    AssertNotInPlay(card);
                }
            }
        }

        protected int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        protected TokenPool GetShiftPool()
        {
            return this.GetShiftTrack().FindTokenPool("ShiftPool");
        }

        protected Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == "ShiftTrack" && c.IsInPlayAndHasGameText, false).FirstOrDefault();
        }

        protected void AssertTrackPosition(int expectedPosition)
        {
            Assert.AreEqual(expectedPosition, CurrentShiftPosition(), "Expected position: " + expectedPosition + ", was: " + CurrentShiftPosition());
        }

        protected void AssertNotFlipped(params Card[] cards)
        {
            cards.ForEach(c => base.AssertNotFlipped(c));
        }

        protected void GoToShiftPosition(int position)
        {
            if (position > CurrentShiftPosition())
            {
                DecisionSelectFunction = 2;
                for (int i = CurrentShiftPosition(); i < position; i++)
                {
                    AddTokensToPool(GetShiftPool(), 1);
                }
            }
            else
            {
                DecisionSelectFunction = 1;
                for (int i = CurrentShiftPosition(); i > position; i--)
                {
                    RemoveTokensFromPool(GetShiftPool(), 1);
                }
            }
        }

        public Card GetPositionalBreachShiftTrack(int position)
        {

            return GetCard("ThroughTheBreachShiftTrack" + position);
        }

        protected GameController ReplayGameFromPath(string path)
        {
            try
            {
                var savedGame = LoadGamePath(path);

                if (savedGame != null)
                {
                    var newGame = MakeReplayableGame(savedGame);
                    SetupGameController(newGame);

                    Console.WriteLine("Successfully created game to replay...");

                    StartGame();
                    this.ReplayingGame = true;

                    // Keep moving the game forward until we have reached the stopping point.
                    int sanity = 1000;
                    while (this.ReplayingGame)
                    {
                        RunActiveTurnPhase();
                        EnterNextTurnPhase();
                        sanity--;

                        if (sanity == 0)
                        {
                            Log.Error("Save game never seemed to end: " + path);
                            this.ReplayingGame = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to load and replay game.");
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to load and replay game. Reason: " + e.Message);
                throw;
            }

            return this.GameController;
        }

        //OblivAeon

        protected void AssertCardsInBattleZone(BattleZone bz, params Card[] cards)
        {
            cards.ForEach(c => AssertBattleZone(c, bz));
        }

        protected void MoveToSpecificBattleZone(BattleZone bz, TurnTakerController ttc)
        {
            if (ttc.BattleZone == bz)
                return;

            SwitchBattleZone(ttc);
        }
    }
}

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
        protected HeroTurnTakerController tango { get { return FindHero("TangoOne"); } }
        protected HeroTurnTakerController terminus { get { return FindHero("Terminus"); } }
        protected HeroTurnTakerController knight { get { return FindHero("TheKnight"); } }
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
        protected TurnTakerController outlander { get { return FindVillain("Outlander"); } }
        protected TurnTakerController phase { get { return FindVillain("PhaseVillain"); } }
        protected TurnTakerController scream { get { return FindVillain("ScreaMachine"); } }
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }
        protected TurnTakerController choir { get { return FindVillain("TheInfernalChoir"); } }
        protected TurnTakerController fate { get { return FindVillain("TheMistressOfFate"); } }
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }
        protected TurnTakerController vector { get { return FindVillain("Vector"); } }

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealDamageTrigger(TurnTakerController ttc, Card specificCard)
        {
            CannotDealDamageStatusEffect cannotDealDamageEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageEffect.SourceCriteria.IsSpecificCard = specificCard;
            cannotDealDamageEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        protected void AddCannotPlayCardsStatusEffect(TurnTakerController ttc, bool heroesCannotPlay, bool villainsCannotPlay)
        {
            CannotPlayCardsStatusEffect effect = new CannotPlayCardsStatusEffect();
            effect.CardCriteria.IsHero = new bool?(heroesCannotPlay);
            effect.CardCriteria.IsVillain = new bool?(villainsCannotPlay);
            effect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(ttc.CharacterCardController)));
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

        protected void AddReduceDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            reduceDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(reduceDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        protected void AddIncreaseDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(amount);
            increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            increaseDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(increaseDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
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

        protected void GoToShiftPosition(int position)
        {
            if (position > CurrentShiftPosition())
            {
                DecisionSelectFunction = 2;
                for (int i = CurrentShiftPosition(); i < position; i++)
                {
                    UsePower(drift);
                }
            }
            else
            {
                DecisionSelectFunction = 1;
                for (int i = CurrentShiftPosition(); i > position; i--)
                {
                    UsePower(drift);
                }
            }
        }

        public Card GetPositionalBreachShiftTrack(int position)
        {

            return GetCard("ThroughTheBreachShiftTrack" + position);
        }

    }
}

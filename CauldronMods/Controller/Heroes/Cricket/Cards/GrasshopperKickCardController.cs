using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cricket
{
    public class GrasshopperKickCardController : CardController
    {
        public GrasshopperKickCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => StatusEffectMessage, showInEffectsList: () => true).Condition = () => Game.HasGameStarted && Card.IsInPlayAndHasGameText && IsImmuneToEnvironmentDamage;
        }

        public readonly string IsImmuneToEnvironmentDamageKey = "IsImmuneToEnvironmentDamage";
        private string StatusEffectMessage => $"{CharacterCard.Title} is immune to damage dealt by environment targets.";

        public bool IsImmuneToEnvironmentDamage 
        {
            get
            {
                return GetCardPropertyJournalEntryBoolean(IsImmuneToEnvironmentDamageKey).HasValue && GetCardPropertyJournalEntryBoolean(IsImmuneToEnvironmentDamageKey).Value == true;
            }
        }

        public override void AddTriggers()
        {
            AddImmuneToDamageTrigger(dd => dd.Target == CharacterCard && dd.DamageSource.IsEnvironmentTarget && IsImmuneToEnvironmentDamage) ;
            AddTrigger((PhaseChangeAction pca) => pca.ToPhase.Phase == Phase.Start && pca.ToPhase.TurnTaker == TurnTaker, ResetImmunityProperty, TriggerType.Hidden, TriggerTiming.After);
            ResetFlagAfterLeavesPlay(IsImmuneToEnvironmentDamageKey);
        }

        private IEnumerator ResetImmunityProperty(PhaseChangeAction pca)
        {
            SetCardProperty(IsImmuneToEnvironmentDamageKey, false);
            IEnumerator coroutine = GameController.SendMessageAction($"Expiring: {StatusEffectMessage}", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetNumeral = GetPowerNumeral(0, 1);
            int damageNumeral = GetPowerNumeral(1, 2);
            //{Cricket} deals 1 target 2 melee damage. 
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Melee, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            SetCardPropertyToTrueIfRealAction(IsImmuneToEnvironmentDamageKey);
            coroutine = GameController.SendMessageAction(StatusEffectMessage, Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
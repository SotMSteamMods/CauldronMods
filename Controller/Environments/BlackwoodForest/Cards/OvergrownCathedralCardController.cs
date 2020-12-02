using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class OvergrownCathedralCardController : CardController
    {
        //==============================================================
        // The first time damage is dealt each turn,
        // this card deals each target that has not been dealt
        // damage 1 psychic damage.
        // At the start of the environment turn, you may destroy this card.
        //==============================================================

        public static readonly string Identifier = "OvergrownCathedral";

        private const string FirstTimeDamageDealtPropName = "FirstTimeDamageDealt";
        private const int DamageToDeal = 1;

        public OvergrownCathedralCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn("FirstTimeDamageIsDealt", 
                "{0} has already taken effect this turn.", "{0} has not yet taken effect this turn.", null);
        }

        public override void AddTriggers()
        {
            // Give option to destroy this at start of env. turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestructionChoiceResponse, TriggerType.DestroySelf);

            // Deal damage reaction
            base.AddTrigger<DealDamageAction>(p => !base.IsPropertyTrue(FirstTimeDamageDealtPropName), 
                this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After, 
                ActionDescription.Unspecified, false, true, null, 
                false, null, null);

            base.AddTriggers();
        }

        private IEnumerator DestructionChoiceResponse(PhaseChangeAction pca)
        {
            List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();

            // Ask if player wants to destroy this card
            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.DestroyCard, this.Card, null, storedYesNoResults, null, GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Return if they chose not to discard from their deck
            if (!base.DidPlayerAnswerYes(storedYesNoResults))
            {
                yield break;
            }

            IEnumerator destroyRoutine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealtPropName);

            IEnumerator dealDamageRoutine = base.DealDamage(this.Card, card => !dda.AllTargets.Contains(card), 
                DamageToDeal, DamageType.Psychic);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}
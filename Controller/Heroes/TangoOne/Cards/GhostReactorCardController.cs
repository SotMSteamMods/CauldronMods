using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class GhostReactorCardController : TangoOneBaseCardController
    {
        //==============================================================
        // {TangoOne} is immune to psychic damage.
        // Power: Draw a card. Increase the next damage dealt by {TangoOne} by 2.
        //==============================================================

        public static readonly string Identifier = "GhostReactor";

        private const int DamageIncrease = 2;

        public GhostReactorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.DamageType == DamageType.Psychic && action.Target == base.CharacterCard);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // Draw a card
            IEnumerator drawCardRoutine = base.DrawCard(this.HeroTurnTaker);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
            }

            // Increase the next damage dealt by {TangoOne} by 2
            int powerNumeral = GetPowerNumeral(0, DamageIncrease);
            var effect = new IncreaseDamageStatusEffect(powerNumeral);
            effect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            effect.CardSource = Card;
            effect.Identifier = Card.Identifier;
            effect.NumberOfUses = 1;

            IEnumerator increaseDamageRoutine = base.AddStatusEffect(effect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(increaseDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(increaseDamageRoutine);
            }
        }
    }
}
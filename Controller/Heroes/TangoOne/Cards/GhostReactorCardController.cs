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

        public static string Identifier = "GhostReactor";

        private const int DamageIncrease = 2;

        public GhostReactorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.DamageTypeCriteria.AddType(DamageType.Psychic);
            immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
            immuneToDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            immuneToDamageStatusEffect.CardDestroyedExpiryCriteria.Card = base.CharacterCard;

            IEnumerator immuneToDamageRoutine = base.AddStatusEffect(immuneToDamageStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(immuneToDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(immuneToDamageRoutine);
            }
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

            IEnumerator increaseDamageRoutine = base.AddStatusEffect(new IncreaseDamageStatusEffect(powerNumeral)
            {
                SourceCriteria =
                {
                    IsSpecificCard = base.Card.Owner.CharacterCard
                },
                NumberOfUses = 1,
                CardDestroyedExpiryCriteria =
                {
                    Card = base.Card
                }
            }, true);

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
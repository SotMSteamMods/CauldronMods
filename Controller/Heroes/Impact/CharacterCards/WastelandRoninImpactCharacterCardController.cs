using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Impact
{
    public class WastelandRoninImpactCharacterCardController : HeroCharacterCardController
    {
        public WastelandRoninImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} regains 1 HP, or the next time one of your ongoing cards is destroyed, play it."
            int numHPGain = GetPowerNumeral(0, 1);
            var functions = new Function[]
            {
                new Function(DecisionMaker, $"Regain {numHPGain} HP", SelectionType.GainHP, () => GameController.GainHP(this.CharacterCard, numHPGain, cardSource: GetCardSource())),
                new Function(DecisionMaker, "The next time one of your ongoing cards is destroyed, play it.", SelectionType.PlayCard, AddOngoingDestructionTrigger)
            };
            var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, false, cardSource: GetCardSource()); ;
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator AddOngoingDestructionTrigger()
        {
            var statusEffect = new WhenCardIsDestroyedStatusEffect(this.Card, "PlayDestroyedOngoing", $"The next time one of {this.TurnTaker.Name}'s ongoings is destroyed, they play it again.", new TriggerType[] { TriggerType.PlayCard }, DecisionMaker.HeroTurnTaker, this.Card);
            statusEffect.NumberOfUses = 1;
            statusEffect.CardDestroyedCriteria.OwnedBy = this.TurnTaker;
            statusEffect.CardDestroyedCriteria.HasAnyOfTheseKeywords = new List<string> { "ongoing" };
            statusEffect.PostDestroyDestinationMustBeChangeable = true;

            IEnumerator coroutine = AddStatusEffect(statusEffect);
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

        public IEnumerator PlayDestroyedOngoing(DestroyCardAction dc, HeroTurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            dc.AddAfterDestroyedAction(() => GameController.PlayCard(DecisionMaker, dc.CardToDestroy.Card, cardSource: GetCardSource()), this);
            dc.PostDestroyDestinationCanBeChanged = false;
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One hero may use a power now.",
                        yield break;
                    }
                case 1:
                    {
                        //"Environment cards cannot be played during the next environment turn.",
                        break;
                    }
                case 2:
                    {
                        //"Select a target. The next damage it deals is irreducible."
                        break;
                    }
            }
            yield break;
        }
    }
}
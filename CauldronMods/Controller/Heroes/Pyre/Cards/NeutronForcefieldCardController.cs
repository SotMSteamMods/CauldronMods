using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class NeutronForcefieldCardController : PyreUtilityCardController
    {
        private bool WasPlayedIrradiated = false;
        public NeutronForcefieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((PlayCardAction pc) => pc.CardToPlay == Card, MarkIrradiatedPlay, TriggerType.Hidden, TriggerTiming.Before, outOfPlayTrigger: true);
        }
        public override IEnumerator Play()
        {
            //"If this card is {PyreIrradiate} when you play it, it becomes indestructible until the end of your turn.",
            if(WasPlayedIrradiated)
            {
                WasPlayedIrradiated = false;
                var effect = new MakeIndestructibleStatusEffect();
                effect.CardsToMakeIndestructible.IsSpecificCard = Card;
                effect.ToTurnPhaseExpiryCriteria.Phase = Phase.End;
                effect.ToTurnPhaseExpiryCriteria.TurnTaker = TurnTaker;
                effect.CardSource = Card;

                IEnumerator coroutine = AddStatusEffect(effect, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Select a hero target. Until the start of your next turn, that target is immune to damage. Destroy this card."
            var decision = new SelectCardDecision(GameController, DecisionMaker, SelectionType.PreventDamage, GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource())), cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(decision, AddDamageImmunityEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DestroyThisCardResponse(null);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
        private IEnumerator AddDamageImmunityEffect(SelectCardDecision scd)
        {
            Card target = scd.SelectedCard;
            var immuneEffect = new ImmuneToDamageStatusEffect();
            immuneEffect.TargetCriteria.IsSpecificCard = target;
            immuneEffect.UntilStartOfNextTurn(TurnTaker);
            immuneEffect.CreateImplicitExpiryConditions();
            immuneEffect.CardSource = Card;

            IEnumerator coroutine = AddStatusEffect(immuneEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
        private IEnumerator MarkIrradiatedPlay(PlayCardAction pc)
        {
            WasPlayedIrradiated = IsIrradiated(Card);
            yield return null;
            yield break;
        }
    }
}

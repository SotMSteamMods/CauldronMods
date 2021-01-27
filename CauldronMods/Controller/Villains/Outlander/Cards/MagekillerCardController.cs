using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class MagekillerCardController : OutlanderUtilityCardController
    {
        public MagekillerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(OncePerTurn), () => "A hero one shot has entered play yet this turn.", () => "A hero one shot has not entered play yet this turn.");
        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time a hero one-shot enters play each turn, {Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.CardEnteringPlay.IsHero && action.CardEnteringPlay.IsOneShot, this.OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals the hero target with the highest HP 3 melee damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(CardEntersPlayAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            var a = FindCardsWhere((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText);
            //...{Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 1, DamageType.Lightning, true);
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

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the hero target with the highest HP 3 melee damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 3, DamageType.Melee);
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
    }
}

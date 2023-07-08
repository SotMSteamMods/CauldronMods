using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class MagekillerCardController : OutlanderTraceCardController
    {
        public MagekillerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(OncePerTurn), () => "A hero one shot has entered play this turn.", () => "A hero one shot has not yet entered play this turn.");
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        protected const string OncePerTurn = "OutlanderMagekillerOncePerTurn";

        public override void AddTriggers()
        {
            //The first time a hero one-shot enters play each turn, {Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.
            AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !HasBeenSetToTrueThisTurn(OncePerTurn) && IsHero(action.CardEnteringPlay) && action.CardEnteringPlay.IsOneShot, OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals the hero target with the highest HP 3 melee damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(CardEntersPlayAction action)
        {
            SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...{Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => 1, DamageType.Lightning, isIrreducible: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the hero target with the highest HP 3 melee damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => 3, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}

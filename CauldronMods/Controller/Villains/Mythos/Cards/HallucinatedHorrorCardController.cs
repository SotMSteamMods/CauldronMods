using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class HallucinatedHorrorCardController : MythosUtilityCardController
    {
        public HallucinatedHorrorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //{MythosMadness}{MythosDanger} When this card enters play, play the top card of the villain deck.
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier) || base.IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                IEnumerator coroutine = PlayTheTopCardOfTheVillainDeckWithMessageResponse(null);
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

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals each hero target 2 sonic damage.
            base.AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHero(c), TargetType.All, 2, DamageType.Sonic);
            //Destroy this card when a hero is dealt damage by another hero target.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DidDealDamage && IsHero(action.Target) && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.IsHero && action.Target != action.DamageSource.Card, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }
    }
}

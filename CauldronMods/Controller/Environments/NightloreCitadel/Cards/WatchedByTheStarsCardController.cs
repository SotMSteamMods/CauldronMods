using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class WatchedByTheStarsCardController : NightloreCitadelUtilityCardController
    {
        public WatchedByTheStarsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("StarlightOfOros");
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals each villain target 2 radiant damage. 
            //If Starlight of Oros is in play, this card deals each hero target 2 cold damage instead.
            IEnumerator coroutine;
            if (!IsStarlightOfOrosInPlay())
            {
                coroutine = DealDamage(Card, (Card c) => IsVillainTarget(c), 2, DamageType.Radiant);
            } else
            {
                coroutine = DealDamage(Card, (Card c) => IsHeroTarget(c), 2, DamageType.Cold);
            }

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

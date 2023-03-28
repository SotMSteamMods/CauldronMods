using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class TheRatsInTheWallsCardController : VaultFiveUtilityCardController
    {
        public TheRatsInTheWallsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildPlayersWithArtifactInHandSpecialString());
        }

        public override void AddTriggers()
        {
            //Whenever a player draws an Artifact card, they may draw a card.
            AddTrigger<DrawCardAction>((DrawCardAction dca) => dca.DidDrawCard && dca.DrawnCard != null && IsArtifact(dca.DrawnCard), DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);
            //Increase damage dealt to heroes with Artifact cards in their hands by 1
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.Target != null && IsHeroCharacterCard(dd.Target) && GetPlayersWithArtifactsInHand().Contains(dd.Target.Owner), 1);
        }

        private IEnumerator DrawCardResponse(DrawCardAction dca)
        {
            IEnumerator coroutine = DrawCard(hero: dca.HeroTurnTaker, optional: true);
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

        public override IEnumerator Play()
        {
            //When this card enters play, it deals each target 1 psychic damage.",
            IEnumerator coroutine = DealDamage(Card, (Card c) => c.IsTarget, 1, DamageType.Psychic);
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

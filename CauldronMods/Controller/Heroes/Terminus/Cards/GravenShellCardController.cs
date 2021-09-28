using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class GravenShellCardController : TerminusMementoCardController
    {
        /* 
         * This card is indestructible. If another Memento would enter play, instead remove it from the game and increase damage 
         * dealt by {Terminus} by 1.
         * Whenever {Terminus} destroys a card, add 2 tokens to your Wrath pool and she regains 1HP.
         */
        public GravenShellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.OutOfGame, new LinqCardCriteria((Card c) => GameController.DoesCardContainKeyword(c, "memento"), "memento"));
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(TriggerCriteria, DestroyCardActionResponse, new TriggerType[] { TriggerType.AddTokensToPool, TriggerType.GainHP }, TriggerTiming.After);
            base.AddTriggers();
        }

        private bool TriggerCriteria(DestroyCardAction dca)
        {
            return dca.WasCardDestroyed && dca.GetCardDestroyer() != null && dca.WasDestroyedBy((card) => card.Owner.CharacterCard == base.CharacterCard);
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            IEnumerator coroutine;

            coroutine = base.AddWrathTokens(2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.GainHP(base.CharacterCard, 1, cardSource: base.GetCardSource());
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

        protected override IEnumerator OnOtherMementoRemoved()
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect;

            // increase damage  dealt by { Terminus} by 1.
            increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            increaseDamageStatusEffect.UntilTargetLeavesPlay(base.CharacterCard);

            return base.AddStatusEffect(increaseDamageStatusEffect);
        }
    }
}

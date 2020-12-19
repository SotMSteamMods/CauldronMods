using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class ExposedLifeforceCardController : TheWanderingIsleCardController
    {
        public ExposedLifeforceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by villain cards by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsCard && IsVillain(dd.DamageSource.Card), 1);
            //Destroy this card if Teryx regains 10HP in a single round.
            base.AddTrigger<GainHPAction>((GainHPAction gh) => gh.HpGainer.Identifier == TeryxIdentifier && this.DidTeryxGain10OrMoreHpThisRound(), this.DestroyCardResponse, new TriggerType[] { TriggerType.DestroySelf }, TriggerTiming.After);
        }

        private IEnumerator DestroyCardResponse(GainHPAction action)
        {
            IEnumerator coroutine = base.GameController.SendMessageAction("Teryx has gained 10 or more HP in a single round.", Priority.Medium, base.GetCardSource());
            Log.Debug("Teryx has gained 10 or more HP in a single round");
            IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(destroy);
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            return PlayTeryxFromDeckOrTrashThenShuffle();
        }

        private bool DidTeryxGain10OrMoreHpThisRound()
        {
            return base.GameController.Game.Journal.GainHPEntries()
                       .Where(e => e.Round == this.Game.Round && IsTeryx(e.TargetCard))
                       .Sum(e => e.Amount) >= 10;
        }
    }
}

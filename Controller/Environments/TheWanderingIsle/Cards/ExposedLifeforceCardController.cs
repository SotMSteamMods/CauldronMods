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
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.Card != null && dd.DamageSource.Card.IsVillain, (DealDamageAction dd) => 1, false);
            //Destroy this card if Teryx regains 10HP in a single round.
            base.AddTrigger<GainHPAction>((GainHPAction gh) => gh.HpGainer != null && gh.HpGainer.Identifier == "Teryx" && this.DidTeryxGain10OrMoreHpThisRound(), new Func<GainHPAction, IEnumerator>(this.DestroyCardResponse), new TriggerType[] { TriggerType.DestroySelf }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
        }

        private IEnumerator DestroyCardResponse(GainHPAction action)
        {
            IEnumerator coroutine = base.GameController.SendMessageAction("Teryx has gained 10 or more HP in a single round.", Priority.Medium, base.GetCardSource(null), null, false);
            Log.Debug("Teryx has gained 10 or more HP in a single round");
            IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
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

            //When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocations(new Location[]
            {
                base.TurnTaker.Deck,
                base.TurnTaker.Trash
            }, "Teryx", true, null, false, true, true);
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

        private IEnumerable<GainHPJournalEntry> GainHPEntriesThisRound()
        {
            return from e in base.GameController.Game.Journal.GainHPEntries()
                   where e.Round == this.Game.Round
                   select e;
        }


        private bool DidTeryxGain10OrMoreHpThisRound()
        {
            int amountTeryxGainedThisRound = (from e in this.GainHPEntriesThisRound()
                                              where e.TargetCard != null && this.IsTeryx(e.TargetCard)
                                              select e.Amount).Sum();

            return amountTeryxGainedThisRound >= 10;
        }
    }
}

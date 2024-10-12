using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class BootlegMesmerPendantCardController : CardController
    {
        private static string mesmerKey = "BootlegMesmerPendantDealtDamage";
        public BootlegMesmerPendantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This is not the effect of the actual Mesmer Pendant, merely an approximation that can be easily made in the engine.");
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a non-character target.",
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter), storedResults, isPutIntoPlay, decisionSources);
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
        public override void AddTriggers()
        {
            //"When that card would cause damage to a hero target, redirect it to a villain target that hasn't been damaged this way this turn.",
            AddTrigger((DealDamageAction dda) => dda.CardSource != null && dda.CardSource.Card == GetCardThisCardIsNextTo() && IsHeroTarget(dda.Target), RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            AddTrigger((DealDamageAction dda) => dda.DidDealDamage && dda.NumberOfTimesRedirected > 0 && dda.DamageModifiers.Any((ModifyDealDamageAction mdd) => mdd.CardSource != null && mdd.CardSource.Card == this.Card), MarkDamagedByRedirect, TriggerType.Hidden, TriggerTiming.After);

            //"When it would destroy a hero ongoing, prevent it and destroy a villain ongoing."
            AddTrigger((DestroyCardAction dc) => dc.CardSource != null && dc.CardSource.Card == GetCardThisCardIsNextTo() && IsHero(dc.CardToDestroy.Card) && IsOngoing(dc.CardToDestroy.Card),
                           DestroyVillainOngoingInsteadResponse,
                           new TriggerType[] { TriggerType.CancelAction, TriggerType.DestroyCard },
                           TriggerTiming.Before);

            //"At the start of your turn destroy this card."
            AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: false);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagsAfterLeavesPlay(mesmerKey), TriggerType.Hidden);
        }

        /*
        private bool LogDealDamageAndReturnTrue(DealDamageAction dda)
        {
            Log.Debug($"DDA Log - Target: {dda.Target.Title}");
            Log.Debug($"Number of times redirected: {dda.NumberOfTimesRedirected}");
            Log.Debug($"Listing damage mod sources:");
            foreach (ModifyDealDamageAction mdd in dda.DamageModifiers)
            {
                Log.Debug(mdd.CardSource.Card.Title);
            }
            Log.Debug($"This card was involved: {dda.DamageModifiers.Any((ModifyDealDamageAction mdd) => mdd.CardSource != null && mdd.CardSource.Card == this.Card)}");
            return true;
        }
        */
        private IEnumerator RedirectDamageResponse(DealDamageAction dd)
        {
            if (!dd.IsRedirectable)
            {
                yield break;
            }

            IEnumerator coroutine = GameController.SelectTargetAndRedirectDamage(DecisionMaker,
                                                            (Card c) => IsVillainTarget(c) && !HasBeenSetToTrueThisTurn(GeneratePerTargetKey(mesmerKey, c)),
                                                            dd,
                                                            cardSource: GetCardSource());
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

        private IEnumerator MarkDamagedByRedirect(DealDamageAction dd)
        {
            if (IsVillainTarget(dd.Target))
            {
                SetCardPropertyToTrueIfRealAction(GeneratePerTargetKey(mesmerKey, dd.Target));
            }
            yield return null;
            yield break;
        }
        private IEnumerator DestroyVillainOngoingInsteadResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = CancelAction(dc);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && IsVillain(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "villain ongoing"), false, cardSource: GetCardSource());
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
        private IEnumerator LogAction(GameAction ga)
        {
            //Log.Debug($"GameAction detected from {ga.CardSource.Card.Title}");
            yield break;
        }
    }
}

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class OverwatchCardController : CardController
    {
        //==============================================================
        //Power: "Once before your next turn, when a hero target is dealt damage by a non-hero target, {Echelon} may deal the source of that damage 3 melee damage"
        //==============================================================

        public static string Identifier = "Overwatch";

        public OverwatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int numDamage = GetPowerNumeral(0, 3);

            //"Once before your next turn, when a hero target is dealt damage by a non-hero target, {Echelon} may deal the source of that damage 3 melee damage"
            var statusEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(MaybeCounterDamageResponse), $"Once before their next turn, when a hero target is dealt damage {DecisionMaker.Name} may deal the source of that damage {numDamage} melee damage.", new TriggerType[] { TriggerType.DealDamage }, DecisionMaker.TurnTaker, this.Card, new int[] { numDamage });
            statusEffect.TargetCriteria.IsHero = true;
            statusEffect.TargetCriteria.IsTarget = true;
            statusEffect.SourceCriteria.IsHero = false;
            statusEffect.SourceCriteria.IsTarget = true;
            statusEffect.DamageAmountCriteria.GreaterThan = 0;
            statusEffect.UntilStartOfNextTurn(DecisionMaker.TurnTaker);
            statusEffect.UntilTargetLeavesPlay(CharacterCard);
            statusEffect.BeforeOrAfter = BeforeOrAfter.After;
            statusEffect.DoesDealDamage = true;
            statusEffect.CanEffectStack = true;

            IEnumerator coroutine = AddStatusEffect(statusEffect);
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

        public IEnumerator MaybeCounterDamageResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            int numDamage = powerNumerals?[0] ?? 3;
            var player = FindHeroTurnTakerController(hero?.ToHero());

            if (dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsCard && dd.DamageSource.IsTarget && player != null && !dd.DamageSource.Card.IsBeingDestroyed && effect.CardMovedExpiryCriteria.Card == null)
            {
                var target = dd.DamageSource.Card;


                //...{Echelon} may...
                var storedYesNo = new List<YesNoCardDecision>();
                DealDamageAction fakeDamage = new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), target, numDamage, DamageType.Melee);
                IEnumerator coroutine = GameController.MakeYesNoCardDecision(player, SelectionType.DealDamage, this.Card, fakeDamage, storedResults: storedYesNo, associatedCards: new Card[] { target }, cardSource: GetCardSource(effect));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (!DidPlayerAnswerYes(storedYesNo))
                {
                    yield break;
                }

                //Once before the start of your next turn...
                //we set a criterion that will (hopefully) not matter much so that future reactions know it's being used
                effect.CardMovedExpiryCriteria.Card = CharacterCard;

                var damageSource = CharacterCard;
                if (effect is OnDealDamageStatusEffect oddEffect)
                {
                    damageSource = oddEffect.TargetLeavesPlayExpiryCriteria.Card ?? damageSource;
                }

                //deal the source of that damage 3 melee damage
                coroutine = DealDamage(damageSource, target, numDamage, DamageType.Melee, isCounterDamage: true, cardSource: GetCardSource(effect));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.ExpireStatusEffect(effect, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }



            }
            yield break;
        }
    }
}

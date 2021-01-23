using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class DanchengTheGiantCardController : TheInfernalChoirUtilityCardController
    {
        public DanchengTheGiantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private ITrigger _reduceDamageTrigger;
        private readonly string drawDiscardReaction = "DrawDiscardReaction";

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddTrigger((DrawCardAction drawCard) => drawCard.IsSuccessful && drawCard.DidDrawCard, DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            _reduceDamageTrigger = AddTrigger(new ReduceDamageTrigger(GameController, VerySecretCriteria, null, DiscardedCardDamageReduction, false, false, GetCardSource()));
        }

        //TODO - If Character card cannot deal damage, flag may not be cleared correctly
        private bool VerySecretCriteria(DealDamageAction dda)
        {
            return dda.OriginalAmount == 4 && dda.OriginalDamageType == DamageType.Infernal && dda.DamageSource.IsSameCard(CharacterCard) && Journal.GetCardPropertiesBoolean(dda.Target, drawDiscardReaction) == true && dda.CardSource.Card == Card;
        }

        private IEnumerator DiscardedCardDamageReduction(DealDamageAction dda)
        {
            if (IsRealAction())
            {
                Handelabra.Log.Debug($"{Card.Title} clearing flag: {drawDiscardReaction} on {dda.Target.Title}");
                Journal.RecordCardProperties(dda.Target, drawDiscardReaction, (bool?)null);
            }
            var coroutine = GameController.ReduceDamage(dda, 3, _reduceDamageTrigger, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(DrawCardAction drawCard)
        {
            List<Card> storedCharacter = new List<Card>();
            IEnumerator coroutine = FindCharacterCardToTakeDamage(drawCard.HeroTurnTaker, storedCharacter, base.CharacterCard, 4, DamageType.Infernal);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedCharacter.FirstOrDefault();
            if (card != null)
            {
                List<YesNoCardDecision> result = new List<YesNoCardDecision>();
                var httc = FindHeroTurnTakerController(drawCard.HeroTurnTaker);
                coroutine = GameController.MakeYesNoCardDecision(httc, SelectionType.DiscardCard, drawCard.DrawnCard, storedResults: result, associatedCards: new[] { Card }, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayerAnswerYes(result))
                {
                    coroutine = GameController.DiscardCard(httc, drawCard.DrawnCard, result.OfType<IDecision>(), TurnTaker, null, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                if (DidPlayerAnswerYes(result) && IsRealAction())
                {
                    Handelabra.Log.Debug($"{Card.Title} setting flag: {drawDiscardReaction} on {card.Title}");
                    Journal.RecordCardProperties(card, drawDiscardReaction, true);
                }

                coroutine = DealDamage(base.CharacterCard, card, 4, DamageType.Infernal);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}

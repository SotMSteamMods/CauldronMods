using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class EclipseCardController : TheInfernalChoirUtilityCardController
    {
        public EclipseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCardsInTrash();
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => EndOfTurnDamage(), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnDamage()
        {
            List<TurnTaker> result = new List<TurnTaker>();
            var coroutine = GameController.DetermineTurnTakersWithMostOrFewest(true, 1, 1, tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame, tt => tt.Trash.NumberOfCards, SelectionType.DealDamage, result, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> target = new List<Card>();
            coroutine = base.FindCharacterCardToTakeDamage(result.First(), target, CharacterCard, 3, DamageType.Infernal);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.DealDamageToTarget(new DamageSource(GameController, CharacterCard), target.First(), 3, DamageType.Infernal, cardSource: GetCardSource());
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

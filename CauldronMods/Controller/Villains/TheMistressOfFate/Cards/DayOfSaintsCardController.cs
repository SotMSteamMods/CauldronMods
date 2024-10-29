using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class DayOfSaintsCardController : DayCardController
    {
        public DayOfSaintsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
        }
        protected override IEnumerator DayFlipFaceUpEffect()
        {
            //"When this card flips face up, increase damage dealt by villain targets by 2 until the start of the next villain turn.",
            var boostEffect = new IncreaseDamageStatusEffect(2);
            boostEffect.UntilStartOfNextTurn(TurnTaker);
            boostEffect.SourceCriteria.IsVillain = true;
            boostEffect.SourceCriteria.IsTarget = true;

            IEnumerator coroutine = AddStatusEffect(boostEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //"Then {TheMistressOfFate} deals the hero with the lowest HP {H} times 3 psychic damage."
            coroutine = DealDamageToLowestHP(CharacterCard, 1, (Card c) =>  IsHeroCharacterCard(c), c => H * 3, DamageType.Psychic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

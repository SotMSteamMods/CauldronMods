using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class SliceCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public SliceCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Slice)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP(2);
        }

        protected override string AbilityDescription => $"{Card.Title} deals the hero with the second lowest HP *{H - 1}* sonic damage.";

        protected override string UltimateFormMessage => "Rock stars come and go. Musicians play until they die."; //Eddie Van Halen

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = DealDamageToLowestHP(Card, 2, c =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame, c => H - 1, DamageType.Sonic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected override void AddFlippedSideTriggers()
        {
            AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => UltimateEndOfTurn(), TriggerType.DealDamage));
        }

        private IEnumerator UltimateEndOfTurn()
        {
            List<DealDamageAction> results = new List<DealDamageAction>();
            var coroutine = DealDamageToLowestHP(Card, 2, c =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame, c => H - 1, DamageType.Sonic,
                                storedResults: results,
                                evenIfCannotDealDamage: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!results.Any() || results.First() is null)
                yield break;

            var effect = new CannotDealDamageStatusEffect();
            effect.SourceCriteria.IsAtLocation = results.First().Target.Owner.PlayArea;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.CardSource = Card;

            coroutine = AddStatusEffect(effect);
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
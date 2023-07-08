using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class DeathMetalCardController : ScreaMachineUtilityCardController
    {
        public DeathMetalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Valentine, ScreaMachineBandmate.Value.Bloodlace };

        private IEnumerator DeathMetalDestroyCard(TurnTaker tt, List<DestroyCardAction> result)
        {
            var httc = FindHeroTurnTakerController(tt.ToHero());
            var criteria = new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && c.Owner == tt && (IsOngoing(c) || IsEquipment(c)), "equipment or ongoing");
            var coroutine = GameController.SelectAndDestroyCard(httc, criteria, true, storedResultsAction: result, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator Play()
        {
            var result = new List<DestroyCardAction>();
            var fake = new DealDamageAction(GetCardSource(), new DamageSource(GameController, TurnTaker), null, 3, DamageType.Psychic);
            var coroutine = GameController.SelectTurnTakersAndDoAction(null, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame), SelectionType.DestroyCard,
                                actionWithTurnTaker: tt => DeathMetalDestroyCard(tt, result),
                                allowAutoDecide: true,
                                dealDamageInfo: new[] { fake },
                                cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var heroesThatDidNotDestroyCards = base.GetHeroesThatDidNotDestroyCards(result);

            IEnumerator coroutine2 = DealDamage(null, (Card c) =>  IsHeroCharacterCard(c) && heroesThatDidNotDestroyCards.Contains(FindHeroTurnTakerController(c.Owner.ToHero())), 3, DamageType.Psychic,
                                        damageSourceInfo: new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria(c => IsVillainTarget(c))));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            coroutine = base.ActivateBandAbilities(AbilityIcons);
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

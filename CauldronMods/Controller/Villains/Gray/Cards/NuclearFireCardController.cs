using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Gray
{
    public class NuclearFireCardController : CardController
    {
        public NuclearFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP(1, 2);
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP(1, 2);
        }

        public override IEnumerator Play()
        {
            //{Gray} deals the 2 hero targets with the highest HP {H - 1} energy damage each.
            IEnumerator coroutine = DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => new int?(Game.H - 1), DamageType.Energy, numberOfTargets: () => 2);
            //{Gray} deals the 2 hero targets with the lowest HP {H - 2} fire damage each.
            IEnumerator coroutine2 = DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => new int?(Game.H - 2), DamageType.Fire, numberOfTargets: 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}
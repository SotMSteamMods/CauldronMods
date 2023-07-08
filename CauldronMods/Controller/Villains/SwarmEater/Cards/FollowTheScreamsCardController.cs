using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class FollowTheScreamsCardController : CardController
    {
        public FollowTheScreamsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(numberOfTargets: () => Game.H, cardCriteria: new LinqCardCriteria((Card c) => !c.IsCharacter, "non-character"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //{SwarmEater} deals the {H} non-character targets with the lowest HP 4 irreducible projectile damage each.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => !c.IsCharacter, (Card c) => 4, DamageType.Projectile, true, numberOfTargets: Game.H);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //{SwarmEater} deals the hero target with the highest HP {H} melee damage.
            coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHero(c), (Card c) => Game.H, DamageType.Melee);
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
    }
}
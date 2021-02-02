using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class JailbreakerCardController : TerminusBaseCardController
    {
        /*
         * When this card enters play, add 3 tokens to your Wrath pool and destroy all other copies of Jailbreaker.
         * Powers 
         * {Terminus} deals herself 1 cold damage and any number of other targets 2 projectile damage each.
         */

        private int ColdDamageAmount => GetPowerNumeral(0, 1);
        private int ProjectileDamageAmount => GetPowerNumeral(1, 2);

        public JailbreakerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            coroutine = AddWrathTokens(3);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((lcc) => lcc.Identifier == base.Card.Identifier && lcc != base.Card && lcc.IsInPlay), cardSource: base.GetCardSource());

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

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.DealDamageToSelf(DecisionMaker, (card) => card == base.CharacterCard, ColdDamageAmount, DamageType.Cold, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((lcc) => lcc.IsTarget && lcc.IsInPlayAndHasGameText && lcc != base.CharacterCard), SelectionType.DealDamage, ActionWithCardResponse, null, false, 0, cardSource: base.GetCardSource());                
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

        private IEnumerator ActionWithCardResponse(Card targetCard)
        {
            IEnumerator coroutine;
            
            coroutine = base.DealDamage(base.CharacterCard, (card) => targetCard == card, ProjectileDamageAmount, DamageType.Projectile);
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

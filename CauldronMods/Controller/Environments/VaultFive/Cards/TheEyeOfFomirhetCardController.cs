using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class TheEyeOfFomirhetCardController : ArtifactCardController
    {
        public TheEyeOfFomirhetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UniqueOnPlayEffect()
        {
            //a hero from its deck...
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = SelectActiveHeroCharacterCardToDoAction(storedResults, SelectionType.GainHP);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card hero = storedResults.FirstOrDefault();
            if (hero == null)
            {
                yield break;
            }
            //...regains 4HP...
            coroutine = GameController.GainHP(hero, 4, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //.. and deals each other hero target 1 infernal damage
            coroutine = DealDamage(hero, (Card c) => IsHeroTarget(c) && c != hero, 1, DamageType.Infernal);
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

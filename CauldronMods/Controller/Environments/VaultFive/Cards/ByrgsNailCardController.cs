using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class ByrgsNailCardController : ArtifactCardController
    {
        public ByrgsNailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(numberOfTargets: () => 3);
        }

        public override IEnumerator UniqueOnPlayEffect()
        {
            //a hero from its deck deals the 3 targets with the lowest HP 3 melee damage each
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = SelectActiveHeroCharacterCardToDoAction(storedResults, SelectionType.HeroToDealDamage);
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
            coroutine = DealDamageToLowestHP(hero, 1, (Card c) => c.IsTarget, (Card c) => 3, DamageType.Melee, numberOfTargets: 3);
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

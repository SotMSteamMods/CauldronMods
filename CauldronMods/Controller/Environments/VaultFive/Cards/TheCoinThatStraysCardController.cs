using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class TheCoinThatStraysCardController : ArtifactCardController
    {
        public TheCoinThatStraysCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UniqueOnPlayEffect()
        {
            //a hero from its deck
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
            //... deals themselves...
            coroutine = DealDamage(hero, hero, 1, DamageType.Psychic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...and 1 other target 1 psychic damage each...
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, hero), 1, DamageType.Psychic, 1, false, 1, additionalCriteria: (Card c) => c != hero, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...and plays a card
            coroutine = SelectAndPlayCardFromHand(FindHeroTurnTakerController(hero.Owner.ToHero()));
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

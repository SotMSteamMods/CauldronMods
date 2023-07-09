using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class StellarWindCardController : StarlightCardController
    {
        public StellarWindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsTarget && IsNextToConstellation(c), useCardsSuffix: false, singular: "target next to a constellation", plural: "targets next to a constellation"));
        }
        public override IEnumerator Play()
        {
            //for multi-char promo
            List<Card> storedResults = new List<Card> { };
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, 2, DamageType.Cold);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDamageSource);
            }
            Card actingStarlight = storedResults.FirstOrDefault();

            IEnumerator damageRoutine;
            if (actingStarlight == null)
            {
                damageRoutine = GameController.SendMessageAction("There was no appropriate source of damage.", Priority.High, GetCardSource());
            }
            else
            {
                //allowAutodecide will hit every target it can. Sometimes this is fine (all villains, Celestial Aura out)
                //Sometimes this is not.
                bool safeToAutodecide = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "CelestialAura")).Any() ||
                                            FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsHero(c) && IsNextToConstellation(c))).Count() == 0;
                //"{Starlight} may deal 2 cold damage to each target next to a constellation.",
                damageRoutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, actingStarlight), 2, DamageType.Cold, null, false, 0, allowAutoDecide: safeToAutodecide, additionalCriteria: (Card c) => IsNextToConstellation(c), cardSource: GetCardSource());
            }

            //"Draw 2 cards."
            IEnumerator draw = DrawCards(HeroTurnTakerController, 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(damageRoutine);
                yield return GameController.StartCoroutine(draw);
            }
            else
            {
                GameController.ExhaustCoroutine(damageRoutine);
                GameController.ExhaustCoroutine(draw);
            }
            yield break;
        }

    }
}
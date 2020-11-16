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
        }
        public override IEnumerator Play()
        {
            //for multi-char promo
            List<Card> storedResults = new List<Card> { };
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, 2, DamageType.Cold);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseDamageSource);
            }
            Card actingStarlight = storedResults.FirstOrDefault();

            IEnumerator damageRoutine;
            if (actingStarlight == null)
            {
                damageRoutine = GameController.SendMessageAction("There was no appropriate source of damage.", Priority.High, GetCardSource());
            }
            else
            {
                //"{Starlight} may deal 2 cold damage to each target next to a constellation.",
                damageRoutine = GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, actingStarlight), 2, DamageType.Cold, null, false, 0, additionalCriteria: (Card c) => IsNextToConstellation(c), cardSource:GetCardSource());
            }

            //"Draw 2 cards."
            IEnumerator draw = DrawCards(HeroTurnTakerController, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
                yield return base.GameController.StartCoroutine(draw);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
                base.GameController.ExhaustCoroutine(draw);
            }
            yield break;
        }

    }
}
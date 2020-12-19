﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class AssassinsSignatureCardController : CardController
    {
        //==============================================================
        // {Vector} deals the hero target with the highest HP {H} projectile damage.
        // That hero must destroy 1 of their ongoing or equipment cards."
        //==============================================================

        public static readonly string Identifier = "AssassinsSignature";

        private const int CardsToDestroy = 1;

        public AssassinsSignatureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            int damageAmount = base.Game.H;

            List<DealDamageAction> ddas = new List<DealDamageAction>();
            IEnumerator damageRoutine 
                = base.DealDamageToHighestHP(base.CharacterCard, 1, card => card.IsHero && card.IsTarget && card.IsInPlay, 
                    card => damageAmount, DamageType.Projectile, storedResults: ddas);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
            }

            if (!ddas.Any())
            {
                yield break;
            }

            HeroTurnTakerController httc = FindHeroTurnTakerController(ddas.First().Target.Owner.ToHero());

            IEnumerator destroyRoutine = base.GameController.SelectAndDestroyCards(httc,
                new LinqCardCriteria(card => (card.IsOngoing || IsEquipment(card)) && card.IsInPlay && card.Owner == httc.TurnTaker), 
                CardsToDestroy, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }

        }
    }
}
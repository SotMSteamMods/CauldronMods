using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WarpHaloCardController : StarlightCardController
    {
        public WarpHaloCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => IsHeroTarget(c) && IsNextToConstellation(c), useCardsSuffix: false, singular: "hero target next to a constellation", plural: "hero targets next to a constellation"));
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => c.IsTarget && !IsHeroTarget(c) && IsNextToConstellation(c), useCardsSuffix: false, singular: "non-hero target next to a constellation", plural: "non-hero targets next to a constellation"));
        }

        public override void AddTriggers()
        {
            //"If a hero target next to a constellation would deal damage to a non-hero target next to a constellation, increase that damage by 1."
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null &&  dd.DamageSource.IsHeroTarget && IsNextToConstellation(dd.DamageSource.Card) && !IsHeroTarget(dd.Target) && IsNextToConstellation(dd.Target), 1);
        }

    }
}

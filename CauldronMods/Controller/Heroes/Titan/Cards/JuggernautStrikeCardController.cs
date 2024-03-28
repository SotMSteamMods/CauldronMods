using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class JuggernautStrikeCardController : TitanCardController
    {
        public JuggernautStrikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            Card titanform = base.GetTitanform();
            IEnumerator coroutine = null;
            //If Titanform is in your hand you may play it now.
            if (titanform.Location.IsHand && titanform.Location.OwnerTurnTaker == base.TurnTaker)
            {
                coroutine = base.GameController.PlayCard(base.TurnTakerController, titanform, optional: true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //{Titan} deals 1 target 4 infernal damage...
            IEnumerable<Card> choices = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay));
            List<SelectCardDecision> selectTargets = new List<SelectCardDecision>();
            coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 4, DamageType.Infernal, 1, false, 1, storedResultsDecisions: selectTargets, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidSelectCard(selectTargets))
            {
                yield break;
            }
            Card selectedTarget = GetSelectedCard(selectTargets);
            //..and each other target from that deck 1 projectile damage.
            coroutine = base.GameController.DealDamage(base.HeroTurnTakerController, base.CharacterCard, (Card c) => c != selectedTarget && GetNativeDeck(c) == GetNativeDeck(selectedTarget), 1, DamageType.Projectile, cardSource: base.GetCardSource());
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
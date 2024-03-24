using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CyberdefenseCardController : CypherBaseCardController
    {

        //==============================================================
        // Destroy any number of Augments.
        // {Cypher} deals each non-hero target X lightning damage,
        // where X equals the number of cards destroyed this way.
        //==============================================================

        public static string Identifier = "Cyberdefense";

        public CyberdefenseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowSpecialStringNumberOfAugmentsInPlay();
        }

        public override IEnumerator Play()
        {
            // Destroy any number of Augments.
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator routine = base.GameController.SelectAndDestroyCards(this.HeroTurnTakerController, base.AugmentCardCriteria(), 
                null, false, 0, storedResultsAction: storedResults, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Cypher deals each non-hero target X lightning damage, where X equals the number of cards destroyed this way.
            int damageAmount = storedResults.Count((DestroyCardAction dc) => dc.WasCardDestroyed);
            routine = base.GameController.DealDamage(this.HeroTurnTakerController, this.CharacterCard, c => !IsHeroTarget(c),
                damageAmount, DamageType.Lightning, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheYoungKnightCharacterCardController : MultiKnightIndividualCardController
    {
        public TheYoungKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int otherTargets = GetPowerNumeral(0, 2);
            int damage = GetPowerNumeral(1, 2);
            //"{TheYoungKnightCharacter} deals herself... 2 toxic damage"
            IEnumerator coroutine = DealDamage(this.Card, this.Card, damage, DamageType.Toxic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //...[Knight deals] 2 other targets 2 toxic damage each
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.Card), damage, DamageType.Toxic, otherTargets, false, otherTargets, additionalCriteria:(Card c) => c != this.Card, cardSource: GetCardSource());
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

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            //"flippedBody": "When {TheYoungKnightCharacter} flips to this side, destroy all equipment cards next to her.",
            yield return base.AfterFlipCardImmediateResponse();
            if (this.Card.IsFlipped)
            {
                IEnumerator coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => GetKnightCardUser(c) == this.Card, $"equipment by The Young Knight", false), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
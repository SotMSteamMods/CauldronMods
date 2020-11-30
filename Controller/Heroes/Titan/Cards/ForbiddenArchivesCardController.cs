using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron
{
    public class ForbiddenArchivesCardController : CardController
    {
        public ForbiddenArchivesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<DrawCardAction> storedResults = new List<DrawCardAction>();
            //Each player may draw 2 cards now.
            IEnumerator coroutine = this.GameController.DrawCards(new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.ToHero().IsIncapacitatedOrOutOfGame, "active heroes"), 2, true, storedResults: storedResults, cardSource: this.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            foreach (DrawCardAction action in storedResults)
            {
                coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class SubharmonicReceiverCardController : CardController
    {
        public SubharmonicReceiverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Each player may draw a card. When a player draws a card this way, 1 other player must discard a card.
            int otherPlayers = GetPowerNumeral(0, 1);
            List<DrawCardAction> storedResults = new List<DrawCardAction>();
            //Each player may draw a card.
            IEnumerator coroutine = base.EachPlayerDrawsACard(optional: true, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //When a player draws a card this way...
            if (storedResults.Any())
            {
                foreach (DrawCardAction action in storedResults)
                {
                    //If Guise makes more/fewer players discard cards
                    for (int i = 0; i < otherPlayers; i++)
                    {
                        //...1 other player must discard a card.
                        IEnumerable<TurnTaker> choices = base.Game.TurnTakers.Where((TurnTaker tt) => tt.IsHero && tt.ToHero() != action.HeroTurnTaker);
                        SelectTurnTakerDecision decision = new SelectTurnTakerDecision(base.GameController, base.HeroTurnTakerController, choices, SelectionType.DiscardCard, true, cardSource: base.GetCardSource());
                        coroutine = base.GameController.SelectTurnTakerAndDoAction(decision, (TurnTaker tt) => base.GameController.SelectAndDiscardCard(base.FindHeroTurnTakerController(tt.ToHero())));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                }
            }
            yield break;
        }
    }
}
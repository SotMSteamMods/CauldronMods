using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class BattlefieldScavengerCardController : TheKnightCardController
    {
        public BattlefieldScavengerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Each player may draw a card, or take an Equipment or Ongoing card from their trash and put it on top of their deck.",
            IEnumerator coroutine = base.EachPlayerSelectsFunction((HeroTurnTakerController h) => !h.IsIncapacitatedOrOutOfGame, httc => ChoiceFunction(httc), 0,
                outputIfCannotChooseFunction: httc => httc.Name + " has no cards in their trash.");
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

        private IEnumerable<Function> ChoiceFunction(HeroTurnTakerController httc)
        {
            var drawACard = new Function(httc, "Draw a card", SelectionType.DrawCard, () => this.DrawCard(httc.HeroTurnTaker), CanDrawCards(httc));

            var criteria = new LinqCardCriteria(c => IsEquipment(c) || c.IsOngoing, "equipment or ongoing");
            var coroutine = this.GameController.SelectCardFromLocationAndMoveIt(httc, httc.HeroTurnTaker.Trash, criteria, new[] { new MoveCardDestination(httc.HeroTurnTaker.Deck) }, cardSource: this.GetCardSource());
            var pullFromTrash = new Function(httc, "Select an Equipment or Ongoing from the trash to put on top of your deck", SelectionType.MoveCardOnDeck, () => coroutine, httc.HeroTurnTaker.Trash.HasCards);

            return new Function[]
            {
                drawACard,
                pullFromTrash
            };
        }
    }
}

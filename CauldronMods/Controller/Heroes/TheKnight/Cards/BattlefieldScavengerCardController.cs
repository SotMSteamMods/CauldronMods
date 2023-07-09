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
            var query = GameController.HeroTurnTakerControllers.Where(httc => httc.TurnTaker.Trash.Cards.Any(c => IsEquipment(c) || IsOngoing(c)))
                                                               .Select(httc => httc.TurnTaker.Trash);
            var ss = SpecialStringMaker.ShowNumberOfCardsAtLocations(() => query, new LinqCardCriteria(c => IsEquipment(c) || IsOngoing(c), "equipment or ongoing"));
            ss.Condition = () => GameController.HeroTurnTakerControllers.Any(httc => httc.TurnTaker.Trash.Cards.Any(c => IsEquipment(c) || IsOngoing(c)));

            ss = SpecialStringMaker.ShowSpecialString(() => "No hero has any equipment or ongoing cards in their trash.");
            ss.Condition = () => GameController.HeroTurnTakerControllers.All(httc => !httc.TurnTaker.Trash.Cards.Any(c => IsEquipment(c) || IsOngoing(c)));
        }

        public override IEnumerator Play()
        {
            //"Each player may draw a card, or take an Equipment or Ongoing card from their trash and put it on top of their deck.",
            IEnumerator coroutine = base.EachPlayerSelectsFunction((HeroTurnTakerController h) => !h.IsIncapacitatedOrOutOfGame, httc => ChoiceFunction(httc), 0,
                outputIfCannotChooseFunction: httc => httc.Name + " has no cards in their trash and cannot draw cards.");
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

            var criteria = new LinqCardCriteria(c => c.IsInLocation(httc.TurnTaker.Trash) && (IsEquipment(c) || IsOngoing(c)), "equipment or ongoing");
            var coroutine = this.GameController.SelectCardFromLocationAndMoveIt(httc, httc.HeroTurnTaker.Trash, criteria, new[] { new MoveCardDestination(httc.HeroTurnTaker.Deck) }, showOutput: true, cardSource: this.GetCardSource());
            var pullFromTrash = new Function(httc, "Select an Equipment or Ongoing from the trash to put on top of your deck", SelectionType.MoveCardOnDeck, () => coroutine, httc.FindCardsWhere(criteria.Criteria).Any());

            return new Function[]
            {
                drawACard,
                pullFromTrash
            };
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class DesolationCardController : CardController
    {
        //==============================================================
        // When this card enters play, each hero must discard all but 1 card,
        // or this card deals that hero {H + 1} psychic damage.
        // At the end of the environment turn, destroy this card.
        //==============================================================

        public static readonly string Identifier = "Desolation";

        public DesolationCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            // Destroy self at end of env. turn
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker,
                base.DestroyThisCardResponse,
                TriggerType.DestroySelf);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            int damageToDeal = Game.H + 1;

            IEnumerable<Function> Functions(HeroTurnTakerController httc) => new Function[2]
            {
                new Function(httc, "Discard all but 1 card", SelectionType.DiscardCard,
                    () => base.GameController.SelectCardsFromLocationAndMoveThem(httc, httc.HeroTurnTaker.Hand,
                        httc.NumberOfCardsInHand - 1, httc.NumberOfCardsInHand - 1, 
                        new LinqCardCriteria(card => card.Location == httc.HeroTurnTaker.Hand, "hand"),
                        new[] { new MoveCardDestination(httc.TurnTaker.Trash) }, cardSource: base.GetCardSource())),

                new Function(httc, $"Take {damageToDeal} psychic damage", SelectionType.DealDamage,
                    () => this.DealDamage(this.Card, (Card c) => httc.CharacterCards.Contains(c) && c.IsTarget, damageToDeal, DamageType.Psychic))
            };

            IEnumerator coroutine = EachPlayerSelectsFunction(h =>
                    !h.IsIncapacitatedOrOutOfGame, Functions, Game.H, null,
                h => h.Name + " has no cards in their hand.");

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

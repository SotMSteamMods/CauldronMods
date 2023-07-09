using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class BioterrorSquadCardController : VectorBaseCardController
    {
        //==============================================================
        // At the start of the villain turn, shuffle a Virus card
        // from the villain trash into the villain deck.
        //
        // At the end of the villain turn, this card deals the hero target
        // with the highest HP {H} projectile damage, then {Vector}
        // deals this card 1 toxic damage.
        //==============================================================

        public static readonly string Identifier = "BioterrorSquad";

        private const int DamageToDealSelf = 1;

        public BioterrorSquadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsVirus(c), "virus"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            AddStartOfTurnTrigger(tt => tt == TurnTaker, StartOfTurnResponse, TriggerType.ShuffleCardIntoDeck);

            AddEndOfTurnTrigger(tt => tt == TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            if (FindCardsWhere((Card c) => TurnTaker.Trash.HasCard(c) && IsVirus(c)).Any())
            {
                var destination = new[] { new MoveCardDestination(TurnTaker.Deck) };
                IEnumerator routine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, null, 1,
                                    new LinqCardCriteria(c => IsVirus(c), "virus"),
                                    destination,
                                    showOutput: true,
                                    selectionType: SelectionType.ShuffleCardFromTrashIntoDeck,
                                    cardSource: GetCardSource());
                IEnumerator routine2 = ShuffleDeck(DecisionMaker, TurnTaker.Deck);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                    yield return base.GameController.StartCoroutine(routine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                    base.GameController.ExhaustCoroutine(routine2);
                }
            }
            else
            {
                IEnumerator routine = base.GameController.SendMessageAction($"There are no Virus cards in {CharacterCard.Title} 's Trash", Priority.Medium, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            yield break;
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            int damageAmount = Game.H;

            IEnumerator damageRoutine = DealDamageToHighestHP(Card, 1, c => IsHero(c) && c.IsInPlay, c => damageAmount, DamageType.Projectile);

            IEnumerator damageRoutine2 = DealDamage(CharacterCard, Card, DamageToDealSelf, DamageType.Toxic, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
                yield return base.GameController.StartCoroutine(damageRoutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
                base.GameController.ExhaustCoroutine(damageRoutine2);
            }
        }
    }
}
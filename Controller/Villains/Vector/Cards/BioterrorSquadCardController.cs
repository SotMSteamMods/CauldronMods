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
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, 
                StartOfTurnResponse, TriggerType.ShuffleCardIntoDeck);

            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker,
                EndOfTurnResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            if (FindCardsWhere((Card c) => base.TurnTaker.Trash.HasCard(c) && IsVirus(c)).Any())
            {

                MoveCardDestination turnTakerDeck = new MoveCardDestination(base.TurnTaker.Deck);

                IEnumerator routine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker,
                    base.TurnTaker.Trash, new LinqCardCriteria(c => base.TurnTaker.Trash.HasCard(c) && IsVirus(c)),
                    turnTakerDeck.ToEnumerable(),
                    showOutput: true, cardSource: base.GetCardSource());

                IEnumerator routine2 = base.ShuffleDeck(this.DecisionMaker, base.TurnTaker.Deck);

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
            } else
            {
                IEnumerator routine = base.GameController.SendMessageAction("There are no Virus cards in " + base.CharacterCard.Title + "'s Trash", Priority.Medium, cardSource: GetCardSource());
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
            int damageAmount = base.Game.H;
            
            IEnumerator damageRoutine = this.DealDamageToHighestHP(this.Card, 1, c => c.IsHero && c.IsInPlay, 
                c => damageAmount, DamageType.Projectile);

            IEnumerator damageRoutine2 = this.DealDamage(this.CharacterCard, this.Card, DamageToDealSelf, 
                DamageType.Toxic, cardSource: base.GetCardSource());

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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class SupervirusCardController : VectorBaseCardController
    {
        //==============================================================
        // At the start of the villain turn you may put 1 Virus card
        // from the villain trash beneath this card. Then, {Vector}
        // deals each hero 1 toxic damage and regains {H x 2} HP.
        //
        // If {Vector} is destroyed, the heroes lose.
        //==============================================================

        public static readonly string Identifier = "Supervirus";

        private const int DamageToDeal = 1;
        private const string LoseMessage = "Vector was destroyed while Super Virus was active!  The Heroes lose!";

        public SupervirusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.Card.UnderLocation).Condition = ()=> base.Card.IsInPlayAndHasGameText ;
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnResponse,
                new[]
                {
                    TriggerType.MoveCard,
                    TriggerType.DealDamage
                });

            base.AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy != null && dca.WasCardDestroyed 
                    && dca.CardToDestroy.Card == this.CharacterCard, 
                GameOverResponse, TriggerType.GameOver, TriggerTiming.After);

            base.AddTrigger<MoveCardAction>((MoveCardAction mca) => mca.Destination == base.Card.UnderLocation, FlipVectorResponse, TriggerType.FlipCard, TriggerTiming.After);
            base.AddTrigger<BulkMoveCardsAction>((BulkMoveCardsAction bmca) => bmca.Destination == base.Card.UnderLocation, FlipVectorResponse, TriggerType.FlipCard, TriggerTiming.After);

            base.AddTriggers();
        }

        private IEnumerator FlipVectorResponse(MoveCardAction mca)
        {
            // A virus card was moved under this card, check for flip condition
            if (!ShouldVectorFlip())
            {
                yield break;
            }

            // Flip Vector
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(FlipVector());
            }
            else
            {
                base.GameController.ExhaustCoroutine(FlipVector());
            }

            yield break;
        }

        private IEnumerator FlipVectorResponse(BulkMoveCardsAction bmca)
        {
            // A virus card was moved under this card, check for flip condition
            if (!ShouldVectorFlip())
            {
                yield break;
            }

            // Flip Vector
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(FlipVector());
            }
            else
            {
                base.GameController.ExhaustCoroutine(FlipVector());
            }

            yield break;
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            // You may put 1 Virus card from the villain trash beneath this card
            List<SelectCardDecision> cardsSelected = new List<SelectCardDecision>();
            MoveCardDestination underThisCard = new MoveCardDestination(base.Card.UnderLocation);

            IEnumerator routine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria(c => c.IsInTrash && IsVirus(c)),
                underThisCard.ToEnumerable(), isPutIntoPlay: true, playIfMovingToPlayArea: false, optional: true, storedResults: cardsSelected, showOutput: true, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

           if(!IsVectorFlipped())
            {
                // {Vector} deals each hero 1 toxic damage
                routine = base.DealDamage(this.CharacterCard, c =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame,
                    DamageToDeal, DamageType.Toxic);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                // Regain {H x 2} HP
                int hpGain = base.Game.H * 2;
                routine = this.GameController.GainHP(this.CharacterCard, hpGain, cardSource: GetCardSource());
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

        private IEnumerator GameOverResponse(DestroyCardAction dca)
        {
            IEnumerator routine = base.GameController.GameOver(EndingResult.AlternateDefeat, LoseMessage, actionSource: dca, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card || card.Location == base.Card.UnderLocation;
        }
    }
}
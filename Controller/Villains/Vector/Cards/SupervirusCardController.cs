
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

        private const int CardsToMove = 1;
        private const int DamageToDeal = 1;

        public SupervirusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //AddTrigger((GameAction t) => (t is DestroyCardAction && (t as DestroyCardAction).CardToDestroy.Card == this.CharacterCard, (GameAction t) => GameOverResponse(didHeroesWin: true), TriggerType.GameOver, TriggerTiming.After);

            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnResponse,
                new[]
                {
                    TriggerType.MoveCard,
                    TriggerType.DealDamage
                });



            /*
            base.AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy != null ,
                this.DestroyVectorResponse,
                new[]
                {
                    TriggerType.DestroyCard
                }, TriggerTiming.After, null, false, true, true);
            */

            //base.AddTrigger<DealDamageAction>(dda => dda.Target == this.CharacterCard && this.CharacterCard.HitPoints <= 0, 
                //this.GameOverResponse, TriggerType.GameOver, TriggerTiming.Before);

            base.AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy != null && dca.WasCardDestroyed 
                    && dca.CardToDestroy.Card == this.CharacterCard, 
                GameOverResponse, TriggerType.GameOver, TriggerTiming.After);


            //AddTrigger(ga => (ga is DestroyCardAction dca) && dca.CardToDestroy != null && dca.CardToDestroy.Card == this.CharacterCard, 
            //(GameAction t) => GameOverResponse, TriggerType.DestroyCard, TriggerTiming.After);


            //AddTrigger(d => d.WasCardDestroyed && d.CardToDestroy.Card == this.CharacterCard && d.CardSource != null, 
            //(DestroyCardAction d) => GameOverResponse, 
            //TriggerType.AddTokensToPool, TriggerTiming.After);

            //base.AddWhenDestroyedTrigger(WhenDestroyedResponse, TriggerType.GameOver);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            // Make this card indestructible
            MakeIndestructibleStatusEffect ise = new MakeIndestructibleStatusEffect();
            ise.CardsToMakeIndestructible.IsSpecificCard = this.Card;
            IEnumerator routine = base.AddStatusEffect(ise, true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            // You may put 1 Virus card from the villain trash beneath this card
            List<SelectCardDecision> cardsSelected = new List<SelectCardDecision>();
            MoveCardDestination underThisCard = new MoveCardDestination(base.Card.UnderLocation);

            IEnumerator routine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria(c => c.IsInTrash && IsVirus(c)),
                underThisCard.ToEnumerable(), optional: true, storedResults: cardsSelected, showOutput: true, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // {Vector} deals each hero 1 toxic damage
            routine = base.DealDamage(this.CharacterCard, c => c.IsHero && c.IsTarget && c.IsInPlay, 
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
            routine = this.GameController.GainHP(this.CharacterCard, hpGain);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (!cardsSelected.Any())
            {
                yield break;
            }

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
        }

        /*
        private IEnumerator GameOverResponse(DealDamageAction dda)
        {
            string ending = "Vector was destroyed while Super Virus was active!  The Heroes lose!";
            
            IEnumerator routine = base.GameController.GameOver(EndingResult.AlternateDefeat, ending, actionSource: dda, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
        */
        private IEnumerator GameOverResponse(DestroyCardAction dca)
        {
            string ending = "Vector was destroyed while Super Virus was active!  The Heroes lose!";

            IEnumerator routine = base.GameController.GameOver(EndingResult.AlternateDefeat, ending, actionSource: dca, cardSource: GetCardSource());
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
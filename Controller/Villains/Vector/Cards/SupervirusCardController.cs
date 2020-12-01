
using System.Collections;
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

            AddTrigger(ga => (ga is DestroyCardAction dca) && dca.WasCardDestroyed && dca.CardToDestroy.Card == this.CharacterCard, 
                (GameAction t) => GameOverResponse(), TriggerType.GameOver, TriggerTiming.After);


            //AddTrigger(d => d.WasCardDestroyed && d.CardToDestroy.Card == this.CharacterCard && d.CardSource != null, 
                //(DestroyCardAction d) => GameOverResponse, 
                //TriggerType.AddTokensToPool, TriggerTiming.After);

            //base.AddWhenDestroyedTrigger(WhenDestroyedResponse, TriggerType.GameOver);

            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnResponse, 
                new[]
                {
                    TriggerType.MoveCard, 
                    TriggerType.DealDamage
                });

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
            MoveCardDestination underThisCard = new MoveCardDestination(base.Card.UnderLocation);

            IEnumerator routine = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsInTrash && IsVirus(c)),
                underThisCard.ToEnumerable<MoveCardDestination>(), optional: true, showOutput: true, cardSource: base.GetCardSource());

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
        }

        private IEnumerator GameOverResponse()
        {
            string ending = "Vector was destroyed while Super Virus was active!  The Heroes lose!";
            
            IEnumerator routine = base.GameController.GameOver(EndingResult.AlternateDefeat, ending, cardSource: GetCardSource());
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
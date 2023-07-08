using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighTormulCardController : OriphelGuardianCardController
    {
        public HighTormulCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"At the start of each hero turn, this card deals that hero 2 toxic damage.",
            AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame,
                                      DamageTurnTakerHeroResponse,
                                      TriggerType.DealDamage);
        }

        private IEnumerator DamageTurnTakerHeroResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(FindHeroTurnTakerController(Game.ActiveTurnTaker.ToHero()),
                                                                                new DamageSource(GameController, this.Card),
                                                                                2,
                                                                                DamageType.Toxic,
                                                                                1,
                                                                                false,
                                                                                1,
                                                                                additionalCriteria: (Card c) => c.Owner == Game.ActiveTurnTaker &&  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame,
                                                                                cardSource: GetCardSource());
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
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class FaithfulProselyteCardController : MythosUtilityCardController
    {
        public FaithfulProselyteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.DeckIconList());
            base.SpecialStringMaker.ShowSpecialString(() => base.ThisCardsIcon());
        }

        public override IEnumerator Play()
        {
            //{MythosMadness} When this card enters play, destroy {H - 2} equipment cards.
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c)), base.Game.H - 2, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the hero target with the second highest HP {H - 1} lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the second highest HP {H - 1} lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(this.Card, 2, (Card c) => c.IsHero, (Card c) => base.Game.H - 1, DamageType.Lightning);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}

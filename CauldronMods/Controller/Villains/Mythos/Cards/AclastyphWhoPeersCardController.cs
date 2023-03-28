using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class AclastyphWhoPeersCardController : MythosUtilityCardController
    {
        public AclastyphWhoPeersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn:
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.MoveCard,
                TriggerType.GainHP
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine;
            if (base.IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                //{MythosDanger} This card deals the hero target with the highest HP 2 melee damage. 
                coroutine = base.DealDamageToHighestHP(this.Card, 1, (Card c) => IsHero(c) && c.IsInPlayAndHasGameText, (Card c) => 2, DamageType.Melee);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Discard the top card of the villain deck.
                coroutine = base.DiscardCardsFromTopOfDeck(base.TurnTakerController, 1);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (base.IsTopCardMatching(MythosClueDeckIdentifier))
            {
                //{MythosClue} This card regains 2HP.
                coroutine = base.GameController.GainHP(this.Card, 2, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Discard the top card of the villain deck.
                coroutine = base.DiscardCardsFromTopOfDeck(base.TurnTakerController, 1);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                //{MythosMadness} This card deals each other target 1 psychic damage.
                coroutine = base.DealDamage(this.Card, (Card c) => c != this.Card, 1, DamageType.Psychic);
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
    }
}

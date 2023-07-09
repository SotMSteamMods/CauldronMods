using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class PreyUponTheMindCardController : MythosUtilityCardController
    {
        public PreyUponTheMindCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Destroy {H - 2} hero ongoing cards.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && IsOngoing(c)), Game.H - 2, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                //{MythosMadness} Each player discards 1 card.
                coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (base.IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                //{MythosDanger} {Mythos} deals each hero target 1 infernal damage.
                coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHeroTarget(c), 1, DamageType.Infernal);
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

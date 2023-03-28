using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class PelagaCardController : VaultFiveUtilityCardController
    {
        public PelagaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildPlayersWithMostArtifactsSpecialString(this));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each target from the hero deck with the most Artifacts 3 toxic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);

            //Whenever this card is dealt damage by a hero target, that hero must discard 1 non-Artifact card.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.Target == Card && dd.DidDealDamage && dd.DamageSource.IsHero && dd.DamageSource.IsTarget, DiscardCardResponse, TriggerType.DiscardCard, TriggerTiming.After);
        }

        private IEnumerator DiscardCardResponse(DealDamageAction dd)
        {
            //that hero must discard 1 non - Artifact card.

            HeroTurnTakerController hero = FindHeroTurnTakerController(dd.DamageSource.Card.Owner.ToHero());
            IEnumerator coroutine = base.GameController.SelectAndDiscardCards(hero, 1, false, 1, cardCriteria: new LinqCardCriteria((Card c) => !IsArtifact(c), "non-artifact"), cardSource: GetCardSource());
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

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //find hero with the most artifacts
            IEnumerable<TurnTaker> heroesWithMostArtifacts = FindHeroWithMostArtifacts(this.GetCardSource());
            TurnTaker hero = null;
            if(heroesWithMostArtifacts.Count() == 0)
            {
                List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>() ;
                IEnumerator coroutine = GameController.SelectTurnTaker(DecisionMaker, SelectionType.DealDamage, storedResults, additionalCriteria: (TurnTaker tt) => IsHero(tt) && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectTurnTaker(storedResults))
                {
                    hero = GetSelectedTurnTaker(storedResults);
                }
            } else if(heroesWithMostArtifacts.Count() == 1)
            {
                hero = heroesWithMostArtifacts.FirstOrDefault();
            } else
            {
                List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
                IEnumerator coroutine = GameController.SelectTurnTaker(DecisionMaker, SelectionType.DealDamage, storedResults, additionalCriteria: (TurnTaker tt) => heroesWithMostArtifacts.Contains(tt), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectTurnTaker(storedResults))
                {
                    hero = GetSelectedTurnTaker(storedResults);
                }
            }

            if (hero == null)
            {
                IEnumerator message = GameController.SendMessageAction("Something went wrong and we couldn't find a hero! Please submit a bug report!", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            IEnumerator message2 = GameController.SendMessageAction(hero.ShortName + " is the hero with the most artifacts." , Priority.High, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(message2);
            }

            //this card deals each target from the hero deck with the most Artifacts 3 toxic damage.
            IEnumerator coroutine2 = DealDamage(Card, (Card c) => c.Owner == hero && c.IsInPlayAndHasGameText, 3, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            yield break;
        }
    }
}

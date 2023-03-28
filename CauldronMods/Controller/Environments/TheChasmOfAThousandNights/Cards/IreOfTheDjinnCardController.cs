using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class IreOfTheDjinnCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public IreOfTheDjinnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            // Redirect all damage dealt by environment targets to the hero target with the highest HP. Nature cards cannot redirect this damage.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(dd.DamageSource.Card, GetCardSource()), RedirectDamageResponse, TriggerType.RedirectDamage, TriggerTiming.Before);

            // At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dealDamage)
        {
            IEnumerator coroutine = RedirectDamage(dealDamage, TargetType.HighestHP, (Card c) => IsHero(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()));
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

        public override IEnumerator Play()
        {
            //When this card enters play, 3 players each draw 1 card.

            List<SelectTurnTakerDecision> storedDraws = new List<SelectTurnTakerDecision>();
            List<TurnTaker> usedHeroes = new List<TurnTaker>();
            IEnumerator coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, optionalDrawCard: false, storedResults: storedDraws, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectTurnTaker(storedDraws))
            {
                usedHeroes.Add(GetSelectedTurnTaker(storedDraws));
            }
            if (FindActiveHeroTurnTakerControllers().Any((HeroTurnTakerController httc) => !usedHeroes.Contains(httc.TurnTaker) && GameController.IsTurnTakerVisibleToCardSource(httc.TurnTaker, GetCardSource())))
            {
                storedDraws = new List<SelectTurnTakerDecision>();
                coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, optionalDrawCard: false, storedResults: storedDraws, additionalCriteria: new LinqTurnTakerCriteria(tt => !usedHeroes.Contains(tt)), cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectTurnTaker(storedDraws))
                {
                    usedHeroes.Add(GetSelectedTurnTaker(storedDraws));
                }
                if (FindActiveHeroTurnTakerControllers().Any((HeroTurnTakerController httc) => !usedHeroes.Contains(httc.TurnTaker) && GameController.IsTurnTakerVisibleToCardSource(httc.TurnTaker, GetCardSource())))
                {
                    storedDraws = new List<SelectTurnTakerDecision>();
                    coroutine = GameController.SelectHeroToDrawCard(DecisionMaker, optionalDrawCard: false, storedResults: storedDraws, additionalCriteria: new LinqTurnTakerCriteria(tt => !usedHeroes.Contains(tt)), cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            yield break;
        }
    }
}

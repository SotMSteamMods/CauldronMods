using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class AcidBreathCardController : CardController
    {
        #region Constructors

        public AcidBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResultsAction = new List<DestroyCardAction>();
            //Destroy 2 hero ongoing cards and 2 hero equipment cards.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsOngoing && c.IsHero), new int?(2), false, null, null, storedResultsAction, null, false, null, null, null, this.GetCardSource(null));
            IEnumerator coroutine2 = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.DoKeywordsContain("equipment") && c.IsHero), new int?(2), false, null, null, storedResultsAction, null, false, null, null, null, this.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }

            //The Head with the highest HP deals {H} toxic damage to each hero who did not destroy a card this way.
            List<Card> characterCardsWithDestroyed = new List<Card>();
            if(storedResultsAction.Count<DestroyCardAction>() > 0)
            {
                using (List<DestroyCardAction>.Enumerator enumerator = storedResultsAction.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        DestroyCardAction destroy = enumerator.Current;
                        characterCardsWithDestroyed.Add(destroy.CardSource.HeroTurnTakerController.CharacterCard);
                    }
                }
            }
            coroutine = base.DealDamage(null, (Card c) => !characterCardsWithDestroyed.Contains(c) && c.IsHeroCharacterCard, base.H, DamageType.Toxic, false, false, null, null, null, false, null, new TargetInfo(HighestLowestHP.HighestHP, 1, 1, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "the head with the highest HP", true, false, null, null, false)), false, false);
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

        #endregion Methods
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class AmphibiousAssaultCardController : TheWanderingIsleCardController
    {
        public AmphibiousAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP(numberOfTargets: Game.H - 1);
            SpecialStringMaker.ShowIfElseSpecialString(() => WasHeroCardPlayedThisRound(), () => "A hero card has been played this round.", () => "A hero card has not been played this round.");
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, if any hero cards were played this round, play the top card of the villain deck. Then, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.StartOfTurnResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.DestroySelf });
        }

        public override IEnumerator Play()
        {
            // When this card enters play, the { H - 1} villain targets with the lowest HP each deal 3 lightning damage to a different hero target.
            //Find the villain targets with the lowest HP
            List<Card> damageDealers = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetsWithLowestHitPoints(1, Game.H - 1, (Card c) => IsVillainTarget(c), damageDealers, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //coroutine = MultipleDamageSourcesDealDamage(new LinqCardCriteria((Card c) => damageDealers.Contains(c), "H-1 villain targets with the lowest HP", useCardsSuffix: false), TargetType.SelectTarget, 1, new LinqCardCriteria((Card c) => IsHeroTarget(c), "hero target"), 3, DamageType.Lightning);
            //if (base.UseUnityCoroutines)
            //{
            //	yield return base.GameController.StartCoroutine(coroutine);
            //}
            //else
            //{
            //	base.GameController.ExhaustCoroutine(coroutine);
            //}
            //hero targets so we can exclude from the selection
            List<Card> heroTargets = new List<Card>();
			List<Card> usedSources = new List<Card>();
            //while there are villian targets to deal damage, and hero targets to recieve damage
            //for (int index = 0; index < damageDealers.Count; index++)
            while (damageDealers.Count() > 0)
            {
				IEnumerable<Card> source = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && damageDealers.Contains(c) && !usedSources.Contains(c));
				if (source.Count() == 0)
				{
					break;
				}
				Card villainSource = damageDealers.First();
				if (damageDealers.Count() > 1)
				{					
					List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
					coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, source, storedResults, optional: false, allowAutoDecide: true, cardSource: GetCardSource());
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(coroutine);
					}
					else
					{
						GameController.ExhaustCoroutine(coroutine);
					}
					SelectCardDecision selectCardDecision = storedResults.FirstOrDefault();
					if (selectCardDecision != null)
					{ 
						villainSource = GetSelectedCard(storedResults);
					}
				} 


				usedSources.Add(villainSource);

                List<SelectCardDecision> selectCards = new List<SelectCardDecision>();

                coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, villainSource), 3, DamageType.Lightning, 1, false, 1,
                    additionalCriteria: c => IsHeroTarget(c) && c.IsInPlayAndHasGameText && !heroTargets.Contains(c),
                    storedResultsDecisions: selectCards,
                    cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                heroTargets.Add(GetSelectedCard(selectCards));
               
            }


            yield break;
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            if (WasHeroCardPlayedThisRound())
            {
                //play the top card of the villain deck.
                IEnumerator play = base.PlayTheTopCardOfTheVillainDeckResponse(pca);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(play);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(play);
                }
            }

            //Then, destroy this card
            IEnumerator destroy = this.DestroyThisCardResponse(pca);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(destroy);
            }
            else
            {
                this.GameController.ExhaustCoroutine(destroy);
            }

            yield break;
        }

        /// <summary>
        /// Determine if a hero card was played this round
        /// </summary>
        /// <returns>Whether a hero card was played this round</returns>
        private bool WasHeroCardPlayedThisRound()
        {
            return base.GameController.Game.Journal.PlayCardEntries()
                            .Any(e => e.Round == this.Game.Round && !e.IsPutIntoPlay && IsHero(e.CardPlayed));
        }

	}
}

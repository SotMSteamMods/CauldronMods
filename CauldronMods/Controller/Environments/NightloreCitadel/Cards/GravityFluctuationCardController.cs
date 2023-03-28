using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class GravityFluctuationCardController : NightloreCitadelUtilityCardController
    {
        public GravityFluctuationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildHeroesWithMoreThan3CardsInHandString());
        }

        private string BuildHeroesWithMoreThan3CardsInHandString()
        {
            IEnumerable<TurnTaker> source = FindTurnTakersWhere(tt => IsHero(tt) && tt.ToHero().NumberOfCardsInHand > 3);
            int num = source.Count();
            string heroSpecial = "";
            if (num > 0)
            {
                heroSpecial += "There " + num + " heroes with more than 3 cards in hand: ";
                heroSpecial += String.Join(", ", source.Select(tt => tt.Name).ToArray());
            } else
            {
                heroSpecial += "There are no heroes with more than 3 cards in hand.";
            }

            return heroSpecial;

        }

        public override void AddTriggers()
        {
            //Reduce all damage dealt by 1.
            AddReduceDamageTrigger((Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);
            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            // When this card enters play, it deals each hero with more than 3 cards in their hand 2 irreducible melee damage. 
            IEnumerable<HeroTurnTakerController> heroList = FindActiveHeroTurnTakerControllers().Where(httc => httc.HeroTurnTaker.NumberOfCardsInHand > 3);
            IEnumerable<Card> characters = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget &&  IsHeroCharacterCard(c) && heroList.Select(httc => httc.TurnTaker).Contains(c.Owner));
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = DealDamage(base.Card, (Card c) => characters.Contains(c), 2, DamageType.Melee, isIrreducible: true, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> damagedHeroes = new List<Card>();
            if(storedResults.Any())
            {
                foreach(DealDamageAction dd in storedResults)
                {
                    if(dd.DidDealDamage)
                    {
                        damagedHeroes.Add(dd.Target);

                    }
                }
            }
            //One hero that was dealt no damage this way may deal 1 target 3 melee damage.

            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroToDealDamage, new LinqCardCriteria(c =>  IsHeroCharacterCard(c) &&  !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndHasGameText && !damagedHeroes.Contains(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults: storedDecision, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedDecision))
            {
                Card selectedHero = GetSelectedCard(storedDecision);
                HeroTurnTakerController selectedHeroOwner = FindHeroTurnTakerController(selectedHero.Owner.ToHero());
                coroutine = GameController.SelectTargetsAndDealDamage(selectedHeroOwner, new DamageSource(GameController, selectedHero), 3, DamageType.Melee, 1, false, 0, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}

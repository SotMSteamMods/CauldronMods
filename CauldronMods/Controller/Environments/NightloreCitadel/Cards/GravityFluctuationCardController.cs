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
            IEnumerator coroutine;
            Card card;
            List<Card> damagedHeroes = new List<Card>();
            foreach (HeroTurnTakerController httc in heroList)
            {
                if (!Card.IsInPlayAndHasGameText)
                {
                    yield break;
                }
                List<Card> storedCharacter = new List<Card>();
                coroutine = FindCharacterCardToTakeDamage(httc.HeroTurnTaker, storedCharacter, base.CharacterCard, 2, DamageType.Melee, isIrreducible: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                card = storedCharacter.FirstOrDefault();
                if (card != null)
                {
                    List<DealDamageAction> storedDamage = new List<DealDamageAction>();
                    coroutine = DealDamage(Card, card, 2, DamageType.Melee, isIrreducible: true, storedResults: storedDamage, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (storedDamage != null && storedDamage.Any() && storedDamage.FirstOrDefault().DidDealDamage)
                    {
                        damagedHeroes.Add(storedDamage.FirstOrDefault().Target);
                    }
                }
            }

            //One hero that was dealt no damage this way may deal 1 target 3 melee damage.

            List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroToDealDamage, new LinqCardCriteria(c => c.IsHeroCharacterCard && !damagedHeroes.Contains(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults: storedDecision, false, cardSource: GetCardSource());
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

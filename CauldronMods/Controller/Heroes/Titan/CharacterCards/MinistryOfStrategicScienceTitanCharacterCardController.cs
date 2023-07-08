using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class MinistryOfStrategicScienceTitanCharacterCardController : TitanBaseCharacterCardController
    {
        public MinistryOfStrategicScienceTitanCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private List<Card> actedHeroes;

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //Each hero may deal themselves 1 psychic damage to draw a card.
                        this.actedHeroes = new List<Card>();
                        IEnumerable<Function> functionsBasedOnCard(Card c) => new Function[]
                        {
                            new Function(base.FindCardController(c).DecisionMaker, "Deal self 1 psychic damage to draw a card now.", SelectionType.DrawCard, () => this.DealDamageAndDrawResponse(c))
                        };
                        coroutine = base.GameController.SelectCardsAndPerformFunction(this.DecisionMaker, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && !this.actedHeroes.Contains(c), "active hero character cards", false, false, null, null, false), functionsBasedOnCard, true, base.GetCardSource(null));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //Destroy an environment card.
                        coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment), false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;

                    }
                case 2:
                    {
                        //Prevent the next damage that would be dealt to a hero target.
                        CannotDealDamageStatusEffect statusEffect = new CannotDealDamageStatusEffect()
                        {
                            TargetCriteria = { IsHero = true },
                            NumberOfUses = 1
                        };
                        coroutine = base.AddStatusEffect(statusEffect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
        }

        private IEnumerator DealDamageAndDrawResponse(Card card)
        {
            if (card != null)
            {
                IEnumerator coroutine = base.DealDamage(card, card, 1, DamageType.Psychic, cardSource: base.GetCardSource());
                IEnumerator coroutine2 = base.DrawCard(card.Owner.ToHero());
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
                this.LogActedCard(card);
            }
            yield break;
        }

        private void LogActedCard(Card card)
        {
            if (card.SharedIdentifier != null)
            {
                IEnumerable<Card> collection = base.FindCardsWhere((Card c) => c.SharedIdentifier != null && c.SharedIdentifier == card.SharedIdentifier && c != card, false, null, false);
                this.actedHeroes.AddRange(collection);

            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //You may play an ongoing card.
            IEnumerator coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardCriteria: new LinqCardCriteria((Card c) => IsOngoing(c)), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Draw a card.
            coroutine = base.DrawCard();
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
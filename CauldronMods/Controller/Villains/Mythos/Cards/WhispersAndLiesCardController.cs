using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class WhispersAndLiesCardController : MythosUtilityCardController
    {
        public WhispersAndLiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddAsPowerContributor();
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowVillainTargetWithLowestHP();
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, the villain target with the lowest HP deals the hero target with the highest HP 2 sonic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...the villain target with the lowest HP deals the hero target with the highest HP 2 sonic damage.
            IEnumerable<Card> lowestVillain = base.FindAllTargetsWithLowestHitPoints((Card c) => base.IsVillainTarget(c), 1);
            if (lowestVillain.Any())
            {
                Card damageSource = lowestVillain.FirstOrDefault();
                IEnumerator coroutine;
                if (lowestVillain.Count() > 1)
                {
                    List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
                    coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => lowestVillain.Contains(c)), cardDecisions, false, cardSource: base.GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    damageSource = cardDecisions.FirstOrDefault().SelectedCard;
                }

                coroutine = base.DealDamageToHighestHP(damageSource, 1, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Sonic);
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

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            //{MythosMadness} Heroes gain the following power:
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier) && cardController.HeroTurnTakerController != null && cardController.Card.IsHeroCharacterCard && cardController.Card.Owner.IsHero && !cardController.Card.Owner.ToHero().IsIncapacitatedOrOutOfGame && !cardController.Card.IsFlipped && cardController.Card.IsRealCard)
            {
                //Power: Shuffle 2 cards from the villain trash into the villain deck.
                Power power = new Power(cardController.HeroTurnTakerController, cardController, "Shuffle 2 cards from the villain trash into the villain deck.", this.ShuffleVillainCardsResponse(cardController), 0, null, base.GetCardSource());
                return new Power[]
                {
                    power
                };
            }
            return null;
        }

        private IEnumerator ShuffleVillainCardsResponse(CardController cardController)
        {
            IEnumerable<MoveCardDestination> cardDestinations = new MoveCardDestination(base.TurnTaker.Deck).ToEnumerable();
            //Shuffle 2 cards from the villain trash into the villain deck.
            IEnumerator coroutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, base.TurnTaker.Trash, 2, 2, new LinqCardCriteria((Card c) => c.Location.IsTrash && c.Location.IsVillain), cardDestinations,  cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
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

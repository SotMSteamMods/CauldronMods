using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightCharacterCardController : HeroCharacterCardController
    {
        public StarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Draw a card, or play a Constellation from your trash
            IEnumerator coroutine = DrawACardOrPlayConstellationFromTrash();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"Until the start of your next turn, prevent all damage that would be dealt to or by the target with the lowest HP.",
                        OnDealDamageStatusEffect lowestTargetImmunity = new OnDealDamageStatusEffect(Card, "LowestTargetImmunity", "The target with the lowest HP is immune to damage and cannot deal damage.", new TriggerType[] { TriggerType.MakeImmuneToDamage }, TurnTaker, Card);
                        lowestTargetImmunity.UntilStartOfNextTurn(TurnTaker);
                        lowestTargetImmunity.CardSource = Card;
                        lowestTargetImmunity.SourceCriteria.IsTarget = true;
                        lowestTargetImmunity.BeforeOrAfter = BeforeOrAfter.Before;
                        IEnumerator coroutine = AddStatusEffect(lowestTargetImmunity);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"1 player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        IEnumerator coroutine3 = GameController.SelectAndGainHP(HeroTurnTakerController, 2, optional: false, (Card c) => c.IsInPlay && c.IsHero && c.IsTarget, 1, null, allowAutoDecide: false, null, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }


            yield break;
        }

        private IEnumerator DrawACardOrPlayConstellationFromTrash()
        {
            List<Function> list = new List<Function>();
            string forceDrawCardEnder = " cannot play a constellations from their trash, so they must draw a card.";
            string forcePlayConstellationEnder = " cannot draw any cards, so they must play a constellation from their trash.";
            string forceDoNothingEnder = " cannot draw nor play any cards, so the power has no effect.";
            string heroName = ((TurnTaker == null) ? Card.Title : TurnTaker.Name);

            list.Add(new Function(HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker, false), HeroTurnTakerController != null && CanDrawCards(HeroTurnTakerController), heroName + forceDrawCardEnder));

            list.Add(new Function(HeroTurnTakerController, "Play a constellation from your trash", SelectionType.PlayCard, () => SelectAndPlayConstellationFromTrash(HeroTurnTakerController), HeroTurnTakerController != null && GetPlayableConstellationsInTrash().Count() > 0, heroName + forcePlayConstellationEnder));

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, HeroTurnTakerController, list, false, null, heroName + forceDoNothingEnder, null, GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator SelectAndPlayConstellationFromTrash(HeroTurnTakerController hero)
        {
            IEnumerator coroutine = GameController.SelectAndPlayCard(hero, GetPlayableConstellationsInTrash(), cardSource: GetCardSource());
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

        private IEnumerable<Card> GetPlayableConstellationsInTrash()
        {
            return HeroTurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), false, null, false, true) == CanPlayCardResult.CanPlay);
        }

        private bool IsConstellation(Card card)
        {
            if (card != null)
            {
                return GameController.DoesCardContainKeyword(card, "constellation");
            }
            return false;
        }

        public IEnumerator LowestTargetImmunity(DealDamageAction dealDamage, HeroTurnTaker hero = null, StatusEffect effect = null, int[] powerNumerals = null)
        {
            List<bool> storedResults = new List<bool>();

            //Is the target of the damage the lowest HP target?
            IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.Target, highest: false, (Card card) => GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //If not, is the source of the damage the lowest HP target?
            if (!storedResults.First() && dealDamage.DamageSource.IsTarget)
            {
                IEnumerator coroutine2 = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.DamageSource.Card, highest: false, (Card card) => GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine2);
                }
            }

            //If we answered yes to either question, prevent the damage.
            if (storedResults.Contains(true))
            {
                IEnumerator coroutine3 = CancelAction(dealDamage, showOutput: true, cancelFutureRelatedDecisions: true, null, isPreventEffect: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine3);
                }
            }

            yield break;
        }
    }
}
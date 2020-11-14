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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"Until the start of your next turn, prevent all damage that would be dealt to or by the target with the lowest HP.",
                        OnDealDamageStatusEffect lowestTargetImmunity = new OnDealDamageStatusEffect(base.Card, "LowestTargetImmunity", "The target with the lowest HP is immune to damage and cannot deal damage.", new TriggerType[1] { TriggerType.MakeImmuneToDamage }, DecisionMaker.TurnTaker, base.Card);
                        lowestTargetImmunity.UntilStartOfNextTurn(base.TurnTaker);
                        lowestTargetImmunity.SourceCriteria.IsTarget = true;
                        lowestTargetImmunity.BeforeOrAfter = BeforeOrAfter.Before;
                        IEnumerator coroutine = AddStatusEffect(lowestTargetImmunity);
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
                        //"1 player may use a power now.",
                        IEnumerator coroutine2 = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        IEnumerator coroutine3 = base.GameController.SelectAndGainHP(DecisionMaker, 2, optional: false, (Card c) => c.IsInPlay && c.IsHero && c.IsTarget, 1, null, allowAutoDecide: false, null, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }


            yield break;
        }

        private IEnumerator DrawACardOrPlayConstellationFromTrash()
        {
            List<Function> list = new List<Function>();
            string str = "so they must draw a card.";
            string str2 = "so they must play a constellation from their trash.";
            string str3 = ", so the power has no effect.";
            string str4 = ((DecisionMaker == null) ? Card.Title : DecisionMaker.TurnTaker.Name);
            if ((DecisionMaker != null && DecisionMaker.TurnTaker.Identifier == "Guise") || Card.Identifier == "GuiseCharacter")
            {
                str = "so he's gotta draw one. Woo! Free card!";
                str2 = "so he's gotta play one. Make sure it's a good one!";
                str3 = ". Bummer!";
            }
            list.Add(new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(DecisionMaker.HeroTurnTaker, false), DecisionMaker != null && CanDrawCards(DecisionMaker), str4 + " cannot play any cards, " + str));
            
            list.Add(new Function(DecisionMaker, "Play a constellation from your trash", SelectionType.PlayCard, () => SelectAndPlayConstellationFromTrash(DecisionMaker, false), DecisionMaker != null && GetPlayableConstellationsInTrash().Count() > 0, str4 + " cannot draw any cards, " + str2));

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, null, str4 + " cannot draw nor play any cards" + str3, null, GetCardSource());
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

        private IEnumerator SelectAndPlayConstellationFromTrash(HeroTurnTakerController hero, bool optional)
        {
            IEnumerator coroutine = GameController.SelectAndPlayCard(hero, GetPlayableConstellationsInTrash());
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
            return DecisionMaker.HeroTurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), false, null, false, true) == CanPlayCardResult.CanPlay);  
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
            IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.Target, highest: false, (Card card) => base.GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If not, is the source of the damage the lowest HP target?
            if (!storedResults.First() && dealDamage.DamageSource.IsTarget)
            {
                IEnumerator coroutine2 = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dealDamage.DamageSource.Card, highest: false, (Card card) => base.GameController.IsCardVisibleToCardSource(card, GetCardSource()), dealDamage, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            //If we answered yes to either question, prevent the damage.
            if (storedResults.Contains(true))
            {

                IEnumerator coroutine3 = CancelAction(dealDamage, showOutput: true, cancelFutureRelatedDecisions: true, null, isPreventEffect: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
            }

            yield break;
        }
    }
}
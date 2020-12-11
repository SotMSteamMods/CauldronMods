using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class FirstResponseCypherCharacterCardController : CypherBaseCharacterCardController
    {
        private const int PowerTargetAmount = 1;
        private const int Incapacitate3DamageIncrease = 1;

        public FirstResponseCypherCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            // {Cypher} deals 1 target X energy damage, where X is the number of augments in play.

            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int targetsNumeral = base.GetPowerNumeral(0, PowerTargetAmount);
            int damageNumeral = base.GetPowerNumeral(1, GetAugmentsInPlay().Count);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                damageNumeral, DamageType.Energy, targetsNumeral, false, targetsNumeral,
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator routine;
            switch (index)
            {
                case 0:

                    // One player may draw a card now.
                    routine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;

                case 1:

                    // Look at the bottom card of each deck and replace or discard each one.
                    routine = base.DoActionToEachTurnTakerInTurnOrder(criteria => true, this.EachTurnTakerResponse,
                        base.TurnTaker);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;

                case 2:

                    // Increase the next damage dealt by a hero target by 1.
                    IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(Incapacitate3DamageIncrease)
                    {
                        NumberOfUses = 1,
                        SourceCriteria = { IsHero = true }
                    };
                    routine = base.AddStatusEffect(statusEffect);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;
            }
        }

        private IEnumerator EachTurnTakerResponse(TurnTakerController turnTakerController)
        {
            // Look at the bottom card of each deck and replace or discard each one.
            List<Card> revealedCards = new List<Card>();
            TurnTaker turnTaker = turnTakerController.TurnTaker;

            IEnumerator routine = base.GameController.RevealCards(turnTakerController, turnTaker.Deck, 1, revealedCards, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            Card revealedCard = revealedCards.FirstOrDefault();
            if (revealedCard == null)
            {
                yield break;
            }

            routine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, revealedCard, new[]
            {
                new MoveCardDestination(turnTaker.Deck, true),
                new MoveCardDestination(turnTaker.Trash)
            }, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}
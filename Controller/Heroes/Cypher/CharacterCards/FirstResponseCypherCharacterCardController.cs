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
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsAugment(c), "augment"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // {Cypher} deals 1 target X energy damage, where X is the number of augments in play.
            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int targetsNumeral = base.GetPowerNumeral(0, PowerTargetAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, damageSource, c => GetAugmentsInPlay().Count,
                DamageType.Energy, () => targetsNumeral, false, targetsNumeral,
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
            switch (index)
            {
                case 0:
                    {
                        // One player may draw a card now.
                        var routine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }
                    }
                    break;

                case 1:
                    {
                        // Look at the bottom card of each deck and replace or discard each one.
                        var routine = base.DoActionToEachTurnTakerInTurnOrder(tt => !tt.IsIncapacitatedOrOutOfGame, this.EachTurnTakerResponse, base.TurnTaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }
                    }
                    break;
                case 2:
                    {
                        // Increase the next damage dealt by a hero target by 1.
                        IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(Incapacitate3DamageIncrease);
                        statusEffect.NumberOfUses = 1;
                        statusEffect.SourceCriteria.IsTarget = true;
                        statusEffect.SourceCriteria.IsHero = true;
                        statusEffect.CardSource = CharacterCard;

                        var routine = base.AddStatusEffect(statusEffect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(routine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(routine);
                        }
                    }
                    break;
            }
        }

        private IEnumerator EachTurnTakerResponse(TurnTakerController turnTakerController)
        {
            // Look at the bottom card of each deck and replace or discard each one.
            List<Card> revealedCards = new List<Card>();
            TurnTaker turnTaker = turnTakerController.TurnTaker;

            foreach (var deck in turnTaker.Decks)
            {
                var trash = FindTrashFromDeck(deck);

                IEnumerator routine = base.GameController.RevealCards(turnTakerController, deck, 1, revealedCards, true,
                                        cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                Card revealedCard = revealedCards.FirstOrDefault();
                if (revealedCard != null)
                {
                    var destinations = new[]
                    {
                        new MoveCardDestination(deck, true),
                        new MoveCardDestination(trash)
                    };

                    routine = base.GameController.SelectLocationAndMoveCard(DecisionMaker, revealedCard, destinations, cardSource: base.GetCardSource());
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
    }
}
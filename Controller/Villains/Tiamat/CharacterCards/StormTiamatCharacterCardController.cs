using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    class StormTiamatCharacterCardController : TiamatCharacterCardController
    {
        public StormTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfLightning", "element of lightning")).Condition = () => base.Card.IsFlipped; ;
            base.SpecialStringMaker.ShowDamageDealt(new LinqCardCriteria((Card c) => c == base.Card, base.Card.Title, useCardsSuffix: false), thisTurn: true).Condition = () => Game.ActiveTurnTaker == base.TurnTaker && !base.Card.IsFlipped;

        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Mouth of the Storm is immune to Lightning damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Lightning, false),
				//At the end of the villain turn, if {Tiamat}, The Mouth of the Storm dealt no damage this turn, she deals the hero target with the highest HP {H - 2} Lightning damage.
				base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, (PhaseChangeAction p) => this.DidDealDamageThisTurn())
                //ElementOfLightning: The hero with the most cards in hand may not draw cards until the start of the next villain turn.
        };
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
				//Increase damage dealt by {Tiamat}, The Mouth of the Storm by 1.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.IsCard && dealDamage.DamageSource.Card == base.Card, 1)
            };
        }

        protected override ITrigger[] AddDecapitatedTriggers()
        {
            return new ITrigger[]
            {
				//When a spell card causes a head to deal damage, increase that damage by 1 for each “Element of Lightning“ card in the villain trash.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.CardSource != null && dealDamage.DamageSource != null &&  IsSpell(dealDamage.CardSource.Card) && IsHead(dealDamage.DamageSource.Card), GetNumberOfElementOfLightningInTrash())
                //ElementOfLightning: The hero with the most cards in hand may not draw cards until the start of the next villain turn.
            };
        }

        protected override ITrigger[] AddDecapitatedAdvancedTriggers()
        {
            return new ITrigger[]
            {
				//Reduce damage dealt by heads by 1.
				base.AddReduceDamageTrigger((Card c) => IsHead(c),1)
            };
        }

        //Deal H-2 Lightning damage to highest hero target
        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsTarget && c.IsHero, (Card c) => new int?(base.H - 2), DamageType.Lightning);
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

        //Get number of "Element of Lightning" cards in trash
        private int GetNumberOfElementOfLightningInTrash()
        {
            return (from card in base.TurnTaker.Trash.Cards
                    where card.Identifier == "ElementOfLightning"
                    select card).Count<Card>();
        }
    }
}

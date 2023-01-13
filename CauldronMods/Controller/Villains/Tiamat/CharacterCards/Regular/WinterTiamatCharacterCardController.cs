using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cauldron.Drift;
using Cauldron.Starlight;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class WinterTiamatCharacterCardController : TiamatCharacterCardController
    {
        public WinterTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => !base.Card.IsFlipped;
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfIce", "element of ice")).Condition = () => base.Card.IsFlipped;
            base.SpecialStringMaker.ShowDamageDealt(new LinqCardCriteria((Card c) => c == base.Card, base.Card.Title, useCardsSuffix: false), thisTurn: true).Condition = () => Game.ActiveTurnTaker == base.TurnTaker && !base.Card.IsFlipped;
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is TiamatTurnTakerController tttc && !tttc.ArePromosSetup)
            {
                tttc.SetupPromos(tttc.availablePromos);
                tttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Jaws of Winter is immune to Cold damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Cold, false),
				//At the end of the villain turn, if {Tiamat}, The Jaws of Winter dealt no damage this turn, she deals the hero target with the highest HP {H - 2} Cold damage. 
				base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, (PhaseChangeAction p) => !this.DidDealDamageThisTurn())
            };
        }

        public override void AddStartOfGameTriggers()
        {
            base.AddStartOfGameTriggers();
            AddTrigger((GameAction ga) => TurnTakerController is TiamatTurnTakerController tttc && !tttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        protected override ITrigger[] AddFrontAdvancedTriggers()
        {
            return new ITrigger[]
            {
				//Increase damage dealt by {Tiamat}, The Jaws of Winter by 1.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.Card != null && dealDamage.DamageSource.IsCard && dealDamage.DamageSource.Card == base.Card, 1)
            };
        }

        protected override ITrigger[] AddDecapitatedAdvancedTriggers()
        {
            return new ITrigger[]
            {
				//Reduce damage dealt by heads by 1.
				base.AddReduceDamageTrigger((Card c) => IsHead(c), 1)
            };
        }
        protected override ITrigger[] AddDecapitatedChallengeTriggers()
        {
            return new ITrigger[]
            {
				//"Whenever a villain Spell card enters play, if the head it names is decapitated, flip that head and restore it to {H * 3} HP.",
				AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.IsVillain && cep.CardEnteringPlay.Identifier == "ElementOfIce", ChallengeRestoreHeadResponse, TriggerType.FlipCard, TriggerTiming.After)
            };
        }

        protected override ITrigger[] AddDecapitatedTriggers()
        {
            return new ITrigger[]
            {
				//When a spell card causes a head to deal damage, increase that damage by 1 for each “Element of Cold“ card in the villain trash.
				base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.Card != null && dealDamage.CardSource != null && IsSpell(dealDamage.CardSource.Card) && IsHead(dealDamage.DamageSource.Card), GetNumberOfElementOfIceInTrash())
            };
        }

        //Deal H-2 Cold damage to highest hero target
        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsTarget && IsHero(c), (Card c) => new int?(base.H - 2), DamageType.Cold);
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

        //Get number of "Element of Ice" cards in trash
        private int GetNumberOfElementOfIceInTrash()
        {
            return GetNumberOfSpecificCardInTrash("ElementOfIce");
        }
    }
}

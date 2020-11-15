using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementalFormCardController : CardController
    {
        public ElementalFormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a Head takes damage, it becomes immune to damage of that type until the start of the next villain turn.
            CreateTriggers(base.TurnTaker.FindCard("InfernoTiamatCharacter"));
            CreateTriggers(base.TurnTaker.FindCard("StormTiamatCharacter"));
            CreateTriggers(base.TurnTaker.FindCard("WinterTiamatCharacter"));
            if (!(base.CharacterCardController is WinterTiamatCharacterCardController))
            {
                CreateTriggers(base.TurnTaker.FindCard("EarthTiamatCharacter"));
                CreateTriggers(base.TurnTaker.FindCard("DecayTiamatCharacter"));
                CreateTriggers(base.TurnTaker.FindCard("WindTiamatCharacter"));
            }
        }

        private void CreateTriggers(Card characterCard)
        {
            Func<DealDamageAction, bool> criteria = (DealDamageAction dealDamage) => characterCard.IsInPlayAndHasGameText && dealDamage.Target == characterCard && dealDamage.DidDealDamage;
            base.AddTrigger<DealDamageAction>(criteria, new Func<DealDamageAction, IEnumerator>(this.ImmuneResponse), TriggerType.ImmuneToDamage, TriggerTiming.After, ActionDescription.DamageTaken);
            base.AddTrigger<DealDamageAction>(criteria, new Func<DealDamageAction, IEnumerator>(this.WarningMessageResponse), TriggerType.Hidden, TriggerTiming.After);
        }

        private IEnumerator WarningMessageResponse(DealDamageAction dealDamage)
        {
            if (base.IsFirstOrOnlyCopyOfThisCardInPlay())
            {
                IEnumerator coroutine = base.GameController.SendMessageAction("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage.", Priority.High, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                Log.Debug("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage. ");
            }
            yield return null;
            yield break;
        }

        private IEnumerator ImmuneResponse(DealDamageAction dealDamage)
        {
            DamageType value = dealDamage.DamageType;
            //this.GetDamageTypeThatHeadIsImmuneTo(dealDamage.Target) ?? default;
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.DamageTypeCriteria.AddType(value);
            immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = dealDamage.Target;
            immuneToDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            immuneToDamageStatusEffect.CardDestroyedExpiryCriteria.Card = dealDamage.Target;
            IEnumerator coroutine2 = base.AddStatusEffect(immuneToDamageStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}
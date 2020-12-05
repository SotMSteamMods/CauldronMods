using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Quicksilver
{
    public class MalleableArmorCardController : CardController
    {
        public MalleableArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.CharacterCard && ddje.Amount > 0).Any() ? "Quicksilver has dealt damage this turn." : "Quicksilver has not dealt damage this turn.");
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"If {Quicksilver} has not dealt damage this turn, she regains 3HP."

            int regains = GetPowerNumeral(0, 3);

            bool hasDealtDamage = Journal.DealDamageEntriesThisTurn().Where((DealDamageJournalEntry ddje) => ddje.SourceCard == this.CharacterCard && ddje.Amount > 0).Any();
            IEnumerator coroutine;
            if (hasDealtDamage)
            {
                coroutine = GameController.SendMessageAction($"{this.CharacterCard.Title} has dealt damage this turn, and does not regain HP.", Priority.High, GetCardSource());
            }
            else
            {
                coroutine = GameController.GainHP(this.CharacterCard, regains, cardSource: GetCardSource());
            }

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
        public override void AddTriggers()
        {
            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP...
            base.AddTrigger<DealDamageAction>(delegate (DealDamageAction action)
            {
                if (action.Target == base.CharacterCard && base.CharacterCard.HitPoints > 1)
                {
                    int amount = action.Amount;
                    int? hitPoints = action.Target.HitPoints;
                    return amount >= hitPoints.GetValueOrDefault() & hitPoints != null;
                }
                return false;
            }, new Func<DealDamageAction, IEnumerator>(this.DealDamageResponse), new TriggerType[]
            {
                TriggerType.WouldBeDealtDamage,
                TriggerType.GainHP
            }, TriggerTiming.Before);
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //I think this has the possibility to interact incorrectly with some esoteric effects.
            //Not sure of a better way to do it, though.
            //...restore her to 1HP.
            base.CharacterCard.SetHitPoints(action.Amount + 1);
            yield break;
        }
    }
}
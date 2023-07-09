using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace Cauldron.TheRam
{
    public class ForcefieldNodeCardController : TheRamUtilityCardController
    {
        public ForcefieldNodeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers();
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are not Up Close.",
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsHero && dd.DamageSource.IsCard && !IsUpClose(dd.DamageSource.Card));
            //"Reduce damage dealt to {TheRam} by 2.",
            AddReduceDamageTrigger((Card c) => c == GetRam, 2);
            //"Whenever a copy of Up Close enters play next to a hero, this card deals that hero {H - 2} energy damage."
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.Identifier == "UpClose",
                        DamageOnUpCloseEntryResponse,
                        new TriggerType[] { TriggerType.DealDamage },
                        TriggerTiming.After);
        }

        private IEnumerator DamageOnUpCloseEntryResponse(CardEntersPlayAction cep)
        {
            Card upClose = cep.CardEnteringPlay;
            if (upClose.Location.IsNextToCard && upClose.Location.OwnerCard != null && upClose.Location.OwnerCard.IsTarget && IsHero(upClose.Location.OwnerCard))
            {
                IEnumerator damage = DealDamage(this.Card, upClose.Location.OwnerCard, H - 2, DamageType.Energy);
                if (base.UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(damage);
                }
                else
                {
                    GameController.ExhaustCoroutine(damage);
                }
            }
            yield break;
        }
    }
}
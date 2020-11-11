using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class FarsightCardController : TangoOneBaseCardController
    {

        //==============================================================
        // Damage dealt by {TangoOne} Is irreducible.
        //==============================================================

        public static string Identifier = "Farsight";

        public FarsightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {

            base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.Card.Owner.CharacterCard));

            /*
            this.AddTrigger<MakeDamageIrreducibleAction>((Func<MakeDamageIrreducibleAction, bool>)
                (mdia => mdia.CardSource != null),
                (Func<MakeDamageIrreducibleAction, IEnumerator>)(hp
                    => this.GameController.MakeDamageIrreducible(hp, this.GetCardSource())),
                (IEnumerable<TriggerType>)new TriggerType[1]
                {
                    TriggerType.MakeDamageIrreducible
                }, TriggerTiming.Before);
            */
            base.AddTriggers();
        }
    }
}
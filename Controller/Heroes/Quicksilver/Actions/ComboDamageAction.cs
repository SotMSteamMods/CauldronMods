using Handelabra.Sentinels.Engine.Controller;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class ComboDamageAction : DealDamageAction
    {
        public ComboDamageAction(DealDamageAction action) : base(action)
        {
            this.dealDamagaction = action;
            dealDamagaction.DoAction();
        }

        public DealDamageAction dealDamagaction;
    }
}
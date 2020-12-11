using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class VeilShardkeyCardController : OriphelShardkeyCardController
    {
        public VeilShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Shardkey Transformation trigger
            base.AddTriggers();

            //"Whenever another villain target is destroyed, play the top card of the villain deck."
            AddTrigger((DestroyCardAction dc) => dc.WasCardDestroyed && IsVillainTarget(dc.CardToDestroy.Card) && dc.CardToDestroy != this,
                            PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard, TriggerTiming.After);
        }
    }
}
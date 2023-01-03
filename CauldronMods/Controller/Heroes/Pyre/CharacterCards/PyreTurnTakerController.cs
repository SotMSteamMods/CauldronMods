using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class PyreTurnTakerController : HeroTurnTakerController
    {
        public PyreTurnTakerController(TurnTaker tt, GameController gc):base(tt, gc)
        {
        }

        public const string Irradiated = "{Rad}";
        public string[] availablePromos = new string[] { "WastelandRoninPyre" };
        public bool ArePromosSetup { get; set; } = false;

        public void MoveMarkersToSide()
        {
            var markersInDeckOrHand = TurnTaker.GetAllCards(false).Where((Card c) => !c.IsRealCard && c.Identifier == "IrradiatedMarker" && (c.Location == TurnTaker.Deck || c.Location == HeroTurnTaker.Hand));
            if(markersInDeckOrHand.Any())
            {
                foreach(Card marker in markersInDeckOrHand)
                {
                    TurnTaker.MoveCard(marker, TurnTaker.OffToTheSide);
                }
                while(HeroTurnTaker.NumberOfCardsInHand < 4)
                {
                    HeroTurnTaker.DrawCard(null);
                }
            }
        }

        public void AddIrradiatedSpecialString(Card c)
        {
            var irradiateString = SpecialStringMaker.ShowSpecialString(() => $"{c.Title} is {Irradiated}.", relatedCards: () => new Card[] { c });
            irradiateString.ShowInEffectsList = () => c.IsInHand;
            irradiateString.Condition = () => c.IsInHand;
            irradiateString.ShowWhileIncapacitated = true;
            
        }
    }
}

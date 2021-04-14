using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.SwarmEater
{
    public abstract class AugCardController : CardController
    {
        protected AugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public virtual void AddAbsorbTriggers()
        {
            if(this.CardThatAbsorbedThis() != null)
            {
                AddAbsorbTriggers(CardThatAbsorbedThis());
            }
        }
        public virtual void AddAbsorbTriggers(Card absorbingCard)
        {

        }
        public override void AddStartOfGameTriggers()
        {
            if(CanAbsorbEffectTrigger())
            {
                RemoveAllTriggers();
                GameController.RemoveInhibitor(this);
                AddAbsorbTriggers();
            }
        }
        public Card CardThatAbsorbedThis()
        {
            Card nextTo = this.Card.Location.OwnerCard;
            if (nextTo != null && nextTo.Identifier == "AbsorbedNanites")
            {
                nextTo = base.CharacterCard;
            }
            return nextTo;
        }

        protected bool CanAbsorbEffectTrigger()
        {
            if (!Card.IsInPlay || Card.IsInPlayAndNotUnderCard)
                return false;

            var card = this.Card.Location.OwnerCard;
            if (card is null)
                return false;

            if (card.Identifier == "AbsorbedNanites")
                return true;

            if (CharacterCardController is DistributedHivemindSwarmEaterCharacterCardController && IsNanomutant(card) && card.IsInPlay)
            {
                return true;
            }
            return false;
        }

        private bool IsNanomutant(Card c)
        {
            return c.DoKeywordsContain("nanomutant");
        }
    }
}
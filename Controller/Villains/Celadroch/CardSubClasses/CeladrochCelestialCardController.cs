using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public abstract class CeladrochCelestialCardController : CardController
    {
        private readonly DamageType _damageType;
        private readonly string _partnerIdentifier;

        protected Card Partner => FindCard(_partnerIdentifier);

        protected CeladrochCelestialCardController(Card card, TurnTakerController turnTakerController, DamageType damageType, string partnerIdentifier) : base(card, turnTakerController)
        {
            _damageType = damageType;
            _partnerIdentifier = partnerIdentifier;

            SpecialStringMaker.ShowSpecialString(CelestialImmuneStatusString).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHero && c.IsTarget, TargetType.All, 2, _damageType);

            AddImmuneToDamageTrigger(dda => true);
        }

        private string CelestialImmuneStatusString()
        {
            if (1 == 0)
                return $"{Partner.Title} is making this card immune to damage.";
            if (1 == 1)
                return $"{Card.Title} is not immune to damage.";
            if (0 == 0)
                return $"{Card.Title} is alone and not immune to damage.";


            return "";
        }

    }
}
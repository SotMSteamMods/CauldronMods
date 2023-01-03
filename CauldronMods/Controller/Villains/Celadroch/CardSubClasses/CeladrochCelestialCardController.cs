using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public abstract class CeladrochCelestialCardController : CardController
    {
        /*  TatteredDevil
         * 	"At the end of the villain turn, this card deals each hero target 2 infernal damage.",
			"When Hollow Angel is dealt damage, it becomes immune to damage until this card is dealt damage or leaves play."
         */

        /*  HollowAngel
         * 	"At the end of the villain turn, this card deals each hero target 2 radiant damage.",
			"When Tattered devil is dealt damage, it becomes immune to damage until this card is dealt damage or leaves play."
         */

        private readonly DamageType _damageType;
        private readonly string _partnerIdentifier;
        private readonly string _IsImmuneKey = "CelestialMadeImmuneKey";

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

            //when my partner receives damage make them immune to damage.
            AddTrigger<DealDamageAction>(dda => dda.DidDealDamage && !dda.DidDestroyTarget && dda.Target == Partner && !dda.Target.IsLeavingPlay,
                (dda) => { SetCardPropertyIfRealAction(Partner, _IsImmuneKey, true); return DoNothing(); },
                TriggerType.FirstTrigger,
                TriggerTiming.After
            );

            //when I receive damage, clear my partner's immunity
            AddTrigger<DealDamageAction>(dda => dda.DidDealDamage && !dda.DidDestroyTarget && dda.Target == Card && !dda.Target.IsLeavingPlay,
                (dda) => { SetCardPropertyIfRealAction(Partner, _IsImmuneKey, false); return DoNothing(); },
                TriggerType.Hidden,
                TriggerTiming.After,
                priority: TriggerPriority.High
            );

            //make my partner immune to damage
            AddImmuneToDamageTrigger(dda => dda.Target == Partner && GameController.GetCardPropertyJournalEntryBoolean(Partner, _IsImmuneKey) == true);

            //when I leave play clear both flags
            AddAfterLeavesPlayAction(() =>
            {
                SetCardPropertyIfRealAction(Partner, _IsImmuneKey, false);
                SetCardPropertyIfRealAction(Card, _IsImmuneKey, false);

                return DoNothing();
            });
        }

        private void SetCardPropertyIfRealAction(Card card, string key, bool? value)
        {
            if (IsRealAction())
            {
                GameController.AddCardPropertyJournalEntry(card, key, value);
            }
        }

        private string CelestialImmuneStatusString()
        {
            var card = Partner;
            if (card != null && card.IsInPlay)
            {
                if (GetCardPropertyJournalEntryBoolean(_IsImmuneKey) == true)
                {
                    return $"{Card.Title} is immune to damage.";
                }
                else
                {
                    return $"{Card.Title} is not immune to damage.";
                }
            }
            else
            {
                return $"{Card.Title} is alone and not immune to damage.";
            }
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineBandCardController : ScreaMachineUtilityCardController
    {
        public string BandmateIdentifier { get; }
        public string AbilityKey { get; }

        protected ScreaMachineBandCardController(Card card, TurnTakerController turnTakerController, string bandmate, string abilityKey) : base(card, turnTakerController)
        {
            BandmateIdentifier = bandmate;
            AbilityKey = abilityKey;
        }

        public Card GetBandmate()
        {
            return FindCard(BandmateIdentifier);
        }

        public bool IsBandmateInPlay
        {
            get
            {
                var bandmate = GetBandmate();
                if (bandmate == null || bandmate.IsOutOfGame)
                    return false;
                return bandmate.IsInPlay;
            }
        }

        protected abstract IEnumerator ActivateBandAbility();

        public override IEnumerator ActivateAbility(string abilityKey)
        {
            if (abilityKey == AbilityKey)
                return ActivateBandAbility();

            return base.ActivateAbility(abilityKey);
        }

        public override bool CanBeDestroyed => false;

        public override IEnumerator Play()
        {
            if (!IsBandmateInPlay)
                return TheSetListCardController.RevealTopCardOfTheSetList();
            else
                return DoNothing();
        }

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            if (gameAction is ActivateAbilityAction aaa && aaa.ActivatableAbility.AbilityKey == AbilityKey && aaa.ActivatableAbility.CardController == this && !IsBandmateInPlay)
            {
                return false;
            }
            return base.AskIfActionCanBePerformed(gameAction);
        }
    }
}

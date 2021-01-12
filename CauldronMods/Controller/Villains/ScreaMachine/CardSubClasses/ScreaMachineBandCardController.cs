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
        public ScreaMachineBandmate.Value Member { get; }
        private readonly string _memberIdentifier;
        private readonly string _memberAbilityKey;

        protected ScreaMachineBandCardController(Card card, TurnTakerController turnTakerController, ScreaMachineBandmate.Value member) : base(card, turnTakerController)
        {
            Member = member;
            _memberIdentifier = member.GetIdentifier();
            _memberAbilityKey = member.GetAbilityKey();
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => Enumerable.Empty<ScreaMachineBandmate.Value>();

        public Card GetBandmate()
        {
            return FindCard(_memberIdentifier);
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
            if (abilityKey == _memberAbilityKey)
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
            if (gameAction is ActivateAbilityAction aaa && aaa.ActivatableAbility.AbilityKey == _memberAbilityKey && aaa.ActivatableAbility.CardController == this && !IsBandmateInPlay)
            {
                return false;
            }
            return base.AskIfActionCanBePerformed(gameAction);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            var setList = GameController.FindCardsWhere(c => c.Identifier == "TheSetList", false).First();
            return card == base.Card && setList.IsFlipped;
        }


    }
}

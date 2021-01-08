using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineUtilityCardController : CardController
    {
        protected ScreaMachineUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public abstract IEnumerable<ScreaMachineBandmate.Value> AbilityIcons { get; }

        protected IEnumerator AcivateBandAbilities(IEnumerable<ScreaMachineBandmate.Value> members)
        {
            var keys = members.Select(m => m.GetAbilityKey()).ToArray();
            if (!keys.Any())
                yield break;

            foreach (var card in FindCardsWhere(c => c.IsVillain && c.IsInPlayAndNotUnderCard && !c.IsOneShot))
            {
                var cc = FindCardController(card);
                foreach (var ability in cc.GetActivatableAbilities().Where(aa => keys.Contains(aa.AbilityKey)))
                {
                    var coroutine = GameController.ActivateAbility(ability, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
        }

        protected Card SliceCharacter => base.FindCard("SliceCharacter");

        protected Card ValentineCharacter => base.FindCard("ValentineCharacter");

        protected Card BloodlaceCharacter => base.FindCard("BloodlaceCharacter");

        protected Card RickyGCharacter => base.FindCard("RickyGCharacter");

        protected TheSetListCardController TheSetListCardController => base.FindCardController(base.FindCard("TheSetList", false)) as TheSetListCardController;




    }
}

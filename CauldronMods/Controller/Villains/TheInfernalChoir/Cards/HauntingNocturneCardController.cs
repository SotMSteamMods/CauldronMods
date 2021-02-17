using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class HauntingNocturneCardController : TheInfernalChoirUtilityCardController
    {
        public HauntingNocturneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(EclipseIdentifier);
        }

        public readonly string EclipseIdentifier = "Eclipse";

        public override IEnumerator Play()
        {
            var locations = new Location[]
           {
                base.TurnTaker.Deck,
                base.TurnTaker.Trash
           };

            IEnumerator coroutine = base.PlayCardFromLocations(locations, EclipseIdentifier, isPutIntoPlay: true, showMessageIfFailed: false, shuffleAfterwardsIfDeck: false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.IsSameCard(CharacterCard), 1);
        }
    }
}

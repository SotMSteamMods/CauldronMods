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
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("Eclipse");
        }

        public override IEnumerator Play()
        {
            var card = TurnTaker.FindCard("Eclipse");
            if (card is null || card.IsInPlay)
                yield break;

            var coroutine = GameController.PlayCard(TurnTakerController, card, isPutIntoPlay: true, cardSource: GetCardSource());
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

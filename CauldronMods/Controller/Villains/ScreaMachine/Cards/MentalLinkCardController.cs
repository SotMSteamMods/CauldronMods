using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class MentalLinkCardController : ScreaMachineBandCardController
    {
        public MentalLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
        }

        private static readonly string SuppressActivatableAbilites = "MentalLinkNoAbilities";

        protected override IEnumerator ActivateBandAbility()
        {
            // Development notes:
            // so initial investigation focused on usingthe AddInhibitor/AddInhibitorException methods
            // This failed becuse those conditions for those is tied to GameContoller.AllowInhibitors and that prop
            // is never true, it's only set to true during certain trigger resolution.
            // AskIfActionCanBePerformed is the answer.
            

            IEnumerator coroutine;

            if (TurnTaker.Deck.Cards.Count() == 0)
            {
                coroutine = GameController.ShuffleTrashIntoDeck(TurnTakerController, necessaryToPlayCard: true, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //stolen message action from PlayTopCard
            var cardToPlay = TurnTaker.Deck.TopCard;
            var cc = FindCardController(cardToPlay);
            if (GameController.CanPlayCard(cc) == CanPlayCardResult.CanPlay)
            {
                //..because we actually Get the Top Card, and directly play it
                coroutine = GameController.SendMessageAction($"{ Card.Title} plays the top card of {TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource(), null, true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            } //if we can't play the card for whatever reason, the normal playCard messaging will cover us.

            cc.SetCardPropertyToTrueIfRealAction(SuppressActivatableAbilites);
            coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (IsRealAction())
            {
                cc.SetCardProperty(SuppressActivatableAbilites, false);
            }
        }

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            //pre-filter to activateAbilityActions on ScreaMachine cards
            if (gameAction is ActivateAbilityAction aa && ScreaMachineBandmate.AbilityKeys.Contains(aa.ActivatableAbility.AbilityKey))
            {
                if (Journal.GetCardPropertiesBoolean(aa.CardSource.Card, SuppressActivatableAbilites) == true)
                {
                    return false;
                }
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }
    }
}

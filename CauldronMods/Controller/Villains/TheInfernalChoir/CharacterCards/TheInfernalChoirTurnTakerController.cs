using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using Handelabra;

namespace Cauldron.TheInfernalChoir
{
    public class TheInfernalChoirTurnTakerController : TurnTakerController
    {
        public TheInfernalChoirTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public HeroTurnTakerController DebugForceHeartPlayer { get; set; }

        /*
         * "At the start of the game, put {TheInfernalChoir}'s villain character cards into play, “Unfinished Business“ side up.",
           "Search the villain deck for the card Vagrant Heart and put it face-up in the play area of a random hero. Shuffle the villain deck."
         */

        public override IEnumerator StartGame()
        {
            var p1Heart = TurnTaker.FindCard("VagrantHeartPhase1", false);
            IEnumerator coroutine;
            var p2Heart = TurnTaker.FindCard("VagrantHeartPhase2", false);

            coroutine = GameController.FlipCard(FindCardController(p2Heart));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var hero = GameController.FindHeroTurnTakerControllers().ToList().Shuffle(GameController.Game.RNG).First();
            if (DebugForceHeartPlayer != null)
            {
                hero = DebugForceHeartPlayer;
                Log.Debug($"Forcing Vagrant Heart Hero target to be {hero.TurnTaker.NameRespectingVariant}");
            }

            coroutine = GameController.SendMessageAction($"The {p1Heart.Title} moves into {hero.TurnTaker.PlayArea.GetFriendlyName()}.", Priority.Medium, CharacterCardController.GetCardSource(), new[] { p1Heart });
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayCard(this, p1Heart, overridePlayLocation: hero.TurnTaker.PlayArea, canBeCancelled: false, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }
    }
}
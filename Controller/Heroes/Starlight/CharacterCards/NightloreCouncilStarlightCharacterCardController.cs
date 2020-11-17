using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Starlight
{
    public class NightloreCouncilStarlightCharacterCardController : HeroCharacterCardController
    {
        public NightloreCouncilStarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddStartOfGameTriggers()
        {
            Log.Debug("Loading sub-characters...");
            (HeroTurnTakerController as StarlightTurnTakerController).LoadSubCharactersNoEnumerator();
            Log.Debug("Load routine complete...");
            //if (UseUnityCoroutines)
            //{
            //    yield return GameController.StartCoroutine(loadCharacters);
            //}
            //else
            //{
            //    GameController.ExhaustCoroutine(loadCharacters);
            //}
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        var coroutine = GameController.SendMessageAction("Temporarily out of order, very sorry.", Priority.High, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"1 player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        IEnumerator coroutine3 = GameController.SelectAndGainHP(HeroTurnTakerController, 2, optional: false, (Card c) => c.IsInPlay && c.IsHero && c.IsTarget, 1, null, allowAutoDecide: false, null, GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }


            yield break;
        }

    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Tiamat
{
    public class TiamatTurnTakerController : TurnTakerController
    {
        public TiamatTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            if (base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter").MaximumHitPoints == 15)
            {
                Card earth = base.TurnTaker.GetCardByIdentifier("EarthTiamatCharacter");
                Card decay = base.TurnTaker.GetCardByIdentifier("DecayTiamatCharacter");
                Card wind = base.TurnTaker.GetCardByIdentifier("WindTiamatCharacter");
                Card earthInstructions = base.TurnTaker.GetCardByIdentifier("FrigidEarthTiamatInstructions");
                Card decayInstructions = base.TurnTaker.GetCardByIdentifier("NoxiousFireTiamatInstructions");
                Card windInstructions = base.TurnTaker.GetCardByIdentifier("ThunderousGaleTiamatInstructions");

                IEnumerator coroutine = base.GameController.PlayCard(this, earthInstructions);
                IEnumerator coroutine2 = base.GameController.PlayCard(this, decayInstructions);
                IEnumerator coroutine3 = base.GameController.PlayCard(this, windInstructions);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                    base.GameController.ExhaustCoroutine(coroutine3);
                }

                coroutine = base.GameController.MoveCard(this, earth, base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter").UnderLocation, flipFaceDown: true);
                coroutine2 = base.GameController.MoveCard(this, decay, base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacter").UnderLocation, flipFaceDown: true);
                coroutine3 = base.GameController.MoveCard(this, wind, base.TurnTaker.GetCardByIdentifier("StormTiamatCharacter").UnderLocation, flipFaceDown: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
            }
            yield break;
        }
    }
}
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

        private Card[] inPlay;
        private Card[] inBox;

        public override IEnumerator StartGame()
        {
            //Winter is in both, just has promoIdentifier differentiating
            Card winter = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter");
            //Regular
            Card inferno = base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacter");
            Card storm = base.TurnTaker.GetCardByIdentifier("StormTiamatCharacter");
            //Hydra
            Card hydraStorm = base.TurnTaker.GetCardByIdentifier("HydraStormTiamatCharacter");
            Card hydraInferno = base.TurnTaker.GetCardByIdentifier("HydraInfernoTiamatCharacter");
            Card hydraEarth = base.TurnTaker.GetCardByIdentifier("HydraEarthTiamatCharacter");
            Card hydraDecay = base.TurnTaker.GetCardByIdentifier("HydraDecayTiamatCharacter");
            Card hydraWind = base.TurnTaker.GetCardByIdentifier("HydraWindTiamatCharacter");
            Card hydraEarthInstructions = base.TurnTaker.GetCardByIdentifier("HydraFrigidEarthTiamatInstructions");
            Card hydraDecayInstructions = base.TurnTaker.GetCardByIdentifier("HydraNoxiousFireTiamatInstructions");
            Card hydraWindInstructions = base.TurnTaker.GetCardByIdentifier("HydraThunderousGaleTiamatInstructions");

            if (base.CharacterCard.PromoIdentifierOrIdentifier == "WinterTiamatCharacter")
            {//Regular Tiamat
                this.inPlay = new Card[] { inferno, storm };
                this.inBox = new Card[] { hydraInferno, hydraStorm, hydraEarth, hydraDecay, hydraWind, hydraEarthInstructions, hydraDecayInstructions, hydraWindInstructions };
                IEnumerator coroutine = this.ManageCharacters();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else if (base.CharacterCard.PromoIdentifierOrIdentifier == "HydraWinterTiamatCharacter")
            {//Elemental Hydra
                this.inBox = new Card[] { inferno, storm };
                this.inPlay = new Card[] { hydraInferno, hydraStorm, hydraEarthInstructions, hydraDecayInstructions, hydraWindInstructions };
                IEnumerator coroutine = this.ManageCharacters();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //Secondary Heads start underneath other heads
                coroutine = base.GameController.MoveCard(this, hydraEarth, winter.UnderLocation, flipFaceDown: true);
                IEnumerator coroutine2 = base.GameController.MoveCard(this, hydraDecay, hydraInferno.UnderLocation, flipFaceDown: true);
                IEnumerator coroutine3 = base.GameController.MoveCard(this, hydraWind, hydraStorm.UnderLocation, flipFaceDown: true);
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

        private IEnumerator ManageCharacters()
        {
            IEnumerator coroutine;
            //Start in play
            foreach (Card c in this.inPlay)
            {
                coroutine = base.GameController.PlayCard(this, c);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Banish to Box
            foreach (Card c in this.inBox)
            {
                coroutine = base.GameController.MoveCard(this, c, base.TurnTaker.InTheBox);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
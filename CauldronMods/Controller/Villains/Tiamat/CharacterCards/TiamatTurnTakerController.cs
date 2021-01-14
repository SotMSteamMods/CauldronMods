using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Tiamat
{
    public class TiamatTurnTakerController : TurnTakerController
    {
        public TiamatTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Elemental Hydra
            if (base.FindCardController(base.CharacterCard) is HydraWinterTiamatCharacterCardController)
            {
                Card winter = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter");
                Card hydraStorm = base.TurnTaker.GetCardByIdentifier("HydraStormTiamatCharacter");
                Card hydraInferno = base.TurnTaker.GetCardByIdentifier("HydraInfernoTiamatCharacter");
                Card hydraEarth = base.TurnTaker.GetCardByIdentifier("HydraEarthTiamatCharacter");
                Card hydraDecay = base.TurnTaker.GetCardByIdentifier("HydraDecayTiamatCharacter");
                Card hydraWind = base.TurnTaker.GetCardByIdentifier("HydraWindTiamatCharacter");
                Card hydraEarthInstructions = base.TurnTaker.GetCardByIdentifier("HydraFrigidEarthTiamatInstructions");
                Card hydraDecayInstructions = base.TurnTaker.GetCardByIdentifier("HydraNoxiousFireTiamatInstructions");
                Card hydraWindInstructions = base.TurnTaker.GetCardByIdentifier("HydraThunderousGaleTiamatInstructions");

                //Secondary Heads start underneath other heads
                IEnumerator coroutine = base.GameController.MoveCard(this, hydraEarth, winter.UnderLocation, flipFaceDown: true);
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

            if (base.FindCardController(base.CharacterCard) is FutureTiamatCharacterCardController)
            {
                Card[] future = new Card[]
                {
                    base.TurnTaker.GetCardByIdentifier("ExoscaleCharacter"),
                    base.TurnTaker.GetCardByIdentifier("NeoscaleCharacter")
                };

                //Dragonscales have X HP where X = {H - 1}.
                int maxHP = base.GameController.Game.H - 1;
                if (base.GameController.Game.IsAdvanced)
                {
                    //Advanced: X = {H + 1} instead.
                    maxHP = base.GameController.Game.H + 1;
                }

                foreach (Card scale in future)
                {
                    scale.SetMaximumHP(maxHP, scale.MaximumHitPoints != null);
                }
            }
            yield break;
        }

        public void MoveStartingCards()
        {
            //Winter is in all, just has promoIdentifier differentiating
            Card winter = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter");
            //Regular
            Card[] regular = new Card[] {
                base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacter"),
                base.TurnTaker.GetCardByIdentifier("StormTiamatCharacter")
            };
            //Hydra
            Card[] hydra = new Card[] {
                base.TurnTaker.GetCardByIdentifier("HydraStormTiamatCharacter"),
                base.TurnTaker.GetCardByIdentifier("HydraInfernoTiamatCharacter"),
                base.TurnTaker.GetCardByIdentifier("HydraFrigidEarthTiamatInstructions"),
                base.TurnTaker.GetCardByIdentifier("HydraNoxiousFireTiamatInstructions"),
                base.TurnTaker.GetCardByIdentifier("HydraThunderousGaleTiamatInstructions")
            };
            //Hydra secondary heads that do not need to be moved into play if Hydra is the variant
            Card[] secondaryHydra = new Card[]
            {
                base.TurnTaker.GetCardByIdentifier("HydraEarthTiamatCharacter"),
                base.TurnTaker.GetCardByIdentifier("HydraDecayTiamatCharacter"),
                base.TurnTaker.GetCardByIdentifier("HydraWindTiamatCharacter")
            };
            //2199
            Card[] future = new Card[]
            {
                base.TurnTaker.GetCardByIdentifier("ExoscaleCharacter"),
                base.TurnTaker.GetCardByIdentifier("NeoscaleCharacter")
            };

            Card[] inPlay;
            Card[] inBox;

            if (base.FindCardController(base.CharacterCard) is WinterTiamatCharacterCardController)
            {//Regular Tiamat
                inPlay = regular;
                inBox = hydra.Concat(secondaryHydra).Concat(future).ToArray();
            }
            else if (base.FindCardController(base.CharacterCard) is HydraWinterTiamatCharacterCardController)
            {//Elemental Hydra
                inPlay = hydra;
                inBox = regular.Concat(future).ToArray();
            }
            else if (base.FindCardController(base.CharacterCard) is FutureTiamatCharacterCardController)
            {//2199
                inPlay = future;
                inBox = regular.Concat(hydra).Concat(secondaryHydra).ToArray();
            }
            else
            {
                throw new InvalidOperationException("Character Controller is not a Tiamat Character Card Controller");
            }

            foreach (Card c in inPlay)
            {
                TurnTaker.MoveCard(c, TurnTaker.PlayArea);
            }
            foreach (Card c in inBox)
            {
                TurnTaker.MoveCard(c, TurnTaker.InTheBox);
            }
        }
    }
}
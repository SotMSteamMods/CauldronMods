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

        public string[] availablePromos = new string[] { "HydraTiamat" };

        public bool ArePromosSetup { get; set; } = false;

        public IEnumerator SupplementalSetup()
        {

            //Elemental Hydra
            if (base.FindCardController(base.CharacterCard) is HydraWinterTiamatCharacterCardController)
            {
                Card winter = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter");
                Card hydraStorm = base.TurnTaker.GetCardByIdentifier("HydraStormTiamatCharacter");
                Card hydraInferno = base.TurnTaker.GetCardByIdentifier("HydraInfernoTiamatCharacter");
                Card hydraEarth = base.TurnTaker.FindCard("HydraEarthTiamatCharacter");
                Card hydraDecay = base.TurnTaker.FindCard("HydraDecayTiamatCharacter");
                Card hydraWind = base.TurnTaker.FindCard("HydraWindTiamatCharacter");
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

        public override IEnumerator StartGame()
        {
            //Winter is in all, just has promoIdentifier differentiating
            Card winter = base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacter");
            
            //Regular
            Card[] regular = new Card[] {
                base.TurnTaker.FindCard("InfernoTiamatCharacter"),
                base.TurnTaker.FindCard("StormTiamatCharacter")
            };
            
            //Hydra
            Card[] hydra = new Card[] {
                base.TurnTaker.FindCard("HydraInfernoTiamatCharacter"),
                base.TurnTaker.FindCard("HydraStormTiamatCharacter"),
                base.TurnTaker.FindCard("HydraFrigidEarthTiamatInstructions", false),
                base.TurnTaker.FindCard("HydraNoxiousFireTiamatInstructions", false),
                base.TurnTaker.FindCard("HydraThunderousGaleTiamatInstructions", false)
            };
            
            //Hydra secondary heads that do not need to be moved into play if Hydra is the variant
            Card[] secondaryHydra = new Card[]
            {
                base.TurnTaker.FindCard("HydraEarthTiamatCharacter"),
                base.TurnTaker.FindCard("HydraDecayTiamatCharacter"),
                base.TurnTaker.FindCard("HydraWindTiamatCharacter")
            };
            
            //2199
            Card[] future = new Card[]
            {
                base.TurnTaker.FindCard("ExoscaleCharacter"),
                base.TurnTaker.FindCard("NeoscaleCharacter")
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

            IEnumerator coroutine;
            foreach (Card c in inPlay)
            {
                if (c.Location.IsOffToTheSide)
                { 
                    coroutine = GameController.PlayCard(this, c, isPutIntoPlay: true, cardSource: FindCardController(c).GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                      
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            foreach (Card c in inBox)
            {
                if (c.Location.IsOffToTheSide)
                {
                    TurnTaker.MoveCard(c, TurnTaker.InTheBox);
                    coroutine = GameController.MoveCard(this, c, TurnTaker.InTheBox, cardSource: FindCardController(c).GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);

                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            coroutine = SupplementalSetup();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override bool IsGameWinnable()
        {
            
            if(GameController.FindCardController(TurnTaker.FindCard("WinterTiamatCharacter")) is HydraWinterTiamatCharacterCardController)
            {
                return TurnTaker.GetCardsWhere((Card c) => c.IsCharacter && c.IsInGame).Count() == 6;
            }
            
            return true;
        }
    }
}
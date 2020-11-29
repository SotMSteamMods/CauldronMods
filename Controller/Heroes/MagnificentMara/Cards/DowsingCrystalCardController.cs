using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class DowsingCrystalCardController : CardController
    {
        public DowsingCrystalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay);
        }

        public override IEnumerator Play()
        {
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Once before your next turn when a non-hero card enters play, one hero target may deal a non-hero target 2 damage of a type of their choosing. You may destroy this card to increase that damage by 2."

            //There is no way to set up a status effect that triggers when any card enters play.
            //The actual mechanical function of this effect lives on MaraUtilityCharacter, 
            //this just notifies it that it's time to start working.
            

            ActivateEffectStatusEffect dowsingTrigger = new ActivateEffectStatusEffect(TurnTaker, TurnTaker.CharacterCard, "Dowsing Crystal trigger");
            dowsingTrigger.UntilEndOfNextTurn(TurnTaker);
            IEnumerator coroutine = GameController.AddStatusEffect(dowsingTrigger, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Log.Debug($"Dowsing Crystal trigger effects allowed? {CanActivateEffect((TurnTaker.CharacterCard), "Dowsing Crystal trigger")}");

            yield break;
        }
    }
}
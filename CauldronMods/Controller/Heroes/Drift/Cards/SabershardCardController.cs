using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class SabershardCardController(Card card, TurnTakerController turnTakerController) : DriftBaseCardController(card, turnTakerController)
{
    public override IEnumerator UsePower(int index = 0) => index switch
    {
        0 => PastPower(),
        1 => FuturePower(),
        _ => DoNothing()
    };

    private IEnumerator PastPower()
    {
        IEnumerator coroutine;
        //{DriftPast} 
        if (this.IsTimeMatchingPast)
        {
            //Draw a card. 
            coroutine = base.DrawCard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shift {R}.
            coroutine = this.ShiftR();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        else
        {
            coroutine = GameController.SendMessageAction($"{CharacterCard.Title} is not on a {this.Past} space, so nothing happens!", Priority.High, GetCardSource(), showCardSource: true);
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

    private IEnumerator FuturePower()
    {
        IEnumerator coroutine;
        //{DriftFuture} 
        if (this.IsTimeMatchingFuture)
        {
            int targetNumeral = base.GetPowerNumeral(0, 1);
            int damageNumeral = base.GetPowerNumeral(1, 2);

            //{Drift} 1 target 2 radiant damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), damageNumeral, DamageType.Radiant, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shift {L}.
            coroutine = this.ShiftL();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        else
        {
            coroutine = GameController.SendMessageAction($"{CharacterCard.Title} is not on a {this.Future} space, so nothing happens!", Priority.High, GetCardSource(), showCardSource: true);
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
}

using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class VortexInterferenceCardController : CardController
    {

        public VortexInterferenceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


        public override void AddTriggers()
        {
            //Whenever a hero uses a power, destroy 1 hero ongoing or equipment card.
            base.AddTrigger<UsePowerAction>((UsePowerAction p) => GameController.IsTurnTakerVisibleToCardSource(p.HeroUsingPower.TurnTaker, GetCardSource()), this.DestroyHeroOngoingOrEquipmentResponse, new TriggerType[] { TriggerType.DestroyCard }, TriggerTiming.After);
            //When another environment card enters play, destroy this card.
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction p) => p.CardEnteringPlay.IsEnvironment && p.CardEnteringPlay.Identifier != base.Card.Identifier && GameController.IsCardVisibleToCardSource(p.CardEnteringPlay, GetCardSource()), base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DestroyHeroOngoingOrEquipmentResponse(UsePowerAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || base.IsEquipment(c)) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "hero ongoing or equipment"), false, cardSource: base.GetCardSource());
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
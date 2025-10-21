using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Gray
{
    public class RadioactiveCascadeCardController : GrayCardController
    {
        public RadioactiveCascadeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation"), "radiation"));
        }

        public override IEnumerator Play()
        {
            //When this card enters play, {Gray} deals the hero with the highest HP X energy damage, where X is 2 plus the number of Radiation cards in play.
            IEnumerator coroutine = DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHeroCharacterCard(c), (Card c) => this.FindNumberOfRadiationCardsInPlay() + 2, DamageType.Energy);
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

        public override void AddTriggers()
        {
            //When another villain card is destroyed, destroy this card.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.WasCardDestroyed && IsVillain(action.CardToDestroy.Card), base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }
    }
}

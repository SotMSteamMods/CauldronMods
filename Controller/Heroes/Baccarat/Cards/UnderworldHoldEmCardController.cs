using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class UnderworldHoldEmCardController : CardController
    {
        #region Constructors

        public UnderworldHoldEmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //One player may draw a card.
            IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController,numberOfCards: new int?(1), cardSource: base.GetCardSource(null));
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

        #endregion Methods
    }
}
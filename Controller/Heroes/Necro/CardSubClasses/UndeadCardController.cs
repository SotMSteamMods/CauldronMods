using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
    public abstract class UndeadCardController : NecroCardController
    {
        protected UndeadCardController(Card card, TurnTakerController turnTakerController, int baseHP) : base(card, turnTakerController)
        {
            this.BaseHP = baseHP;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = base.GameController.ChangeMaximumHP(base.Card, BaseHP + GetNumberOfRitualsInPlay(), true, cardSource: base.GetCardSource());
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

        protected int BaseHP { get; private set; }
    }
}

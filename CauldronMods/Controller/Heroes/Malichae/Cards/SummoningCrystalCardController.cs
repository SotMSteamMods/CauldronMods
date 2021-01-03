using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class SummoningCrystalCardController : MalichaeCardController
    {
        public SummoningCrystalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            var coroutine = base.SelectAndPlayCardFromHand(this.DecisionMaker,
                cardCriteria: new LinqCardCriteria(c => IsDjinn(c), DjinnKeyword),
                isPutIntoPlay: true);
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

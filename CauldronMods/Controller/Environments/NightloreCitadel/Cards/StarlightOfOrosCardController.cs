using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class StarlightOfOrosCardController : NightloreCitadelUtilityCardController
    {
        public StarlightOfOrosCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 2 infernal damage and each other environment target 2 psychic damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()) && c.IsTarget && IsHero(c), TargetType.All, 2, DamageType.Infernal);
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource()) && c.IsEnvironmentTarget && c != Card, TargetType.All, 2, DamageType.Psychic);
            //Then, each villain target next to a Constellation regains {H} HP.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, VillainGainHPResponse, TriggerType.GainHP);
        }

        private IEnumerator VillainGainHPResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = GameController.GainHP(DecisionMaker, (Card c) => IsVillainTarget(c) && c.GetAllNextToCards(false).Any(card => IsConstellation(card)), Game.H, cardSource: GetCardSource());
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

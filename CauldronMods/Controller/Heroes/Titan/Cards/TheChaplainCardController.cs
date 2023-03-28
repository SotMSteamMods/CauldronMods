using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class TheChaplainCardController : TitanCardController
    {
        public TheChaplainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            switch (index)
            {
                case 0:
                    {
                        int targetNumeral = base.GetPowerNumeral(0, 1);
                        int damageNumeral = base.GetPowerNumeral(1, 3);
                        //{Titan} deals 1 target 3 projectile damage.
                        coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Projectile, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        if (base.GetTitanform().IsInPlayAndHasGameText)
                        {
                            int ongoingNumeral = base.GetPowerNumeral(2, 1);
                            //If Titanform is in play, destroy 1 ongoing card.
                            coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c)), false, cardSource: base.GetCardSource());
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
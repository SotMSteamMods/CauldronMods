using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class RunecarvedTheStrangerCharacterCardController : TheStrangerBaseCharacterCardController
    {
        public RunecarvedTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //1 hero target regains 2 HP.
            int targets = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 2);
            IEnumerator coroutine = GameController.SelectAndGainHP(base.HeroTurnTakerController, amount, additionalCriteria: (Card c) => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, numberOfTargets: targets, cardSource: GetCardSource());
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now.
                        IEnumerator coroutine = SelectHeroToPlayCard(DecisionMaker);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //Destroy 1 hero ongoing, 1 non-hero ongoing, and 1 environment card.
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && IsHero(c) && c.IsInPlay, "hero ongoing"), false, cardSource: GetCardSource());
                        IEnumerator coroutine3 = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => IsOngoing(c) && !IsHero(c) && c.IsInPlay, "non-hero ongoing"), false, cardSource: GetCardSource());
                        IEnumerator coroutine4 = base.GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlay, "environment"),false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                            yield return base.GameController.StartCoroutine(coroutine3);
                            yield return base.GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                            base.GameController.ExhaustCoroutine(coroutine3);
                            base.GameController.ExhaustCoroutine(coroutine4);

                        }
                        break;

                    }
                case 2:
                    {
                        //Villain targets may not regain HP until the start of your turn.
                        CannotGainHPStatusEffect cannotGainHPStatusEffect = new CannotGainHPStatusEffect();
                        cannotGainHPStatusEffect.TargetCriteria.IsVillain = true;
                        cannotGainHPStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                        IEnumerator coroutine5 = AddStatusEffect(cannotGainHPStatusEffect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine5);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine5);
                        }
                        break;
                    }
            }
            yield break;
        }

    }
}

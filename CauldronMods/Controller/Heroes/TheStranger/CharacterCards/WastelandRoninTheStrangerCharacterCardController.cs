using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class WastelandRoninTheStrangerCharacterCardController : TheStrangerBaseCharacterCardController
    {
        public WastelandRoninTheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard a rune. If you do, {TheStranger} deals 1 target 4 infernal damage.
            int target = GetPowerNumeral(0, 1);
            int amount = GetPowerNumeral(1, 4);

            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, 1, optional: false, storedResults: storedResults, cardCriteria: IsRuneCriteria());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidDiscardCards(storedResults))
            {
                coroutine = base.GameController.SelectTargetsAndDealDamage(HeroTurnTakerController, new DamageSource(GameController, CharacterCard), amount, DamageType.Infernal, target, optional: false, target, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        //One hero target deals 1 target 1 infernal damage.
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        IEnumerator coroutine2 = base.GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.CardToDealDamage, new LinqCardCriteria((Card c) => c.IsInPlay && IsHeroTarget(c), "hero target"), storedResults, optional: false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card source = GetSelectedCard(storedResults);
                            coroutine2 = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, source), 1, DamageType.Infernal, 1, optional: false, 1, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine2);
                            }
                        }
                        break;

                    }
                case 2:
                    {
                        //Destroy all environment cards. Play the top card of the environment deck.
                        IEnumerator coroutine3 = base.GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), cardSource: GetCardSource());
                        IEnumerator message = GameController.SendMessageAction(Card.Title + " plays the top card of the environment deck...", Priority.High, GetCardSource(), showCardSource: true);
                        IEnumerator playE = base.GameController.PlayTopCard(DecisionMaker, FindEnvironment(), cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                            yield return base.GameController.StartCoroutine(message);
                            yield return base.GameController.StartCoroutine(playE);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                            base.GameController.ExhaustCoroutine(message);
                            base.GameController.ExhaustCoroutine(playE);
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}

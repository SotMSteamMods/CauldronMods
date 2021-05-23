using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class AllAboardCardController : CatchwaterHarborUtilityCardController
    {
        public AllAboardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible.
            return card == Card;
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, the players may activate the Travel text of a Transport card. If they do, destroy that card at the start of the next environment turn.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.Other,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //the players may activate the Travel text of a Transport card. If they do, destroy that card at the start of the next environment turn.

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.ActivateAbility, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsTransport(c), "transport"), storedResults, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidSelectCard(storedResults))
            {
                Card transport = GetSelectedCard(storedResults);
                CardController transportController = FindCardController(transport);
                var abilities = transport.Definition.ActivatableAbilities.Where(def => def.Name == "travel");
                if (abilities.Any())
                {
                    
                    SetCardPropertyToTrueIfRealAction(DestroyNextTurnKey, transport);
                    coroutine = transportController.ActivateAbilityEx(abilities.FirstOrDefault());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.SendMessageAction($"{transport.Title} will be destroyed at the start of the next environment turn!", Priority.High, GetCardSource(), showCardSource: true);
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
            yield break;
        }
    }
}

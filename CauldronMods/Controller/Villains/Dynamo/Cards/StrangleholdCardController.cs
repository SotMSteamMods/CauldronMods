using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class StrangleholdCardController : DynamoUtilityCardController
    {
        public StrangleholdCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(PythonIdentifier);
            SpecialStringMaker.ShowSpecialString(() => $"{FindPython().Title} is in {FindPython().Location.GetFriendlyName()}.").Condition = () => Game.HasGameStarted && !FindPython().Location.IsPlayArea;
        }

        public readonly string PythonIdentifier = "Python";

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //If Python is in play...
            if (base.FindPython().IsInPlayAndHasGameText)
            {
                //...destroy {H} hero ongoing and/or equipment cards.
                coroutine = base.GameController.SelectAndDestroyCards(base.DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (base.IsEquipment(c) || IsOngoing(c))), base.Game.H, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //Otherwise, search the villain deck and trash for Python and put him into play.
                Location pythonLoc = base.FindPython().Location;
                if (pythonLoc.IsVillain && (pythonLoc.IsDeck || pythonLoc.IsTrash))
                {
                    coroutine = base.GameController.PlayCard(base.TurnTakerController, base.FindPython(), true, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    //If you searched the villain deck, shuffle it.
                    if (pythonLoc.IsDeck)
                    {
                        coroutine = base.ShuffleDeck(base.DecisionMaker, base.TurnTaker.Deck);
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
            }
            yield break;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class HelmetedChargeCardController : DynamoUtilityCardController
    {
        public HelmetedChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(CopperheadIdentifier);
            SpecialStringMaker.ShowSpecialString(() => $"{FindCopperhead().Title} is in {FindCopperhead().Location.GetFriendlyName()}.").Condition = () => Game.HasGameStarted && !FindCopperhead().Location.IsPlayArea;
        }

        public readonly string CopperheadIdentifier = "Copperhead";

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //If Copperhead is in play...
            if (base.FindCopperhead().IsInPlayAndHasGameText)
            {
                //...he deals each hero target 2 melee damage.
                coroutine = base.DealDamage(base.FindCopperhead(), (Card c) => IsHeroTarget(c), 2, DamageType.Melee);
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
                //Otherwise, seach the villain deck and trash for Copperhead and put him into play.
                Location copperheadLoc = base.FindCopperhead().Location;
                if (copperheadLoc.IsVillain && (copperheadLoc.IsDeck || copperheadLoc.IsTrash))
                {
                    coroutine = base.GameController.PlayCard(base.TurnTakerController, base.FindCopperhead(), true, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    //If you searched the villain deck, shuffle it.
                    if (copperheadLoc.IsDeck && copperheadLoc.IsVillain)
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

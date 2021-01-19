using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class OmnivoreCardController : GyrosaurUtilityCardController
    {
        public OmnivoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy a target with 3 or fewer HP.",
            var storedSelect = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.DestroyCard, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.HitPoints <= 3 && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "", false, singular: "target with 3 or fewer HP", plural: "targets with 3 or fewer HP"), storedSelect, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"{Gyrosaur} regains X HP, where X is the HP of that target before it was destroyed.",
            if(DidSelectCard(storedSelect))
            {
                var selectedCard = GetSelectedCard(storedSelect);
                int hpToGain = selectedCard.HitPoints ?? 0;

                coroutine = GameController.DestroyCard(DecisionMaker, selectedCard, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.GainHP(CharacterCard, hpToGain, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"You may shuffle your trash into your deck."
            var storedYesNo = new List<YesNoCardDecision>();
            coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.ShuffleTrashIntoDeck, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(storedYesNo))
            {
                coroutine = GameController.ShuffleTrashIntoDeck(DecisionMaker, cardSource: GetCardSource());
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
    }
}

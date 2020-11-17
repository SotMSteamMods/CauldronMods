using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class MirrorWraithCardController : CardController
    {
        //==============================================================
        // When this card enters play,
        // it gains the text, keywords, and max HP of the non-character
        // target with the lowest HP in play.
        // If there are no other non-character targets in play when
        // this card enters play, this card deals each
        // target 2 sonic damage and is destroyed.
        //==============================================================


        //==============================================================
        // Possible cards that may cause issue if copied?
        //==============================================================
        /*
         *
         * Huginn & Muninn (Harpy) - Double boosting Harpy?
         */
        //==============================================================


        public static string Identifier = "MirrorWraith";

        private const int DamageToDeal = 2;

        private IEnumerable<string> _copiedKeywords;
        private Dictionary<string, List<ITrigger>> _copiedTriggers;


        public MirrorWraithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _copiedKeywords = Enumerable.Empty<string>();
            _copiedTriggers = new Dictionary<string, List<ITrigger>>();

            // Identify this card controller as one who can modify keyword query answers
            base.AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetsWithLowestHitPoints(1, 1,
                c => c.IsTarget && c.IsInPlay && !c.IsCharacter && !c.Equals(this.Card), storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithLowestHpRoutine);
            }

            if (!storedResults.Any())
            {
                // No eligible targets were found, deal all targets 2 sonic damage
                IEnumerator dealDamageRoutine
                    = this.DealDamage(this.Card, card => card.IsTarget && !card.Equals(this.Card), DamageToDeal,
                        DamageType.Sonic);

                // Destroy self
                IEnumerator destroyRoutine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(dealDamageRoutine);
                    yield return base.GameController.StartCoroutine(destroyRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(dealDamageRoutine);
                    base.GameController.ExhaustCoroutine(destroyRoutine);
                }
            }
            else
            {
                // Gains the text, keywords, and max HP of found target
                Card cardToCopy = storedResults.First();

                IEnumerator setHpRoutine = base.GameController.SetHP(this.Card, cardToCopy.MaximumHitPoints.Value, this.GetCardSource());

                // TODO: gain text
                CopyGameText(cardToCopy);

                // Add the target's keywords to our copied list which will be returned on keyword queries
                _copiedKeywords = cardToCopy.Definition.Keywords;

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(setHpRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(setHpRoutine);
                }
            }
        }

        public override bool AskIfCardContainsKeyword(Card card, string keyword, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            // If the card being queried is this card and we have the copied keyword, return true
            // otherwise let the base method handle the query

            return (card == this.Card &&_copiedKeywords.Contains(keyword) 
                    || base.AskIfCardContainsKeyword(card, keyword, evenIfUnderCard, evenIfFaceDown));
        }

        public override IEnumerable<string> AskForCardAdditionalKeywords(Card card)
        {
            // If the card being queried is this card and we have a non empty copied keyword list, return it
            // otherwise let the base method handle the return

            if (card == this.Card && _copiedKeywords.Any())
            {
                return _copiedKeywords;
            }

            return base.AskForCardAdditionalKeywords(card);
        }

        private void CopyGameText(Card sourceCard)
        {
            //IEnumerable<ITrigger> trigger =
//                FindTriggersWhere(t => t.CardSource.CardController, CardWithoutReplacements == sourceCard);


        }
    }
}
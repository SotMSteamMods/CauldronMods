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
        private Card _copiedCard;
        private List<ITrigger> _copiedTriggers;


        public MirrorWraithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _copiedKeywords = Enumerable.Empty<string>();
            _copiedTriggers = new List<ITrigger>();
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

                // Identify this card controller as one who can modify keyword query answers
                base.AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);
                
                // Identify this card controller as one who can modify card query answers
                base.AddThisCardControllerToList(CardControllerListType.ReplacesCards);

                // Identify this card controller as one who can modify card source query answers
                base.AddThisCardControllerToList(CardControllerListType.ReplacesCardSource);

                // TODO: Determine if these are needed - don't think so
                //base.AddThisCardControllerToList(CardControllerListType.ActivatesEffects);
                //base.AddThisCardControllerToList(CardControllerListType.ReplacesTurnTakerController);

                _copiedCard = storedResults.First();

                // Set HP
                IEnumerator setHpRoutine = base.GameController.SetHP(this.Card, _copiedCard.MaximumHitPoints.Value, this.GetCardSource());

                // Set card text
                CopyGameText(_copiedCard);

                // Add the target's keywords to our copied list which will be returned on keyword queries
                _copiedKeywords = _copiedCard.Definition.Keywords;

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

        public override CardSource AskIfCardSourceIsReplaced(CardSource cardSource, GameAction gameAction = null,
            ITrigger trigger = null)
        {
            if (cardSource == null || !cardSource.AllowReplacements || this._copiedCard == null 
                || !this._copiedCard.Equals(cardSource.Card) 
                || cardSource.AssociatedCardSources.All(acs => acs.Card != this.Card) 
                || !ShouldSwapCardSources(cardSource, trigger))
            {
                return null;
            }

            CardSource newCardSource = (from cs in cardSource.AssociatedCardSources
                where cs.CardController == this
                select cs).LastOrDefault<CardSource>();

            if (newCardSource == null)
            {
                return null;
            }

            if (newCardSource.AssociatedCardSources.All(cs => cs.CardController != cardSource.CardController))
            {
                newCardSource.AddAssociatedCardSource(cardSource);
            }

            cardSource.RemoveAssociatedCardSourcesWhere(cs => cs.CardController == this);

            return newCardSource;
        }

        public override TurnTakerController AskIfTurnTakerControllerIsReplaced(TurnTakerController ttc, CardSource cardSource)
        {

            // TODO: Don't think this override is necessary.  More testing needed
            int i = 0;

            if (cardSource != null && cardSource.AllowReplacements && this._copiedCard != null &&
                cardSource.CardController == this)
            {

            }

            /*
            if (cardSource != null && cardSource.AllowReplacements && this._ongoings != null && cardSource.CardController == this)
            {
                Card card = (from cs in cardSource.AssociatedCardSources
                    select cs.Card into a
                    where this._ongoings.Contains(a)
                    select a).FirstOrDefault<Card>();
                if (card != null && ttc.TurnTaker == card.Owner)
                {
                    return base.TurnTakerController;
                }
            }
            */

            return null;
        }

        public override Card AskIfCardIsReplaced(Card card, CardSource cardSource)
        {
            if (cardSource == null || !cardSource.AllowReplacements || this._copiedCard == null ||
                cardSource.CardController != this || card == base.CardWithoutReplacements)
            {
                return null;
            }

            CardController cardController = cardSource.AssociatedCardSources.Select(cs => cs.CardController)
                .FirstOrDefault(cc => cc.CardWithoutReplacements == this._copiedCard);

            if (cardController == null)
            {
                return null;
            }

            Card result = null;
            if (cardController.CharacterCardsWithoutReplacements.Contains(card))
            {
                result = base.CharacterCard;
            }
            else if (cardController.CardWithoutReplacements == card)
            {
                result = base.CardWithoutReplacements;
            }

            return result;
        }

        private void CopyGameText(Card sourceCard)
        {
            IEnumerable<ITrigger> triggers =
                FindTriggersWhere(t => t.CardSource.CardController.CardWithoutReplacements == sourceCard);

            foreach (ITrigger trigger in triggers)
            {
                if (trigger.IsStatusEffect)
                {
                    continue;
                }

                ITrigger clonedTrigger = (ITrigger)trigger.Clone();
                clonedTrigger.CardSource = base.FindCardController(sourceCard).GetCardSource();
                clonedTrigger.CardSource.AddAssociatedCardSource(base.GetCardSource());
                clonedTrigger.SetCopyingCardController(this);
                base.AddTrigger(clonedTrigger);
                this._copiedTriggers.Add(clonedTrigger);
            }
        }

        private bool ShouldSwapCardSources(CardSource cardSource, ITrigger trigger = null)
        {
            var cardSources = cardSource.AssociatedCardSources.ToList();
            
            CardSource thisCardSource = (from cs in cardSources
                where cs != null && cs.CardController == this
                select cs).LastOrDefault<CardSource>();

            if (thisCardSource != null)
            {
                bool flag = this._copiedCard == (cardSource.CardController.CardWithoutReplacements);
                bool flag2 = (from cs in cardSources
                    select cs.CardController).Contains(this);
                if (flag && flag2)
                {
                    bool flag3 = cardSource.SourceLimitation != null;
                    CardSource.Limitation? cardSourceLimitation = base.CardSourceLimitation;
                    CardSource.Limitation limitation = CardSource.Limitation.BeforeDestroyed;
                    if (cardSourceLimitation.GetValueOrDefault() == limitation & cardSourceLimitation != null)
                    {
                        thisCardSource.SourceLimitation = new CardSource.Limitation?(CardSource.Limitation.BeforeDestroyed);
                    }
                    else
                    {
                        cardSourceLimitation = base.CardSourceLimitation;
                        limitation = CardSource.Limitation.AfterDestroyed;
                        if (cardSourceLimitation.GetValueOrDefault() == limitation & cardSourceLimitation != null)
                        {
                            thisCardSource.SourceLimitation = new CardSource.Limitation?(CardSource.Limitation.AfterDestroyed);
                        }
                    }
                    bool flag4 = thisCardSource.SourceLimitation != null;
                    return !flag3 || !flag4 || cardSource.SourceLimitation.Value == thisCardSource.SourceLimitation.Value;
                }
            }
            return false;
        }
    }
}
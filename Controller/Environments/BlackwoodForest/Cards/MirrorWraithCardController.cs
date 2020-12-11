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
        // Current: Issues
        //==============================================================
        /*
         * Unable to respond to keyword queries for cards that ask
         * for keywords via Definition.Keywords rather than the newer
         * GameController.DoesHaveKeywords which allows cards to tap into
         * the queries to generate desired responses
         *
         * Examples:
         *
         * Proletariat: A copied clone will not be destroyed by 'Regroup & Recover'
         *
         * Akash'Thriya: Copying a primordial seed will not be an available choice of cards
         * if Akash plays 'Verdant Explosion'
         *
         */
        /*
         * Since the card transforms as part of Play, triggers for CardPlay/EntersPlay and copied card's on entering play text aren't triggered.
         * Reworking to play logic to happen during 'DeterminePlayLocation' might resolve this.
         * 
         * Copied cards that are Played Next to a Target (Pins) probally won't work.  Moving logic up to DeterminePlayArea, and calling the copied cards version of that might
         * work.
         */
        //==============================================================

        public static readonly string Identifier = "MirrorWraith";

        private const int DamageToDeal = 2;

        private readonly List<ITrigger> _copiedTriggers;

        private static readonly string CopiedCardKey = "MirrorWraithCopy";
        private static readonly IEnumerable<string> BaseKeywords = new[] { "creature" };
        private static bool AllowReflectionSelfModification = true;

        public Card CopiedCard => GetCardPropertyJournalEntryCard(CopiedCardKey);
        public IEnumerable<string> CopiedKeywords => CopiedCard?.Definition.Keywords ?? Enumerable.Empty<string>();

        public MirrorWraithCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _copiedTriggers = new List<ITrigger>();

            base.SpecialStringMaker.ShowSpecialString(() =>
                CopiedCard == null ? "Not copying a card" : $"Copying card: {CopiedCard.Title}");

            base.SpecialStringMaker.ShowSpecialString(CopiedBodyText).Condition = () => CopiedCard != null;

            if (CopiedCard != null)
            {
                //WARNING: This could desync multi-player
                //this test is only true when the game deserilizes a game from load/resume
                ModifyDefinitionKeywords(BaseKeywords.Concat(CopiedCard.Definition.Keywords));

                //We don't reset the definition here as that's done when the card leaves play.
            }
        }

        private string CopiedBodyText()
        {
            var card = CopiedCard; //buffer the resolution
            string replacementTitle = "*" + Card.Title + "*";
            var sa = card.Definition.Body.Select(b => b.Replace("{" + card.Title + "}", replacementTitle).Replace(card.Title, replacementTitle)).ToArray();

            return "Copied card text: " + string.Join(System.Environment.NewLine, sa);
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1,
                c => c.IsTarget && c.IsInPlay && !c.IsCharacter
                     && !c.Equals(this.Card), storedResults, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
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
                var copiedCard = storedResults.First();
                Journal.RecordCardProperties(Card, CopiedCardKey, copiedCard);

                // Identify this card controller as one who can modify keyword query answers
                base.AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);

                // Identify this card controller as one who can modify card query answers
                base.AddThisCardControllerToList(CardControllerListType.ReplacesCards);

                // Identify this card controller as one who can modify card source query answers
                base.AddThisCardControllerToList(CardControllerListType.ReplacesCardSource);

                // Identify this card controller as one who can turntaker query answers
                //base.AddThisCardControllerToList(CardControllerListType.ReplacesTurnTakerController);

                // Set HP
                IEnumerator makeTargetRoutine = this.GameController.MakeTargettable(CardWithoutReplacements, copiedCard.MaximumHitPoints.Value, copiedCard.MaximumHitPoints.Value,
                    this.GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(makeTargetRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(makeTargetRoutine);
                }

                // Set card text
                CopyGameText(copiedCard);
                ModifyDefinitionKeywords(BaseKeywords.Concat(copiedCard.Definition.Keywords));

                var playRoutine = DupliPlayCopiedCard(copiedCard);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(playRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(playRoutine);
                }
            }
        }

        private IEnumerator DupliPlayCopiedCard(Card card)
        {
            CardController cc = FindCardController(card);
            CardSource source = GetCardSource();
            source.SourceLimitation = CardSource.Limitation.Play;
            cc.AddAssociatedCardSource(source);
            IEnumerator coroutine = cc.Play();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            cc.RemoveAssociatedCardSource(source);
        }


        private void ModifyDefinitionKeywords(IEnumerable<string> newKeywords)
        {
            if (AllowReflectionSelfModification)
            {
                var fi = CardWithoutReplacements.Definition.GetType().GetField("_keywords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var newList = newKeywords.ToList();
                fi.SetValue(CardWithoutReplacements.Definition, newList);
            }
        }

        public override void AddTriggers()
        {
            AddAfterLeavesPlayAction(LeavePlaysResponse);
        }

        private IEnumerator LeavePlaysResponse()
        {
            //no copied card, no shenaigans to reset
            if (CopiedCard is null)
                yield break;

            var coroutine = GameController.RemoveTarget(CardWithoutReplacements, false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            base.RemoveThisCardControllerFromList(CardControllerListType.ModifiesKeywords);
            base.RemoveThisCardControllerFromList(CardControllerListType.ReplacesCards);
            base.RemoveThisCardControllerFromList(CardControllerListType.ReplacesCardSource);
            //base.RemoveThisCardControllerFromList(CardControllerListType.ReplacesTurnTakerController);

            coroutine = ResetFlagAfterLeavesPlay(CopiedCardKey);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            _copiedTriggers.Clear();
            ModifyDefinitionKeywords(BaseKeywords);

            //Note that normal card destruction clears all triggers and removes from list.
        }


        public override bool AskIfCardContainsKeyword(Card card, string keyword, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            // If the card being queried is this card and we have the copied keyword, return true
            // otherwise let the base method handle the query

            if (card == this.CardWithoutReplacements && CopiedKeywords.Contains(keyword))
                return true;

            return base.AskIfCardContainsKeyword(card, keyword, evenIfUnderCard, evenIfFaceDown);
        }

        public override IEnumerable<string> AskForCardAdditionalKeywords(Card card)
        {
            // If the card being queried is this card and we have a non empty copied keyword list, return it
            // otherwise let the base method handle the return
            //!AllowReflectionSelfModification && 
            if (card == this.CardWithoutReplacements && CopiedKeywords.Any())
            {
                return CopiedKeywords;
            }

            return base.AskForCardAdditionalKeywords(card);
        }

        public override CardSource AskIfCardSourceIsReplaced(CardSource cardSource, GameAction gameAction = null,
            ITrigger trigger = null)
        {
            if (cardSource == null || !cardSource.AllowReplacements
                || cardSource.AssociatedCardSources.All(acs => acs.Card != this.CardWithoutReplacements)
                || !ShouldSwapCardSources(cardSource, trigger)
                || this.CopiedCard == null || !this.CopiedCard.Equals(cardSource.Card))
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

        public override Card AskIfCardIsReplaced(Card card, CardSource cardSource)
        {
            if (cardSource == null || !cardSource.AllowReplacements
                || cardSource.CardController != this || card == CardWithoutReplacements
                || CopiedCard == null)
            {
                return null;
            }

            CardController cardController = cardSource.AssociatedCardSources.Select(cs => cs.CardController)
                .FirstOrDefault(cc => cc.CardWithoutReplacements == this.CopiedCard);

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

        //public override TurnTakerController AskIfTurnTakerControllerIsReplaced(TurnTakerController ttc, CardSource cardSource)
        //{
        //    var copiedCard = CopiedCard;
        //    if (cardSource != null && cardSource.AllowReplacements && copiedCard != null && cardSource.CardController == this)
        //    {
        //        Card card = cardSource.AssociatedCardSources.Select((CardSource cs) => cs.Card).FirstOrDefault((Card a) => a == copiedCard);
        //        if (card != null && ttc.TurnTaker == card.Owner)
        //        {
        //            return base.TurnTakerController;
        //        }
        //    }
        //    return null;
        //}

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
                bool isCardSourceMyCopiedCard = this.CopiedCard == (cardSource.CardController.CardWithoutReplacements);

                bool isAnyCardSourceMyself = cardSources.Any(cs => cs.CardController == this);
                if (isCardSourceMyCopiedCard && isAnyCardSourceMyself)
                {
                    bool otherHasSourceLimitation = cardSource.SourceLimitation != null;
                    CardSource.Limitation? cardSourceLimitation = base.CardSourceLimitation;
                    CardSource.Limitation limitation = CardSource.Limitation.BeforeDestroyed;
                    if (cardSourceLimitation.GetValueOrDefault() == limitation & cardSourceLimitation != null)
                    {
                        thisCardSource.SourceLimitation = CardSource.Limitation.BeforeDestroyed;
                    }
                    else
                    {
                        cardSourceLimitation = base.CardSourceLimitation;
                        limitation = CardSource.Limitation.AfterDestroyed;
                        if (cardSourceLimitation.GetValueOrDefault() == limitation & cardSourceLimitation != null)
                        {
                            thisCardSource.SourceLimitation = CardSource.Limitation.AfterDestroyed;
                        }
                    }
                    bool thisHasSourceLimitation = thisCardSource.SourceLimitation != null;
                    return !otherHasSourceLimitation || !thisHasSourceLimitation || cardSource.SourceLimitation.Value == thisCardSource.SourceLimitation.Value;
                }
            }
            return false;
        }

        /*
        private IEnumerator SetCardSourceLimitationsWhenDestroy(DestroyCardAction dc, SelfDestructTrigger destroyTrigger)
        {
            destroyTrigger.CardSource?.CardController?.SetCardSourceLimitation(this, CardSource.Limitation.WhenDestroyed);

            IEnumerator routine = destroyTrigger.Response(dc);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            destroyTrigger.CardSource?.CardController?.RemoveCardSourceLimitation(this);

            yield break;
        }

        private void CopyWhenDestroyedTriggers(CardController cc)
        {
            //foreach (ITrigger trigger in cc.GetWhenDestroyedTriggers())
            //{
            //    //SelfDestructTrigger destroyTrigger = trigger as SelfDestructTrigger;
            //    //base.AddWhenDestroyedTrigger(dc => this.SetCardSourceLimitationsWhenDestroy(dc, destroyTrigger), 
            //        //destroyTrigger.Types.ToArray(), null, null).CardSource.AddAssociatedCardSource(cc.GetCardSource());
            //}
        }
        */
    }
}
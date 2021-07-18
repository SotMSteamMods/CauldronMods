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
         * Copied cards that are Played Next to a Target (Pins) probally won't work.  Calling the copied cards version of that might
         * work.
         * 
         * Unclear if the implementation correctly captures SelfDestruction/Leaves play type triggers.
         * 
         */
        //==============================================================

        public static readonly string Identifier = "MirrorWraith";

        private const int DamageToDeal = 2;

        private readonly List<ITrigger> _copiedTriggers;

        private static readonly string CopiedCardKey = "MirrorWraithCopy";
        private static readonly IEnumerable<string> BaseKeywords = new[] { "creature" };
        private static bool AllowReflectionSelfModification = true;

        private SelfDestructTrigger _removeCardSourceWhenDestroyedTrigger;

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

            string[] sa;
            if (!card.IsFlipped)
            {
                sa = card.Definition.Body.Select(b => b.Replace("{" + card.Title + "}", replacementTitle).Replace(card.Title, replacementTitle)).ToArray();
            } else
            {
                sa = card.Definition.FlippedBody.Select(b => b.Replace("{" + card.Title + "}", replacementTitle).Replace(card.Title, replacementTitle)).ToArray();

            }
            return "Copied card text: " + string.Join(System.Environment.NewLine, sa);
        }

        public override IEnumerator Play()
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1,
                c => c.IsTarget && c.IsInPlay && !c.IsCharacter
                     && !c.Equals(this.Card), storedResults, cardSource: GetCardSource());

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
                System.Console.WriteLine("**DEBUG** No Results from query.");

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
                if(copiedCard.Identifier == "MirrorWraith")
                {
                    copiedCard = (FindCardController(copiedCard) as MirrorWraithCardController).CopiedCard;
                }
                Journal.RecordCardProperties(Card, CopiedCardKey, copiedCard);

                AddToControllerLists(copiedCard);

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

                var copiedController = FindCardController(copiedCard);
                var destinations = new List<MoveCardDestination>();
                IEnumerator selectLocation = copiedController.DeterminePlayLocation(destinations, true, new List<IDecision>());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(selectLocation);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(selectLocation);
                }

                MoveCardDestination moveTo = destinations.Any() ? destinations.First() : new MoveCardDestination(this.TurnTaker.PlayArea);
                if(moveTo.Location != copiedCard.Owner.PlayArea && moveTo.Location != this.TurnTaker.PlayArea && moveTo.Location.OwnerCard != this.Card)
                {
                    IEnumerator moveCard = GameController.MoveCard(TurnTakerController, this.Card, moveTo.Location, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(moveCard);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(moveCard);
                    }
                }

                // Set card text
                CopyGameText(copiedCard);
                ModifyDefinitionKeywords(BaseKeywords.Concat(copiedCard.IsFlipped ? copiedCard.Definition.FlippedKeywords : copiedCard.Definition.Keywords));

                var messageRoutine = GameController.SendMessageAction($"{Card.Title} copies {copiedCard.Title}.", Priority.High, GetCardSource(), new[] { copiedCard });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(messageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(messageRoutine);
                }

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

        public override void AddLastTriggers()
        {
            Card copiedCard = CopiedCard;
            if(copiedCard is null)
            {
                //this will be handled by the Play()
                return;
            }
            var copiedFromOutOfPlay = false;
            var copiedController = FindCardController(copiedCard);
            if(!copiedCard.IsInPlayAndHasGameText)
            {
                copiedFromOutOfPlay = true;
                copiedController.AddAllTriggers();
            }

            AddToControllerLists(copiedCard);
            CopyGameText(copiedCard);

            if(copiedFromOutOfPlay)
            {
                copiedController.RemoveAllTriggers(false);
            }

        }

        private IEnumerator ReplaceCardSourceWhenDestroyed(DestroyCardAction dc)
        {
            var copiedCard = CopiedCard;
            if(copiedCard != null)
            { 
                CardController cardController = FindCardController(copiedCard);
                CardSource cardSource = GetCardSource();
                cardSource.SourceLimitation = CardSource.Limitation.WhenDestroyed;
                cardController.AddAssociatedCardSource(cardSource);
            }
            yield return null;
        }
        private IEnumerator RemoveCardSourceWhenDestroyed(DestroyCardAction dc)
        {
            var copiedCard = CopiedCard;
            if (copiedCard != null)
            {
                CardController cardController = FindCardController(copiedCard);
                CardSource cardSource = GetCardSource();
                cardSource.SourceLimitation = CardSource.Limitation.WhenDestroyed;
                cardController.RemoveAssociatedCardSource(cardSource);
            }
            yield return null;
        }

        private IEnumerator DupliPlayCopiedCard(Card card)
        {
            var cc = FindCardController(card);
            CardSource source = GetCardSource();
            source.SourceLimitation = CardSource.Limitation.Play;
            cc.AddAssociatedCardSource(source);
            var coroutine = cc.Play();
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

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            var card = CopiedCard;
            //if the card is in play it will do the prevention itself, only if it is out of play do we need to proxy the effect
            if (card != null && !card.IsInPlayAndHasGameText)
            {
                var cc = FindCardController(card);
                return cc.AskIfActionCanBePerformed(gameAction);
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //this isn't called unless the card is in the MakeIndestrucible list

            var copiedCard = CopiedCard;
            if (copiedCard != null)
            {
                var cc = FindCardController(copiedCard);
                if (card == Card)
                {
                    //asking for a friend bro.  This is a mixed bag.  If the card makes itself immune, this returns true
                    //but if it's looking at state on the itself/card this fails.
                    //There's probally someway
                    return cc.AskIfCardIsIndestructible(copiedCard);
                }
                else
                {
                    //proxy to the other cardController
                    return cc.AskIfCardIsIndestructible(card);
                }
            }

            return base.AskIfCardIsIndestructible(card);
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            //only called if CC is in associated list
            var card = CopiedCard;
            if (card != null)
            {
                var cc = FindCardController(card);
                return cc.AskIfIncreasingCurrentPhaseActionCount();
            }
            return base.AskIfIncreasingCurrentPhaseActionCount();
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
            if (GameController.IsInCardControllerList(Card, CardControllerListType.MakesIndestructible))
            {
                base.RemoveThisCardControllerFromList(CardControllerListType.MakesIndestructible);
            }
            if (GameController.IsInCardControllerList(Card, CardControllerListType.IncreasePhaseActionCount))
            {
                base.RemoveThisCardControllerFromList(CardControllerListType.IncreasePhaseActionCount);
            }

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
        private void CopyGameText(Card sourceCard)
        {
            IEnumerable<ITrigger> triggers =
                FindTriggersWhere(t => t.CardSource.CardController.CardWithoutReplacements == sourceCard);
            var hasWhenDestroyedTriggers = false;
            foreach (ITrigger trigger in triggers)
            {
                if (trigger.IsStatusEffect)
                {
                    continue;
                }

                if (trigger is SelfDestructTrigger sdt)
                {
                    hasWhenDestroyedTriggers = true;
                    CardController originalController = GameController.FindCardController(sourceCard);
                    SelfDestructTrigger destroyTrigger = trigger as SelfDestructTrigger;
                    base.AddWhenDestroyedTrigger(dc => this.SetCardSourceLimitationsWhenDestroy(dc, destroyTrigger), 
                         destroyTrigger.Types.ToArray(), null, null).CardSource.AddAssociatedCardSource(originalController.GetCardSource());
                }
                else
                {
                    ITrigger clonedTrigger = (ITrigger)trigger.Clone();
                    clonedTrigger.CardSource = base.FindCardController(sourceCard).GetCardSource();
                    clonedTrigger.CardSource.AddAssociatedCardSource(base.GetCardSource());
                    clonedTrigger.SetCopyingCardController(this);

                    base.AddTrigger(clonedTrigger);
                    this._copiedTriggers.Add(clonedTrigger);
                }
            }

            if(hasWhenDestroyedTriggers)
            {
                AddWhenDestroyedTrigger(ReplaceCardSourceWhenDestroyed, TriggerType.Hidden);
                _removeCardSourceWhenDestroyedTrigger = AddWhenDestroyedTrigger(RemoveCardSourceWhenDestroyed, TriggerType.HiddenLast);
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

        private void AddToControllerLists(Card copiedCard)
        {
            // Identify this card controller as one who can modify keyword query answers
            base.AddThisCardControllerToList(CardControllerListType.ModifiesKeywords);

            // Identify this card controller as one who can modify card query answers
            base.AddThisCardControllerToList(CardControllerListType.ReplacesCards);

            // Identify this card controller as one who can modify card source query answers
            base.AddThisCardControllerToList(CardControllerListType.ReplacesCardSource);

            // Identify this card controller as one who can potentially be indestructible
            if (GameController.IsInCardControllerList(copiedCard, CardControllerListType.MakesIndestructible))
            {
                base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            }

            //identify this card as one that could potentially increase phase actions
            if (GameController.IsInCardControllerList(copiedCard, CardControllerListType.IncreasePhaseActionCount))
            {
                base.AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
            }
        }

        
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
        /*
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
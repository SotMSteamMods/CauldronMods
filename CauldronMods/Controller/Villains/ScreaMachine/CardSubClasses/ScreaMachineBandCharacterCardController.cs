using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineBandCharacterCardController : ScreaMachineUtilityCharacterCardController
    {
        public ScreaMachineBandmate.Value Member { get; }
        private readonly string _memberKeyword;
        private readonly string _memberAbilityKey;
        private int NumberOfCardsNeededToFlip => Game.IsChallenge ? 2 : 3;

        protected ScreaMachineBandCharacterCardController(Card card, TurnTakerController turnTakerController, ScreaMachineBandmate.Value member) : base(card, turnTakerController)
        {
            Member = member;
            _memberAbilityKey = member.GetAbilityKey();
            _memberKeyword = member.GetKeyword();

            SpecialStringMaker.ShowSpecialString(() => BuildNumberOfMemberCardsSpecialString());
        }


        protected abstract string AbilityDescription { get; }

        protected abstract IEnumerator ActivateBandAbility();
        public override IEnumerator ActivateAbilityEx(CardDefinition.ActivatableAbilityDefinition definition)
        {
            if (!Card.IsFlipped && definition.Name == _memberAbilityKey)
            {
                return ActivateBandAbility();
            }
            return base.ActivateAbilityEx(definition);
        }

        public override IEnumerable<ActivatableAbility> GetActivatableAbilities(string key = null, TurnTakerController activatingTurnTaker = null)
        {
            var abilities = new List<ActivatableAbility>();
            if (!Card.IsFlipped && (key is null || key == _memberAbilityKey))
            {
                var abilityDef = CustomAbilityDefinition();
                abilities.Add(new ActivatableAbility(TurnTakerController, this, abilityDef, ActivateAbilityEx(abilityDef), 0, null, activatingTurnTaker, GetCardSource()));
            }
            return abilities;
        }

        protected CardDefinition.ActivatableAbilityDefinition CustomAbilityDefinition()
        {
            var activatable = new CardDefinition.ActivatableAbilityDefinition();
            activatable.Name = _memberAbilityKey;
            activatable.Text = AbilityDescription;
            activatable.Number = 0;
            return activatable;
        }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger<MoveCardAction>(mca => mca.Destination.IsInPlayAndNotUnderCard && mca.WasCardMoved && FlipCriteria(mca.CardToMove), ca => base.FlipThisCharacterCardResponse(ca), TriggerType.FlipCard, TriggerTiming.After));
                AddSideTrigger(AddTrigger<PlayCardAction>(pca => pca.WasCardPlayed && FlipCriteria(pca.CardToPlay), ca => FlipBandmateResponse(ca), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddFlippedSideTriggers();
            }
            AddSideTrigger(AddTrigger((DestroyCardAction destroyCard) => destroyCard.CardToDestroy == this, CannotBeMovedResponse, TriggerType.Hidden, TriggerTiming.Before));
            AddAfterLeavesPlayAction(ga => CheckForDefeat(ga), TriggerType.GameOver);
        }

        private IEnumerator CheckForDefeat(GameAction ga)
        {
            IEnumerator coroutine;


            if (TurnTakerController.CharacterCards.All(c => c.IsOutOfGame))
            {
                coroutine =  DefeatedResponse(ga);
            }
            else
            {
                coroutine = GameController.SendMessageAction($"[i]{Card.Title}[/i] has to leave the stage, but.....\n[b]THE SHOW MUST GO ON![b]", Priority.Medium, GetCardSource());
            }
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool FlipCriteria(Card card)
        {
            if (!card.IsVillain)
                return false;

            var cc = FindCardController(card);
            if (cc is ScreaMachineBandCardController bandCC && bandCC.Member == this.Member)
            {
                var cards = GameController.FindCardsWhere(c => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain(_memberKeyword, true, true), true, GetCardSource()).ToList();
                Console.WriteLine($"DEBUG - {Card.Title} has {cards.Count} {_memberKeyword} cards in play.");
                return cards.Count >= NumberOfCardsNeededToFlip;
            }

            return false;
        }

        protected int GetNumberOfMemberKeywordInPlay()
        {
            var cards = GameController.FindCardsWhere(c => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain(_memberKeyword, true, true), true, GetCardSource()).ToList();
            return cards.Count;
        }

        private string BuildNumberOfMemberCardsSpecialString()
        {
            int num = GetNumberOfMemberKeywordInPlay();
            string memberSpecial = "There ";
            if(num == 1)
            {
                memberSpecial += $"is 1 {_memberKeyword} card ";
            } else
            {
                memberSpecial += $"are {num} {_memberKeyword} cards ";
            }
            memberSpecial += "in play.";

            return memberSpecial;
        }
        protected abstract string UltimateFormMessage { get; }

        protected IEnumerator FlipBandmateResponse(GameAction action)
        {
            var coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine;

            coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            string message = "[b]" + Card.Title + " - " + TurnTaker.NameRespectingVariant + "[/b]".PadRight(UltimateFormMessage.Length) + "\n[i]\u201C" + UltimateFormMessage + "\u201D[/i]";

            coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), showCardSource: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected abstract void AddFlippedSideTriggers();

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(TurnTaker.OutOfGame);
        }
    }
}

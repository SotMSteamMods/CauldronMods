using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.TheMistressOfFate
{
    public class SeeThePatternCardController : TheMistressOfFateUtilityCardController
    {
        private GameAction _storedFlip;
        public SeeThePatternCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals each hero {H} psychic damage and {H} melee damage.",
            var damages = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, H, DamageType.Psychic),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, H, DamageType.Melee)
            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damages, (Card c) =>  IsHeroCharacterCard(c));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"When {TheMistressOfFate} flips, each player may move 1 card from their trash to their hand. If any cards were moved this way, destroy this card."
            AddTrigger((FlipCardAction fc) => fc.CardToFlip.Card == CharacterCard && NoStoredFlipExists(fc), fc => DoNothing(), TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger((FlipCardAction fc) => fc.CardToFlip.Card == CharacterCard && NoStoredFlipExists(fc), MayReturnCardsResponse, TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator MayReturnCardsResponse(FlipCardAction fc)
        {
            var moveStorage = new List<MoveCardAction>();
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                                                        new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame),
                                                        SelectionType.MoveCardToHandFromTrash,
                                                        (TurnTaker tt) => ReturnCardToHand(tt, moveStorage),
                                                        allowAutoDecide: true,
                                                        requiredDecisions: 0,
                                                        cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if(moveStorage.Any((MoveCardAction mc) => mc.WasCardMoved))
            {
                coroutine = DestroyThisCardResponse(fc);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            _storedFlip = null;
            yield break;
        }
        private IEnumerator ReturnCardToHand(TurnTaker hero, List<MoveCardAction> storedResults)
        {
            var selectCardInTrash = new SelectCardDecision(GameController, FindHeroTurnTakerController(hero.ToHero()), SelectionType.MoveCardToHandFromTrash, hero.Trash.Cards, isOptional: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardInTrash, (SelectCardDecision scd) => GameController.MoveCard(DecisionMaker, scd.SelectedCard, hero.ToHero().Hand, storedResults: storedResults, cardSource: GetCardSource()));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
        private bool NoStoredFlipExists(FlipCardAction fc)
        {
            //Log.Debug("Checking if there is a pending flip trigger...");
            if(_storedFlip == null || _storedFlip == fc)
            {
                //Log.Debug("There is not");
                _storedFlip = fc;
                return true;
            }
            //Log.Debug("There is.");
            return false;
        }
    }
}

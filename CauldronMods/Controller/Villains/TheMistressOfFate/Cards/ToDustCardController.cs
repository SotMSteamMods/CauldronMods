using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ToDustCardController : TheMistressOfFateUtilityCardController
    {
        public ToDustCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            RemoveTemporaryTriggers();

            //"{TheMistressOfFate} deals the hero target with the second highest HP 15 projectile damage.",
            var storedTarget = new List<Card>();
            var previewDamage = new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), null, 15, DamageType.Projectile);
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(2, (Card c) => IsHero(c), storedTarget, dealDamageInfo: new List<DealDamageAction> { previewDamage }, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            var hero = storedTarget.FirstOrDefault();
            if (hero == null)
            {
                yield break;
            }

            //"That hero may shuffle 10 cards from their trash into their deck. If they do, they may redirect that damage to another target."
            var didShuffle = new List<bool>();
            coroutine = ShuffleCardsIntoDeck(hero, didShuffle);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(didShuffle.FirstOrDefault())
            {
                AddToTemporaryTriggerList(AddTrigger((DealDamageAction dd) => dd.CardSource.Card == this.Card, dd => RedirectDamageResponse(dd, hero), TriggerType.RedirectDamage, TriggerTiming.Before));
            }

            coroutine = DealDamage(CharacterCard, hero, 15, DamageType.Projectile);
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

        private IEnumerator ShuffleCardsIntoDeck(Card hero, List<bool> successStorage)
        {
            var heroTTC = FindHeroTurnTakerController(hero.Owner.ToHero());
            IEnumerator coroutine;
            if(heroTTC.TurnTaker.Trash.NumberOfCards < 10)
            {
                coroutine = GameController.SendMessageAction($"{heroTTC.TurnTaker.Trash.GetFriendlyName()} does not have 10 cards to shuffle in!", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                var storedYesNo = new List<YesNoCardDecision>();
                coroutine = GameController.MakeYesNoCardDecision(heroTTC, SelectionType.ShuffleCardFromTrashIntoDeck, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(DidPlayerAnswerYes(storedYesNo))
                {
                    var storedMove = new List<MoveCardAction>();
                    var deckDestination = new List<MoveCardDestination> { new MoveCardDestination(heroTTC.TurnTaker.Deck)};
                    coroutine = GameController.SelectCardsFromLocationAndMoveThem(heroTTC, heroTTC.TurnTaker.Trash, 10, 10, new LinqCardCriteria(), deckDestination, selectionType: SelectionType.ShuffleCardIntoDeck, cardSource: GetCardSource(), storedResultsMove: storedMove, allowAutoDecide: true);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    if(DidMoveCard(storedMove))
                    {
                        successStorage.Add(true);
                    }
                }
            }

            yield break;
        }

        private IEnumerator RedirectDamageResponse(DealDamageAction dd, Card hero)
        {
            IEnumerator coroutine = GameController.SelectTargetAndRedirectDamage(FindHeroTurnTakerController(hero.Owner.ToHero()), (Card c) => true, dd, true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}

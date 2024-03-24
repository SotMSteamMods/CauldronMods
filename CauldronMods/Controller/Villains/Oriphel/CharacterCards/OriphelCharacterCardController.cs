using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelCharacterCardController : VillainCharacterCardController
    {
        public OriphelCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsRelic, "relic")).Condition = () => Card.Title == "Oriphel";
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: 2).Condition = () => Card.Title == "Oriphel";
        }

        private int NumberOfRelicsInTrashToFlip => Game.IsChallenge ? 3 : 2;

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                //"Whenever a villain relic enters play, play the top card of the villain deck.",
                AddSideTrigger(AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && cep.CardEnteringPlay.IsRelic && IsVillain(cep.CardEnteringPlay),
                                                PlayTheTopCardOfTheVillainDeckResponse,
                                                TriggerType.PlayCard,
                                                TriggerTiming.After));
                //"Whenever a villain ongoing card enters play, destroy it and play the top card of the villain deck."
                AddSideTrigger(AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && IsOngoing(cep.CardEnteringPlay) && IsVillain(cep.CardEnteringPlay),
                                DestroyOngoingAndPlayCardResponse,
                                new TriggerType[] { TriggerType.PlayCard, TriggerType.DestroyCard },
                                TriggerTiming.After));
                if (Game.IsAdvanced)
                {
                    //"Increase damage dealt by villain targets by 1.",
                    AddSideTrigger(AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && IsVillainTarget(dd.DamageSource.Card), 1));
                }
            }
            else
            {
                //"Reduce damage dealt to {Oriphel} by 1.",
                if (!Game.IsAdvanced)
                {
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c == this.Card, 1));
                }
                else //Game.IsAdvanced
                {
                    //"Reduce damage dealt to {Oriphel} by 1.", X 2
                    AddSideTrigger(AddReduceDamageTrigger((Card c) => c == this.Card, 2));
                }

                //"At the end of the villain turn, {Oriphel} deals the 2 hero targets with the highest HP {H - 1} infernal damage each.",
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(this.TurnTaker, this.Card, (Card c) => IsHeroTarget(c), TargetType.HighestHP, H - 1, DamageType.Infernal, numberOfTargets: 2));

                //"When there are 2 villain relics in the villain trash, flip {Oriphel}'s villain character cards."
                AddSideTrigger(AddTrigger<GameAction>(CheckCardsInTrashCriteria, FlipThisCharacterCardResponse, TriggerType.FlipCard, TriggerTiming.After));
            }

            if(Game.IsChallenge)
            {
                //Reduce damage dealt to villain relics by 2.
                AddSideTrigger(AddReduceDamageTrigger((Card c) => IsVillain(c) && c.IsRelic, 2));
            }

            AddDefeatedIfDestroyedTriggers();
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            RemoveSideTriggers();
            if (this.Card.IsFlipped)
            {
                //"When {Oriphel} is flipped to this side, shuffle all relics in the villain trash into the villain deck.",
                var relicsInTrash = FindCardsWhere((Card c) => c.Location == TurnTaker.Trash && c.IsRelic, true, GetCardSource());
                IEnumerator coroutine = GameController.ShuffleCardsIntoLocation(DecisionMaker, relicsInTrash, TurnTaker.Deck, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            AddSideTriggers();
            yield break;
        }

        private bool CheckCardsInTrashCriteria(GameAction ga)
        {
            if (ga is MoveCardAction || ga is BulkMoveCardsAction || ga is DestroyCardAction)
            {
                return FindCardsWhere((Card c) => c.Location == TurnTaker.Trash && c.IsRelic, true, GetCardSource()).Count() >= NumberOfRelicsInTrashToFlip;
            }
            return false;
        }
        private IEnumerator DestroyOngoingAndPlayCardResponse(CardEntersPlayAction cep)
        {
            IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, cep.CardEnteringPlay, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = PlayTheTopCardOfTheVillainDeckResponse(cep);
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
    }
}
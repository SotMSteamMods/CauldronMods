using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.TheKnight
{
    public class WastelandRoninTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public WastelandRoninTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private Card _youngKnight = null;
        private Card _oldKnight = null;

        private Card youngKnight
        {
            get
            {
                if (_youngKnight == null)
                {
                    _youngKnight = GameController.FindCard("TheYoungKnightCharacter");
                }
                return _youngKnight;
            }
        }
        private Card oldKnight
        {
            get
            {
                if (_oldKnight == null)
                {
                    _oldKnight = GameController.FindCard("TheOldKnightCharacter");
                }
                return _oldKnight;
            }
        }
        public override void AddStartOfGameTriggers()
        {
            if (Card.IsIncapacitatedOrOutOfGame)
            {
                return;
            }

            if (HeroTurnTakerController is TheKnightTurnTakerController knightTTC)
            {
                var cards = knightTTC.ManageCharactersOffToTheSide(false);

                _youngKnight = cards.Where((Card c) => c.Identifier == "TheYoungKnightCharacter").FirstOrDefault();
                _oldKnight = cards.Where((Card c) => c.Identifier == "TheOldKnightCharacter").FirstOrDefault();


                //"If you have no hero character targets in play, flip this card.",
                //"When 1 of your equipment cards enter play, put it next to 1 of your active knights.",
                //"When your cards refer to “The Knight”, choose 1 of your active knights. For equipment cards, you must choose the knight they are next to. Stalwart Shield does not reduce damage to the other knight's equipment targets.",

                //"Whenever an equipment enters play next to The Young Knight, she deals 1 target 1 toxic damage.",
                AddTrigger((CardEntersPlayAction cep) => IsEquipment(cep.CardEnteringPlay) && GetKnightCardUser(cep.CardEnteringPlay) == youngKnight, YoungKnightDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
                //"Whenever an equipment card enters play next to The Old Knight, draw a card."
                AddTrigger((CardEntersPlayAction cep) => IsEquipment(cep.CardEnteringPlay) && GetKnightCardUser(cep.CardEnteringPlay) == oldKnight, OldKnightDrawResponse, TriggerType.DealDamage, TriggerTiming.After);
            }
        }

        private IEnumerator YoungKnightDamageResponse(CardEntersPlayAction cep)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, youngKnight), 1, DamageType.Toxic, 1, false, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
        private IEnumerator OldKnightDrawResponse(CardEntersPlayAction cep)
        {
            IEnumerator coroutine = DrawCard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger(FlipCriteria, (GameAction ga) => base.GameController.FlipCard(FindCardController(base.Card), cardSource: GetCardSource()), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddSideTriggers(AddTargetEntersPlayTrigger((Card c) => base.Card.IsFlipped && base.CharacterCards.Contains(c), (Card c) => base.GameController.FlipCard(FindCardController(base.Card), treatAsPlayed: false, treatAsPutIntoPlay: false, null, null, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, isConditional: false, outOfPlayTrigger: true));
            }
        }

        private bool FlipCriteria(GameAction ga)
        {
            return((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !base.Card.IsFlipped && FindCardsWhere((Card c) => c.Owner == base.TurnTaker &&  IsHeroCharacterCard(c) && c.IsActive && c != base.Card).Count() == 0);
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            RemoveAllTriggers();
            AddSideTriggers();
            yield return null;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One hero deals 1 target 1 projectile damage.",
                        coroutine = GameController.SelectHeroToSelectTargetAndDealDamage(DecisionMaker, 1, DamageType.Projectile, optionalDealDamage: false, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"One target regains 1 HP.",
                        coroutine = GameController.SelectAndGainHP(DecisionMaker, 1, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"One player may discard a card to play 2 cards now."
                        var storedHero = new List<SelectTurnTakerDecision> { };
                        var storedDiscard = new List<DiscardCardAction> { };
                        coroutine = GameController.SelectHeroToDiscardCard(DecisionMaker, storedResultsTurnTaker: storedHero, storedResultsDiscard: storedDiscard, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if(DidDiscardCards(storedDiscard) && DidSelectTurnTaker(storedHero))
                        {
                            var hero = FindHeroTurnTakerController(storedHero.FirstOrDefault().SelectedTurnTaker.ToHero());
                            coroutine = GameController.SelectAndPlayCardsFromHand(hero, 2, false, 2, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }
    }
}
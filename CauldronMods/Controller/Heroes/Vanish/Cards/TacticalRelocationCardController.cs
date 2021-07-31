using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class TacticalRelocationCardController : CardController
    {
        public TacticalRelocationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var query = GameController.HeroTurnTakerControllers.Select(httc => httc.HeroTurnTaker.Trash)
                                                               .Where(loc => loc.Cards.Any(IsEquipmentOrOngoing));
            var ss = SpecialStringMaker.ShowNumberOfCardsAtLocations(() => query, new LinqCardCriteria(c => IsEquipmentOrOngoing(c) && c.IsInTrash, "equipment or ongoing"));
            ss.Condition = () => GameController.HeroTurnTakerControllers.Any(httc => httc.HeroTurnTaker.Trash.Cards.Any(IsEquipmentOrOngoing));

            ss = SpecialStringMaker.ShowSpecialString(() => "No hero's Trash has any equipment or ongoing cards");
            ss.Condition = () => GameController.HeroTurnTakerControllers.All(httc => !httc.HeroTurnTaker.Trash.Cards.Any(IsEquipmentOrOngoing));
        }

        private bool IsEquipmentOrOngoing(Card c)
        {
            return c.IsOngoing || IsEquipment(c);
        }

        public override IEnumerator Play()
        {
            List<HeroTurnTaker> selected = new List<HeroTurnTaker>();
            List<HeroTurnTaker> damaged = new List<HeroTurnTaker>();
            bool done = false;
            IEnumerator coroutine;

            Func<DealDamageAction, IEnumerator> addDamagedToList = delegate (DealDamageAction dda)
            {
                if(dda.DidDealDamage && dda.Target.IsHeroCharacterCard && dda.Target.Owner.IsHero)
                {
                    damaged.Add(dda.Target.Owner.ToHero());
                }
                return DoNothing();
            };
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Energy, null, false, 0, additionalCriteria: c => c.IsHeroCharacterCard, addStatusEffect: addDamagedToList, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //then allow all the damaged hero's to move a card
            var damagedDecision = new SelectTurnTakersDecision(GameController, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt is HeroTurnTaker htt && damaged.Contains(htt), "heroes"), SelectionType.Custom,
                    requiredDecisions: null,
                    allowAutoDecide: true,
                    cardSource: GetCardSource());
            coroutine = GameController.SelectTurnTakersAndDoAction(damagedDecision, DamagedTurnTakerResponse, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //and allow all the other hero's to draw and discard

            var sttd = new SelectTurnTakersDecision(GameController, null, new LinqTurnTakerCriteria((TurnTaker tt) => tt is HeroTurnTaker htt && !damaged.Contains(htt), "heroes"), SelectionType.DiscardAndDrawCard,
                                requiredDecisions: null,
                                allowAutoDecide: true,
                                cardSource: GetCardSource());
            coroutine = GameController.SelectTurnTakersAndDoAction(sttd, DrawThenDiscardResponse, cardSource: GetCardSource());
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

        private IEnumerator DamagedTurnTakerResponse(TurnTaker tt)
        {
            IEnumerator coroutine = GameController.SelectCardFromLocationAndMoveIt(FindHeroTurnTakerController(tt.ToHero()), tt.Trash, new LinqCardCriteria(c => c.IsOngoing || IsEquipment(c), "ongoing or equipment"), new List<MoveCardDestination> { new MoveCardDestination(tt.PlayArea) }, isPutIntoPlay: true, optional: true, cardSource: GetCardSource());
            return coroutine;
        }

        private IEnumerator DrawThenDiscardResponse(TurnTaker tt)
        {
            var result = new List<DrawCardAction>();
            var htt = tt.ToHero();
            var httc = GameController.FindHeroTurnTakerController(htt);

            var yesNoStorage = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(httc, SelectionType.Custom, Card, storedResults: yesNoStorage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidPlayerAnswerYes(yesNoStorage))
            {
                yield return null;
            }

            coroutine = DrawCard(htt, optional: true, cardsDrawn: result);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = SelectAndDiscardCards(httc, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield return null;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if(decision is YesNoCardDecision)
            {
                return new CustomDecisionText(
                    "Do you want to draw a card and discard a card?",
                    "{0} is choosing whether to draw and discard...",
                    "Vote for whether {0} should draw and discard.",
                    "whether to draw and discard"
                );
            }
            return new CustomDecisionText(
                "Select a hero to move an equipment or ongoing card from their trash into play.",
                "selecting a hero to put a card into play...",
                "Vote for who should move an equipment or ongoing card from their trash into play.",
                "card to move from trash into play"
            );
        }
    }
}
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
            SpecialStringMaker.ShowNumberOfCardsAtLocations(
                    () => GameController.HeroTurnTakerControllers.Select(httc => httc.HeroTurnTaker.Trash),
                    new LinqCardCriteria(c => c.IsOngoing && IsEquipment(c) && c.IsInTrash, "equipment or ongoings in trash")
                );
        }

        public override IEnumerator Play()
        {
            List<HeroTurnTaker> selected = new List<HeroTurnTaker>();
            List<HeroTurnTaker> damaged = new List<HeroTurnTaker>();
            bool done = false;
            IEnumerator coroutine;

            var choices = GameController.GetAllCards().Where(c => c.IsHeroCharacterCard && c.IsInPlay && !c.IsIncapacitatedOrOutOfGame && !selected.Contains(c.Owner.ToHero())).ToArray();
            while (choices.Length > 0 && !done)
            {
                var dda = new DealDamageAction(GameController, new DamageSource(GameController, CharacterCard), null, 3, DamageType.Energy);
                var scd = new SelectCardDecision(GameController, DecisionMaker, SelectionType.DealDamage, choices,
                    isOptional: true,
                    allowAutoDecide: true,
                    dealDamageInfo: new[] { dda },
                    cardSource: GetCardSource());

                coroutine = GameController.MakeDecisionAction(scd);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (scd.SelectedCard == null)
                {
                    //if we made no selection, exit our loop
                    done = true;
                }
                else
                {
                    //otherwise attempt to damage the selected card, recording the owner, then regenerate the list of choices ignoring those already picked.
                    var owner = scd.SelectedCard.Owner.ToHero();
                    selected.Add(owner);
                    dda.Target = scd.SelectedCard;

                    coroutine = GameController.DoAction(dda);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (dda.DidDealDamage)
                    {
                        damaged.Add(owner);
                    }

                    choices = GameController.GetAllCards().Where(c => c.IsHeroCharacterCard && c.IsInPlay && !c.IsIncapacitatedOrOutOfGame && !selected.Contains(c.Owner.ToHero())).ToArray();
                }
            }

            //then allow all the damaged hero's to move a card
            coroutine = GameController.SelectCardAndDoAction_ManyPlayers(httc => damaged.Contains(httc.HeroTurnTaker), WasDamagedSelection, WasDamagedReponse, cardSource: GetCardSource());
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
                                isOptional: true,
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

        private SelectCardDecision WasDamagedSelection(HeroTurnTakerController httc)
        {
            return new SelectCardDecision(httc.GameController, httc, SelectionType.MoveCardToPlayArea, httc.GetCardsAtLocation(httc.HeroTurnTaker.Trash),
                            isOptional: true,
                            allowAutoDecide: true,
                            additionalCriteria: c => IsEquipment(c) || c.IsOngoing,
                            cardSource: GetCardSource());
        }

        private IEnumerator WasDamagedReponse(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                return GameController.MoveCard(scd.HeroTurnTakerController, scd.SelectedCard, scd.SelectedCard.Owner.PlayArea, isPutIntoPlay: true, showMessage: true, decisionSources: new[] { scd }, cardSource: GetCardSource());
            }
            else
            {
                return DoNothing();
            }
        }

        private IEnumerator DrawThenDiscardResponse(TurnTaker tt)
        {
            var result = new List<DrawCardAction>();
            var htt = tt.ToHero();
            var httc = GameController.FindHeroTurnTakerController(htt);
            var coroutine = DrawCard(htt, optional: true, cardsDrawn: result);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (GetNumberOfCardsDrawn(result) > 0)
            {
                coroutine = SelectAndDiscardCards(httc, 1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}
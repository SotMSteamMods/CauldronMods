using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Drift
{
    public class DriftTurnTakerController : HeroTurnTakerController
    {
        public DriftTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        protected const string ShiftTrack = "ShiftTrack";
        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        //public override IEnumerator StartGame()
        //{
        //    IEnumerator coroutine = SetupDrift();
        //    if (base.UseUnityCoroutines)
        //    {
        //        yield return base.GameController.StartCoroutine(coroutine);
        //    }
        //    else
        //    {
        //        base.GameController.ExhaustCoroutine(coroutine);
        //    }
        //}

        public string[] availablePromos = new string[] {  };
        public Dictionary<string, string[]> nonDriftPromos = new Dictionary<string, string[]>()
        {
            { "Necro", new string[] {"LastOfTheForgottenOrderNecro"} }
        };

        public bool ArePromosSetup { get; set; } = false;

        public IEnumerator SetupDrift()
        {
            string promoIdentifier = Base;
            if (base.CharacterCardController is DualDriftCharacterCardController)
            {
                promoIdentifier = Dual;
            }
            if (base.CharacterCardController is ThroughTheBreachDriftCharacterCardController)
            {
                promoIdentifier = ThroughTheBreach;
            }

            string[] tracks = new string[] {
                promoIdentifier + ShiftTrack + 1,
                promoIdentifier + ShiftTrack + 2,
                promoIdentifier + ShiftTrack + 3,
                promoIdentifier + ShiftTrack + 4,
            };

            //At the start of the game, after drawing your cards, place a token on 1 of the 4 spaces of the shift track.
            List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this, SelectionType.AddTokens, new LinqCardCriteria((Card c) => c.SharedIdentifier == ShiftTrack && tracks.Contains(c.Identifier), "Shift Track Position"), cardDecisions, false, includeRealCardsOnly: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            
            if(cardDecisions.Any())
            {
                Card selectedTrack = cardDecisions.FirstOrDefault().SelectedCard;
                PlayCardAction playShiftAction = new PlayCardAction(GameController, this, selectedTrack, isPutIntoPlay: true, responsibleTurnTaker: TurnTaker, null, null, null, false, canBeCancelled: false);
                playShiftAction.AllowTriggersToRespond = false;
                coroutine = GameController.DoAction(playShiftAction);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                CardController selectTrackController = base.FindCardController(selectedTrack);
                int tokensToAdd = 0;
                if (selectTrackController is BaseShiftTrack1CardController || selectTrackController is DualShiftTrack1CardController || selectTrackController is ThroughTheBreachShiftTrack1CardController)
                {
                    tokensToAdd = 1;
                }
                else if (selectTrackController is BaseShiftTrack2CardController || selectTrackController is DualShiftTrack2CardController || selectTrackController is ThroughTheBreachShiftTrack2CardController)
                {
                    tokensToAdd = 2;
                }
                else if (selectTrackController is BaseShiftTrack3CardController || selectTrackController is DualShiftTrack3CardController || selectTrackController is ThroughTheBreachShiftTrack3CardController)
                {
                    tokensToAdd = 3;
                }
                else if (selectTrackController is BaseShiftTrack4CardController || selectTrackController is DualShiftTrack4CardController || selectTrackController is ThroughTheBreachShiftTrack4CardController)
                {
                    tokensToAdd = 4;
                }

                coroutine = base.GameController.AddTokensToPool(selectedTrack.FindTokenPool("ShiftPool"), tokensToAdd, new CardSource(base.CharacterCardController));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            

            if(!(base.CharacterCardController is DualDriftSubCharacterCardController))
            {
                coroutine = GameController.BulkMoveCards(this, FindCardsWhere((Card c) => FindCardController(c) is DualDriftSubCharacterCardController), TurnTaker.InTheBox);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        public override bool IsIncapacitated
        {
            get
            {
                if (base.TurnTaker.Identifier == "Drift")
                {
                    if (this.GetActiveCharacterCard().IsFlipped)
                    {
                        return true;
                    }
                }
                return base.IsIncapacitated;
            }
        }

        public override bool IsIncapacitatedOrOutOfGame
        {
            get
            {
                if (!IsIncapacitated)
                {
                    return IncapacitationCardController.Card.IsOutOfGame;
                }
                return true;
            }
        }

        public Card GetActiveCharacterCard()
        {
            return base.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == this.TurnTaker).FirstOrDefault();
        }
    }
}
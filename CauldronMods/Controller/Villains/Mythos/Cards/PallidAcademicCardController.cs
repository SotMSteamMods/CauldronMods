﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class PallidAcademicCardController : MythosUtilityCardController
    {
        public PallidAcademicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override string DeckIdentifier
        {
            get
            {
                return MythosEyeDeckIdentifier;
            }
        }

        public override IEnumerator Play()
        {
            //When this card enters play, put all ongoing cards in the villain trash into play.
            IEnumerator coroutine = base.PlayCardsFromLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.IsOngoing));
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
            //When a hero target is dealt damage by another hero target:
            //--{MythosDanger} Play the top card of the villain deck.
            //--{MythosMadness} Move the bottom card of a deck to the top of that deck.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsHero && action.Target.IsHero, this.DangerMadnessResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.MoveCard }, TriggerTiming.After);
        }

        private IEnumerator DangerMadnessResponse(DealDamageAction action)
        {
            IEnumerator coroutine;
            if (base.IsTopCardMatching(MythosFearDeckIdentifier))
            {
                //--{MythosDanger} Play the top card of the villain deck.
                coroutine = base.PlayTheTopCardOfTheVillainDeckResponse(action);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.IsTopCardMatching(MythosMindDeckIdentifier))
            {
                //--{MythosMadness} Move the bottom card of a deck to the top of that deck.
                List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                coroutine = base.GameController.SelectADeck(base.DecisionMaker, SelectionType.MoveCard, (Location deck) => deck.IsDeck, storedResults, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (storedResults.Any())
                {
                    Location selectedDeck = storedResults.FirstOrDefault().SelectedLocation.Location;
                    coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedDeck.BottomCard, selectedDeck, cardSource: base.GetCardSource());
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
            yield break;
        }
    }
}

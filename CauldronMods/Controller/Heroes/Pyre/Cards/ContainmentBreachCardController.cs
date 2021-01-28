﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ContainmentBreachCardController : PyreUtilityCardController
    {
        private List<Guid> _recentIrradiatedPlays;
        private List<Guid> RecentIrradiatedCardPlays
        {
            get
            {
                if (_recentIrradiatedPlays == null)
                {
                    _recentIrradiatedPlays = new List<Guid>();
                }
                return _recentIrradiatedPlays;
            }
        }
        public ContainmentBreachCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            //"Whenever a player plays a {PyreIrradiate} card, increase energy damage dealt by {Pyre} by 1 until the end of your turn. Then shuffle a Cascade card from your trash into your deck.",
            AddTrigger((PlayCardAction pc) => IsIrradiated(pc.CardToPlay) && !pc.IsPutIntoPlay, NoteIrradiatedPlay, TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger((PlayCardAction pc) => RecentIrradiatedCardPlays.Contains(pc.InstanceIdentifier), IrradiatedPlayResponse, TriggerType.DealDamage, TriggerTiming.After, requireActionSuccess: false);

        }
        private IEnumerator NoteIrradiatedPlay(PlayCardAction pc)
        {
            RecentIrradiatedCardPlays.Add(pc.InstanceIdentifier);
            yield return null;
            yield break;
        }
        private IEnumerator IrradiatedPlayResponse(PlayCardAction pc)
        {
            RecentIrradiatedCardPlays.Remove(pc.InstanceIdentifier);
            if (pc.IsSuccessful)
            {
                //Increase energy damage dealt by {Pyre} by 1 until the end of your turn. 
                var effect = new IncreaseDamageStatusEffect(1);
                effect.UntilEndOfPhase(TurnTaker, Phase.End);
                effect.SourceCriteria.IsSpecificCard = CharacterCard;
                effect.DamageTypeCriteria.AddType(DamageType.Energy);
                effect.CreateImplicitExpiryConditions();
                effect.CardSource = Card;

                IEnumerator coroutine = AddStatusEffect(effect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                //Then shuffle a Cascade card from your trash into your deck.
                if (TurnTaker.Trash.Cards.Any((Card c) => IsCascade(c)))
                {
                    var cardToMove = TurnTaker.Trash.Cards.Where((Card c) => IsCascade(c)).FirstOrDefault();
                    coroutine = GameController.SendMessageAction($"{Card.Title} shuffles {cardToMove.Title} into {TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource(), new Card[] { cardToMove });
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.MoveCard(DecisionMaker, cardToMove, TurnTaker.Deck, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
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
        public override IEnumerator UsePower(int index = 0)
        {
            int numDamage = GetPowerNumeral(0, 1);
            //"{Pyre} deals himself and each non-hero target 1 energy damage."
            IEnumerator coroutine = DealDamage(CharacterCard, CharacterCard, numDamage, DamageType.Energy, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DealDamage(CharacterCard, (Card c) => !c.IsHero, numDamage, DamageType.Energy);
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

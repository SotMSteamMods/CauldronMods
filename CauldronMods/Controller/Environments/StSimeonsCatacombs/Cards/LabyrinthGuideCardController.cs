using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class LabyrinthGuideCardController : StSimeonsBaseCardController
    {
        public static readonly string Identifier = "LabyrinthGuide";

        public LabyrinthGuideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("TwistingPassages");
        }

        public override void AddTriggers()
        {
            //At the start of a hero's turn, that hero may discard 2 cards to destroy a Room in play.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt.IsHero, this.Discard2ToDestroyRoom, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroyCard
            }, (PhaseChangeAction p) => !p.ToPhase.TurnTaker.ToHero().IsIncapacitatedOrOutOfGame);

            //At the start of the environment turn, if Twisting Passages is not in play, this card deals each hero target 1 psychic damage or it is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.StartOfEnvironmentResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator StartOfEnvironmentResponse(PhaseChangeAction arg)
        {
            //if Twisting Passages is not in play
            if (!this.IsTwistingPassagesInPlay())
            {
                bool availableHeroes = FindActiveHeroTurnTakerControllers().Any(httc => GameController.IsTurnTakerVisibleToCardSource(httc.TurnTaker, GetCardSource()));
                if (availableHeroes)
                {
                    List<bool> storedResults = new List<bool>();
                    IEnumerator coroutine = base.MakeUnanimousDecision((HeroTurnTakerController hero) => !hero.IsIncapacitatedOrOutOfGame, SelectionType.DealDamage, storedResults: storedResults);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (storedResults.Count<bool>() > 0 && storedResults.First<bool>())
                    {
                        // this card deals each hero target 1 psychic damage .
                        IEnumerator coroutine2 = base.DealDamage(base.Card, (Card c) => c.IsHero && c.IsTarget, 1, DamageType.Psychic);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }

                        yield break;

                    }
                } else
                {
                    IEnumerator coroutine = GameController.SendMessageAction($"There are no heroes who can be damaged by {Card.Title}", Priority.Medium, GetCardSource(), showCardSource: true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //or it is destroyed
                IEnumerator coroutine3 = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine3);
                }
                
            }
            yield break;
        }

        private IEnumerator Discard2ToDestroyRoom(PhaseChangeAction phaseChange)
        {
            if (phaseChange.ToPhase.TurnTaker.IsHero)
            {
                //...that hero...
                HeroTurnTakerController heroTurnTakerController = base.GameController.FindHeroTurnTakerController(phaseChange.ToPhase.TurnTaker.ToHero());
                if (heroTurnTakerController.NumberOfCardsInHand >= 2)
                {
                    //... may discard 2 cards...
                    List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
                    IEnumerator coroutine = base.SelectAndDiscardCards(heroTurnTakerController, 2, true, storedResults: storedResults);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (base.DidDiscardCards(storedResults, 2))
                    {
                        //... to destroy a Room in play.
                        IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsRoom, "room"), false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine2);
                        }
                    }
                }
                else
                {
                    //send message if not enough cards in hand
                    IEnumerator coroutine3 = base.GameController.SendMessageAction(heroTurnTakerController.Name + " does not have enough cards in their hand to discard for " + base.Card.Title + ".", Priority.Low, base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine3);
                    }
                }
            }
            yield break;

        }
        private bool IsTwistingPassagesInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == TwistingPassagesCardController.Identifier).Count() > 0;
        }
    }
}

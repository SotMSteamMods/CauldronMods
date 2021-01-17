using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class FutureDriftCharacterCardController : DualDriftSubCharacterCardController
    {
        public FutureDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Play an ongoing card. At the end of your next turn, return it from play to your hand. Shift {DriftRR}.
            IEnumerator coroutine;
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it and that hero uses that power.
                        List<SelectLocationDecision> selectedDeck = new List<SelectLocationDecision>();
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location loc) => loc.IsHero && loc.IsDeck, storedResults: selectedDeck, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (selectedDeck.Any())
                        {
                            //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it...
                            List<Card> playResult = new List<Card>();
                            coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, selectedDeck.FirstOrDefault().SelectedLocation.Location, true, false, false, new LinqCardCriteria((Card c) => c.HasPowers), null, 1, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, storedPlayResults: playResult);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            //...and that hero uses that power.
                            if (playResult.Any())
                            {
                                for (int i = 0; i < playResult.FirstOrDefault().NumberOfPowers; i++)
                                {
                                    coroutine = UsePowerOnOtherCard(playResult.FirstOrDefault(), i);
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
                        break;
                    }
                case 2:
                    {
                        //One target regains 2 HP.
                        coroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, 2, cardSource: base.GetCardSource());
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
            }
            yield break;
        }
    }
}

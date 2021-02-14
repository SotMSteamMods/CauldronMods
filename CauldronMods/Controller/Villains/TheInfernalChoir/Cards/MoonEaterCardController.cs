using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class MoonEaterCardController : TheInfernalChoirUtilityCardController
    {
        public MoonEaterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(VagrantHeartSoulRevealedIdentifier);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            foreach (var httc in GameController.FindHeroTurnTakerControllers().Where(httc => !httc.IsIncapacitatedOrOutOfGame))
            {
                List<SelectNumberDecision> numberResult = new List<SelectNumberDecision>();
                coroutine = GameController.SelectNumber(httc, SelectionType.DiscardFromDeck, 1, 3, storedResults: numberResult, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                List<MoveCardAction> discardResult = new List<MoveCardAction>();
                var number = numberResult.First()?.SelectedNumber ?? 0;
                if (number > 0)
                {
                    coroutine = DiscardCardsFromTopOfDeck(httc, number, storedResults: discardResult, responsibleTurnTaker: TurnTaker);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }

                if (discardResult.Any())
                {
                    int discards = GetNumberOfCardsMoved(discardResult);

                    if(IsVagrantHeartSoulRevealedInPlay())
                    {
                        discards *= 2;
                        coroutine = GameController.SendMessageAction($"{FindVagrantHeartSoulRevealed().Title} is in play, so X is doubled!", Priority.Medium, GetCardSource(), showCardSource: true);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                    }

                    int damage = 5 - discards;

                    if (damage <= 0) continue;

                    coroutine = GameController.DealDamageToSelf(httc, c => httc.CharacterCards.Contains(c) && !c.IsIncapacitatedOrOutOfGame, damage, DamageType.Cold, cardSource: GetCardSource());
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
    }
}

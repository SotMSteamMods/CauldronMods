using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class TheInfernalChoirCharacterCardController : VillainCharacterCardController
    {
        public TheInfernalChoirCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(() => !Card.IsFlipped ? FindCard("VagrantHeartPhase1") : FindCard("VagrantHeartPhase2"));
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => PlayTheTopCardOfTheVillainDeckWithMessageResponse(pca), TriggerType.PlayCard));
                AddSideTrigger(AddDealDamageAtEndOfTurnTrigger(TurnTaker, CharacterCard, c => !(IsGhost(c) || IsVillainTarget(c)), TargetType.All, 1, DamageType.Infernal));

                if (Game.IsAdvanced)
                {
                    AddSideTrigger(AddIncreaseDamageTrigger(dda => true, 1));
                }
            }
            else
            {
                AddSideTrigger(AddRedirectDamageTrigger(dda => dda.DamageSource.IsHero && IsVillainTarget(dda.Target) && Game.ActiveTurnTaker == TurnTaker, () => GameController.OrderTargetsByHighestHitPoints(c => c.IsHero, false, GetCardSource()).First()));
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => FlippedCardRemoval(pca), TriggerType.RemoveFromGame));
                AddDefeatedIfDestroyedTriggers();
            }
        }

        protected bool IsGhost(Card c, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return c != null && (c.DoKeywordsContain("ghost", evenIfUnderCard, evenIfFaceDown) || GameController.DoesCardContainKeyword(c, "ghost", evenIfUnderCard, evenIfFaceDown));
        }

        public override bool CanBeDestroyed => !Card.IsFlipped;

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            var p1Heart = TurnTaker.FindCard("VagrantHeartPhase1", false);
            var p2Heart = TurnTaker.FindCard("VagrantHeartPhase2", false);

            var tt = p1Heart.Location.OwnerTurnTaker;
            var httc = FindHeroTurnTakerController(tt.ToHero());

            var coroutine = GameController.ShuffleCardsIntoLocation(httc, p1Heart.UnderLocation.Cards, tt.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            
            coroutine = GameController.SwitchCards(p1Heart, p2Heart, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SetHP(CharacterCardWithoutReplacements, CharacterCardWithoutReplacements.MaximumHitPoints.Value);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator FlippedCardRemoval(GameAction action)
        {
            return GameController.SendMessageAction("Ghost lady takes your cards, wooooooo.", Priority.Medium, GetCardSource());
        }
    }
}

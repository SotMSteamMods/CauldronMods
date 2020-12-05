using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Cauldron.Celadroch
{
    public class CeladrochCharacterCardController : VillainCharacterCardController
    {
        /*
         * Black Wind Rising:
         * Gameplay:
         * Villain cards cannot be played.
		 * At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card.
		 * Otherwise, add 1 token to the storm pool.
         *
         * Advanced:
         * Reduce Damage Dealt to Relics by 1.
         *
         */

        public static readonly string StormPoolIdentifier = "StormPool";

        public TokenPool StormPool => CharacterCard.FindTokenPool(StormPoolIdentifier);

        public CeladrochCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TopCardSpecialString, null, () => new[] { FindCeladrochsTopCard() }).Condition = () => !Card.IsFlipped;
        }

        private Card FindCeladrochsTopCard()
        {
            return Game.Journal.CardPropertiesEntries(j => j.Key == "CeladrochsTopCard").FirstOrDefault()?.Card;
        }

        private string TopCardSpecialString()
        {
            var card = FindCeladrochsTopCard();
            if (card != null)
            {
                if (card == TurnTaker.Deck.TopCard)
                    return $"Celadroch's top card is {card.Title}";
                return $"Celadroch's top card was {card.Title}";
            }
            return "";
        }

        public override void AddSideTriggers()
        {
            // Front side (Black Wind Rising)
            if (!base.Card.IsFlipped)
            {
                //Villain Cards cannot be played
                CannotPlayCards(ttc => ttc.IsVillain);

                //At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card. Otherwise, add 1 token to the storm pool.
                AddStartOfTurnTrigger(tt => tt == TurnTaker, FrontSideAddTokensOrFlip, new[] { TriggerType.AddTokensToPool, TriggerType.FlipCard });

                if (IsGameAdvanced)
                {
                    AddReduceDamageTrigger(c => c.IsTarget && c.IsRelic, 1);
                }
            }
            // Flipped side (The Dark Mountain)
            else
            {

                if (this.IsGameAdvanced)
                {

                }

                base.AddDefeatedIfDestroyedTriggers();
            }
        }


        private IEnumerator FrontSideAddTokensOrFlip(PhaseChangeAction action)
        {
            //At the start of the villain turn, if the storm pool has 3 or more tokens, flip {Celadroch}'s character card. Otherwise, add 1 token to the storm pool.
            IEnumerator coroutine;
            if (StormPool.CurrentValue >= 3)
            {
                coroutine = FlipThisCharacterCardResponse(action);
            }
            else
            {
                coroutine = GameController.AddTokensToPool(StormPool, 1, GetCardSource());
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }


        public override bool CanBeDestroyed => base.CharacterCard.IsFlipped;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (base.Card.IsFlipped)
            {
                yield break;
            }


        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {


            yield break;
        }

    }
}
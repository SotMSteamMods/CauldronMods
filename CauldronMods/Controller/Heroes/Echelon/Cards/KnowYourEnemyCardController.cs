using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class KnowYourEnemyCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card.
        // If you do not, draw a card and destroy this card.
        // The first time a hero destroys a non-hero target each turn, you may draw a card.
        //==============================================================

        public static string Identifier = "KnowYourEnemy";
        private readonly string drawKey = "KnowYourEnemyDrawnCardThisTurnKey";

        public KnowYourEnemyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(drawKey);
        }

        protected override void AddTacticEffectTrigger()
        {
            //The first time a hero destroys a non-hero target each turn, you may draw a card.
            AddTrigger((DestroyCardAction dc) => dc.WasCardDestroyed && dc.CardToDestroy.Card.IsTarget && !IsHeroTarget(dc.CardToDestroy.Card) && !HasBeenSetToTrueThisTurn(drawKey) && IsByHero(dc),
                                FirstDrawResponse, TriggerType.DrawCard, TriggerTiming.After);
        }

        private bool IsByHero(DestroyCardAction dc)
        {
            if(dc != null && dc.ActionSource is DealDamageAction damage)
            {
                return damage.DamageSource.IsHero;
            }
            else if (dc != null && dc.CardSource != null)
            {
                return IsHero(dc.CardSource.Card);
            }
            return false;
        }

        private IEnumerator FirstDrawResponse(DestroyCardAction _)
        {
            SetCardPropertyToTrueIfRealAction(drawKey);
            IEnumerator coroutine = DrawCard(DecisionMaker.HeroTurnTaker, optional: true);
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

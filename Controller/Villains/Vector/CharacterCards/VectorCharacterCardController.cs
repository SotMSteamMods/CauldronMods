using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class VectorCharacterCardController : VectorBaseCharacterCardController
    {
        /*
         *
         * Setup:
         *
         * At the start of the game, put {Vector}'s villain character cards into play, "Asymptomatic Carrier" side up, with 40 HP.
         *
         * Gameplay:
         *
         * - Whenever {Vector} is dealt damage, play the top card of the villain deck.
         * - If {Vector} regains all his HP, he escapes. Game over.
         * - Supervirus is indestructible. Cards beneath it are indestructible and have no game text.
         * - Whenever Supervirus is in play and there are {H + 2} or more cards beneath it, flip {Vector}'s villain character cards.
         * - If {Vector} is dealt damage by an environment card, he becomes immune to damage dealt by environment cards until the end of the turn.
         *
         *
         * Advanced:
         *
         * - At the end of the villain turn, {Vector} regains 2 HP.
         *
         * Flipped Gameplay:
         *
         * - Whenever {Vector} flips to this side, remove Supervirus from the game. Put all virus cards that were beneath it into the villain trash.
         * - Reduce damage dealt to {Vector} by 1 for each villain target in play.
         * - At the end of the villain turn, play the top card of the villain deck.
         *
         * Flipped Advanced:
         *
         * - Increase damage dealt by {Vector} by 2.
         *
         */

        private const int AdvancedHpGain = 2;
        private const int AdvancedDamageIncrease = 2;
        private const string EndingMessage = "Vector regained all of his health!  He escapes and the Heroes lose!";

        public VectorCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            // Front side (Asymptomatic Carrier)
            if (!base.Card.IsFlipped)
            {
                // Whenever {Vector} is dealt damage, play the top card of the villain deck.
                base.SideTriggers.Add(
                    base.AddTrigger<DealDamageAction>(dda => dda.DamageSource != null 
                    && dda.Target.Equals(this.Card) && dda.DidDealDamage, PlayTheTopCardOfTheVillainDeckWithMessageResponse, new[] { TriggerType.PlayCard }, TriggerTiming.After));

                // If {Vector} regains all his HP, he escapes. Game over.
                base.SideTriggers.Add(base.AddTrigger<GainHPAction>(gha => gha.IsSuccessful && gha.HpGainer.Equals(this.Card), 
                    CheckHpResponse, TriggerType.GainHP, TriggerTiming.After));

                // If {Vector} is dealt damage by an environment card, he becomes
                // immune to damage dealt by environment cards until the end of the turn.
                base.SideTriggers.Add(base.AddTrigger<DealDamageAction>(dda => dda.DamageSource != null && 
                    dda.DamageSource.IsEnvironmentCard && dda.Target.Equals(this.Card) && dda.DidDealDamage,
                DealtDamageByEnvResponse, new[] {TriggerType.ImmuneToDamage}, TriggerTiming.After));
                
                if (this.IsGameAdvanced)
                {
                    // At the end of the villain turn, {Vector} regains 2 HP.
                    base.SideTriggers.Add(base.AddEndOfTurnTrigger(tt => tt == this.TurnTaker,
                        AdvancedEndOfTurnResponse, TriggerType.GainHP));
                }
            }
            else
            {
                // At the end of the villain turn, play the top card of the villain deck.
                base.SideTriggers.Add(base.AddEndOfTurnTrigger(tt => tt == this.TurnTaker,
                    PlayTheTopCardOfTheVillainDeckWithMessageResponse, new[] { TriggerType.PlayCard }));


                // Reduce damage dealt to {Vector} by 1 for each villain target in play.
                base.SideTriggers.Add(base.AddReduceDamageTrigger(c => c == base.CharacterCard, FindNumberOfVillainCardsInPlay() ?? default));

                

                if (this.IsGameAdvanced)
                {
                    // Increase damage dealt by {Vector} by 2.
                    base.SideTriggers.Add(base.AddIncreaseDamageTrigger(dd => dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard), dd => 2));

                }
            }
            AddVectorDefeatedIfDestroyedTriggers();
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            RemoveSideTriggers();
            // Remove Super Virus card from the game
            if (IsSuperVirusInPlay())
            {
                // Move any cards underneath Super Virus into villain trash
                IEnumerable<Card> cardsUnder = FindCardsWhere(c => c.Location == GetSuperVirusCard().UnderLocation);
                IEnumerator r1;

                var copyCards = cardsUnder.ToArray();
                for (int i = 0; i < copyCards.Count(); i++)
                {
                    var card = copyCards.ElementAt(i);
                    r1 = GameController.MoveCard(DecisionMaker, card, card.NativeTrash,evenIfIndestructible: true, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(r1);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(r1);
                    }
                }

                // Remove Super Virus from game
                IEnumerator r2 = this.GameController.MoveCard(this.TurnTakerController, GetSuperVirusCard(),
                    new Location(this.Card, LocationName.OutOfGame), evenIfIndestructible: true, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(r2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(r2);
                }
            }
            AddSideTriggers();
        }

        private IEnumerator DealtDamageByEnvResponse(DealDamageAction dda)
        {
            ImmuneToDamageStatusEffect itdse = new ImmuneToDamageStatusEffect
            {
                TargetCriteria = { IsSpecificCard = this.CharacterCard },
                SourceCriteria = { IsEnvironment = true }
            };
            itdse.UntilThisTurnIsOver(base.Game);

            IEnumerator routine = base.GameController.AddStatusEffect(itdse, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator CheckHpResponse(GainHPAction gha)
        {
            if (this.Card.HitPoints != this.Card.MaximumHitPoints)
            {
                yield break;
            }

            // Reached maximum HP, game over
            IEnumerator routine = base.GameController.GameOver(EndingResult.AlternateDefeat, EndingMessage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        private IEnumerator AdvancedEndOfTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator routine = this.GameController.GainHP(this.CharacterCard, AdvancedHpGain, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        protected void AddVectorDefeatedIfDestroyedTriggers(bool canBeMoved = false)
        {
           
            if (!canBeMoved)
            {
                SideTriggers.Add(AddTrigger((DestroyCardAction destroyCard) => destroyCard.CardToDestroy == this && !IsSuperVirusInPlay(), CannotBeMovedResponse, TriggerType.Hidden, TriggerTiming.Before));
            }
            AddVectorDefeatedIfMovedOutOfGameTriggers();
            AddVectorTriggerGameOver();
           
        }

        protected void AddVectorTriggerGameOver()
        {
            SideTriggers.Add(AddTrigger<DestroyCardAction>((DestroyCardAction destroyCard) => destroyCard.CardToDestroy == this && !IsSuperVirusInPlay(), DefeatedResponse, TriggerType.GameOver, TriggerTiming.Before));
        }

        protected void AddVectorDefeatedIfMovedOutOfGameTriggers()
        {
            SideTriggers.Add(AddTrigger((MoveCardAction moveCard) => !IsSuperVirusInPlay() && moveCard.CardToMove == base.Card && moveCard.Destination.Name == LocationName.OutOfGame, (MoveCardAction m) => DefeatedResponse(m), TriggerType.GameOver, TriggerTiming.Before));
        }

      

        public int? FindNumberOfVillainCardsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsVillainTarget).Count();
        }
    }
}
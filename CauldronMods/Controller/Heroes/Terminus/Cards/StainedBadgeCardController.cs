using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class StainedBadgeCardController : TerminusMementoCardController
    {
        /*
         * This card and {Terminus} are indestructible unless all other heroes are incapacitated. If another Memento would 
         * enter play, instead remove it from the game.
         * At the end of your turn, add 1 token to your Wrath pool if {Terminus} has 1 or more HP.
         */
        public StainedBadgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        protected override void ShowIndestructibleString()
        {
            Func<int> otherActiveHeroes = () => base.GameController.AllTurnTakers.Count((tt) =>
                    tt != base.TurnTaker &&
                    IsHero(tt) &&
                    !tt.IsIncapacitatedOrOutOfGame &&
                    base.GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) &&
                    base.BattleZone == tt.BattleZone);
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.").Condition = () => otherActiveHeroes() > 0;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            bool isIndestructible = false;

            if ((card == base.CharacterCard || card == this.Card) &&
                base.GameController.AllTurnTakers.Count((tt) =>
                    tt != base.TurnTaker &&
                    IsHero(tt) &&
                    !tt.IsIncapacitatedOrOutOfGame && 
                    base.GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && 
                    base.BattleZone == tt.BattleZone) > 0)
            {
                isIndestructible = true;
            }

            return isIndestructible;
        }

        public override void AddTriggers()
        {
            base.AddTrigger((FlipCardAction fca) => IsHeroCharacterCard(fca.CardToFlip.Card), (FlipCardAction fca) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
            base.AddTrigger((SwitchBattleZoneAction sb) => sb.Origin == base.Card.BattleZone, (SwitchBattleZoneAction sb) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);
            base.AddTrigger((CardEntersPlayAction cep) => GameController.IsInCardControllerList(cep.CardEnteringPlay, CardControllerListType.ChangesVisibility), (CardEntersPlayAction cep) => base.GameController.DestroyAnyCardsThatShouldBeDestroyed(ignoreBattleZone: true, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, ActionDescription.Unspecified, isConditional: false, requireActionSuccess: true, null, outOfPlayTrigger: false, null, null, ignoreBattleZone: true);

            base.AddEndOfTurnTrigger((tt) => tt == base.TurnTaker && base.CharacterCard.HitPoints > 0, PhaseChangeActionResponse, TriggerType.AddTokensToPool);
            base.AddTriggers();
        }

        protected IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;

            coroutine = base.AddWrathTokens(1); 
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

        protected override IEnumerator OnOtherMementoRemoved()
        {            
            return DoNothing();
        }
    }
}

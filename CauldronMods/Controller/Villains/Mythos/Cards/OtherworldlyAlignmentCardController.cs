using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class OtherworldlyAlignmentCardController : MythosUtilityCardController
    {
        public OtherworldlyAlignmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn:
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = null;
            if (base.IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                //{MythosDanger} {Mythos} deals the hero target with the highest HP {H} infernal damage.
                coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => IsHero(c), (Card c) => base.Game.H, DamageType.Infernal);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                //{MythosMadness} {Mythos} deals each non-villain target 1 infernal damage.
                coroutine = base.DealDamage(base.CharacterCard, (Card c) => !base.IsVillain(c), 1, DamageType.Infernal);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (base.IsTopCardMatching(MythosClueDeckIdentifier))
            {
                //{MythosClue} {Mythos} deals the hero target with the lowest HP {H} psychic damage
                coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => IsHero(c), (Card c) => base.Game.H, DamageType.Psychic);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}

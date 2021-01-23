using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class WretchedSymphonyCardController : TheInfernalChoirUtilityCardController
    {
        public WretchedSymphonyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(() => FindVagrantHeartHiddenSoul() ?? FindVagrantHeartSoulRevealed());
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddReduceDamageTrigger(c => c == CharacterCard, 1);
            AddEndOfTurnTrigger(tt => tt == TurnTaker && IsHiddenHeartInPlay(), pca => DealDamageEndOfTurnAction(), TriggerType.DealDamage);
            AddEndOfTurnTrigger(tt => tt == TurnTaker && IsSoulRevealedInPlay(), pca => RestoreGhostsEndOfTurnAction(), TriggerType.GainHP);
        }

        private IEnumerator DealDamageEndOfTurnAction()
        {
            bool a = IsHiddenHeartInPlay();
            var vagrantTurnTaker = FindVagrantHeartHiddenSoul().Location.OwnerTurnTaker;
            var httc = FindHeroTurnTakerController(vagrantTurnTaker.ToHero());
            var result = new List<SelectCardDecision>();
            var coroutine = GameController.SelectHeroCharacterCard(httc, SelectionType.CardToDealDamage, result, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(result))
            {
                var source = GetSelectedCard(result);
                coroutine = DealDamage(source, c => c.IsHeroCharacterCard && c != source, 2, DamageType.Cold);
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

        private IEnumerator RestoreGhostsEndOfTurnAction()
        {
            var coroutine = GameController.SetHP(DecisionMaker,
                                criteria: c => IsGhost(c) && c.IsTarget && c.IsInPlayAndHasGameText && c.HitPoints.Value <= c.MaximumHitPoints.Value,
                                amountBasedOnTarget: c => c.MaximumHitPoints.Value,
                                cardSource: GetCardSource());
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

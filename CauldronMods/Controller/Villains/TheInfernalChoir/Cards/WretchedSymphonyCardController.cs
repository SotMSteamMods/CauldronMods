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
            SpecialStringMaker.ShowIfElseSpecialString(() => FindVagrantHeartHiddenHeart() is null, () => $"{FindVagrantHeartSoulRevealed().Title} is in play.", () => $"{FindVagrantHeartHiddenHeart().Title} is in {FindVagrantHeartHiddenHeart().Location.GetFriendlyName()}.").Condition = () => Game.HasGameStarted;
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddReduceDamageTrigger(c => c == CharacterCard, 1);
            AddEndOfTurnTrigger(tt => tt == TurnTaker && IsVagrantHeartHiddenHeartInPlay(), pca => DealDamageEndOfTurnAction(), TriggerType.DealDamage);
            AddEndOfTurnTrigger(tt => tt == TurnTaker && IsVagrantHeartSoulRevealedInPlay(), pca => RestoreGhostsEndOfTurnAction(), TriggerType.GainHP);
        }

        private IEnumerator DealDamageEndOfTurnAction()
        {
            var vagrantTurnTaker = FindVagrantHeartHiddenHeart().Location.OwnerTurnTaker;
            var httc = FindHeroTurnTakerController(vagrantTurnTaker.ToHero());
            var result = new List<SelectCardDecision>();
            DealDamageAction gameAction = new DealDamageAction(GetCardSource(), null, null, 2, DamageType.Cold);
            var coroutine = base.GameController.SelectCardAndStoreResults(httc, SelectionType.HeroToDealDamage, new LinqCardCriteria((Card c) => c.Owner == httc.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active heroes"), result, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
           
            if (DidSelectCard(result))
            {
                var source = GetSelectedCard(result);
                coroutine = DealDamage(source, c =>  IsHeroCharacterCard(c) && c != source, 2, DamageType.Cold);
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

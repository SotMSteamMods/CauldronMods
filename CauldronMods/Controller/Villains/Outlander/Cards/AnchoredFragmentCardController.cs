using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class AnchoredFragmentCardController : OutlanderUtilityCardController
    {
        public AnchoredFragmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            var ss = SpecialStringMaker.ShowSpecialString(() => "Outlander has taken " + Journal.DealDamageEntriesThisRound().Where(je => je.TargetCard == CharacterCard).Sum(je => je.Amount) + " damage this round");
            ss.ShowInEffectsList = () => true;
            ss.RelatedCards = () => new[] { CharacterCard };
        }

        public override IEnumerator Play()
        {
            //When this card enters play, {Outlander} deals the hero target with the highest HP 1 melee damage.
            IEnumerator coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHero(c), (Card c) => 1, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            //At the start of the villain turn, if {Outlander} was not dealt at least {H} times 2 damage in the last round, destroy {H} hero ongoing and/or equipment cards.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, MaybeDestroyCardsResponse, TriggerType.DestroyCard);
        }

        private IEnumerator MaybeDestroyCardsResponse(PhaseChangeAction action)
        {
            //...if {Outlander} was not dealt at least {H} times 2 damage in the last round,
            int damageDealt = Journal.DealDamageEntries().Where(je => je.Round == Game.Round - 1 && je.TargetCard == CharacterCard).Sum(je => je.Amount);
            if (damageDealt < (Game.H * 2))
            {
                IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} reacts!{{BR}}Outlander was only dealt {damageDealt} damage last round.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //...destroy {H} hero ongoing and/or equipment cards.
                coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || IsEquipment(c)), "hero ongoing or equipment"), Game.H, cardSource: GetCardSource());
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

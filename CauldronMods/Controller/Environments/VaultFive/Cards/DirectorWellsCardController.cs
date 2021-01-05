using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class DirectorWellsCardController : VaultFiveUtilityCardController
    {
        public DirectorWellsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP().Condition = () => !Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowSpecialString(() => BuildPlayersWithArtifactInHandSpecialString());
        }

        public override void AddTriggers()
        {
            //That hero is immune to psychic damage.
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Psychic && GetCardThisCardIsNextTo() != null && dd.Target == GetCardThisCardIsNextTo());

            //Players with Artifact cards in their hand cannot draw cards.
            CannotDrawCards((TurnTakerController ttc) => GetPlayersWithArtifactsInHand().Contains(ttc.TurnTaker));
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the lowest HP
            List<Card> foundTarget = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) => c.IsHeroCharacterCard && (overridePlayArea == null || c.IsAtLocationRecursive(overridePlayArea)), foundTarget, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card lowestHero = foundTarget.FirstOrDefault();
            if (lowestHero != null && storedResults != null)
            {
                //Play this card next to the hero with the lowest HP
                storedResults.Add(new MoveCardDestination(lowestHero.NextToLocation, false, false, false));
            }
            yield break;
        }
    }
}

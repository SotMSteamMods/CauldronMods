using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class ForbiddenKnowledgeCardController : VaultFiveUtilityCardController
    {
        public ForbiddenKnowledgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowSpecialString(() => BuildPlayersWithArtifactInHandSpecialString());
		}

		public override void AddTriggers()
        {
			//At the end of the environment turn, this card deals 1 hero 2 infernal damage.
			AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageEnvironmentTurnResponse, new TriggerType[]
			{
				TriggerType.DealDamage,
				TriggerType.DrawCard
			});

            //At the end of each hero's turn, if they have an Artifact card in their hand, this card deals that hero 2 psychic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && DoesPlayerHaveArtifactInHand(tt.ToHero()), DealDamageHeroTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageEnvironmentTurnResponse(PhaseChangeAction arg)
        {
			//this card deals 1 hero 2 infernal damage.
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 2, DamageType.Infernal, 1, false, 1, additionalCriteria: (Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResultsDamage: storedResults, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if(DidDealDamage(storedResults))
            {
				// A hero dealt damage this way may draw 1 card.
				Card target = storedResults.FirstOrDefault().Target;
				if(IsHeroCharacterCard(target))
                {
					coroutine = DrawCard(target.Owner.ToHero(), optional: true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
			}

			yield break;
		}

        private IEnumerator DealDamageHeroTurnResponse(PhaseChangeAction pca)
        {
			//this card deals that hero 2 psychic damage
			List<Card> storedCharacter = new List<Card>();
			IEnumerator coroutine = FindCharacterCardToTakeDamage(pca.ToPhase.TurnTaker, storedCharacter, Card, 2, DamageType.Psychic);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			Card card2 = storedCharacter.FirstOrDefault();
			if (card2 != null)
			{
				coroutine = DealDamage(Card, card2, 2, DamageType.Psychic, cardSource: GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
		}
    }
}

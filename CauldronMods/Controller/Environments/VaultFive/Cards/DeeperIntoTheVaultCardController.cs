using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class DeeperIntoTheVaultCardController : VaultFiveUtilityCardController
    {
        public DeeperIntoTheVaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Increase all psychic damage dealt by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Psychic, 1);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, reveal the top {H} cards of the environment deck. Put any Artifact cards revealed into play and discard the other revealed cards.
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, Game.H, new LinqCardCriteria((Card c) => IsArtifact(c), "artifact"), isPutIntoPlay: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }
    }
}

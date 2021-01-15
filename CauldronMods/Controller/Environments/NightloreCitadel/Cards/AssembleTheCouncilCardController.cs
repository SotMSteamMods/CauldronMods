using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class AssembleTheCouncilCardController : NightloreCitadelUtilityCardController
    {
        public AssembleTheCouncilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, reveal cards from the top of the environment deck until a target is revealed, put it into play, and discard the other revealed cards. 

            List<Card> playCardList = new List<Card>() ;
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(base.TurnTakerController, base.TurnTaker.Deck, null, new LinqCardCriteria((Card c) => c.IsTarget, "target"), isPutIntoPlay: true, playedCards: playCardList, fromBottom: false, revealUntilNumberOfMatchingCards: 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If Starlight of Oros was put into play this way, she deals each other target 1 psychic damage.
            if(playCardList.Any(c => c.Identifier == StarlightOfOrosIdentifier))
            {
                Card oros = FindStarlightOfOrosInPlay();
                coroutine = DealDamage(oros, (Card c) => c.IsTarget && c != oros, 1, DamageType.Psychic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(null);
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

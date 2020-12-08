using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class DistributedHivemindSwarmEaterCharacter : VillainCharacterCardController
    {
        public DistributedHivemindSwarmEaterCharacter(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("nanomutant"), "nanomutant"));
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                //Whenever a villain target would deal damage to another villain target, redirect that damage to the hero target with the highest HP.
                base.AddRedirectDamageTrigger((DealDamageAction action) => action.DamageSource.IsVillainTarget && action.Target.IsVillainTarget, () => );
                //When a villain target enters play, flip {SwarmEater}'s villain character cards.
                //At then end of the villain turn, if there are no nanomutants in play, play the top card of the villain deck.
            }
            else
            {
                //When {SwarmEater} flips to this side, discard cards from the top of the villain deck until a target is discarded.
                //Put the discarded target beneath the villain target that just entered play. Then flip {SwarmEater}'s character cards.
                if (base.Game.IsAdvanced)
                {
                    //When {SwarmEater} flips to this side he regains {H - 2} HP.
                }
            }
            base.AddDefeatedIfDestroyedTriggers();
        }

        private IEnumerator HeroTargetWithHighestHP()
        {
            List<Card> highestCard = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => c.IsHero, highestCard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card highestHero = highestCard.FirstOrDefault();
            if (highestCard.Count() > 1)
            {

            }
            yield return highestHero;
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Front - Advanded:
            //Single-Minded Pursuit is indestructible.
            return base.IsGameAdvanced && !base.CharacterCard.IsFlipped && base.FindCardController(card) is SingleMindedPursuitCardController;
        }
    }
}
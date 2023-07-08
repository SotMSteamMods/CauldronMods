using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class PoltergeistCardController : StSimeonsGhostCardController
    {
        public static readonly string Identifier = "Poltergeist";

        public PoltergeistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { SacrificialShrineCardController.Identifier }, false)
        {

        }
        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero 1 projectile damage for each equipment card they have in play. Then, destroy 1 equipment card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroyCard
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals each hero 1 projectile damage for each equipment card they have in play
            foreach (var httc in GameController.FindHeroTurnTakerControllers().Where(httc => !httc.IsIncapacitatedOrOutOfGame))
            {
                int count = httc.TurnTaker.PlayArea.Cards.Where(c => c.IsInPlay && IsEquipment(c)).Count();
                for (int index = 0; index < count; index++)
                {
                    //get potential targets
                    var characterTargets = httc.CharacterCards.Where(ch => ch.IsInPlay && !ch.IsIncapacitatedOrOutOfGame).ToArray();
                    if (characterTargets.Length == 1) //only a single target, deal damage
                    {
                        var coroutine = base.DealDamage(base.Card, characterTargets.First(), 1, DamageType.Projectile, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    else if (characterTargets.Length > 1)
                    {
                        //multiple targets, let the hero choose the victim
                        var coroutine = GameController.SelectTargetsAndDealDamage(httc, new DamageSource(GameController, Card), 1, DamageType.Projectile, 1, false, 1,
                            additionalCriteria: c =>  IsHeroCharacterCard(c) && c.IsInPlay && !c.IsIncapacitatedOrOutOfGame,
                            cardSource: GetCardSource());
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

            //destroy 1 equipment card
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => base.IsEquipment(c), "equipment");
            IEnumerator coroutine2 = base.GameController.SelectAndDestroyCard(this.DecisionMaker, criteria, false, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}
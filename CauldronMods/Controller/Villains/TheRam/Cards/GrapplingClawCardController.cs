using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace Cauldron.TheRam
{
    public class GrapplingClawCardController : TheRamUtilityCardController
    {
        public GrapplingClawCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(1, null, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !IsUpClose(c), "", false, singular: "hero that is not Up Close", plural: "heroes that are not Up Close"));
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by {TheRam} by 1.",
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(GetRam), 1);
            //"At the end of the villain turn, {TheRam} deals {H - 2} projectile damage to the hero with the lowest HP that is not Up Close. Put a copy of Up Close from the villain trash into play next to that hero."
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, EndOfTurnGrappleResponse, TriggerType.DealDamage);
        }

        private bool LogCardChecks(Card c)
        {
            if (IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText)
            {
                Log.Debug($"{c.Title} is in play and has game text. Is it up close? {IsUpClose(c)}. Is it visible? {AskIfCardIsVisibleToCardSource(c, GetCardSource())}");
                return  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !IsUpClose(c) && AskIfCardIsVisibleToCardSource(c, GetCardSource()) != false;

            }
            return false;
            //(Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !IsUpClose(c) && AskIfCardIsVisibleToCardSource(c, GetCardSource()) == true, 

        }
        public IEnumerator EndOfTurnGrappleResponse(PhaseChangeAction pca)
        {

            List<Card> results = new List<Card> { };
            DealDamageAction damagePreview = new DealDamageAction(GetCardSource(), new DamageSource(GameController, GetRam), null, H - 2, DamageType.Projectile);
            IEnumerator chooseTarget = GameController.FindTargetWithLowestHitPoints(1,
                                                                      (Card c) =>  IsHeroCharacterCard(c) && c.IsInPlayAndHasGameText && !IsUpClose(c) && AskIfCardIsVisibleToCardSource(c, GetCardSource()) != false,
                                                                      results, evenIfCannotDealDamage: true, dealDamageInfo: new List<DealDamageAction> { damagePreview }, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseTarget);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseTarget);
            }

            Card target = results.FirstOrDefault();

            if (target == null)
            {
                IEnumerator message = GameController.SendMessageAction($"{this.Card.Title} could not find a hero that was not Up Close.", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            //{TheRam} deals {H - 2} projectile damage to the hero with the lowest HP that is not Up Close.
            if (RamIfInPlay != null)
            {
                IEnumerator damage = DealDamage(GetRam, target, H - 2, DamageType.Projectile, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(damage);
                }
                else
                {
                    GameController.ExhaustCoroutine(damage);
                }
            }
            else
            {
                IEnumerator message = MessageNoRamToAct(GetCardSource(), "deal damage");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
            }

            if (target.Owner.IsIncapacitatedOrOutOfGame)
            {
                IEnumerator message = GameController.SendMessageAction($"{target.Owner.Name} was incapacitated before they could be pulled Up Close.", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            //Put a copy of Up Close from the villain trash into play next to that hero.
            Card upClose = FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash && c.Location.OwnerTurnTaker.IsVillain && c.Identifier == "UpClose")).FirstOrDefault();
            if (upClose == null)
            {
                IEnumerator message = GameController.SendMessageAction("There were no copies of Up Close in the villain trash.", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(message);
                }
                else
                {
                    GameController.ExhaustCoroutine(message);
                }
                yield break;
            }

            IEnumerator pullUpClose = PlayGivenUpCloseByGivenCard(upClose, target, true);
            IEnumerator finalMessage = GameController.SendMessageAction($"{target.Owner.Name} was pulled Up Close!", Priority.High, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(pullUpClose);
                yield return GameController.StartCoroutine(finalMessage);
            }
            else
            {
                GameController.ExhaustCoroutine(pullUpClose);
                GameController.ExhaustCoroutine(finalMessage);
            }
            yield break;
        }
    }
}
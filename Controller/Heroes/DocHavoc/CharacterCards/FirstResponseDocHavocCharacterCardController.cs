using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class FirstResponseDocHavocCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageToDeal = 1;
        private const int Incapacitate1HpThreshold = 6;
        private const int Incapacitate2CardsToDraw = 1;
        private const int Incapacitate3CardsToDestroy = 2;

        public FirstResponseDocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Draw a card. {DocHavoc} may deal 1 target 1 melee damage.
            //==============================================================

            IEnumerator routine = this.DrawCard(this.HeroTurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            int powerNumeral = GetPowerNumeral(0, PowerDamageToDeal);
            DamageSource ds = new DamageSource(this.GameController, this.TurnTaker);
            IEnumerator routine2 = base.GameController.SelectTargetsAndDealDamage(this.HeroTurnTakerController, ds, powerNumeral,
                DamageType.Melee, 1, true, 0, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine2);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Until the start of your next turn, reduce damage dealt to targets with fewer than 6 HP by
                    //==============================================================

                    break;

                case 1:

                    //==============================================================
                    // One player may take a one-shot from their trash and put it on top of their deck.
                    //==============================================================

                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.MoveCardOnDeck, 
                        new LinqCardCriteria(c => c.IsInTrash && c.Location.IsHero && c.IsOneShot, "in any hero's trash", useCardsSuffix: false, useCardsPrefix: true), 
                        storedResults, optional: false,  cardSource: GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (storedResults.Any((SelectCardDecision d) => d.Completed && d.SelectedCard != null))
                    {
                        Card selectedCard = storedResults.FirstOrDefault().SelectedCard;
                        Location hand = selectedCard.Location.OwnerTurnTaker.ToHero().Hand;
                        coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedCard, hand, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, storedResults.Cast<IDecision>(), null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                    }



                    /*
                    coroutine = GameController.SelectHeroToMoveCardFromTrash(base.HeroTurnTakerController,
                        httc => httc.HeroTurnTaker.Deck,
                        optionalMoveCard: false,
                        cardCriteria: new LinqCardCriteria(card => card.IsOneShot, "one-shot"), 
                        cardSource: GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    */

                    break;

                case 2:

                    //==============================================================
                    // Destroy 2 environment targets.
                    //==============================================================

                    coroutine = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, 
                        new LinqCardCriteria(c => c.IsEnvironment, "environment"), Incapacitate3CardsToDestroy,
                        requiredDecisions: 0, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;


            }
        }
    }
}

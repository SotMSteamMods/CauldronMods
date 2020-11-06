using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class HealingMagicCardController : CardController
    {
        #region Constructors

        public HealingMagicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            IEnumerable<Card> heads = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsTarget));
            Card lowestHPHead = heads.FirstOrDefault();
            int lowestHP = Convert.ToInt32(heads.FirstOrDefault().HitPoints);
            if (base.IsLowestHitPointsUnique((Card c) => c.DoKeywordsContain("head") && c.IsTarget))
            {
                foreach (Card head in heads)
                {
                    if (head.HitPoints >= lowestHPHead.HitPoints)
                    {
                        lowestHPHead = head;
                    }
                }
            }
            else
            {
                foreach (Card head in heads)
                {
                    if (head.HitPoints >= lowestHPHead.HitPoints)
                    {
                        lowestHP = Convert.ToInt32(head.HitPoints);
                    }
                }
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTarget, new LinqCardCriteria((Card c) => c.DoKeywordsContain("head") && c.IsTarget && c.HitPoints == lowestHP), storedResults, false, false, null, true, null);
                lowestHPHead = storedResults.FirstOrDefault().SelectedCard;
            }
            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            coroutine = base.GameController.GainHP(lowestHPHead, PlusNumberOfCardInTrash(Game.H, "HealingMagic"));
            //Play the top card of the villain deck.
            IEnumerator coroutine2 = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, false, 1, false, null, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        private int PlusNumberOfCardInTrash(int damage, string identifier)
        {
            return damage + (from card in base.TurnTaker.Trash.Cards
                             where card.Identifier == identifier
                             select card).Count<Card>();
        }

        #endregion Methods
    }
}
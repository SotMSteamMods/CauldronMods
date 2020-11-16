using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class PoltergeistCardController : GhostCardController
    {
        #region Constructors

        public PoltergeistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { "SacrificialShrine" })
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero 1 projectile damage for each equipment card they have in play. Then, destroy 1 equipment card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroyCard
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //this card deals each hero 1 projectile damage for each equipment card they have in play
           
            //find out how many instances of damage to do to each hero
            Dictionary<Card, int> cardAttacks = new Dictionary<Card, int>();
            using (IEnumerator<Card> enumerator = base.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.IsTarget && c.IsInPlay).GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    Card card = enumerator.Current;
                    int num = (from d in FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlay && c.IsHero && base.IsEquipment(c)))
                               where d.Owner == card.Owner
                               select d).Count();
                    if (num > 0)
                    {
                        cardAttacks.Add(card, num);
                    }
                }
            }

            //deal the appropiate number of instances to each hero
            foreach(Card target in cardAttacks.Keys)
            {
                for (int i = 0; i < cardAttacks[target]; i++)
                {
                    if (!target.IsIncapacitatedOrOutOfGame)
                    {
                        IEnumerator coroutine = base.DealDamage(base.Card, target, 1, DamageType.Projectile);
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

        #endregion Methods
    }
}
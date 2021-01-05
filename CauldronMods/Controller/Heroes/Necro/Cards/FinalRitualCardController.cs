using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Cauldron.Necro
{
    public class FinalRitualCardController : NecroCardController
    {
        public FinalRitualCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsUndead(c), "undead"));
        }
        public override IEnumerator Play()
        {
            //Search your trash for up to 2 Undead and put them into play. Necro deals each of those cards 2 toxic damage.

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = base.SearchForCards(this.DecisionMaker, false, true, 0, 2, new LinqCardCriteria((Card c) => this.IsUndead(c), "undead"), true, false, false, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var cards = storedResults.Where(d => d.Completed).Select(d => d.SelectedCard).ToArray();
            if (cards.Any())
            {
                int numUndeadPlayed = 0;

                //Necro deals each of those cards 2 toxic damage.
                foreach (var undeadTarget in cards)
                {
                    if (undeadTarget != null)
                    {
                        numUndeadPlayed++;
                        coroutine = base.DealDamage(base.CharacterCard, (Card card) => card == undeadTarget, 2, DamageType.Toxic);
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

                //Then Necro deals himself X toxic damage, where X is 2 times the number of cards put into play this way."
                coroutine = base.DealDamage(base.CharacterCard, (Card card) => card == base.CharacterCard, 2 * numUndeadPlayed, DamageType.Toxic);
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
}

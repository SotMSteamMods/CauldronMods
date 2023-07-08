using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class StarlightSubCharacterCardController : HeroCharacterCardController
    {
        protected bool IsCoreCharacterCard = true;
        private List<string> QualifiedNightloreCouncilIdentifiers
        {
            get
            {
                return new List<string> { "Cauldron.StarlightOfTerraCharacter", "Cauldron.StarlightOfAsheronCharacter", "Cauldron.StarlightOfCryosFourCharacter" };
            }
        }
        public StarlightSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            if(IsCoreCharacterCard && !Card.IsIncapacitatedOrOutOfGame && HeroTurnTakerController is StarlightTurnTakerController starlightTTC)
            {
                starlightTTC.ManageCharactersOffToTheSide(true);
            }
        }

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();
            if(IsCoreCharacterCard)
            {
                AddSideTrigger(AddTrigger((MakeDecisionAction md) => IsSwapForOtherCardDecision(md), RemoveSubCharactersFromDecision, TriggerType.Hidden, TriggerTiming.After));
            }
        }
        private bool IsSwapForOtherCardDecision(MakeDecisionAction md)
        {
            if(md.Decision is SelectFromBoxDecision sfb && md.CardSource != null && IsHero(md.CardSource.Card))
            {
                //Log.Debug($"TurnTakerIdentifier: {sfb.SelectedTurnTakerIdentifier}");
                return sfb.SelectedTurnTakerIdentifier == "Cauldron.Starlight";
            }
            return false;
        }
        private IEnumerator RemoveSubCharactersFromDecision(MakeDecisionAction md)
        {
            IEnumerator coroutine = DoNothing();
            if(md.Decision is SelectFromBoxDecision sfb && QualifiedNightloreCouncilIdentifiers.Contains(sfb.SelectedIdentifier))
            {
                sfb.SelectedIdentifier = null;
                coroutine = GameController.SendMessageAction($"Sorry, {md.DecisionMaker.Name}, you can't swap in a Nightlore Council character - it breaks things!", Priority.Medium, GetCardSource());
            }
            return coroutine;
        }
        protected bool IsConstellation(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "constellation");
        }
        protected bool IsNextToConstellation(Card card)
        {
            if (card != null && card.NextToLocation != null && card.NextToLocation.Cards != null)
            {
                int num = card.NextToLocation.Cards.Where((Card c) => IsConstellation(c) && c.IsInPlayAndHasGameText).Count();
                return num > 0;
            }
            return false;
        }
        protected IEnumerator SelectActiveCharacterCardToDealDamage(List<Card> storedResults, int? damageAmount = null, DamageType? damageType = null)
        {
            //future-proofing for Nightlore Council
            if (IsMultiCharPromo())
            {
                List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();

                IEnumerator coroutine;
                if (damageAmount == null || damageType == null)
                {
                    coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.HeroCharacterCard, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active Starlight"), storedDecision, optional: false, allowAutoDecide: false, includeRealCardsOnly: true, cardSource: GetCardSource());
                }
                else
                {

                    DealDamageAction previewDamage = new DealDamageAction(GetCardSource(), null, null, (int)damageAmount, (DamageType)damageType);
                    coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.HeroToDealDamage, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active Starlight"), storedDecision, optional: false, allowAutoDecide: false, previewDamage, includeRealCardsOnly: true, GetCardSource());
                }

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
                if (selectCardDecision != null)
                {
                    storedResults.Add(selectCardDecision.SelectedCard);
                }
            }
            else
            {
                storedResults.Add(TurnTaker.CharacterCard);
            }
            yield break;
        }

        protected bool IsMultiCharPromo(bool allowReplacements = true)
        {
            return this.CharacterCards.Count() > 1;
        }
    }
}

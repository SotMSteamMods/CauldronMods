using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class PastTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        private readonly string VigilarKey = "PastKnightVigilarKey";
        public PastTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ReplacesCards);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Play an equipment target next to a hero. Treat {TheKnight}'s name on that equipment as the name of the hero it is next to."

            //"..an equipment target..."
            List<SelectCardDecision> storedEquipment = new List<SelectCardDecision> { };
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.PlayCard, new LinqCardCriteria((Card c) => c.Location == this.HeroTurnTaker.Hand && IsEquipment(c) && c.IsTarget && GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay, "", false, singular:"playable equipment target", plural: "playable equipment targets"), storedEquipment, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if(!DidSelectCard(storedEquipment))
            {
                yield break;
            }
            Card equipment = storedEquipment.FirstOrDefault().SelectedCard;
            //"...next to a hero"
            List<SelectCardDecision> storedHero = new List<SelectCardDecision> { };
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.HeroCharacterCard, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHeroCharacterCard && !c.IsIncapacitated, "hero character"), storedHero, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (!DidSelectCard(storedHero))
            {
                yield break;
            }
            Card hero = storedHero.FirstOrDefault().SelectedCard;

            //...Treat {TheKnight}'s name on that equipment as the name of the hero it is next to."
            GameController.AddCardPropertyJournalEntry(equipment, VigilarKey, hero);

            //"Play an equipment target next to a hero."
            List<bool> wasCardPlayed = new List<bool> { };
            coroutine = GameController.PlayCard(DecisionMaker, equipment, wasCardPlayed: wasCardPlayed, overridePlayLocation: hero.Owner.PlayArea, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(wasCardPlayed.Any() && wasCardPlayed.FirstOrDefault())
            {
                coroutine = GameController.SendMessageAction($"{this.Card.Title} lends {equipment.Title} to {hero.Title}", Priority.Medium, GetCardSource(), new Card[] { equipment });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                GameController.AddCardPropertyJournalEntry(equipment, VigilarKey, (Card)null);
            }

            yield break;
        }

        private bool WasCardPlayedWithVigilar(Card c)
        {
            var property = GameController.GetCardPropertyJournalEntryCard(c, VigilarKey);
            if (property != null)
            {
                return true;
            }
            return false;
        }
        private IEnumerator RemoveVigilarKey(MoveCardAction mc)
        {
            GameController.AddCardPropertyJournalEntry(mc.CardToMove, VigilarKey, (Card)null);
            yield return null;
            yield break;
        }
        public override void AddTriggers()
        {
            AddTrigger((MoveCardAction mc) => mc.Origin.IsInPlay && !mc.Destination.IsInPlay && mc.WasCardMoved && WasCardPlayedWithVigilar(mc.CardToMove), RemoveVigilarKey, TriggerType.Hidden, TriggerTiming.After);
        }
        public override Card AskIfCardIsReplaced(Card originalCard, CardSource effectSource)
        {
            if(effectSource.AllowReplacements && effectSource.Card != null && originalCard == this.CharacterCardWithoutReplacements && WasCardPlayedWithVigilar(effectSource.Card))
            {
                return GameController.GetCardPropertyJournalEntryCard(effectSource.Card, VigilarKey);
            }
            return null;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        
                        //"One player may draw a card now.",
				
				
                        yield break;
                    }
                case 1:
                    {
                        //"Reveal the top card of the villain deck, then replace it.",
                        break;
                    }
                case 2:
                    {
                        //"Select a target, increase the next damage dealt to and by that target by 2."
                        break;
                    }
            }
            yield break;
        }
    }
}
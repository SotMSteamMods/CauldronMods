using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightCharacterCardController : HeroCharacterCardController
    {
        public StarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Draw a card, or play a Constellation from your trash
            IEnumerator coroutine = DrawACardOrPlayConstellationFromTrash();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"Until the start of your next turn, prevent all damage that would be dealt to or by the target with the lowest HP.",
                        break;
                    }
                case 1:
                    {
                        //"1 player may use a power now.",
                        break;
                    }
                case 2:
                    {
                        //"1 hero target regains 2 HP."
                        break;
                    }
            }


            yield break;
        }

        private IEnumerator DrawACardOrPlayConstellationFromTrash()
        {
            List<Function> list = new List<Function>();
            string str = "so they must draw a card.";
            string str2 = "so they must play a constellation from their trash.";
            string str3 = ", so the power has no effect.";
            string str4 = ((DecisionMaker == null) ? Card.Title : DecisionMaker.TurnTaker.Name);
            if ((DecisionMaker != null && DecisionMaker.TurnTaker.Identifier == "Guise") || Card.Identifier == "GuiseCharacter")
            {
                str = "so he's gotta draw one. Woo! Free card!";
                str2 = "so he's gotta play one. Make sure it's a good one!";
                str3 = ". Bummer!";
            }
            list.Add(new Function(DecisionMaker, "Draw a card", SelectionType.DrawCard, () => DrawCard(DecisionMaker.HeroTurnTaker, false), DecisionMaker != null && CanDrawCards(DecisionMaker), str4 + " cannot play any cards, " + str));
            
            list.Add(new Function(DecisionMaker, "Play a constellation from your trash", SelectionType.PlayCard, () => SelectAndPlayConstellationFromTrash(DecisionMaker, false), DecisionMaker != null && GetPlayableConstellationsInTrash().Count() > 0, str4 + " cannot draw any cards, " + str2));

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, null, str4 + " cannot draw nor play any cards" + str3, null, GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator SelectAndPlayConstellationFromTrash(HeroTurnTakerController hero, bool optional)
        {
            IEnumerator coroutine = GameController.SelectAndPlayCard(hero, GetPlayableConstellationsInTrash());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerable<Card> GetPlayableConstellationsInTrash()
        {
            return DecisionMaker.HeroTurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), false, null, false, true) == CanPlayCardResult.CanPlay);  
        }

        private bool IsConstellation(Card card)
        {
            if (card != null)
            {
                return GameController.DoesCardContainKeyword(card, "constellation");
            }
            return false;
        }
    }
}
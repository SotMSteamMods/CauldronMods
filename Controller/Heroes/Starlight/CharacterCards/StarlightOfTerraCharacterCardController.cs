using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Starlight
{
    public class StarlightOfTerraCharacterCardController : StarlightSubCharacterCardController
    {
        public StarlightOfTerraCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            IsCoreCharacterCard = false;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.Identifier == "Exodus", "", useCardsSuffix: false, singular: "copy of Exodus", plural:"copies of Exodus"));
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Discard a card." 
            IEnumerator discard = SelectAndDiscardCards(HeroTurnTakerController, 1);
            //"Search your deck for a copy of Exodus and put it in your hand. Shuffle your deck."
            IEnumerator search = SearchForCards(HeroTurnTakerController, 
                                                searchDeck:true, 
                                                searchTrash:false, 
                                                1, 1, //maximum and required
                                                new LinqCardCriteria((Card c) => c.Identifier == "Exodus", "copy of Exodus"), 
                                                false, true, false, //(Can't/Can/Can't) be put in (Play/Hand/Trash)
                                                autoDecideCard: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(discard);
                yield return GameController.StartCoroutine(search);
            }
            else
            {
                GameController.ExhaustCoroutine(discard);
                GameController.ExhaustCoroutine(search);
            }
            yield break;
        }
    }
}
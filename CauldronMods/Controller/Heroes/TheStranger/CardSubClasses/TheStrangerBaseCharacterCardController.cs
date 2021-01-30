using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.TheStranger
{
    public abstract class TheStrangerBaseCharacterCardController : HeroCharacterCardController
    {
        protected TheStrangerBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(IsRuneCriteria(), owners: new[] { TurnTaker });
            SpecialStringMaker.ShowNumberOfCardsInPlay(IsGlyphCriteria(), owners: new[] { TurnTaker });
        }

        protected LinqCardCriteria IsRuneCriteria(Func<Card, bool> additionalCriteria = null)
        {
            var result = new LinqCardCriteria(c => IsRune(c), "rune", true);
            if (additionalCriteria != null)
                result = new LinqCardCriteria(result, additionalCriteria);

            return result;
        }

        protected LinqCardCriteria IsGlyphCriteria(Func<Card, bool> additionalCriteria = null)
        {
            var result = new LinqCardCriteria(c => IsGlyph(c), "glyph", true);
            if (additionalCriteria != null)
                result = new LinqCardCriteria(result, additionalCriteria);

            return result;
        }

        protected bool IsRune(Card card, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", evenIfUnderCard, evenIfFaceDown);
        }

        protected bool IsGlyph(Card card, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "glyph", evenIfUnderCard, evenIfFaceDown);
        }
    }
}

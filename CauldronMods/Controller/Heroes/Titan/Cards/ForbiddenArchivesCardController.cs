using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Titan
{
    public class ForbiddenArchivesCardController : CardController
    {
        public ForbiddenArchivesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private List<DrawCardAction> _storedResults;

        public override IEnumerator Play()
        {
            _storedResults = new List<DrawCardAction>();
            //Each player may draw 2 cards now.
            IEnumerator coroutine = SelectTurnTakersToDrawCards();
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            var otherDrawingHeroes = new List<HeroTurnTaker> { };
            foreach (DrawCardAction draw in _storedResults)
            {
                if (draw.HeroTurnTaker != this.HeroTurnTaker && !otherDrawingHeroes.Contains(draw.HeroTurnTaker))
                {
                    otherDrawingHeroes.Add(draw.HeroTurnTaker);
                }
            }

            int numSelfDamage = otherDrawingHeroes.Count();
            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            for (int i = 0; i < numSelfDamage; i++)
            {
                coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Psychic, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Would you like to draw 2 cards?", "Should they draw 2 cards?", "Vote for if they should draw 2 cards?", "whether to draw 2 cards");

        }

        private IEnumerator SelectTurnTakersToDrawCards()
        {
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => !tt.IsIncapacitatedOrOutOfGame && IsHero(tt) && GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource())), SelectionType.DrawCard, YesNoResponse, cardSource: GetCardSource()) ;
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator YesNoResponse(TurnTaker tt)
        {
            IEnumerator coroutine;
            if(tt == TurnTaker)
            {
                coroutine = DrawCards(HeroTurnTakerController, 2, optional: true, storedResults: _storedResults);
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
            HeroTurnTakerController httc = FindHeroTurnTakerController(tt.ToHero());
            List<YesNoCardDecision> yesNoStored = new List<YesNoCardDecision>() ;
            coroutine = GameController.MakeYesNoCardDecision(httc, SelectionType.Custom, Card, storedResults: yesNoStored, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidPlayerAnswerYes(yesNoStored))
            {
                yield break;
            }

            coroutine = DrawCards(httc, 2, storedResults: _storedResults);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }

}
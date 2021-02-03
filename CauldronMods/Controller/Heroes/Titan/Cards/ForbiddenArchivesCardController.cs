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

        public override IEnumerator Play()
        {
            List<DrawCardAction> storedResults = new List<DrawCardAction>();
            //Each player may draw 2 cards now.
            IEnumerator coroutine = base.GameController.YesNoDoAction_ManyPlayers((HeroTurnTakerController hero) => !hero.IsIncapacitatedOrOutOfGame, (HeroTurnTakerController hero) => this.YesNoDecisionMaker(hero), (HeroTurnTakerController hero, YesNoDecision decision) => YesAction(hero, decision, storedResults), selectionType: SelectionType.DrawExtraCard, cardSource: base.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            var otherDrawingHeroes = new List<HeroTurnTaker> { };
            foreach (DrawCardAction draw in storedResults)
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

        private YesNoDecision YesNoDecisionMaker(HeroTurnTakerController hero)
        {
            return new YesNoDecision(base.GameController, hero, SelectionType.DrawExtraCard, cardSource: base.GetCardSource());
        }

        private IEnumerator YesAction(HeroTurnTakerController hero, YesNoDecision decision, List<DrawCardAction> storedDraw)
        {
            IEnumerator coroutine = base.DrawCards(hero, 2, storedResults: storedDraw);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Titan
{
    public class ForbiddenArchivesCardController : CardController
    {
        public ForbiddenArchivesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        int count;

        public override IEnumerator Play()
        {
            List<DrawCardAction> storedResults = new List<DrawCardAction>();
            //Each player may draw 2 cards now.
            IEnumerator coroutine = base.GameController.YesNoDoAction_ManyPlayers((HeroTurnTakerController hero) => !hero.IsIncapacitatedOrOutOfGame, (HeroTurnTakerController hero) => this.YesNoDecisionMaker(hero), (HeroTurnTakerController hero, YesNoDecision decision) => YesAction(hero, decision), selectionType: SelectionType.DrawExtraCard, cardSource: base.GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            //For each other player that draws cards this way, {Titan} deals himself 2 psychic damage.
            for (int i = 0; i < count; i++)
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
            return new YesNoDecision(base.GameController, hero, SelectionType.DrawExtraCard, cardSource: base.GetCardSource()); ;
        }

        private IEnumerator YesAction(HeroTurnTakerController hero, YesNoDecision decision)
        {
            IEnumerator coroutine = base.DrawCards(hero, 2);
            if (hero != base.HeroTurnTakerController)
            {
                count++;
            }
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
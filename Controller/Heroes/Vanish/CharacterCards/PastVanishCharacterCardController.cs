using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Vanish
{
    public class PastVanishCharacterCardController : HeroCharacterCardController
    {
        public PastVanishCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowSpecialString(() => "The next time a target enters play, it deals itself 1 energy damage." + (Incap2Uses() > 1 ? $" x{Incap2Uses()}" : ""), () => true);
            ss.Condition = () => Incap2Uses().HasValue;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Draw a card. Increase damage dealt by hero targets by 1 until the end of the turn."
            int increase = GetPowerNumeral(0, 1);

            var coroutine = DrawCard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var effect = new IncreaseDamageStatusEffect(increase);
            effect.SourceCriteria.IsHero = true;
            effect.SourceCriteria.IsTarget = true;
            effect.CardSource = Card;
            effect.UntilThisTurnIsOver(Game);

            coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private const string Incap2Key = "Incap2";

        private int? Incap2Uses()
        {
            return GameController.GetCardPropertyJournalEntryInteger(CharacterCard, Incap2Key);
        }

        private void AddIncap2Use()
        {
            int? uses = Incap2Uses();
            uses = (uses ?? 0) + 1;

            Game.Journal.RecordCardProperties(CharacterCard, Incap2Key, uses);
        }

        private void ResetIncap2Uses()
        {
            Game.Journal.RecordCardProperties(CharacterCard, Incap2Key, (int?)null);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            /*
             * "One player may play a card now.",
                "The next time a target enters play, it deals itself 1 energy damage.",
                "Shuffle up to 2 non-target environment cards into their deck."
             */

            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        IEnumerator coroutine = base.GameController.SelectHeroToPlayCard(this.DecisionMaker, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        AddIncap2Use();
                        //"The next time a target enters play, it deals itself 1 energy damage.",
                        IEnumerator coroutine = GameController.SendMessageAction("Applying ongoing effect: The next time a target enters play, it deals itself 1 energy damage.", Priority.High, GetCardSource(), showCardSource: true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 2:
                    {
                        //"Shuffle up to 2 non-target environment cards into their deck."
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();

                        var env = GameController.FindEnvironmentTurnTakerController();

                        var coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, env.TurnTaker.PlayArea, 0, 2,
                                            new LinqCardCriteria(c => c.IsEnvironment && !c.IsTarget, "non-environment target"),
                                            new[] { new MoveCardDestination(env.TurnTaker.Deck) },
                                            storedResults: storedResults,
                                            shuffleAfterwards: false,
                                            selectionType: SelectionType.ShuffleCardIntoDeck,
                                            cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectCard(storedResults))
                        {
                            coroutine = GameController.ShuffleLocation(env.TurnTaker.Deck, cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
            }
            yield break;
        }

        public override void AddTriggers()
        {
            AddTrigger<CardEntersPlayAction>(cpa => cpa.CardEnteringPlay != null && cpa.CardEnteringPlay.IsTarget && Incap2Uses().HasValue, cpa => TargetEntersPlayResponse(cpa.CardEnteringPlay), TriggerType.DealDamage, TriggerTiming.After, outOfPlayTrigger: true);
        }

        private IEnumerator TargetEntersPlayResponse(Card target)
        {
            int? uses = Incap2Uses();
            if (uses.HasValue)
            {
                ResetIncap2Uses();

                int count = uses.Value;
                while (--count >= 0 && target.IsInPlay)
                {
                    var coroutine = GameController.DealDamageToTarget(new DamageSource(GameController, target), target, 1, DamageType.Energy, cardSource: GetCardSource());
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
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementOfLightningCardController : SpellCardController
    {
        public ElementOfLightningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroWithMostCards(true);
            if (base.CharacterCardController is FutureTiamatCharacterCardController)
            {
                base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => base.IsSpell(c), "spell"));
            }
            else
            {
                base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "ElementOfLightning", "element of lightning"));
            }
        }

        public static readonly string PreventDrawPropertyKey = "ElementOfLightningCannotDraw";

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            Card characterCard = null;
            if (base.CharacterCardController is WinterTiamatCharacterCardController)
            {
                characterCard = base.TurnTaker.FindCard("StormTiamatCharacter");
            }
            if (base.CharacterCardController is HydraWinterTiamatCharacterCardController)
            {
                characterCard = base.TurnTaker.FindCard("HydraStormTiamatCharacter");
            }
            if (base.CharacterCardController is FutureTiamatCharacterCardController)
            {
                characterCard = base.CharacterCard;
            }
            //If {Tiamat}, The Eye of the Storm is active, she deals each hero target 2+X lightning damage, where X is the number of Element of Lightning cards in the villain trash.
            if (characterCard.IsInPlayAndHasGameText && (!characterCard.IsFlipped || base.FindCardController(characterCard) is FutureTiamatCharacterCardController))
            {
                coroutine = base.DealDamage(characterCard, (Card c) => c.IsHero, (Card c) => PlusNumberOfThisCardInTrash(2), DamageType.Lightning);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //The hero with the most cards in hand...
            List<TurnTaker> storedResults = new List<TurnTaker>();
            coroutine = base.FindHeroWithMostCardsInHand(storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults.Any())
            {
                TurnTaker biggestHandTurnTaker = storedResults.First();
                //...may not draw cards until the start of the next villain turn.
                //We secretly set a property on the victim's character card to indicate that they can't draw cards.
                //A CannotDrawCards query on TiamatCharacterCardController actually makes this happen
                GameController.AddCardPropertyJournalEntry(biggestHandTurnTaker.CharacterCard, PreventDrawPropertyKey, true);

                //This status effect makes them able to draw again at start of next villain
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(DoNothing), $"{biggestHandTurnTaker.Name} cannot draw cards.", new TriggerType[] { TriggerType.CreateStatusEffect }, base.Card);
                effect.UntilStartOfNextTurn(base.TurnTaker);
                effect.TurnTakerCriteria.IsSpecificTurnTaker = base.TurnTaker;
                effect.TurnPhaseCriteria.Phase = Phase.Start;
                effect.CanEffectStack = true;
                coroutine = base.AddStatusEffect(effect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public IEnumerator ResumeDrawEffect(PhaseChangeAction _, OnPhaseChangeStatusEffect _2)
        {
            System.Console.WriteLine("### DEBUG ### - ElementOfLightning.ResumeDrawEffect triggered");

            //Clear the secret property from all Character Cards 
            foreach (HeroTurnTaker hero in Game.HeroTurnTakers)
            {
                GameController.AddCardPropertyJournalEntry(hero.CharacterCard, PreventDrawPropertyKey, (bool?)null);
            }
            yield break;
        }
    }
}
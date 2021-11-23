using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class FirstResponseCricketCharacterCardController : HeroCharacterCardController
    {
        public FirstResponseCricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine = null;
            switch (index)
            {
                case 0:
                    {
                        //One player may play a card now.
                        coroutine = base.GameController.SelectHeroToPlayCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        //Shuffle the environment trash into the environment deck.
                        List<SelectLocationDecision> storedLocation = new List<SelectLocationDecision>();
                        Location envDeck = base.Game.EnvironmentTurnTakers.FirstOrDefault().Deck;
                        if (base.Game.EnvironmentTurnTakers.Count() > 1)
                        {
                            coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.ShuffleTrashIntoDeck, (Location l) => l.IsEnvironment, storedLocation, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            if (DidSelectLocation(storedLocation))
                            {
                                envDeck = GetSelectedLocation(storedLocation);
                            }
                        }
                        coroutine = base.GameController.ShuffleTrashIntoDeck(base.TurnTakerController, overrideDeck: envDeck);
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
                        //1 hero target regains 2 HP.
                        List<SelectTargetDecision> selectedTarget = new List<SelectTargetDecision>();
                        IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHero && c.IsTarget && c.IsInPlayAndHasGameText));
                        coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, choices, selectedTarget, selectionType: SelectionType.GainHP, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (selectedTarget != null && selectedTarget.Any())
                        {
                            SelectTargetDecision selectedTargetDecision = selectedTarget.FirstOrDefault();
                            if(selectedTargetDecision != null && selectedTargetDecision.SelectedCard != null)
                            {
                                coroutine = base.GameController.GainHP(selectedTarget.FirstOrDefault().SelectedCard, 2, cardSource: base.GetCardSource());
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
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //{Cricket} deals 2 targets 1 sonic damage each. You may use a power now.
            int targetNumeral = GetPowerNumeral(0, 2);
            int damageNumeral = GetPowerNumeral(1, 1);

            //{Cricket} deals 2 targets 1 sonic damage each.
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Sonic, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may use a power now.
            coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
    }
}
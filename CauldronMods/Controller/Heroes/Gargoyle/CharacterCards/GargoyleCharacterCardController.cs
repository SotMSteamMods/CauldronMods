using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class GargoyleCharacterCardController : HeroCharacterCardController
    {
        private const int REDUCE_DAMAGE_POWER = 1;
        private const int REDUCE_DAMAGE_INCAP = 2;
        private const int INCREASE_DAMAGE_POWER = 1;
        private const int INCREASE_DAMAGE_INCAP = 2;
        private const int NUMBER_USES_ONCE = 1;

        public GargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        /*
         * Power:
         * "Select a target. Reduce the next damage it deals by 1. Increase the next damage Gargoyle deals by 1."
         */
        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText));
            ReduceDamageStatusEffect reduceDamageStatusEffect;
            IncreaseDamageStatusEffect increaseDamageStatusEffect; 
            IEnumerator coroutine;
            Card selectedCard;

            // Select a target. 
            coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, choices, storedResults, selectionType: SelectionType.None, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.FirstOrDefault().SelectedCard != null)
            {
                selectedCard = storedResults.FirstOrDefault().SelectedCard;

                // Reduce the next damage it deals by 1
                reduceDamageStatusEffect = new ReduceDamageStatusEffect(REDUCE_DAMAGE_POWER)
                {
                    SourceCriteria = { IsSpecificCard = selectedCard },
                    NumberOfUses = NUMBER_USES_ONCE
                };
                coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // Increase the next damage Gargoyle deals by 1
                increaseDamageStatusEffect = new IncreaseDamageStatusEffect(INCREASE_DAMAGE_POWER)
                {
                    SourceCriteria = { IsSpecificCard = base.CharacterCard },
                    NumberOfUses = NUMBER_USES_ONCE
                };
                coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
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

        /* 
         * Incap
         * "Increase the next damage dealt by a hero target by 2."
         * "Reduce the next damage dealt to a hero target by 2."
         * "One player may draw a card now."
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;

            switch (index)
            {
                // "Increase the next damage dealt by a hero target by 2."
                case 0:
                    {
                        coroutine = this.DoIncapacitateOption1();
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
                    // "Reduce the next damage dealt to a hero target by 2."
                    {
                        coroutine = this.DoIncapacitateOption2();
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
                    // "One player may draw a card now."
                    {
                        coroutine = this.DoIncapacitateOption3();
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
            }
            yield break;
        }

        private IEnumerator DoIncapacitateOption1()
        {
            //==============================================================
            // Increase the next damage dealt by a hero target by 2.
            //==============================================================            
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(INCREASE_DAMAGE_INCAP)
            {
                SourceCriteria = { IsHeroCharacterCard = true },
                NumberOfUses = NUMBER_USES_ONCE
            };
            return base.AddStatusEffect(increaseDamageStatusEffect);
        }

        private IEnumerator DoIncapacitateOption2()
        {
            //==============================================================
            // Reduce the next damage dealt to a hero target by 2.
            //==============================================================
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(REDUCE_DAMAGE_INCAP)
            {
                SourceCriteria = { IsHeroCharacterCard = true },
                NumberOfUses = NUMBER_USES_ONCE
            };
            return base.AddStatusEffect(reduceDamageStatusEffect);
        }

        private IEnumerator DoIncapacitateOption3()
        {
            //==============================================================
            // One player may draw a card now.
            //==============================================================
            return base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController, optionalDrawCard: true, numberOfCards: new int?(1), cardSource: base.GetCardSource(null));
        }

    }
}

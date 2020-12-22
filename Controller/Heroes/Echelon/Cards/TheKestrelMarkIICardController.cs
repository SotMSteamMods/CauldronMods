using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class TheKestrelMarkIICardController : CardController
    {
        //==============================================================
        // Power: {Echelon} deals 1 target 1 projectile damage and regains 2HP.
        // Power: Destroy this card. One player draws 2 cards.
        //==============================================================

        public static string Identifier = "TheKestrelMarkII";

        private const int DamageToDeal = 1;
        private const int HpGain = 2;
        private const int CardsToDraw = 2;

        public TheKestrelMarkIICardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator routine;
            switch (index)
            {
                case 0:

                    //{Echelon} deals 1 target 1 projectile damage and regains 2HP.

                    DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
                    int targetsNumeral = GetPowerNumeral(0, 1);
                    int damageNumeral = GetPowerNumeral(1, DamageToDeal);
                    int hpNumeral = GetPowerNumeral(2, HpGain);

                    routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                        damageNumeral,
                        DamageType.Projectile, targetsNumeral, false, targetsNumeral,
                        cardSource: base.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    routine = this.GameController.GainHP(this.CharacterCard, hpNumeral, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;


                case 1:

                    // Destroy this card. One player draws 2 cards.

                    int cardsNumeral = GetPowerNumeral(0, CardsToDraw);

                    routine = base.GameController.SelectHeroToDrawCards(this.HeroTurnTakerController, cardsNumeral, optionalDrawCards: false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    routine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    break;
            }
        }
    }
}
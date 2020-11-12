using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class TangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 1;

        public TangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // {TangoOne} deals 1 target 1 projectile damage.
            //==============================================================

            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int powerNumeral = base.GetPowerNumeral(0, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                powerNumeral,
                DamageType.Projectile, new int?(1), false, new int?(1),
                cardSource: base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator incapRoutine = null;
            switch (index)
            {
                case 0:
                    incapRoutine = GetIncapacitateOption1();
                    break;

                case 1:
                    incapRoutine = GetIncapacitateOption2();
                    break;

                case 2:
                    incapRoutine = GetIncapacitateOption3();
                    break;
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(incapRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(incapRoutine);
            }
        }

        private IEnumerator GetIncapacitateOption1()
        {
            //==============================================================
            // One player may draw a card now.
            //==============================================================

            yield break;
        }

        private IEnumerator GetIncapacitateOption2()
        {
            //==============================================================
            // Reveal the top card of a deck, then replace it or discard it.
            //==============================================================

            yield break;
        }

        private IEnumerator GetIncapacitateOption3()
        {
            //==============================================================
            // Up to 2 ongoing hero cards may be played now.
            //==============================================================

            yield break;
        }
    }
}

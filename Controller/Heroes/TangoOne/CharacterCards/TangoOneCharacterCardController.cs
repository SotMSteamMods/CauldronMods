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

        public TangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // {TangoOne} deals 1 target 1 projectile damage.
            //==============================================================

            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int powerNumeral = base.GetPowerNumeral(0, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource, powerNumeral,
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
            return base.UseIncapacitatedAbility(index);
        }
    }
}

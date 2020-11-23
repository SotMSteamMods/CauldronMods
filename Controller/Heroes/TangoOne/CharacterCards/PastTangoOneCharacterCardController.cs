using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PastTangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int Incapacitate3CardsToDestroy = 1;

        public PastTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Select a target, at the start of your next turn,
            // {TangoOne} deals that target 3 projectile damage.
            //==============================================================

            return base.UsePower(index);
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may draw 2 cards.
                    //==============================================================

                    


                    break;

                case 1:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may play 2 cards.
                    //==============================================================



                    break;

                case 2:

                    //==============================================================
                    // Destroy an environment card.
                    //==============================================================

                    IEnumerator destroyRoutine 
                        = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController, 
                            new LinqCardCriteria(c => c.IsEnvironment, "environment"), Incapacitate3CardsToDestroy, optional: false, 0, null, null, null, ignoreBattleZone: false, null, null, null, GetCardSource());
                    
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(destroyRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(destroyRoutine);
                    }

                    break;
            }
        }
    }
}

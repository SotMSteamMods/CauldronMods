using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class HackingProgramCardController : CardController
    {
        //==============================================================
        // Power: {Cypher} deals himself 2 irreducible energy damage.
        // If he takes damage this way, destroy 1 ongoing or environment card.
        //==============================================================

        public static string Identifier = "HackingProgram";

        private const int DamageToDealSelf = 2;
        private const int EnvOrOngoingToDestroy = 1;

        public HackingProgramCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            int powerNumeral = GetPowerNumeral(0, DamageToDealSelf);
            int toDestroyNumeral = GetPowerNumeral(1, EnvOrOngoingToDestroy);

            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator routine = this.GameController.DealDamageToSelf(this.DecisionMaker, c => c == this.CharacterCard, powerNumeral,
                DamageType.Energy, true, storedResults, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (DidDealDamage(storedResults, this.CharacterCard))
            {
                routine = base.GameController.SelectAndDestroyCards(base.HeroTurnTakerController,
                    new LinqCardCriteria(c => (c.IsEnvironment || IsOngoing(c)) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "environment or ongoing"),
                    toDestroyNumeral, requiredDecisions: toDestroyNumeral, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            yield break;
        }
    }
}
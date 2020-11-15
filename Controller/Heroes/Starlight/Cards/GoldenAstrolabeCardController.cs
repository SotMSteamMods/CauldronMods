using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class GoldenAstrolabeCardController : StarlightCardController
    {
        public GoldenAstrolabeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Starlight deals herself 2 energy damage. One hero character next to a constellation may use a power now."
            int powerNumeral = GetPowerNumeral(0, 2);
            List<Card> storedResults = new List<Card> { };
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, powerNumeral, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseDamageSource);
            }

            Card damageSource = storedResults.FirstOrDefault();
            IEnumerator selfDamage = DealDamage(damageSource, damageSource, powerNumeral, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selfDamage);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selfDamage);
            }

            LinqTurnTakerCriteria ttCriteria = new LinqTurnTakerCriteria((TurnTaker tt) => 
                                                        tt is HeroTurnTaker && 
                                                        !tt.IsIncapacitatedOrOutOfGame && 
                                                        tt.HasCardsWhere((Card c) => c.IsHeroCharacterCard && IsNextToConstellation(c)) && 
                                                        GetUsablePowersFromAllowedSource(GameController.FindHeroTurnTakerController(tt as HeroTurnTaker)).Count() > 0);

            //because of Nightlore Council Starlight, Sentinels, and others, restricting power use to either:
            //A: non-character card
            //B: character card next to constellation

            var storedTurnTakerDecision = new List<SelectTurnTakerDecision> { };
            IEnumerator pickHeroToUsePower = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.UsePower, optional: false, allowAutoDecide: false, storedResults: storedTurnTakerDecision, heroCriteria: ttCriteria);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(pickHeroToUsePower);
            }
            else
            {
                base.GameController.ExhaustCoroutine(pickHeroToUsePower);
            }

            TurnTaker turnTaker = (from d in storedTurnTakerDecision
                                   where d.Completed
                                   select d.SelectedTurnTaker).FirstOrDefault();

            if (turnTaker != null && turnTaker is HeroTurnTaker)
            {
                HeroTurnTakerController powerUser = GameController.FindHeroTurnTakerController(turnTaker as HeroTurnTaker);

                if (powerUser != null)
                {
                    Func<Power, bool> powerCriteria = (Power p) => !p.CardSource.Card.IsHeroCharacterCard || IsNextToConstellation(p.CardSource.Card);
                    var usePower = GameController.SelectAndUsePower(powerUser, optional:true, powerCriteria);
                    if(base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(usePower);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(usePower);
                    }
                }
            }
            yield break;
        }

        private List<Power> GetUsablePowersFromAllowedSource(HeroTurnTakerController powerUser)
        {
            List<Power> allUsablePowers = GameController.GetUsablePowersThisTurn(powerUser).ToList();
            List<Power> powerOnGoodCard = (from p in allUsablePowers where !p.CardSource.Card.IsHeroCharacterCard || IsNextToConstellation(p.CardSource.Card) select p).ToList();
            return powerOnGoodCard;
        }
    }
}
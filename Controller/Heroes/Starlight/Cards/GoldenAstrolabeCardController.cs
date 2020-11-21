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
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDamageSource);
            }
            Card damageSource = storedResults.FirstOrDefault();

            //"Starlight deals herself 2 energy damage."
            IEnumerator selfDamage = DealDamage(damageSource, damageSource, powerNumeral, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(selfDamage);
            }
            else
            {
                GameController.ExhaustCoroutine(selfDamage);
            }

            LinqTurnTakerCriteria ttCriteria = new LinqTurnTakerCriteria((TurnTaker tt) =>
                                                        tt is HeroTurnTaker &&
                                                        !tt.IsIncapacitatedOrOutOfGame &&
                                                        tt.HasCardsWhere((Card c) => c.IsHeroCharacterCard && IsNextToConstellation(c)) &&
                                                        GetUsablePowersFromAllowedSource(GameController.FindHeroTurnTakerController(tt.ToHero())).Count() > 0);

            //because of Nightlore Council Starlight, Sentinels, and others, restricting power use to either:
            //A: non-character card
            //B: character card next to constellation


            //"One hero character next to a constellation..." 
            var storedTurnTakerDecision = new List<SelectTurnTakerDecision> { };
            IEnumerator pickHeroToUsePower = GameController.SelectHeroTurnTaker(HeroTurnTakerController, SelectionType.UsePower, optional: false, allowAutoDecide: false, storedResults: storedTurnTakerDecision, heroCriteria: ttCriteria, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(pickHeroToUsePower);
            }
            else
            {
                GameController.ExhaustCoroutine(pickHeroToUsePower);
            }

            TurnTaker turnTaker = (from d in storedTurnTakerDecision
                                   where d.Completed
                                   select d.SelectedTurnTaker).FirstOrDefault();

            if (turnTaker != null && turnTaker is HeroTurnTaker)
            {
                HeroTurnTakerController powerUser = GameController.FindHeroTurnTakerController(turnTaker.ToHero());

                if (powerUser != null)
                {
                    //"...may use a power now."
                    var usePower = GameController.SelectAndUsePower(powerUser, optional: true, PowerCriteria);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(usePower);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(usePower);
                    }
                }
            }
            yield break;
        }

        private List<Power> GetUsablePowersFromAllowedSource(HeroTurnTakerController powerUser)
        {
            return GameController.GetUsablePowersThisTurn(powerUser).Where(PowerCriteria).ToList();
        }

        private bool PowerCriteria(Power p)
        {
            return !p.CardController.Card.IsHeroCharacterCard || IsNextToConstellation(p.CardController.Card);
        }
    }
}
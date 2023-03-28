using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class TornPageCardController : MythosUtilityCardController
    {
        public TornPageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override void ShowUniqueSpecialStrings()
        {
            SpecialStringMaker.ShowSpecialString(() => BuildTornPageSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeActivated).Condition = () => Card.IsInPlayAndHasGameText;

        }

        private const string FirstTimeActivated = "FirstTimeActivated";


        public override void AddTriggers()
        {
            //The first time each turn that:
            //{MythosDanger} a hero card is drawn...
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && base.IsTopCardMatching(MythosDangerDeckIdentifier) && action.DidDrawCard, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //{MythosMadness} a hero card enters play...
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && base.IsTopCardMatching(MythosMadnessDeckIdentifier) && IsHero(action.CardEnteringPlay) && action.IsSuccessful, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //{MythosClue} a power is used...
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && base.IsTopCardMatching(MythosClueDeckIdentifier) && action.IsSuccessful, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //...this card deals that hero 2 infernal damage and 2 psychic damage.
        }

        private IEnumerator DealDamageResponse(GameAction action)
        {

            IEnumerator coroutine;
            //...this card deals that hero 2 infernal damage and 2 psychic damage.
            Card target = null;
            HeroTurnTaker htt = null;
            if (action is UsePowerAction upa)
            {
                htt = upa.HeroUsingPower.HeroTurnTaker;
            } else if(action is DrawCardAction dca)
            {
                htt = dca. HeroTurnTaker;
            } else if(action is CardEntersPlayAction cpa)
            {
                htt = cpa.CardEnteringPlay.Owner.ToHero();
            }
            else
            {
                //its some other action that we aren't expecting
                yield break;
            }

            SetCardPropertyToTrueIfRealAction(FirstTimeActivated);


            if (htt.HasMultipleCharacterCards)
            {
                List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DealDamage, new LinqCardCriteria((Card c) => c.Owner == htt &&  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame), storedResults, false, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if(!DidSelectCard(storedResults))
                {
                    yield break;
                }
                target = GetSelectedCard(storedResults);
            } else
            {
                target = htt.CharacterCard;
            }

            coroutine = base.DealMultipleInstancesOfDamage(new List<DealDamageAction>()
                { 
                    //...2 infernal damage...
                    new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Infernal),
                    //...and 2 psychic damage.
                    new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.Card), null, 2, DamageType.Psychic)
                }, (Card c) => c == target);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private string BuildTornPageSpecialString()
        {
            string special = $"{Card.Title} will react to ";
            if(IsTopCardMatching(MythosDangerDeckIdentifier))
            {
                special += "card draws.";
            }

            if(IsTopCardMatching(MythosMadnessDeckIdentifier))
            {
                special += "cards entering play.";
            }

            if(IsTopCardMatching(MythosClueDeckIdentifier))
            {
                special += "power uses.";
            }

            return special;
        }

    }
}

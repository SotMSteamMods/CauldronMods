using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Starlight
{
    public class NightloreCouncilStarlightCharacterCardController : StarlightSubCharacterCardController
    {
        public NightloreCouncilStarlightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private Card _terra = null;
        private Card _asheron = null;
        private Card _cryos = null;

        private Card terra
        {
            get
            {
                if (_terra == null)
                {
                    _terra = GameController.FindCard("StarlightOfTerraCharacter");
                }
                return _terra;
            }
        }
        private Card asheron
        {
            get
            {
                if (_asheron == null)
                {
                    _asheron = GameController.FindCard("StarlightOfAsheronCharacter");
                }
                return _asheron;
            }
        }
        private Card cryos
        {
            get
            {
                if (_cryos == null)
                {
                    _cryos = GameController.FindCard("StarlightOfCryosFourCharacter");
                }
                return _cryos;
            }
        }
        public override void AddStartOfGameTriggers()
        {
            var cards = (HeroTurnTakerController as StarlightTurnTakerController).LoadSubCharactersAndReturnThem();

            _terra = cards.Where((Card c) => c.Identifier == "StarlightOfTerraCharacter").FirstOrDefault();
            _asheron = cards.Where((Card c) => c.Identifier == "StarlightOfAsheronCharacter").FirstOrDefault();
            _cryos = cards.Where((Card c) => c.Identifier == "StarlightOfCryosFourCharacter").FirstOrDefault();

            AddStartOfTurnTrigger((TurnTaker tt) => TurnTaker == this.TurnTaker,
                            (PhaseChangeAction pca) => TerraHealTeamResponse(),
                            TriggerType.GainHP);
            AddIncreaseDamageTrigger(AsheronBoostDamageCriteria, 1);
            AddTrigger<CardEntersPlayAction>(CryosCardDrawCriteria, (CardEntersPlayAction cep) => DrawCard(), new List<TriggerType> { TriggerType.DrawCard }, TriggerTiming.After);
        }

        public IEnumerator TerraHealTeamResponse()
        {
            //"If Starlight of Terra has a constellation next to her at the start of your turn, each Starlight regains 1 HP."
            if (IsNextToConstellation(terra))
            {
                IEnumerator heal = GameController.SelectAndGainHP(ToHeroTurnTakerController(TurnTaker), 1, false, (Card c) => c == terra || c == asheron || c == cryos, 3, 3, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(heal);
                }
                else
                {
                    GameController.ExhaustCoroutine(heal);
                }
            }
            yield break;
        }
        public bool AsheronBoostDamageCriteria(DealDamageAction dda)
        {
            //"If Starlight of Asheron has a constellation next to her, increase damage she does to non-hero targets by 1."
            return dda.DamageSource.IsSameCard(asheron) && IsNextToConstellation(asheron) && !dda.Target.IsHero;
        }
        public bool CryosCardDrawCriteria(CardEntersPlayAction cep)
        {
            //"Whenever a Constellation enters play next to Starlight of Cryos-4, draw a card."
            if (IsConstellation(cep.CardEnteringPlay))
            {
                var location = cep.CardEnteringPlay.Location;
                if (location != null && location.IsNextToCard && location.OwnerCard != null)
                    return location.OwnerCard == cryos;
            }
            return false;
        }

        public override void AddSideTriggers()
        {
            if (!base.Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger(FlipCriteria, (GameAction ga) => base.GameController.FlipCard(FindCardController(base.Card), treatAsPlayed: false, treatAsPutIntoPlay: false, null, null, GetCardSource()), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddSideTriggers(AddTargetEntersPlayTrigger((Card c) => base.Card.IsFlipped && base.CharacterCards.Contains(c), (Card c) => base.GameController.FlipCard(FindCardController(base.Card), treatAsPlayed: false, treatAsPutIntoPlay: false, null, null, GetCardSource()), TriggerType.Hidden, TriggerTiming.After, isConditional: false, outOfPlayTrigger: true));
            }
        }

        private bool FlipCriteria(GameAction ga)
        {
            return((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !base.Card.IsFlipped && FindCardsWhere((Card c) => c.Owner == base.TurnTaker && c.IsHeroCharacterCard && c.IsActive && c != base.Card).Count() == 0) ? true : false;
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            RemoveAllTriggers();
            AddSideTriggers();
            yield return null;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        var coroutine = GameController.SelectHeroToPlayCard(HeroTurnTakerController, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
                case 1:
                    {
                        //"One play may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        break;
                    }
                case 2:
                    {
                        //"The next time a hero target deals damage, choose the type of that damage."
                        OnDealDamageStatusEffect chooseNextHeroDamageType = new OnDealDamageStatusEffect(this.Card,
                                                                            "ChooseDamageTypeResponse",
                                                                            "The next time a hero target deals damage, Starlight chooses the type of that damage.",
                                                                            new TriggerType[1] { TriggerType.ChangeDamageType },
                                                                            TurnTaker,
                                                                            this.Card);
                        chooseNextHeroDamageType.NumberOfUses = 1;
                        chooseNextHeroDamageType.SourceCriteria.IsHero = true;
                        chooseNextHeroDamageType.SourceCriteria.IsTarget = true;
                        IEnumerator coroutine3 = AddStatusEffect(chooseNextHeroDamageType);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine3);
                        }
                        break;
                    }
            }


            yield break;
        }

        private IEnumerator ChooseDamageTypeResponse(DealDamageAction dd)
        {
            var coroutine = SelectDamageTypeForDealDamageAction(dd);
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
    }
}
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
            SpecialStringMaker.ShowSpecialString(() => $"{terra.Title} is next to a constellation", relatedCards: () => new Card[] { terra }).Condition = () => IsNextToConstellation(terra);
            SpecialStringMaker.ShowSpecialString(() => $"{asheron.Title} is next to a constellation", relatedCards: () => new Card[] { asheron }).Condition = () => IsNextToConstellation(asheron);
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(
                                                        (Card c) => (c == terra || c == asheron || c == cryos) &&
                                                                        c.NextToLocation.Cards.Any((Card nextTo) => nextTo.Identifier == "RetreatIntoTheNebula"),
                                                        "",
                                                        useCardsSuffix: false,
                                                        singular: "Starlight being protected by Retreat into the Nebula",
                                                        plural: "Starlights being protected by Retreat into the Nebula"))
                                    .Condition = () => FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "RetreatIntoTheNebula")).Any();
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
            if(Card.IsIncapacitatedOrOutOfGame)
            {
                return;
            }
            if (HeroTurnTakerController is StarlightTurnTakerController starlightTTC)
            {

                var cards = starlightTTC.ManageCharactersOffToTheSide(false);

                _terra = cards.Where((Card c) => c.Identifier == "StarlightOfTerraCharacter").FirstOrDefault();
                _asheron = cards.Where((Card c) => c.Identifier == "StarlightOfAsheronCharacter").FirstOrDefault();
                _cryos = cards.Where((Card c) => c.Identifier == "StarlightOfCryosFourCharacter").FirstOrDefault();

                AddStartOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker && IsNextToConstellation(terra),
                                (PhaseChangeAction pca) => TerraHealTeamResponse(),
                                TriggerType.GainHP);
                AddIncreaseDamageTrigger(AsheronBoostDamageCriteria, 1);
                AddTrigger<CardEntersPlayAction>(CryosCardDrawCriteria, (CardEntersPlayAction cep) => DrawCard(), new List<TriggerType> { TriggerType.DrawCard }, TriggerTiming.After);
            }
        }

        public IEnumerator TerraHealTeamResponse()
        {
            //"If Starlight of Terra has a constellation next to her at the start of your turn, each Starlight regains 1 HP."
            IEnumerator heal = GameController.GainHP(ToHeroTurnTakerController(TurnTaker), (Card c) => c == terra || c == asheron || c == cryos, 1, optional: false, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(heal);
            }
            else
            {
                GameController.ExhaustCoroutine(heal);
            }
            yield break;
        }
        public bool AsheronBoostDamageCriteria(DealDamageAction dda)
        {
            //"If Starlight of Asheron has a constellation next to her, increase damage she does to non-hero targets by 1."
            return dda.DamageSource.IsSameCard(asheron) && IsNextToConstellation(asheron) && !IsHeroTarget(dda.Target);
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
            return ((ga is FlipCardAction || ga is BulkRemoveTargetsAction || ga is MoveCardAction) && !base.Card.IsFlipped && FindCardsWhere((Card c) => c.Owner == base.TurnTaker &&  IsHeroCharacterCard(c) && c.IsActive && c != base.Card).Count() == 0);
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
                        OnDealDamageStatusEffect chooseNextHeroDamageType = new OnDealDamageStatusEffect(CardWithoutReplacements,
                                                                                    nameof(ChooseDamageTypeResponse),
                                                                                    "The next time a hero target deals damage, Starlight chooses the type of that damage.",
                                                                                    new TriggerType[1] { TriggerType.ChangeDamageType },
                                                                                    TurnTaker,
                                                                                    this.Card);
                        chooseNextHeroDamageType.NumberOfUses = 1;
                        chooseNextHeroDamageType.SourceCriteria.IsHero = true;
                        chooseNextHeroDamageType.SourceCriteria.IsTarget = true;
                        chooseNextHeroDamageType.BeforeOrAfter = BeforeOrAfter.Before;
                        chooseNextHeroDamageType.CanEffectStack = true;
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

        public IEnumerator ChooseDamageTypeResponse(DealDamageAction dd, HeroTurnTaker hero = null, StatusEffect effect = null, int[] powerNumerals = null)
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

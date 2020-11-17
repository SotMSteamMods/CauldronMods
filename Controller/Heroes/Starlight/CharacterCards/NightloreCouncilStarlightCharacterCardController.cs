using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Starlight
{
    public class NightloreCouncilStarlightCharacterCardController : HeroCharacterCardController
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
            Log.Debug("Loading sub-characters...");
            var cards = (HeroTurnTakerController as StarlightTurnTakerController).LoadSubCharactersAndReturnThem();
            Log.Debug("Load routine complete...");

            _terra = cards.Where((Card c) => c.Identifier == "StarlightOfTerraCharacter").FirstOrDefault();
            Log.Debug(_terra.Title + " stored in _terra");
            _asheron = cards.Where((Card c) => c.Identifier == "StarlightOfAsheronCharacter").FirstOrDefault();
            Log.Debug(_asheron.Title + " stored in _asheron");
            _cryos = cards.Where((Card c) => c.Identifier == "StarlightOfCryosFourCharacter").FirstOrDefault();
            Log.Debug(_cryos.Title + " stored in _cryos");

            AddStartOfTurnTrigger((TurnTaker tt) => TurnTaker == this.TurnTaker,
                            (PhaseChangeAction pca) => TerraHealTeamResponse(),
                            TriggerType.GainHP);
            AddIncreaseDamageTrigger(AsheronBoostDamageCriteria, 1);
            AddTrigger<CardEntersPlayAction>(CryosCardDrawCriteria, (CardEntersPlayAction cep) => DrawCard(), new List<TriggerType> { TriggerType.DrawCard }, TriggerTiming.After);
        }

        public IEnumerator TerraHealTeamResponse()
        {
            Log.Debug("TerraHealTeamResponse activates");
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
            return dda.DamageSource.IsSameCard(asheron) && IsNextToConstellation(asheron) && !dda.Target.IsHero;
        }
        public bool CryosCardDrawCriteria(CardEntersPlayAction cep)
        {
            if (IsConstellation(cep.CardEnteringPlay))
            {
                var enteringBy = FindCardsWhere((Card c) => c.IsInPlay && c.GetAllNextToCards(false).Contains(cep.CardEnteringPlay)).FirstOrDefault();
                return enteringBy == cryos;
            }
            return false;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        var coroutine = GameController.SendMessageAction("Temporarily out of order, very sorry.", Priority.High, GetCardSource());
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
                        //"1 player may use a power now.",
                        IEnumerator coroutine2 = GameController.SelectHeroToUsePower(HeroTurnTakerController, optionalSelectHero: false, optionalUsePower: true, allowAutoDecide: false, null, null, null, omitHeroesWithNoUsablePowers: true, canBeCancelled: true, GetCardSource());
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
                        //"1 hero target regains 2 HP."
                        IEnumerator coroutine3 = GameController.SelectAndGainHP(HeroTurnTakerController, 2, optional: false, (Card c) => c.IsInPlay && c.IsHero && c.IsTarget, 1, null, allowAutoDecide: false, null, GetCardSource());
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
        private bool IsConstellation(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "constellation");
        }
        protected bool IsNextToConstellation(Card card)
        {
            if (card != null && card.NextToLocation != null && card.NextToLocation.Cards != null)
            {
                int num = card.NextToLocation.Cards.Where((Card c) => IsConstellation(c) && c.IsInPlayAndHasGameText).Count();
                return num > 0;
            }
            return false;
        }
    }
}
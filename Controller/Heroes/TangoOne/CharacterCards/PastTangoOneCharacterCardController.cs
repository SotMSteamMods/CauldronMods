using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class PastTangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageToDeal = 3;
        private const int Incapacitate1CardsToDraw = 2;
        private const int Incapacitate2CardsToPlay = 2;
        private const int Incapacitate3CardsToDestroy = 1;

        private Card _innatePowerSelectedTarget = null;
        private Card _incapSelectedHero = null;


        public PastTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Select a target, at the start of your next turn,
            // {TangoOne} deals that target 3 projectile damage.
            //==============================================================

            IEnumerable<Card> targets = FindCardsWhere(card => card.IsTarget && card.IsInPlay);

            List<SelectTargetDecision> storedResults = new List<SelectTargetDecision>();
            IEnumerator selectTargetRoutine
                = base.GameController.SelectTargetAndStoreResults(this.HeroTurnTakerController, targets, storedResults, false,
                    false, null, this.Card, card => 3, DamageType.Projectile, false, 
                    null, null, null, null, null,
                    SelectionType.SelectTargetNoDamage, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectTargetRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectTargetRoutine);
            }

            _innatePowerSelectedTarget = storedResults.FirstOrDefault()?.SelectedCard;

            IEnumerator addStatusEffectRoutine = base.GameController.AddStatusEffect(
                MakeOnPhaseChangeStatusEffect(nameof(StartOfTurnDealDamageResponse), 
                    $"{base.Card.Title} will deal {PowerDamageToDeal} projectile damage to {_innatePowerSelectedTarget?.Title} at the start of her next turn"), 
                true, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(addStatusEffectRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(addStatusEffectRoutine);
            }
        }

        public IEnumerator StartOfTurnDealDamageResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            if (_innatePowerSelectedTarget == null)
            {
                yield break;
            }

            int powerNumeral = GetPowerNumeral(0, PowerDamageToDeal);
            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, _innatePowerSelectedTarget, powerNumeral, DamageType.Projectile);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }

            _innatePowerSelectedTarget = null;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may draw 2 cards.
                    //==============================================================

                    List<SelectCardDecision> heroToDrawStoredResults = new List<SelectCardDecision>();
                    IEnumerator selectDrawHeroRoutine = base.GameController.SelectHeroCharacterCard(this.HeroTurnTakerController, SelectionType.CharacterCard,
                        heroToDrawStoredResults, false, false, base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(selectDrawHeroRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(selectDrawHeroRoutine);
                    }

                    _incapSelectedHero = heroToDrawStoredResults.FirstOrDefault()?.SelectedCard;

                    IEnumerator addStatusEffectRoutine = base.GameController.AddStatusEffect(
                        MakeOnPhaseChangeStatusEffect(nameof(StartOfTurnDrawCardsResponse),
                        $"{_incapSelectedHero?.Title} may draw {Incapacitate1CardsToDraw} cards at the start of {this.Card.Title}'s next turn"), 
                        true, this.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(addStatusEffectRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(addStatusEffectRoutine);
                    }

                    break;

                case 1:

                    //==============================================================
                    // Select a player, at the start of your next turn, they may play 2 cards.
                    //==============================================================

                    List<SelectCardDecision> heroToPlayStoredResults = new List<SelectCardDecision>();
                    IEnumerator selectPlayHeroRoutine = base.GameController.SelectHeroCharacterCard(this.HeroTurnTakerController, SelectionType.CharacterCard,
                        heroToPlayStoredResults, false, false, base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(selectPlayHeroRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(selectPlayHeroRoutine);
                    }

                    _incapSelectedHero = heroToPlayStoredResults.FirstOrDefault()?.SelectedCard;

                    addStatusEffectRoutine = base.GameController.AddStatusEffect(
                        MakeOnPhaseChangeStatusEffect(nameof(StartOfTurnPlayCardsResponse),
                            $"{_incapSelectedHero?.Title} may play {Incapacitate2CardsToPlay} cards at the start of {this.Card.Title}'s next turn"),
                        true, this.GetCardSource());

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(addStatusEffectRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(addStatusEffectRoutine);
                    }

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

        public IEnumerator StartOfTurnDrawCardsResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            if (_incapSelectedHero == null)
            {
                yield break;
            }
            
            IEnumerator drawCardRoutine = base.DrawCards(FindHeroTurnTakerController(_incapSelectedHero.Owner.ToHero()), 
                Incapacitate1CardsToDraw, true, allowAutoDraw: false);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
            }

            _incapSelectedHero = null;
        }

        public IEnumerator StartOfTurnPlayCardsResponse(PhaseChangeAction action, OnPhaseChangeStatusEffect sourceEffect)
        {
            if (_incapSelectedHero == null)
            {
                yield break;
            }

            HeroTurnTakerController httc = FindHeroTurnTakerController(_incapSelectedHero.Owner.ToHero());

            IEnumerator playCardsRoutine = base.GameController.PlayCards(httc,
                card => card.Owner == _incapSelectedHero.Owner && card.IsInHand, true, true, 
                Incapacitate2CardsToPlay, cardSource: httc.CharacterCardController.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardsRoutine);
            }

            _incapSelectedHero = null;
        }

        private OnPhaseChangeStatusEffect MakeOnPhaseChangeStatusEffect(string methodToCall, string description)
        {
            OnPhaseChangeStatusEffect onPhaseChangeStatusEffect = new OnPhaseChangeStatusEffect(this.CardWithoutReplacements,
                methodToCall,
                description, new[]
                {
                    TriggerType.PhaseChange
                }, this.Card);

            onPhaseChangeStatusEffect.UntilEndOfNextTurn(base.HeroTurnTaker);
            onPhaseChangeStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.HeroTurnTaker;
            onPhaseChangeStatusEffect.UntilCardLeavesPlay(base.Card);
            onPhaseChangeStatusEffect.DoesDealDamage = false;
            onPhaseChangeStatusEffect.TurnPhaseCriteria.Phase = Phase.Start;
            onPhaseChangeStatusEffect.BeforeOrAfter = BeforeOrAfter.After;

            return onPhaseChangeStatusEffect;
        }
    }
}

﻿using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class GhostOpsTangoOneCharacterCardController : HeroCharacterCardController
    {

        private const int CardsToDraw = 2;
        private const int CardsToTopDeck = 2;
        private const int Incapacitate3DamageIncrease = 2;

        public GhostOpsTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Draw 2 cards. Then put 2 cards from your hand on top of your deck.
            //==============================================================

            int cardDraws = GetPowerNumeral(0, CardsToDraw);
            int topDecks = GetPowerNumeral(1, CardsToTopDeck);

            // Draw 2 cards
            IEnumerator drawCardsRoutine = base.DrawCards(DecisionMaker, cardDraws);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardsRoutine);
            }

            // Top deck 2 cards from your hand
            List<MoveCardDestination> list = new List<MoveCardDestination>
            {
                new MoveCardDestination(this.TurnTaker.Deck)
            };

            IEnumerator selectCardsFromLocationRoutine = base.GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker,
                this.HeroTurnTaker.Hand,
                topDecks, topDecks,
                new LinqCardCriteria(c => c.Location == this.HeroTurnTaker.Hand, "hand"),
                list, shuffleAfterwards: false, cardSource: base.GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(selectCardsFromLocationRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(selectCardsFromLocationRoutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //==============================================================
                        // One player may play a card now.
                        //==============================================================

                        IEnumerator coroutine = this.GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());

                        if (this.UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    break;

                case 1:
                    {
                        //==============================================================
                        // All damage is irreducible until the start of your next turn.
                        //==============================================================

                        MakeDamageIrreducibleStatusEffect mdise = new MakeDamageIrreducibleStatusEffect();
                        mdise.CardSource = Card;
                        mdise.UntilStartOfNextTurn(this.TurnTaker);

                        IEnumerator damageIrreducibleRoutine = AddStatusEffect(mdise);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(damageIrreducibleRoutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(damageIrreducibleRoutine);
                        }
                    }
                    break;

                case 2:
                    {
                        //==============================================================
                        // The next time a hero uses a power which deals damage, increase that damage by 2.
                        //==============================================================
                        IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(Incapacitate3DamageIncrease);
                        effect.SourceCriteria.IsHero = true;
                        effect.NumberOfUses = 1;
                        effect.CardSource = Card;

                        IEnumerator increaseDamageRoutine = base.AddStatusEffect(effect);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(increaseDamageRoutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(increaseDamageRoutine);
                        }
                    }
                    break;
            }
        }
    }
}

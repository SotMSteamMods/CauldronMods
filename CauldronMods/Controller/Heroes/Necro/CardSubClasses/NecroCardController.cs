﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;
using Handelabra;

namespace Cauldron.Necro
{
    public abstract class NecroCardController : CardController
    {
        public static readonly string RitualKeyword = "ritual";
        public static readonly string UndeadKeyword = "undead";
        public static readonly string PastNecroPowerKey = "HeroVillainFlipped";

        protected NecroCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected ITrigger AddUndeadDestroyedTrigger(Func<DestroyCardAction, IEnumerator> response, TriggerType triggerType)
        {
            return base.AddTrigger<DestroyCardAction>(d => this.IsUndead(d.CardToDestroy.Card) && d.WasCardDestroyed, response, triggerType, TriggerTiming.After);
        }

        protected bool IsHeroConsidering1929(Card card)
        {
            if (GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) != null && GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) == true)
            {
                return IsVillain(card);
            }
            return card.IsHero;
        }

        protected bool IsVillianConsidering1929(Card card)
        {
            if (GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) != null && GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) == true)
            {
                return card.IsHero;
            }
            return base.IsVillain(card);
        }

        protected string HeroStringConsidering1929
        {
            get
            {
                if (GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) != null && GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) == true)
                {
                    return "villain";
                }
                return "hero";
            }
        }

        protected string VillianStringConsidering1929
        {
            get
            {
                if (GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) != null && GameController.GetCardPropertyJournalEntryBoolean(base.CharacterCard, PastNecroPowerKey) == true)
                {
                    return "hero";
                }
                return "villian";
            }
        }

        protected bool IsRitual(Card card)
        {
            return card != null && card.DoKeywordsContain(RitualKeyword);
        }

        protected int GetNumberOfRitualsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsRitual(c)).Count();
        }

        protected bool IsUndead(Card card)
        {
            return card != null && card.DoKeywordsContain(UndeadKeyword);
        }
    }
}

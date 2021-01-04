using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatFutureTests : BaseTest
    {
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Radiant);
        }

        protected void AddCannotDealNextDamageTrigger(TurnTakerController ttc, Card card)
        {
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.NumberOfUses = 1;
            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = card;
            this.RunCoroutine(this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        private void AddShuffleTrashCounterAttackTrigger(TurnTakerController ttc, TurnTaker turnTakerToReshuffleTrash)
        {
            Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.Target == ttc.CharacterCard;
            Func<DealDamageAction, IEnumerator> response = (DealDamageAction dd) => this.GameController.ShuffleTrashIntoDeck(this.GameController.FindTurnTakerController(turnTakerToReshuffleTrash));
            this.GameController.AddTrigger<DealDamageAction>(new Trigger<DealDamageAction>(this.GameController, criteria, response, new TriggerType[] { TriggerType.ShuffleTrashIntoDeck }, TriggerTiming.After, this.GameController.FindCardController(turnTakerToReshuffleTrash.CharacterCard).GetCardSource()));
        }

        private bool IsSpell(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "spell");
        }

        [Test()]
        public void TestHydraTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat/FutureTiamatCharacter", "Legacy", "Bunker", "Haka", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(tiamat);
            Assert.IsInstanceOf(typeof(FutureTiamatCharacterCardController), tiamat.CharacterCardController);
            AssertNumberOfCardsInPlay(tiamat, 3);
            AssertHitPoints(tiamat, 120);
        }
    }
}

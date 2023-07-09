using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Titan
{
    public abstract class TitanBaseCharacterCardController : HeroCharacterCardController
    {
        protected TitanBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private static readonly string TitanformIdentifier = "Titanform";

        public static bool GetChangedCutoutInfo(CutoutInfo currentInfo, TurnTakerController ttc, out CutoutInfo changedInfo)
        {
            changedInfo = currentInfo;

            bool isTitanformInPlay = ttc.GameController.FindCardsWhere(c => c.Identifier == TitanformIdentifier && c.IsInPlayAndHasGameText).Any();
            string suffix = "Form";
            string nosuffix = "";
            if (isTitanformInPlay && currentInfo.HeroTurnSuffix != suffix)
            {
                changedInfo.HeroTurnSuffix = suffix;
                changedInfo.VillainTurnSuffix = suffix;
            }
            if (!isTitanformInPlay && currentInfo.HeroTurnSuffix != nosuffix)
            {
                changedInfo.HeroTurnSuffix = nosuffix;
                changedInfo.VillainTurnSuffix = nosuffix;
            }

            return changedInfo.HeroTurnSuffix != currentInfo.HeroTurnSuffix;
        }

        public override bool ShouldChangeCutout(CutoutInfo currentInfo, GameAction action, ActionTiming timing, out CutoutInfo changedInfo, out CutoutAnimation animation)
        {
            bool baseFlag = base.ShouldChangeCutout(currentInfo, action, timing, out changedInfo, out animation);
            bool formFlag = false;
            animation = CutoutAnimation.Fade;

            if (action == null || (action is MoveCardAction mca && timing == ActionTiming.DidPerform && mca.CardToMove.Identifier == TitanformIdentifier ) || (action is PlayCardAction pca && timing == ActionTiming.DidPerform && pca.CardToPlay.Identifier == TitanformIdentifier))
            {
                formFlag = GetChangedCutoutInfo(currentInfo, TurnTakerControllerWithoutReplacements, out changedInfo);
            }
            return baseFlag || formFlag;
        }
    }
}
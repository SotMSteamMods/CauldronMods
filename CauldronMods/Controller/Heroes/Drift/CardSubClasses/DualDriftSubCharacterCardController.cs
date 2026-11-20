using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class DualDriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : DriftSubCharacterCardController(card, turnTakerController)
{
    protected override bool shouldRunSetUp => false;
}

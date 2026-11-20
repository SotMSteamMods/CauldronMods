using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class DualDriftSubCharacterCardController : DriftSubCharacterCardController
{
    public DualDriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) 
    { }

    protected override bool shouldRunSetUp => false;
}

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    public class CauldronBaseTest : BaseTest
    {
        //Heroes
        protected HeroTurnTakerController baccarat { get { return FindHero("Baccarat"); } }
        protected HeroTurnTakerController cricket { get { return FindHero("Cricket"); } }
        protected HeroTurnTakerController cypher { get { return FindHero("Cypher"); } }
        protected HeroTurnTakerController doc { get { return FindHero("DocHavoc"); } }
        protected HeroTurnTakerController drift { get { return FindHero("drift"); } }
        protected HeroTurnTakerController echelon { get { return FindHero("Echelon"); } }
        protected HeroTurnTakerController gargoyle { get { return FindHero("Gargoyle"); } }
        protected HeroTurnTakerController gyrosaur { get { return FindHero("Gyrosaur"); } }
        protected HeroTurnTakerController impact { get { return FindHero("Impact"); } }
        protected HeroTurnTakerController ladyWood { get { return FindHero("LadyOfTheWood"); } }
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        protected HeroTurnTakerController malichae { get { return FindHero("Malichae"); } }
        protected HeroTurnTakerController necro { get { return FindHero("Necro"); } }
        protected HeroTurnTakerController pyre { get { return FindHero("Pyre"); } }
        protected HeroTurnTakerController quicksilver { get { return FindHero("Quicksilver"); } }
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        protected HeroTurnTakerController tango { get { return FindHero("TangoOne"); } }
        protected HeroTurnTakerController terminus { get { return FindHero("Terminus"); } }
        protected HeroTurnTakerController knight { get { return FindHero("TheKnight"); } }
        protected HeroTurnTakerController stranger { get { return FindHero("TheStranger"); } }
        protected HeroTurnTakerController titan { get { return FindHero("Titan"); } }
        protected HeroTurnTakerController vanish { get { return FindHero("Vanish"); } }

        //Villains
        protected TurnTakerController anathema { get { return FindVillain("Anathema"); } }
        protected TurnTakerController celadroch { get { return FindVillain("Celadroch"); } }
        protected TurnTakerController dendron { get { return FindVillain("Dendron"); } }
        protected TurnTakerController dynamo { get { return FindVillain("Dynamo"); } }
        protected TurnTakerController gray { get { return FindVillain("Gray"); } }
        protected TurnTakerController menagerie { get { return FindVillain("Menagerie"); } }
        protected TurnTakerController mythos { get { return FindVillain("Mythos"); } }
        protected TurnTakerController oriphel { get { return FindVillain("Oriphel"); } }
        protected TurnTakerController outlander { get { return FindVillain("Outlander"); } }
        protected TurnTakerController phase { get { return FindVillain("PhaseVillain"); } }
        protected TurnTakerController scream { get { return FindVillain("ScreaMachine"); } }
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }
        protected TurnTakerController choir { get { return FindVillain("TheInfernalChoir"); } }
        protected TurnTakerController fate { get { return FindVillain("TheMistressOfFate"); } }
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }
        protected TurnTakerController vector { get { return FindVillain("Vector"); } }













    }
}

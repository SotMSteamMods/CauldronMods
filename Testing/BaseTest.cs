using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Handelabra.Sentinels.Engine.Controller.PromoCardUnlockControllers;

namespace Handelabra.Sentinels.UnitTest
{
    public class BaseTest
    {
        protected GameController GameController { get; set; }

        // Properties you can set for MakeDecisions to use
        protected Card DecisionSelectTarget { get; set; }
        protected Card DecisionSelectCard { get; set; }
        protected SelectionType? DecisionDoNotSelectCard { get; set; }
        protected bool DecisionDoNotSelectFunction { get; set; }
        protected IEnumerable<Card> DecisionSelectCards { get; set; }
        protected IEnumerable<DamageType?> DecisionSelectDamageTypes { get; set; }
        protected int DecisionSelectDamageTypesIndex { get; set; }
        protected int DecisionSelectCardsIndex { get; set; }
        protected Card DecisionSelectCardToPlay { get; set; }
        protected Card[] DecisionSelectTargets { get; set; }
        protected int DecisionSelectTargetsIndex { get; set; }
        protected TurnTaker DecisionSelectTurnTaker { get; set; }
        protected IEnumerable<TurnTaker> DecisionSelectTurnTakers { get; set; }
        protected int DecisionSelectTurnTakersIndex { get; set; }
        protected bool DecisionDoNotSelectTurnTaker { get; set; }
        protected Card DecisionRedirectTarget { get; set; }
        protected Card DecisionAmbiguousCard { get; set; }
        protected IEnumerable<Card> DecisionAmbiguousCards { get; set; }
        protected int DecisionAmbiguousCardsIndex { get; set; }
        protected int? DecisionAmbiguousCardAtIndex { get; set; }
        protected int?[] DecisionAmbiguousCardAtIndices { get; set; }
        protected int DecisionAmbiguousCardAtIndicesIndex { get; set; }
        protected DamageType? DecisionSelectDamageType { get; set; }
        protected bool? DecisionYesNo { get; set; }
        protected IEnumerable<bool> DecisionsYesNo { get; set; }
        protected int DecisionsYesNoIndex { get; set; }
        protected MoveCardDestination DecisionMoveCardDestination { get; set; }
        protected int DecisionPowerIndex { get; set; }
        protected Card DecisionDiscardCard { get; set; }
        protected Card DecisionMoveCard { get; set; }
        protected Card DecisionDestroyCard { get; set; }
        protected Card[] DecisionDestroyCards { get; set; }
        protected int DecisionDestroyCardsIndex { get; set; }
        protected int DecisionIncapacitatedAbilityIndex { get; set; }
        protected int? DecisionSelectFunction { get; set; }
        protected int?[] DecisionSelectFunctions { get; set; }
        protected int DecisionSelectFunctionsIndex { get; set; }
        protected int? DecisionSelectNumber { get; set; }
        protected Card DecisionHighestHP { get; set; }
        protected Card DecisionLowestHP { get; set; }
        protected int? ExpectedDecisionChoiceCount { get; set; }
        protected Card DecisionNextToCard { get; set; }
        protected SelectionType? DecisionNextSelectionType { get; set; }
        protected Card[] DecisionRedirectTargets { get; set; }
        protected int DecisionRedirectTargetsIndex { get; set; }
        protected Card DecisionSelectPower { get; set; }
        protected int DecisionSelectPowerIndex { get; set; }
        protected Card[] DecisionSelectPowers { get; set; }
        protected int DecisionSelectPowersIndex { get; set; }
        protected LocationChoice DecisionSelectLocation { get; set; }
        protected bool DecisionDoNotSelectLocation { get; set; } // Used to pass a null value to a SelectLocationDecision
        protected bool ShowDamagePreview { get; set; }
        protected Card[] DecisionActivateAbilities { get; set; }
        protected int DecisionActivateAbilitiesIndex { get; set; }
        protected bool ShowActionDecisionSources { get; set; }
        protected SelectionType? DecisionAutoDecide { get; set; }
        protected bool DecisionDoNotActivatableAbility { get; set; }
        protected Card DecisionSelectTargetFriendly { get; set; }
        protected MoveCardDestination[] DecisionMoveCardDestinations { get; set; }
        protected int DecisionMoveCardDestinationsIndex { get; set; }
        protected Card DecisionReturnToHand { get; set; }
        protected Card DecisionGainHP { get; set; }
        protected string DecisionSelectWord { get; set; }
        protected string[] DecisionSelectWords { get; set; }
        protected int DecisionSelectWordsIndex { get; set; }
        protected bool AllowGameOverDuringGoToPhase { get; set; }
        protected IEnumerable<DecisionAnswerJournalEntry> ReplayDecisionAnswers { get; set; }
        protected bool ReplayingGame { get; set; }
        protected Card DecisionUnincapacitateHero { get; set; }
        protected IEnumerable<LocationChoice> DecisionSelectLocations { get; set; }
        protected int DecisionSelectLocationsIndex { get; set; }
        protected bool DecisionAutoDecideIfAble { get; set; }
        protected TurnPhase DecisionSelectTurnPhase { get; set; }
        protected string[] DecisionSelectFromBoxIdentifiers { get; set; }
        protected string DecisionSelectFromBoxTurnTakerIdentifier { get; set; }
        protected int DecisionSelectFromBoxIndex { get; set; }
        protected bool DecisionSelectWordSkip { get; set; }

        private IEnumerable<Card> _includedCardsInNextDecision;
        private IEnumerable<Card> _notIncludedCardsInNextDecision;
        private bool _expectedMessageWasShown;

        private IEnumerable<TurnTaker> _includedTurnTakersInNextDecision;
        private IEnumerable<TurnTaker> _notIncludedTurnTakersInNextDecision;

        private IEnumerable<LocationChoice> _includedLocationsInNextDecision;
        private IEnumerable<LocationChoice> _notIncludedLocationsInNextDecision;

        private IEnumerable<Card> _includedPowersInNextDecision;
        private IEnumerable<Card> _notIncludedPowersInNextDecision;

        private int? _numberOfChoicesInNextDecision;
        private SelectionType? _numberOfChoicesInNextDecisionSelectionType;

        private Dictionary<Card, int> _quickHPStorage;
        private Dictionary<HeroTurnTakerController, int> _quickHandStorage;
        private Dictionary<TurnTakerController, Card> _quickTopCardStorage;
        private Dictionary<TokenPool, int> _quickTokenPoolStorage;
        private Dictionary<Location, int> _quickShuffleStorage;

        private Dictionary<string, object> _savedViewData;

        private bool _continueRunningGame = true;

        private Func<IDecision, bool> _decisionSourceCriteria;
        private string _expectedDecisionSourceOutput;

        private Card _notDamageSource;
        private SelectionType? _assertDecisionOptional;

        protected int NumberOfDecisionsAnswered { get; private set; }

        // Turn taker controller helpers
        protected HeroTurnTakerController adept { get { return FindHero("TheArgentAdept"); } }
        protected HeroTurnTakerController az { get { return FindHero("AbsoluteZero"); } }
        protected HeroTurnTakerController bench { get { return FindHero("Benchmark"); } }
        protected HeroTurnTakerController bunker { get { return FindHero("Bunker"); } }
        protected HeroTurnTakerController chrono { get { return FindHero("ChronoRanger"); } }
        protected HeroTurnTakerController comodora { get { return FindHero("LaComodora"); } }
        protected HeroTurnTakerController cosmic { get { return FindHero("CaptainCosmic"); } }
        protected HeroTurnTakerController expatriette { get { return FindHero("Expatriette"); } }
        protected HeroTurnTakerController fanatic { get { return FindHero("Fanatic"); } }
        protected HeroTurnTakerController fixer { get { return FindHero("MrFixer"); } }
        protected HeroTurnTakerController guise { get { return FindHero("Guise"); } }
        protected HeroTurnTakerController haka { get { return FindHero("Haka"); } }
        protected HeroTurnTakerController harpy { get { return FindHero("TheHarpy"); } }
        protected HeroTurnTakerController knyfe { get { return FindHero("Knyfe"); } }
        protected HeroTurnTakerController legacy { get { return FindHero("Legacy"); } }
        protected HeroTurnTakerController luminary { get { return FindHero("Luminary"); } }
        protected HeroTurnTakerController lifeline { get { return FindHero("Lifeline"); } }
        protected HeroTurnTakerController mist { get { return FindHero("NightMist"); } }
        protected HeroTurnTakerController naturalist { get { return FindHero("TheNaturalist"); } }
        protected HeroTurnTakerController omnix { get { return FindHero("OmnitronX"); } }
        protected HeroTurnTakerController parse { get { return FindHero("Parse"); } }
        protected HeroTurnTakerController ra { get { return FindHero("Ra"); } }
        protected HeroTurnTakerController scholar { get { return FindHero("TheScholar"); } }
        protected HeroTurnTakerController sentinels { get { return FindHero("TheSentinels"); } }
        protected HeroTurnTakerController setback { get { return FindHero("Setback"); } }
        protected HeroTurnTakerController sky { get { return FindHero("SkyScraper"); } }
        protected HeroTurnTakerController stunt { get { return FindHero("Stuntman"); } }
        protected HeroTurnTakerController tachyon { get { return FindHero("Tachyon"); } }
        protected HeroTurnTakerController thriya { get { return FindHero("AkashThriya"); } }
        protected HeroTurnTakerController tempest { get { return FindHero("Tempest"); } }
        protected HeroTurnTakerController unity { get { return FindHero("Unity"); } }
        protected HeroTurnTakerController visionary { get { return FindHero("TheVisionary"); } }
        protected HeroTurnTakerController voidMedico { get { return FindHero("VoidGuardDrMedico"); } }
        protected HeroTurnTakerController voidMainstay { get { return FindHero("VoidGuardMainstay"); } }
        protected HeroTurnTakerController voidIdealist { get { return FindHero("VoidGuardTheIdealist"); } }
        protected HeroTurnTakerController voidWrithe { get { return FindHero("VoidGuardWrithe"); } }
        protected HeroTurnTakerController wraith { get { return FindHero("TheWraith"); } }

        // The Sentinels
        protected Card medico { get { return GetCard("DrMedicoCharacter"); } }
        protected Card mainstay { get { return GetCard("MainstayCharacter"); } }
        protected Card idealist { get { return GetCard("TheIdealistCharacter"); } }
        protected Card writhe { get { return GetCard("WritheCharacter"); } }

        // Villains
        protected TurnTakerController akash { get { return FindVillain("AkashBhuta"); } }
        protected TurnTakerController ambuscade { get { return FindVillain("Ambuscade"); } }
        protected TurnTakerController apostate { get { return FindVillain("Apostate"); } }
        protected TurnTakerController baron { get { return FindVillain("BaronBlade"); } }
        protected TurnTakerController capitan { get { return FindVillain("LaCapitan"); } }
        protected TurnTakerController chairman { get { return FindVillain("TheChairman"); } }
        protected TurnTakerController choke { get { return FindVillain("Chokepoint"); } }
        protected TurnTakerController dawn { get { return FindVillain("CitizenDawn"); } }
        protected TurnTakerController deadline { get { return FindVillain("Deadline"); } }
        protected TurnTakerController dreamer { get { return FindVillain("TheDreamer"); } }
        protected TurnTakerController ennead { get { return FindVillain("TheEnnead"); } }
        protected TurnTakerController gloom { get { return FindVillain("GloomWeaver"); } }
        protected TurnTakerController infinitor { get { return FindVillain("Infinitor"); } }
        protected TurnTakerController iron { get { return FindVillain("IronLegacy"); } }
        protected TurnTakerController kismet { get { return FindVillain("Kismet"); } }
        protected TurnTakerController matriarch { get { return FindVillain("TheMatriarch"); } }
        protected TurnTakerController miss { get { return FindVillain("MissInformation"); } }
        protected TurnTakerController omnitron { get { return FindVillain("Omnitron"); } }
        protected TurnTakerController plague { get { return FindVillain("PlagueRat"); } }
        protected TurnTakerController progeny { get { return FindVillain("Progeny"); } }
        protected TurnTakerController spite { get { return FindVillain("Spite"); } }
        protected TurnTakerController voss { get { return FindVillain("GrandWarlordVoss"); } }
        protected TurnTakerController wager { get { return FindVillain("WagerMaster"); } }
        protected TurnTakerController warfang { get { return FindVillain("KaargraWarfang"); } }

        // Team villains
        protected TurnTakerController baronTeam { get { return FindVillainTeamMember("BaronBlade"); } }
        protected TurnTakerController ermineTeam { get { return FindVillainTeamMember("Ermine"); } }
        protected TurnTakerController frictionTeam { get { return FindVillainTeamMember("Friction"); } }
        protected TurnTakerController frightTeam { get { return FindVillainTeamMember("FrightTrain"); } }
        protected TurnTakerController proleTeam { get { return FindVillainTeamMember("Proletariat"); } }
        protected TurnTakerController ambuscadeTeam { get { return FindVillainTeamMember("Ambuscade"); } }
        protected TurnTakerController biomancerTeam { get { return FindVillainTeamMember("Biomancer"); } }
        protected TurnTakerController bugbearTeam { get { return FindVillainTeamMember("Bugbear"); } }
        protected TurnTakerController greazerTeam { get { return FindVillainTeamMember("Greazer"); } }
        protected TurnTakerController lacapitanTeam { get { return FindVillainTeamMember("LaCapitan"); } }
        protected TurnTakerController missinfoTeam { get { return FindVillainTeamMember("MissInformation"); } }
        protected TurnTakerController plagueratTeam { get { return FindVillainTeamMember("PlagueRat"); } }
        protected TurnTakerController sgtsteelTeam { get { return FindVillainTeamMember("SergeantSteel"); } }
        protected TurnTakerController operativeTeam { get { return FindVillainTeamMember("TheOperative"); } }
        protected TurnTakerController hammeranvilTeam { get { return FindVillainTeamMember("CitizensHammerAndAnvil"); } }

        // Citizens Hammer and Anvil
        protected Card hammer { get { return GetCard("CitizenHammerTeamCharacter"); } }
        protected Card anvil { get { return GetCard("CitizenAnvilTeamCharacter"); } }

        protected TurnTakerController env { get { return FindEnvironment(); } }

        // OblivAeon mode
        protected TurnTakerController oblivaeon { get { return FindVillain("OblivAeon"); } }
        protected BattleZone bzOne { get { return this.GameController.GetBattleZone("BattleZoneOne"); } }
        protected BattleZone bzTwo { get { return this.GameController.GetBattleZone("BattleZoneTwo"); } }
        protected TurnTakerController envOne { get { return this.GameController.FindEnvironmentTurnTakerController(bzOne); } }
        protected TurnTakerController envTwo { get { return this.GameController.FindEnvironmentTurnTakerController(bzTwo); } }
        protected TurnTakerController scionOne { get { return this.GameController.FindTurnTakerController(bzOne.FindScion()); } }
        protected TurnTakerController scionTwo { get { return this.GameController.FindTurnTakerController(bzTwo.FindScion()); } }

        protected Card aeonScion { get { return GetCard("AeonMasterCharacter"); } }
        protected Card borrScion { get { return GetCard("BorrTheUnstableCharacter"); } }
        protected Card mindScion { get { return GetCard("DarkMindCharacter"); } }
        protected Card empScion { get { return GetCard("EmpyreonCharacter"); } }
        protected Card faultScion { get { return GetCard("FaultlessCharacter"); } }
        protected Card nixScion { get { return GetCard("NixiousTheChosenCharacter"); } }
        protected Card progScion { get { return GetCard("ProgenyScionCharacter"); } }
        protected Card vossScion { get { return GetCard("RainekKelVossCharacter"); } }
        protected Card sanctScion { get { return GetCard("SanctionCharacter"); } }
        protected Card voidScion { get { return GetCard("VoidsoulCharacter"); } }

        protected List<TurnPhase> turnPhaseList = null;

        private int _coroutineCount;
        private static int _totalCoroutineCount;

        [SetUp]
        public void Init()
        {
            // Reset log settings for each test
            Log.ResetSettings();

            ResetDecisions();

            _savedViewData = new Dictionary<string, object>();

            _quickTopCardStorage = new Dictionary<TurnTakerController, Card>();
            _quickHandStorage = new Dictionary<HeroTurnTakerController, int>();
            _quickHPStorage = new Dictionary<Card, int>();
            _quickShuffleStorage = new Dictionary<Location, int>();
            _quickTokenPoolStorage = new Dictionary<TokenPool, int>();

            ReplayingGame = false;
            ReplayDecisionAnswers = null;
            turnPhaseList = null;

            _coroutineCount = 0;
        }

        [TearDown]
        public void Finish()
        {
            this.GameController = null;

            if (IsOnTeamCity())
            {
                // Use number type so we can make a chart
                Console.WriteLine("##teamcity[testMetadata name='coroutine count' value='{0}' type='number']", _coroutineCount);
            }

            _totalCoroutineCount += _coroutineCount;
            _coroutineCount = 0;
        }

        [OneTimeTearDown]
        public void FinishAll()
        {
            if (IsOnTeamCity())
            {
                Console.WriteLine("##teamcity[buildStatisticValue key='total coroutine count' value='{0}']", _totalCoroutineCount);
            }
        }

        protected void ResetDecisions()
        {
            // Reset all decisions so there are no "left overs" from previous tests, as properties carry over.
            DecisionSelectTarget = null;
            DecisionSelectCard = null;
            DecisionDoNotSelectCard = null;
            DecisionDoNotSelectFunction = false;
            DecisionSelectCards = null;
            DecisionSelectCardsIndex = 0;
            DecisionSelectCardToPlay = null;
            DecisionSelectTargets = null;
            DecisionSelectTargetsIndex = 0;
            DecisionSelectTurnTaker = null;
            DecisionSelectTurnTakers = null;
            DecisionSelectTurnTakersIndex = 0;
            DecisionDoNotSelectTurnTaker = false;
            DecisionRedirectTarget = null;
            DecisionAmbiguousCard = null;
            DecisionSelectDamageType = null;
            DecisionSelectDamageTypes = null;
            DecisionSelectDamageTypesIndex = 0;
            DecisionYesNo = null;
            DecisionsYesNo = null;
            DecisionsYesNoIndex = 0;
            DecisionMoveCardDestination = new MoveCardDestination(null);
            DecisionPowerIndex = 0;
            DecisionDiscardCard = null;
            DecisionMoveCard = null;
            DecisionDestroyCard = null;
            DecisionDestroyCards = null;
            DecisionDestroyCardsIndex = 0;
            DecisionIncapacitatedAbilityIndex = 0;
            DecisionSelectFunction = null;
            DecisionSelectFunctions = null;
            DecisionSelectFunctionsIndex = 0;
            ExpectedDecisionChoiceCount = null;
            DecisionHighestHP = null;
            DecisionLowestHP = null;
            ShowDamagePreview = true;
            DecisionNextToCard = null;
            DecisionSelectPower = null;
            DecisionSelectPowerIndex = 0;
            DecisionSelectLocation = new LocationChoice(null);
            DecisionSelectPowers = null;
            DecisionSelectPowersIndex = 0;
            DecisionActivateAbilities = null;
            DecisionActivateAbilitiesIndex = 0;
            ShowActionDecisionSources = false;
            DecisionAutoDecide = null;
            DecisionDoNotActivatableAbility = false;
            DecisionAmbiguousCards = null;
            DecisionAmbiguousCardsIndex = 0;
            DecisionAmbiguousCardAtIndex = null;
            DecisionAmbiguousCardAtIndices = null;
            DecisionAmbiguousCardAtIndicesIndex = 0;
            _includedCardsInNextDecision = null;
            _notIncludedCardsInNextDecision = null;
            _includedPowersInNextDecision = null;
            _notIncludedPowersInNextDecision = null;
            _notIncludedTurnTakersInNextDecision = null;
            _includedTurnTakersInNextDecision = null;
            _includedLocationsInNextDecision = null;
            _notIncludedLocationsInNextDecision = null;
            _numberOfChoicesInNextDecision = null;
            _numberOfChoicesInNextDecisionSelectionType = null;
            _notDamageSource = null;
            DecisionSelectTargetFriendly = null;
            DecisionMoveCardDestinations = null;
            DecisionMoveCardDestinationsIndex = 0;
            DecisionReturnToHand = null;
            DecisionGainHP = null;
            _decisionSourceCriteria = null;
            _expectedDecisionSourceOutput = null;
            DecisionSelectWord = null;
            DecisionSelectWords = null;
            DecisionSelectWordsIndex = 0;
            AllowGameOverDuringGoToPhase = false;
            _assertDecisionOptional = null;
            DecisionUnincapacitateHero = null;
            DecisionSelectLocations = null;
            DecisionSelectLocationsIndex = 0;
            DecisionAutoDecideIfAble = false;
            NumberOfDecisionsAnswered = 0;
            DecisionSelectTurnPhase = null;
            DecisionSelectFromBoxIdentifiers = null;
            DecisionSelectFromBoxTurnTakerIdentifier = null;
            DecisionSelectFromBoxIndex = 0;
            DecisionNextSelectionType = null;
            DecisionDoNotSelectLocation = false;
        }

        private static bool IsOnTeamCity()
        {
            return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));
        }

        protected GameController SetupGameController(IEnumerable<string> identifiers,
                                                     bool advanced = false,
                                                     IDictionary<string, string> promoIdentifiers = null,
                                                     int? randomSeed = null,
                                                     IEnumerable<string> advancedIdentifiers = null,
                                                     bool challenge = false,
                                                     IEnumerable<string> challengeIdentifiers = null,
                                                     string shieldIdentifier = null,
                                                     IEnumerable<string> scionIdentifiers = null)
        {
            var promoShortcuts = identifiers.Where(s => s.Contains("/"));

            if (promoShortcuts.Count() > 0)
            {
                if (promoIdentifiers == null)
                {
                    promoIdentifiers = new Dictionary<string, string>();
                }

                foreach (var shortcut in promoShortcuts)
                {
                    var shortcutSplit = shortcut.Split('/');
                    identifiers = identifiers.Replace(s => s == shortcut, s => shortcutSplit.ElementAt(0));
                    var character = shortcutSplit.ElementAt(1);
                    if (!character.Contains("Character"))
                    {
                        character += "Character";
                    }
                    promoIdentifiers.Add(shortcutSplit.ElementAt(0), character);
                }
            }

            Game game = new Game(identifiers, advanced, promoIdentifiers, randomSeed,
                                 advancedIdentifiers: advancedIdentifiers,
                                 isChallenge: challenge,
                                 challengeIdentifiers: challengeIdentifiers,
                                 shieldIdentifier: shieldIdentifier,
                                 scionIdentifiers: scionIdentifiers);
            Console.WriteLine("Created game with random seed: {0}", game.RandomSeed);

            if (IsOnTeamCity())
            {
                Console.WriteLine("##teamcity[testMetadata name='seed' value='{0}']", game.RandomSeed);
            }

            this.GameController = SetupGameController(game);
            return this.GameController;
        }

        protected GameController SetupGameController(params string[] identifiers)
        {
            return SetupGameController(identifiers, false, null, null);
        }

        protected void PrintTurnTakers()
        {
            Console.WriteLine("---- Turn Takers in Game ----");
            for (var i = 0; i < this.GameController.TurnTakerControllers.Count(); i++)
            {
                Console.WriteLine((i + 1) + " " + this.GameController.TurnTakerControllers.ElementAt(i).Name);
            }
            Console.WriteLine("------------------------");
        }

        protected GameController SetupGameController(Game game)
        {
            GameController gameController = new GameController(game);
            gameController.StartCoroutine = StartCoroutine;
            gameController.ExhaustCoroutine = RunCoroutine;
            gameController.OnMakeDecisions -= this.MakeDecisions;
            gameController.OnMakeDecisions += this.MakeDecisions;
            gameController.OnSendMessage += this.ReceiveMessage;
            gameController.OnWillPerformAction += this.WillPerformAction;
            gameController.OnWillApplyActionChanges += this.WillApplyActionChanges;
            gameController.OnDidPerformAction += this.DidPerformAction;
            gameController.OnSetPersistentValueInView += SetPersistentValueInView;
            gameController.OnGetPersistentValueFromView += GetPersistentValueFromView;
            gameController.OnGetHeroCardsInBox += HandleGetHeroCardsInBoxRequest;

            this.GameController = gameController;
            this._continueRunningGame = true;

            return gameController;
        }

        IEnumerable<KeyValuePair<string, string>> HandleGetHeroCardsInBoxRequest(Func<string, bool> identifierCriteria, Func<string, bool> turnTakerCriteria)
        {
            var result = new List<KeyValuePair<string, string>>();

            // Find all the playable hero character cards in the box (including other sizes of Sky-Scraper)
            var availableHeroes = DeckDefinition.AvailableHeroes;
            foreach (var heroTurnTaker in availableHeroes.Where(turnTakerCriteria))
            {
                var heroDefinition = DeckDefinitionCache.GetDeckDefinition(heroTurnTaker);

                foreach (var cardDef in heroDefinition.CardDefinitions.Concat(heroDefinition.PromoCardDefinitions))
                {
                    // Ignore non-real cards (Sentinels Intructions) and cards that do not start in play (Sky-Scraper sizes)
                    if (cardDef.IsCharacter
                        && cardDef.IsRealCard
                        && identifierCriteria(cardDef.PromoIdentifierOrIdentifier))
                    {
                        // It's in the box!
                        var kvp = new KeyValuePair<string, string>(heroTurnTaker, cardDef.PromoIdentifierOrIdentifier);
                        //Debug.LogFormat("In the box {0}: {1}", result.Count, kvp.Value);
                        result.Add(kvp);
                    }
                }
            }

            return result;
        }

        protected object GetPersistentValueFromView(string key, Type type)
        {
            object result = null;

            if (_savedViewData.ContainsKey(key))
            {
                result = _savedViewData[key];
            }

            return result;
        }

        protected T GetPersistentValueFromView<T>(string key)
        {
            T result = default(T);

            if (_savedViewData.ContainsKey(key) && _savedViewData[key] is T)
            {
                result = (T)_savedViewData[key];
            }

            return result;
        }


        protected void SetPersistentValueInView(string key, object value)
        {
            _savedViewData[key] = value;
        }

        protected void RunCoroutine(IEnumerator e)
        {
            _coroutineCount += 1;

            while (_continueRunningGame && e.MoveNext())
            {

            }
        }

        protected IEnumerator StartCoroutine(IEnumerator e)
        {
            while (_continueRunningGame && e.MoveNext())
            {

            }

            yield return null;
        }

        // You can override this, or just set the properties that it will use to make decisions.
        protected virtual IEnumerator MakeDecisions(IDecision decision)
        {
            // Make sure we are not allowing fast coroutines!
            if (this.GameController.PeekFastCoroutines())
            {
                Assert.Fail("MakeDecisions was forcing fast coroutines!");
            }

            Console.WriteLine("MakeDecisions: " + decision.ToStringForMultiplayerDebugging());

            // Any decision with 0 choices is an error and should never happen.
            var numChoices = decision.NumberOfChoices;
            if (numChoices.HasValue)
            {
                Assert.Greater(numChoices, 0, "Decision has 0 choices: {0}", decision);
            }

            // In most cases, a non-optional decision with 1 choice is an error.
            // Exception: ActivateAbilityDecision (The Argent Adept) and UsePowerDecision (Guise, for now)
            if (!(decision is ActivateAbilityDecision || decision is UsePowerDecision))
            {
                Assert.IsFalse(numChoices == 1 && !decision.IsOptional, "Non-optional decision has 1 choice: {0}", decision);
            }

            // If decision answers is not null, check to see if it has an entry for the decision.
            if (this.ReplayDecisionAnswers != null)
            {
                var answer = this.ReplayDecisionAnswers.Where(d => d.DecisionIdentifier.Equals(decision.DecisionIdentifier)).FirstOrDefault();
                if (answer != null)
                {
                    if (answer.AnswerIndex.HasValue)
                    {
                        decision.ChooseIndex(answer.AnswerIndex.Value);
                    }
                    else if (answer.Skipped)
                    {
                        decision.Skip();
                    }
                    else if (answer.AutoDecided)
                    {
                        decision.AutoDecide();
                    }

                    // If this was the very last decision choice, we are no longer able to replay the game.
                    if (answer == this.ReplayDecisionAnswers.LastOrDefault())
                    {
                        this.ReplayingGame = false;
                    }
                }
                else
                {
                    Console.WriteLine("WARNING: ReplayDecisionAnswers did not have an entry for this decision: " + decision);

                    // Stop the replay.
                    this.ReplayingGame = false;
                }
            }

            if (!decision.Completed)
            {
                if (this.DecisionAutoDecide.HasValue && decision.SelectionType == this.DecisionAutoDecide)
                {
                    if (decision.AllowAutoDecide)
                    {
                        Console.WriteLine("Auto-deciding for decision: " + decision.SelectionType);
                        decision.AutoDecide();
                    }
                    else
                    {
                        Assert.Fail("Test wanted to auto-decide for this decision, but the decision does not allow it: " + decision);
                    }
                }
                else if (this.DecisionAutoDecideIfAble)
                {
                    if (decision.AllowAutoDecide)
                    {
                        Console.WriteLine("Auto-deciding for decision: " + decision.SelectionType);
                        decision.AutoDecide();
                    }
                }

                if (_assertDecisionOptional != null && decision.SelectionType == _assertDecisionOptional)
                {
                    Assert.IsTrue(decision.IsOptional, "Decision was not optional: " + decision);
                }

                if (decision is SelectCardDecision)
                {
                    SelectCardDecision selectCardDecision = (SelectCardDecision)decision;

                    Assert.IsNotNull(selectCardDecision.Choices, "Choices must not be null");

                    if (selectCardDecision.ExtraInfo != null)
                    {
                        Console.WriteLine(selectCardDecision.ExtraInfo());
                    }

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        var check = true;
                        if (_numberOfChoicesInNextDecisionSelectionType != null && _numberOfChoicesInNextDecisionSelectionType != decision.SelectionType)
                        {
                            check = false;
                        }

                        if (check)
                        {
                            Assert.AreEqual(_numberOfChoicesInNextDecision, selectCardDecision.Choices.Count(), "SelectCardDecision has the wrong number of choices.");
                            _numberOfChoicesInNextDecision = null;
                        }
                    }

                    var originalChoices = new List<Card>();
                    originalChoices.AddRange(selectCardDecision.Choices);

                    if (_includedCardsInNextDecision != null)
                    {
                        _includedCardsInNextDecision.ForEach(e => Assert.IsTrue(selectCardDecision.Choices.Contains(e), "SelectCardDecision did not include: " + e.Title + "."));
                        _includedCardsInNextDecision = null;
                    }

                    if (_notIncludedCardsInNextDecision != null)
                    {
                        _notIncludedCardsInNextDecision.ForEach(e => Assert.IsFalse(selectCardDecision.Choices.Contains(e), "SelectCardDecision should not include: " + e.Title + ". (Choices: " + selectCardDecision.Choices.Select(c => c.Title).ToCommaList() + ")"));
                        _notIncludedCardsInNextDecision = null;
                    }

                    if (selectCardDecision.Choices.Count() == 1 && !selectCardDecision.IsOptional)
                    {
                        Assert.Fail("This test presented a decision with only 1 choice, and it was not optional.");
                    }

                    if (this.ExpectedDecisionChoiceCount != null)
                    {
                        Assert.AreEqual(this.ExpectedDecisionChoiceCount.Value, selectCardDecision.Choices.Count(), "Expected the decision to have " + this.ExpectedDecisionChoiceCount + " choices, but it had " + selectCardDecision.Choices.Count() + ".");
                    }

                    if (selectCardDecision.Choices.Count() > 0)
                    {
                        if (selectCardDecision.AutoDecided)
                        {
                            selectCardDecision.SelectedCard = selectCardDecision.Choices.FirstOrDefault();
                        }
                        else
                        {
                            string secondary = "";
                            if (selectCardDecision.SecondarySelectionType != null)
                            {
                                secondary = " and secondary type " + selectCardDecision.SecondarySelectionType;
                            }

                            string cardSource = "";
                            if (selectCardDecision.CardSource != null)
                            {
                                cardSource = "[" + selectCardDecision.CardSource.Card.Title + "] ";
                            }

                            string offset = "";
                            if (selectCardDecision.SelectionTypeOrdinal.HasValue)
                            {
                                offset = " (" + (selectCardDecision.SelectionTypeOrdinal.Value).ToOrdinalString() + ") ";
                            }
                            var auto = selectCardDecision.AllowAutoDecide ? ", Auto" : "";
                            var skip = selectCardDecision.IsOptional ? ", Skip" : "";
                            var who = selectCardDecision.HeroTurnTakerController != null ? selectCardDecision.HeroTurnTakerController.Name : "Everybody";

                            string choices = "";
                            IEnumerable<Card> cardChoices = selectCardDecision.Choices;
                            if (selectCardDecision.Choices.All(c => c.PlayIndex.HasValue))
                            {
                                cardChoices = cardChoices.OrderBy(c => c.PlayIndex.Value);
                            }
                            for (int i = 0; i < cardChoices.Count(); i++)
                            {
                                choices += cardChoices.ElementAt(i).Title;
                                var associated = selectCardDecision.GetAssociatedCard(i);
                                if (associated != null)
                                {
                                    choices += " (" + associated.Title + ")";
                                }

                                if (i < cardChoices.Count() - 1)
                                {
                                    choices += ", ";
                                }
                            }

                            string dealDamageInfo = "";
                            if (selectCardDecision.SelectionType == SelectionType.AmbiguousDecision && selectCardDecision.SecondarySelectionType == SelectionType.RedirectDamage)
                            {
                                Assert.NotNull(selectCardDecision.DealDamageInfo);
                                Assert.NotNull(selectCardDecision.DealDamageInfo.FirstOrDefault());

                                var info = selectCardDecision.DealDamageInfo.First();
                                dealDamageInfo = " (" + info.DamageSource.TitleOrName + " dealing " + info.Amount + " " + info.DamageType + " damage to " + info.Target.Title;
                            }

                            Console.WriteLine(cardSource + who + ", Make a SelectCardDecision of type " + selectCardDecision.SelectionType + secondary + offset + ": [" + choices + auto + skip + "]" + dealDamageInfo + ")");

                            if (selectCardDecision.IsOptional && this.DecisionDoNotSelectCard == selectCardDecision.SelectionType)
                            {
                                selectCardDecision.FinishedSelecting = true;
                            }
                            else if (this.DecisionSelectCards != null)
                            {
                                // Select each of the given cards in order
                                Card toSelect = this.DecisionSelectCards.ElementAt(this.DecisionSelectCardsIndex);
                                if (toSelect != null)
                                {
                                    if (selectCardDecision.SelectionType == SelectionType.RedirectDamage && selectCardDecision.GameAction is DealDamageAction)
                                    {
                                        var dealDamage = selectCardDecision.GameAction as DealDamageAction;
                                        if (dealDamage.DamageSource != null && dealDamage.DamageSource.IsCard)
                                        {
                                            // Show a preview of the redirect.
                                            Console.WriteLine("=== Redirect Preview ===");

                                            var damageSource = dealDamage.DamageSource.Card;
                                            GetDamagePreviewResults(damageSource, toSelect,
                                                dealDamage.Amount, dealDamage.DamageType, dealDamage.IsIrreducible);

                                            Console.WriteLine("======");
                                        }
                                    }

                                    selectCardDecision.SelectedCard = toSelect;
                                }
                                else
                                {
                                    selectCardDecision.FinishedSelecting = true;
                                }

                                if (this.DecisionSelectCards.Count() - 1 > this.DecisionSelectCardsIndex)
                                {
                                    this.DecisionSelectCardsIndex++;
                                }
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.SelectTargetFriendly && this.DecisionSelectTargetFriendly != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionSelectTargetFriendly;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.DiscardCard && this.DecisionDiscardCard != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionDiscardCard;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.MoveCard && this.DecisionMoveCard != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionMoveCard;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.DestroyCard && (this.DecisionDestroyCard != null || this.DecisionDestroyCards != null))
                            {
                                if (this.DecisionDestroyCards != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionDestroyCards.ElementAt(this.DecisionDestroyCardsIndex);
                                    this.DecisionDestroyCardsIndex++;
                                }
                                else if (this.DecisionDestroyCard != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionDestroyCard;
                                }
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.MoveCardNextToCard && this.DecisionNextToCard != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionNextToCard;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.UnincapacitateHero && this.DecisionUnincapacitateHero != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionUnincapacitateHero;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.ReturnToHand && this.DecisionReturnToHand != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionReturnToHand;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.RedirectDamage)
                            {
                                if (this.DecisionRedirectTargets != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionRedirectTargets.ElementAt(this.DecisionRedirectTargetsIndex);
                                    this.DecisionRedirectTargetsIndex++;
                                }
                                else if (this.DecisionRedirectTarget != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionRedirectTarget;
                                }
                                else if (selectCardDecision.IsOptional)
                                {
                                    selectCardDecision.SelectedCard = null;
                                }
                                else
                                {
                                    selectCardDecision.SelectedCard = selectCardDecision.Choices.FirstOrDefault();
                                }
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.AmbiguousDecision
                                     && (this.DecisionAmbiguousCardAtIndex != null
                                         || this.DecisionAmbiguousCardAtIndices != null
                                         || this.DecisionAmbiguousCard != null
                                         || this.DecisionAmbiguousCards != null))
                            {
                                if (this.DecisionAmbiguousCardAtIndex != null || this.DecisionAmbiguousCardAtIndices != null)
                                {
                                    int indexValue;
                                    if (this.DecisionAmbiguousCardAtIndices != null)
                                    {
                                        indexValue = this.DecisionAmbiguousCardAtIndices.ElementAt(this.DecisionAmbiguousCardAtIndicesIndex++).Value;
                                    }
                                    else
                                    {
                                        indexValue = this.DecisionAmbiguousCardAtIndex.Value;
                                    }

                                    if ((decision as SelectCardDecision).Choices.Count() >= indexValue + 1)
                                    {
                                        selectCardDecision.ChooseIndex(indexValue);
                                    }
                                }
                                else if ((this.DecisionAmbiguousCard != null || this.DecisionAmbiguousCards != null))
                                {
                                    if (this.DecisionAmbiguousCards != null)
                                    {
                                        selectCardDecision.SelectedCard = this.DecisionAmbiguousCards.ElementAt(this.DecisionAmbiguousCardsIndex++);
                                    }
                                    else
                                    {
                                        selectCardDecision.SelectedCard = this.DecisionAmbiguousCard;
                                    }
                                }
                            }
                            else if ((selectCardDecision.SelectionType == SelectionType.HighestHP && this.DecisionHighestHP != null)
                                 || (selectCardDecision.SelectionType == SelectionType.LowestHP))
                            {
                                if (selectCardDecision.SelectionType == SelectionType.HighestHP && this.DecisionHighestHP != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionHighestHP;
                                }
                                else
                                {
                                    selectCardDecision.SelectedCard = this.DecisionLowestHP;
                                }

                                if (selectCardDecision.GameAction != null && selectCardDecision.GameAction is DealDamageAction && selectCardDecision.SelectedCard != null)
                                {
                                    DealDamageAction dealDamage = (selectCardDecision.GameAction as DealDamageAction);
                                    Console.WriteLine("--- BEGIN PREVIEW ---");
                                    Console.WriteLine("Selected: " + selectCardDecision.SelectedCard.Title);
                                    GetDamagePreviewResults(dealDamage, selectCardDecision.SelectedCard);
                                    Console.WriteLine("--- END PREVIEW ---");
                                }
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.SelectTarget || selectCardDecision.SelectionType == SelectionType.HighestHP || selectCardDecision.SelectionType == SelectionType.LowestHP || selectCardDecision.SelectionType == SelectionType.SelectTargetNoDamage || selectCardDecision.SelectionType == SelectionType.DealDamageSelf)
                            {
                                if (this.DecisionSelectTargets != null)
                                {
                                    // Select each of the given targets in order
                                    selectCardDecision.SelectedCard = this.DecisionSelectTargets.ElementAt(this.DecisionSelectTargetsIndex);
                                    if (this.DecisionSelectTargets.Count() - 1 > this.DecisionSelectTargetsIndex)
                                    {
                                        this.DecisionSelectTargetsIndex++;
                                    }
                                    else
                                    {
                                        this.DecisionSelectTargetsIndex = 0;
                                    }
                                }
                                else if (this.DecisionSelectTarget != null)
                                {
                                    selectCardDecision.SelectedCard = this.DecisionSelectTarget;
                                }
                                else
                                {
                                    selectCardDecision.SelectedCard = selectCardDecision.Choices.FirstOrDefault();
                                }

                                if (selectCardDecision is SelectTargetDecision && this.ShowDamagePreview && selectCardDecision.SelectedCard != null)
                                {
                                    SelectTargetDecision selectTarget = selectCardDecision as SelectTargetDecision;
                                    Console.WriteLine("--- BEGIN PREVIEW ---");
                                    Console.WriteLine("Selected Target: " + selectCardDecision.SelectedCard.Title);
                                    var source = selectTarget.DamageSource;
                                    if (source == null && selectTarget.SelectionType == SelectionType.DealDamageSelf)
                                    {
                                        source = new DamageSource(this.GameController, selectTarget.SelectedCard);
                                    }
                                    //Console.WriteLine("Damage Source: " + source.Title);
                                    GetDamagePreviewResults(source, selectTarget.SelectedCard, selectTarget.DynamicAmount, selectTarget.DamageType.Value, selectTarget.IsIrreducible, selectCardDecision.CardSource);
                                    Console.WriteLine("--- END PREVIEW ---");
                                }
                            }
                            else if ((selectCardDecision.SelectionType == SelectionType.PlayCard || selectCardDecision.SelectionType == SelectionType.PutIntoPlay) && this.DecisionSelectCardToPlay != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionSelectCardToPlay;
                            }
                            else if (selectCardDecision.SelectionType == SelectionType.GainHP && this.DecisionGainHP != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionGainHP;
                            }
                            else if (this.DecisionSelectCard != null)
                            {
                                selectCardDecision.SelectedCard = this.DecisionSelectCard;
                            }
                            else
                            {
                                Console.WriteLine("WARNING: No card was specified for the test, selecting the first available choice.");
                                selectCardDecision.SelectedCard = selectCardDecision.Choices.FirstOrDefault();
                            }

                            if (selectCardDecision.SelectedCard != null)
                            {
                                // Make sure the selected card is one of the possible choices.
                                Console.WriteLine("Selected: " + selectCardDecision.SelectedCard.Title);
                                Assert.IsTrue(originalChoices.Contains(selectCardDecision.SelectedCard), "SelectCardDecision selected \"" + selectCardDecision.SelectedCard.Title + "\", which is not one of the decision options: " + originalChoices.Select(c => c.Title).ToCommaList());
                            }
                            else
                            {
                                Console.WriteLine("WARNING: No card selected.");
                            }
                        }
                    }
                }
                else if (decision is YesNoDecision)
                {
                    YesNoDecision yesNo = decision as YesNoDecision;
                    var source = yesNo.CardSource != null ? yesNo.CardSource.Card.Title : "GameController";
                    var who = yesNo.HeroTurnTakerController != null ? yesNo.HeroTurnTakerController.Name : "Everyone";
                    Console.WriteLine("[" + source + "] " + who + ", Make a YesNoDecision of type " + yesNo.SelectionType);
                    if (this.DecisionsYesNo != null)
                    {
                        Assert.Greater(this.DecisionsYesNo.Count(), this.DecisionsYesNoIndex, "Not enough DecisionsYesNo were provided.");
                        yesNo.Answer = this.DecisionsYesNo.ElementAt(this.DecisionsYesNoIndex);
                        this.DecisionsYesNoIndex += 1;
                    }
                    else
                    {
                        yesNo.Answer = this.DecisionYesNo;
                    }
                    Console.WriteLine("Selected: " + yesNo.Answer);
                }
                else if (decision is YesNoCardDecision)
                {
                    YesNoCardDecision yesNo = decision as YesNoCardDecision;
                    var source = yesNo.CardSource != null ? yesNo.CardSource.Card.Title : "GameController";
                    var who = yesNo.HeroTurnTakerController != null ? yesNo.HeroTurnTakerController.Name : "Everyone";
                    Console.WriteLine("[" + source + "] " + who + ", Make a YesNoCardDecision of type " + yesNo.SelectionType + " with card " + yesNo.Card.Title);
                    if (this.DecisionsYesNo != null)
                    {
                        Assert.Greater(this.DecisionsYesNo.Count(), this.DecisionsYesNoIndex, "Not enough DecisionsYesNo were provided.");
                        yesNo.Answer = this.DecisionsYesNo.ElementAt(this.DecisionsYesNoIndex);
                        this.DecisionsYesNoIndex += 1;
                    }
                    else
                    {
                        yesNo.Answer = this.DecisionYesNo;
                    }
                    Console.WriteLine("Selected: " + yesNo.Answer);
                }
                else if (decision is SelectDamageTypeDecision)
                {
                    SelectDamageTypeDecision damage = decision as SelectDamageTypeDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, damage.Choices.Count(), "SelectDamageTypeDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }

                    var who = damage.HeroTurnTakerController != null ? damage.HeroTurnTakerController.Name + ", " : "";
                    Console.WriteLine(who + "Make a SelectDamageTypeDecision: [" + damage.Choices.ToCommaList(useWordOr: true) + "]");

                    if (this.DecisionSelectDamageType != null)
                    {
                        damage.SelectedDamageType = this.DecisionSelectDamageType;
                        Console.WriteLine("Selected: " + damage.SelectedDamageType);
                    }
                    else if (this.DecisionSelectDamageTypes != null)
                    {
                        // Select each of the given targets in order
                        damage.SelectedDamageType = this.DecisionSelectDamageTypes.ElementAt(this.DecisionSelectDamageTypesIndex);
                        if (this.DecisionSelectDamageTypes.Count() - 1 > this.DecisionSelectDamageTypesIndex)
                        {
                            this.DecisionSelectDamageTypesIndex++;
                        }
                        else
                        {
                            this.DecisionSelectDamageTypesIndex = 0;
                        }
                    }
                    else
                    {
                        damage.SelectedDamageType = damage.Choices.First();
                        Console.WriteLine("No damage type provided. Selected: " + damage.SelectedDamageType);
                    }
                }
                else if (decision is MoveCardDecision)
                {
                    MoveCardDecision moveCard = decision as MoveCardDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, moveCard.PossibleDestinations.Count(), "MoveCardDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }

                    Console.WriteLine("Make a MoveCardDecision with destinations: [" + moveCard.PossibleDestinations.ToCommaList() + "]");
                    if (moveCard.PossibleDestinations.Count() == 1 && !moveCard.IsOptional)
                    {
                        Assert.Fail("This test presented a decision with only 1 choice, and it was not optional.");
                    }
                    if (this.ExpectedDecisionChoiceCount != null)
                    {
                        Assert.AreEqual(this.ExpectedDecisionChoiceCount.Value, moveCard.PossibleDestinations.Count());
                    }

                    MoveCardDestination chosenDestination = new MoveCardDestination();
                    if (moveCard.SelectionType == SelectionType.MoveCardNextToCard && this.DecisionNextToCard != null)
                    {
                        chosenDestination = new MoveCardDestination(this.DecisionNextToCard.NextToLocation);
                    }
                    else if (this.DecisionMoveCardDestinations != null)
                    {
                        chosenDestination = this.DecisionMoveCardDestinations[this.DecisionMoveCardDestinationsIndex++];
                    }
                    else if (this.DecisionMoveCardDestination.Location != null)
                    {
                        chosenDestination = this.DecisionMoveCardDestination;
                    }

                    if (chosenDestination.Location != null)
                    {
                        if (moveCard.PossibleDestinations.Any(d => d.Location == chosenDestination.Location && d.ToBottom == chosenDestination.ToBottom))
                        {
                            moveCard.Destination = chosenDestination;
                            Console.WriteLine("Selected: " + moveCard.Destination);
                        }
                        else
                        {
                            Assert.Fail("The selected destination was not a choice: {0}", chosenDestination);
                        }
                    }
                    else
                    {
                        chosenDestination = moveCard.PossibleDestinations.First();
                        Console.WriteLine("No selection was provided, so choosing destination at index {0}", chosenDestination);
                        moveCard.Destination = chosenDestination;
                    }
                }
                else if (decision is SelectLocationDecision)
                {
                    SelectLocationDecision selectLocation = decision as SelectLocationDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, selectLocation.Choices.Count(), "SelectLocationDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }

                    var choices = selectLocation.Choices.ToCommaList();
                    if (selectLocation.IsOptional)
                    {
                        choices += ", Skip";
                    }
                    Console.WriteLine("Make a SelectLocationDecision with locations: [" + choices + "]");
                    if (selectLocation.Choices.Count() == 1 && !selectLocation.IsOptional)
                    {
                        Assert.Fail("This test presented a decision with only 1 choice, and it was not optional.");
                    }
                    if (this.ExpectedDecisionChoiceCount != null)
                    {
                        Assert.AreEqual(this.ExpectedDecisionChoiceCount.Value, selectLocation.Choices.Count());
                    }

                    if (_includedLocationsInNextDecision != null)
                    {
                        _includedLocationsInNextDecision.ForEach(e => Assert.IsTrue(selectLocation.Choices.Any(lc => lc.Location == e.Location), "SelectLocationsDecision did not include: " + e.Location.GetFriendlyName() + "."));
                        _includedLocationsInNextDecision = null;
                    }

                    if (_notIncludedLocationsInNextDecision != null)
                    {
                        _notIncludedLocationsInNextDecision.ForEach(e => Assert.IsFalse(selectLocation.Choices.Any(l => l.Location == e.Location), "SelectLocationsDecision should not include: " + e.Location.GetFriendlyName() + ". (Choices: " + selectLocation.Choices.Select(l => l.Location.GetFriendlyName()).ToCommaList() + ")"));
                        _notIncludedLocationsInNextDecision = null;
                    }

                    if (selectLocation.SelectionType == SelectionType.MoveCardNextToCard && this.DecisionNextToCard != null)
                    {
                        selectLocation.SelectedLocation = selectLocation.Choices.Where(c => c.Location == this.DecisionNextToCard.NextToLocation).FirstOrDefault();
                    }
                    else if (this.DecisionSelectLocations != null)
                    {
                        var location = this.DecisionSelectLocations.ElementAt(this.DecisionSelectLocationsIndex++);
                        if (location.Location == null && selectLocation.IsOptional)
                        {
                            selectLocation.FinishedSelecting = true;
                        }
                        else if (!selectLocation.Choices.Any(c => c.Location == location.Location))
                        {
                            Assert.Fail("The selected location was not a choice: {0}", location);
                        }
                        else
                        {
                            selectLocation.SelectedLocation = selectLocation.Choices.Where(c => c.Location == location.Location).FirstOrDefault();
                        }
                    }
                    else if (this.DecisionSelectLocation.Location != null)
                    {
                        if (!selectLocation.Choices.Any(c => c.Location == this.DecisionSelectLocation.Location))
                        {
                            Assert.Fail("The selected location was not a choice: {0}", this.DecisionSelectLocation);
                        }
                        else
                        {
                            selectLocation.SelectedLocation = selectLocation.Choices.Where(c => c.Location == this.DecisionSelectLocation.Location).FirstOrDefault();
                            this.DecisionDoNotSelectLocation = false;
                        }
                    }
                    else if (this.DecisionDoNotSelectLocation)
                    {
                        if (!selectLocation.IsOptional)
                        {
                            Log.Warning("Skipping a non-optional SelectLocationDecision");
                        }

                        this.DecisionDoNotSelectLocation = false;
                        selectLocation.FinishedSelecting = true;
                    }
                    else
                    {
                        var location = selectLocation.Choices.First();
                        Console.WriteLine("No selection was provided, so choosing Location at index {0}", location);
                        selectLocation.SelectedLocation = location;
                    }
                    Console.WriteLine("Selected: " + selectLocation.SelectedLocation);
                }
                else if (decision is UsePowerDecision)
                {
                    UsePowerDecision power = decision as UsePowerDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, power.Choices.Count(), "UsePowerDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }

                    var who = power.HeroTurnTakerController != null ? power.HeroTurnTakerController.Name : "Everybody";

                    Console.WriteLine(who + ", Make a UsePowerDecision: [" + power.Choices.Select(p => p.Title).ToCommaList() + "]");

                    var powerCards = power.Choices.Select(p => p.CardController.Card);
                    if (_includedPowersInNextDecision != null)
                    {
                        _includedPowersInNextDecision.ForEach(e => Assert.IsTrue(powerCards.Contains(e), "SelectPowerDecision did not include: " + e.Title + "."));
                        _includedPowersInNextDecision = null;
                    }

                    if (_notIncludedPowersInNextDecision != null)
                    {
                        _notIncludedPowersInNextDecision.ForEach(e => Assert.IsFalse(powerCards.Contains(e), "SelectPowerDecision should not include: " + e.Title + ". (Choices: " + powerCards.Select(c => c.Title).ToCommaList() + ")"));
                        _notIncludedPowersInNextDecision = null;
                    }

                    if (this.DecisionSelectPowers != null)
                    {
                        power.SelectedPower = power.Choices.Where(p => p.CardController.Card == this.DecisionSelectPowers[this.DecisionSelectPowersIndex]).FirstOrDefault();
                        this.DecisionSelectPowersIndex++;
                    }
                    else if (this.DecisionSelectPower != null)
                    {
                        var powers = power.Choices.Where(p => p.CardController.Card == this.DecisionSelectPower);
                        if (this.DecisionSelectPowerIndex < powers.Count())
                        {
                            power.SelectedPower = powers.ElementAt(this.DecisionSelectPowerIndex);
                        }
                        else
                        {
                            power.SelectedPower = powers.FirstOrDefault();
                        }
                    }

                    if (power.SelectedPower == null)
                    {
                        power.SelectedPower = power.Choices.ElementAt(this.DecisionPowerIndex);
                    }

                    Console.WriteLine("Selected: " + power.SelectedPower.Description);
                }
                else if (decision is UseIncapacitatedAbilityDecision)
                {
                    UseIncapacitatedAbilityDecision ability = decision as UseIncapacitatedAbilityDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, ability.Choices.Count(), "UseIncapacitatedAbilityDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }

                    Console.WriteLine("Make a UseIncapacitatedAbilityDecision: [" + ability.Choices.Select(a => a.Description).ToCommaList() + "]");
                    ability.SelectedAbility = ability.Choices.ElementAt(this.DecisionIncapacitatedAbilityIndex);
                    Console.WriteLine("Selected: " + ability.SelectedAbility.Description);
                }
                else if (decision is SelectCardsDecision)
                {
                    SelectCardsDecision selectCards = decision as SelectCardsDecision;
                    if (selectCards.IsOptional && selectCards.AllowAutoDecide)
                    {
                        Assert.Fail("A SelectCardsDecision may not be both optional and allow for auto-decisions.");
                    }
                    if (selectCards.AllAtOnce)
                    {
                        if (selectCards.NumberOfCards.HasValue)
                        {
                            for (int i = 0; i < selectCards.NumberOfCards.Value; i++)
                            {
                                SelectCardDecision selectCardDecision = selectCards.GetNextSelectCardDecision();
                                this.RunCoroutine(this.MakeDecisions(selectCardDecision));
                            }
                        }
                    }
                    else
                    {
                        selectCards.ReadyForNext = true;
                    }
                }
                else if (decision is SelectTurnTakerDecision)
                {
                    SelectTurnTakerDecision selectTurnTaker = decision as SelectTurnTakerDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        Assert.AreEqual(_numberOfChoicesInNextDecision, selectTurnTaker.Choices.Count(), "SelectTurnTakerDecision has the wrong number of choices.");
                        _numberOfChoicesInNextDecision = null;
                    }


                    string sequence = "";
                    if (selectTurnTaker.NumberOfCards != null)
                    {
                        sequence = ", " + selectTurnTaker.NumberOfCards.Value.ToOrdinalString();
                    }
                    var auto = selectTurnTaker.AllowAutoDecide ? ", Auto" : "";
                    var skip = selectTurnTaker.IsOptional ? ", Skip" : "";
                    var who = selectTurnTaker.HeroTurnTakerController != null ? selectTurnTaker.HeroTurnTakerController.Name + ", " : "";
                    Console.WriteLine(who + "Make a SelectTurnTakerDecision (" + selectTurnTaker.SelectionType + sequence + "): [" + selectTurnTaker.Choices.Select(tt => tt.Name).ToCommaList() + auto + skip + "]");

                    if (_includedTurnTakersInNextDecision != null)
                    {
                        _includedTurnTakersInNextDecision.ForEach(e => Assert.IsTrue(selectTurnTaker.Choices.Contains(e), "SelectTurnTakerDecision did not include: " + e.Name + "."));
                        _includedTurnTakersInNextDecision = null;
                    }

                    if (_notIncludedTurnTakersInNextDecision != null)
                    {
                        _notIncludedTurnTakersInNextDecision.ForEach(e => Assert.IsFalse(selectTurnTaker.Choices.Contains(e), "SelectTurnTakerDecision should not include: " + e.Name + ". (Choices: " + selectTurnTaker.Choices.Select(tt => tt.Name).ToCommaList() + ")"));
                        _notIncludedTurnTakersInNextDecision = null;
                    }

                    if (DecisionDoNotSelectTurnTaker && selectTurnTaker.IsOptional)
                    {
                        selectTurnTaker.SelectedTurnTaker = null;
                        selectTurnTaker.FinishedSelecting = true;
                    }
                    else if (this.DecisionSelectTurnTakers != null)
                    {
                        Console.WriteLine("Using DecisionSelectTurnTakers at index {0}", DecisionSelectTurnTakersIndex);
                        selectTurnTaker.SelectedTurnTaker = this.DecisionSelectTurnTakers.ElementAt(this.DecisionSelectTurnTakersIndex++);
                        if (selectTurnTaker.SelectedTurnTaker == null && selectTurnTaker.IsOptional)
                        {
                            selectTurnTaker.FinishedSelecting = true;
                        }
                    }
                    else if (this.DecisionSelectTurnTaker != null)
                    {
                        selectTurnTaker.SelectedTurnTaker = this.DecisionSelectTurnTaker;
                    }
                    else
                    {
                        selectTurnTaker.SelectedTurnTaker = selectTurnTaker.Choices.FirstOrDefault();
                    }
                    string ttname = selectTurnTaker.SelectedTurnTaker != null ? selectTurnTaker.SelectedTurnTaker.Name : "None";
                    Console.WriteLine("Selected: " + ttname);

                    if (selectTurnTaker.SelectedTurnTaker != null && !selectTurnTaker.Choices.Contains(selectTurnTaker.SelectedTurnTaker))
                    {
                        Assert.Fail("The test selected " + selectTurnTaker.SelectedTurnTaker.Name + ", which is not one of the options: " + selectTurnTaker.Choices.Select(tt => tt.Name).ToCommaList());
                    }
                }
                else if (decision is SelectFunctionDecision)
                {
                    SelectFunctionDecision selectAction = decision as SelectFunctionDecision;

                    if (_numberOfChoicesInNextDecision != null)
                    {
                        var check = true;
                        if (_numberOfChoicesInNextDecisionSelectionType != null && _numberOfChoicesInNextDecisionSelectionType != decision.SelectionType)
                        {
                            check = false;
                        }

                        if (check)
                        {
                            Assert.AreEqual(_numberOfChoicesInNextDecision, selectAction.Choices.Count(), "SelectFunctionDecision has the wrong number of choices.");
                            _numberOfChoicesInNextDecision = null;
                        }
                    }

                    var who = "";
                    if (selectAction.HeroTurnTakerController != null)
                    {
                        who = selectAction.HeroTurnTakerController.Name + ", ";
                    }

                    var skip = decision.IsOptional ? ", Skip" : "";
                    Console.WriteLine(who + "Make a SelectFunctionDecision: [" + selectAction.Choices.Select(d => d.DisplayText).ToCommaList() + skip + "]");

                    if (this.DecisionDoNotSelectFunction)
                    {
                        selectAction.SelectedFunction = null;
                        selectAction.FinishedSelecting = true;
                    }
                    else if (this.DecisionSelectFunction != null || this.DecisionSelectFunctions != null)
                    {
                        if (this.DecisionSelectFunctions != null)
                        {
                            if (this.DecisionSelectFunctions[this.DecisionSelectFunctionsIndex].HasValue)
                            {
                                selectAction.SelectedFunction = selectAction.Choices.ElementAt(this.DecisionSelectFunctions[this.DecisionSelectFunctionsIndex].Value);
                            }
                            else
                            {
                                selectAction.FinishedSelecting = true;
                            }

                            this.DecisionSelectFunctionsIndex++;
                        }
                        else
                        {
                            selectAction.SelectedFunction = selectAction.Choices.ElementAt(this.DecisionSelectFunction.Value);
                        }
                    }
                    else
                    {
                        selectAction.SelectedFunction = selectAction.Choices.FirstOrDefault();
                    }

                    if (selectAction.SelectedFunction != null)
                    {
                        Console.WriteLine("Selected: " + selectAction.SelectedFunction.DisplayText);
                    }
                    else
                    {
                        Console.WriteLine("Selected: No function");
                    }
                }
                else if (decision is SelectNumberDecision)
                {
                    SelectNumberDecision selectNumber = decision as SelectNumberDecision;
                    Console.WriteLine("Make a SelectNumberDecision: [" + selectNumber.Choices.ToCommaList() + "]");

                    if (this.DecisionSelectNumber != null)
                    {
                        selectNumber.SelectedNumber = this.DecisionSelectNumber.Value;
                    }
                    else
                    {
                        selectNumber.SelectedNumber = selectNumber.Choices.FirstOrDefault();
                    }
                    Console.WriteLine("Selected: " + selectNumber.SelectedNumber);
                }
                else if (decision is ActivateAbilityDecision)
                {
                    ActivateAbilityDecision activateAbility = decision as ActivateAbilityDecision;
                    var who = activateAbility.HeroTurnTakerController != null ? activateAbility.HeroTurnTakerController.Name : "Everybody";
                    var skip = activateAbility.IsOptional ? ", Skip" : "";

                    var abilityChoices = activateAbility.Choices.Select(aa => aa.CardController.Card.Title);
                    var choices = "";
                    for (int i = 0; i < abilityChoices.Count(); i++)
                    {
                        choices += abilityChoices.ElementAt(i);
                        var associated = activateAbility.GetAssociatedCard(i);
                        if (associated != null)
                        {
                            choices += " (" + associated.Title + ")";
                        }

                        if (i < abilityChoices.Count() - 1)
                        {
                            choices += ", ";
                        }
                    }
                    choices += skip;

                    Console.WriteLine(who + ", Make an ActivateAbilityDecision: [" + choices + "]");

                    if (this.DecisionDoNotActivatableAbility)
                    {
                        Console.WriteLine("Selected to skip using an ability.");
                        activateAbility.FinishedSelecting = true;
                    }
                    else
                    {
                        if (this.DecisionActivateAbilities != null)
                        {
                            var ability = activateAbility.Choices.Where(aa => aa.CardController.Card == this.DecisionActivateAbilities[this.DecisionActivateAbilitiesIndex]).FirstOrDefault();
                            activateAbility.SelectedAbility = ability;
                            this.DecisionActivateAbilitiesIndex++;
                        }
                        else
                        {
                            activateAbility.SelectedAbility = activateAbility.Choices.FirstOrDefault();
                        }
                        Console.WriteLine("Selected: " + activateAbility.SelectedAbility.CardController.Card.Title);
                    }
                }
                else if (decision is SelectWordDecision)
                {
                    SelectWordDecision selectWord = decision as SelectWordDecision;
                    Console.WriteLine("Make a SelectWordDecision: [" + selectWord.Choices.ToCommaList() + "]");

                    if (this.DecisionSelectWords != null)
                    {
                        var word = this.DecisionSelectWords[this.DecisionSelectWordsIndex];
                        if (!selectWord.Choices.Contains(word))
                        {
                            Assert.Fail("The SelectWordDecision does not contain the word: " + word);
                        }

                        selectWord.SelectedWord = word;
                        this.DecisionSelectWordsIndex++;
                    }
                    else if (this.DecisionSelectWord != null)
                    {
                        if (!selectWord.Choices.Contains(this.DecisionSelectWord))
                        {
                            Assert.Fail("The SelectWordDecision does not contain the word: " + this.DecisionSelectWord);
                        }

                        selectWord.SelectedWord = this.DecisionSelectWord;
                    }
                    else if (this.DecisionSelectWordSkip)
                    {
                        if (!selectWord.IsOptional)
                        {
                            Assert.Fail("The SelectWordDecision is not optional so cannot be skipped.");
                        }

                        selectWord.Skip();
                    }
                    else
                    {
                        Log.Warning("Automatically choosing first word: " + selectWord.Choices.FirstOrDefault());

                        selectWord.SelectedWord = selectWord.Choices.FirstOrDefault();
                    }

                    Console.WriteLine("Selected: " + selectWord.SelectedWord);
                }
                else if (decision is SelectFromBoxDecision)
                {
                    SelectFromBoxDecision selectFromBox = decision as SelectFromBoxDecision;
                    Console.WriteLine("Make a SelectFromBoxDecision: ");

                    if (this.DecisionSelectFromBoxIdentifiers != null
                        && this.DecisionSelectFromBoxIdentifiers.Count() > 0
                        && this.DecisionSelectFromBoxIndex < this.DecisionSelectFromBoxIdentifiers.Count())
                    {
                        var identifier = this.DecisionSelectFromBoxIdentifiers[this.DecisionSelectFromBoxIndex];
                        this.DecisionSelectFromBoxIndex++;
                        if (!selectFromBox.IsIdentifierAllowed(identifier))
                        {
                            Assert.Fail("The SelectFromBoxDecision does not allow the identifier: " + identifier);
                        }

                        selectFromBox.SelectedIdentifier = identifier;
                        selectFromBox.SelectedTurnTakerIdentifier = this.DecisionSelectFromBoxTurnTakerIdentifier;
                    }
                    else
                    {
                        var firstHero = selectFromBox.Choices.FirstOrDefault();
                        var selectedPromo = firstHero.Value;

                        Log.Warning("No hero was provided automatically selecting a variant of the first hero: " + selectedPromo);
                        selectFromBox.SelectedIdentifier = selectedPromo;
                        selectFromBox.SelectedTurnTakerIdentifier = firstHero.Key;
                    }
                    Console.WriteLine("Selected " + selectFromBox.SelectedIdentifier + " from " + selectFromBox.SelectedTurnTakerIdentifier + ".");
                }
                else if (decision is SelectTurnPhaseDecision)
                {
                    SelectTurnPhaseDecision selectPhase = decision as SelectTurnPhaseDecision;
                    Console.WriteLine("Make a SelectTurnPhaseDecision: [" + selectPhase.Choices.Select(tp => tp.Phase).ToCommaList() + "]");

                    if (this.DecisionSelectTurnPhase != null)
                    {
                        if (!selectPhase.Choices.Contains(this.DecisionSelectTurnPhase))
                        {
                            Assert.Fail("The SelectTurnPhaseDecision does not contain the phase: " + this.DecisionSelectTurnPhase);
                        }

                        selectPhase.SelectedPhase = this.DecisionSelectTurnPhase;
                    }
                    else
                    {
                        Log.Warning("Automatically first phase: " + selectPhase.Choices.Select(tp => tp.Phase).FirstOrDefault());

                        selectPhase.SelectedPhase = selectPhase.Choices.FirstOrDefault();
                    }
                }
            }

            this.NumberOfDecisionsAnswered += 1;

            yield return null;
        }

        private IEnumerator MakeDecisions(IDecision decision, string failAfter)
        {
            this.StartCoroutine(MakeDecisions(decision));

            if (failAfter != null)
            {
                Assert.Fail(failAfter);
            }

            yield return null;
        }

        public IEnumerator ReceiveMessage(MessageAction message)
        {
            string msg = message.Message;
            if (message.AssociatedCards != null)
            {
                msg += " [ " + message.AssociatedCards.Select(m => m.Title).ToCommaList() + " ]";
            }
            else if (message.ShowCardSource)
            {
                if (message.CardSource != null)
                {
                    msg += " [ " + message.CardSource.Card.Title + " ]";
                }
                else
                {
                    msg += " [ NULL CARD SOURCE ]";
                    Assert.Fail("Attempted to show null CardSource for message: " + msg);
                }
            }
            if (message is StatusEffectMessageAction)
            {
                if ((message as StatusEffectMessageAction).State == StatusEffectMessageAction.StatusEffectState.Removing)
                {
                    msg = "Expiring: " + msg;
                }
            }
            Console.WriteLine("Message: " + msg);
            yield return null;
        }

        #region Convenience Methods

        protected IEnumerable<Card> PutIntoPlay(TurnTakerController ttc, params string[] ids)
        {
            return PlayCards(ids, true);
        }

        protected Card PutIntoPlay(string cardIdentifier)
        {
            var card = GetCard(cardIdentifier);
            var ttc = this.GameController.FindTurnTakerController(card.Owner);
            PutIntoPlay(ttc, new string[] { cardIdentifier });
            return card;
        }

        protected Card PutInHand(HeroTurnTakerController httc, string identifier, int index = 0)
        {
            return this.MoveCard(httc, GetCard(identifier, index), httc.HeroTurnTaker.Hand);
        }

        protected void PutInHand(HeroTurnTakerController httc, Card card)
        {
            this.MoveCard(httc, card, httc.HeroTurnTaker.Hand);
        }

        protected void PutInDeck(HeroTurnTakerController httc, Card card)
        {
            this.MoveCard(httc, card, httc.HeroTurnTaker.Deck);
        }

        protected Card PutInHand(string identifier)
        {
            return PutInHand(GetCard(identifier));
        }

        protected void PutInHand(IEnumerable<Card> cards)
        {
            cards.ForEach(c => PutInHand(c));
        }

        protected Card PutInHand(Card card)
        {
            PutInHand(this.GameController.FindHeroTurnTakerController(card.Owner.ToHero()), card);
            return card;
        }

        protected Card PutInDeck(string identifier)
        {
            Card card = GetCard(identifier);
            PutInDeck(this.GameController.FindHeroTurnTakerController(card.Owner.ToHero()), card);
            return card;
        }

        protected Card PutInDeck(Card card)
        {
            PutInDeck(this.GameController.FindHeroTurnTakerController(card.Owner.ToHero()), card);
            return card;
        }

        protected void PutInHand(HeroTurnTakerController httc, Card[] cards)
        {
            foreach (Card card in cards)
            {
                PutInHand(httc, card);
            }
        }

        protected IList<Card> PutInHand(HeroTurnTakerController httc, string[] identifiers)
        {
            List<Card> results = new List<Card>(identifiers.Length);
            foreach (string id in identifiers)
            {
                var card = PutInHand(httc, id);
                results.Add(card);
            }

            return results;
        }

        protected Card PutInTrash(string identifier, int index = 0)
        {
            Card card = GetCard(identifier, index);
            PutInTrash(card);
            return card;
        }

        protected IEnumerable<Card> PutInTrash(params string[] identifiers)
        {
            return identifiers.Select(s => PutInTrash(s)).ToList();
        }

        protected void PutInTrash(IEnumerable<Card> cards)
        {
            cards.ForEach(c => PutInTrash(c));
        }

        protected void PutInTrash(TurnTakerController ttc, Func<Card, bool> cardCriteria)
        {
            var cards = FindCardsWhere(c => c.Owner == ttc.TurnTaker && cardCriteria(c));
            foreach (Card card in cards)
            {
                PutInTrash(card);
            }
        }

        protected void PutInTrash(TurnTakerController ttc, IEnumerable<Card> cards)
        {
            cards.ForEach(c => PutInTrash(ttc, c));
        }

        protected Card PutInTrash(Card card)
        {
            var ttc = this.GameController.FindTurnTakerController(card.Owner);
            MoveCard(ttc, card, ttc.TurnTaker.Trash);
            return card;
        }

        protected void PutInTrash(TurnTakerController ttc, Card card)
        {
            MoveCard(ttc, card, ttc.TurnTaker.Trash);
        }

        protected IEnumerable<Card> PutInTrash(string[] identifiers, int index = 0)
        {
            return identifiers.Select(id => PutInTrash(id, index)).ToList();
        }

        protected void PutInTrash(Card[] cards, int index = 0)
        {
            foreach (var card in cards)
            {
                PutInTrash(card);
            }
        }

        protected void StartGame(bool resetDecisions = true)
        {
            PrintSeparator("START GAME");

            if (resetDecisions)
            {
                ResetDecisions();
            }

            RunCoroutine(this.GameController.StartGame());

            // FLAKY FIX:
            // If Unity starts without a bot in hand, it causes tests to fail when they assert messages or 
            // try to go to her play card phase. Make sure that never happens.
            var unityTTC = FindHero("Unity", false);
            if (unityTTC != null && unityTTC.HeroTurnTaker.Hand.Cards.All(c => IsMechanicalGolem(c)))
            {
                // Replace one with a non-mechanical golem.
                Console.WriteLine("WARNING: Unity started with all mechanical golems in hand, replacing one of them with something else!");
                MoveCard(unityTTC, unityTTC.HeroTurnTaker.Hand.TopCard, unityTTC.TurnTaker.Deck);
                MoveCard(unityTTC, unityTTC.TurnTaker.Deck.Cards.FirstOrDefault(c => !IsMechanicalGolem(c)), unityTTC.HeroTurnTaker.Hand);
            }

            EnterNextTurnPhase();
        }

        protected void DestroyNonCharacterVillainCards()
        {
            DestroyCards(c => c.IsVillain && !c.IsCharacter);
        }

        protected GameController SaveAndLoad(GameController controller = null)
        {
            if (controller == null)
            {
                controller = this.GameController;
            }

            var oldStateString = controller.Game.ToStateString();
            SaveGameToTemp("UnitTest");
            var result = LoadGame("UnitTest", false, true, true);
            var newStateString = result.Game.ToStateString();
            Assert.AreEqual(oldStateString, newStateString, "Game state string should be the same after saving and loading");

            return result;
        }

        protected void TakeDamage(TurnTakerController ttc, int amount)
        {
            TakeDamage(ttc.CharacterCard, amount);
        }

        protected void TakeDamage(Card card, int amount)
        {
            card.TakeDamage(amount);
        }

        private void GoToPhase(TurnTakerController ttc, Phase phase)
        {
            // Sanity check, at this point there should be no cards in revealed locations.
            foreach (var tt in ttc.GameController.Game.TurnTakers)
            {
                Assert.AreEqual(0, tt.Revealed.Cards.Count(), tt.Name + " has cards in Revealed!");

                // All cards should have a location
                Assert.IsTrue(tt.GetAllCards().All(c => c.Location != null), "All cards should have a location");
            }

            // We would get an endless loop if we are going to the PlayCard phase with no cards left in hand, so fail the test.
            if (phase == Phase.PlayCard && ttc.IsHero && this.GameController.GetPlayableCardsInHand(ttc.ToHero(), false).Count() == 0)
            {
                Assert.Fail("Tried to go to PlayCard phase while no more playable cards are in the player's hand.");
            }

            int sanity = 1000;
            do
            {
                if (!this.AllowGameOverDuringGoToPhase)
                {
                    // The game should not end while we're doing this.
                    AssertNotGameOver("The game should not be over while doing a GoToPhase call.");
                }

                EnterNextTurnPhase();

                sanity -= 1;
                if (sanity < 0)
                {
                    Assert.Fail("Sanity check failed in GoToPhase");
                }
            } while (!(this.GameController.ActiveTurnPhase.Phase == phase && this.GameController.ActiveTurnTakerController == ttc));

            Console.WriteLine("--- " + ttc.Name + "'s " + phase + " phase ---");
        }

        protected void GoToBeforeStartOfTurn(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.BeforeStart);
        }

        protected void GoToStartOfTurn(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.Start);
        }

        protected void GoToPlayCardPhase(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.PlayCard);
        }

        protected Card GoToPlayCardPhaseAndPlayCard(TurnTakerController ttc, string identifier)
        {
            GoToPlayCardPhase(ttc);
            return PlayCard(identifier);
        }

        protected void GoToUsePowerPhase(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.UsePower);
        }

        protected void GoToDrawCardPhase(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.DrawCard);
        }

        protected void GoToUseIncapacitatedAbilityPhase(TurnTakerController ttc)
        {
            GoToPhase(ttc, Phase.UseIncapacitatedAbility);
        }

        protected void GoToEndOfTurn(TurnTakerController ttc = null)
        {
            if (ttc == null)
            {
                ttc = this.GameController.ActiveTurnTakerController;
            }

            GoToPhase(ttc, Phase.End);
        }

        protected void GoToAfterEndOfTurn(TurnTakerController ttc = null)
        {
            if (ttc == null)
            {
                ttc = this.GameController.ActiveTurnTakerController;
            }

            GoToPhase(ttc, Phase.AfterEnd);
        }

        protected void StoreHP(TurnTakerController ttc, ref int characterHP)
        {
            StoreHP(ttc.CharacterCard, ref characterHP);
        }

        protected void StoreHP(Card card, ref int cardHP)
        {
            cardHP = card.HitPoints.Value;
        }

        // Set "a"'s hit points to be the same as "b"'s.
        protected void SetToSameHitPoints(TurnTakerController a, TurnTakerController b)
        {
            SetToSameHitPoints(a.CharacterCard, b.CharacterCard);
        }

        protected int SetHitPoints(TurnTakerController ttc, int amount)
        {
            SetHitPoints(ttc.CharacterCard, amount);
            return amount;
        }

        protected void SetHitPoints(IEnumerable<TurnTakerController> ttcs, int amount)
        {
            ttcs.ForEach(ttc => SetHitPoints(ttc, amount));
        }

        protected int SetHitPoints(Func<Card, bool> cardCriteria, int amount)
        {
            SetHitPoints(FindCardsWhere(c => c.IsTarget && cardCriteria(c)), amount);
            return amount;
        }

        protected int SetHitPoints(IEnumerable<Card> targets, int amount)
        {
            foreach (var card in targets)
            {
                SetHitPoints(card, amount);
            }
            return amount;
        }

        protected int SetHitPoints(Card card, int amount)
        {
            if (amount < card.HitPoints.Value)
            {
                card.TakeDamage(card.HitPoints.Value - amount);
            }
            else if (amount > card.HitPoints.Value)
            {
                GainHP(card, amount - card.HitPoints.Value);
            }
            return amount;
        }

        // Set "a"'s hit points to be the same as "b"'s.
        protected void SetToSameHitPoints(Card changeHP, Card fixedHP)
        {
            int diff = fixedHP.HitPoints.Value - changeHP.HitPoints.Value;
            if (diff < 0)
            {
                changeHP.TakeDamage(-diff);
            }
            else if (diff > 0)
            {
                changeHP.GainHP(changeHP, diff);
            }
            Console.WriteLine("Set HP of " + changeHP.Title + " to match " + fixedHP.Title + " at " + changeHP.HitPoints.Value + ".");
        }

        protected int GetHitPoints(TurnTakerController ttc)
        {
            return ttc.CharacterCard.HitPoints.Value;
        }

        protected int GetHitPoints(Card card)
        {
            return card.HitPoints.Value;
        }

        protected int GetNumberOfCardsInPlay(TurnTakerController ttc)
        {
            return ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay).Count();
        }

        protected int GetNumberOfCardsInPlay(Func<Card, bool> cardCriteria)
        {
            return FindCardsWhere(c => c.IsInPlay && cardCriteria(c)).Count();
        }

        protected int GetNumberOfCardsInDeck(TurnTakerController ttc)
        {
            return ttc.TurnTaker.GetAllCards().Where(c => c.IsInDeck).Count();
        }

        protected int GetNumberOfCardsInHand(HeroTurnTakerController httc)
        {
            return httc.HeroTurnTaker.Hand.Cards.Count();
        }

        protected int GetNumberOfCardsInTrash(TurnTakerController ttc, Func<Card, bool> cardCriteria = null)
        {
            if (cardCriteria == null)
            {
                cardCriteria = card => true;
            }
            return ttc.TurnTaker.Trash.Cards.Where(cardCriteria).Count();
        }

        protected int GetNumberOfCardsNextToCard(Card card)
        {
            return card.NextToLocation.Cards.Count();
        }

        protected int GetNumberOfCardsUnderCard(Card card)
        {
            return card.UnderLocation.Cards.Count();
        }

        protected Card GetCard(string identifier, int index = 0, Func<Card, bool> criteria = null)
        {
            IEnumerable<Card> cards = GetCards(identifier);

            if (criteria != null)
            {
                cards = cards.Where(criteria);
            }

            Card result = null;

            // Show preference to cards that are in the deck or the trash.
            IEnumerable<Card> inDeck = cards.Where(card => card.Location.IsDeck || card.Location.IsTrash);
            if (inDeck.Count() > index)
            {
                result = inDeck.ElementAtOrDefault(index);
            }

            if (result == null)
            {
                // Otherwise, grab it from the hand, if hero.
                result = cards.Where(card => card.Location.IsHand).FirstOrDefault();

                if (result == null)
                {
                    // Otherwise, we'll just take what we can get.
                    result = cards.ElementAtOrDefault(index);
                }
            }

            if (result == null)
            {
                Assert.Fail("Tried to get card by identifer: " + identifier + " but could not make a card instance.");
            }

            return result;
        }

        protected TurnTakerController GetTurnTakerController(Card card)
        {
            return this.GameController.FindTurnTakerController(card.Owner);
        }

        protected Card GetTopCardOfDeck(TurnTakerController ttc, int offset = 0)
        {
            return ttc.TurnTaker.Deck.Cards.ElementAt(ttc.TurnTaker.Deck.Cards.Count() - offset - 1);
        }

        protected IEnumerable<Card> GetTopCardsOfDeck(TurnTakerController ttc, int numberOfCards)
        {
            return ttc.TurnTaker.Deck.GetTopCards(numberOfCards);
        }

        protected Card GetBottomCardOfDeck(TurnTakerController ttc, int offset = 0)
        {
            return ttc.TurnTaker.Deck.Cards.ElementAt(offset);
        }

        protected void AssertNotOnTopOfDeck(TurnTakerController ttc, Card card, int offset = 0)
        {
            Assert.AreNotEqual(card, GetTopCardOfDeck(ttc, offset));
        }

        protected void AssertNotOnBottomOfDeck(TurnTakerController ttc, Card card, int offset = 0)
        {
            Assert.AreNotEqual(card, GetBottomCardOfDeck(ttc, offset));
        }

        protected Card GetTopMatchingCardOfLocation(Location location, Func<Card, bool> criteria, int foundIndex = 1)
        {
            int found = 0;
            foreach (Card c in location.Cards.Reverse())
            {
                if (criteria(c))
                {
                    found++;
                    if (found == foundIndex)
                    {
                        return c;
                    }
                }
            }
            return null;
        }

        protected Card GetCardInPlay(string identifier, int index = 0)
        {
            var cards = FindCardsWhere(c => c.Identifier == identifier && c.IsInPlay);
            if (cards.Count() == 0)
            {
                Assert.Fail("There are no cards with identifier \"" + identifier + "\" in play.");
            }
            return cards.ElementAt(index);
        }

        protected IEnumerable<Card> GetCards(string identifier)
        {
            return this.GameController.FindCardsWhere(card => card.Identifier == identifier, false);
        }

        protected IEnumerable<Card> GetCards(params string[] identifiers)
        {
            var result = new List<Card>();
            foreach (var id in identifiers)
            {
                result.Add(GetCard(id));
            }
            return result;
        }

        protected Card GetRandomCardFromHand(HeroTurnTakerController hero)
        {
            if (hero.HeroTurnTaker.HasCardsInHand)
            {
                return hero.HeroTurnTaker.Hand.Cards.First();
            }
            else
            {
                Assert.Fail(hero.Name + " has no cards in hand to choose from.");
                return null;
            }
        }

        protected Card GetCardFromHand(HeroTurnTakerController hero, string identifier, int index = 0)
        {
            return FindCardsWhere(card => card.Location == hero.HeroTurnTaker.Hand && card.Identifier == identifier).ElementAt(index);
        }

        protected Card GetCardFromTrash(TurnTakerController tt, string identifier, int index = 0)
        {
            return FindCardsWhere(card => card.Location == tt.TurnTaker.Trash && card.Identifier == identifier).ElementAt(index);
        }

        protected Card GetCardFromHand(string identifier, int index = 0)
        {
            return this.GetCardFromHand(this.GameController.FindHeroTurnTakerController(GetCard(identifier).Owner.ToHero()), identifier, index);
        }

        protected Card GetCardFromHand(HeroTurnTakerController hero, int index = 0)
        {
            return hero.HeroTurnTaker.Hand.Cards.ElementAt(index);
        }

        protected void EnterNextTurnPhase(int times = 1)
        {
            for (int i = 0; i < times; i++)
            {
                this.RunCoroutine(this.GameController.EnterNextTurnPhase());
            }
        }

        protected void StartTurnPhaseListStorage()
        {
            turnPhaseList = new List<TurnPhase>();
        }

        protected void StopTurnPhaseListStorage()
        {
            turnPhaseList = null;
        }

        protected List<TurnPhase> TurnPhaseListCheck()
        {
            return turnPhaseList;
        }

        protected IEnumerable<Card> DrawCard(HeroTurnTakerController httc, int numberOfCards = 1, bool optional = false)
        {
            var storedResults = new List<DrawCardAction>();
            for (int i = 0; i < numberOfCards; i++)
            {
                this.RunCoroutine(this.GameController.DrawCard(httc.HeroTurnTaker, optional, storedResults));
            }
            return storedResults.Select(d => d.DrawnCard);
        }

        protected Card PlayCard(TurnTakerController ttc, string identifier, int index = 0, bool isPutIntoPlay = false)
        {
            Card card = GetCard(identifier, index);
            if (card != null)
            {
                PlayCard(ttc, card, isPutIntoPlay);
            }
            else
            {
                throw new Exception("No such card with identifier: " + identifier);
            }
            return card;
        }

        protected Card PlayCard(string identifier, int index = 0, bool isPutIntoPlay = false)
        {
            Card card = GetCard(identifier, index);
            if (card != null)
            {
                var cc = FindCardController(card);
                if (cc.AreTriggersActive)
                {
                    FindCardController(card).RemoveAllTriggers(true);
                }

                PlayCard(this.GameController.FindTurnTakerController(card.Owner), card, isPutIntoPlay);
            }
            else
            {
                throw new Exception("No such card with identifier: " + identifier);
            }
            return card;
        }

        protected Card PlayCard(Card card, bool isPutIntoPlay = false)
        {
            PlayCard(this.GameController.FindTurnTakerController(card.Owner), card, isPutIntoPlay);
            return card;
        }

        protected IEnumerable<Card> PlayCards(TurnTakerController ttc, IEnumerable<string> cardsToPlay)
        {
            List<Card> cards = new List<Card>();
            foreach (string card in cardsToPlay)
            {
                var playedCard = PlayCard(ttc, card);
                cards.Add(playedCard);
            }
            return cards;
        }

        protected IEnumerable<Card> PlayCards(params Card[] cards)
        {
            cards.ForEach(c => PlayCard(c));
            return cards;
        }

        protected IEnumerable<Card> PlayCards(params string[] ids)
        {
            var cards = GetCards(ids);
            return PlayCards(cards);
        }

        protected IEnumerable<Card> PlayCards(Func<Card, bool> cardCriteria, int? maxNumberOfCards = null)
        {
            var cards = FindCardsWhere(cardCriteria);
            if (maxNumberOfCards != null)
            {
                cards.Take(maxNumberOfCards.Value).ForEach(c => PlayCard(c));
            }
            else
            {
                cards.ForEach(c => PlayCard(c));
            }
            return cards;
        }

        protected IEnumerable<Card> PlayCards(IEnumerable<string> cardsToPlay, bool isPutIntoPlay = false)
        {
            List<Card> cards = new List<Card>();
            foreach (string card in cardsToPlay)
            {
                var playedCard = PlayCard(card, 0, isPutIntoPlay);
                cards.Add(playedCard);
            }
            return cards;
        }

        protected void PlayCards(TurnTakerController ttc, IEnumerable<Card> cardsToPlay)
        {
            foreach (Card card in cardsToPlay)
            {
                PlayCard(ttc, card);
            }
        }

        protected IEnumerable<Card> PlayCards(IEnumerable<Card> cardsToPlay)
        {
            foreach (Card card in cardsToPlay)
            {
                PlayCard(card);
            }

            return cardsToPlay;
        }

        protected Card PlayCardFromHand(HeroTurnTakerController hero, string identifier)
        {
            var card = hero.HeroTurnTaker.Hand.Cards.Where(c => c.Identifier == identifier).FirstOrDefault();
            if (card != null)
            {
                PlayCard(card);
            }
            else
            {
                Assert.Fail("Could not find card in " + hero.Name + "'s hand: " + identifier);
            }
            return card;
        }

        protected Card MoveIntoPlay(TurnTakerController ttc, Card card, TurnTaker whosePlayArea, CardSource cardSource = null)
        {
            if (card != null)
            {
                RunCoroutine(this.GameController.MoveIntoPlay(ttc, card, whosePlayArea, cardSource));
            }
            else
            {
                Assert.Fail("Tried to move a null card into play.");
            }
            return card;
        }

        protected Card MoveCard(TurnTakerController ttc, Card card, Location location, bool toBottom = false, bool playIfPlayArea = true, bool overrideIndestructible = false, CardSource cardSource = null)
        {
            if (card != null)
            {
                if (!overrideIndestructible)
                {
                    RunCoroutine(this.GameController.MoveCard(ttc, card, location, toBottom, playCardIfMovingToPlayArea: playIfPlayArea, cardSource: cardSource));
                }
                else
                {
                    ttc.TurnTaker.MoveCard(card, location, toBottom);
                    RemoveCardTriggers(card, false);
                }
            }
            else
            {
                Assert.Fail("Tried to move a null card.");
            }
            return card;
        }

        protected Card MoveCard(TurnTakerController ttc, string identifier, Location location, bool toBottom = false)
        {
            return MoveCard(ttc, GetCard(identifier), location, toBottom);
        }

        protected void MoveCards(TurnTakerController ttc, IEnumerable<Card> cards, Location location, bool toBottom = false, bool playIfPlayArea = true, int leaveSomeCards = 0, bool overrideIndestructible = false)
        {
            var copyCards = cards.ToArray();
            for (int i = 0; i < copyCards.Count() - leaveSomeCards; i++)
            {
                var card = copyCards.ElementAt(i);
                MoveCard(ttc, card, location, toBottom, playIfPlayArea, overrideIndestructible);
            }
        }

        protected void MoveCards(TurnTakerController ttc, IEnumerable<Card> cards, Func<Card, Location> locationBasedOnCard, bool toBottom = false, bool playIfPlayArea = true, int leaveSomeCards = 0, bool overrideIndestructible = false)
        {
            var copyCards = cards.ToArray();
            for (int i = 0; i < copyCards.Count() - leaveSomeCards; i++)
            {
                var card = copyCards.ElementAt(i);
                MoveCard(ttc, card, locationBasedOnCard(card), toBottom, playIfPlayArea, overrideIndestructible);
            }
        }

        protected void MoveCards(TurnTakerController ttc, Func<Card, bool> cardCriteria, Location location, bool toBottom = false, int? numberOfCards = null, bool overrideIndestructible = false, bool playIfPlayArea = true)
        {
            var cards = FindCardsWhere(cardCriteria);
            if (numberOfCards.HasValue)
            {
                cards = cards.Take(numberOfCards.Value);
            }
            MoveCards(ttc, cards, location, toBottom, playIfPlayArea, overrideIndestructible: overrideIndestructible);
        }

        protected IEnumerable<Card> MoveCards(TurnTakerController ttc, IEnumerable<string> identifiers, Location location, bool toBottom = false)
        {
            List<Card> results = new List<Card>();
            for (int i = identifiers.Count() - 1; i >= 0; i--)
            {
                Card card = MoveCard(ttc, GetCard(identifiers.ElementAt(i)), location, toBottom);
                results.Add(card);
            }
            return results;
        }

        protected void MoveAllCards(TurnTakerController ttc, Location source, Location destination, bool playIfPlayArea = true, int leaveSomeCards = 0)
        {
            this.MoveCards(ttc, source.Cards, destination, playIfPlayArea: playIfPlayArea, leaveSomeCards: leaveSomeCards);
        }

        protected void MoveAllCardsFromHandToDeck(HeroTurnTakerController ttc)
        {
            this.MoveAllCards(ttc, ttc.HeroTurnTaker.Hand, ttc.TurnTaker.Deck);
        }

        protected void MoveCardsToPlayAreaWithoutPlaying(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                MoveCard(this.GameController.FindTurnTakerController(card.Owner), card, card.Owner.PlayArea, playIfPlayArea: false);
            }
        }

        protected Card PlayCard(TurnTakerController ttc, Card card, bool isPutIntoPlay = false, bool evenIfAlreadyInPlay = false, Location overridePlayLocation = null)
        {
            RunCoroutine(this.GameController.PlayCard(ttc, card, isPutIntoPlay, overridePlayLocation: overridePlayLocation, evenIfAlreadyInPlay: evenIfAlreadyInPlay));
            return card;
        }

        protected Card PlayTopCard(TurnTakerController ttc)
        {
            List<Card> card = new List<Card>();
            RunCoroutine(this.GameController.PlayTopCard(null, ttc, playedCards: card));
            return card.FirstOrDefault();
        }

        protected Card PlayTopCard(TurnTakerController ttc, Location location, Location overridePlayLocation = null)
        {
            List<Card> card = new List<Card>();
            RunCoroutine(this.GameController.PlayTopCardOfLocation(ttc, location, overridePlayLocation: overridePlayLocation, playedCards: card));
            return card.FirstOrDefault();
        }

        protected void ShuffleDeck(TurnTakerController ttc)
        {
            RunCoroutine(this.GameController.ShuffleLocation(ttc.TurnTaker.Deck, null));
        }

        protected void ShuffleLocation(Location location)
        {
            RunCoroutine(this.GameController.ShuffleLocation(location, null));
        }

        protected IEnumerable<Card> DiscardTopCards(TurnTaker tt, int amount)
        {
            var cards = tt.Deck.GetTopCards(amount);
            RunCoroutine(this.GameController.DiscardTopCards(null, tt.Deck, amount));
            return cards;
        }

        protected void DiscardTopCards(Location location, int amount)
        {
            RunCoroutine(this.GameController.DiscardTopCards(null, location, amount));
        }

        protected IEnumerable<Card> DiscardTopCards(TurnTakerController ttc, int amount)
        {
            return DiscardTopCards(ttc.TurnTaker, amount);
        }

        protected Card DestroyCard(Card card, Card cardSource = null)
        {
            CardSource cs = null;
            if (cardSource != null)
            {
                cs = new CardSource(this.GameController.FindCardController(cardSource));
            }
            RunCoroutine(this.GameController.DestroyCard(null, card, cardSource: cs));
            return card;
        }

        protected Card DestroyCard(string identifier)
        {
            return DestroyCard(GetCardInPlay(identifier));
        }

        protected Card DestroyCard(HeroTurnTakerController hero)
        {
            return DestroyCard(hero.CharacterCard);
        }

        protected void DestroyNonEssentialSpiteCards()
        {
            DestroyCards(c => c.IsInPlay && c.IsVillain && !c.IsCharacter && c.Identifier != "SafeHouse");
        }

        protected void DestroyCardAndMoveToDeck(Card card, Card cardSource = null)
        {
            this.DestroyCard(card, cardSource);
            this.MoveCard(this.GameController.FindTurnTakerController(card.Owner), card, card.Owner.Deck);
        }

        protected IEnumerable<Card> DestroyCards(Func<Card, bool> criteria)
        {
            Func<Card, bool> fullCriteria = c => c.IsInPlayAndHasGameText && criteria(c);
            IEnumerable<Card> cards = FindCardsWhere(fullCriteria);
            cards.ForEach(card => DestroyCard(card));
            return cards;
        }

        protected IEnumerable<Card> DestroyCards(params Card[] cards)
        {
            var cardList = new List<Card>();
            cardList.AddRange(cards);
            return DestroyCards(cardList);
        }

        protected IEnumerable<Card> DestroyCards(IEnumerable<Card> cards)
        {
            foreach (Card card in cards.ToList())
            {
                DestroyCard(card);
            }

            return cards;
        }

        protected void UsePower(HeroTurnTakerController hero, int powerIndex = 0)
        {
            UsePower(hero.CharacterCard, powerIndex);
        }

        protected Card UsePower(string identifier, int powerIndex = 0)
        {
            var powerCard = FindCardInPlay(identifier);
            if (powerCard != null)
            {
                this.RunCoroutine(this.GameController.UsePower(powerCard, powerIndex));
            }
            else
            {
                Assert.Fail("Attempted to use a power from a card not in play");
            }
            return powerCard;
        }

        protected void UsePower(Card card, int powerIndex = 0)
        {
            this.RunCoroutine(this.GameController.UsePower(card, powerIndex));
        }

        protected void UseIncapacitatedAbility(HeroTurnTakerController hero, int abilityIndex)
        {
            this.RunCoroutine(this.GameController.UseIncapacitatedAbility(hero, abilityIndex));
        }

        protected void UseIncapacitatedAbility(Card hero, int abilityIndex)
        {
            if (hero.IsHeroCharacterCard)
            {
                var heroCC = FindCardController(hero) as HeroCharacterCardController;
                this.RunCoroutine(heroCC.UseIncapacitatedAbility(abilityIndex));
            }
        }

        protected void DiscardAllCards(HeroTurnTakerController hero)
        {
            for (int i = hero.HeroTurnTaker.Hand.Cards.Count() - 1; i >= 0; i--)
            {
                Card card = hero.HeroTurnTaker.Hand.Cards.ElementAt(i);
                this.RunCoroutine(this.GameController.DiscardCard(hero, card, null));
            }
        }

        protected void DiscardAllCards(params HeroTurnTakerController[] heroes)
        {
            heroes.ForEach(htt => DiscardAllCards(htt));
        }

        protected void DealDamage(TurnTakerController source, TurnTakerController target, int amount, DamageType type, bool isIrreducible = false, bool ignoreBattleZone = false)
        {
            DealDamage(source.CharacterCard, target.CharacterCard, amount, type, isIrreducible, ignoreBattleZone);
        }

        protected void DealDamage(TurnTakerController source, Card target, int amount, DamageType type, bool ignoreBattleZone = false)
        {
            DealDamage(source.CharacterCard, target, amount, type, ignoreBattleZone: ignoreBattleZone);
        }

        protected void DealDamage(TurnTakerController source, Func<Card, bool> targetCriteria, int amount, DamageType type)
        {
            this.RunCoroutine(this.GameController.DealDamage(source.FindDecisionMaker(), source.CharacterCard, targetCriteria, amount, type, cardSource: new CardSource(source.CharacterCardController)));
        }

        protected void DealDamage(Card source, Func<Card, bool> targetCriteria, int amount, DamageType type)
        {
            this.RunCoroutine(this.GameController.DealDamage(null, source, targetCriteria, amount, type, cardSource: new CardSource(this.GameController.FindCardController(source))));
        }

        protected void DealDamage(Card source, IEnumerable<Card> targets, int amount, DamageType type)
        {
            this.RunCoroutine(this.GameController.DealDamage(null, source, c => targets.Contains(c), amount, type, cardSource: new CardSource(this.GameController.FindCardController(source))));
        }

        protected void DealDamage(Card source, Card target, int amount, DamageType type, bool isIrreducible = false, bool ignoreBattleZone = false)
        {
            this.RunCoroutine(this.GameController.DealDamageToTarget(new DamageSource(this.GameController, source), target, amount, type, isIrreducible, ignoreBattleZone: ignoreBattleZone, cardSource: new CardSource(this.GameController.FindCardController(source))));
        }

        protected void DealDamage(Card source, TurnTakerController target, int amount, DamageType type, bool isIrreducible = false)
        {
            DealDamage(source, target.CharacterCard, amount, type);
        }

        protected void SelectTargetAndDealDamage(HeroTurnTakerController hero, Card source, int amount, DamageType type)
        {
            this.RunCoroutine(this.GameController.SelectTargetsAndDealDamage(hero, new DamageSource(this.GameController, source), amount, type, 1, false, null, cardSource: new CardSource(this.GameController.FindCardController(source))));
        }

        protected void GainHP(Card card, int amount)
        {
            this.RunCoroutine(this.GameController.GainHP(card, amount));
        }

        protected void RestoreToMaxHP(TurnTakerController ttc)
        {
            RestoreToMaxHP(ttc.CharacterCard);
        }

        protected void RestoreToMaxHP(Card card)
        {
            if (card.HitPoints.HasValue && card.MaximumHitPoints.HasValue && card.MaximumHitPoints.Value > card.HitPoints.Value)
            {
                GainHP(card, card.MaximumHitPoints.Value - card.HitPoints.Value);
            }
        }

        protected void GainHP(TurnTakerController ttc, int amount)
        {
            GainHP(ttc.CharacterCard, amount);
        }

        protected void FlipCard(CardController card)
        {
            this.RunCoroutine(this.GameController.FlipCard(card, cardSource: new CardSource(card)));
        }

        protected void FlipCard(Card card)
        {
            FlipCard(this.GameController.FindCardController(card));
        }

        protected void FlipCard(TurnTakerController ttc)
        {
            this.RunCoroutine(this.GameController.FlipCard(ttc.CharacterCardController));
        }

        protected void FlipCards(params Card[] cards)
        {
            cards.ForEach(c => FlipCard(c));
        }

        protected void FlipCards(IEnumerable<Card> cards)
        {
            cards.ForEach(c => FlipCard(c));
        }

        protected void ReplaceOnMakeDecisions(GameControllerDecisionEvent decider)
        {
            this.GameController.OnMakeDecisions -= this.MakeDecisions;
            this.GameController.OnMakeDecisions += decider;
        }

        protected void RestoreOnMakeDecisions(GameControllerDecisionEvent decider)
        {
            this.GameController.OnMakeDecisions -= decider;
            this.GameController.OnMakeDecisions += this.MakeDecisions;
        }

        protected IEnumerable<DamagePreviewResult> GetDamagePreviewResults(Card source, Card target, int amount, DamageType? damageType, bool isIrreducible = false)
        {
            var results = this.GameController.GetDamagePreviewResults(new DamageSource(this.GameController, source), target, amount, null, damageType, isIrreducible);
            OutputDamagePreviewResults(results);
            return results;
        }

        protected IEnumerable<DamagePreviewResult> GetDamagePreviewResults(TurnTaker source, Card target, int amount, DamageType? damageType, bool isIrreducible = false)
        {
            var results = this.GameController.GetDamagePreviewResults(new DamageSource(this.GameController, source), target, amount, null, damageType, isIrreducible);
            OutputDamagePreviewResults(results);
            return results;
        }

        protected IEnumerable<DamagePreviewResult> GetDamagePreviewResults(TurnTakerController source, TurnTakerController target, int amount, DamageType? damageType, bool isIrreducible = false)
        {
            return GetDamagePreviewResults(source.CharacterCard, target.CharacterCard, amount, damageType, isIrreducible);
        }

        protected IEnumerable<DamagePreviewResult> GetDamagePreviewResults(DamageSource source, Card target, Func<Card, int?> amount, DamageType? damageType, bool isIrreducible = false, CardSource cardSource = null)
        {
            var results = this.GameController.GetDamagePreviewResults(source, target, 0, amount, damageType, isIrreducible, cardSource);
            OutputDamagePreviewResults(results);
            return results;
        }

        protected IEnumerable<DamagePreviewResult> GetDamagePreviewResults(DealDamageAction dealDamage, Card selectedTarget)
        {
            var results = this.GameController.GetDamagePreviewResults(dealDamage, selectedTarget);
            OutputDamagePreviewResults(results);
            return results;
        }

        private void OutputDamagePreviewResults(IEnumerable<DamagePreviewResult> results)
        {
            foreach (DamagePreviewResult preview in results)
            {
                if (preview.DoesOrderAffectOutcome)
                {
                    Console.WriteLine("Outcome is affected by the order of effects.");
                }
                else if (preview.DealDamageAction != null && preview.Amount.HasValue)
                {
                    int displayAmount = preview.Amount.Value;
                    if (preview.Amount < 0)
                    {
                        displayAmount = 0;
                    }

                    string sourceTitle = preview.DamageSource == null ? "UNKNOWN" : preview.DamageSource.TitleOrName;
                    string targetTitle = preview.Target == null ? "UNKNOWN" : preview.Target.Title;
                    string type = preview.DamageType.HasValue ? "" + preview.DamageType.Value : "UNKNOWN";
                    Console.WriteLine("Final Result: {0} deals {1} {2} damage to {3}.", sourceTitle, displayAmount, type, targetTitle);
                }
                else
                {
                    Console.WriteLine("Unknown Result.");
                }
                if (!preview.DoesOrderAffectOutcome)
                {
                    foreach (GameAction action in preview.OrderedGameActions)
                    {
                        string cardsource = "";
                        if (action.CardSource != null)
                        {
                            cardsource = action.CardSource.Card.Title;
                            if (action.CardSource.Card.IsHero && action.CardSource.Card.IsCharacter && action.CardSource.Card.Definition.Body != null && action.CardSource.Card.Definition.Body.Count() > 0)
                            {
                                cardsource = cardsource + " (" + action.CardSource.Card.Definition.Body.FirstOrDefault() + ")";
                            }

                            if (action.CardSource.Card.Identifier == "UhYeahImThatGuy" && action.CardSource.AssociatedCardSources.Count() > 0)
                            {
                                // Find out which card I'm That Guy is copying.
                                cardsource += " (copying " + action.CardSource.AssociatedCardSources.Select(cs => cs.Card.Title).ToCommaList() + ")";
                            }
                        }
                        string output = "";
                        string punctuation = ".";
                        if (!preview.IsResultKnowable)
                        {
                            punctuation = " ?";
                        }

                        if (preview.IsActionRemovedAfterRedirect(action))
                        {
                            if (preview.IsActionConditionForRedirect(action))
                            {
                                punctuation = " (Greyed Out)";
                            }
                            else
                            {
                                punctuation = " (Removed)";
                            }
                        }

                        if (action is IncreaseDamageAction)
                        {
                            IncreaseDamageAction increase = (action as IncreaseDamageAction);
                            output = "+" + increase.AmountToIncrease;
                            if (increase.IsNemesisEffect)
                            {
                                cardsource = "Nemesis Effect";
                            }
                        }
                        else if (action is ReduceDamageAction)
                        {
                            output = "-" + (action as ReduceDamageAction).AmountToReduce;
                        }
                        else if (action is RedirectDamageAction)
                        {
                            RedirectDamageAction redirect = action as RedirectDamageAction;
                            string newTarget = "";
                            if (redirect.NewTarget == null)
                            {
                                newTarget = "???";
                            }
                            else
                            {
                                newTarget = redirect.NewTarget.Title;
                            }
                            output = "<-> " + newTarget;
                        }
                        else if (action is ImmuneToDamageAction)
                        {
                            output = "IMMUNE";
                        }
                        else if (action is MakeDamageIrreducibleAction)
                        {
                            output = "Make damage irreducible";
                        }
                        else if (action is MakeDamageUnincreasableAction)
                        {
                            output = "Make damage unincreasable";
                        }
                        else if (action is MakeDamageNotRedirectableAction)
                        {
                            output = "Make damage not redirectable";
                        }
                        else if (action is DestroyCardAction)
                        {
                            output = "Destroy Card";
                        }
                        else if (action is ChangeDamageTypeAction)
                        {
                            output = "Change type to " + (action as ChangeDamageTypeAction).DamageType;
                        }
                        else if (action is CancelAction)
                        {
                            CancelAction cancel = (action as CancelAction);
                            if (cancel.ActionToCancel is DealDamageAction)
                            {
                                output = "Damage cancelled";
                            }
                            else if (cancel.ActionToCancel is RedirectDamageAction
                                     || (cancel.ActionToCancel is MakeDecisionAction && (cancel.ActionToCancel as MakeDecisionAction).Decision.IsRedirectDecision))
                            {
                                output = "Redirection cancelled";
                            }
                        }
                        else if (action is GainHPAction)
                        {
                            GainHPAction gainHP = (action as GainHPAction);
                            output = gainHP.HpGainer.Title + " gains " + gainHP.Amount + " HP";
                        }
                        else if (action is MakeDecisionAction)
                        {
                            IDecision decision = (action as MakeDecisionAction).Decision;
                            if (decision is SelectDamageTypeDecision)
                            {
                                output = "Change type to " + (decision as SelectDamageTypeDecision).Choices.ToCommaList(false, true);
                            }
                            else if (decision is SelectFunctionDecision)
                            {
                                output = (decision as SelectFunctionDecision).Choices.Select(f => f.DisplayText).ToCommaList(false, true);
                            }
                            else if (decision is SelectCardDecision)
                            {
                                SelectCardDecision selectCardDecision = (decision as SelectCardDecision);
                                if (decision.SelectionType == SelectionType.AmbiguousDecision)
                                {
                                    cardsource = "Choice between ";
                                    if (selectCardDecision.SecondarySelectionType.HasValue)
                                    {
                                        if (selectCardDecision.SecondarySelectionType.Value == SelectionType.RedirectDamage)
                                        {
                                            cardsource = "Which redirects first";
                                        }
                                        else if (selectCardDecision.SecondarySelectionType.Value == SelectionType.DestroyCard)
                                        {
                                            cardsource = "Which one gets destroyed first";
                                        }
                                    }
                                }
                                if (decision.SelectionType == SelectionType.RedirectDamage)
                                {
                                    output = "Redirect to ";
                                }
                                output += (decision as SelectCardDecision).Choices.Select(c => c.Title).ToCommaList(false, true);
                            }
                            else if (decision is YesNoDecision)
                            {
                                output = "Yes or No";
                            }
                        }

                        Console.WriteLine(cardsource + ": " + output + punctuation);
                    }
                }
            }
        }

        protected void AssertIsAtMaxHP(TurnTakerController ttc)
        {
            AssertIsAtMaxHP(ttc.CharacterCard);
        }

        protected void AssertIsAtMaxHP(Card card)
        {
            Assert.AreEqual(card.MaximumHitPoints.Value, card.HitPoints.Value, "The HP of " + card.Title + " is not at maximum.");
        }

        protected void AssertAndStoreHP(TurnTakerController ttc, ref int characterHP, int change)
        {
            AssertAndStoreHP(ttc.CharacterCard, ref characterHP, change);
        }

        protected void AssertAndStoreHP(Card card, ref int cardHP, int change)
        {
            Assert.AreEqual(cardHP + change, card.HitPoints.Value, "The HP of " + card.Title + " is incorrect.");
            StoreHP(card, ref cardHP);
        }

        protected void AssertHitPoints(Card card, int expected)
        {
            if (card.HitPoints.HasValue)
            {
                var actual = card.HitPoints.Value;
                Assert.AreEqual(expected, actual, "The HP of {0} is incorrect.", card.Identifier);
            }
            else
            {
                Assert.Fail(card.Identifier + " is not a target!");
            }
        }

        protected void AssertHitPoints(TurnTakerController hero, int cardHP)
        {
            AssertHitPoints(hero.CharacterCard, cardHP);
        }

        protected void AssertMaximumHitPoints(TurnTakerController hero, int maxHP)
        {
            AssertMaximumHitPoints(hero.CharacterCard, maxHP);
        }

        protected void AssertMaximumHitPoints(Card card, int maxHP)
        {
            Assert.AreEqual(maxHP, card.MaximumHitPoints.Value, card.Title + "'s maximum hit points was expected to be " + maxHP + " but was " + card.MaximumHitPoints.Value + ".");
        }

        protected void QuickHPStorage(params TurnTakerController[] ttcs)
        {
            QuickHPStorage(ttcs.Select(ttc => ttc.CharacterCard).ToArray());
        }

        protected void QuickShuffleStorage(params TurnTakerController[] ttcs)
        {
            _quickShuffleStorage = new Dictionary<Location, int>();

            foreach (var ttc in ttcs)
            {
                _quickShuffleStorage.Add(ttc.TurnTaker.Deck, ttc.TurnTaker.Deck.ShuffleCount);
            }
        }

        protected void QuickShuffleStorage(params Location[] locations)
        {
            _quickShuffleStorage = new Dictionary<Location, int>();

            foreach (var location in locations)
            {
                _quickShuffleStorage.Add(location, location.ShuffleCount);
            }
        }

        protected void QuickShuffleCheck(params int?[] shuffleChanges)
        {
            Assert.AreEqual(_quickShuffleStorage.Count, shuffleChanges.Length, "QuickShuffleCheck passed {0} values but {1} are stored.", shuffleChanges.Length, _quickShuffleStorage.Count);

            for (int i = 0; i < shuffleChanges.Count(); i++)
            {
                var location = _quickShuffleStorage.Keys.ElementAt(i);
                var previousShuffle = _quickShuffleStorage[location];
                var changeShuffle = shuffleChanges.ElementAt(i);
                if (changeShuffle.HasValue)
                {
                    var expected = previousShuffle + changeShuffle.Value;
                    var actual = location.ShuffleCount;
                    Assert.AreEqual(expected, actual, "Expected " + location.GetFriendlyName() + "'s shuffle count to be " + expected + ", but it was " + actual + ".");
                    _quickShuffleStorage[location] = actual;
                }
            }
        }

        protected void QuickHPStorage(params Card[] cards)
        {
            _quickHPStorage = new Dictionary<Card, int>();
            Card previous = null;
            foreach (var card in cards)
            {
                //Log.Debug("Storing card HP: " + card.Identifier + ", " + card.HitPoints.Value);
                if (card != null)
                {
                    if (card.HitPoints.HasValue)
                    {
                        _quickHPStorage.Add(card, card.HitPoints.Value);
                    }
                    else
                    {
                        _quickHPStorage.Add(card, 0);
                    }
                    previous = card;
                }
                else if (previous != null)
                {
                    Assert.Fail("Card after " + previous.Title + " in QuickHPStorage is null.");
                }
                else
                {
                    Assert.Fail("First card in QuickHPStorage is null.");
                }
            }
        }

        protected void QuickHandStorage(params HeroTurnTakerController[] heroes)
        {
            _quickHandStorage = new Dictionary<HeroTurnTakerController, int>();
            foreach (var hero in heroes)
            {
                var count = hero.HeroTurnTaker.Hand.Cards.Count();
                Console.WriteLine("Storing cards in hand for {0}: {1}", hero.TurnTaker.Identifier, count);
                _quickHandStorage.Add(hero, count);
            }
        }

        protected void QuickTopCardStorage(params TurnTakerController[] ttcs)
        {
            _quickTopCardStorage = new Dictionary<TurnTakerController, Card>();
            foreach (var ttc in ttcs)
            {
                _quickTopCardStorage.Add(ttc, ttc.TurnTaker.Deck.TopCard);
            }
        }

        protected void QuickTopCardCheck(Func<TurnTakerController, Location> expectedLocations)
        {
            foreach (var pair in _quickTopCardStorage)
            {
                var ttc = pair.Key;
                var location = pair.Value.Location;

                Assert.AreEqual(expectedLocations(ttc), location, "Top card of " + ttc.Name + "'s deck was expected to be at " + expectedLocations(ttc) + " but was at " + location + ".");
            }
        }

        protected void QuickTokenPoolStorage(params TokenPool[] tokenPools)
        {
            _quickTokenPoolStorage = new Dictionary<TokenPool, int>();
            foreach (var pool in tokenPools)
            {
                _quickTokenPoolStorage.Add(pool, pool.CurrentValue);
            }
        }

        protected void QuickHPCheck(params int?[] hpChange)
        {
            Assert.AreEqual(_quickHPStorage.Count, hpChange.Length, "QuickHPCheck passed {0} values but {1} are stored.", hpChange.Length, _quickHPStorage.Count);

            for (int i = 0; i < hpChange.Count(); i++)
            {
                var card = _quickHPStorage.Keys.ElementAt(i);
                var previousHP = _quickHPStorage[card];
                var changeHP = hpChange.ElementAt(i);
                if (changeHP.HasValue)
                {
                    var expected = previousHP + changeHP.Value;
                    if (card.HitPoints.HasValue)
                    {
                        var actual = card.HitPoints.Value;
                        Assert.AreEqual(expected, actual, "Expected " + card.Title + "'s HP to be " + expected + ", but it was " + actual + ".");
                        _quickHPStorage[card] = actual;
                    }
                    else
                    {
                        Assert.Fail("QuickHPCheck: " + card.Title + " isn't a target!");
                    }
                }
                else if (card.IsTarget)
                {
                    AssertNotInPlay(card);
                }
            }
        }

        protected void QuickHPCheckZero()
        {
            var change = new int?[_quickHPStorage.Count];
            for (int i = 0; i < _quickHPStorage.Count; i++)
            {
                change[i] = 0;
            }

            QuickHPCheck(change);
        }

        protected void QuickHandCheck(params int[] cardNumberChange)
        {
            Assert.AreEqual(_quickHandStorage.Count, cardNumberChange.Length, "QuickHandCheck passed {0} values but {1} are stored.", cardNumberChange.Length, _quickHandStorage.Count);

            for (int i = 0; i < cardNumberChange.Count(); i++)
            {
                var hero = _quickHandStorage.Keys.ElementAt(i);
                var previousHandCount = _quickHandStorage[hero];
                var changeHand = cardNumberChange.ElementAt(i);

                var expected = previousHandCount + changeHand;
                var actual = Math.Max(hero.HeroTurnTaker.Hand.Cards.Count(), 0);
                Assert.AreEqual(expected, actual, "Expected " + hero.Name + " to have " + expected + " cards in hand, but they had " + actual + ".");
                _quickHandStorage[hero] = actual;
            }
        }

        protected void QuickHandCheckZero()
        {
            var change = new int[_quickHandStorage.Count];
            QuickHandCheck(change);
        }

        protected void QuickTokenPoolCheck(params int[] tokenChange)
        {
            Assert.AreEqual(_quickTokenPoolStorage.Count, tokenChange.Length, "QuickTokenPoolCheck passed {0} values but {1} are stored.", tokenChange.Length, _quickTokenPoolStorage.Count);

            for (int i = 0; i < tokenChange.Count(); i++)
            {
                var pool = _quickTokenPoolStorage.Keys.ElementAt(i);
                var previousCount = _quickTokenPoolStorage[pool];
                var change = tokenChange.ElementAt(i);

                var expected = previousCount + change;
                var actual = pool.CurrentValue;
                Assert.AreEqual(expected, actual, "Expected " + pool.Name + " to have " + expected + " tokens in it, but it had " + actual + ".");
                _quickTokenPoolStorage[pool] = actual;
                Console.WriteLine(pool.Name + ": " + pool.CurrentValue);
            }
        }

        /// <summary>
        /// If the HP of the targets has been changed manually, this method will update all the expected values for the current "quick HP" targets.
        /// </summary>
        protected void QuickHPUpdate()
        {
            QuickHPStorage(_quickHPStorage.Keys.ToArray());
        }

        protected void QuickHandUpdate()
        {
            QuickHandStorage(_quickHandStorage.Keys.ToArray());
        }

        protected void GainAndStoreHP(Card card, ref int cardHP, int change)
        {
            GainHP(card, change);
            StoreHP(card, ref cardHP);
        }

        protected void AssertIsInPlay(Card card)
        {
            Assert.IsTrue(card.IsInPlay, card.Title + " should be in play.");
        }

        protected void AssertRevealed(params Card[] cards)
        {
            cards.ForEach(c => AssertRevealed(c));
        }

        protected void AssertRevealed(Card card)
        {
            Assert.IsTrue(card.Location.IsRevealed, card.Title + " is not revealed.");
        }

        protected void AssertNoGameText(Card card)
        {
            Assert.IsFalse(card.HasGameText, card.Title + " should not have game text.");
        }

        protected void AssertPlayIndex(Card card, int? playIndex)
        {
            string expectedValue = playIndex.HasValue ? "" + playIndex.Value : "null";
            string actualValue = card.PlayIndex.HasValue ? "" + card.PlayIndex.Value : "null";

            Assert.AreEqual(playIndex, card.PlayIndex, "A play index of " + expectedValue + " was expected, but it was " + actualValue);
        }

        protected void AssertHasGameText(Card card)
        {
            Assert.IsTrue(card.HasGameText, card.Title + " should have game text.");
        }

        protected void AssertDoesNotHaveGameText(Card card)
        {
            Assert.IsTrue(!card.HasGameText, card.Title + " should not have game text.");
        }

        protected void AssertCardHasKeyword(Card card, string keyword, bool isAdditional)
        {
            Assert.IsTrue(this.GameController.DoesCardContainKeyword(card, keyword), "{0} should have keyword: {1}", card.Identifier, keyword);
            if (isAdditional)
            {
                Assert.IsTrue(this.GameController.GetAdditionalKeywords(card).Contains(keyword), "{0} should have additional keyword: {1}", card.Identifier, keyword);
            }
        }

        protected void AssertCardNoKeyword(Card card, string keyword)
        {
            Assert.IsFalse(this.GameController.DoesCardContainKeyword(card, keyword), "{0} should not have keyword: {1}", card.Identifier, keyword);
            Assert.IsFalse(this.GameController.GetAdditionalKeywords(card).Contains(keyword), "{0} should not have additional keyword: {1}", card.Identifier, keyword);
        }

        protected void AssertIsInPlay(params Card[] cards)
        {
            cards.ForEach(c => AssertIsInPlay(c));
        }

        protected void AssertIsInPlayAndNotUnderCard(Card card)
        {
            Assert.IsTrue(card.IsInPlayAndHasGameText, card.Title + " should be in play, but not under another card.");
        }

        protected void AssertIsInPlayAndNotUnderCard(string identifier, int minQuantity = 1, int maxQuantity = 1)
        {
            int count = FindCardsWhere(card => card.IsInPlay && card.Identifier == identifier).Count();
            Assert.IsTrue(count >= minQuantity && count <= maxQuantity, identifier + " is not in play.");
        }

        protected void AssertIsInPlay(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                AssertIsInPlay(card);
            }
        }

        protected void AssertIsInPlay(params string[] identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertIsInPlay(id);
            }
        }

        protected void AssertIsInPlay(string identifier, int minQuantity = 1, int maxQuantity = 100, string promoIdentifier = null)
        {
            var cards = this.GameController.FindCardsWhere(card => card.IsInPlay && card.PromoIdentifierOrIdentifier == identifier, false);
            int count = cards.Count();
            Assert.IsTrue(count >= minQuantity && count <= maxQuantity, identifier + " is not in play.");
        }

        protected void AssertNotInPlay(Card card)
        {
            Assert.IsFalse(card.IsInPlay, card.Title + " should not be in play, but it is.");
        }

        protected void AssertNotInPlay(params Card[] cards)
        {
            cards.ForEach(c => AssertNotInPlay(c));
        }

        protected void AssertNotInPlay(params string[] identifiers)
        {
            identifiers.ForEach(s => AssertNotInPlay(s));
        }

        protected void AssertNotInTrash(TurnTakerController ttc, string identifier)
        {
            Assert.IsFalse(ttc.TurnTaker.Trash.Cards.Any(c => c.Identifier == identifier), identifier + " should not be in the trash, but it is.");
        }

        protected void AssertNotInTrash(params Card[] cards)
        {
            foreach (var card in cards)
            {
                Assert.IsFalse(card.Location.IsTrash, card.Title + " was not supposed to be in the trash, but it was.");
            }
        }

        protected void AssertNotInPlay(IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertNotInPlay(id);
            }
        }

        protected void AssertNotInPlay(IEnumerable<Card> cards)
        {
            foreach (Card card in cards)
            {
                AssertNotInPlay(card);
            }
        }

        protected void AssertNotInPlay(Func<Card, bool> cardCriteria)
        {
            Assert.IsFalse(FindCardsWhere(card => card.IsInPlay).Any(cardCriteria), "Card should not be in play.");
        }

        protected void AssertNotInPlay(string identifier)
        {
            Assert.IsTrue(FindCardsWhere(card => card.IsInPlay && card.Identifier == identifier).Count() == 0, identifier + " should not be in play.");
        }

        protected void AssertOffToTheSide(string identifier)
        {
            Assert.IsTrue(FindCardsWhere(card => card.Location.IsOffToTheSide && card.Identifier == identifier).Count() > 0, "There are no cards with identifier " + identifier + " that are off to the side.");
        }

        protected void AssertNotOffToTheSide(string identifier)
        {
            Assert.IsTrue(FindCardsWhere(card => !card.Location.IsOffToTheSide && card.Identifier == identifier).Count() > 0, "There are cards with identifier " + identifier + " that are off to the side.");
        }

        protected void AssertNotOffToTheSide(TurnTakerController ttc, Func<Card, bool> cardCriteria)
        {
            Assert.IsFalse(ttc.TurnTaker.OffToTheSide.Cards.Any(cardCriteria), "There were cards in  " + ttc.Name + "'s off to the side location that shouldn't be there.");
        }

        protected void AssertNotInDeck(string identifier)
        {
            Assert.IsTrue(FindCardsWhere(card => card.Location.IsDeck && card.Identifier == identifier).Count() == 0, "There are cards with identifier " + identifier + " that are in a deck.");
        }

        protected void AssertOffToTheSide(Card card)
        {
            Assert.IsTrue(card.Location.IsOffToTheSide, card.Title + " is not off to the side.");
        }

        protected void AssertNotOffToTheSide(Card card)
        {
            Assert.IsTrue(!card.Location.IsOffToTheSide, card.Title + " is off to the side.");
        }

        protected void AssertOffToTheSide(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertOffToTheSide(c));
        }

        protected void AssertNotOffToTheSide(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertNotOffToTheSide(c));
        }

        protected void AssertInTrash(TurnTakerController ttc, Card card)
        {
            Assert.IsTrue(card.Location == ttc.TurnTaker.Trash, card.Title + " should be in " + ttc.Name + "'s trash, but it was in " + card.Location.GetFriendlyName());
        }
        protected void AssertInTrash(params Card[] cards)
        {
            cards.ForEach(c => AssertInTrash(c));
        }

        protected void AssertInTrash(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertInTrash(c));
        }

        protected void AssertInTrash(params string[] identifiers)
        {
            identifiers.ForEach(s => AssertInTrash(s));
        }

        protected void AssertInTrash(Card card)
        {
            AssertInTrash(this.GameController.FindTurnTakerController(card.Owner), card);
        }

        protected void AssertInTrash(string identifier)
        {
            var owner = GetCard(identifier).Owner;
            AssertInTrash(FindTurnTakerController(owner), identifier);
        }

        protected void AssertInTrash(TurnTakerController ttc, string identifier)
        {
            Assert.IsTrue(ttc.TurnTaker.Trash.Cards.Any(c => c.Identifier == identifier), identifier + " should be in " + ttc.Name + "'s trash.");
        }

        protected void AssertInTrash(TurnTakerController ttc, string[] identifiers)
        {
            foreach (string identifier in identifiers)
            {
                AssertInTrash(ttc, identifier);
            }
        }

        protected void AssertInTrash(TurnTakerController ttc, IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                AssertInTrash(ttc, card);
            }
        }

        protected void AssertInPlayArea(TurnTakerController ttc, Card card)
        {
            Assert.IsTrue(card.Location == ttc.TurnTaker.PlayArea, card.Title + " should be in " + ttc.Name + "'s Play Area, but was instead in " + card.Location.GetFriendlyName());
            Assert.IsTrue(ttc.TurnTaker.PlayArea.Cards.Contains(card), ttc.TurnTaker.PlayArea.GetFriendlyName() + " does not contain a reference to " + card.Title);
        }

        protected void AssertInPlayArea(TurnTakerController ttc, IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertInPlayArea(ttc, c));
        }

        protected void AssertNotInPlayArea(TurnTakerController ttc, Card card)
        {
            Assert.IsFalse(card.Location == ttc.TurnTaker.PlayArea, card.Title + " should not be in " + ttc.Name + "'s Play Area, but it is.");
            Assert.IsFalse(ttc.TurnTaker.PlayArea.Cards.Contains(card), ttc.TurnTaker.PlayArea.GetFriendlyName() + " contains a reference to " + card.Title);
        }

        protected void AssertPhaseActionCount(int? phaseActionCount, TurnPhase turnPhase = null)
        {
            if (turnPhase == null)
            {
                turnPhase = this.GameController.ActiveTurnPhase;
            }
            Assert.AreEqual(phaseActionCount, turnPhase.GetPhaseActionCount(), "Phase action count is incorrect");
        }

        protected void AssertAtLocation(Card card, Location location, bool onBottom = false)
        {
            Assert.AreEqual(location, card.Location, "Expected " + card.Identifier + " to be at " + location.GetFriendlyName() + ", but it is at " + card.Location.GetFriendlyName());
            if (onBottom)
            {
                Assert.AreEqual(location.BottomCard, card, "Expected " + card.Identifier + " to be at the bottom of " + location.GetFriendlyName() + ", but it is not.");
            }
        }

        protected void AssertAtLocation(Func<Card, bool> cardCriteria, Location location)
        {
            foreach (var card in FindCardsWhere(cardCriteria))
            {
                Assert.AreEqual(location, card.Location);
            }
        }

        protected void AssertAtLocation(IEnumerable<Card> cards, Location location)
        {
            foreach (var card in cards)
            {
                AssertAtLocation(card, location);
            }
        }

        protected void AssertUnderCard(Card mainCard, Card cardUnderCard)
        {
            AssertAtLocation(cardUnderCard, mainCard.UnderLocation);
        }


        protected void AssertNumberOfCardsInHand(HeroTurnTakerController httc, int number)
        {
            int actual = GetNumberOfCardsInHand(httc);
            number = Math.Max(number, 0);
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in their hand, but instead had {2}.", httc.Name, number, actual));
        }

        protected void AssertNumberOfCardsNextToCard(Card card, int number)
        {
            int actual = GetNumberOfCardsNextToCard(card);
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards next to it, but instead had {2}.", card.Title, number, actual));
        }

        protected void AssertNumberOfCardsInDeck(TurnTakerController ttc, int number)
        {
            int actual = ttc.TurnTaker.Deck.Cards.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in their deck, but instead had {2}.", ttc.Name, number, actual));
        }

        protected void AssertNumberOfCardsInTrash(TurnTakerController ttc, int number, Func<Card, bool> cardCriteria = null)
        {
            if (cardCriteria == null)
            {
                cardCriteria = c => true;
            }

            var trash = ttc.TurnTaker.Trash;
            int actual = trash.Cards.Where(cardCriteria).Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in their trash, but instead had {2}: {3}", ttc.Name, number, actual, trash.Cards.Select(c => c.Identifier).ToRecursiveString()));
        }

        protected void AssertNumberOfCardsAtLocation(Location location, int number, Func<Card, bool> cardCriteria = null)
        {
            if (cardCriteria == null)
            {
                cardCriteria = c => true;
            }
            int actual = location.Cards.Where(cardCriteria).Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards, but instead had {2}.", location.GetFriendlyName(), number, actual));
        }

        protected void AssertNumberOfCardsOutOfGame(TurnTakerController ttc, int number)
        {
            int actual = ttc.TurnTaker.OutOfGame.Cards.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in their out-of-game, but instead had {2}.", ttc.Name, number, actual));
        }

        protected void AssertNumberOfCardsInRevealed(TurnTakerController ttc, int number)
        {
            int actual = ttc.TurnTaker.Revealed.Cards.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in their revealed area, but instead had {2}.", ttc.Name, number, actual));
        }

        protected void AssertNumberOfCardsInPlay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay);
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }

        protected void AssertNumberOfCardsInPlay(Func<Card, bool> criteria, int number, bool includeUnderCard = false)
        {
            int actual = FindCardsWhere(c => c.IsInPlayAndHasGameText && criteria(c)).Count();
            if (includeUnderCard)
            {
                actual = FindCardsWhere(c => c.IsInPlay && criteria(c)).Count();
            }
            Assert.AreEqual(number, actual, String.Format("There are supposed to be {0} cards of matching criteria in play, but there are {1}.", number, actual));
        }

        protected void AssertNumberOfCardsInGame(Func<Card, bool> criteria, int number)
        {
            int actual = FindCardsWhere(c => criteria(c) && !c.IsOutOfGame).Count();
            Assert.AreEqual(number, actual, String.Format("There are supposed to be {0} cards of matching criteria in the game, but there are {1}.", number, actual));
        }

        protected void AssertNumberOfCardsInPlay(string identifier, int number)
        {
            AssertNumberOfCardsInPlay(c => c.Identifier == identifier, number);
        }

        protected void AssertNumberOfUsablePowers(Card card, int numberExpected)
        {
            int actual = this.GameController.GetUsablePowersThisTurn(this.GameController.FindCardController(card).HeroTurnTakerController).Where(p => p.CardController.Card == card).Count();
            Assert.AreEqual(numberExpected, actual, "There were " + actual + " usable powers this turn for " + card.Title + ".");
        }

        protected void AssertNumberOfUsablePowers(HeroTurnTakerController httc, int numberExpected)
        {
            var usablePowers = this.GameController.GetUsablePowersThisTurn(httc);
            int actual = usablePowers.Count();
            Assert.AreEqual(numberExpected, actual, "There were " + actual + " usable powers this turn for " + httc.Name + ".");
        }

        protected void AssertIncapacitated(HeroTurnTakerController httc)
        {
            Assert.IsTrue(httc.HeroTurnTaker.IsIncapacitatedOrOutOfGame, httc.Name + " should be incapacitated or out of game.");
        }

        protected void AssertIncapacitated(TurnTakerController ttc)
        {
            Assert.IsTrue(ttc.IsIncapacitatedOrOutOfGame, ttc.Name + " should be incapacitated or out of game.");
        }

        protected void AssertFlipped(TurnTakerController ttc)
        {
            Assert.IsTrue(ttc.CharacterCard.IsFlipped, ttc.Name + " should be flipped.");
        }

        protected void AssertFlipped(Card card)
        {
            Assert.IsTrue(card.IsFlipped, card.Title + " should be flipped.");
        }

        protected void AssertNotIncapacitatedOrOutOfGame(TurnTakerController ttc)
        {
            Assert.IsFalse(ttc.IsIncapacitatedOrOutOfGame, ttc.Name + " is not supposed to be considered incapacitated or out of game.");
        }

        protected void AssertFlipped(params Card[] cards)
        {
            cards.ForEach(c => AssertFlipped(c));
        }

        protected void AssertNotFlipped(TurnTakerController ttc)
        {
            Assert.IsFalse(ttc.CharacterCard.IsFlipped, ttc.Name + " should not be flipped.");
        }

        protected void AssertNotFlipped(Card card)
        {
            Assert.IsFalse(card.IsFlipped, card.Title + " should not be flipped.");
        }

        protected void AssertGameOver(EndingResult? expectedResult = null)
        {
            Assert.IsTrue(this.GameController.IsGameOver, "The game should be over.");
            if (expectedResult.HasValue)
            {
                if (this.GameController.GameOverEndingResult.HasValue)
                {
                    Assert.IsTrue(this.GameController.GameOverEndingResult.Value == expectedResult.Value, "Expected game over ending result " + expectedResult + ", but was " + this.GameController.GameOverEndingResult.Value);
                }
                else
                {
                    Assert.Fail("GameController didn't have a GameOverEndingResult!");
                }
            }
        }

        protected void AssertNotGameOver(string message = "The game should not be over.")
        {
            Assert.IsFalse(this.GameController.IsGameOver, message);
        }

        protected void AssertPretendGameOver(string message = "The game should pretend to be over.")
        {
            Assert.IsTrue(this.GameController.IsPretendGameOver, message);
        }

        protected void AssertTurnTakerNotInGame(string identifier)
        {
            Assert.IsFalse(this.GameController.AllTurnTakers.Any(tt => tt.Identifier == identifier));
        }

        protected void AssertTurnTakerInGame(string identifier)
        {
            Assert.IsTrue(this.GameController.AllTurnTakers.Any(tt => tt.Identifier == identifier));
        }

        protected void ForceGameOver(EndingResult result, string output)
        {
            RunCoroutine(this.GameController.GameOver(result, output));
        }

        protected void AssertNextDecisionChoices(IEnumerable<Card> included = null, IEnumerable<Card> notIncluded = null)
        {
            if (included != null)
            {
                _includedCardsInNextDecision = included;
            }

            if (notIncluded != null)
            {
                _notIncludedCardsInNextDecision = notIncluded;
            }
        }

        protected void AssertNextPowerDecisionChoices(IEnumerable<Card> included = null, IEnumerable<Card> notIncluded = null)
        {
            if (included != null)
            {
                _includedPowersInNextDecision = included;
            }

            if (notIncluded != null)
            {
                _notIncludedPowersInNextDecision = notIncluded;
            }
        }

        protected void AssertNextDecisionChoices(IEnumerable<LocationChoice> included = null, IEnumerable<LocationChoice> notIncluded = null)
        {
            if (included != null)
            {
                _includedLocationsInNextDecision = included;
            }

            if (notIncluded != null)
            {
                _notIncludedLocationsInNextDecision = notIncluded;
            }
        }

        protected void AssertNextDecisionChoices(IEnumerable<TurnTaker> included, IEnumerable<TurnTaker> notIncluded)
        {
            if (included != null)
            {
                _includedTurnTakersInNextDecision = included;
            }

            if (notIncluded != null)
            {
                _notIncludedTurnTakersInNextDecision = notIncluded;
            }
        }

        protected void AssertNextDecisionChoices(IEnumerable<TurnTakerController> included, IEnumerable<TurnTakerController> notIncluded)
        {
            IEnumerable<TurnTaker> includedTT = null;
            IEnumerable<TurnTaker> notIncludedTT = null;

            if (included != null)
            {
                includedTT = included.Select(ttc => ttc.TurnTaker);
            }

            if (notIncluded != null)
            {
                notIncludedTT = notIncluded.Select(ttc => ttc.TurnTaker);
            }

            AssertNextDecisionChoices(includedTT, notIncludedTT);
        }

        protected void AssertNumberOfChoicesInNextDecision(int number, SelectionType? selectionType = null)
        {
            _numberOfChoicesInNextDecision = number;
            _numberOfChoicesInNextDecisionSelectionType = selectionType;
        }

        protected void AssertInHand(HeroTurnTakerController hero, Card card)
        {
            Assert.IsTrue(hero.HeroTurnTaker.Hand.Cards.Contains(card), "Expected " + card.Title + " to be in " + hero.Name + "'s hand, but it was in " + card.Location.GetFriendlyName());
        }

        protected void AssertInHand(params Card[] cards)
        {
            cards.ForEach(c => AssertInHand(c));
        }

        protected void AssertInHand(Card card)
        {
            if (card.Owner != null && card.Owner.IsHero)
            {
                var hero = card.Owner.ToHero();
                Assert.IsTrue(hero.Hand.Cards.Contains(card), "Expected " + card.Title + " to be in " + hero.Name + "'s hand, but it was in " + card.Location.GetFriendlyName());
            }
            else
            {
                Assert.Fail("Asserting that " + card.Title + " is in a hero's hand, but it is not a hero card!");
            }
        }

        protected void AssertInHand(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertInHand(c));
        }

        protected void AssertInHand(HeroTurnTakerController hero, string[] identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertInHand(hero, id);
            }
        }

        protected void AssertInHand(HeroTurnTakerController hero, string identifier)
        {
            Assert.IsTrue(hero.HeroTurnTaker.Hand.Cards.Any(card => card.Identifier == identifier), identifier + " is not in " + hero.Name + "'s hand.");
        }

        protected void AssertInHand(string identifier)
        {
            var owner = GetCard(identifier).Owner;
            AssertInHand(FindTurnTakerController(owner).ToHero(), identifier);
        }

        protected void AssertInHand(params string[] identifiers)
        {
            identifiers.ForEach(s => AssertInHand(s));
        }

        protected void AssertNotInHand(HeroTurnTakerController hero, string identifier)
        {
            Assert.IsFalse(hero.HeroTurnTaker.Hand.Cards.Any(card => card.Identifier == identifier), identifier + " was found in " + hero.Name + "'s hand.");
        }

        protected void AssertNotInHand(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertNotInHand(c));
        }

        protected void AssertNotInHand(Card card)
        {
            Assert.IsFalse(card.Location.IsHand, card.Title + " is not supposed to be in " + card.Owner.Name + "'s hand!");
        }

        protected void AssertInDeck(TurnTakerController ttc, Card card)
        {
            Assert.IsTrue(ttc.TurnTaker.Deck.Cards.Contains(card), card.Title + " was supposed to be in " + ttc.Name + "'s deck, but was in " + card.Location.GetFriendlyName() + ".");
        }

        protected void AssertInDeck(Card card)
        {
            AssertInDeck(this.GameController.FindTurnTakerController(card.Owner), card);
        }

        protected void AssertInDeck(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertInDeck(c));
        }

        /// <summary>
        /// Returns the index of the position where the card is in the deck. returns null if it is not in the deck.
        /// </summary>
        /// <param name="card">Card.</param>
        protected int? FindIndexInDeck(Card card)
        {
            if (card.Location.IsDeck && card.Location.Cards.Contains(card))
            {
                return card.Location.Cards.Count() - card.Location.Cards.IndexOf(card) - 1;
            }
            return null;
        }

        protected void AssertInDeckOrHand(HeroTurnTakerController httc, Card card)
        {
            Assert.IsTrue(httc.TurnTaker.Deck.Cards.Contains(card) || httc.HeroTurnTaker.Hand.Cards.Contains(card), card.Title + " was supposed to be in " + httc.Name + "'s deck or hand, but was in " + card.Location.GetFriendlyName() + ".");
        }

        protected void AssertInDeck(TurnTakerController ttc, IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                AssertInDeck(ttc, card);
            }
        }

        protected void AssertOnTopOfDeck(TurnTakerController ttc, CardController card, int offset = 0)
        {
            AssertOnTopOfDeck(ttc, card.Card, offset);
        }

        protected void AssertOnTopOfDeck(TurnTakerController ttc, Card card, int offset = 0)
        {
            if (offset == 0)
            {
                Assert.IsTrue(ttc.TurnTaker.Deck.TopCard == card, "Expected " + card.Title + " to be on top of " + ttc.Name + "'s deck, but it was " + ttc.TurnTaker.Deck.TopCard.Title + ".");
            }
            else
            {
                Assert.IsTrue(ttc.TurnTaker.Deck.Cards.ElementAt(ttc.TurnTaker.Deck.Cards.Count() - 1 - offset) == card, "Expected " + card.Title + " to be offset " + offset + " on top of " + ttc.Name + "'s deck, but it was " + ttc.TurnTaker.Deck.TopCard.Title + ".");
            }
        }

        protected void AssertOnTopOfLocation(Card card, Location location, int offset = 0)
        {
            var actual = location.Cards.Reverse().Skip(offset).FirstOrDefault();
            Assert.AreSame(card, actual, "Expected {0} to be on top of {1}, but it was {2}.", card.Title, location.GetFriendlyName(), actual.Title);
        }

        protected void AssertOnBottomOfLocation(Card card, Location location, int offset = 0)
        {
            var actual = location.Cards.Skip(offset).FirstOrDefault();
            Assert.AreSame(card, actual, "Expected {0} to be on bottom of {1}, but it was {2}.", card.Title, location.GetFriendlyName(), actual.Title);
        }

        protected void AssertOnTopOfDeck(Card card, int offset = 0)
        {
            AssertOnTopOfDeck(this.GameController.FindTurnTakerController(card.Owner), card, offset);
        }

        protected void AssertOnTopOfDeck(params string[] identifiers)
        {
            identifiers.ForEach(s => AssertOnTopOfDeck(s));
        }

        protected void AssertOnTopOfDeck(string identifier)
        {
            var owner = GetCard(identifier).Owner;
            AssertOnTopOfDeck(FindTurnTakerController(owner), identifier);
        }

        protected void AssertOnTopOfDeck(TurnTakerController ttc, string identifier, int offset = 0)
        {
            Assert.AreEqual(identifier, ttc.TurnTaker.Deck.TopCard.Identifier, "Expected " + identifier + " to be on top of " + ttc.Name + "'s deck.");
        }

        protected void AssertOnBottomOfDeck(Card card, int offset = 0)
        {
            AssertOnBottomOfDeck(this.GameController.FindTurnTakerController(card.Owner), card, offset);
        }

        protected void AssertOnBottomOfDeck(params string[] identifiers)
        {
            identifiers.ForEach(s => AssertOnBottomOfDeck(s));
        }

        protected void AssertOnBottomOfDeck(string identifier)
        {
            var owner = GetCard(identifier).Owner;
            AssertOnBottomOfDeck(FindTurnTakerController(owner), identifier);
        }

        protected void AssertOnBottomOfTrash(TurnTakerController ttc, Card card)
        {
            Assert.IsTrue(ttc.TurnTaker.Trash.BottomCard == card, "Expected " + card.Identifier + " to be on bottom of " + ttc.Name + "'s trash.");
        }

        protected void AssertOnBottomOfDeck(TurnTakerController ttc, Card card, int offset = 0)
        {
            Assert.IsTrue(ttc.TurnTaker.Deck.GetBottomCards(offset + 1).LastOrDefault() == card, "Expected " + card.Identifier + " to be on bottom of " + ttc.Name + "'s deck.");
        }

        protected void AssertOnBottomOfDeck(TurnTakerController ttc, string identifier)
        {
            Assert.AreEqual(identifier, ttc.TurnTaker.Deck.BottomCard.Identifier, "Expected " + identifier + " to be on the bottom of " + ttc.Name + "'s deck.");
        }

        protected void AssertOnTopOfTrash(TurnTakerController ttc, Card card, int offset = 0)
        {
            if (offset == 0)
            {
                Assert.IsTrue(ttc.TurnTaker.Trash.TopCard == card);
            }
            else
            {
                Assert.IsTrue(ttc.TurnTaker.Trash.Cards.ElementAt(ttc.TurnTaker.Trash.Cards.Count() - 1 - offset) == card);
            }
        }

        protected void AssertCardsInPlayOrder(Card[] orderedCards)
        {
            Assert.AreEqual(orderedCards.Count(), this.GameController.Game.OrderedCardsInPlay.Count());
            for (int i = 0; i < orderedCards.Count(); i++)
            {
                Assert.AreEqual(this.GameController.Game.OrderedCardsInPlay.ElementAt(i), orderedCards[i]);
            }
        }

        protected void AssertCurrentTurnPhase(TurnTakerController ttc, Phase phase, bool isEphemeral = false)
        {
            AssertTurnPhaseDetails(this.GameController.ActiveTurnPhase, ttc, phase, isEphemeral);
        }

        protected void AssertTurnPhaseDetails(TurnPhase turnPhase, TurnTakerController ttc, Phase phase, bool isEphemeral = false)
        {
            var ephemeralString = turnPhase.EphemeralSource != null ? "ephemeral " : null;
            Assert.AreEqual(ttc.TurnTaker, turnPhase.TurnTaker, "Expected " + ttc.TurnTaker.Name + "'s " + phase + " phase, but was " + ephemeralString + turnPhase.TurnTaker.Name + "'s " + turnPhase.Phase + " phase.");
            Assert.AreEqual(phase, turnPhase.Phase, "Expected " + phase + ", but was " + turnPhase.Phase + ".");
            Assert.AreEqual(isEphemeral, turnPhase.IsEphemeral, ttc.TurnTaker.Name + "'s " + phase + " phase should be ephemeral.");
        }

        protected void AssertTurnPhaseList(int index, TurnTakerController ttc, Phase phase, bool isEphemeral = false)
        {
            AssertTurnPhaseDetails(turnPhaseList.ElementAt(index), ttc, phase, isEphemeral);
        }

        protected CardController GetMobileDefensePlatform()
        {
            return this.GameController.FindCardControllersWhere(c => c.Identifier == "MobileDefensePlatform" && c.IsInPlay).First();
        }

        protected void RemoveMobileDefensePlatform()
        {
            this.RunCoroutine(this.GameController.DestroyCard(null, GetMobileDefensePlatform().Card));
        }

        protected void RemoveInitialCitizens(TurnTakerController citizenDawn)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsCitizen && card.IsInPlay && card.Identifier != "CitizenDawnCharacter"), citizenDawn.TurnTaker.Deck, true);
        }

        protected void RemoveInitialManifestations(TurnTakerController infinitor)
        {
            MoveCards(infinitor, FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsManifestation), infinitor.TurnTaker.Deck, overrideIndestructible: true);
        }

        protected void RemoveInitialProjections(TurnTakerController dreamer)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsProjection && card.IsInPlay), dreamer.TurnTaker.Deck, true);
        }

        protected void RemoveInitialRelics(TurnTakerController skinWalker)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsRelic && card.IsInPlay), skinWalker.TurnTaker.Deck, true);
        }

        protected void RemoveInitialMinions(TurnTakerController voss)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsMinion && card.IsInPlay), voss.TurnTaker.Deck, true);
        }

        protected void RemoveInitialVictim(TurnTakerController spite)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsVictim && card.IsInPlay), spite.TurnTaker.Deck, true);
        }

        protected void RemoveInitialDevices(TurnTakerController ambuscade)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsDevice && card.IsInPlay), ambuscade.TurnTaker.Deck, true);
            if (ambuscade.CharacterCard.IsFlipped)
            {
                FlipCard(ambuscade.CharacterCardController);
            }
        }

        protected void RemoveInitialComponents(TurnTakerController cosmicOmnitron)
        {
            this.MoveCards(null, FindCardsWhere(card => card.IsComponent && card.IsInPlay), cosmicOmnitron.TurnTaker.Deck, true);
        }

        protected void RemoveInitialDiversion(TurnTakerController missInformation)
        {
            this.MoveCards(missInformation, FindCardsWhere(c => c.IsInPlay && c.IsDiversion), missInformation.TurnTaker.Deck, true);
        }

        protected void RemoveVillainTriggers()
        {
            foreach (var villain in this.GameController.FindVillainTurnTakerControllers(false))
            {
                if (!villain.HasMultipleCharacterCards)
                {
                    villain.CharacterCardController.RemoveAllTriggers(true, false, false);
                }
                else
                {
                    foreach (var character in villain.CharacterCards)
                    {
                        FindCardController(character).RemoveAllTriggers(true, false, false);
                    }
                }
            }
        }

        protected void RemoveCardTriggers(Card card, bool includingOutOfPlay = false)
        {
            this.GameController.RemoveTriggers(t => t.CardSource.Card == card, includingOutOfPlay);
        }

        protected void RemoveCardTriggers(params Card[] cards)
        {
            cards.ForEach(c => RemoveCardTriggers(c));
        }

        protected void RemoveCardTriggers(params TurnTakerController[] ttc)
        {
            ttc.ForEach(tt => RemoveCardTriggers(tt.CharacterCard));
        }

        protected void RemoveVillainCards()
        {
            DestroyCards(c => c.IsVillain && !c.IsCharacter);
        }

        protected void RemoveCardTriggers(IEnumerable<Card> cards)
        {
            cards.ForEach(c => RemoveCardTriggers(c));
        }

        protected void RemoveCardTriggers(Func<Card, bool> cardCriteria)
        {
            RemoveCardTriggers(FindCardsWhere(cardCriteria));
            RemoveOnDestroyTriggers(FindCardsWhere(cardCriteria));
        }

        protected void RemoveOnDestroyTriggers(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                var cc = this.GameController.FindCardController(card);
                cc.RemoveDestroyActions();
            }
        }

        protected void RemovePlagueLocus(TurnTakerController plagueRat)
        {
            this.MoveCards(null, FindCardsWhere(card => card.Identifier == "PlagueLocus"), plagueRat.TurnTaker.Deck, true);
        }

        protected void SetInitialEnnead(TurnTakerController ennead, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var initial = GetCard(id);
                if (!initial.IsInPlayAndHasGameText)
                {
                    PlayCard(initial);
                }
            }
            MoveCards(ennead, c => !identifiers.Contains(c.Identifier) && c.IsVillain && c.IsInPlayAndHasGameText && c.IsTarget, GetCard("TheShrineOfTheEnnead").UnderLocation);
        }

        protected Card FindCardInPlay(string identifier, int index = 0)
        {
            return FindCardsWhere(c => c.IsInPlay && c.Identifier == identifier).ElementAt(index);
        }


        protected Card FindCardInPlay(string identifier)
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == identifier).FirstOrDefault();
        }

        protected TurnTakerController FindVillain(string identifier = null)
        {
            TurnTakerController result = null;

            if (identifier != null)
            {
                result = this.GameController.FindTurnTakerController(identifier);
            }
            else
            {
                result = this.GameController.FindVillainTurnTakerControllers(false).FirstOrDefault();
            }

            Assert.IsNotNull(result, "FindVillain could not find {0} in the game.", identifier);

            return result;
        }

        protected TurnTakerController FindVillainTeamMember(string identifier)
        {
            TurnTakerController result = null;

            if (identifier.Contains("Team"))
            {
                result = this.GameController.FindTurnTakerController(identifier);
            }
            else
            {
                result = this.GameController.FindTurnTakerController(identifier + "Team");
            }

            Assert.IsNotNull(result, "FindVillainTeamMember could not find {0} in the game.", identifier);

            return result;
        }

        protected TurnTakerController FindEnvironment(BattleZone bz = null)
        {
            TurnTakerController result = this.GameController.FindEnvironmentTurnTakerController(bz);
            Assert.IsNotNull(result, "FindEnvironment could not find an environment in the game.");
            return result;
        }

        protected HeroTurnTakerController FindHero(string identifier = null, bool assertIfMissing = true)
        {
            HeroTurnTakerController result = null;

            if (identifier == null)
            {
                result = this.GameController.AllHeroControllers.FirstOrDefault().ToHero();
            }
            else
            {
                var turnTaker = this.GameController.FindTurnTakerController(identifier);
                if (turnTaker != null)
                {
                    result = turnTaker.ToHero();
                }
            }

            if (identifier != null && assertIfMissing)
            {
                Assert.IsNotNull(result, "FindHero could not find {0} in the game.", identifier);
            }

            return result;
        }

        protected TokenPool FindTokenPool(string cardIdentifier, string poolIdentifier)
        {
            var card = GetCard(cardIdentifier);
            return card.FindTokenPool(poolIdentifier);
        }

        protected void AssertRecordedDecisionAnswer(string decisionIdentifier, int? answerIndex, bool? skipped, bool? autodecided)
        {
            var journal = this.GameController.Game.Journal;
            var entries = journal.DecisionAnswerEntries(e => e.DecisionIdentifier == decisionIdentifier);
            if (entries.Count() == 0)
            {
                Assert.Fail("No entry for decision identifier " + decisionIdentifier + " found in journal!");
            }
            else if (entries.Count() > 1)
            {
                Assert.Fail("Multiple entries for decision identifier " + decisionIdentifier + " found in journal!");
            }
            else
            {
                var entry = entries.First();
                Assert.AreEqual(answerIndex, entry.AnswerIndex, "Answer index should be " + answerIndex ?? "null" + " but was " + entry.AnswerIndex ?? "null");
                Assert.AreEqual(skipped, entry.Skipped, "Skipped should be " + skipped ?? "null" + " but was " + entry.Skipped ?? "null");
                Assert.AreEqual(autodecided, entry.AutoDecided, "Auto decided should be " + autodecided ?? "null" + " but was " + entry.AutoDecided ?? "null");
            }
        }

        protected GameControllerDecisionEvent AssertNoDecision()
        {
            GameControllerDecisionEvent decider = decision =>
            {
                return this.MakeDecisions(decision, failAfter: "No decisions were expected to be present, but there was a decision:\n" + decision.ToStringForMultiplayerDebugging());
            };

            ReplaceOnMakeDecisions(decider);
            return decider;
        }

        protected GameControllerDecisionEvent AssertNoDecision(SelectionType selectionTypeThatShouldNotShowUp)
        {
            GameControllerDecisionEvent decider = decision =>
            {
                if (decision.SelectionType == selectionTypeThatShouldNotShowUp)
                {
                    Assert.Fail("No decision of selection type " + selectionTypeThatShouldNotShowUp + " was expected to be present, but there was a decision: " + decision.ToStringForMultiplayerDebugging());
                }

                return this.MakeDecisions(decision);
            };

            ReplaceOnMakeDecisions(decider);
            return decider;
        }

        protected GameControllerDecisionEvent AssertMaxNumberOfDecisions(int maxNumber)
        {
            int numberSoFar = 0;

            GameControllerDecisionEvent decider = decisions =>
            {
                numberSoFar++;
                Assert.LessOrEqual(numberSoFar, maxNumber, "There are more decisions than expected.");
                return this.MakeDecisions(decisions);
            };

            ReplaceOnMakeDecisions(decider);
            return decider;
        }

        protected void AssertCanPerformPhaseAction()
        {
            Assert.IsTrue(this.GameController.CanPerformPhaseAction(this.GameController.ActiveTurnPhase), "Should be able to perform action in phase {0}", this.GameController.ActiveTurnPhase);
        }

        protected void AssertCannotPerformPhaseAction()
        {
            Assert.IsFalse(this.GameController.CanPerformPhaseAction(this.GameController.ActiveTurnPhase), "Should not be able to perform action in phase {0}", this.GameController.ActiveTurnPhase);
        }

        protected void AssertCanPlayCards(TurnTakerController ttc)
        {
            Assert.IsTrue(this.GameController.CanPerformAction<PlayCardAction>(ttc, null), ttc.Name + " should be able to play cards.");
        }

        protected void AssertCannotPlayCards(TurnTakerController ttc)
        {
            Assert.IsFalse(this.GameController.CanPerformAction<PlayCardAction>(ttc, null), ttc.Name + " should not be able to play cards.");
            var keeper = ttc.TurnTaker.GetAllCards().Where(c => !c.IsInPlay && !c.IsCharacter && c.IsKeeper).FirstOrDefault();
            Console.WriteLine("Checking to make sure {0} cannot play cards by playing {1} from {2}", ttc.Name, keeper.Identifier, keeper.Location.GetFriendlyName());
            PlayCard(keeper);
            AssertNotInPlay(keeper);
        }

        protected void AssertCannotPlayCards(TurnTakerController ttc, Card testCard)
        {
            Assert.IsFalse(this.GameController.CanPerformAction<PlayCardAction>(ttc, null), ttc.Name + " should not be able to play cards.");
            Console.WriteLine("Checking to make sure {0} cannot play cards by playing {1} from {2}", ttc.Name, testCard.Identifier, testCard.Location.GetFriendlyName());
            PlayCard(testCard);
            AssertNotInPlay(testCard);
        }

        protected void AssertDamagePreviewResults(IEnumerable<DamagePreviewResult> results, int index, Card target, int amount, DamageType? damageType)
        {
            DamagePreviewResult result = results.ElementAt(index);
            Assert.AreEqual(target, result.Target, "Damage preview target is incorrect");
            Assert.AreEqual(amount, result.Amount, "Damage preview amount is incorrect");
            if (damageType.HasValue)
            {
                Assert.AreEqual(damageType.Value, result.DamageType, "Damage preview type is incorrect");
            }
            else
            {
                Assert.IsFalse(result.DamageType.HasValue, "Damage preview type expected to be unknown, but was defined.");
            }
        }

        protected void AssertDamagePreviewResultsNumberOfActions(IEnumerable<DamagePreviewResult> results, int index, int expectedNumberOfActions)
        {
            DamagePreviewResult result = results.ElementAt(index);
            Assert.AreEqual(expectedNumberOfActions, result.OrderedGameActions.Count(), "Damage preview number of actions are incorrect");
        }

        protected void AssertDamagePreviewNoDuplicates(IEnumerable<DamagePreviewResult> results, int index)
        {
            DamagePreviewResult result = results.ElementAt(index);
            for (int i = 0; i < result.OrderedGameActions.Count(); i++)
            {
                GameAction thisAction = result.OrderedGameActions.ElementAt(i);
                for (int j = 0; j < result.OrderedGameActions.Count(); j++)
                {
                    if (i != j)
                    {
                        GameAction otherAction = result.OrderedGameActions.ElementAt(j);
                        if (thisAction.CardSource.Card == otherAction.CardSource.Card && thisAction.GetType() == otherAction.GetType())
                        {
                            Assert.Fail("Duplicate game actions detected: " + thisAction.CardSource.Card);
                        }
                    }
                }
            }
        }

        protected void AssertDamagePreviewResultNotKnowable(IEnumerable<DamagePreviewResult> results, int index)
        {
            Assert.IsFalse(results.ElementAt(index).IsResultKnowable, "Preview result of this damage should not be knowable, but is marked as knowable.");
        }

        protected void AssertDamagePreviewResultKnowable(IEnumerable<DamagePreviewResult> results, int index)
        {
            Assert.IsTrue(results.ElementAt(index).IsResultKnowable, "Preview result of this damage should be knowable, but is marked as not knowable.");
        }

        protected void AssertDamagePreviewResultsOrderAffectsOutcome(IEnumerable<DamagePreviewResult> results, int index)
        {
            Assert.IsTrue(results.ElementAt(index).DoesOrderAffectOutcome);
        }

        public void AssertNumberOfStatusEffectsInPlay(int number)
        {
            Assert.AreEqual(number, this.GameController.StatusEffectControllers.Count());
        }

        public void PrintCannotPerformStringsAndAssertNumber(int number)
        {
            var strings = this.GameController.GetListOfActionsThatTurnTakersCannotPerformAsStrings();
            foreach (string s in strings)
            {
                Console.WriteLine(s);
            }
            Assert.AreEqual(number, strings.Count());
        }

        public void PrintSpecialStringsForCard(Card card)
        {
            Console.WriteLine("===== " + card.Title + " Special Strings =====");
            var strings = this.GameController.GetSpecialStringsForCard(card).Select(ss => ss.GeneratedString());
            foreach (string s in strings)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("==============================");
        }

        public void PrintCardsInPlayWithGameText(Func<Card, bool> cardCriteria = null)
        {
            Func<Card, bool> fullCriteria = c => c.IsInPlayAndHasGameText;
            if (cardCriteria != null)
            {
                fullCriteria = c => c.IsInPlayAndHasGameText && cardCriteria(c);
            }
            var cards = FindCardsWhere(fullCriteria);
            var result = "";
            foreach (var card in cards)
            {
                result += card.Title;
                if (card.IsFlipped)
                {
                    result += " (Flipped)";
                }
                result += ", ";
            }
            Console.WriteLine("***---> CARDS IN PLAY: " + result);
        }

        public void PrintTargetsInPlay()
        {
            foreach (var target in FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsTarget))
            {
                Console.WriteLine(target.Title + " (" + target.HitPoints.Value + " HP)");
            }
        }

        public void PrintSpecialStringsForEffectsListAndAssertNumber(int number)
        {
            var strings = this.GameController.GetSpecialStringsForEffectsList();
            foreach (SpecialString ss in strings)
            {
                Console.WriteLine(ss.GeneratedString());
            }
            Assert.AreEqual(number, strings.Count());
        }

        public void PrintUsablePowers(HeroTurnTakerController hero)
        {
            var powers = this.GameController.GetUsablePowersThisTurn(hero);

            Console.WriteLine("Powers usable by " + hero.Name + ":");
            foreach (var power in powers)
            {
                Console.WriteLine(power);
            }
        }

        public void AssertPowerIndexDescription(HeroTurnTakerController hero, Card card, int index, string description)
        {
            var usable = GetUsablePowersThisTurn(hero);
            var indexes = usable.Where(p => p.CardController.Card == card && p.Index == index);

            foreach (var powerIndex in indexes)
            {
                Assert.AreEqual(description, powerIndex.Description);
            }
        }

        public void PrintUsablePowers(HeroTurnTakerController hero, Card card)
        {
            var powers = this.GameController.GetUsablePowersThisTurn(hero).Where(p => p.CardController.Card == card);

            Console.WriteLine("Powers usable on " + card.Title + ":");
            foreach (var power in powers)
            {
                Console.WriteLine(power);
            }
        }

        protected void PrintTriggers(bool includingRemoved = true)
        {
            this.GameController.PrintTriggers(includingRemoved);
        }

        protected void PrintStatusEffectStrings()
        {
            Console.WriteLine("----Status Effects In Play----");
            foreach (var sec in this.GameController.StatusEffectControllers)
            {
                Console.WriteLine(sec.StatusEffect.ToString());
            }
            Console.WriteLine("---- End List ----");
        }

        protected void AssertStatusEffectsContains(string statusEffect)
        {
            var strings = this.GameController.StatusEffectControllers.Select(s => s.StatusEffect.ToString());
            Assert.IsTrue(strings.Any(ss => ss.Contains(statusEffect)), "Status Effects were expected to contain string \"" + statusEffect + "\".");
        }
        protected void AssertStatusEffectsDoesNotContain(string statusEffect)
        {
            var strings = this.GameController.StatusEffectControllers.Select(s => s.StatusEffect.ToString());
            Assert.IsFalse(strings.Any(ss => ss.Contains(statusEffect)), "Status Effects were not expected to contain string \"" + statusEffect + "\".");
        }

        protected void PrintHand(HeroTurnTakerController hero)
        {
            Console.WriteLine("\t" + hero.Name + "'s hand: [" + hero.HeroTurnTaker.Hand.Cards.Select(c => c.Title).ToCommaList() + "]");
        }

        protected void PrintJournal()
        {
            Console.WriteLine("----Journal----");
            Console.WriteLine(this.GameController.Game.Journal.Entries.ToRecursiveString("\n"));
            Console.WriteLine("----End Journal----");
        }

        protected void AssertEffectsListSpecialStringsContains(string specialString)
        {
            var strings = this.GameController.GetSpecialStringsForEffectsList().Select(ss => ss.GeneratedString());
            Assert.IsTrue(strings.Any(ss => ss.Contains(specialString)), "Effects list was expected to contain string \"" + specialString + "\".");
        }

        protected void AssertEffectsListSpecialStringsContains(Card card, int stringNumber, string specialString)
        {
            var strings = this.GameController.GetSpecialStringsForEffectsList();
            Assert.IsTrue(strings.Where(ss => ss.CardSource.Card == card).ElementAt(stringNumber).GeneratedString().Contains(specialString), "[" + card.Title + "]: Was expected to contain string \"" + specialString + "\" but string was: \"" + strings.Where(ss => ss.CardSource.Card == card).ElementAt(stringNumber).GeneratedString() + "\".");
        }

        protected void AssertEffectsListSpecialStringsContains(string identifier, int stringNumber, string specialString)
        {
            var output = this.GameController.GetSpecialStringsForEffectsList().Where(ss => ss.CardSource.Card.Identifier == identifier).ElementAt(stringNumber).GeneratedString();
            Assert.IsTrue(output.Contains(specialString), "[" + identifier + "]: Was expected to contain string \"" + specialString + "\" but string was: \"" + output + "\".");
        }

        protected void AssertCardSpecialString(Card card, int stringNumber, string specialString)
        {
            var specials = this.GameController.GetSpecialStringsForCard(card);
            var special = specials.ElementAtOrDefault(stringNumber);
            Assert.NotNull(special, "{0} does not have a special string at index {1}", card.Identifier, stringNumber);
            string output = special.GeneratedString();
            Console.WriteLine("Special String: " + output);
            Assert.AreEqual(specialString, output, "[" + card.Identifier + "]: Special string was expected to be: \"" + specialString + "\" but was \"" + output + "\".");
        }

        protected void AssertCardSpecialString(IEnumerable<Card> cards, int stringNumber, string specialString)
        {
            foreach (var card in cards)
            {
                AssertCardSpecialString(card, stringNumber, specialString);
            }
        }

        protected void AssertNumberOfCardSpecialStrings(Card card, int number)
        {
            var specials = this.GameController.GetSpecialStringsForCard(card);
            int actual = specials.Count();
            if (number != actual)
            {
                foreach (var special in specials)
                {
                    Console.WriteLine("Special String: " + special.GeneratedString());
                }
            }
            Assert.AreEqual(number, actual, "[" + card.Title + "]: Expected to have " + number + " special strings, but had " + actual + ".");
        }

        protected void AssertNumberOfCardsUnderCard(Card card, int number)
        {
            Assert.AreEqual(number, card.UnderLocation.Cards.Count());
        }

        protected IEnumerable<Card> GetCardsUnderCard(Card card)
        {
            return card.UnderLocation.Cards;
        }

        protected void AssertStatusEffectAssociatedTurnTaker(int statusNumber, TurnTaker associatedTurnTaker)
        {
            var actual = this.GameController.StatusEffectControllers.ElementAt(statusNumber).FindAssociatedTurnTaker(this.GameController);
            Assert.AreEqual(associatedTurnTaker, actual, "Status effect associated turn taker should be {0} but was {1}", associatedTurnTaker.Identifier, actual.Identifier);
        }

        protected GameControllerMessageEvent AssertNextMessage(string expectedMessage, GameControllerMessageEvent oldReceiver = null)
        {
            if (oldReceiver != null)
            {
                RemoveAssertNextMessage(oldReceiver);
            }
            GameControllerMessageEvent receiver = (message) =>
            {
                RunCoroutine(this.ReceiveMessage(message));
                Assert.AreEqual(expectedMessage, message.Message);
                _expectedMessageWasShown = true;
                return DoNothing();
            };

            this.GameController.OnSendMessage += receiver;
            _expectedMessageWasShown = false;
            return receiver;
        }

        protected GameControllerMessageEvent AssertNextMessageContains(string expectedMessage, GameControllerMessageEvent oldReceiver = null)
        {
            if (oldReceiver != null)
            {
                RemoveAssertNextMessage(oldReceiver);
            }
            GameControllerMessageEvent receiver = (message) =>
            {
                RunCoroutine(this.ReceiveMessage(message));
                Assert.IsTrue(message.Message.Contains(expectedMessage));
                _expectedMessageWasShown = true;
                return DoNothing();
            };

            this.GameController.OnSendMessage += receiver;
            _expectedMessageWasShown = false;
            return receiver;
        }

        protected GameControllerMessageEvent AssertNextMessages(string[] expectedMessages, GameControllerMessageEvent oldReceiver = null)
        {
            int index = 0;

            if (oldReceiver != null)
            {
                RemoveAssertNextMessage(oldReceiver);
            }
            GameControllerMessageEvent receiver = (message) =>
            {
                RunCoroutine(this.ReceiveMessage(message));
                var expected = expectedMessages.ElementAtOrDefault(index);
                if (expected != null)
                {
                    Assert.AreEqual(expected, message.Message);
                }
                else
                {
                    Assert.Fail("There are more messages than expected.");
                }

                index += 1;

                if (index == expectedMessages.Count())
                {
                    _expectedMessageWasShown = true;
                }

                return DoNothing();
            };

            this.GameController.OnSendMessage += receiver;
            return receiver;
        }

        protected GameControllerMessageEvent AssertNextMessages(params string[] expectedMessages)
        {
            return AssertNextMessages(expectedMessages, null);
        }

        protected GameControllerMessageEvent AssertNoMessage(GameControllerMessageEvent oldReceiver = null)
        {
            if (oldReceiver != null)
            {
                RemoveAssertNextMessage(oldReceiver);
            }
            GameControllerMessageEvent receiver = (message) =>
            {
                RunCoroutine(this.ReceiveMessage(message));
                Assert.Fail("No message was expected, but there was one: '" + message.Message + "'");
                return DoNothing();
            };

            this.GameController.OnSendMessage += receiver;
            return receiver;
        }

        protected void RemoveAssertNextMessage(GameControllerMessageEvent receiver)
        {
            this.GameController.OnSendMessage -= receiver;
        }

        protected void AssertPromoCardNotUnlocked(string identifier)
        {
            if (DeckDefinition.AvailablePromos.Contains(identifier) && this.GameController.IsPromoCardUnlocked(identifier))
            {
                Assert.Fail("Promo card '" + identifier + "' should not yet be unlocked.");
            }
        }

        protected void AssertPromoCardIsUnlockableThisGame(string identifier)
        {
            if (DeckDefinition.AvailablePromos.Contains(identifier) && !this.GameController.IsPromoCardUnlockableThisGame(identifier))
            {
                Assert.Fail("Promo card '" + identifier + "' is not unlockable this game.");
            }
        }

        protected void AssertPersistentValue<T>(string key, T value)
        {
            T actual = GetPersistentValueFromView<T>(key);
            Assert.AreEqual(value, actual, "Persistent value for " + key + " was expected to be " + value + " but was " + actual + ".");
        }

        protected void AssertPromoCardIsNotUnlockableThisGame(string identifier)
        {
            if (DeckDefinition.AvailablePromos.Contains(identifier) && this.GameController.IsPromoCardUnlockableThisGame(identifier))
            {
                Assert.Fail("Promo card '" + identifier + "' is unlockable this game.");
            }
        }

        protected void AssertPromoCardUnlocked(string identifier)
        {
            if (DeckDefinition.AvailablePromos.Contains(identifier) && !this.GameController.IsPromoCardUnlocked(identifier))
            {
                Assert.Fail("Promo card '" + identifier + "' should be unlocked.");
            }
        }

        protected void AssertNextToCard(Card card, Card cardThatItShouldBeNextTo)
        {
            Assert.AreEqual(cardThatItShouldBeNextTo.NextToLocation, card.Location);
        }

        protected void AssertBelowCard(Card card, Card cardThatItShouldBeBelow)
        {
            Assert.AreEqual(cardThatItShouldBeBelow.BelowLocation, card.Location, "{0} should be below {1} but it is not.", card.Identifier, cardThatItShouldBeBelow.Identifier);
        }

        protected void AssertNotBelowCard(Card card, Card cardThatItShouldNotBeBelow)
        {
            Assert.AreNotEqual(cardThatItShouldNotBeBelow.BelowLocation, card.Location);
        }

        protected void AssertNotNextToCard(Card card, Card cardThatItShouldBeNextTo)
        {
            Assert.AreNotEqual(card.Location, cardThatItShouldBeNextTo.NextToLocation);
        }

        protected void AssertUsablePower(HeroTurnTakerController hero, string identifier)
        {
            Assert.IsTrue(this.GameController.GetUsablePowersThisTurn(hero).Select(p => p.CardController.Card).Any(c => c.Identifier == identifier), "Power on " + identifier + " is not usable this turn.");
        }

        protected void AssertNotUsablePower(HeroTurnTakerController hero, string identifier)
        {
            Assert.IsFalse(this.GameController.GetUsablePowersThisTurn(hero).Select(p => p.CardController.Card).Any(c => c.Identifier == identifier), "Power on " + identifier + " is still usable this turn.");
        }

        protected void AssertUsablePower(HeroTurnTakerController hero, Card card, Card cardSource = null)
        {
            var usable = this.GameController.GetUsablePowersThisTurn(hero).Where(p => p.CardController.Card == card);
            if (cardSource != null)
            {
                usable = usable.Where(p => p.CardSource.Card == cardSource);
            }
            Assert.IsTrue(usable.Count() > 0, "Power on " + card.Title + " is not usable this turn.");
        }

        protected void AssertNotUsablePower(HeroTurnTakerController hero, Card card, Card cardSource = null)
        {
            var usable = this.GameController.GetUsablePowersThisTurn(hero).Where(p => p.CardController.Card == card);
            if (cardSource != null)
            {
                usable = usable.Where(p => p.CardSource.Card == cardSource);
            }
            Assert.IsFalse(usable.Count() > 0, "Power on " + card.Title + " is still usable this turn.");
        }

        protected void StackAfterShuffle(Location location, string[] identifiers)
        {
            var source = location.OwnerTurnTaker.CharacterCard;
            if (source == null)
            {
                source = FindVillain().CharacterCard;
            }
            var trigger = new Trigger<ShuffleCardsAction>(this.GameController, c => c.Location == location, StackCardsResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After, cardSource: new CardSource(this.GameController.FindCardController(source)));
            if (_stackAfterReshuffle.ContainsKey(location))
            {
                _stackAfterReshuffle[location] = identifiers;
            }
            else
            {
                _stackAfterReshuffle.Add(location, identifiers);
            }
            this.GameController.AddTrigger<ShuffleCardsAction>(trigger);
        }

        protected IEnumerable<Card> StackDeck(TurnTakerController ttc, string[] identifiers, bool toBottom = false)
        {
            MoveCards(ttc, identifiers, ttc.TurnTaker.Deck, toBottom);
            return ttc.TurnTaker.StackDeck(identifiers, toBottom);
        }

        protected void StackDeck(TurnTakerController ttc, IEnumerable<Card> cards, bool toBottom = false)
        {
            MoveCards(ttc, cards, c => c.NativeDeck, toBottom, overrideIndestructible: true);
        }

        protected void StackDeckAfterShuffle(TurnTakerController ttc, string[] identifiers, bool toBottom = false)
        {
            ITrigger trigger = null;
            Func<ShuffleCardsAction, IEnumerator> StackDeckAndRemoveTriggerResponse = action =>
            {
                this.StackDeck(ttc, identifiers, toBottom);
                this.GameController.RemoveTrigger(trigger);
                return DoNothing();
            };

            trigger = new Trigger<ShuffleCardsAction>(this.GameController, s => s.Location == ttc.TurnTaker.Deck, StackDeckAndRemoveTriggerResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After, new CardSource(ttc.CharacterCardController));
            this.GameController.AddTrigger(trigger);
        }

        protected void StackDeckAfterShuffle(TurnTakerController ttc, IEnumerable<Card> cards, bool toBottom = false)
        {
            ITrigger trigger = null;
            Func<ShuffleCardsAction, IEnumerator> StackDeckAndRemoveTriggerResponse = action =>
            {
                this.StackDeck(ttc, cards, toBottom);
                this.GameController.RemoveTrigger(trigger);
                return DoNothing();
            };

            trigger = new Trigger<ShuffleCardsAction>(this.GameController, s => s.Location == ttc.TurnTaker.Deck, StackDeckAndRemoveTriggerResponse, new TriggerType[] { TriggerType.MoveCard }, TriggerTiming.After, new CardSource(ttc.CharacterCardController));
            this.GameController.AddTrigger(trigger);
        }

        protected Card StackDeck(TurnTakerController ttc, string identifier, bool toBottom = false)
        {
            return StackDeck(ttc, new string[] { identifier }, toBottom).FirstOrDefault();
        }

        protected IEnumerable<Card> StackDeck(params string[] identifiers)
        {
            return identifiers.Select(s => StackDeck(s)).ToList();
        }

        protected Card StackDeck(string identifier, bool toBottom = false, int index = 0)
        {
            var card = GetCard(identifier, index);
            var ttc = this.GameController.FindTurnTakerController(card.Owner);
            StackDeck(ttc, new Card[] { card }, toBottom);
            return card;
        }

        protected void StackDeck(TurnTakerController ttc, Card card, bool toBottom = false)
        {
            MoveCard(ttc, card, ttc.TurnTaker.Deck, toBottom);
        }

        protected void StackDeck(Card card, bool toBottom = false)
        {
            MoveCard(FindTurnTakerController(card.Owner), card, card.NativeDeck, toBottom);
        }

        Dictionary<Location, string[]> _stackAfterReshuffle = new Dictionary<Location, string[]>();

        private IEnumerator StackCardsResponse(ShuffleCardsAction action)
        {
            if (_stackAfterReshuffle.ContainsKey(action.Location) && _stackAfterReshuffle[action.Location] != null)
            {
                if (action.Location.Name == LocationName.Deck)
                {
                    StackDeck(this.GameController.FindTurnTakerController(action.Location.OwnerTurnTaker), _stackAfterReshuffle[action.Location]);
                }
            }
            yield return null;
        }

        protected void AssertSourceDamageModified(IEnumerable<Card> cards, IEnumerable<int> modifications, Card testTarget)
        {
            Assert.AreEqual(cards.Count(), modifications.Count(), "AssertDamageModified: The number of cards provided and the number of expected results do not match up: " + cards.Count() + " and " + modifications.Count());
            for (int i = 0; i < cards.Count(); i++)
            {
                var card = cards.ElementAt(i);
                var modification = modifications.ElementAt(i);
                var targetHP = GetHitPoints(testTarget);
                DealDamage(card, testTarget, 3, DamageType.Radiant);
                int expectedDifference = 3 + modification;
                AssertAndStoreHP(testTarget, ref targetHP, -expectedDifference);
                GainHP(testTarget, expectedDifference);
            }
        }

        protected void AssertTargetDamageModified(IEnumerable<Card> targets, IEnumerable<int> modifications, Card testSource)
        {
            Assert.AreEqual(targets.Count(), modifications.Count(), "AssertDamageModified: The number of cards provided and the number of expected results do not match up: " + targets.Count() + " and " + modifications.Count());
            for (int i = 0; i < targets.Count(); i++)
            {
                var target = targets.ElementAt(i);
                var modification = modifications.ElementAt(i);
                var targetHP = GetHitPoints(target);
                DealDamage(testSource, target, 3, DamageType.Radiant);
                int expectedDifference = 3 + modification;
                AssertAndStoreHP(target, ref targetHP, -expectedDifference);
                GainHP(target, expectedDifference);
            }
        }

        protected void AssertHpAtStartOfTurn(TurnTakerController ttc, IEnumerable<Card> cards, IEnumerable<int?> hpChanges)
        {
            Assert.AreEqual(cards.Count(), hpChanges.Count(), "AssertHPAtEndOfTurn: The number of cards provided and the number of expected results do not match up: " + cards.Count() + " and " + hpChanges.Count());
            int index = this.GameController.TurnTakerControllers.IndexOf(ttc).Value;
            TurnTakerController previousTTC = null;
            if (index > 0)
            {
                previousTTC = this.GameController.TurnTakerControllers.ElementAt(index - 1);
            }
            else
            {
                previousTTC = this.GameController.TurnTakerControllers.Last();
            }
            var now = this.GameController.ActiveTurnPhase;
            if (now.TurnTaker != previousTTC.TurnTaker || now.Phase != Phase.End)
            {
                GoToEndOfTurn(previousTTC);
            }
            QuickHPStorage(cards.ToArray());
            GoToStartOfTurn(ttc);
            QuickHPCheck(hpChanges.ToArray());
        }

        protected void AssertHpAtEndOfTurn(TurnTakerController ttc, IEnumerable<Card> cards, IEnumerable<int?> hpChanges)
        {
            Assert.AreEqual(cards.Count(), hpChanges.Count(), "AssertHPAtEndOfTurn: The number of cards provided and the number of expected results do not match up: " + cards.Count() + " and " + hpChanges.Count());
            var now = this.GameController.ActiveTurnPhase;
            if (now.TurnTaker != ttc.TurnTaker || now.Phase != Phase.PlayCard)
            {
                GoToPlayCardPhase(ttc);
            }
            QuickHPStorage(cards.ToArray());
            GoToEndOfTurn(ttc);
            QuickHPCheck(hpChanges.ToArray());
        }

        #endregion

        protected void SaveGameToTemp(string name, bool quiet = false, GameController controller = null)
        {
            if (controller == null)
            {
                controller = this.GameController;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
            FileStream stream = null;
            try
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                var path = Path.Combine(Path.GetTempPath(), name.Trim() + ".dat");
                stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, controller.Game);

                if (!quiet)
                {
                    Console.WriteLine("Serialized game is " + stream.Length + " bytes");
                    Console.WriteLine("Successfully saved game to " + path);
                }
            }
            catch (SerializationException e)
            {
                Assert.Inconclusive("Failed to serialize. Reason: " + e.Message);

            }
            catch (IOException e)
            {
                Assert.Inconclusive("Failed to create file. Reason: " + e.Message);
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                if (stream != null)
                {
                    stream.Close();
                }
            }
        }

        public static Game LoadGamePath(string path)
        {
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                try
                {
                    AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                    Game game = (Game)formatter.Deserialize(stream);
                    return game;
                }
                catch (Exception ex)
                {
                    Assert.Inconclusive("Exception loading game at path: " + path);
                    Console.WriteLine(ex);

                    if (ex is SerializationException && ex.Message.Contains("Unexpected binary element: 9"))
                    {
                        Assert.Inconclusive("Failed to load game due to .NET version: {0}", ex.Message);
                    }

                    return null;
                }
                finally
                {
                    AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                    stream.Close();
                }
            }
            else
            {
                Assert.Fail("Data file not found: " + path);
                return null;
            }
        }

        protected GameController LoadGame(string name, bool addDataPath = false, bool addExtension = false, bool addTempPath = false)
        {
            try
            {
                Game game = null;

                name = name.Trim();

                if (addExtension)
                {
                    name += ".dat";
                }

                var path = name;

                if (addDataPath)
                {
                    var dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    path = Path.GetDirectoryName(dllpath.Replace("file://", "")).Replace("\\D:", "D:").Replace("\\C:", "C:");
                    //path = Path.Combine(path, "..", "..", "DataFiles", name);
                }
                else if (addTempPath)
                {
                    path = Path.Combine(Path.GetTempPath(), name.Trim());
                }

                game = LoadGamePath(path);

                if (game != null)
                {
                    this.GameController = SetupGameController(game);
                    RunCoroutine(this.GameController.StartGame(false));
                    Console.WriteLine("Successfully loaded game.");
                }
                else
                {
                    return null;
                }
            }
            catch (SerializationException e)
            {
                Assert.Inconclusive("Failed to load game. Reason: " + e.Message);
                throw;
            }

            return this.GameController;
        }

        protected GameController ReplayGame(string name)
        {
            try
            {
                name += ".dat";

                var dllpath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var path = Path.GetDirectoryName(dllpath.Replace("file://", "")).Replace("\\D:", "D:").Replace("\\C:", "C:");
                //path = Path.Combine(path, "..", "..", "DataFiles", name);
                var savedGame = LoadGamePath(path);

                if (savedGame != null)
                {
                    var newGame = MakeReplayableGame(savedGame);
                    SetupGameController(newGame);

                    Console.WriteLine("Successfully created game to replay...");

                    StartGame();
                    this.ReplayingGame = true;

                    // Keep moving the game forward until we have reached the stopping point.
                    int sanity = 1000;
                    while (this.ReplayingGame)
                    {
                        RunActiveTurnPhase();
                        EnterNextTurnPhase();
                        sanity--;

                        if (sanity == 0)
                        {
                            Log.Error("Save game never seemed to end: " + name);
                            this.ReplayingGame = false;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Failed to load and replay game.");
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine("Failed to load and replay game. Reason: " + e.Message);
                throw;
            }

            return this.GameController;
        }

        /// <summary>
        /// Creates a new game object from an existing one, copying its decision answers so it can be replayed.
        /// </summary>
        /// <param name="game">Game.</param>
        private Game MakeReplayableGame(Game existingGame)
        {
            // Get the information from the copied game, but not the state of it.
            var turnTakerIds = existingGame.TurnTakers.Select(tt => tt.Identifier);
            var isAdvanced = existingGame.IsAdvanced;
            var promoIds = new Dictionary<string, string>();
            foreach (var ttWithPromo in existingGame.TurnTakers.Where(tt => tt.PromoIdentifier != null))
            {
                promoIds.Add(ttWithPromo.Identifier, ttWithPromo.PromoIdentifier);
            }
            var randomSeed = existingGame.RandomSeed.Value;
            var isMultiplayer = existingGame.IsMultiplayer;
            var randomizer = existingGame.InitialRNG;

            var game = new Game(turnTakerIds, isAdvanced, promoIds, randomSeed, isMultiplayer, randomizer);
            this.ReplayDecisionAnswers = existingGame.Journal.DecisionAnswerEntries(e => true).ToList();
            Console.WriteLine("# of saved replay decision answers: " + this.ReplayDecisionAnswers.Count());

            return game;
        }

        private static System.Reflection.Assembly OnAssemblyResolve(System.Object sender, System.ResolveEventArgs reArgs)
        {
            Console.WriteLine("OnAssemblyResolve: " + reArgs.Name);
            foreach (System.Reflection.Assembly assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                System.Reflection.AssemblyName assemblyName = assembly.GetName();
                if (reArgs.Name.StartsWith(assemblyName.Name))
                {
                    Console.WriteLine("Returning " + assembly);
                    return (assembly);
                }
            }

            return null;
        }

        protected void AssertNoSpecialStrings(Card card)
        {
            AssertNumberOfCardSpecialStrings(card, 0);
        }

        protected void AssertNoSpecialStrings(IEnumerable<Card> cards)
        {
            foreach (var card in cards)
            {
                AssertNoSpecialStrings(card);
            }
        }

        protected void ActivateAbility(string key, Card card)
        {
            var cc = this.GameController.FindCardController(card);
            var ability = new ActivatableAbility(this.GameController.FindTurnTakerController(card.Owner), cc, key, cc.Card.GetActivatableAbilityDescription(key), cc.ActivateAbility(key), 0, null, null, new CardSource(cc));
            this.RunCoroutine(this.GameController.ActivateAbility(ability, new CardSource(cc)));
        }

        protected void AssertNotTargets(Func<Card, bool> cardCriteria)
        {
            var cards = FindCardsWhere(cardCriteria).ToList();
            foreach (var c in cards)
            {
                AssertNotTarget(c);
            }
        }

        protected void AssertNotTarget(Card card)
        {
            Assert.IsFalse(card.IsTarget, card.Title + " should not be a target.");
        }

        protected void AssertAreTargets(Func<Card, bool> cardCriteria)
        {
            var cards = FindCardsWhere(cardCriteria).ToList();
            foreach (var c in cards)
            {
                AssertIsTarget(c);
            }
        }

        protected void AssertIsTarget(Card card, int? maxHitPoints = null)
        {
            Assert.IsTrue(card.IsTarget, card.Title + " should be a target.");
            if (maxHitPoints.HasValue)
            {
                Assert.AreEqual(maxHitPoints.Value, card.HitPoints.Value);
            }
        }

        protected void AssertIsTarget(TurnTakerController ttc)
        {
            AssertIsTarget(ttc.CharacterCard);
        }

        protected CardController GetCardController(Card card)
        {
            return this.GameController.FindCardController(card);
        }

        protected CardController GetCardController(string identifier)
        {
            return this.GameController.FindCardController(GetCard(identifier));
        }

        protected void ShuffleTrashIntoDeck(TurnTakerController ttc, Location location = null)
        {
            RunCoroutine(this.GameController.ShuffleTrashIntoDeck(ttc, overrideDeck: location));
        }

        private IEnumerator WillPerformAction(GameAction gameAction)
        {
            // The test should fail if a DealDamageAction is about to occur before a previous one has resolved.
            var running = this.GameController.UnresolvedActions.Where(ga => ga is DealDamageAction).FirstOrDefault();
            if (gameAction is MakeDecisionAction && this.GameController.PretendMode && this.GameController.PeekFastCoroutines())
            {
                Assert.Fail("Tried to make a decision with FastCoroutines in Pretend mode");
            }

            if (running != null && !(running as DealDamageAction).IsResolved && gameAction is DealDamageAction && running != gameAction)
            {
                Log.Warning("DealDamageAction A tried to be performed before DealDamageAction B was finished:\nA: " + gameAction + "\nB: " + running);
            }

            yield return null;
        }

        private IEnumerator WillApplyActionChanges(GameAction gameAction)
        {
            if (gameAction is PhaseChangeAction && turnPhaseList != null)
            {
                turnPhaseList.Add((gameAction as PhaseChangeAction).ToPhase);
            }

            yield return null;
        }

        private IEnumerator DidPerformAction(GameAction gameAction)
        {
            if (this.GameController.FindCardsWhere(c => c.Identifier == "UhYeahImThatGuy" && c.IsInPlayAndHasGameText).FirstOrDefault() != null)
            {
                // When I'm That Guy is in play, it should have a Destroy Trigger.
                // AssertTriggersWhere(t => t.CardSource != null && t.CardSource.Card.Identifier == "UhYeahImThatGuy");
            }

            if (gameAction is DealDamageAction && _notDamageSource != null)
            {
                Assert.AreNotEqual(_notDamageSource, (gameAction as DealDamageAction).DamageSource, _notDamageSource.Title + " was not expected to be a damage source.");
            }

            if (_decisionSourceCriteria != null)
            {
                if (gameAction.DecisionSources != null)
                {
                    var match = gameAction.DecisionSources.Where(_decisionSourceCriteria).FirstOrDefault();
                    Assert.IsNotNull(match, "There were no decision sources matching the expected criteria.");
                    _decisionSourceCriteria = null;
                }
            }

            if (this.ShowActionDecisionSources)
            {
                if (gameAction.DecisionSources != null && gameAction.DecisionSources.Count() > 0)
                {
                    if (gameAction is MoveCardAction)
                    {
                        // We need to show a message to the other players so they know what's going on.
                        var decision = gameAction.DecisionSources.Where(d => (d is SelectCardDecision || d is YesNoCardDecision || d is MoveCardDecision) && d.HeroTurnTakerController != null).LastOrDefault();
                        var action = gameAction as MoveCardAction;

                        string output = null;

                        if (decision != null)
                        {
                            HeroTurnTaker hero = decision.HeroTurnTakerController.HeroTurnTaker;
                            string heroID = hero.Identifier;
                            var who = hero == action.Destination.OwnerTurnTaker ? "their" : action.Destination.OwnerTurnTaker.Name + "'s";

                            if (action.Origin.IsRevealed || action.Origin.IsTrash)
                            {
                                string from = null;
                                if (action.Origin.IsRevealed)
                                {
                                    from = hero.Name + " moves revealed card " + action.CardToMove.Title;
                                }
                                else if (action.Origin.IsTrash)
                                {
                                    from = hero.Name + " moves " + action.CardToMove.Title + " from " + who + " trash";
                                }

                                if (from != null)
                                {
                                    if (action.Destination.IsHand)
                                    {
                                        output = from + " into " + who + " hand.";
                                    }
                                    else if (action.Destination.IsTrash)
                                    {
                                        output = from + " into " + who + " trash.";
                                    }
                                    else if (action.Destination.IsDeck)
                                    {
                                        if (!action.ToBottom)
                                        {
                                            output = from + " on top of " + who + " deck.";
                                        }
                                        else
                                        {
                                            output = from + " on the bottom of " + who + " deck.";
                                        }
                                    }
                                    else if (action.Destination.IsNextToCard)
                                    {
                                        output = from + " next to " + action.Destination.OwnerCard.Title + ".";
                                    }
                                }
                            }
                            else if ((action.Origin.IsHand || action.Origin.IsDeck) && action.Destination.IsTrash)
                            {
                                output = hero.Name + " discards " + action.CardToMove.Title + ".";
                            }
                            else if (action.Destination.IsNextToCard && action.Destination.OwnerCard != null)
                            {
                                output = hero.Name + " moves " + action.CardToMove.Title + " next to " + action.Destination.OwnerCard.Title + ".";
                            }
                            else if (action.Destination.IsUnderCard && action.Destination.OwnerCard != null)
                            {
                                output = hero.Name + " moves " + action.CardToMove.Title + " under " + action.Destination.OwnerCard.Title + ".";
                            }

                            if (output != null)
                            {
                                Console.WriteLine("---DECISION SOURCE: " + output);

                                if (_expectedDecisionSourceOutput != null)
                                {
                                    Assert.AreEqual(_expectedDecisionSourceOutput, output, "Decision source output was expected to be: " + _expectedDecisionSourceOutput + ", but was " + output + ".");
                                    _expectedDecisionSourceOutput = null;
                                }
                            }
                        }
                        else
                        {
                            Log.Warning("Decision sources were null or of an unsupported type.");
                        }
                    }
                    else
                    {
                        foreach (IDecision d in gameAction.DecisionSources)
                        {
                            if (d.HeroTurnTakerController != null)
                            {
                                Console.WriteLine("---DECISION SOURCE: " + gameAction + " decision maker was: " + d.HeroTurnTakerController.Name);
                            }
                        }
                    }
                }

                if (gameAction is MakeDecisionAction)
                {
                    var makeDecision = gameAction as MakeDecisionAction;
                    if (makeDecision.Decision is YesNoCardDecision)
                    {
                        var yesNo = makeDecision.Decision as YesNoCardDecision;
                        if (yesNo.SelectionType == SelectionType.DiscardCard && yesNo.Answer.HasValue && yesNo.Answer.Value == false)
                        {
                            Console.WriteLine("---DECISION: " + yesNo.HeroTurnTakerController.Name + " chose not to discard " + yesNo.Card.Title + ".");
                        }
                    }
                }
            }

            if (gameAction is GameOverAction && gameAction.IsSuccessful)
            {
                Console.WriteLine("GAME OVER");
                this._continueRunningGame = false;
            }

            yield return null;
        }

        public void AssertPromoCardNotUnlockableInThisGame(IEnumerable<string> setup, string promoIdentifier, Dictionary<string, string> promosInGame = null, bool isAdvanced = false)
        {
            if (DeckDefinition.AvailablePromos.Contains(promoIdentifier))
            {
                SetupGameController(setup, promoIdentifiers: promosInGame, advanced: isAdvanced);
                MakePromoCardNotUnlocked(promoIdentifier);

                StartGame();

                AssertPromoCardNotUnlocked(promoIdentifier);
                AssertPromoCardIsNotUnlockableThisGame(promoIdentifier);
            }
        }

        public void AssertPromoCardNotUnlockableInTheseGames(IEnumerable<IEnumerable<string>> setups, string promoIdentifier)
        {
            setups.ForEach(s => AssertPromoCardNotUnlockableInThisGame(s, promoIdentifier));
        }

        public void AssertPromoCardUnlockableInThisGame(IEnumerable<string> setup, string promoIdentifier)
        {
            if (DeckDefinition.AvailablePromos.Contains(promoIdentifier))
            {
                SetupGameController(setup);
                MakePromoCardNotUnlocked(promoIdentifier);

                StartGame();

                AssertPromoCardNotUnlocked(promoIdentifier);
                AssertPromoCardIsUnlockableThisGame(promoIdentifier);
            }
        }

        public void AssertPromoCardUnlockableInTheseGames(IEnumerable<IEnumerable<string>> setup, string promoIdentifier)
        {
            setup.ForEach(s => AssertPromoCardUnlockableInThisGame(s, promoIdentifier));
        }

        public Card FindCard(Func<Card, bool> cardCriteria)
        {
            return FindCardsWhere(cardCriteria).FirstOrDefault();
        }

        public void PutOnDeck(TurnTakerController ttc, CardController card, bool toBottom = false)
        {
            PutOnDeck(ttc, card.Card, toBottom);
        }

        public Card PutOnDeck(TurnTakerController ttc, Card card, bool toBottom = false)
        {
            return MoveCard(ttc, card, ttc.TurnTaker.Deck, toBottom);
        }

        public void PutOnDeck(TurnTakerController ttc, IEnumerable<Card> cards)
        {
            cards.ForEach(c => MoveCard(ttc, c, ttc.TurnTaker.Deck));
        }

        public Card PutOnDeck(string identifier, bool toBottom = false)
        {
            var card = GetCard(identifier);
            PutOnDeck(GetTurnTakerController(card), card, toBottom);
            return card;
        }

        public void MakePromoCardNotUnlocked(string promoIdentifier, IEnumerable<string> undefeatedVillains = null, IEnumerable<string> defeatedVillains = null)
        {
            SetPersistentValueInView(promoIdentifier + GameController.HasPromoCardBeenUnlockedString, false);

            if (undefeatedVillains != null)
            {
                foreach (var villain in undefeatedVillains)
                {
                    SetVillainHasBeenDefeated(villain, false);
                }
            }

            if (defeatedVillains != null)
            {
                foreach (var villain in defeatedVillains)
                {
                    SetVillainHasBeenDefeated(villain, true);
                }
            }
        }

        protected void AddTokensToPool(TokenPool pool, int numberOfTokens, CardSource cardSource = null)
        {
            RunCoroutine(this.GameController.AddTokensToPool(pool, numberOfTokens, cardSource));
        }

        protected void RemoveTokensFromPool(TokenPool pool, int numberOfTokens, CardSource cardSource = null)
        {
            RunCoroutine(this.GameController.RemoveTokensFromPool(pool, numberOfTokens, null, false, null, cardSource));
        }

        protected CardController FindCardController(Card card)
        {
            return this.GameController.FindCardController(card);
        }

        protected CardController FindCardController(string identifier)
        {
            return this.GameController.FindCardController(GetCard(identifier));
        }

        protected void SetVillainHasBeenDefeated(string identifier, bool defeated)
        {
            SetPersistentValueInView(identifier + GameController.HasVillainBeenDefeatedString, defeated);
        }

        public void SetAllTargetsToMaxHP()
        {
            var targets = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsTarget);
            targets.ForEach(c => SetHitPoints(c, c.MaximumHitPoints.Value));
        }

        protected IEnumerable<Card> FindCardsWhere(Func<Card, bool> cardCriteria, bool realCardsOnly = true)
        {
            return this.GameController.FindCardsWhere(cardCriteria, realCardsOnly);
        }

        protected void SelectCardsForNextDecision(params Card[] cards)
        {
            this.DecisionSelectCards = cards;
            this.DecisionSelectCardsIndex = 0;
        }

        protected void SelectTurnTakersForNextDecision(params TurnTaker[] tts)
        {
            this.DecisionSelectTurnTakers = tts;
            this.DecisionSelectTurnTakersIndex = 0;
        }

        protected void SelectTurnTakerControllersForNextDecision(params TurnTakerController[] ttcs)
        {
            var tts = ttcs.Select(ttc => ttc == null ? null : ttc.TurnTaker);
            SelectTurnTakersForNextDecision(tts.ToArray());
        }

        protected void ResetTurnTakersForNextDecision()
        {
            this.DecisionSelectTurnTakers = null;
            this.DecisionSelectTurnTakersIndex = 0;
        }

        protected void ResetYesNoForNextDecision()
        {
            this.DecisionsYesNo = null;
            this.DecisionsYesNoIndex = 0;
        }

        protected void SelectYesNoForNextDecision(params bool[] yesNo)
        {
            this.DecisionsYesNo = yesNo;
            this.DecisionsYesNoIndex = 0;
        }

        protected void SelectTurnPhaseForNextDecision(TurnPhase tp)
        {
            this.DecisionSelectTurnPhase = tp;
        }

        protected void SelectTurnPhaseForNextDecision(TurnTakerController ttc, Phase p)
        {
            this.DecisionSelectTurnPhase = ttc.TurnTaker.TurnPhases.Where(tp => tp.Phase == p).FirstOrDefault();
        }

        protected void SelectFromBoxForNextDecision(string identifier, string turnTakerIdentifier)
        {
            this.DecisionSelectFromBoxIdentifiers = new string[] { identifier };
            this.DecisionSelectFromBoxTurnTakerIdentifier = turnTakerIdentifier;
            this.DecisionSelectFromBoxIndex = 0;
        }

        protected void SelectFromBoxForNextDecision(string[] identifiers, string turnTakerIdentifier)
        {
            this.DecisionSelectFromBoxIdentifiers = identifiers;
            this.DecisionSelectFromBoxTurnTakerIdentifier = turnTakerIdentifier;
            this.DecisionSelectFromBoxIndex = 0;
        }

        protected GameControllerDecisionEvent AssertNextDecisionSelectionType(SelectionType type)
        {
            this.DecisionNextSelectionType = type;

            GameControllerDecisionEvent decider = decision =>
            {
                if (this.DecisionNextSelectionType.HasValue)
                {
                    Assert.AreEqual(this.DecisionNextSelectionType, decision.SelectionType,
                        "The next decision type was expected to be " + type + " but was " + decision.SelectionType);
                    this.DecisionNextSelectionType = null;
                }

                return this.MakeDecisions(decision);
            };

            ReplaceOnMakeDecisions(decider);
            return decider;
        }

        protected GameControllerDecisionEvent AssertNextDecisionMaker(HeroTurnTakerController decisionMaker, bool removeExpectationAfterFirstDecision = true)
        {
            bool expected = true;

            GameControllerDecisionEvent decider = decision =>
            {
                if (expected)
                {
                    var expectedMaker = decisionMaker != null ? decisionMaker.Name : "a communal vote";
                    var actualMaker = decision.HeroTurnTakerController != null ? decision.HeroTurnTakerController.Name : "a communal vote";
                    Assert.AreEqual(decisionMaker, decision.HeroTurnTakerController, "The next decision maker was expected to be " + expectedMaker + " but was " + actualMaker + ".");
                    if (removeExpectationAfterFirstDecision)
                    {
                        expected = false;
                    }
                }
                return this.MakeDecisions(decision);
            };

            ReplaceOnMakeDecisions(decider);
            return decider;
        }

        protected void AssertOutOfGame(Card card)
        {
            Assert.IsTrue(card.Location.Name == LocationName.OutOfGame, card.Title + " was not out of game.");
        }

        protected void AssertOutOfGame(params Card[] cards)
        {
            cards.ForEach(c => AssertOutOfGame(c));
        }

        protected void AssertNotOutOfGame(Card card)
        {
            Assert.IsTrue(card.Location.Name != LocationName.OutOfGame, card.Title + " was out of game.");
        }

        protected void AssertOutOfGame(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertOutOfGame(c));
        }

        protected void AssertInTheBox(Card card)
        {
            Assert.IsTrue(card.Location.Name == LocationName.InTheBox, card.Title + " was not in the box.");
        }

        protected void AssertInTheBox(params Card[] cards)
        {
            cards.ForEach(c => AssertInTheBox(c));
        }

        protected void AssertNotInTheBox(Card card)
        {
            Assert.IsTrue(card.Location.Name != LocationName.InTheBox, card.Title + " was in the box.");
        }

        protected void AssertInTheBox(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertInTheBox(c));
        }

        protected void AssertNoTriggersWhere(Func<ITrigger, bool> criteria)
        {
            var triggers = this.GameController.FindTriggersWhere(criteria);
            Assert.IsFalse(triggers.Count() > 0, "Triggers were found that matched criteria when none were expected: " + triggers.ToCommaList());
        }

        protected void AssertTriggersWhere(Func<ITrigger, bool> criteria)
        {
            var triggers = this.GameController.FindTriggersWhere(criteria);
            Assert.IsTrue(triggers.Count() > 0, "Triggers were not found that matched criteria when none were expected: " + triggers.ToCommaList());
        }

        protected void AssertNotOutOfGame(IEnumerable<Card> cards)
        {
            cards.ForEach(c => AssertNotOutOfGame(c));
        }

        protected Card GetCardWithLittleEffect(HeroTurnTakerController hero)
        {
            // First try a card that just has a power on it, that is either not limited or not in play.
            var safeCard = hero.HeroTurnTaker.GetAllCards().Where(c => !c.IsInPlay && c.HasPowers && !this.GameController.IsLimitedAndInPlay(c)).FirstOrDefault();
            if (safeCard == null)
            {
                // Otherwise, try an equipment card.
                safeCard = hero.HeroTurnTaker.GetAllCards().Where(c => !c.IsInPlay && this.GameController.DoesCardContainKeyword(c, CardDefinition.KEYWORD_EQUIPMENT) && !this.GameController.IsLimitedAndInPlay(c)).FirstOrDefault();
            }
            return safeCard;
        }

        protected void AssertExpectedMessageWasShown(GameControllerMessageEvent receiver = null)
        {
            Assert.IsTrue(_expectedMessageWasShown, "Expected a message that was never shown.");
            _expectedMessageWasShown = false;

            if (receiver != null)
            {
                RemoveAssertNextMessage(receiver);
            }
        }

        protected void AssertDeckShuffled(TurnTakerController ttc, Card originalTop, Card originalTop2, Card originalBottom, Card originalBottom2)
        {
            var deck = ttc.TurnTaker.Deck;
            var same = deck.TopCard == originalTop && deck.GetTopCards(2).ElementAt(1) == originalTop2 && deck.BottomCard == originalBottom && deck.GetBottomCards(2).ElementAt(1) == originalBottom2;
            Assert.IsFalse(same, ttc.Name + " deck was not shuffled.");
        }

        protected Card DiscardCard(HeroTurnTakerController hero)
        {
            var storedResults = new List<DiscardCardAction>();
            RunCoroutine(this.GameController.DiscardCard(hero, hero.HeroTurnTaker.Hand.TopCard, null, storedResults: storedResults));
            var action = storedResults.FirstOrDefault();
            if (action != null)
            {
                return action.CardToDiscard;
            }

            return null;
        }

        protected void SetNumberOfCardsInHand(HeroTurnTakerController hero, int numberOfCards)
        {
            while (GetNumberOfCardsInHand(hero) < numberOfCards)
            {
                DrawCard(hero);
            }

            while (GetNumberOfCardsInHand(hero) > numberOfCards)
            {
                DiscardCard(hero);
            }
        }

        protected void SetToHighestHitPoints(TurnTakerController ttc, Func<Card, bool> criteria)
        {
            var character = ttc.CharacterCard;
            var hp = character.HitPoints.Value;
            var others = FindCardsWhere(c => c.IsInPlayAndHasGameText && c.IsTarget && criteria(c));
            others.ForEach(c => SetHitPoints(c, hp - 1));
        }

        protected void AssertIncapLetsHeroPlayCard(HeroTurnTakerController incappedHero, int incapIndex, TurnTakerController ttcThatPlaysCard, string identifierForCardToPlay)
        {
            var cardToPlay = PutInHand(identifierForCardToPlay);
            AssertNotInPlay(cardToPlay);
            this.DecisionSelectTurnTaker = ttcThatPlaysCard.TurnTaker;
            this.DecisionSelectCardToPlay = cardToPlay;
            UseIncapacitatedAbility(incappedHero, incapIndex);
            AssertIsInPlay(cardToPlay);
        }

        protected void AssertIncapLetsHeroUsePower(HeroTurnTakerController incappedHero, int incapIndex, HeroTurnTakerController heroThatUsesPower)
        {
            this.DecisionSelectPower = heroThatUsesPower.CharacterCard;
            this.DecisionSelectTurnTaker = heroThatUsesPower.TurnTaker;
            AssertUsablePower(heroThatUsesPower, heroThatUsesPower.CharacterCard.Identifier);
            UseIncapacitatedAbility(incappedHero, incapIndex);
            AssertNotUsablePower(heroThatUsesPower, heroThatUsesPower.CharacterCard.Identifier);
        }

        protected void AssertIncapLetsHeroDrawCard(HeroTurnTakerController incappedHero, int incapIndex, HeroTurnTakerController heroThatDrawsCard, int numberOfCardsToDraw)
        {
            this.DecisionSelectTurnTaker = heroThatDrawsCard.TurnTaker;
            QuickHandStorage(heroThatDrawsCard);
            UseIncapacitatedAbility(incappedHero, incapIndex);
            QuickHandCheck(numberOfCardsToDraw);
        }

        protected TurnTakerController FindTurnTakerController(TurnTaker tt)
        {
            return this.GameController.FindTurnTakerController(tt);
        }

        protected IEnumerable<Power> GetUsablePowersThisTurn(HeroTurnTakerController hero)
        {
            return this.GameController.GetUsablePowersThisTurn(hero);
        }


        protected void AssertNextDecisionSourceOutput(string output)
        {
            _expectedDecisionSourceOutput = output;
        }

        protected void AssertExpectedDecisionSourceOutputWasShown()
        {
            Assert.IsNull(_expectedDecisionSourceOutput, "Expected decision source output was not shown: " + _expectedDecisionSourceOutput);
        }

        protected void RevealCards(TurnTakerController ttc, int numberOfCards)
        {
            RunCoroutine(this.GameController.RevealCards(ttc, ttc.TurnTaker.Deck, numberOfCards, null));
        }

        protected void PrintGameSummary()
        {
            Console.WriteLine(this.GameController.Game.ToSummaryString());
        }

        protected void PrintSeparator(string title = "")
        {
            if (!string.IsNullOrEmpty(title))
            {
                int length = title.Length < 58 ? title.Length : 58;
                string output = "================================ " + title + " ===============================";
                Console.WriteLine(output.Substring(1 + (length / 2), output.Length - length - 1));
            }
            else
            {
                Console.WriteLine("==============================================================");
            }
        }

        protected void AssertTokenPoolCount(TokenPool pool, int count)
        {
            Assert.AreEqual(count, pool.CurrentValue, "{0} should have {1} tokens but it has {2}", pool.Identifier, count, pool.CurrentValue);
        }

        protected void RemoveInitialConditions(TurnTakerController wager)
        {
            PrintSeparator("Removing initial conditions!");
            RemoveCardTriggers(c => c.IsCondition);
            MoveCards(wager, c => c.IsCondition, wager.TurnTaker.Deck, overrideIndestructible: true);
        }

        protected DateTime StartTimer()
        {
            return DateTime.Now;
        }

        protected double StopTimer(DateTime start)
        {
            var after = DateTime.Now;

            return (after - start).TotalMilliseconds;
        }

        protected bool RunActiveTurnPhase()
        {
            var phase = this.GameController.ActiveTurnPhase;
            var controller = this.GameController.ActiveTurnTakerController;
            bool skipped = false;

            int sanity = 1000;

            if (phase.IsPlayCard)
            {
                if (phase.TurnTaker.IsHero && !phase.TurnTaker.ToHero().HasCardsInHand)
                {
                    Log.Debug(phase.TurnTaker.Name + " has no cards in their hand to play.");
                }
                else
                {
                    while (this.GameController.CanPerformPhaseAction(phase) && !skipped)
                    {
                        if (this.GameController.IsGameOver)
                        {
                            return false;
                        }

                        PlayCard(controller, out skipped);

                        sanity -= 1;
                        if (sanity <= 0)
                        {
                            Console.WriteLine("Infinite loop detected in RunPhase() - PlayCard");
                            Environment.Exit(1);
                        }
                    }
                }
            }
            else if (phase.IsAfterEnd)
            {
                RunCoroutine(this.GameController.RunAfterEndOfTurn());
            }
            else if (controller is HeroTurnTakerController)
            {
                if (phase.IsUsePower)
                {
                    while (this.GameController.CanPerformPhaseAction(phase) && !skipped)
                    {
                        if (this.GameController.IsGameOver)
                        {
                            return false;
                        }

                        SelectAndUsePower(controller.ToHero(), out skipped);

                        sanity -= 1;
                        if (sanity <= 0)
                        {
                            Console.WriteLine("Infinite loop detected in RunPhase() - UsePower");
                            Environment.Exit(1);
                        }
                    }
                }
                else if (phase.IsDrawCard)
                {
                    while (this.GameController.CanPerformPhaseAction(phase))
                    {
                        if (this.GameController.IsGameOver)
                        {
                            return false;
                        }

                        DrawCard(controller.ToHero());

                        sanity -= 1;
                        if (sanity <= 0)
                        {
                            Console.WriteLine("Infinite loop detected in RunPhase() - DrawCard");
                            Environment.Exit(1);
                        }
                    }
                }
                else if (phase.IsUseIncapacitatedAbility)
                {
                    while (this.GameController.CanPerformPhaseAction(phase))
                    {
                        if (this.GameController.IsGameOver)
                        {
                            return false;
                        }

                        this.SelectAndUseIncapacitatedAbility(controller.ToHero());

                        sanity -= 1;
                        if (sanity <= 0)
                        {
                            Console.WriteLine("Infinite loop detected in RunPhase() - UseIncapacitatedAbility");
                            Environment.Exit(1);
                        }
                    }
                }
                else if (phase.IsBeforeStart)
                {
                    RunCoroutine(this.GameController.RunBeforeStartOfTurn(controller.ToHero()));
                }
            }

            return true;
        }

        private void PlayCard(TurnTakerController ttc, out bool skipped)
        {
            if (ttc.IsHero)
            {
                var storedResults = new List<PlayCardAction>();
                RunCoroutine(this.GameController.SelectAndPlayCardFromHand(ttc.ToHero(), true, storedResults: storedResults));
                skipped = (storedResults.Count == 0);
            }
            else
            {
                RunCoroutine(this.GameController.PlayTopCard(null, ttc));
                skipped = false;
            }
        }

        protected void SelectAndUsePower(HeroTurnTakerController httc, out bool skipped)
        {
            var storedResults = new List<UsePowerDecision>();
            RunCoroutine(this.GameController.SelectAndUsePower(httc, true, storedResults: storedResults));
            skipped = (storedResults.Any(d => d.Skipped));
        }

        private void SelectAndUseIncapacitatedAbility(HeroTurnTakerController httc)
        {
            RunCoroutine(this.GameController.SelectIncapacitatedHeroAndUseAbility(httc));
        }

        protected void PrintReplays()
        {
            if (this.ReplayDecisionAnswers != null)
            {
                var i = 0;
                foreach (var decision in ReplayDecisionAnswers)
                {
                    Console.WriteLine(i++ + ": " + decision);
                }
            }
        }

        private void PrintReplayDecisionAnswers()
        {
            Console.WriteLine(this.ReplayDecisionAnswers.Select(d => d.DecisionIdentifier).ToCommaList());
        }

        public void AssertNotDamageSource(Card card)
        {
            _notDamageSource = card;
        }

        public void AssertDecisionIsOptional(SelectionType type)
        {
            _assertDecisionOptional = type;
        }

        public void GoToNextTurn()
        {
            var next = this.GameController.FindTurnTakerController(this.GameController.FindNextTurnTaker());
            if (!this.GameController.IsOblivAeonMode)
            {
                GoToStartOfTurn(next);
            }
            else
            {
                GoToBeforeStartOfTurn(next);
            }
        }

        protected void SelectAndPlayCardFromHand(HeroTurnTakerController hero)
        {
            RunCoroutine(this.GameController.SelectAndPlayCardFromHand(hero, true));
        }

        protected bool IsEquipment(Card card)
        {
            return this.GameController.DoesCardContainKeyword(card, CardDefinition.KEYWORD_EQUIPMENT);
        }

        protected bool IsMechanicalGolem(Card card)
        {
            return this.GameController.DoesCardContainKeyword(card, CardDefinition.KEYWORD_MECHANICAL_GOLEM);
        }

        protected static string PWAAUnlockedKey()
        {
            return GameController.PromoCardHasBeenUnlockedKey(PrimeWardensArgentAdeptCharacterPromoCardUnlockController.PrimeWardensArgentAdeptCharacterIdentifier);
        }

        protected IEnumerator DoNothing()
        {
            yield return null;
        }

        protected Card SetupDynamicSiphonSmokeBomb(Card cardWithDynamicSiphon, int siphonHP = 100)
        {
            // Make Dynamic Siphon the highest HP target, and damage to it is irreducible.
            PlayCard("SmokeBombs");

            var previousNextTo = DecisionNextToCard;
            DecisionNextToCard = cardWithDynamicSiphon;
            var siphon = PlayCard("DynamicSiphon");
            DecisionNextToCard = previousNextTo;

            siphon.SetMaximumHP(siphonHP, true);

            MakeDamageIrreducibleStatusEffect effect = new MakeDamageIrreducibleStatusEffect();
            effect.TargetCriteria.IsSpecificCard = siphon;
            effect.UntilCardLeavesPlay(siphon);
            RunCoroutine(this.GameController.AddStatusEffect(effect, false, new CardSource(FindCardController(siphon))));

            return siphon;
        }

        protected void AssertFaceUp(Card card)
        {
            Assert.IsTrue(card.IsFaceUp, card.Title + " was supposed to be face-up.");
        }

        protected void AssertBattleZone(TurnTakerController ttc, BattleZone bz)
        {
            Assert.AreEqual(bz, ttc.BattleZone, "{0} should be in {1} but they were in {2}", ttc.Name, bz.Identifier, ttc.BattleZone.Identifier);
        }

        protected void AssertBattleZone(Card card, BattleZone bz)
        {
            Assert.AreEqual(bz, card.BattleZone, "{0} should be in {1} but they were in {2}", card.Identifier, bz.Identifier, card.BattleZone.Identifier);
        }

        protected void SwitchCards(Card cardA, Card cardB)
        {
            RunCoroutine(this.GameController.SwitchCards(cardA, cardB));
        }

        protected void SwitchBattleZone(TurnTakerController ttc)
        {
            RunCoroutine(this.GameController.SwitchBattleZone(ttc));
        }

        protected IList<Card> MakeCustomHeroHand(HeroTurnTakerController hero, IList<string> cardIdentifiers)
        {
            // Put hero's current hand back in the deck
            MoveAllCardsFromHandToDeck(hero);

            // Move desired cards to hero's hand
            List<Card> results = new List<Card>(cardIdentifiers.Count);
            foreach (string cardIdentifier in cardIdentifiers)
            {
                Card card = MoveCard(hero, hero.TurnTaker.Deck.Cards.FirstOrDefault(c => c.Identifier.Equals(cardIdentifier)), hero.HeroTurnTaker.Hand);
                results.Add(card);
            }

            return results;
        }
    }
}


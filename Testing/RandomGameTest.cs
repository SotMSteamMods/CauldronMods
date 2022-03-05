using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random;
using Troschuetz.Random.Generators;
using System.IO;
using Handelabra;
using CauldronTests;

namespace CauldronTests.Random
{
    // Subclass this to make your own random test cases - See MyRandomTest.cs for an example!
    [TestFixture]
    public class RandomGameTest : CauldronBaseTest
    {
        private IGenerator rng = new MT19937Generator();
        private System.Random seededRng;

        protected IEnumerable<string> PreferredCardsToPlay = null;

        protected GameController SetupRandomGameController(bool reasonable, IEnumerable<string> availableHeroes = null, IEnumerable<string> availableVillains = null, IEnumerable<string> availableEnvironments = null,
            string useEnvironment = null, List<string> useHeroes = null, string useVillain = null, int? seed = null)
        {
            string environment = useEnvironment;
            var heroes = new List<string>();
            var promoIdentifiers = new Dictionary<string, string>();

            if (availableHeroes == null)
            {
                availableHeroes = DeckDefinition.AvailableHeroes;
            }

            if (availableVillains == null)
            {
                availableVillains = DeckDefinition.AvailableVillains;
            }

            if (availableEnvironments == null)
            {
                availableEnvironments = DeckDefinition.AvailableEnvironments;
            }

            var villain = "";
            var villainName = "";
            if (useVillain != null)
            {
                var identifier = useVillain;
                string fullIdentifier = identifier;
                var promoId = "";
                if (identifier.Contains("/"))
                {
                    var identifierSplit = identifier.Split('/');
                    identifier = identifierSplit[0];
                    promoId = identifierSplit[1];
                }
                villain = identifier;
                var villainDefinition = DeckDefinitionCache.GetDeckDefinition(identifier);
                villainName = villainDefinition.Name;
                if (promoId != "")
                {
                    var promo = villainDefinition.PromoCardDefinitions.Where(def => def.PromoIdentifier == promoId).First();
                    promoIdentifiers[identifier] = promo.PromoIdentifier;
                    villainName = promo.PromoTitle;
                }
            }
            else
            {
                var villainInfo = GetRandomVillain(availableVillains, promoIdentifiers);
                villain = villainInfo.Keys.FirstOrDefault();
                villainName = villainInfo.Values.FirstOrDefault();
            }

            Console.WriteLine(villainName + " threatens the Multiverse!");

            // Choose an environment
            if (environment == null)
            {
                environment = GetRandomEnvironment(availableEnvironments);
            }

            var definition = DeckDefinitionCache.GetDeckDefinition(environment);
            var environmentName = definition.Name;

            Console.WriteLine(environmentName + " is the location of the conflict.");

            // Choose heroes
            var heroesLeft = availableHeroes.ToList();
            int numHeroes = GetRandomNumber(3, 6);
            while (heroes.Count < numHeroes)
            {
                string identifier = "";
                string name = "";
                if (useHeroes != null && useHeroes.Count() > 0)
                {
                    identifier = useHeroes.First();
                    string fullIdentifier = identifier;
                    var promoId = "";
                    if(identifier.Contains("/"))
                    {
                        var identifierSplit = identifier.Split('/');
                        identifier = identifierSplit[0];
                        promoId = identifierSplit[1];
                    }
                    var heroDefinition = DeckDefinitionCache.GetDeckDefinition(identifier);
                    name = heroDefinition.Name;
                    if(promoId != "")
                    {
                        var promo = heroDefinition.PromoCardDefinitions.Where(def => def.PromoIdentifier == promoId).First();
                        promoIdentifiers[identifier] = promo.PromoIdentifier;
                        name = promo.PromoTitle;
                    }
                    useHeroes.Remove(fullIdentifier);
                }
                else
                {
                    var heroInfo = GetRandomHero(heroesLeft, promoIdentifiers);
                    identifier = heroInfo.FirstOrDefault().Key;
                    name = heroInfo.FirstOrDefault().Value;
                }

                Console.WriteLine(name + " joins the team!");
                heroes.Add(identifier);
                heroesLeft.Remove(identifier);
            }

            bool advanced = (GetRandomNumber(2) == 1);

            List<string> identifiers = MakeGameIdentifiers(villain, heroes, environment);

            seed = seed ?? GetRandomNumber();
            var gc = SetupGameController(identifiers, advanced, promoIdentifiers, randomSeed: seed);
            this.seededRng = gc.Game.RNG;

            gc.OnMakeDecisions -= MakeDecisions;
            if (reasonable)
            {
                gc.OnMakeDecisions += MakeSomewhatReasonableDecisions;
            }
            else
            {
                gc.OnMakeDecisions += MakeRandomDecisions;
            }

            return gc;
        }

        protected GameController SetupRandomOblivAeonGameController(bool reasonable, IEnumerable<string> availableHeroes = null, IEnumerable<string> availableEnvironments = null,
        List<string> useEnvironments = null, List<string> useHeroes = null, string shieldIdentifier = null, IEnumerable<string> scionIdentifiers = null, int? seed = null)
        {
            var heroes = new List<string>();
            var environments = new List<string>();

            var promoIdentifiers = new Dictionary<string, string>();

            if (availableHeroes == null)
            {
                availableHeroes = DeckDefinition.AvailableHeroes;
            }

            if (availableEnvironments == null)
            {
                availableEnvironments = DeckDefinition.AvailableEnvironments;
            }

            var villain = "OblivAeon";
            Console.WriteLine("OblivAeon threatens the very existence of the Multiverse!");

            // Choose an environment
            var envLeft = availableEnvironments.ToList();
            int numEnv = 5;
            while (environments.Count < numEnv)
            {
                string identifier = "";
                string name = "";
                if (useEnvironments != null && useEnvironments.Count() > 0)
                {
                    identifier = useEnvironments.First();
                    useEnvironments.Remove(identifier);
                }
                else
                {
                    identifier = GetRandomEnvironment(envLeft);
                }
                var definition = DeckDefinitionCache.GetDeckDefinition(identifier);
                name = definition.Name;
                
                Console.WriteLine(name + " is one of the locations threatened by OblivAeon.");
                environments.Add(identifier);
                envLeft.Remove(identifier);
            }

            // Choose heroes
            var heroesLeft = availableHeroes.ToList();
            int numHeroes = GetRandomNumber(3, 6);
            while (heroes.Count < numHeroes)
            {
                string identifier = "";
                string name = "";
                if (useHeroes != null && useHeroes.Count() > 0)
                {
                    identifier = useHeroes.First();
                    string fullIdentifier = identifier;
                    var promoId = "";
                    if (identifier.Contains("/"))
                    {
                        var identifierSplit = identifier.Split('/');
                        identifier = identifierSplit[0];
                        promoId = identifierSplit[1];
                    }
                    var heroDefinition = DeckDefinitionCache.GetDeckDefinition(identifier);
                    name = heroDefinition.Name;
                    if (promoId != "")
                    {
                        var promo = heroDefinition.PromoCardDefinitions.Where(def => def.PromoIdentifier == promoId).First();
                        promoIdentifiers[identifier] = promo.PromoIdentifier;
                        name = promo.PromoTitle;
                    }
                    useHeroes.Remove(fullIdentifier);
                }
                else
                {
                    var heroInfo = GetRandomHero(heroesLeft, promoIdentifiers);
                    identifier = heroInfo.FirstOrDefault().Key;
                    name = heroInfo.FirstOrDefault().Value;
                }

                Console.WriteLine(name + " joins the team!");
                heroes.Add(identifier);
                heroesLeft.Remove(identifier);
            }

            bool advanced = (GetRandomNumber(2) == 1);

            List<string> identifiers = MakeGameIdentifiers(villain, heroes, environments);

            seed = seed ?? GetRandomNumber();
            var gc = SetupGameController(identifiers, advanced, promoIdentifiers, randomSeed: seed, shieldIdentifier: shieldIdentifier, scionIdentifiers: scionIdentifiers);
            this.seededRng = gc.Game.RNG;

            gc.OnMakeDecisions -= MakeDecisions;
            if (reasonable)
            {
                gc.OnMakeDecisions += MakeSomewhatReasonableDecisions;
            }
            else
            {
                gc.OnMakeDecisions += MakeRandomDecisions;
            }

            return gc;
        }

        private List<string> MakeGameIdentifiers(string villain, List<string> heroes, string environment)
        {
            var result = new List<string>();
            result.Add(villain);
            result.AddRange(heroes);
            result.Add(environment);

            return result;
        }

        private List<string> MakeGameIdentifiers(string villain, List<string> heroes, List<string> environments)
        {
            var result = new List<string>();
            result.Add(villain);
            result.AddRange(heroes);
            result.AddRange(environments);

            return result;
        }

        private Dictionary<string, string> GetRandomHero(List<string> availableHeroes, Dictionary<string, string> promoIdentifiers)
        {
            int index = GetRandomNumber(availableHeroes.Count);
            var identifier = availableHeroes.ElementAt(index);
            var definition = DeckDefinitionCache.GetDeckDefinition(identifier);

            // If there is a promo, maybe choose it
            int promoIndex = GetRandomNumber(1 + definition.PromoCardDefinitions.Count());
            string name = definition.Name;

            if (promoIndex > 0)
            {
                var promo = definition.PromoCardDefinitions.ElementAt(promoIndex - 1);
                promoIdentifiers[identifier] = promo.PromoIdentifier;
                name = promo.PromoTitle;
            }

            return new Dictionary<string, string> { { identifier, name } };
        }

        private string GetRandomEnvironment(IEnumerable<string> availableEnvironments)
        {
            int environmentIndex = GetRandomNumber(availableEnvironments.Count());
            return availableEnvironments.ElementAt(environmentIndex);
        }

        private Dictionary<string, string> GetRandomVillain(IEnumerable<string> availableVillains, Dictionary<string, string> promoIdentifiers)
        {
            string villain = null;

            // Choose a villain
            int villainIndex = GetRandomNumber(availableVillains.Count());
            villain = availableVillains.ElementAt(villainIndex);
            var villainDefinition = DeckDefinitionCache.GetDeckDefinition(villain);
            var villainName = villainDefinition.Name;

            // If there is a promo, maybe choose it
            int villainPromoIndex = GetRandomNumber(1 + villainDefinition.PromoCardDefinitions.Count());
            if (villainPromoIndex > 0)
            {
                var promo = villainDefinition.PromoCardDefinitions.ElementAt(villainPromoIndex - 1);
                promoIdentifiers[villain] = promo.PromoIdentifier;
                villainName = promo.PromoTitle;
            }

            return new Dictionary<string, string> { { villain, villainName } };
        }

        protected void RunParticularGame(IEnumerable<string> turnTakers, bool advanced, IDictionary<string, string> promos, int seed, bool reasonable)
        {
            SetupGameController(turnTakers, advanced, promos, seed);
            this.GameController.OnMakeDecisions -= MakeDecisions;
            if (reasonable)
            {
                this.GameController.OnMakeDecisions += MakeSomewhatReasonableDecisions;
            }
            else
            {
                this.GameController.OnMakeDecisions += MakeRandomDecisions;
            }

            this.seededRng = this.GameController.Game.RNG;
            RunGame(this.GameController);
        }

        protected void RunGame(GameController gameController)
        {
            PrintGameSummary();

            RunCoroutine(gameController.StartGame());

            int roundLimit = 100;

            while (!gameController.IsGameOver && gameController.Game.Round <= roundLimit)
            {
                RunCoroutine(gameController.EnterNextTurnPhase());

                var state = gameController.Game.ToStateString();
                Assert.IsNotNull(state);

                // Prevent infinite loops and let us know what happened.
                int sanityCheck = 10;

                if (gameController.ActiveTurnPhase.IsPlayCard)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        PlayCard(gameController.ActiveTurnTakerController);

                        Console.WriteLine("PLAY CARD SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.IsTrue(sanityCheck > 0, "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsUsePower)
                {
                    HeroTurnTakerController hero = gameController.ActiveTurnTakerController.ToHero();
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.SelectAndUsePower(hero));

                        Console.WriteLine("POWER SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.IsTrue(sanityCheck > 0, "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsDrawCard)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.DrawCard(gameController.ActiveTurnTaker.ToHero()));

                        Console.WriteLine("DRAW CARD SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.IsTrue(sanityCheck > 0, "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
                else if (gameController.ActiveTurnPhase.IsUseIncapacitatedAbility)
                {
                    while (!gameController.IsGameOver && gameController.CanPerformPhaseAction(gameController.ActiveTurnPhase))
                    {
                        RunCoroutine(gameController.SelectAndUseIncapacitatedAbility(gameController.ActiveTurnTakerController.ToHero()));

                        Console.WriteLine("INCAPACITATED SANITY: " + sanityCheck);
                        sanityCheck -= 1;
                        Assert.IsTrue(sanityCheck > 0, "Sanity check failed in " + gameController.ActiveTurnPhase);
                    }
                }
            }

            if (gameController.IsGameOver)
            {
                Console.WriteLine("Game over!");
                Console.WriteLine(gameController.Game);

                EndingResult[] winConditions = new EndingResult[] { EndingResult.VillainDestroyedVictory, EndingResult.PrematureVictory, EndingResult.AlternateVictory };
                if (gameController.GameOverEndingResult.HasValue && winConditions.Contains(gameController.GameOverEndingResult.Value))
                {
                    Assert.Warn("The game was won!");
                }
            }
        }

        protected void PlayCard(TurnTakerController taker)
        {
            // If hero, player decides
            if (taker is HeroTurnTakerController)
            {
                RunCoroutine(taker.GameController.SelectAndPlayCard(taker.ToHero(), card => card.Location == taker.ToHero().HeroTurnTaker.Hand && FindCardController(card).CanBePlayedNow, false));
            }
            else
            {
                // Otherwise play the top card of the deck.
                RunCoroutine(taker.GameController.PlayTopCard(null, taker));
            }
        }

        protected IEnumerator MakeRandomDecisions(IDecision decision)
        {
            // Make random decisions!
            if (decision is SelectCardDecision)
            {
                var selectCardDecision = (SelectCardDecision)decision;
                var choices = selectCardDecision.Choices;
                selectCardDecision.SelectedCard = choices.ElementAtOrDefault(GetRandomNumber(choices.Count()));
            }
            else if (decision is YesNoDecision)
            {
                var yesNo = decision as YesNoDecision;
                yesNo.Answer = GetRandomNumber(1) == 1;
            }
            else if (decision is SelectDamageTypeDecision)
            {
                var damage = decision as SelectDamageTypeDecision;
                damage.SelectedDamageType = damage.Choices.ElementAtOrDefault(GetRandomNumber(damage.Choices.Count()));
            }
            else if (decision is MoveCardDecision)
            {
                var moveCard = decision as MoveCardDecision;
                moveCard.Destination = moveCard.PossibleDestinations.ElementAtOrDefault(GetRandomNumber(moveCard.PossibleDestinations.Count()));
            }
            else if (decision is UsePowerDecision)
            {
                var power = decision as UsePowerDecision;
                power.SelectedPower = power.Choices.ElementAtOrDefault(GetRandomNumber(power.Choices.Count()));
            }
            else if (decision is UseIncapacitatedAbilityDecision)
            {
                var ability = decision as UseIncapacitatedAbilityDecision;
                ability.SelectedAbility = ability.Choices.ElementAtOrDefault(GetRandomNumber(ability.Choices.Count()));
            }
            else if (decision is SelectTurnTakerDecision)
            {
                var turn = decision as SelectTurnTakerDecision;
                turn.SelectedTurnTaker = turn.Choices.ElementAtOrDefault(GetRandomNumber(turn.Choices.Count()));
            }
            else if (decision is SelectCardsDecision)
            {
                var cards = decision as SelectCardsDecision;
                cards.ReadyForNext = true;
            }
            else if (decision is SelectFunctionDecision)
            {
                var function = decision as SelectFunctionDecision;
                function.SelectedFunction = function.Choices.ElementAtOrDefault(GetRandomNumber(function.Choices.Count()));
            }
            else if (decision is SelectNumberDecision)
            {
                var number = decision as SelectNumberDecision;
                number.SelectedNumber = number.Choices.ElementAtOrDefault(GetRandomNumber(number.Choices.Count()));
            }
            else if (decision is SelectLocationDecision)
            {
                var selectLocation = decision as SelectLocationDecision;
                selectLocation.SelectedLocation = selectLocation.Choices.ElementAtOrDefault(GetRandomNumber(selectLocation.Choices.Count()));
            }
            else if (decision is ActivateAbilityDecision)
            {
                var activate = decision as ActivateAbilityDecision;
                activate.SelectedAbility = activate.Choices.ElementAtOrDefault(GetRandomNumber(activate.Choices.Count()));
            }
            else if (decision is SelectWordDecision)
            {
                var word = decision as SelectWordDecision;
                word.SelectedWord = word.Choices.ElementAtOrDefault(GetRandomNumber(word.Choices.Count()));
            }
            else if (decision is SelectFromBoxDecision)
            {
                var box = decision as SelectFromBoxDecision;
                var heroes = DeckDefinition.AvailableHeroes;
                var selectedTurnTaker = heroes.ElementAtOrDefault(GetRandomNumber(heroes.Count()));
                var promos = DeckDefinitionCache.GetDeckDefinition(selectedTurnTaker).PromoCardDefinitions.Select(cd => cd.PromoIdentifier);
                var selectedPromo = promos.ElementAtOrDefault(GetRandomNumber(promos.Count()));
                box.SelectedIdentifier = selectedPromo;
                box.SelectedTurnTakerIdentifier = selectedTurnTaker;
            }
            else if (decision is SelectTurnPhaseDecision)
            {
                var selectPhase = decision as SelectTurnPhaseDecision;
                selectPhase.SelectedPhase = selectPhase.Choices.ElementAtOrDefault(GetRandomNumber(selectPhase.Choices.Count()));
            }
            else
            {
                Assert.Fail("Unhandled decision: " + decision);
            }

            yield return null;
        }

        protected Card ChooseBestTarget(IEnumerable<Card> targets)
        {
            Card result = null;

            // Prefer the villain target with the lowest HP if available
            result = targets.Where(c => c.IsVillain).OrderBy(c => c.HitPoints).FirstOrDefault();

            if (result == null)
            {
                // Try an environment target with the lowest HP
                result = targets.Where(c => c.IsEnvironmentTarget).OrderBy(c => c.HitPoints).FirstOrDefault();
            }

            if (result == null)
            {
                // Go for the hero with the highest HP
                result = targets.Where(c => c.IsHero).OrderBy(c => c.HitPoints).LastOrDefault();
            }

            if (result == null)
            {
                Console.WriteLine("ChooseBestTarget had nothing to choose from!");
            }
            else
            {
                Console.WriteLine("ChooseBestTarget from <" + targets.Select(c => c.Title).ToRecursiveString() + "> choosing " + result.Title);
            }

            return result;
        }

        protected Card ChooseDestroyCard(IEnumerable<Card> cards)
        {
            Card result = null;

            // Prefer a villain card
            result = cards.FirstOrDefault(c => c.IsVillain);

            if (result == null)
            {
                // Try an environment card
                result = cards.FirstOrDefault(c => c.IsEnvironment);
            }

            if (result == null)
            {
                // Go for a hero
                result = cards.FirstOrDefault(c => c.IsHero);
            }

            if (result == null)
            {
                Console.WriteLine("ChooseDestroyCard had nothing to choose from!");
            }
            else
            {
                Console.WriteLine("ChooseDestroyCard from <" + cards.Select(c => c.Title).ToRecursiveString() + "> choosing " + result.Title);
            }

            return result;
        }

        protected IEnumerator MakeSomewhatReasonableDecisions(IDecision decision)
        {
            // Make random decisions!
            if (decision is SelectCardDecision)
            {
                var selectCardDecision = (SelectCardDecision)decision;

                if (selectCardDecision.SelectionType == SelectionType.RedirectDamage || selectCardDecision.SelectionType == SelectionType.DealDamage)
                {
                    selectCardDecision.SelectedCard = ChooseBestTarget(selectCardDecision.Choices);

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Nothing good to choose from, try skipping
                        if (selectCardDecision.IsOptional)
                        {
                            selectCardDecision.Skip();
                        }
                        else
                        {
                            // Just pick something
                            selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                        }
                    }
                }
                else if (selectCardDecision.SelectionType == SelectionType.DestroyCard)
                {
                    selectCardDecision.SelectedCard = ChooseDestroyCard(selectCardDecision.Choices);

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Nothing good to choose from, try skipping
                        if (selectCardDecision.IsOptional)
                        {
                            selectCardDecision.Skip();
                        }
                        else
                        {
                            // Just pick something
                            selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                        }
                    }
                }
                else if (selectCardDecision.SelectionType == SelectionType.PlayCard)
                {
                    if (PreferredCardsToPlay != null)
                    {
                        // Try to play a card we prefer
                        foreach (var identifier in PreferredCardsToPlay)
                        {
                            var card = selectCardDecision.Choices.FirstOrDefault(c => c.Identifier == identifier);
                            if (card != null && GameController.CanPlayCard(FindCardController(card)) == CanPlayCardResult.CanPlay)
                            {
                                Console.WriteLine("Playing preferred card: " + identifier);
                                selectCardDecision.SelectedCard = card;
                                break;
                            }
                        }
                    }

                    if (selectCardDecision.SelectedCard == null)
                    {
                        // Pick a card that can be played
                        var actualChoices = selectCardDecision.Choices.Where(c => GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay);
                        if (actualChoices.Count() > 0)
                        {
                            selectCardDecision.SelectedCard = actualChoices.ElementAtOrDefault(GetRandomNumber(actualChoices.Count()));
                        }
                        else
                        {
                            // Just skip
                            selectCardDecision.Skip();
                        }
                    }
                }
                else
                {
                    // Pick a random one
                    selectCardDecision.SelectedCard = selectCardDecision.Choices.ElementAtOrDefault(GetRandomNumber(selectCardDecision.Choices.Count()));
                }
            }
            else if (decision is YesNoDecision)
            {
                var yesNo = decision as YesNoDecision;
                yesNo.Answer = GetRandomNumber(1) == 1;
            }
            else if (decision is SelectDamageTypeDecision)
            {
                var damage = decision as SelectDamageTypeDecision;
                damage.SelectedDamageType = damage.Choices.ElementAtOrDefault(GetRandomNumber(damage.Choices.Count()));
            }
            else if (decision is MoveCardDecision)
            {
                var moveCard = decision as MoveCardDecision;
                moveCard.Destination = moveCard.PossibleDestinations.ElementAtOrDefault(GetRandomNumber(moveCard.PossibleDestinations.Count()));
            }
            else if (decision is UsePowerDecision)
            {
                var power = decision as UsePowerDecision;
                power.SelectedPower = power.Choices.ElementAtOrDefault(GetRandomNumber(power.Choices.Count()));
            }
            else if (decision is UseIncapacitatedAbilityDecision)
            {
                var ability = decision as UseIncapacitatedAbilityDecision;
                ability.SelectedAbility = ability.Choices.ElementAtOrDefault(GetRandomNumber(ability.Choices.Count()));
            }
            else if (decision is SelectTurnTakerDecision)
            {
                var turn = decision as SelectTurnTakerDecision;
                turn.SelectedTurnTaker = turn.Choices.ElementAtOrDefault(GetRandomNumber(turn.Choices.Count()));
            }
            else if (decision is SelectCardsDecision)
            {
                var cards = decision as SelectCardsDecision;
                cards.ReadyForNext = true;
            }
            else if (decision is SelectFunctionDecision)
            {
                var function = decision as SelectFunctionDecision;
                function.SelectedFunction = function.Choices.ElementAtOrDefault(GetRandomNumber(function.Choices.Count()));
            }
            else if (decision is SelectNumberDecision)
            {
                var number = decision as SelectNumberDecision;
                number.SelectedNumber = number.Choices.ElementAtOrDefault(GetRandomNumber(number.Choices.Count()));
            }
            else if (decision is SelectLocationDecision)
            {
                var selectLocation = decision as SelectLocationDecision;
                selectLocation.SelectedLocation = selectLocation.Choices.ElementAtOrDefault(GetRandomNumber(selectLocation.Choices.Count()));
            }
            else if (decision is ActivateAbilityDecision)
            {
                var activateAbility = decision as ActivateAbilityDecision;
                activateAbility.SelectedAbility = activateAbility.Choices.ElementAtOrDefault(GetRandomNumber(activateAbility.Choices.Count()));
            }
            else if (decision is SelectWordDecision)
            {
                var selectWord = decision as SelectWordDecision;
                selectWord.SelectedWord = selectWord.Choices.ElementAtOrDefault(GetRandomNumber(selectWord.Choices.Count()));
            }
            else if (decision is SelectFromBoxDecision)
            {
                var box = decision as SelectFromBoxDecision;
                var heroes = box.Choices;
                var selectedPair = heroes.ElementAtOrDefault(GetRandomNumber(heroes.Count()));
                box.SelectedIdentifier = selectedPair.Value;
                box.SelectedTurnTakerIdentifier = selectedPair.Key;
            }
            else if (decision is SelectTurnPhaseDecision)
            {
                var selectPhase = decision as SelectTurnPhaseDecision;
                selectPhase.SelectedPhase = selectPhase.Choices.ElementAtOrDefault(GetRandomNumber(selectPhase.Choices.Count()));
            }
            else
            {
                Assert.Fail("Unhandled decision: " + decision);
            }

            yield return null;
        }

        protected int GetRandomNumber(int min, int max)
        {
            if (seededRng != null)
            {
                return seededRng.Next(min, max);
            }
            else
            {
                return rng.Next(min, max);
            }
        }

        protected int GetRandomNumber(int max)
        {
            return GetRandomNumber(0, max);
        }

        protected int GetRandomNumber()
        {
            if (seededRng != null)
            {
                return seededRng.Next();
            }
            else
            {
                return rng.Next();
            }
        }

        // Example test implementations follow
        /*
                [Test]
                public void TestRandomGameToCompletion()
                {
                    //for (int i = 0; i < 200; i++)
                    {
                        GameController gameController = SetupRandomGameController(false);
                        RunGame(gameController);
                    }
                }
                [Test]
                public void TestSomewhatReasonableGameToCompletion()
                {
                    GameController gameController = SetupRandomGameController(true);
                    RunGame(gameController);
                }
                [Test]
                public void TestSkyScraper()
                {
                    //for (int i = 0; i < 10; i++)
                    {
                        GameController gameController = SetupRandomGameController(false, useHeroes: new List<string> { "SkyScraper" });
                        RunGame(gameController);
                    }
                }
                [Test]
                public void TestTachyon_SupersonicResponse()
                {
                    //for (int i = 0; i < 1000; i++)
                    {
                        SetupRandomGameController(true, useHeroes: new List<string> { "Tachyon" });
                        PreferredCardsToPlay = new string[] { "SupersonicResponse", "FleetOfFoot", "HUDGoggles", "PushingTheLimits" };
                        RunGame(this.GameController);
                    }
                }
                [Test]
                public void TestCelestialTribunal()
                {
                    //for (int i = 0; i < 10; i++)
                    {
                        GameController gameController = SetupRandomGameController(true, overrideEnvironment:"TheCelestialTribunal");
                        RunGame(gameController);
                    }
                }
                [Test]
                public void TestRaVersusTheEnnead()
                {
                    GameController gameController = SetupGameController(new string[] { "TheEnnead", "Ra", "Legacy", "TheWraith", "TombOfAnubis" });
                    gameController.OnMakeDecisions -= MakeDecisions;
                    gameController.OnMakeDecisions += MakeSomewhatReasonableDecisions;
                    bool expectNemesis = false;
                    bool nemesisApplied = false;
                    gameController.OnWillPerformAction += action =>
                    {
                        // If it is a deal damage action between the Ennead and Ra, make sure there is some nemesis damage going on.
                        if (action is DealDamageAction)
                        {
                            var dd = action as DealDamageAction;
                            if (IsRaVersusTheEnnead(dd))
                            {
                                // We expect to see some nemesis damage before the end.
                                expectNemesis = true;
                            }
                        }
                        else if (action is IncreaseDamageAction)
                        {
                            var increase = action as IncreaseDamageAction;
                            if (increase.IsNemesisEffect)
                            {
                                nemesisApplied = true;
                            }
                        }
                        return DoNothing();
                    };
                    gameController.OnDidPerformAction += action =>
                    {
                        // If we expected nemesis, assert that it was applied.
                        if (action is DealDamageAction && expectNemesis)
                        {
                            var dd = action as DealDamageAction;
                            if (IsRaVersusTheEnnead(dd) && dd.DidDealDamage)
                            {
                                Assert.IsTrue(nemesisApplied, "Damage was dealt from " + dd.DamageSource.TitleOrName + " and " + dd.Target.Title + ", but nemesis increase was not applied.");
                            }
                            expectNemesis = false;
                            nemesisApplied = false;
                        }
                        return DoNothing();
                    };
                    RunGame(gameController);
                }
                private bool IsRaVersusTheEnnead(DealDamageAction dd)
                {
                    return dd.DamageSource.IsCard && (dd.DamageSource.Card.Identifier == "RaCharacter" && dd.Target.Owner.Identifier == "TheEnnead")
                        || (dd.DamageSource.Card.Owner.Identifier == "TheEnnead" && dd.Target.Identifier == "RaCharacter");
                }
        */
    }
}
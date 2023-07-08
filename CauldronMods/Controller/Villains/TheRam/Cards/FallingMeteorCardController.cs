using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;


namespace Cauldron.TheRam
{
    public class FallingMeteorCardController : TheRamUtilityCardController
    {
        public FallingMeteorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new Location[] { TurnTaker.Trash, TurnTaker.Deck }, new LinqCardCriteria((Card c) => c.Identifier == "UpClose", "", false, singular: "copy of Up Close", plural: "copies of Up Close"));
            SpecialStringMaker.ShowHighestHP(1, () => Game.H - 2, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !IsUpClose(c), "", false, singular: "hero that is not Up Close", plural: "heroes that are not Up Close"));

        }

        public override IEnumerator Play()
        {

            List<Card> storedUpClose = new List<Card> { };
            List<bool> wasInDeck = new List<bool> { };

            //"Search the villain deck and trash for {H - 2} copies of Up Close..."
            IEnumerator coroutine = FindUpClose(storedUpClose, wasInDeck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            bool needToShuffle = wasInDeck.Any();
            int upCloseToPlay = storedUpClose.Count();

            if (upCloseToPlay < H - 2)
            {
                string copyPlural = upCloseToPlay == 1 ? "copy" : "copies";
                coroutine = GameController.SendMessageAction($"There were {upCloseToPlay} {copyPlural} of Up Close left to find.", Priority.High, GetCardSource(), storedUpClose);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"...and play them next to the {H - 2} heroes with the highest HP that are not up close."
            coroutine = SelectTargetsAndPlayUpClose(storedUpClose);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"If you searched the deck, shuffle it."
            if (needToShuffle)
            {
                coroutine = ShuffleDeck(DecisionMaker, TurnTaker.Deck);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //"{TheRam} deals each non-villain target {H} projectile damage."
            if (RamIfInPlay != null)
            {
                IEnumerator damage = DealDamage(GetRam, (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !IsVillainTarget(c), H, DamageType.Projectile);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damage);
                }
            }
            else
            {
                IEnumerator message = MessageNoRamToAct(GetCardSource(), "deal damage");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(message);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(message);
                }
            }
            yield break;
        }

        private IEnumerator FindUpClose(List<Card> storedUpClose, List<bool> didSearchDeck)
        {
            List<Card> upCloseInDeck = TurnTaker.GetCardsWhere((Card c) => c.IsInDeck && c.Identifier == "UpClose").ToList();
            List<Card> upCloseInTrash = TurnTaker.GetCardsWhere((Card c) => c.IsInTrash && c.Identifier == "UpClose").ToList();
            int numberToGet = H - 2;

            if(upCloseInTrash.Count() < numberToGet && upCloseInDeck.Count() > 0)
            {
                //they must get at least one from the deck
                didSearchDeck.Add(true);
            }

            bool isDeckFirst = false;
            if (upCloseInTrash.Count() > 0 && upCloseInDeck.Count() > 0 && upCloseInDeck.Count() + upCloseInTrash.Count() > numberToGet)
            {
                //there are enough Up Closes left that they could decide whether to leave the rest in the deck or the trash
                //so we give them a choice of where to look first

                var storedChoice = new List<SelectLocationDecision> { };
                IEnumerator pickLocation = GameController.SelectLocation(DecisionMaker, new List<LocationChoice> { new LocationChoice(TurnTaker.Deck), new LocationChoice(TurnTaker.Trash) }, SelectionType.SearchLocation, storedChoice, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(pickLocation);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(pickLocation);
                }
                if (DidSelectLocation(storedChoice))
                {
                    if (storedChoice.FirstOrDefault().SelectedLocation.Location == TurnTaker.Deck)
                    {
                        //they are choosing to get one from the deck first
                        isDeckFirst = true;
                        didSearchDeck.Add(true);
                    }  
                }
            }

            List<Card> ordered = new List<Card> { };
            if (isDeckFirst)
            {
                ordered = upCloseInDeck;
                ordered.AddRange(upCloseInTrash);
            }
            else
            {
                ordered = upCloseInTrash;
                ordered.AddRange(upCloseInDeck);
            }

            if (ordered.Count() <= numberToGet)
            {
                storedUpClose.AddRange(ordered);
                yield break;
            }

            for(int i = 0; i < H - 2; i++)
            {
                storedUpClose.Add(ordered[i]);
            }

            yield break;
        }

        private IEnumerator SelectTargetsAndPlayUpClose(List<Card> upCloseList)
        {
            //as we will be playing out the up closes one-at-a-time, we don't want to keep 
            //extending the threshold lower and lower - otherwise the player would be able to pick,
            //if playing two copies, the second-highest and then the new second-highest that used to
            //be third highest. 
            int highestRemainingTargets = H - 2;
            for(int i = 0; i < upCloseList.Count(); i++)
            {
                //this has to be dynamic because once we pick one of a multi-character team the rest are no longer valid
                List<Card> currentHighestOptions = GameController.FindAllTargetsWithHighestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c) && !IsUpClose(c), GetCardSource(), highestRemainingTargets).ToList();
                //Log.Debug($"Found {currentHighestOptions.Count()} viable targets");
                if (currentHighestOptions.Any())
                {
                    var storedChoice = new List<SelectTargetDecision> { };
                    IEnumerator makeDecision = GameController.SelectTargetAndStoreResults(DecisionMaker, currentHighestOptions, storedChoice, selectionType: SelectionType.HeroCharacterCard, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(makeDecision);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(makeDecision);
                    }

                    var actualChoice = storedChoice.FirstOrDefault();

                    if (actualChoice == null || actualChoice.SelectedCard == null)
                    {
                        IEnumerator message = GameController.SendMessageAction("Error, no target was selected.", Priority.High, GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(message);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(message);
                        }
                    }
                    else
                    {
                        IEnumerator play = PlayGivenUpCloseByGivenCard(upCloseList[i], actualChoice.SelectedCard);
                        if (base.UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(play);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(play);
                        }
                    }

                }
                else
                {
                    IEnumerator play = GameController.PlayCard(DecisionMaker, upCloseList[i]);
                    if (base.UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(play);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(play);
                    }
                }

                //keep track of how many targets to put on our next list of options
                highestRemainingTargets--;
                
            }
            yield break;
        }
    }
}
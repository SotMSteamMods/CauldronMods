# Assumulated Coding Wisdom

## UsePowerNumerals
UsePower implementation must use `GetPowerNumeral()` in place of hard coded integers.
This is to support Guise + Harpy edge cases.
This is only for actual numbers (1,2,3) and not words (one, two, three).

## Revealed Cards
When cards are revealed the engine moves those cards to a special location.
Cards must be explicitly moved back to a real location before the end of the effect.

`AssertNumberOfCardsInRevealed(controller, 0)` at the end of the test is required.

## Card Visibility
There's a set of methods, GameController.IsSomethingVisibleToCardSource, that should be used for all Environment deck and Hero GameController calls that query cards.
All CardController methods, generally, include this check already.  This check is critical for Obliveon's battle zone visibility rules, and for decks that put cards out of play.

## Card Queries
* The Engine provides both a Card.IsEquipment propety and a CardController.IsEquipment(Card) method.  Always use the second method to ensure the cards that add the equipment keyword word.
* Likewise, IsVillain(Card) and IsVillainTarget(Card) should be used instead of Card.IsVillain.
* When checking cards for Keywords by default cards that are under or facedown do not have keywords.  This behavior can be overriden by calling GameController.DoesCardContainKeyword directly.

## Is a Card in play, who knows!
The logic around wheather a card is in play when looking at things like cards under other cards, flipped, under and flipped, next to and flipped, etc are obscure.
This distinction matters as game text (read that as 'code') is not run when the card is out of play.
This also leads to unclear scenarios when moving cards from between locations (under a card, to the play area) where the card may not cleanly enter play and fire all triggers.

## The Test Engine and the Game Engine do not have completely identical behavior.
The addition of the UI does change bahavior.  Certain flags that won't effect the tests will effect the UI.

## The Sentinels and Multiple Character Cards
This is a topic we'll learn more about as we go, but first pitfall is that base.CharacterCard is null
if the Hero has multiple character cards.  TheSentinels have the property HasMultipleCharacterCards set true.
Obliveon Mission Reward characters is the other case where we may need to code specifically around.

## Test Suggestions
* Always have multiple characters in the test game.
* Always check that the other characters/targets are not effected by what you are testing.
  * Example: A DealDamage test should check that the other targets are not damaged.
* `QuickHPStorage(deck1, deck2...)` and `QuickHPCheck(0 , 0...)` are fast and easy ways.
* Other useful 'Quicks'
  * `QuickHandStorage/Check` - For checking card play/ card draw effects
  * `QuickShuffleStorage/Check` - For checking shuffle effects
* Be aware that cards are in play and in hand once StartGame is called.
  * Villian/Hero/Enviroment Setup will be completed - Example, BaronBlade will have a MobileDefensePlatform in play
  * Hero's will have a hand of random cards.
* The Test Engine will make default choices for you, but it is recommended that the Decision Properties be used.
* If a test sequence needs multiple choices (even across different choices) the Decision properties that take a
IEnumerable can the loaded up with the choices in order.
* When making a sequence of choices, if the Decision Property is null, the auto choice will be used, for an explicit skip
use a IEnumerable with a null element.  Likewise, a null in the sequence will effectively select Skip part way through.
* Many Test functions that manipulate cards by Identifier return the specific Card object. 
Store this, and use it in subsequent tests.

## Code Reviews
* All code merged into main must be reviewed.
* The minimum for a issue/small change code review is visual inspection of the changes.
    * Ensure that no 'exploit' or dangerous code is added to the DLL.  A new dependancy, file access, or a unexpected using statement should all be considered highly suspicous.
    * Does the code seem to do what it says?
    * It is recommended that a test should be added or modified to capture the issue fix.
* For full deck, or large change review, the above applies, plus
    * The branch should be checked out, and the full test suite run against the branch.
    * The branch should be run up in game and played at least once.
    * Each card's implementation should be compared to the Google Drive card image.
    * The in-game deck text, flavor text should be compared to the Google Drive card image.
    * Changes or recommendations should be discussed in the PR before being made.
    * The reviewer can and should make small edits and improvements to the deck.











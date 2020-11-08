# Assumulated Coding Wisdom

## UsePowerNumerals
UsePower implementation must use `GetPowerNumeral()` in place of hard coded integers.
This is to support Guise + Harpy edge cases.
This is only for actual numbers (1,2,3) and not words (one, two, three).

## Revealed Cards
When cards are revealed the engine moves those cards to a special location.
Cards must be explicitly moved back to a real location before the end of the effect.

`AssertNumberOfCardsInRevealed(controller, 0)` at the end of the test is required.

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












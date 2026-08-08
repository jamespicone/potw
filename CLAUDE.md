# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Parahumans of the Wormverse is a mod for Sentinels of the Multiverse Digital, adding heroes, villains, and environments based on the Parahumans web serials (Worm and Ward) by Wildbow.

## Build Commands

```bash
# Build using dotnet directly (preferred)
dotnet build Mod/ParahumansOfTheWormverse.csproj
dotnet build Test/ParahumansOfTheWormverseUnitTests.csproj

# Run all tests
dotnet test Test/ParahumansOfTheWormverseUnitTests.csproj

# Run tests for a specific hero/villain
dotnet test Test/ParahumansOfTheWormverseUnitTests.csproj --filter "FullyQualifiedName~Dauntless"

# Run a single test
dotnet test Test/ParahumansOfTheWormverseUnitTests.csproj --filter "FullyQualifiedName~TestClassName.TestMethodName"

# Quick test run (no rebuild)
dotnet test Test/ParahumansOfTheWormverseUnitTests.csproj --no-build
```

Note: The `build.ps1` Cake script may have compatibility issues. Use `dotnet` commands directly for reliable builds.

## Architecture

**Framework**: .NET Framework 4.8 (SDK-style projects)
**Game Engine**: Sentinels of the Multiverse modding API (Handelabra.Sentinels.Engine)
**Key Dependencies**: JpSOTMUtilities (2.2.1), NUnit (4.0.1)

**Game Source Reference**: Decompiled game source is available at `D:\Programming\src\new_extracted_sotm\extracted` (extracted via ILSpy). Use this to understand engine internals, find method signatures, or see how base game cards implement mechanics.

### Project Structure

- **Mod/**: Main mod code
  - `Heroes/`, `Villains/`, `Environments/`: Each contains a folder per deck
  - `DeckLists/`: JSON files defining card metadata, stats, and abilities
  - `Assets/`: Images and sprite atlases
  - `BaseGameDecklists/`: Reference JSON for base game cards

- **Test/**: NUnit test project
  - Mirrors the Mod structure (Heroes/, Villains/, Environment/)
  - `ParahumanTest.cs`: Base test class with helper properties for all heroes/villains
  - `RandomGameTest.cs`: Infrastructure for randomized game testing

### Card Controller Pattern

Each deck follows this structure:
```
DeckName/
├── Character/
│   └── DeckNameCharacterCardController.cs  # Character card abilities
└── Cards/
    └── CardNameCardController.cs           # Individual card implementations
```

Card controllers inherit from `CardController` and use:
- `Handelabra.Sentinels.Engine.Controller` and `.Model` namespaces
- `Jp.SOTMUtilities` for helper extensions
- Namespace pattern: `Jp.ParahumansOfTheWormverse.DeckName`

### Deck Identifiers

Heroes, villains, and environments use fully qualified identifiers:
- Heroes: `Jp.ParahumansOfTheWormverse.Alexandria`, `.Armsmaster`, `.Battery`, `.Bitch`, `.Dauntless`, `.Dragon`, `.Grue`, `.JessicaYamada`, `.Labyrinth`, `.Legend`, `.MissMilitia`, `.Skitter`, `.Tattletale`
- Villains: `Jp.ParahumansOfTheWormverse.Behemoth`, `.Coil`, `.Echidna`, `.Leviathan`, `.Lung`, `.Slaughterhouse9`, `.TheMerchants`, `.TheSimurgh`
- Environments: `Jp.ParahumansOfTheWormverse.BrocktonBay`, `.CoilsBase`, `.Kyushu`, `.NewDelhi`

### Testing

Tests inherit from `ParahumanTest` which provides:
- Hero/villain accessor properties (e.g., `alexandria`, `skitter`, `behemoth`)
- `SetupGameController("VillainId", "HeroId", "EnvironmentId")` to initialize games
- Helper methods: `AssertDamageSource()`, `AssertDamageType()`, `IncapacitateCharacter()`, etc.

Base game decks (BaronBlade, InsulaPrimalis, etc.) can be used in tests for standard opponents/environments.

### Card Definitions (Deck Lists)

**Mod deck lists** are in `Mod/DeckLists/`:
- `DeckLists/Heroes/DauntlessDeckList.json`
- `DeckLists/Villains/BehemothDeckList.json`
- `DeckLists/Environments/BrocktonBayDeckList.json`

**Base game deck lists** are in `Mod/BaseGameDecklists/` with full namespaced names:
- `Handelabra.Sentinels.Engine.DeckLists.BaronBladeDeckList.json`
- `Handelabra.Sentinels.Engine.DeckLists.InsulaPrimalisDeckList.json`

These define card identifiers, titles, keywords, HP values, card text, and abilities.

### Game Mechanics

**Keywords vs Card Types**: In Sentinels, "Ongoing", "Equipment", and "One-Shot" are card types. Keywords like "Relic", "Limited", "Charge" are separate. A villain card that says "destroy all hero ongoing cards" won't affect a "Relic" card because Relic is a keyword, not a card type.

**Card Visibility**: Cards can make themselves invisible to certain card sources by overriding `AskIfCardIsVisibleToCardSource`. When a card is invisible to a card source, that source's effects can't target, select, or affect the invisible card. Test visibility with:
```csharp
var cardSource = new CardSource(FindCardController(villainCard));
Assert.That(GameController.IsCardVisibleToCardSource(targetCard, cardSource), Is.False);
```

**Status Effect Timing**: `UntilEndOfNextTurn(TurnTaker)` means the effect expires at the end of that turn taker's NEXT turn, not the current turn. If used during your turn, it lasts through the entire next round until the end of your following turn.

**Damage Reduction Triggers**: To check if a specific status effect reduced damage, examine `DealDamageAction.DamageModifiers` for entries with matching `CardSource.StatusEffectSource.CardSource`.

## Testing Patterns

Tests inherit from `ParahumanTest` which extends `BaseTest` from the Handelabra test framework. Understanding decision handling is critical for writing tests.

### Decision Properties (BaseTest)

The test framework uses decision properties to automate player choices. Set these BEFORE the action that triggers the decision:

```csharp
// Single selections (applies to ALL decisions of that type)
DecisionSelectCard = someCard;           // For card selection UI
DecisionSelectTarget = targetCard;       // For target selection
DecisionSelectTurnTaker = hero.TurnTaker; // For player selection
DecisionSelectDamageType = DamageType.Fire; // For damage type choice
DecisionSelectLocation = new LocationChoice(location); // For location selection

// Multiple selections (consumed in order, one per decision)
DecisionSelectCards = new Card[] { card1, card2 };
DecisionSelectTargets = new Card[] { target1, target2, null }; // null = skip/decline

// Skip optional selections
DecisionDoNotSelectCard = SelectionType.PlayCard;  // Skip optional plays
DecisionDoNotSelectCard = SelectionType.SelectTarget; // Skip optional targeting

// Yes/No decisions (rarely needed - most "optional" actions use card selection with Skip)
DecisionYesNo = true;
```

**Important**: `DecisionSelectCards` array is consumed in order for ALL card/target selections. If a card has multiple selection steps (e.g., attach location + deal damage target), they all consume from this array. Use singular `DecisionSelectCard` when the same choice applies to all selections.

### HP Tracking

```csharp
QuickHPStorage(hero1, hero2, villain);  // Store current HP values
// ... do something that deals damage ...
QuickHPCheck(-2, -1, -3);  // Verify HP changes (negative = damage taken)
```

### Common Test Helpers

```csharp
RemoveMobileDefensePlatform();  // Baron Blade's MDP makes him immune - remove it first
PlayCard("CardName");           // Play from deck
PlayCard("CardName", 0);        // Play specific copy (0-indexed)
PutInHand("CardName");          // Move to hand, returns the card
GetCard("CardName");            // Get card reference without moving it
MoveCard(controller, card, destination);
UsePower(card);                 // Use a power on a card
UseIncapacitatedAbility(hero, index); // Use incap ability (0-indexed)
DealDamage(source, target, amount, DamageType.Fire);
DestroyCard(card);
GoToStartOfTurn(turnTaker);
GoToPlayCardPhase(turnTaker);
AssertIsInPlay(card);
AssertInHand(card);
AssertInTrash(card);
AssertNextToCard(card, otherCard);
IncapacitateCharacter(heroCard, damageSource);
```

### Common Testing Pitfalls

1. **Nemesis Damage**: Baron Blade deals +1 damage to Legacy (nemesis). Use Bunker instead for predictable damage tests.

2. **MDP Immunity**: Baron Blade's Mobile Defense Platform makes him immune to damage while in play. Call `RemoveMobileDefensePlatform()` before tests that need to damage Baron.

3. **Optional Plays**: Cards with "you may play a card" use `SelectAndPlayCardFromHand`, not Yes/No. Skip with `DecisionDoNotSelectCard = SelectionType.PlayCard`.

4. **Limited Cards**: When a second Limited card is played, the second copy goes to trash (not the first).

5. **Target Selection with Skip**: For "up to X targets", use `null` in the array to stop early:
   ```csharp
   DecisionSelectTargets = new Card[] { target1, null, null }; // Select 1 of up to 3
   ```

6. **Decision Array Interleaving**: When a card makes multiple selections of different types, `DecisionSelectCards` is consumed for ALL of them. Either:
   - Use singular `DecisionSelectCard` if the same choice works for all
   - Interleave properly: `{ cardChoice1, targetChoice1, cardChoice2, targetChoice2 }`
   - Simplify the test to avoid complex decision chains

### Test Structure Example

```csharp
[Test()]
public void TestDealsDamageToVillain()
{
    SetupGameController("BaronBlade", "Jp.ParahumansOfTheWormverse.Dauntless", "Bunker", "InsulaPrimalis");
    StartGame();

    RemoveMobileDefensePlatform();  // So Baron can take damage

    QuickHPStorage(baron);
    DecisionSelectTarget = baron.CharacterCard;

    PlayCard("SomeAttackCard");

    QuickHPCheck(-2);  // Verify Baron took 2 damage
}
```

# Test Coverage Plan

**Status:** the deck-by-deck coverage campaign (every card in every deck gets
behavioural tests) is **done** — see [History](#history) — as is the
[Celestial Tribunal power sweep](#celestial-tribunal-power-sweep-2026-08-10---done).
Last full-suite run: 1791 passed, 0 failed, 6 skipped.

---

## Celestial Tribunal power sweep (2026-08-10) - DONE

Three hero decks had been fixed one at a time for the same class of bug (Miss
Militia `ad38769`, Grue `ed1df6d`, Battery `c00ff3b`, plus Battery's
`IsDischargePower` in `5a8810c`). Rather than keep finding these one random-test
crash at a time, every hero character card is now driven through the Tribunal
deliberately. Branch `claude_celestial_tribunal_hero_powers`.

### Scope: what the Tribunal can actually reach

**Only powers on a hero *character* card.** Verified against engine source:
Representative of Earth is the only way one of our cards ends up owned by a
non-hero, and it summons a hero character card (plus its shared-identifier
siblings) and nothing else - no deck, hand or trash. Reaching that card's powers
needs `allowAnyHeroPower: true`, and exactly four cards in the whole game pass
it, each filtering to a character card:

| Card | Power filter |
|---|---|
| Called to Judgement | `power.CardSource.Card == _representative` |
| Character Witness | `power.CardSource.Card == _representative` |
| Guise, "I Can Do That Too!" | `p.CardController.Card.IsHeroCharacterCard` |
| Completionist Guise | `p.CardController.Card == selectedHero` (a character card) |

So the ~30 powers on our non-character cards (halberds, Legend's shot cards,
Skitter's and Tattletale's ongoing/equipment, Labyrinth's, Alexandria's Cape,
Grue's Martial Talent, Battery's Magnetism, Dauntless's Arcshield/Arcstep,
Miss Militia's Weapons) **cannot be lent by the Tribunal** and are out of scope.
They are still reached *indirectly* where a character power says "use a power on
a Weapon card" (Protectorate Captain) or "you may use a power" (Ruler of
Brockton Bay) - those paths get exercised through the character card test.

### The 19 summonable character cards

`GameController.GetHeroCardsInBox` is the authority on what Representative of
Earth can offer, and it is implemented app-side, so it was settled empirically
rather than from the decklists: it lists **Jessica's three real character cards**
(`JessicaYamadaCharacterTarget` / `...Environment` / `...NotTarget`) and **not**
her Instructions cards, which are `isReal: false` - the same shape as The
Sentinels' and The Ennead's instructions cards, which are likewise absent. So her
summoned copy is a plain character card with a self-contained power, and the
multi-character-hero setup the base game mishandles never comes into it.

Two consequences of Representative of Earth calling `SetMaximumHP(10,
alsoSetHP: true)` on whatever it summons, both covered by tests in
`JessicaYamadaTests`:

- Her `...NotTarget` variant, which is deliberately not a target in her own deck,
  becomes a real 10 HP target as a Representative - so killing it is an ordinary
  `EnvironmentDefeat`.
- Her `...Target` variant keeps its "redirect damage from non-hero sources to the
  lowest-HP hero" trigger, which inverts the Tribunal's usual pressure: hitting
  the Representative hurts the real heroes instead, and she never takes the hit.
  Rules as written - she is a therapist, not a combatant - and she cannot redirect
  to herself because the summoned card is not a real card. Left alone, same call
  as Grue's Darkness below.

The full list, all now covered: Alexandria, Armsmaster, Battery x2 (base +
Cauldron Cape), Bitch, Dauntless, Dragon, Grue, Jessica x3, Labyrinth, Legend,
Miss Militia x2 (base + Protectorate Captain), Skitter, Tattletale x3 (base +
Ruler of Brockton Bay + Hunter of Secrets).

### Two shapes, both tested per card

**Summoned and used directly** (`TestBroughtInByRepresentativeOfEarth`) - summon,
run a round of turns, then use each power. Here `CharacterCard` is null,
`HeroTurnTakerController` is null, and there is no deck, hand or trash. Not
reachable from base game content, but it is the state the null `CharacterCard`
lives in and it is where every crash was found. Bar: must not crash, and must
degrade sanely.

**Lent by Called to Judgement** (`TestPowerLentByCalledToJudgement`) - the only
shape reachable in real play. The lender substitutes the borrowing hero in for
the duration, so `TurnTaker.Deck`, `TurnTaker.PlayArea`, `HeroTurnTaker`, `Card`
and `CharacterCard` are all theirs; the tests assert the effect lands on *their*
resources.

Two helpers in `ParahumanTest` drive both: `SummonRepresentativeOfEarth(deck,
characterCard)` and `UsePowerLentByCalledToJudgement(borrower, powerIndex)`. The
latter installs its own `OnMakeDecisions` hook so that only Called to Judgement's
"select a hero" (`SelectionType.UsePowerOnCard`) is answered for the caller -
everything the borrowed power itself selects still goes through the normal
Decision properties.

### Bugs found and fixed

- **Labyrinth - the environment's turn disappeared from the game.** Her
  `AskIfTurnTakerOrderShouldBeChanged` implements "takes her turn immediately
  after the first Environment turn". Once the environment owns her, `TurnTaker`
  *is* the environment, so "if we are going to our turn and the previous turn
  taker wasn't the environment, skip our turn" skipped the environment's turn,
  every round, forever. Guarded with `TurnTaker.IsPlayer`. Much the worst of the
  five - it silently breaks the whole game rather than throwing.
- **Legend - null `CharacterCard` handed to a `DamageSource`.** All 11
  `sourceCard.CharacterCard` sites across the character card, Freezeblast,
  Kaleidoscope and Scatterblast now go through
  `LegendExtensions.FindLegendCharacterCard` (`CharacterCard ?? Card`).
  `UsePower` also passed `CharacterCardController`, which comes from the turn
  taker controller and is therefore null when the environment owns us; it passes
  `this` now, matching what `ApplyEffects` already did.
- **Legend - the lent power silently did nothing.** Separate from the above, and
  only visible in the lent shape. `CardController.GetActivatableAbilities` reads
  `Card.Definition`, and inside the replacement window `Card` is the *borrower's*
  character card - so Legend's own "effect" ability was invisible to the very
  power being lent, `ChooseEffects` found no choices, and the power resolved to
  nothing at all. `LegendCharacterCardController` now overrides
  `GetActivatableAbilities` to read `CardWithoutReplacements.Definition`. This
  also fixes Guise borrowing the power.
- **Skitter - null `CharacterCard` in `AddBugTokenToSkitter`.** Now
  `(co.CharacterCard ?? co.Card).FindBugPool()`, which folds into the existing
  null-pool "sorry guise" guard.
- **Tattletale: Hunter of Secrets - null `CharacterCard.FindTokenPool`.** Same
  shape, same fix.

### Known limitation, deliberately left

**Hunter of Secrets' powers do nothing when lent.** The token pool they count
lives on Tattletale's own card, and under the replacement both `Card` and
`CharacterCard` are the borrower's, so the existing null-pool guard bails out.
That is the pre-existing "power doesn't work if guise uses it. TODO: Better fix?"
behaviour, not a Tribunal regression - reaching our own pool would need
`CardWithoutReplacements`, which would change Guise's behaviour too. The test
asserts the current no-op rather than silently changing it. Worth deciding
separately.

## Reference: Celestial Tribunal mechanics

The Celestial Tribunal's **Representative of Earth** pulls a hero character card
out of the box and puts it into play *for the environment*:

```csharp
cardController = CardControllerFactory.CreateInstance(modelCard, turnTakerController, overrideNamespace);
```

`turnTakerController` is the Tribunal's, so for that copy of the card:

- `CardController.HeroTurnTakerController` is **null** (the turn taker isn't a
  hero), and `CardController.HeroTurnTaker` is
  `HeroTurnTakerController.HeroTurnTaker`, so touching it NREs.
- `CardController.CharacterCard` is **null** — it resolves to
  `TurnTaker.CharacterCard`, and environments have no character card.
- The hero's **deck, hand and trash are not in the game at all**. Only the
  character card is created, plus its shared-identifier siblings.

**The borrowing hero is substituted in while a lent power runs.** All four
`allowAnyHeroPower: true` cards register in `ReplacesTurnTakerController` and
`ReplacesCards` for the duration, so inside the power `TurnTakerController` is
the borrowing hero's: `TurnTaker.Deck`, `TurnTaker.PlayArea` and `HeroTurnTaker`
are theirs, `HeroTurnTakerController` is not null, and both `Card` and
`CharacterCard` resolve to *their* character card. That is the rules-as-written
reading of "you", and it is what the mod should do. The null `CharacterCard`
therefore only exists **outside** that window. Guard it anyway: the constructor
and trigger crashes are real regardless of how a power is used, and `Card` is the
right hero-self reference either way because it follows replacement.

This is *not* the same shape as the Legend/Called to Judgement bug in
[Known engine and flakiness issues](#reference-known-engine-bugs): that one is a
boxed card temporarily *replacing* one of ours; this one is our card being
permanently owned by a non-hero.

### Already fixed

- **Battery**: `IsDischargePower` assumed `UsePowerAction.HeroUsingPower` was
  non-null; powers on these cards have no hero using them at all
  (`GlowingThreadsTests`). Both character cards charged/discharged
  `CharacterCard`, so `BatteryChargedStatusEffect`'s constructor NREd on
  `chargedCard.Title` — they use `Card` now. The base card's charge power also
  passed `HeroTurnTaker` to `DrawCard`; `DrawCard()` with no argument reports
  "has no cards to draw" instead. The discharge power's
  `SelectAndPlayCardFromHand` was already the `CardController` wrapper, which
  null-checks the hero on its own. (`BatteryTests`, `BatteryCauldronCapeTests`)
- **Miss Militia**: the Protectorate Captain **constructor** dereferenced
  `HeroTurnTaker.Hand` for a special string. Base game guards the same call with
  `if (TurnTaker.IsPlayer)` (see `PrimeWardensHakaCharacterCardController`,
  `SkyScraperTinyCharacterCardController`). Its power also called
  `GameController.SelectAndPlayCardFromHand` directly — only the `CardController`
  wrapper of that name null-checks the hero, so use that one. (`MissMilitiaTests`)
- **Grue**: `PutDarknessIntoPlay` synthesises a Darkness by copying an existing
  one's definition, and there are none when the deck isn't in the game. It now
  falls back to the deck definition reached through
  `CardWithoutReplacements.Definition.ParentDeck`. Three things that took getting
  right:
  - `CardControllerFactory` resolves the controller under
    `turnTakerController.TurnTaker.DeckDefinition.Identifier` — the
    *environment's* — so it silently falls back to a plain `CardController` with
    no triggers. Pass `overrideNamespace` (`"<card ns>.<deck identifier>"`) and
    write the same `"OverrideTurnTaker"` card-property journal entry
    Representative of Earth writes, so a reload rebuilds it the same way. On
    *every* synthesis, not just the first — the second finds the first card and
    copies its definition, and would otherwise lose the namespace.
  - `PutDarknessesIntoPlay` passed `CharacterCard` (null) as the card to attach
    to. It uses `Card` now, which is how base game reads a character card's
    reference to its own hero (`TheWraithCharacterCardController` points its
    damage-reduction status effect at `base.Card`). `Card` also follows card
    replacement, so Completionist Guise borrowing the power resolves to Guise's
    card; `CharacterCard` would not.
  - Darkness is a different card, so it can't use `Card` to find him —
    `GrueExtensionMethods.FindGrueCharacterCard` does it: our turn taker's
    character card normally, otherwise the Grue character card in our owner's
    play area.
  - Darkness's "at the end of Grue's next turn remove this from the game" trigger
    is keyed on `tt == TurnTaker`, which is the *environment's* turn once it owns
    him — it would have been removed at the end of every round. Guarded with
    `TurnTaker.IsPlayer` so it never comes due.

  **Decision (2026-08-10): the Darkness cards stay in play forever, rules as
  written. Do not "fix" this.** Grue is not a player and never takes a turn, so
  the end-of-turn condition never happens. There is no base game precedent to
  borrow — a summoned hero has no deck, so base game can never put one of that
  hero's non-character cards into play, and Sky-Scraper's size cards (which
  Representative of Earth does handle) are character cards resolved by
  replacement. The consequence is known and accepted: Character Witness lends
  this power at the start of every environment turn, each use places two Darkness
  that never leave, and multiple Darkness next to the same card each reduce
  separately. It is left in because it is exactly what the card says, it crashes
  nothing, and it is not game-crushing — Darkness reduces the first damage dealt
  *by* the card it is next to as well as the first dealt *to* it, so stacking
  them on a villain shields that villain from the heroes' opening hit each turn
  by the same amount. Treating the environment turn as Grue's turn was considered
  and rejected as not what the card says. (`GrueTests`)

---

## Reference: known engine bugs

Reproductions are kept in an uncommitted `Test/EngineBugRepros.cs` (they fail by
design, so they are deliberately not part of the suite). Decision (2026-08-09):
the Power Overwhelming and Shocking Animation ones are worth reporting upstream;
the tie-break one can't be reached with base game content alone, so we guard our
own cards against it instead.

- **`GameController.DetermineTurnTakersWithMostOrFewest`** stores
  `selectTurnTakerDecision.SelectedTurnTaker` without checking the decision
  completed, so a **refused** decision puts a null in the results list. It is
  refused whenever `CanPerformAction` says no: the card source is inhibited, the
  source card flipped since its CardSource was captured (`IsCardOnWrongSide`), or
  the game is over. `Count() > 0` does **not** protect callers — the list has one
  null element. Check `FirstOrDefault() != null`, which is what most base game
  cards do. The HP equivalent (`DetermineTargetWithLowestOrHighestHitPoints`)
  guards correctly, so `FindTargetWith{Lowest,Highest}HitPoints` call sites are
  fine. Seen via Citizen Summer's end-of-turn most-cards damage.
  *Our seven call sites were audited 2026-08-09* — Trickster, A Terrible Defeat,
  Bakuda and A Fate Selected already null-check; Cherish was crash-safe only via
  a second guard and now checks explicitly; Leap used `Count() > 0` then
  `.First()` (would have given proximity tokens to the wrong two heroes) but only
  runs during its own `Play()` where a card is never inhibited, so that one is
  defensive only.
- **Hades' Power Overwhelming**: `ShouldIncreasePhaseActionCount` assumes the
  card sits in a hero play area; NREs mid-move (seen with Tempest's Into the
  Stratosphere relocating it).
- **Chokepoint's Shocking Animation × Guise's "Uh, Yeah, I'm That Guy!"**: the
  rerun `Play()` calls `MakeTargettable(GetCardThisCardIsNextTo())` with a null
  next-to card and `MakeTargetAction.ToString()` NREs.

### Mod bugs found by the random suite (all fixed 2026-08-08)

- **Behemoth**: `ProximityPool()` returns null for a hero whose marker has left
  play, which happens when they are incapacitated — including part-way through a
  card's own effect. Incinerate and the character card's end-of-turn damage
  dereferenced it unguarded. `BehemothTestBase.RemoveProximityMarker` sets this
  state up.
- **Echidna**: Guise's "Uh, Yeah, I'm That Guy!" reruns the `Play()` of ongoings
  in the high-fived hero's play area with the TurnTaker property replaced by
  Guise. Engulfed (attached next to a hero) looked up the Twisted subdeck through
  `TurnTaker` — null for Guise. Its power-punish trigger is also cloned by Guise
  and needed a null next-to guard. Its `Play()` must *return* the coroutine
  rather than driving it inline with `ExhaustCoroutine`: Engulfed plays the top
  Twisted card, which can be another Engulfed, so inline driving adds a stack
  frame per level and overflows on some seeds — which kills the test host
  silently and hangs the whole run.
- **Legend**: `ChooseEffects` cast every controller offering an "effect"
  activatable ability to `IEffectCardController`. Inside a Called to Judgement
  window a boxed card's controller reports Legend's character card as its own
  (`Card` resolves to `LegendCharacter` while `CardWithoutReplacements` is
  `FanaticCharacter`), so it offers Legend's own "effect" ability and the blind
  cast threw. `ChooseEffects` no longer uses `SelectAndActivateAbility` — its only
  filter is a `LinqCardCriteria` and both copies report the same Card — it gathers
  the abilities itself (`GetActivatableAbilitiesInPlayEx`), keeps only those whose
  controller is one of ours, and runs its own `ActivateAbilityDecision`. That
  matters for play, not just crashes: otherwise the player is offered two
  identical "Legend" entries and picking the borrowed one silently does nothing.
  `CurveshotTests` builds this with Character Witness, whose power-lending happens
  on a turn trigger rather than during its own play, so the borrowed power can be
  set up separately.
- **`CarryTheChargeTests.TestLimited`**: when the random opening hand contained
  all copies of Carry the Charge, both `PutInHand` calls returned the same card
  (`GetCard` prefers deck/trash, falls back to hand). Fixed by returning the hand
  to the deck first.

---

## Reference: test-harness gotchas

Collected from the deck-by-deck campaign; all still apply.

- **`GoToEndOfTurn` / `GoToPhase` only advance phases — they never perform phase
  actions.** Nothing is ever drawn or played by them, so stack ONLY the card
  under test.
- **Never round-trip an in-play card out of play and replay it.** Its triggers
  stay dead (`AreTriggersActive` guard). Use `ResetTriggers` — see
  `LeviathanTestBase.PutTacticInPlay`.
- **`BaseTest.PlayCard(string)` strips a card's triggers before playing it**,
  expecting the play to re-add them. If the card is already in play the engine
  refuses the replay and the triggers stay permanently dead. For character cards
  that a random setup may already have deployed, use a put-in-play helper instead
  (`Slaughterhouse9TestBase.PutMemberInPlay`).
- **`RemoveVillainCards()` must come after `StartGame()`** — Baron Blade's Mobile
  Defense Platform only enters play during StartGame, and otherwise makes him
  immune.
- **Optional draws can't be declined in tests**: the engine's Smart auto-draw
  policy answers them yes whenever no draw triggers are in play (Labyrinth's
  Exploration).
- **Some decisions arrive pre-`AutoDecided`** (Skitter's Sweep the Area "return
  in any order"), so tests can't choose the order — assert contents with
  `Is.EquivalentTo`.
- **`mostFewestSelectionType` only changes the decision label**, not which target
  is selected (caught A Fate Selected hitting the hero with the *most* cards).
- **The journal doesn't record without `StartGame`** — anything that reads card
  properties or looks up flips needs a started game.
- **Default hero variants' character cards are not targets.** To damage one, have
  another hero hit themselves instead (also sidesteps Baron's nemesis bonus).
- **The engine's one-shot cleanup overrides a move made inside `Play()`** —
  override `GetTrashDestination()` instead (Behemoth's movement cards).

---

## History

The original goal — every card in every deck gets behavioural tests, replacing
the ~149 `TestModWorks` stubs — is complete. Phases and outcomes:

| Phase | Scope | Outcome |
|---|---|---|
| 1 | Untested villains: Lung, Leviathan, Behemoth, Coil, The Simurgh | 185 tests; 6 real mod bugs found & fixed |
| 2 | Environments: Brockton Bay, Coil's Base, Kyushu, New Delhi | 97 tests |
| 3 | Partial villains: Slaughterhouse 9, Echidna Twisted cards, The Merchants | 43 tests; 1 real bug (They're All Better Now could never revive) |
| 4 | Hero gaps: Jessica Yamada, Bitch, Alexandria, Labyrinth, Skitter | all stubs replaced |
| 5 | Deepening: S9 character + members, Echidna single-test files | +25 tests; 2 real bugs (flipped Bonesaw's heal, flipped Jack Slash's power damage) |

A `CoverageMetaTests.cs` guardrail (fail if any decklist card lacks a
non-`TestModWorks` fixture) was added in Phase 5 and dropped 2026-08-08 as not
worth keeping. It caught one real gap — the Slaughterhouse 9 character card had
no tests — and two misspelled fixtures.

Finished decks keep one `TestModWorks` load test in the deck's main test file.
The quick manual check for stub-only files:

```bash
grep -rl TestModWorks Test/ | xargs grep -cE '\[Test\(\)?\]' | grep ':1$'
```

### Note on flakiness

Roughly 1 in 3 full-suite runs used to fail from the seeded random tests. All
mod-side causes were fixed 2026-08-08 (see
[mod bugs found by the random suite](#mod-bugs-found-by-the-random-suite-all-fixed-2026-08-08));
the three engine-side bugs above can still fail a random test on rare seeds.

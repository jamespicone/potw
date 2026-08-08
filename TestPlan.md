# Test Coverage Plan

Branch `claude_dauntless_tests`.

> **Picking this up fresh? Jump to [Resume here](#resume-here-state-as-of-2026-07-11)
> at the bottom.** Phases 1–5 are complete; the baseline audit and summary
> table below are the *original* survey and are kept only as a historical
> record of where things started.

## How coverage was measured (original audit)

Every deck has a test file per card, but ~149 of ~250 test files contain only a
`TestModWorks` stub (loads a game, asserts nothing). "Real tests" below counts
`[Test]` methods excluding those stubs. The randomized game tests
(`RandomHeroTests`, `RandomVillainTests`, `RandomEnvironmentTests`,
`RandomGameTest`) give every deck crash/smoke coverage, but no behavioral
assertions — a card whose effect silently does the wrong thing passes them.

## Coverage summary (as of the original audit — now outdated, see Resume here)

| Deck | Real tests | State |
|---|---|---|
| **Heroes** | | |
| Dauntless | 148 | Done |
| Dragon | 158 | Done |
| Grue | 143 | Done |
| Armsmaster | 76 | Done |
| Legend | 72 | Done |
| Labyrinth | 71 | Done (4 single-test files, see below) |
| Skitter | 65 | Done |
| Miss Militia | 60 | Done |
| Tattletale | 59 | Done |
| Battery | 47 | Done |
| Alexandria | 44 | Mostly done (4 single-test files) |
| Bitch | 39 | Partial — dogs + 4 stub cards |
| Jessica Yamada | 28 | Partial — 10 of 12 card files are stubs |
| **Villains** | | |
| Echidna | 39 | Partial — 4 Twisted cards have no test file |
| The Merchants | 26 | Thin — 1–3 tests per card |
| Slaughterhouse 9 | 19 | Partial — 11 stub files |
| Coil | 3 | Effectively untested |
| Behemoth | 0 | Untested |
| Leviathan | 0 | Untested |
| Lung | 0 | Untested |
| The Simurgh | 0 | Untested |
| **Environments** | | |
| Coil's Base | 2 | Effectively untested (only Mercenaries) |
| Brockton Bay | 0 | Untested |
| Kyushu | 0 | Untested |
| New Delhi | 0 | Untested |

## Plan

Continue the established pattern: one branch per deck, one test file per card,
character-card behavior in `<Deck>Tests.cs`, using the decklist JSON card text
as the spec. Replace each `TestModWorks` stub as its file gains real tests
(keep one load-test per deck in the `<Deck>Tests.cs` file).

### Phase 1 — Untested villains (highest risk: they drive whole games)

Suggested order, easiest first:

1. ~~**Lung**~~ DONE (2026-07-02): 30 behavioral tests across all 11 cards +
   character/Brute (flip, trash scaling, damage reduction, advanced).
   `LungTestBase` adds `FillLungTrash`, `RemoveLungTriggers`,
   `RemoveEnvironmentDeck`. Found & fixed a real bug: Bakuda's one-shot branch
   used the revealed card as damage source, which always fizzles. Also fixed
   (2026-08-08): in advanced mode the discard-from-empty-deck reshuffle
   prevented Lung's flip from ever triggering organically — the flip trigger
   now fires on any villain-trash reshuffle, not just necessary-to-play ones.
2. ~~**Leviathan**~~ DONE (2026-07-02): 37 behavioral tests across all 12 cards
   + character (retaliation tokens, flip cycle, tactics, advanced).
   `LeviathanTestBase` adds `PutTacticInPlay` / `MoveTacticsToDeckBottom` /
   `MoveTacticToDeckTop`. Found & fixed a real bug: the advanced-mode
   "Reduce damage dealt to Leviathan by 1" was not implemented on the front
   side. Note: never round-trip an in-play card out of play and replay it in
   tests — its triggers stay dead (AreTriggersActive guard); use
   `ResetTriggers` (see PutTacticInPlay).
3. ~~**Behemoth**~~ DONE (2026-07-02): 34 behavioral tests across the character,
   Hero Tactics (new `HeroTacticsTests.cs`), all villain cards and all six
   movement cards. `BehemothTestBase` adds `Proximity`/`SetProximity`/
   `ClearProximity`, `StackMovementDeck`, `PlayMovementCard`,
   `RemoveBehemothTriggers`. Found & fixed a real bug: played movement cards
   went to the villain trash instead of under Movement Trash (the in-Play()
   move is overridden by the engine's one-shot cleanup; fixed by overriding
   `GetTrashDestination()`), which meant the movement deck could never
   reshuffle and spent movement cards would eventually shuffle into the
   villain deck.
4. ~~**Coil**~~ DONE (2026-07-02): 43 tests — Scheming/Acting magic-text
   thresholds, flip-instead-of-destroy, heroes-win detection, advanced HP
   equalisation/revive, and all cards. `CoilTestBase` adds
   `CleanupSetupNoise` (turn-1 start triggers put a random parahuman and
   environment card into play) and `RemoveCoilTriggers`. Found & fixed a
   real bug: Trainwreck's regeneration was implemented at the END of the
   villain turn but the card says START.
5. ~~**The Simurgh**~~ DONE (2026-07-02): 41 tests — character both sides
   (flip cycle, scream tokens, advanced), all traps (via organic
   A-Plan-Enacted flips), all conditions, play-when-revealed cards, Thinker
   Countermeasures. `SimurghTestBase` adds `PutTrapFaceDownInPlay`,
   `FlipTrapFaceUpDormant` (harness flip + ResetTriggers),
   `FlipTrapWithPlanEnacted` (unique-choice organic flip, no decision
   pollution), `RemoveCountermeasures`. Found & fixed two real bugs:
   A Fate Selected hit the hero with the MOST cards in play (the engine's
   `mostFewestSelectionType` param only changes the decision label), and
   A Countermeasure Defeated played the HIGHEST-danger trash card instead of
   the lowest (`OrderBy(...).Reverse()`).

### Phase 2 — Environments

- ~~**Brockton Bay**~~ DONE (2026-07-02): 19 tests — Uber/Leet duo synergy
  (damage boost + mutual heal), Suburb destroy-others behaviour, Scum scaling,
  Attention reveal-into-play, Civilians/Rooftops discard-to-destroy, damage
  modifiers. `BrocktonBayTestBase` adds `SetupBrocktonBayGame` (Baron Blade
  with MDP/triggers removed as a stable 40 HP top target).
- ~~**Coil's Base**~~ DONE (2026-07-02): 27 tests — structure damage-reduction
  + ablation chains, Blast Doors escalating reduction, Parahuman Prison
  jail/free mechanics, Sealed Chamber game-over + skip-to-heal, Stranger &
  Master Protocols cross-hero isolation, Trapped Chamber, Laser Rifles type
  change. `CoilsBaseTestBase` mirrors the Brockton Bay setup helper.
- ~~**Kyushu**~~ DONE (2026-07-02): 22 tests — Black Kaze stalking/redirect,
  Slide into the Sea (deck attrition, destroy-other-instead, island-sinks
  game over), Collapsing Building discard-to-destroy, environment Lung,
  Sentai scaling, Only the Indomitable Remain, one-shot self-destruct
  pattern. `KyushuTestBase` mirrors the other environment setup helpers.
- ~~**New Delhi**~~ DONE (2026-07-02): 29 tests — Chevalier/Lightning Rod
  redirects, Heroic Sacrifice redirect amplification (nemesis-aware),
  Irradiated token lifecycle (must be played mid-environment-turn or it dies
  at the start-of-turn check before gaining a token — matches real play),
  Phir Se time bomb, Scion lockdown/self-removal, Thanda parahuman removal,
  Unstoppable, Wildfires, Yangban's four typed damages, Accord's Plan,
  Devastation, Perdition, A Chaotic Environment.

### Phase 3 — Fill partial villains

- ~~**Slaughterhouse 9**~~ DONE (2026-07-02): all 11 stub files replaced (16
  new tests). `Slaughterhouse9TestBase` adds `SetupNineGame` (board-only, no
  StartGame — the existing member-test convention) and
  `SetupAndStartNineGame` (random deployment + triggers removed). Found &
  fixed a real bug: They're All Better Now could never revive anyone —
  incapacitated members' flipped sides have no keywords, so
  `DoKeywordsContain("nine")` never matched (fixed via
  `Definition.Keywords`). Test gotchas: the journal doesn't record without
  StartGame (the revive looks up flips in the journal), and deployed
  Bonesaw/Siberian add heal/immunity noise (`ReturnSiberian` +
  post-incap `RemoveVillainTriggers`). Member files deepened 2026-07-11 —
  see Phase 5.
- ~~**Echidna**~~ Twisted gap CLOSED (2026-07-02): new test files for
  `PropagandaTwisted`, `ResistanceTwisted`, `RoutTwisted`,
  `SpearpointTwisted` (8 tests, following the existing
  `ReturnAllTwisted`/`DestroyNonCharacterVillainCards` convention).
  Single-test files for `Bullrush`/`ChimaericalNightmare`/`CloneArmy`/
  `Crush`/`PsychologicalWarfare`/`SquadTactics` remain — optional deepening.
- ~~**The Merchants**~~ DONE (2026-07-11): +19 edge-case tests (45 total).
  Festivals: thug damage boosted/reduced, wrong-direction checks (Blood
  doesn't boost heroes, Excess doesn't shield them), two copies stack,
  Festival of Love on an empty Thug deck is a safe no-op. Not Exactly
  Sanitary: no toxic on self-damage, hero damage, or fully-prevented damage
  (`DidDealDamage` guard), thug end-of-turn damage triggers the rider.
  Immunity scopes: Helicopter protects only Skidmark and only from melee;
  Squealer's environment immunity covers all villain targets but not heroes.
  Thugs: each of Reveller/Sadist/Tough shuffles into the Thug deck when
  destroyed (verified via `AssertAtLocation(card,
  TurnTaker.FindSubDeck("ThugDeck"))`); Tough hits a lone surviving hero
  once for 2 (not twice). No mod bugs found.

### Phase 4 — Hero gaps

- ~~**Jessica Yamada**~~ DONE (2026-07-10). All 10 stub files replaced with real
  tests; the 4 failing tests fixed. Root causes (both test-side):
  `ResilienceAndRespect` targeted Jessica's character card, but the default
  variant isn't a target — fixed by having Legacy hit himself (the
  `SupportAndStability` pattern, which also sidesteps Baron's nemesis bonus).
  `PsychologicalTraining` stacked a filler card for the draw phase to consume,
  but `GoToEndOfTurn`/`GoToPhase` only advance phases and never perform phase
  actions — nothing is ever drawn, so stack ONLY the card under test.
- ~~**Bitch**~~ DONE (2026-07-02): stubs for `Heel`, `Hold`, `TheHunt`,
  `Whistle` replaced (6 tests). Gotcha: `RemoveVillainCards()` must come
  **after** `StartGame()` — Baron Blade's Mobile Defense Platform only enters
  play during StartGame, and otherwise makes him immune. `DogTests` still has
  one shared test covering all 11 dogs — each dog's unique text could use its
  own test (optional deepening).
- ~~**Alexandria**~~ DONE (2026-07-10): deepened `AndTheyKnowMe` (highest-HP
  target selection + effect expiry), `ColdReading` (start-of-turn
  discard/return via `DecisionMoveCardDestination`), `ProstheticEye`
  (reorder — selection order becomes final top-to-bottom order).
  `PureStrength`'s single test already covers its whole card text.
- ~~**Labyrinth**~~ DONE (2026-07-10): deepened `DeviousLabyrinth` (decline
  path), `MightyCastle` (reduction expiry), `TheAsylum` (irreducible
  self-damage vs Defensive Buttress). `Exploration`'s optional draw can't be
  declined in tests — the engine's Smart auto-draw policy answers the optional
  draw yes whenever no draw triggers are in play.
- ~~**Skitter**~~ DONE (2026-07-10): `SweepTheArea` non-target reveal path +
  multi-villain-target damage. Its "return in any order" decision arrives
  pre-`AutoDecided` from the engine, so tests can't choose the order — assert
  contents with `Is.EquivalentTo`, not order.

### Phase 5 — Guardrail + optional deepening (2026-07-11)

- ~~**Meta-test guardrail**~~ DONE, then dropped (2026-08-08) as not worth
  keeping: `Test/CoverageMetaTests.cs` failed if any card
  in the mod decklists (enumerated from the embedded `DeckLists` resources,
  main deck + subdecks) has no fixture containing a non-`TestModWorks` test.
  Mapping rules: `<Identifier>Tests`; Twisted cards drop the `Twisted` suffix;
  character cards may use `<Deck>Tests` or drop a `Character` suffix (team
  members like `JackSlashCharacter` → `JackSlashTests`); dogs map to `DogTests`
  via the `SharedFixtures` dictionary; board pieces (`isReal: false`,
  non-character) are skipped. It immediately caught the one real gap: the
  Slaughterhouse 9 character card itself had no tests. Also surfaced two
  misspelled fixtures, renamed: `ContinousCrackleTests` →
  `ContinuousCrackleTests`, `DissassoctionTests` → `DisassociationTests`.
- ~~**Slaughterhouse 9 character card**~~ DONE: new `Slaughterhouse9Tests.cs`
  (setup deploys H members with the rest under the Nine, flip-instead-of-
  destroy, heroes win when no villain targets — the engine reports
  `EndingResult.VillainDestroyedVictory` — and advanced end-of-turn
  deployment).
- ~~**Bitch per-dog tests**~~ MOOT: all dogs share `BaseDogCardController`
  (identical end-of-turn 1 psychic self-damage; Bastard has no text and 8 HP)
  — there is no unique per-dog text, and `DogTests.TestSelfDamage` already
  covers every dog including Bastard's exemption. Dropped from the to-do.
- ~~**Echidna single-test files**~~ DONE: +6 tests — Bullrush's Engulfed deck
  search, Chimaerical Nightmare ignoring non-environment targets, Clone Army
  villain-turn-only timing (and empty-Twisted-deck grace), Crush with no one
  engulfed, Psychological Warfare hitting non-character hero targets with
  Echidna as source, Squad Tactics not boosting hero/environment damage.
- ~~**Slaughterhouse 9 members**~~ DONE: +19 tests covering every member's
  front-side reactions and flipped sides (JackSlash special/once-per-turn/
  flipped power-punisher, Bonesaw defence-heal/special-toxic/flipped heal,
  Crawler adaptive immunity/attack/regen/flipped, Burnscar special environment
  burn/attack/flipped, Mannequin damage reduction/defence-destroy/flipped,
  Shatterbird H−1 highest attack/flipped, Siberian adjacency immunity/flipped
  melee immunity, Cherish defence-discard). Found & fixed two real bugs:
  flipped **Bonesaw's** end-of-turn heal could never fire (the trigger applied
  a target criteria to the turn taker — `tt.Is(this).Villain().Target()`
  instead of `tt == TurnTaker`), and flipped **Jack Slash** damaged
  `GameController.ActiveTurnTaker` instead of the hero who used the power
  (now `upa.HeroUsingPower`, so out-of-turn power use is punished correctly;
  the test covers the common in-turn path).
- **S9 test gotchas** (new helpers in `Slaughterhouse9TestBase`):
  `ReturnMembersExcept(keep)` puts deployed members back under the Nine,
  `RemoveVillainDeck()` empties deck+trash for deterministic turn crossing,
  and `PutMemberInPlay(id)` (added 2026-07-11) replaces `PlayCard` for member
  character cards. `BaseTest.PlayCard(string)` strips a card's triggers before
  playing it, expecting the play to re-add them; if the random start-of-game
  deployment already put that member into play, the engine refuses the replay
  and the member is left in play with its triggers permanently dead. This made
  every started-game member test flaky (~1 in 5 for CherishTests'
  TestDefenceMakesTheHeroWithTheMostCardsDiscard, seed -380216145 reproduces).
  All 53 member-character `PlayCard` calls in the S9 tests now use
  `PutMemberInPlay`.
  CRITICAL: the heroes-win trigger ends the game on the next real action
  whenever zero villain targets are in play — always play the member under
  test *before* returning the others, and keep a second villain target
  (Spiderbots, or Hatchet Face for end-of-turn tests since he has no
  end-of-turn action) in play before flipping the last member. Crawler's
  adaptive immunity is tracked via card properties, which only record in a
  started game's journal.

## Resume here (state as of 2026-07-11)

**Milestone: ALL planned test work is done.** Phases 1–5 are complete: zero
stub-only files, the S9 character card and all eight members have behavioral
tests (front and flipped sides), and the Echidna single-test files are
deepened. Two more real mod bugs were found & fixed this session (flipped
Bonesaw heal, flipped Jack Slash power damage) — see Phase 5. The final
optional item — The Merchants edge-case deepening — is also done (+19 tests,
see Phase 3), leaving committing the branch as the only remaining task.

Everything is uncommitted on branch `claude_dauntless_tests` (~200 changed/new
files). Nothing has been pushed.

Last full-suite run (after the Merchants deepening and the S9 flake fix):
**1748 passed, 0 failed, 1 skipped (1749 total)**.

### Known pre-existing flakiness (NOT caused by this work)

Roughly 1 in 3 full-suite runs shows a failure from the seeded random tests —
`TestEnvRandomWithGuise`, `TestEnvRandomWithCompletionistGuise`,
`TestRandomWithPWTempest`, `TestRandomWithTribunal`, and the two `TestLimited`
tests (`BatteryCauldronCapeTests`, `CarryTheChargeTests`). Each passes on
rerun. A background task chip was spawned to make these deterministic.

A second flake source — the started-game S9 member tests dying whenever the
random deployment pre-deployed the member under test (via the
`BaseTest.PlayCard` trigger-strip, see the S9 gotchas in Phase 5) — was
identified and FIXED on 2026-07-11 (`PutMemberInPlay`). It was introduced with
the Phase 5 member deepening, not pre-existing.

### What's left

- **Committing this work** — nothing on the branch is committed yet.
- The known random-seed flakes above remain (background task chip spawned).

### Coverage check

Finished decks keep one `TestModWorks` load test in the deck's main test file.
The shell one-liner below is the quick manual check for stub-only files (a
`CoverageMetaTests.cs` meta-test enforced this automatically for a while, but
was dropped 2026-08-08 as not worth keeping):

```bash
grep -rl TestModWorks Test/ | xargs grep -cE '\[Test\(\)?\]' | grep ':1$'
```

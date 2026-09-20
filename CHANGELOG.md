Version 0.23.6
---

Lots of bug fixes

**Heroes**

* Bitch: discarding a card with Find wasn't triggering on-discard effects.
* Labyrinth: if Devious Labyrinth destroyed itself cards it had blanked would never un-blank.
* Legend: interacted badly with power copying effects.
* Lots of fixes for Representative of Earth:
  * Miss Militia: Protectorate Captain crashed on play.
  * Grue: didn't have any Darkness cards.
  * Battery: Charge and Discharge applied to the wrong card, and discharge powers weren't recognised as discharge powers.
  * Skitter: Crashed on power use.
  * Tattletale: Crashed on power use.
  * Legend: Damage source on his effect was wrong.
  * Labyrinth: Turns out "your turn happens after the environment" does not work well if you are the environment. This now does nothing if she's Representative.

**Villains**

* Coil: Trainwreck regained HP at the end of the villain turn instead of the start.
* Leviathan: advanced mode damage reduction didn't actually exist.
* Lung: advanced Lung's end-of-turn discard can now flip him.
* Lung: If Bakuda revealed a One-Shot the One-Shot would do the fire damage.
* The Simurgh: A Fate Selected damaged the hero with the most cards in play instead of the fewest.
* The Simurgh: A Countermeasure Defeated played the revealed card with the highest danger instead of the lowest.
* Slaughterhouse 9: They're All Better Now just didn't work.
* Slaughterhouse 9: Jack Slash dealt his melee damage to whoever's turn it was rather than to the hero who used the power.
* Slaughterhouse 9: Cherish could crash if a tie-break decision was refused.
* Behemoth: Leap could give proximity tokens to the wrong two heroes after a refused tie-break.
* Behemoth: Incinerate and Behemoth's end-of-turn damage crashed if a hero was incapacitated part-way through the effect.
* Behemoth: used movement cards ended up in the villain trash.
* Echidna: Engulfed interacted badly with Guise's "Uh, Yeah, I'm That Guy!".

**Known behaviour**

* If Grue is Representative of Earth and plays a Darkness card the card persists until the end of the game. In my opinion this is the card operating as-written - the card leaves "at the end of Grue's next turn", and Grue doesn't have a turn.

Version 0.1.0
---

* Added Bitch, who plays dogs and orders them to attack villains. Still TODO: Her variants, some informative text on cards, and her bio.
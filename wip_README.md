# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

> Below sections are sorted by priority

-   [JumpRoyale](#jumproyale)
    -   [Character Choice](#character-choice)
    -   [Create the Arena](#create-the-arena)
    -   [Fireballs (Twitch command)](#fireballs-twitch-command)
    -   [Aim command](#aim-command)

---

## Character Choice

Old codebase had a hardcoded maximum value, this should be delegated to the sprite handler class, that should automatically calculate and store the maximum amount of possible characters.

---

## Create the Arena

Now with the Arena Builder in place, it would be a good test to research the drawing methods. Previously, a regular Random placement was being used, which was not bad by any means, but there are also other techniques to try: Perlin Noise.

While Perlin Noise is very good for creating terrains, it should also be tested if it can do any good job with putting Platforms on the arena. There are also other noise functions to try:

-   Discrete
-   Tricubic
-   Perlin (main goal)
-   Simplex
-   Spots
-   Worley

The initial idea is to use Perlin Noise for platforms and possibly mix it with another one for solid blocks, like Spots noise.

---

## Fireballs (Twitch command)

Note from Adam:

```plaintext
🔥 FIREBALLS 🔥
Post-prototype work:
- Make shooting a fireball be a command
- Add a cooldown of ~2 seconds? I don't want too much spam
- Make it only usable at the end of the game
```

Initial design threw a fireball, which was affected by gravity and could collide with other players, pushing them back with some force.

---

## Aim command

Note from `Smu`:

> jump royale idea: ar30 to aim 30 degrees right. It's like jumping, except there is no jump, it just draws a square function that follows your jump trajectory (without collision). It should disappear after ~5 seconds.

Approx functionality:

-   copy the Jump logic for the angle calculation (`j`-> `-90` - `90`)
-   allow executing the command in the following format:
    -   `aim` `<direction> <angle> [power]`
    -   `a` `<direction> <angle> [power]`

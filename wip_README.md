# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

> Below sections are sorted by priority

-   [JumpRoyale](#jumproyale)
    -   [Create the Arena](#create-the-arena)
    -   [Character Choice](#character-choice)
    -   [Fireballs (Twitch command)](#fireballs-twitch-command)
    -   [Aim command](#aim-command)

---

## Create the Arena

The previous (noise generation) idea is not really worth implementing in such simple game, regular, random generation is totally fine here, so moving on.

TODOs:

-   migrate from vectors to regular `int`s...
-   change sprite every 1500 height (*)
-   make a range of random platform length depending on the current drawing-Y

> (*) in regular games, we were able to exceed 5000 height, but on average, the height was around 3500, so it would make sense to change every 1500 or 2000 height.

Changing the sprite could be done either by the Builder or inside the Arena, but I don't really feel adding this into the Builder would fit well as this kind of defeats the purpose of having a "general purpose" drawing class, but since the constructor already has the "hardcoded" tile population, it should not be the problem. Just keep the "configuration" in the constructor and automatically get the drawing tile from a dictionary, where the tile depends on the provided Y, since we provide the coordinates where to draw anyway.

---

## Character Choice

Old codebase had a hardcoded maximum value, this should be delegated to the sprite handler class, that should automatically calculate and store the maximum amount of possible characters.

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

# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

> Below sections are sorted by priority

-   [JumpRoyale](#jumproyale)
    -   [Character Choice](#character-choice)
    -   [Fireballs (Twitch command)](#fireballs-twitch-command)
    -   [Aim command](#aim-command)

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

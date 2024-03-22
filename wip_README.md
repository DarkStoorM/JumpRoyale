# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

> Below sections are sorted by priority

- [JumpRoyale](#jumproyale)
  - [Create the Arena](#create-the-arena)
  - [Character Choice](#character-choice)
  - [Make a new background](#make-a-new-background)
  - [Fireballs (Twitch command)](#fireballs-twitch-command)
  - [Aim command](#aim-command)

---

## Create the Arena

-   Check if it's possible to write tests for Arena Builder, because it references Godot's API and it might just crash, which would probably require those references to be removed and injected at init-phase.

---

## Character Choice

Old codebase had a hardcoded maximum value, this should be delegated to the sprite handler class, that should automatically calculate and store the maximum amount of possible characters.

---

## Make a new background

In the old codebase there were 7 (? :thinking:) different backgrounds, chosen randomly. Find out if it's going to look better if the backgrounds are darker, possibly create something completely new.

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

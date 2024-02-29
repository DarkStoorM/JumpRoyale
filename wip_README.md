# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

> Below sections are sorted by priority

-   [JumpRoyale](#jumproyale)
    -   [Arena Builder](#arena-builder)
    -   [Character Sprite handler](#character-sprite-handler)
    -   [Character Choice](#character-choice)
    -   [Fireballs (Twitch command)](#fireballs-twitch-command)

---

## Arena Builder

Arena Builder is a separate class, which draws sprites on a TileMap. This class should provide a set of methods for different drawing methods:

```cs
DrawPoint(Vector2 startingPoint);
DrawLineHorizontally(Vector2 startingPoint, int length);
DrawLineVertically(Vector2 startingPoint, int length);
DrawSquare(Vector2 startingPoint, int size, bool shouldFill = false);
DrawRectangle(Vector2 startingPoint, Vector2 EndingPoint, bool shouldFill = false);
```

The methods should be really easy to use by external components, reducing the repeated code with the requirement of knowing the location of a sprite in the tileset. The Arena Builder should aim to reduce this step as much as possible, which possibly should only require us to specify the platform type and drawing method, the builder should handle the rest automatically with a single call, not three (platform start, middle, end).

There could potentially be an issue of this not being Testable, since there is nothing to store really and it references Godot's API (probably), which will crash the tests, but it has to be researched first. It would also be really useful if we could check the type of drawn cell at position to allow `Drop` command only for certain platforms.

---

## Character Sprite handler

The old sprite handler was already a separate singleton and it should already work when dropped in from the old codebase, but it should be reviewed first for possible refactors, mainly the resource path extraction.

There should also be a possibility of expanding this class if `privileged` cosmetics are added, like special characters - currently, there are 18 characters (3 clothings, 3 different characters, 2 genders).

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

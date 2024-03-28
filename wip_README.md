# JumpRoyale

Placeholder work-in-progress readme file, which will eventually be updated.

-   [JumpRoyale](#jumproyale)
    -   [Character Choice](#character-choice)
    -   [Create the Arena](#create-the-arena)
    -   [Make a new background](#make-a-new-background)
    -   [Fireballs (Twitch command)](#fireballs-twitch-command)
    -   [Aim command](#aim-command)
    -   [New sprites](#new-sprites)
    -   [Benchmarking](#benchmarking)

---

## Character Choice

~~Old codebase had a hardcoded maximum value, this should be delegated to the sprite handler class, that should automatically calculate and store the maximum amount of possible characters.~~

We don't need this, since the characters will never change anyway, so there is no need to automatically calculate how many character combinations there are.

In order to properly test this, I need to spawn the player in the same area as we do in the live version, that is, spawning the player midair, which should trigger the Fall animation first, but to do that, I need to switch focus on another feature, which is calculating the arena field properly.

I have the arena size, drawing Start/End points on X axis, but in order to get the Y component, I **have to** get the current position, which should be taken from the Camera, not the stored viewport (I think? :thinking: ). The viewport is not moving, we operate on the camera instead in the live version, so I have to do the same thing here, which should be easier since I already have a separate Camera scene with its own script, but I need to figure out how to get this out in the code (It's a packed scene)

-   Figure out how to store the camera instantiated inside Arena scene

---

## Create the Arena

1)   Print Jumper's current height
2)   Measure the real-world achievable height in the new system

~~The new height has to be measured, because with 6000 height, there might be not enough variety when new sprites are considered. Although, 6k sounds like impossible height to climb on a single session, it just has to be checked due to the different block generation system.~~

The height has been set to 6400, which evaluates to 400 tiles, which then divides into even 10 difficulty levels of 40 tiles.

(2) While I would make a good use of this right now, this requires to implement the variable camera movement first and possibly the camera speed per game difficulty, which is a different difficulty level than the current arena height. This depends on the timer, which changes the camera speed every [x] seconds.

Since I already have a dictionary initialization for a range of values, I could also make something similar with the camera speed. The game time is 150 seconds, so for example:

|    current time    | camera speed |
| :----------------: | :----------: |
|         0          |     4px      |
|         30         |     8px      |
|         60         |     16px     |
|         90         |     32px     |
| 120 (final change) |     64px     |

Which, in the end, is just $4*n ^{(n-1)}$

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

---

## New sprites

The currently imported sprites come from a paid asset, so it would be bad to create something completely new. It was not much though, but still.

Currently, we only have 3 sprites for platforms, but there 5 potential sprites that can be used for Walls (solid objects, not pass-through).

There is a possibility for me to make new sprites for platforms, as I know some pixel art, and adjusting a duplicate, changed sprite to the current style should not be that hard.

Also, currently, the sprite change depends on the height the objects are being drawn at, and since we only have three sprites, we change that every third of the arena height. This does not create any good variety, so I would like to aim more at something that resembles Icy Tower's *system*, where new floor has a different sprite up to 1000, which gives 10 different sprites.

Since the average height we get on livestreams is around 4k (250 tiles), it creates a small problem of picking the optimal height for changing the sprites. The screen is 67.5 tiles tall (67) and I would like to see at least 10 sprite changes, if possible, but probably our average height will not allow that, but...

The old system was creating huge blocks at around 3500 height, which made is impossible to progress and the new system literally makes one block per row with certain chance, which reduces the amount of big blocks and potentially opens more possibilities for top height. It comes with a small catch, though: the player stats will now change, because we may potentially climb way higher, increasing the total score. Should we care anyway?

With higher average height, we might get a better opportunity for different sprites per "level", but we also might end up with more hardcoding of the values, BUT... if all Walls and Platforms will have their own sprites, the change in tile types will be equal and it can be just calculated and automatically changed, so only the Arena Builder has to have the tile types populated in the correct order. This might even be totally fine to change sprites per one screen.

If we change the sprite once per screen, which is 68 tiles (+1 to we change it off screen), we get to change it 5 times:

0 -> 68 -> 136 -> 204 -> 272 -> 340

0, being the default sprite, so we can get 6. We could change it more frequently, but I need to measure what is the actual achievable height, because right now it looks impossible to even reach the fourth change (4352px height).

[Edit]
After some tests on bigger screen, the new platforms seem like they are too thin... they will have to be bigger, otherwise this might look really bad on stream.

---

## Benchmarking

In the old codebase, there is a method that takes all payers, sorts them by height and takes the top one. While this does not sound any harmful at all in regards to the performance, since this is a simple sort, there is also one small thing to check, which is how it actually performs if there are many players.

So far, we have tested this on raids with a couple hundred players and while nothing noticeable was happening, it almost felt like the game was losing in puts (?).

Sadly, Godot's profiler is not as good as Unity's, so I will have to rely on simple deltas reports for methods :thinking:

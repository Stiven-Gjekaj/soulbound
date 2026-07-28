# The Time object

### The Time object [E/M/W]

The Time object serves as a way to retrieve game timing without having to keep track of it
yourself, or using a frame counter.

**Read this before timing anything.** A wave's `Update` runs exactly sixty times a second
whatever the frame rate is doing, because the fight advances on a fixed step rather than
once per rendered frame. Counting your own `Update` calls is therefore the most reliable
clock a pattern has, and movement written as an amount per `Update` is already the same on
every machine. The entries below say which of these follow the fight and which follow the
renderer, because they are no longer the same thing.

- **number** `Time.time` (readonly) - Time (in seconds) since the game application
  started. If you want to time specific events, store `Time.time` in a variable of your
  own at the start of what you want to time, then subtract `Time.time` from your stored
  time to calculate the difference.
- **number** `Time.dt` (readonly) - Delta time (in seconds): how long the last **rendered
  frame** took. This follows the renderer, not the fight. One step of the fight is always
  a sixtieth of a second regardless of what this says.
- **number** `Time.mult` (readonly) - The same thing as a multiple of a sixtieth of a
  second (essentially `deltatime*60`). Around 1.0 at 60FPS, ~0.5 at 120FPS, ~2.0 at 30FPS.

  **Do not multiply your movement by this.** It used to be the way to keep a pattern moving
  at the same speed on a slow machine, back when a wave's `Update` ran once per rendered
  frame and a 30FPS machine therefore ran the wave half as many times. `Update` now runs
  sixty times a second on every machine, so movement per `Update` is already equal
  everywhere, and scaling it by this would make your pattern move *faster* on a slow
  machine rather than the same.

  It is still worth reading for what it now means: how hard the renderer is working.
- **number** `Time.dropped` (readonly) - Steps the fight has thrown away since it began
  because the machine could not keep up. Zero on any machine holding a reasonable frame
  rate.

  A frame slow enough to owe more than the catch-up ceiling drops the backlog rather than
  repaying it, because repaying it would make the next frame slower still. When that
  happens the fight skips forward: bullets do not move for those steps and no collision is
  tested for them. If you are investigating a pattern that behaves differently on one
  machine, this is the number that says whether that machine was skipping steps.
- **number** `Time.wave` (readonly) - Returns the elapsed time (in seconds) of the
  current waves while in `DEFENDING`. -1 outside of `DEFENDING`.

  You can use `wavetimer - Time.wave` while in `DEFENDING` to see how much time is left in
  the defense round.
- **number** `Time.frameCount` (readonly) - The total number of **rendered frames** since
  the start of the encounter. This follows the renderer, so it counts differently on
  different machines. Count your own `Update` calls instead if you want a number that does
  not.
- **number** `Time.timeScale` - The scale at which time passes. When timeScale is
  1.0, time passes as fast as real time. When timeScale is 0.5 time passes 2x slower than
  realtime.

  Note: You should examine a timeScale test mod to see how to properly implement waves to
  make them work with this.

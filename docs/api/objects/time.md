# The Time object

### The Time object [E/M/W]

The Time object serves as a way to retrieve game timing without having to keep track of it
yourself, or using a frame counter. Whenever possible, try using the Time object over a
frame-based timing method to ensure equal behaviour across all framerates.

- **number** `Time.time` (readonly) - Time (in seconds) since the game application
  started. If you want to time specific events, store `Time.time` in a variable of your
  own at the start of what you want to time, then subtract `Time.time` from your stored
  time to calculate the difference.
- **number** `Time.dt` (readonly) - Delta time (in seconds). This is the time it took for
  the last game update to complete.
- **number** `Time.mult` (readonly) - Multiplier to ensure equal movement across all
  framerates (this is essentially `deltatime*framerate`). This will be around 1.0 when the
  application runs at 60FPS, ~0.5 at 120FPS and ~2.0 at 30FPS, and so on. By multiplying
  your movement by this value, your waves will be consistent on lower framerates as well.
- **number** `Time.wave` (readonly) - Returns the elapsed time (in seconds) of the
  current waves while in `DEFENDING`. -1 outside of `DEFENDING`.

  You can use `wavetimer - Time.wave` while in `DEFENDING` to see how much time is left in
  the defense round.
- **number** `Time.frameCount` (readonly) - The total number of frames since the
  start of the encounter.
- **number** `Time.timeScale` - The scale at which time passes. When timeScale is
  1.0, time passes as fast as real time. When timeScale is 0.5 time passes 2x slower than
  realtime.

  Note: You should examine a timeScale test mod to see how to properly implement waves to
  make them work with this.

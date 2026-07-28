using System;
using UnityEngine;

/// <summary>
/// The fixed clock the whole fight advances on.
///
/// Before this, the fight advanced once per rendered frame while the player moved by
/// elapsed time. A machine holding 30fps therefore ran a four second wave 120 times
/// instead of 240, so every bullet written as movement per update covered half the
/// ground while the soul kept its full speed. The fight was not slower on slow hardware,
/// it was easier.
///
/// Everything that decides what happens in a fight now advances in whole steps of
/// <see cref="Step"/>: the player, the encounter's Update, the wave scripts, and
/// projectile movement and collision. Rendering still happens per frame and simply shows
/// the last step's state.
///
/// A fixed tick was chosen over scaling movement by elapsed time because the Lua API is
/// public and its patterns are written in terms of "per Update". Delta-time scaling would
/// silently change the speed of every pattern ever written; this leaves them meaning
/// exactly what they meant.
/// </summary>
public static class BattleTick {
    /// <summary>Seconds of game time one step advances. The game also renders at 60.</summary>
    public const float Step = 1f / 60f;

    /// <summary>
    /// Most steps one frame may run. A frame that takes long enough to owe more than this
    /// drops the backlog rather than repaying it: catching up without a ceiling turns one
    /// slow frame into a slower one, which owes more steps again.
    /// </summary>
    private const int MaxCatchUp = 5;

    private static float accumulator;
    private static int   ticks;

    /// <summary>Steps run since the fight began.</summary>
    public static int Ticks { get { return ticks; } }

    /// <summary>
    /// Game time since the fight began. This is what a fight is timed by: it counts what
    /// the fight actually did rather than how long the player sat there, so a machine that
    /// stutters does not post a slower time for the same work.
    /// </summary>
    public static float Elapsed { get { return ticks * Step; } }

    /// <summary>Starts a new fight's clock.</summary>
    public static void Reset() {
        accumulator = 0f;
        ticks       = 0;
    }

    /// <summary>
    /// Runs <paramref name="step"/> once per whole Step owed since the last call.
    ///
    /// Time.deltaTime is already unscaled time multiplied by Time.timeScale, so a boss
    /// that slows time down slows the fight and this clock together. That keeps a recorded
    /// time honest: it measures game time either way.
    /// </summary>
    public static void Advance(Action step) {
        accumulator += Time.deltaTime;

        int ran = 0;
        while (accumulator >= Step) {
            if (ran >= MaxCatchUp) {
                accumulator = 0f;
                break;
            }
            accumulator -= Step;
            ran++;
            ticks++;
            step();
        }
    }
}

using UnityEngine;

/// <summary>
/// Lua binding to retrieve engine timing information. Not marked as static so Lua may have an instance to refer to. Silly, I know.
/// </summary>
public class LuaUnityTime {
    /// <summary>
    /// Time in seconds since the application started.
    /// </summary>
    public static float time { get { return Time.time; } }

    /// <summary>
    /// The time in seconds it took to complete the last update.
    /// </summary>
    public static float dt { get { return Time.deltaTime; } }

    /// <summary>
    /// How long the last rendered frame took, as a multiple of a sixtieth of a second.
    /// 1 at 60FPS, 2 at 30FPS, 0.5 at 120FPS.
    ///
    /// Do not multiply movement by this. It used to be the way to keep a pattern moving at
    /// the same speed on a slow machine, back when a wave's Update ran once per rendered
    /// frame. Update now runs exactly sixty times a second whatever the frame rate is
    /// doing, so movement per Update is already the same everywhere and scaling it by this
    /// would make a pattern faster on a slow machine rather than equal.
    ///
    /// It is still worth reading: it tells a script how hard the renderer is working.
    /// </summary>
    public static float mult { get { return Time.deltaTime * 60; } }

    /// <summary>
    /// Steps the fight has thrown away since it began because the machine could not keep
    /// up. Zero on any machine holding a reasonable frame rate.
    ///
    /// A frame that takes long enough to owe more than the catch-up ceiling drops the
    /// backlog instead of repaying it, because repaying it would make the next frame slower
    /// still. When that happens the fight skips forward: bullets do not move for those
    /// steps and no collision is tested for them. This is the number that says it happened.
    /// </summary>
    public static float dropped { get { return BattleTick.Dropped; } }

    public static float timeScale {
        get { return Time.timeScale; }
        set { Time.timeScale = value; }
    }

    public static float wave {
        get { return UIController.instance.state != "DEFENDING" ? -1f : Time.time - UIController.instance.encounter.waveBeginTime; }
    }

    public static float frameCount { get { return GlobalControls.frame; } }
}
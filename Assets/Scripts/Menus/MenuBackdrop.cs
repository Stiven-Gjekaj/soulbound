using UnityEngine;

/// <summary>
/// Moves the menu's background painting: a slow drift it does on its own, and a pan that
/// follows the pointer.
///
/// The pointer here is not input. Nothing in the menus can be clicked or hovered, and this does
/// not change that: it leans the picture the way looking around a room does, and the keyboard
/// still decides everything.
///
/// Two things about how it moves are worth knowing.
///
/// The pan is smoothed rather than followed, through a critically damped spring, so it eases
/// into motion and eases out of it instead of tracking the pointer rigidly. The curve below
/// shapes how far the pointer's distance from the middle is felt, so the middle of the screen
/// is calm and the edges pull harder.
///
/// And nothing is snapped to whole pixels. An earlier version rounded every offset to an even
/// number, because the backdrop is drawn at twice its sprite and a position between two source
/// pixels samples across the boundary. That is true, but at these speeds it meant the picture
/// stood still for seconds and then jumped two pixels, which reads worse than the sampling
/// does. Smooth motion wins here; the sprites that have to stay on the grid are the ones the
/// player reads, and this is a wall.
/// </summary>
public class MenuBackdrop : MonoBehaviour {
    public RectTransform backdrop;

    [Header("The drift it does on its own")]
    public float driftX = 14f;
    public float driftY = 9f;
    /// <summary>Seconds for one pass across the horizontal drift.</summary>
    public float period = 47f;

    [Header("The pan that follows the pointer")]
    public float panX = 16f;
    public float panY = 12f;
    /// <summary>Roughly how long the pan takes to catch up. Higher is heavier.</summary>
    public float panSmoothing = 0.35f;

    /// <summary>
    /// How the pointer's distance from the middle turns into pan. Eased at both ends, so the
    /// middle of the screen barely moves the picture and the edges do the work.
    /// </summary>
    public AnimationCurve panCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector2 lean;
    private Vector2 leanVelocity;

    private void Update() {
        if (backdrop == null)
            return;

        float t = Time.unscaledTime;
        Vector2 drift = new Vector2(Mathf.Sin(t / period * 2f * Mathf.PI) * driftX,
                                    Mathf.Cos(t / (period * 1.37f) * 2f * Mathf.PI) * driftY);

        // The window is scaled, so the pointer is measured against the size actually being
        // displayed rather than against 640x480.
        Vector2 size = ScreenResolution.displayedSize;
        Vector2 target = Vector2.zero;
        if (size.x > 0f && size.y > 0f) {
            Vector2 mouse = ScreenResolution.mousePosition;
            float nx = Mathf.Clamp(mouse.x / size.x * 2f - 1f, -1f, 1f);
            float ny = Mathf.Clamp(mouse.y / size.y * 2f - 1f, -1f, 1f);
            // Negated: pushing the pointer right should look like turning right, which moves
            // the picture left.
            target = new Vector2(-Shape(nx) * panX, -Shape(ny) * panY);
        }

        lean = Vector2.SmoothDamp(lean, target, ref leanVelocity,
                                  Mathf.Max(0.01f, panSmoothing), Mathf.Infinity, Time.unscaledDeltaTime);

        backdrop.anchoredPosition = Clamp(drift + lean);
    }

    /// <summary>Runs the curve over the magnitude, keeping the sign.</summary>
    private float Shape(float v) {
        return Mathf.Sign(v) * panCurve.Evaluate(Mathf.Abs(v));
    }

    /// <summary>
    /// Holds the offset inside the overscan, so no combination of drift and pan can pull an
    /// edge of the painting onto the screen.
    ///
    /// The limit is measured from the rects rather than assumed from 640x480. The canvas
    /// scales with the window, and in fullscreen it does not necessarily report the size the
    /// layout was authored against, so arithmetic against a constant is exactly the thing that
    /// lets an edge show.
    /// </summary>
    private Vector2 Clamp(Vector2 offset) {
        RectTransform parent = backdrop.parent as RectTransform;
        if (parent == null)
            return offset;

        float slackX = Mathf.Max(0f, (backdrop.rect.width  - parent.rect.width)  * 0.5f);
        float slackY = Mathf.Max(0f, (backdrop.rect.height - parent.rect.height) * 0.5f);
        return new Vector2(Mathf.Clamp(offset.x, -slackX, slackX),
                           Mathf.Clamp(offset.y, -slackY, slackY));
    }
}

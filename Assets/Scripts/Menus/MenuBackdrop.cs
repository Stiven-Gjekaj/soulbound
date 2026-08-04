using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Moves the menu's background painting: a slow drift it does on its own, and a small parallax
/// that follows the pointer.
///
/// The pointer here is not input. Nothing in the menus can be clicked or hovered, and this does
/// not change that: it leans the picture a little, the way looking around a room does, and the
/// keyboard still decides everything.
///
/// The backdrop is drawn at twice its sprite's size, so one source pixel is two on screen and
/// any position between them samples across the boundary and shimmers. Every offset is snapped
/// to an even number for that reason: the image moves in whole source pixels or not at all. It
/// is the same argument the arena border is drawn under, that pixel art has to land on the grid
/// it was drawn for.
///
/// Drift and parallax share the overscan, so their limits are set so that both at once still
/// cannot pull an edge onto the screen.
/// </summary>
public class MenuBackdrop : MonoBehaviour {
    public RectTransform backdrop;

    [Header("The drift it does on its own")]
    public float driftX = 16f;
    public float driftY = 10f;
    /// <summary>Seconds for one pass across the horizontal drift.</summary>
    public float period = 47f;

    [Header("The lean that follows the pointer")]
    public float parallaxX = 14f;
    public float parallaxY = 11f;
    /// <summary>How quickly the lean catches up, per second. Low is heavy.</summary>
    public float parallaxEase = 4f;

    private Vector2 lean;

    private void Update() {
        if (backdrop == null)
            return;

        // Unscaled, because a menu should keep breathing whatever the game has done to the
        // time scale, and nothing here is simulation.
        float t = Time.unscaledTime;
        float dx = Mathf.Sin(t / period * 2f * Mathf.PI) * driftX;
        float dy = Mathf.Cos(t / (period * 1.37f) * 2f * Mathf.PI) * driftY;

        // The window is scaled, so the pointer has to be measured against the size actually
        // being displayed rather than against 640x480.
        Vector2 size = ScreenResolution.displayedSize;
        Vector2 target = Vector2.zero;
        if (size.x > 0f && size.y > 0f) {
            Vector2 mouse = ScreenResolution.mousePosition;
            // -1 at one edge, 1 at the other, and negated: pushing the pointer right should
            // look like turning to the right, which moves the picture left.
            float nx = Mathf.Clamp(mouse.x / size.x * 2f - 1f, -1f, 1f);
            float ny = Mathf.Clamp(mouse.y / size.y * 2f - 1f, -1f, 1f);
            target = new Vector2(-nx * parallaxX, -ny * parallaxY);
        }

        // Eased rather than followed, so a flicked pointer does not snap the picture across.
        lean = Vector2.Lerp(lean, target, 1f - Mathf.Exp(-parallaxEase * Time.unscaledDeltaTime));

        Vector2 offset = new Vector2(dx, dy) + lean;
        backdrop.anchoredPosition = new Vector2(Mathf.Round(offset.x / 2f) * 2f,
                                                Mathf.Round(offset.y / 2f) * 2f);
    }
}

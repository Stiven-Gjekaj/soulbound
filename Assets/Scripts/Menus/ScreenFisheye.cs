using UnityEngine;

/// <summary>
/// Runs the finished frame through the fisheye shader, so the whole menu is curved rather
/// than a few elements being arranged as though they were.
///
/// This replaces an earlier attempt that leaned each element by hand: the lower row a little
/// nearer the middle, the logo shifting further under the pointer than the entries. That was
/// chosen to keep the text pixel exact, and it is pixel exact, and it is also too quiet to
/// notice. A curve you have to be told about is not a curve.
///
/// The cost is real and was accepted rather than avoided. Every glyph on this screen is now
/// resampled, because bending a frame means reading it at positions that are not whole pixels.
/// Point sampling is what makes that bearable: it keeps the hard edges and lets the warp show
/// as the pixel grid itself bending, the way a CRT bends it, instead of smearing the text into
/// something soft. Bilinear here would look like a blurred screenshot.
///
/// The canvas has to be ScreenSpaceCamera for any of this to happen. An Overlay canvas is
/// drawn after the camera is finished rather than through it, so it never reaches an image
/// effect, and the menu would curve everything except the part anyone looks at.
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenFisheye : MonoBehaviour {
    public Material material;

    /// <summary>
    /// How hard the frame bends. Positive bulges the middle toward the viewer and curves the
    /// edges away, which is the barrel a CRT has and the one this screen wants. Negative is
    /// the same curve inverted, the middle sinking away and the edges standing forward.
    /// </summary>
    [Range(-0.9f, 0.9f)] public float strength = 0.25f;

    /// <summary>How much darker the corners get. A lens loses light at its edges.</summary>
    [Range(0f, 1f)] public float vignette = 0.28f;

    /// <summary>
    /// Point rather than bilinear, so the warp reads as the pixel grid bending rather than as
    /// the picture being smeared. On a game drawn at 640x480 this is the difference between
    /// the effect looking deliberate and looking like a mistake.
    /// </summary>
    public bool pointSample = true;

    private Material instance;

    /// <summary>
    /// A copy, never the asset. The shader's numbers are written every frame, and writing them
    /// into the shared material would edit the asset on disk: harmless at runtime, but this
    /// component also runs in the editor so the screenshots show the curve, and there it would
    /// leave the project dirty every time a scene was opened.
    /// </summary>
    private Material Instance() {
        if (instance == null && material != null) {
            instance = new Material(material);
            instance.hideFlags = HideFlags.HideAndDontSave;
        }
        return instance;
    }

    private void OnDisable() {
        if (instance == null)
            return;
        if (Application.isPlaying) Destroy(instance);
        else                       DestroyImmediate(instance);
        instance = null;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dst) {
        Material m = Instance();
        if (m == null) {
            Graphics.Blit(src, dst);
            return;
        }

        if (pointSample)
            src.filterMode = FilterMode.Point;

        m.SetFloat("_Strength", strength);
        m.SetFloat("_Vignette", vignette);
        // From the frame rather than from Screen, because in the editor's screenshot path the
        // camera renders into a texture whose shape is not the window's.
        m.SetFloat("_Aspect", (float)src.width / Mathf.Max(1, src.height));

        Graphics.Blit(src, dst, m);
    }
}

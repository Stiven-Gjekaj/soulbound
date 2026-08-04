using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The specks that drift down the menu.
///
/// They carry no meaning and nothing reads them. They exist because a menu of four words
/// over black is a still image, and a still image looks broken rather than calm: without
/// something moving there is no way to tell the game from a screenshot of it. The specks
/// are the cheapest way to say the screen is alive, which is why they are kept faint
/// enough to be noticed only when looked for.
///
/// The specks are built here rather than laid out in the scene. Forty objects placed by
/// hand would be forty things to keep aligned every time the count or the speed changes,
/// and none of them is addressed by anything.
/// </summary>
public class MenuMotes : MonoBehaviour {
    public Sprite mote;

    public int count = 44;

    /// <summary>Pixels a second. The range is wide on purpose: uniform speed reads as a grid falling.</summary>
    public float slowest = 8f;
    public float fastest = 26f;

    /// <summary>The brightest a speck gets. Everything else is dimmer than this.</summary>
    public float opacity = 0.3f;

    /// <summary>How far a speck wanders sideways, and how long one wander takes.</summary>
    public float driftX = 9f;
    public float driftPeriod = 4.5f;

    private RectTransform area;
    private RectTransform[] specks;
    private float[] speed, phase, wander;
    private Image[] inks;

    private void Start() {
        area = transform as RectTransform;
        if (area == null || mote == null) {
            enabled = false;
            return;
        }

        specks = new RectTransform[count];
        inks   = new Image[count];
        speed  = new float[count];
        phase  = new float[count];
        wander = new float[count];

        for (int i = 0; i < count; i++) {
            GameObject go = new GameObject("Mote", typeof(Image));
            Image ink = go.GetComponent<Image>();
            ink.sprite        = mote;
            ink.raycastTarget = false;
            // Dimmer at the top of the range than the bottom, so the field has depth
            // rather than being one sheet of specks at one distance.
            float near = Random.value;
            ink.color = new Color(1f, 1f, 1f, opacity * Mathf.Lerp(0.25f, 1f, near * near));

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(area, false);
            rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot     = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(mote.rect.width, mote.rect.height) * Mathf.Lerp(1f, 2f, near);

            specks[i] = rt;
            inks[i]   = ink;
            // Nearer specks fall faster, which is the other half of the same depth.
            speed[i]  = Mathf.Lerp(slowest, fastest, near);
            phase[i]  = Random.value * Mathf.PI * 2f;
            wander[i] = Mathf.Lerp(0.4f, 1f, Random.value);

            rt.anchoredPosition = new Vector2(Spread(), Random.Range(-Height() * 0.5f, Height() * 0.5f));
        }
    }

    private float Height() { return Mathf.Max(1f, area.rect.height); }
    private float Width()  { return Mathf.Max(1f, area.rect.width); }
    private float Spread() { return Random.Range(-Width() * 0.5f, Width() * 0.5f); }

    private void Update() {
        if (specks == null)
            return;

        float half = Height() * 0.5f;
        float t = Time.unscaledTime;

        for (int i = 0; i < specks.Length; i++) {
            Vector2 at = specks[i].anchoredPosition;
            at.y -= speed[i] * Time.unscaledDeltaTime;
            // Off the bottom, back to the top somewhere else along it. Wrapping in place
            // would make the same speck fall down the same line forever.
            if (at.y < -half - 8f) {
                at.y = half + 8f;
                at.x = Spread();
            }
            specks[i].anchoredPosition =
                new Vector2(at.x + Mathf.Sin(t / driftPeriod * Mathf.PI * 2f + phase[i]) * driftX * wander[i], at.y);
        }
    }

    /// <summary>
    /// Brings the whole field up or down, so the opening can start it after the title
    /// lands rather than having specks already falling over an empty screen.
    ///
    /// Through a CanvasGroup rather than by walking the specks, because each speck has
    /// its own alpha standing in for its distance, and writing over those would flatten
    /// the field into one sheet the first time it faded.
    /// </summary>
    public void SetVisible(float amount) {
        CanvasGroup group = GetComponent<CanvasGroup>();
        if (group == null)
            group = gameObject.AddComponent<CanvasGroup>();
        group.alpha          = Mathf.Clamp01(amount);
        group.blocksRaycasts = false;
        group.interactable   = false;
    }
}

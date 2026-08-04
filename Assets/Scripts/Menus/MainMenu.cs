using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The screen the game opens on once the disclaimer has been read: the logo, and the four
/// places the player can go from it.
///
/// The four sit two to a side of the logo rather than in a list under it. A list makes the
/// logo a header, something the eye passes on the way to the words; two by two makes it the
/// thing the screen is built around, which is what a title screen is for.
///
/// The curve on this screen is a real one, in ScreenFisheye, applied to the finished frame.
/// It used to be faked here instead, by leaning each element: the lower row nearer the middle
/// and smaller, the logo shifting further under the pointer than the entries. That was chosen
/// to keep the text pixel exact, and it did, and it was also too quiet to notice.
///
/// The lean is still here and still worth having. It does the one thing the shader cannot: a
/// lens bends a picture, but only two things moving by different amounts when the view moves
/// are read as being at different distances. The shader gives the screen its shape and the
/// lean gives it parallax, and they are not the same effect wearing different names.
///
/// Boot is not this screen's job. GlobalControls.Awake builds the singletons, loads the
/// permanent globals and the keybinds, and does it once per run behind its own flag, so the
/// scene carries a GlobalControls object and this script carries no initialisation.
/// </summary>
public class MainMenu : MonoBehaviour {
    /// <summary>
    /// The four entries, left column first: BOSS SELECT, OPTIONS, CREDITS, QUIT. The order is
    /// the order Activate reads, and Column and Row below derive the grid from it, so moving
    /// an entry in the scene without moving it here would move the highlight and not the word.
    /// </summary>
    public Text[] entries;

    [Header("The logo, in the order the opening reveals it")]
    public Image soul;
    public Image sword;
    public Image title;

    [Header("The mark on the selected entry")]
    public RectTransform scope;
    public Image scopeInk;

    public Image fade;
    public MenuMotes motes;

    [Header("How the highlight travels")]
    /// <summary>Seconds to slide between two entries in the same column.</summary>
    public float slideTime = 0.13f;
    /// <summary>Seconds for the whole fade out and back in when the move crosses the logo.</summary>
    public float crossTime = 0.22f;

    [Header("How far things lean under the pointer")]
    public float logoLean  = 11f;
    public float entryLean = 4f;
    public float leanSmoothing = 0.32f;

    public float fadeInTime = 0.6f;

    private static readonly Color Idle   = new Color(0.80f, 0.80f, 0.80f, 1f);
    // Slightly yellow rather than white, so the selected word is marked by its own colour as
    // well as by the scope around it. The two say the same thing, which is the point: the
    // scope is in flight for a fifth of a second and the colour is not.
    private static readonly Color Picked = new Color(1f, 0.93f, 0.55f, 1f);

    /// <summary>
    /// Whether the opening has already run this launch. Static, so coming back from options
    /// or credits drops straight into the finished menu: the sword landing is worth watching
    /// once a run and is in the way every time after that. Relaunching plays it again, which
    /// keeps it testable.
    /// </summary>
    private static bool opened;

    private int selected;
    private bool ready;

    private Vector2 lean, leanVelocity;
    private Vector2 logoHome, swordHome;
    private Vector2[] entryHome;
    private RectTransform logo;

    private Coroutine scopeMove;
    /// <summary>Set while the scope is travelling, so the lean does not fight the move.</summary>
    private bool scopeBusy;

    private int Column(int i) { return i / 2; }
    private int Row(int i)    { return i % 2; }

    private void Start() {
        logo = soul != null ? soul.transform.parent as RectTransform : null;
        if (logo != null)
            logoHome = logo.anchoredPosition;
        // Read before anything moves it. The opening lifts the sword off the top of the
        // screen and drops it, so asking where home is once the strike has begun answers
        // with the position it is falling from.
        if (sword != null)
            swordHome = sword.rectTransform.anchoredPosition;

        entryHome = new Vector2[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            entryHome[i] = entries[i].rectTransform.anchoredPosition;

        Select(0, true);

        if (opened)
            StartCoroutine(Resume());
        else
            StartCoroutine(Open());
    }

    // ------------------------------------------------------------------ the opening

    /// <summary>
    /// The second and later visits. Everything is already where it ends up, so this only has
    /// to bring the screen up out of the black the previous scene faded down to.
    /// </summary>
    private IEnumerator Resume() {
        // First, before any of the presentation. Nothing below decides whether the menu can
        // be used, so nothing below should be able to stop it being usable.
        ready = true;

        Show(soul, 1f);
        Show(sword, 1f);
        Show(title, 1f);
        for (int i = 0; i < entries.Length; i++)
            Show(entries[i], 1f);
        if (scopeInk != null) Show(scopeInk, 1f);
        if (motes != null) motes.SetVisible(1f);
        StartMusic();

        if (fade == null)
            yield break;
        fade.color = Color.black;
        yield return Fade(1f, 0f, fadeInTime);
    }

    /// <summary>
    /// The first visit of the run. The soul arrives, the sword comes down through it, the
    /// title is left behind in it, the entries follow, and only then does anything make a
    /// sound. Ordered rather than simultaneous because the sequence is the sentence: a soul,
    /// then a sword, then a name that was not there before.
    ///
    /// Every wait is skippable. A player who has seen it once and pressed a key wants the
    /// menu, not to be told to wait for the pretty part to finish.
    /// </summary>
    private IEnumerator Open() {
        opened = true;

        Show(soul, 0f);
        Show(sword, 0f);
        Show(title, 0f);
        for (int i = 0; i < entries.Length; i++)
            Show(entries[i], 0f);
        if (scopeInk != null) Show(scopeInk, 0f);
        if (motes != null) motes.SetVisible(0f);

        if (fade != null) {
            fade.color = Color.black;
            StartCoroutine(Fade(1f, 0f, 0.5f));
        }

        yield return RiseGraphic(soul, 0.9f);
        if (Skipped()) yield break;
        yield return Hold(0.25f);
        if (Skipped()) yield break;

        yield return Strike();
        if (Skipped()) yield break;
        yield return Hold(0.28f);
        if (Skipped()) yield break;

        yield return RiseGraphic(title, 0.45f);
        if (Skipped()) yield break;
        yield return Hold(0.2f);
        if (Skipped()) yield break;

        for (int i = 0; i < entries.Length; i++) {
            StartCoroutine(RiseGraphic(entries[i], 0.3f));
            yield return Hold(0.07f);
            if (Skipped()) yield break;
        }
        if (scopeInk != null)
            yield return RiseGraphic(scopeInk, 0.25f);
        if (Skipped()) yield break;

        ready = true;
        StartMusic();
        if (motes != null) motes.SetVisible(1f);
    }

    /// <summary>Drives the sword down through the soul and shakes what it hits.</summary>
    private IEnumerator Strike() {
        if (sword == null) {
            yield break;
        }

        RectTransform blade = sword.rectTransform;
        Vector2 home = blade.anchoredPosition;
        Vector2 from = home + new Vector2(0f, 300f);
        blade.anchoredPosition = from;
        Show(sword, 1f);

        float t = 0f;
        const float fall = 0.16f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / fall;
            // Accelerating into the soul rather than easing out of the fall, so it lands
            // instead of arriving.
            float e = Mathf.Clamp01(t);
            blade.anchoredPosition = Vector2.Lerp(from, home, e * e);
            if (Skipped()) { blade.anchoredPosition = home; yield break; }
            yield return null;
        }
        blade.anchoredPosition = home;

        PlayOnce("slice");

        // A short shake on the soul only. Shaking the whole screen for a logo would be
        // louder than the moment is.
        if (logo != null) {
            float shake = 0f;
            while (shake < 1f) {
                shake += Time.unscaledDeltaTime / 0.22f;
                float amount = (1f - Mathf.Clamp01(shake)) * 7f;
                logo.anchoredPosition = logoHome + new Vector2(Random.Range(-amount, amount),
                                                              Random.Range(-amount, amount));
                if (Skipped()) break;
                yield return null;
            }
            logo.anchoredPosition = logoHome;
        }
    }

    /// <summary>True once a key has been pressed, after snapping the opening to its end.</summary>
    private bool Skipped() {
        if (ready || !ScreenResolution.hasInitialized || !Input.anyKeyDown)
            return false;
        StopAllCoroutines();
        Finish();
        return true;
    }

    /// <summary>Puts the screen in the state the opening would have left it in.</summary>
    private void Finish() {
        // First, for the reason Resume sets it first. This method is reached by pressing a
        // key, so anything that stops it short leaves a menu that has swallowed a keypress
        // and will swallow every one after it.
        ready = true;

        Show(soul, 1f);
        Show(sword, 1f);
        Show(title, 1f);
        for (int i = 0; i < entries.Length; i++)
            Show(entries[i], 1f);
        if (scopeInk != null) Show(scopeInk, 1f);
        if (sword != null)
            sword.rectTransform.anchoredPosition = swordHome;
        if (logo != null)
            logo.anchoredPosition = logoHome;
        if (fade != null) fade.color = new Color(0f, 0f, 0f, 0f);
        if (motes != null) motes.SetVisible(1f);
        StartMusic();
    }

    // ------------------------------------------------------------------ input

    private void Update() {
        if (entries.Length == 0)
            return;

        Lean();

        if (!ready) {
            Skipped();
            return;
        }

        int was = selected;
        if (GlobalControls.input.Down == ButtonState.PRESSED)
            Select(Column(selected) * 2 + (Row(selected) == 0 ? 1 : 0), false);
        else if (GlobalControls.input.Up == ButtonState.PRESSED)
            Select(Column(selected) * 2 + (Row(selected) == 1 ? 0 : 1), false);
        else if (GlobalControls.input.Left == ButtonState.PRESSED)
            Select(Row(selected), false);
        else if (GlobalControls.input.Right == ButtonState.PRESSED)
            Select(2 + Row(selected), false);
        else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
            Activate(selected);

        if (selected != was)
            PlayOnce("menumove");
    }

    /// <summary>
    /// Leans the logo and the entries under the pointer, the logo further than the entries.
    ///
    /// The pointer is not input here. Nothing in the menus can be clicked or hovered and this
    /// does not change that: it moves the view the way looking around a room does, and the
    /// keyboard still decides everything.
    /// </summary>
    private void Lean() {
        Vector2 size = ScreenResolution.displayedSize;
        Vector2 want = Vector2.zero;
        if (size.x > 0f && size.y > 0f) {
            Vector2 mouse = ScreenResolution.mousePosition;
            want = new Vector2(-(Mathf.Clamp(mouse.x / size.x * 2f - 1f, -1f, 1f)),
                               -(Mathf.Clamp(mouse.y / size.y * 2f - 1f, -1f, 1f)));
        }

        lean = Vector2.SmoothDamp(lean, want, ref leanVelocity,
                                  Mathf.Max(0.01f, leanSmoothing), Mathf.Infinity, Time.unscaledDeltaTime);

        if (logo != null && ready)
            logo.anchoredPosition = logoHome + lean * logoLean;
        for (int i = 0; i < entries.Length && i < entryHome.Length; i++)
            entries[i].rectTransform.anchoredPosition = entryHome[i] + lean * entryLean;

        // The entries move under the lean, so the scope has to move with them or it slides
        // off the word it is marking. Only when it is not already travelling somewhere: a
        // slide in flight owns the position until it arrives.
        if (scope != null && !scopeBusy)
            scope.anchoredPosition = Home(selected);
    }

    // ------------------------------------------------------------------ the highlight

    private void Select(int index, bool instant) {
        if (index < 0 || index >= entries.Length || (index == selected && !instant))
            return;

        bool crossed = Column(index) != Column(selected);
        selected = index;

        for (int i = 0; i < entries.Length; i++)
            entries[i].color = i == index ? Picked : Idle;

        if (scope == null)
            return;

        if (scopeMove != null)
            StopCoroutine(scopeMove);

        if (instant) {
            scope.anchoredPosition = Home(index);
            return;
        }
        // Across the middle the scope would pass over the logo, so it goes out on one side
        // and comes back on the other instead of dragging a reticle across the artwork.
        scopeMove = StartCoroutine(crossed ? Cross(index) : Slide(index));
    }

    private Vector2 Home(int index) {
        return entryHome[index] + lean * entryLean;
    }

    private IEnumerator Slide(int index) {
        scopeBusy = true;
        Vector2 from = scope.anchoredPosition;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, slideTime);
            float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);   // cubic ease out
            scope.anchoredPosition = Vector2.Lerp(from, Home(index), e);
            yield return null;
        }
        scope.anchoredPosition = Home(index);
        scopeBusy = false;
    }

    private IEnumerator Cross(int index) {
        scopeBusy = true;
        float half = Mathf.Max(0.02f, crossTime * 0.5f);
        yield return FadeGraphic(scopeInk, 1f, 0f, half);
        scope.anchoredPosition = Home(index);
        yield return FadeGraphic(scopeInk, 0f, 1f, half);
        scopeBusy = false;
    }

    // ------------------------------------------------------------------ leaving

    private void Activate(int index) {
        PlayOnce("menuconfirm");
        switch (index) {
            case 0:
                GlobalControls.modDev = true;
                DiscordControls.StartBossSelect();
                SceneManager.LoadScene("ModSelect");
                break;
            case 1: SceneManager.LoadScene("Options"); break;
            case 2: SceneManager.LoadScene("Credits"); break;
            case 3: Application.Quit(); break;
        }
    }

    // ------------------------------------------------------------------ small helpers

    /// <summary>
    /// The menu has no track of its own yet. When one is dropped into the mod's Audio folder
    /// this is what finds it, and until then the screen is quiet on purpose rather than
    /// borrowing a track from the fork it came from.
    ///
    /// Through MenuAudio rather than the registry, because a missing clip arrives as a thrown
    /// exception and not as a null. Asking directly here took out the line after it, which was
    /// the one that let the menu accept a keypress.
    /// </summary>
    private void StartMusic() {
        MenuAudio.PlayMusic("mus_menu");
    }

    private void PlayOnce(string sound) {
        MenuAudio.PlaySound(sound);
    }

    private static void Show(Graphic g, float alpha) {
        if (g == null)
            return;
        Color c = g.color;
        g.color = new Color(c.r, c.g, c.b, alpha);
    }

    private IEnumerator Hold(float seconds) {
        float t = 0f;
        while (t < seconds) {
            t += Time.unscaledDeltaTime;
            if (Skipped()) yield break;
            yield return null;
        }
    }

    private IEnumerator RiseGraphic(Graphic g, float seconds) {
        yield return FadeGraphic(g, 0f, 1f, seconds);
    }

    private IEnumerator FadeGraphic(Graphic g, float from, float to, float seconds) {
        if (g == null)
            yield break;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, seconds);
            Show(g, Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t))));
            yield return null;
        }
        Show(g, to);
    }

    private IEnumerator Fade(float from, float to, float seconds) {
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, seconds);
            fade.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, to, Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t))));
            yield return null;
        }
        fade.color = new Color(0f, 0f, 0f, to);
    }
}

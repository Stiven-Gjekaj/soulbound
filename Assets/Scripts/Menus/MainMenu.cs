using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The screen the game opens on once the disclaimer has been read: the title, and the four
/// places the player can go from it.
///
/// This is a purpose-built menu rather than the fork's title screen, which asked the player to
/// Continue, Reset or Change Name. Those three were save management wearing a menu's clothes,
/// and the options screen already offers the last two, so the menu offers the destinations
/// instead and lets options keep the settings.
///
/// Boot is not this screen's job. GlobalControls.Awake builds the singletons, loads the
/// permanent globals and the keybinds, and does it once per run behind its own flag, so the
/// scene carries a GlobalControls object and this script carries no initialisation.
/// </summary>
public class MainMenu : MonoBehaviour {
    /// <summary>The rows, top to bottom. Bound in the scene, and the order is the order shown.</summary>
    public Text[] entries;

    /// <summary>
    /// The bar that sits behind whichever row is selected. A filled bar rather than a coloured
    /// word, because the rows are laid over a painting: a colour change has to fight whatever
    /// is behind it, and a bar brings its own background with it.
    /// </summary>
    public RectTransform selectionBar;

    /// <summary>Seconds the bar takes to travel between two rows.</summary>
    public float slideTime = 0.13f;

    /// <summary>
    /// The black the menu comes up out of. The disclaimer fades down to black on its way here,
    /// so without this the handoff is a fade to black followed by the menu appearing all at
    /// once, which wastes the fade.
    /// </summary>
    public Image fade;
    public float fadeInTime = 0.6f;

    private int selected;

    // Where the bar is sliding from, to, and how far along it is. Keyframes rather than a
    // jump: the rows sit close together and a bar that teleports between them reads as the
    // screen redrawing rather than as a selection moving.
    private Vector2 slideFrom, slideTo;
    private float   slide = 1f;

    private static readonly Color Idle   = new Color(0.78f, 0.78f, 0.78f, 1f);
    private static readonly Color Picked = new Color(1f,    1f,    1f,    1f);

    private void Start() {
        AudioSource music = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        if (music != null) {
            music.clip = AudioClipRegistry.GetMusic("mus_menu");
            music.loop = true;
            music.Play();
        }

        // Keyboard only. The menus are driven with the same four directions and two buttons
        // the fight is, so a pointer would be a second way to do everything that has to be
        // kept working and that no controller has.
        if (fade != null) {
            fade.color = Color.black;
            StartCoroutine(FadeIn());
        }

        Select(0);
        if (selectionBar != null && entries.Length > 0) {
            selectionBar.anchoredPosition = entries[0].rectTransform.anchoredPosition;
            slide = 1f;
        }
    }

    private IEnumerator FadeIn() {
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeInTime);
            fade.color = new Color(0f, 0f, 0f, 1f - Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t)));
            yield return null;
        }
        fade.color = new Color(0f, 0f, 0f, 0f);
    }

    private void Update() {
        if (entries.Length == 0)
            return;

        SlideBar();

        if (GlobalControls.input.Down == ButtonState.PRESSED)
            Select(Math.Mod(selected + 1, entries.Length));
        else if (GlobalControls.input.Up == ButtonState.PRESSED)
            Select(Math.Mod(selected - 1, entries.Length));
        else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
            Activate(selected);
    }

    /// <summary>Moves the highlight, and sends the bar after it.</summary>
    private void Select(int index) {
        selected = index;
        for (int i = 0; i < entries.Length; i++)
            entries[i].color = i == index ? Picked : Idle;

        if (selectionBar == null || entries.Length == 0)
            return;

        // The bar takes the row's own position, so rows can be moved or added in the scene
        // without the bar needing to be told where they went.
        slideFrom = selectionBar.anchoredPosition;
        slideTo   = entries[index].rectTransform.anchoredPosition;
        slide     = 0f;
    }

    /// <summary>
    /// Carries the bar from one row to the next. Eased out, so it leaves quickly and arrives
    /// gently, which is what makes it read as one thing moving rather than two things blinking.
    /// </summary>
    private void SlideBar() {
        if (selectionBar == null || slide >= 1f)
            return;

        slide = Mathf.Min(1f, slide + Time.unscaledDeltaTime / Mathf.Max(0.01f, slideTime));

        // Cubic ease out.
        float e = 1f - Mathf.Pow(1f - slide, 3f);
        selectionBar.anchoredPosition = Vector2.Lerp(slideFrom, slideTo, e);
    }

    private void Activate(int index) {
        switch (index) {
            case 0:
                GlobalControls.modDev = true;
                DiscordControls.StartBossSelect();
                SceneManager.LoadScene("ModSelect");
                break;
            case 1:
                SceneManager.LoadScene("Options");
                break;
            case 2:
                SceneManager.LoadScene("Credits");
                break;
            case 3:
                Application.Quit();
                break;
        }
    }
}

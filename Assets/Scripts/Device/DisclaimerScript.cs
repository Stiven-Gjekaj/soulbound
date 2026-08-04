using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The first thing the game shows: what Soulbound is, what it is not, and whose engine it runs
/// on. Any key opens the menu.
///
/// It fades up out of black on the way in and back down on the way out. Unity's splash ends on
/// black and the menu begins on black, so the screen is bracketed by two cuts that would
/// otherwise be hard ones, and a legal notice appearing instantly and vanishing instantly reads
/// like a flash rather than something meant to be read.
///
/// It used to be the game's front door as well as its disclaimer, offering the boss select on
/// Confirm, the fork's intro on Menu, and the credits on Down, none of which a disclaimer is
/// for. It also rebranded itself at runtime, hiding an inherited logo and cloning a title out
/// of the version label, because none of the scene was reachable by field. That method is gone:
/// the screen says what it says in the scene, and the title lives on the menu behind it.
/// </summary>
public class DisclaimerScript : MonoBehaviour {
    /// <summary>The build number, so a tester can say which build they are reporting against.</summary>
    public Text Version;

    /// <summary>The black covering the whole screen, opaque at both ends and clear between.</summary>
    public Image fade;

    public float fadeInTime  = 0.9f;
    public float fadeOutTime = 0.5f;

    private bool leaving;

    private void Start() {
        if (Version != null)
            Version.text = "v" + Application.version;

        AudioSource music = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        if (music != null) {
            music.clip = AudioClipRegistry.GetMusic("mus_barrier");
            music.Play();
        }

        if (fade != null) {
            fade.color = Color.black;
            StartCoroutine(FadeTo(0f, fadeInTime));
        }
    }

    private void Update() {
        if (!ScreenResolution.hasInitialized || leaving)
            return;

        // Any key rather than a named one. The screen asks for nothing and offers one way on,
        // so a player who presses the wrong thing should still get through it. It is accepted
        // during the fade up as well, because a screen that ignores you while it finishes
        // being pretty is worse than one that skips.
        if (!Input.anyKeyDown)
            return;

        leaving = true;
        StartCoroutine(Leave());
    }

    private IEnumerator Leave() {
        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            Misc.RetargetWindow();
        #endif

        if (fade != null)
            yield return FadeTo(1f, fadeOutTime);

        DiscordControls.StartTitle();
        SceneManager.LoadScene("TitleScreen");
    }

    /// <summary>
    /// Runs the cover to an alpha. Unscaled, because this is presentation rather than
    /// simulation and should not care what the game has done to the time scale.
    /// </summary>
    private IEnumerator FadeTo(float target, float seconds) {
        float from = fade.color.a;
        float t = 0f;
        while (t < 1f) {
            t += Time.unscaledDeltaTime / Mathf.Max(0.01f, seconds);
            // Smoothstep, so it leaves and arrives gently rather than starting at full speed.
            float e = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
            fade.color = new Color(0f, 0f, 0f, Mathf.Lerp(from, target, e));
            yield return null;
        }
        fade.color = new Color(0f, 0f, 0f, target);
    }
}

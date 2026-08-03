using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The first thing the game shows: what Soulbound is, what it is not, and whose engine it runs
/// on. Any key opens the menu.
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

    private void Start() {
        if (Version != null)
            Version.text = "v" + Application.version;

        AudioSource music = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        if (music != null) {
            music.clip = AudioClipRegistry.GetMusic("mus_barrier");
            music.Play();
        }
    }

    private void Update() {
        if (!ScreenResolution.hasInitialized)
            return;

        // Any key rather than a named one. The screen asks for nothing and offers one way on,
        // so a player who presses the wrong thing should still get through it.
        if (!Input.anyKeyDown)
            return;

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            Misc.RetargetWindow();
        #endif
        DiscordControls.StartTitle();
        SceneManager.LoadScene("TitleScreen");
    }
}

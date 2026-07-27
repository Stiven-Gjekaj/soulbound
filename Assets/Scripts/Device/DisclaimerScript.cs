using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Attached to the disclaimer screen so you can skip it.
/// </summary>
public class DisclaimerScript : MonoBehaviour {
    public GameObject Logo, LuaKnowledgeDisclaimer, Version;
    private bool inCredits = false;
    private int creditsCameraSpeed = 0;

    private void Start() {
        Rebrand();
        Version.GetComponent<Text>().text = "v" + Application.version;
        Camera.main.GetComponent<AudioSource>().clip = AudioClipRegistry.GetMusic("mus_barrier");
        Camera.main.GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// Disclaimer.unity is still the fork's screen: its logo, its version, its Discord,
    /// and prompts about writing mods. It is the first thing a player sees, and none of it
    /// is reachable from C# by field because most of those fields were removed in v0.3, so
    /// the objects are found by name here instead.
    ///
    /// The credits below this screen are deliberately untouched. They are the attribution
    /// GPLv3 requires, and they name the right people.
    ///
    /// All of this wants doing properly in the scene when the menus are rebuilt for art in
    /// v0.7, at which point this method should go.
    /// </summary>
    private void Rebrand() {
        // The logo is a Create Your Frisk sprite and there is no Soulbound one yet, so the
        // name goes in its place as text, cloned from an existing label so the font, canvas
        // and material are guaranteed to match.
        RectTransform logo = Logo.GetComponent<RectTransform>();
        Logo.GetComponent<Image>().enabled = false;

        Text title = Instantiate(Version.GetComponent<Text>(), Logo.transform.parent);
        title.gameObject.name = "Title";
        title.rectTransform.anchoredPosition = logo.anchoredPosition;
        title.rectTransform.sizeDelta        = new Vector2(2800, 400);
        title.color    = new Color(1f, 1f, 1f, 1f);
        title.fontSize = 220;
        title.text     = "SOULBOUND";

        SetText("ModSelect",              "Press <color=\"#ff0\">Confirm</color>\nor <color=\"#ff0\">Click</color> to\ngo to the <color=\"#ff0\">Boss\nSelect Screen</color>");
        SetText("TheTextNobodyReadsEver", "This is an early build.\nThere is no game in it yet.");
        SetText("DiscordPlug",            "A boss rush based on Soultale. Made by <color=\"#ffff00\">PaperTrail</color>.");
        SetText("LegalStuff",             "Built on Create Your Frisk, and not owned by or affiliated with Toby Fox.\nFree software under the GPLv3. Do not sell it.");
    }

    /// <summary>Sets a disclaimer label by object name, quietly if the scene lost it.</summary>
    private void SetText(string objectName, string value) {
        GameObject go = GameObject.Find(objectName);
        if (go == null)
            return;
        Text text = go.GetComponent<Text>();
        if (text != null)
            text.text = value;
    }

    /// <summary>
    /// Checks if you pressed one of the things the disclaimer tells you to. It's pretty straightforward.
    /// </summary>
    private void Update() {
        if (!ScreenResolution.hasInitialized) return;
        if (inCredits) {
            UpdateCreditsScroll();
            if (creditsCameraSpeed == 0 && inCredits && GlobalControls.input.Up == ButtonState.PRESSED) {
                EndCreditsScroll();
            }
            return;
        }
        if (GlobalControls.input.Menu == ButtonState.PRESSED) {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                Misc.RetargetWindow();
            #endif
            StaticInits.InitAll(StaticInits.EDITOR_MODFOLDER);
            GlobalControls.modDev = false;
            SceneManager.LoadScene("Intro");
            Destroy(this);
        } else if (GlobalControls.input.Confirm == ButtonState.PRESSED || Input.GetMouseButtonDown(0)) {
            #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                Misc.RetargetWindow();
            #endif
            StartCoroutine(ModSelect());
        } else if (GlobalControls.input.Down == ButtonState.PRESSED) {
            StartCreditsScroll();
        }
    }

    /// <summary>
    /// Starts the camera shift to the Credits screen.
    /// </summary>
    private void StartCreditsScroll() {
        creditsCameraSpeed = -8;
        inCredits = true;
    }

    /// <summary>
    /// Updates the scroll to and from the Credits screen.
    /// </summary>
    private void UpdateCreditsScroll() {
        if (creditsCameraSpeed != 0) {
            transform.localPosition += new Vector3(0, creditsCameraSpeed, 0);
            if (Mathf.Abs(transform.localPosition.y) >= 240) {
                inCredits = transform.localPosition.y < 0;
                transform.localPosition = new Vector3(transform.localPosition.x, 240 * Mathf.Sign(transform.localPosition.y), transform.localPosition.z);
                creditsCameraSpeed = 0;
            }
        }
    }

    /// <summary>
    /// Starts the camera shift back to the Disclaimer screen.
    /// </summary>
    private void EndCreditsScroll() {
        creditsCameraSpeed = 8;
    }

    // The boss select screen reads the registry and every boss's portrait before it can
    // show anything, so it can take a moment. To compensate, this function puts "Loading"
    // text on the Disclaimer screen while that happens.
    private IEnumerator ModSelect() {
        LuaKnowledgeDisclaimer.GetComponent<Text>().text = "Loading bosses...";
        yield return new WaitForEndOfFrame();
        GlobalControls.modDev = true;
        DiscordControls.StartBossSelect(false);
        SceneManager.LoadScene("ModSelect");
    }
}
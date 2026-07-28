using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnterNameScript : MonoBehaviour {
    private bool weirdBackspaceShift;
    private bool isNewGame = true;
    private bool confirm;
    private bool hackFirstString;
    // Which of Quit, Backspace and Done is highlighted. It used to be able to hold a single
    // letter too, when the grid existed; setColor still branches on the length for that.
    private string choiceLetter = "Done", playerName = "";
    private readonly Dictionary<string, string> specialNameDict = new Dictionary<string, string>();
    private readonly string[] ForbiddenNames = { "lukark", "rtl", "rhenaud" };
    private string confirmText;

    public GameObject textObjFolder;
    public AudioSource uiAudio;
    public TextManager tmInstr, tmName, tmLettersMaj, tmLettersMin;

    // Use this for initialization
    private void Start() {
        AddToDict();
        isNewGame = SaveLoad.savedGame == null;
        try { GameObject.Find("textframe_border_outer").SetActive(false); }
        catch { /* ignored */ }
        // One line. The name is drawn just below this and a second line collides with it.
        tmInstr.SetTextQueue(new[] { new TextMessage(("Name the fallen human. Type it."), false, true) });
        tmInstr.SetHorizontalSpacing(2);
        tmName.SetHorizontalSpacing(2);
        GameObject firstCamera = GameObject.Find("Main Camera");
        firstCamera.name = "temp";
        if (GameObject.Find("Main Camera"))
            Destroy(GameObject.Find("Main Camera"));
        firstCamera.name = "Main Camera";
        if (!isNewGame) {
            playerName = PlayerCharacter.instance.Name;
        } else {
            Camera.main.GetComponent<AudioSource>().clip = AudioClipRegistry.GetMusic("mus_menu");
            Camera.main.GetComponent<AudioSource>().Play();
        }
        tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });

        // The letter grid is gone. It was never scene content: these two text objects were
        // filled with the alphabet at runtime and their letters renamed so the cursor could
        // find them, so leaving them empty and switched off is the whole removal.
        //
        // It existed because there was no other way to enter a name. Now that letters can be
        // typed it only created an argument over the Z key, which is both a letter and
        // Confirm, and which the grid needed and typing wanted. With nothing to confirm into,
        // Z is only ever a letter.
        //
        // Taking the objects out of EnterName.unity needs the Unity editor, so it happens at
        // v0.6 along with the rest of this screen.
        tmLettersMaj.gameObject.SetActive(false);
        tmLettersMin.gameObject.SetActive(false);

        // Highlighted directly rather than through setColor, which starts by un-highlighting
        // whatever was selected before and would go looking for a grid letter that is no
        // longer there.
        GameObject.Find(choiceLetter).GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
    }

    // Update is called once per frame
    private void Update() {
        if (confirm) return;
        if (!hackFirstString && tmName.transform.childCount != 0 && !isNewGame) {
            hackFirstString = true;
            tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });
            tmName.MoveTo(-calcTotalLength(tmName) / 2, tmName.transform.localPosition.y);
        }
        // Typed characters are consumed before anything else looks at the keyboard. W, A, S
        // and D are bound to the four directions by default, so without this a name with a W
        // in it would move the cursor between buttons while spelling itself.
        if (HandleTypedCharacters()) return;

        if (HandleMouse()) return;

        // Up and Down did nothing but walk in and out of the grid, so with the grid gone
        // they have nowhere to go. Left and Right cycle the three buttons, which is what
        // their Quit/Backspace/Done cases already did.
        if (GlobalControls.input.Right == ButtonState.PRESSED) {
            switch (choiceLetter) {
                case "Quit":      setColor("Backspace"); break;
                case "Backspace": setColor("Done");      break;
                default:          setColor("Quit");      break;
            }
        } else if (GlobalControls.input.Left == ButtonState.PRESSED) {
            switch (choiceLetter) {
                case "Quit":      setColor("Done");      break;
                case "Backspace": setColor("Quit");      break;
                default:          setColor("Backspace"); break;
            }
        } else if (GlobalControls.input.Cancel == ButtonState.PRESSED) {
            weirdBackspaceShift = true;
            if (playerName.Length > 0)
                playerName = playerName.Substring(0, playerName.Length - 1);
            else
                weirdBackspaceShift = false;
            tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });
            tmName.MoveTo(-calcTotalLength(tmName) / 2, tmName.transform.localPosition.y);
        } else if (GlobalControls.input.Confirm == ButtonState.PRESSED) {
            PressSelected();
            return;
        } else
            return;
        uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menumove"));
    }

    /// <summary>
    /// Does whatever the highlighted button does. Confirm, Enter and a mouse click all come
    /// through here, so there is one answer to what pressing a button means.
    /// </summary>
    private void PressSelected() {
        switch (choiceLetter) {
            case "Quit":
                GameObject.Find("Main Camera").GetComponent<AudioSource>().Stop();
                SceneManager.LoadScene("TitleScreen");
                break;
            case "Backspace": {
                weirdBackspaceShift = true;
                if (playerName.Length > 0)
                    playerName = playerName.Substring(0, playerName.Length - 1);
                else
                    weirdBackspaceShift = false;
                break;
            }
            case "Done": {
                // An empty name is accepted and becomes the default. Letters can only be
                // typed now, so a player on a controller can reach this button and has no
                // way to put anything in front of it; refusing them would leave them on a
                // screen with no way out.
                if (playerName.Length == 0)
                    playerName = ControlPanel.instance.BasisName;

                weirdBackspaceShift = false;
                confirm             = true;
                specialNameDict.TryGetValue(playerName.ToLower(), out confirmText);
                StartCoroutine(waitConfirm(ForbiddenNames.Contains(playerName.ToLower())));
                textObjFolder.SetActive(false);
                break;
            }
        }
        tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });
        tmName.MoveTo(-calcTotalLength(tmName) / 2, tmName.transform.localPosition.y);
        uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menuconfirm"));
    }

    private static readonly string[] buttonNames = { "Quit", "Backspace", "Done" };
    private Vector3 lastMousePosition = Vector3.zero;

    /// <summary>
    /// Highlights whichever button the pointer is over, and presses it on a click.
    /// Returns true if a click was handled.
    ///
    /// The three buttons are bare sprites with no colliders, so Unity's OnMouseDown never
    /// fires on them and adding colliders would mean editing the scene. Their world-space
    /// bounds are tested directly instead, which is exact: every scene in this game runs on
    /// the same orthographic camera at a fixed 640x480.
    /// </summary>
    private bool HandleMouse() {
        Camera cam = Camera.main;
        if (cam == null)
            return false;

        // A pointer that has not moved does not get to hold the selection. Without this a
        // mouse left resting over a button re-selects it every frame, so the arrow keys
        // appear dead: the selection moves and is dragged back before the next frame draws.
        // Clicks still work wherever the pointer is sitting.
        bool moved = Input.mousePosition != lastMousePosition;
        lastMousePosition = Input.mousePosition;

        Vector3 point = cam.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0;

        foreach (string name in buttonNames) {
            GameObject go = GameObject.Find(name);
            if (go == null)
                continue;
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr == null)
                continue;

            Bounds b = sr.bounds;
            if (point.x < b.min.x || point.x > b.max.x || point.y < b.min.y || point.y > b.max.y)
                continue;

            if (moved && choiceLetter != name) {
                setColor(name);
                uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menumove"));
            }
            if (Input.GetMouseButtonDown(0)) {
                PressSelected();
                return true;
            }
            return false;
        }
        return false;
    }

    /// <summary>
    /// Takes whatever was typed on the keyboard this frame and puts it in the name.
    /// Returns true if anything was consumed, so Update leaves the buttons alone this frame.
    ///
    /// Every letter types, z and x included. They used to be ambiguous because they are also
    /// Confirm and Cancel and the grid needed them; with no grid to confirm into, the only
    /// thing Confirm can mean here is "press the highlighted button", and Enter and the
    /// mouse both do that.
    /// </summary>
    private bool HandleTypedCharacters() {
        string typed = Input.inputString;
        if (typed.Length == 0)
            return false;

        bool changed = false;
        foreach (char c in typed) {
            // Return and Enter are bound to Confirm, which is what activates Done. Let them
            // fall through to the grid rather than swallowing them here.
            if (c == '\r' || c == '\n')
                continue;

            if (c == '\b') {
                if (playerName.Length > 0) {
                    playerName          = playerName.Substring(0, playerName.Length - 1);
                    weirdBackspaceShift = true;
                    changed             = true;
                }
                continue;
            }

            // Letters only, which is exactly what the grid offers. Accepting more would let
            // a character the font cannot draw sit invisibly inside a saved name.
            if (c < 'A' || c > 'z' || (c > 'Z' && c < 'a'))
                continue;

            if (playerName.Length < 9) playerName  = playerName + c;
            else                       playerName  = playerName.Substring(0, 8) + c;
            weirdBackspaceShift = false;
            changed             = true;
        }

        if (!changed)
            return false;

        tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });
        tmName.MoveTo(-calcTotalLength(tmName) / 2, tmName.transform.localPosition.y);
        uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menuconfirm"));
        return true;
    }

    private void setColor(int a) { setColor(((char)a).ToString());  }
    private void setColor(string str) {
        if (choiceLetter.Length != 1) GameObject.Find(choiceLetter).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        else                          GameObject.Find(choiceLetter).GetComponent<Image>().color = new Color(1, 1, 1, 1);
        choiceLetter = str;
        if (choiceLetter.Length != 1) GameObject.Find(choiceLetter).GetComponent<SpriteRenderer>().color = new Color(1, 1, 0, 1);
        else                          GameObject.Find(choiceLetter).GetComponent<Image>().color = new Color(1, 1, 0, 1);
    }

    private IEnumerator waitConfirm(bool isForbidden = false) {
        yield return 0;
        tmInstr.SetTextQueue(new[] { new TextMessage((confirmText ?? ("Is this name correct?")), false, true) });
        tmName.SetEffect(new ShakeEffect(tmName));
        GameObject.Find("Backspace").GetComponent<SpriteRenderer>().enabled = false;
        tmLettersMaj.gameObject.SetActive(false);
        tmLettersMin.gameObject.SetActive(false);
        setColor("Quit");
        GameObject.Find("Done").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, isForbidden ? 0 : 1);
        float diff = calcTotalLength(tmName)*2;
        float actualX = tmName.transform.localPosition.x, actualY = tmName.transform.localPosition.y;
        while (GlobalControls.input.Confirm != ButtonState.PRESSED) {
            if (tmName.transform.localScale.x < 3) {
                float scale = Mathf.Min(3, tmName.transform.localScale.x + 0.01f);
                tmName.transform.localScale = new Vector3(scale, scale, 1);
                tmName.MoveTo(actualX - (tmName.transform.localScale.x - 1) * diff / 2, actualY - (tmName.transform.localScale.x - 1) * diff / 6);
            }
            if ((GlobalControls.input.Left == ButtonState.PRESSED || GlobalControls.input.Right == ButtonState.PRESSED)
                    && GameObject.Find("Done").GetComponent<SpriteRenderer>().enabled &&!isForbidden) {
                setColor(choiceLetter == "Quit" ? "Done": "Quit");
                uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menumove"));
            }
            yield return 0;
        }
        uiAudio.PlayOneShot(AudioClipRegistry.GetSound("menuconfirm"));
        if (choiceLetter == "Quit") {
            textObjFolder.SetActive(true);
            confirmText = null;
            confirm = false;
            tmName.transform.localScale = new Vector3(1, 1, 1);
            tmName.SetEffect(null);
            tmName.SetTextQueue(new[] { new TextMessage(playerName, false, true) });
            tmName.MoveTo(-calcTotalLength(tmName)/2, 145);
            tmInstr.SetTextQueue(new[] { new TextMessage(("Name the fallen human."), false, true) });
            tmLettersMaj.gameObject.SetActive(true);
            tmLettersMin.gameObject.SetActive(true);
            GameObject.Find("Backspace").GetComponent<SpriteRenderer>().enabled = true;
            setColor("Done");
        } else {
            PlayerCharacter.instance.Name = playerName;
            // Store the name the player actually typed, not the version PlayerCharacter
            // truncated to nine characters, so it round trips.
            PlayerProfile.Name = PlayerCharacter.instance.Name;
            if (isNewGame) {
                GameObject.Find("Main Camera").GetComponent<AudioSource>().Stop();
                GameObject.Find("Main Camera").GetComponent<AudioSource>().PlayOneShot(AudioClipRegistry.GetSound("intro_holdup"));
                SpriteRenderer blank = GameObject.Find("Blank").GetComponent<SpriteRenderer>();
                while (blank.color.a <= 1) {
                    if (tmName.transform.localScale.x < 3) {
                        float scale = Mathf.Min(3, tmName.transform.localScale.x + 0.01f);
                        tmName.transform.localScale = new Vector3(scale, scale, 1);
                        tmName.MoveTo(actualX - (tmName.transform.localScale.x - 1f) * diff / 2f, actualY - (tmName.transform.localScale.x - 1f) * diff / 6);
                    }
                    blank.color = new Color(blank.color.r, blank.color.g, blank.color.b, blank.color.a + 0.003f);
                    yield return 0;
                }
                while (GameObject.Find("Main Camera").GetComponent<AudioSource>().isPlaying)
                    yield return 0;
                GlobalControls.modDev = true;
                SceneManager.LoadScene("ModSelect");
                DiscordControls.StartBossSelect();
            } else {
                SaveLoad.Save();
                SceneManager.LoadScene("TitleScreen");
            }
        }
    }

    float calcTotalLength(TextManager txtmgr) {
        int count = 0;
        float totalWidth = 0;
        RectTransform[] rts = txtmgr.gameObject.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < rts.Length/2; i++) {
            if (weirdBackspaceShift && i == rts.Length /2 - 1)
                break;
            totalWidth += rts[i*2].sizeDelta.x;
            count++;
        }
        totalWidth += txtmgr.hSpacing * count;
        return totalWidth;
    }

    private void AddToDict() {
        specialNameDict.Add("lukark",    "Hey, that's my name!\nDon't copy me.");
        specialNameDict.Add("rtl",       "Still my name, dude.");
        specialNameDict.Add("rhenao",    "The basis name.");
        specialNameDict.Add("rhenaud",   "My real name.");

        specialNameDict.Add("uduu",      "(Broken) The path to victory. Go to\nthe 2nd map. Real name: UDUUL");
        specialNameDict.Add("thefail",   "(Broken) DO 3 BARREL ROLLS!!!");
        specialNameDict.Add("exception", "(Broken) It's me.");
        specialNameDict.Add("fugitive",  "(Broken) *flees*\n/me flees");
        specialNameDict.Add("four",      "4");

        specialNameDict.Add("outbounds", "Go behind that dog!");
        specialNameDict.Add("soulless",  "They shall fall, one\nafter another.");

        specialNameDict.Add("notfound",  "404");
        specialNameDict.Add("404",       "Name not found.");
        specialNameDict.Add("cyf",       "The true name.\nCreate Your Frisk FTW!");
        specialNameDict.Add("credits",   "RhenaudTheLukark and lvkuln.\nThat's it, I think.");
        specialNameDict.Add("mmmmmmmmm", "You just want to watch\nthe engine burn.");
        specialNameDict.Add("wwwwwwwww", "You just want to watch\nthe engine burn.");
        specialNameDict.Add("undertale", "Without this game,\nthis wouldn't exist.");

        specialNameDict.Add("frisk",     "That'll do nothing here.");
        specialNameDict.Add("chara",     "Classic af. Your \"The true name.\"\nis in another castle.");
        specialNameDict.Add("undyne",    "It's not like you'll\nfind her, anyway.");
        specialNameDict.Add("alphys",    "Chances to see\nher: 0%.");
        specialNameDict.Add("asgore",    "Goatdad can't kill\nyou here.");
        specialNameDict.Add("toriel",    "Goatmom can't help\nyou here.");
        specialNameDict.Add("papyrus",   "HAVING MORE THAN\n6 CHARACTERS HELPS!");
        specialNameDict.Add("sans",      "no bad time for ya.");
        specialNameDict.Add("asriel",    "If that's your\nchoice...");
        specialNameDict.Add("flowey",    "You sadist.");
    }
}

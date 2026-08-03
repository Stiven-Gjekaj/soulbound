using UnityEngine;
using UnityEngine.EventSystems;
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

    /// <summary>The soul sitting beside whichever row is selected.</summary>
    public RectTransform cursor;

    /// <summary>How far left of a row's text the soul sits.</summary>
    public float cursorGap = 24f;

    private int selected;

    private static readonly Color Idle   = new Color(1f, 1f, 1f, 1f);
    private static readonly Color Picked = new Color(1f, 1f, 0f, 1f);

    private void Start() {
        AudioSource music = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        if (music != null) {
            music.clip = AudioClipRegistry.GetMusic("mus_menu");
            music.loop = true;
            music.Play();
        }

        // The rows answer to the mouse as well as the keyboard. Hover moves the selection so
        // the soul always marks what a click would activate, rather than the two disagreeing.
        for (int i = 0; i < entries.Length; i++) {
            int index = i;

            Button button = entries[i].GetComponent<Button>();
            if (button != null) {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { Select(index); Activate(index); });
            }

            EventTrigger trigger = entries[i].GetComponent<EventTrigger>();
            if (trigger != null) {
                EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                enter.callback.AddListener(data => Select(index));
                trigger.triggers.Add(enter);
            }
        }

        Select(0);
    }

    private void Update() {
        if (entries.Length == 0)
            return;

        if (GlobalControls.input.Down == ButtonState.PRESSED)
            Select(Math.Mod(selected + 1, entries.Length));
        else if (GlobalControls.input.Up == ButtonState.PRESSED)
            Select(Math.Mod(selected - 1, entries.Length));
        else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
            Activate(selected);
    }

    /// <summary>Moves the highlight, and the soul with it.</summary>
    private void Select(int index) {
        selected = index;
        for (int i = 0; i < entries.Length; i++)
            entries[i].color = i == index ? Picked : Idle;

        if (cursor == null || entries.Length == 0)
            return;

        // Sit the soul off the left edge of the row's own text rather than at a fixed x, so a
        // renamed or translated row keeps the soul against its first letter.
        RectTransform row = entries[index].rectTransform;
        Vector2 position = row.anchoredPosition;
        cursor.anchoredPosition = new Vector2(position.x - row.sizeDelta.x / 2f - cursorGap, position.y);
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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using MoonSharp.Interpreter;
using System.Collections.Generic;

public class OptionsScript : MonoBehaviour {
    // used to prevent the player from erasing session/permanent globals or their save by accident
    private int SessionGlobalResetCooldown;
    private int PermanentGlobalResetCooldown;
    private int SaveCooldown;

    // used to update the Description periodically
    private int DescriptionTimer;

    // game objects
    public GameObject ResetSG, ResetPG, ClearSave, Retro, Scale, Discord, Keys, Exit;
    public Text Description;

    // Used for controller selection
    private int selectedButton = -99;
    private List<MenuButton> buttons = new List<MenuButton>();

    // Use this for initialization
    private void Start() {
        MenuButton playerName = AdoptRetiredRow("Safe", "PlayerName");
        if (playerName) {
            playerName.GetComponentInChildren<Text>().text = "Change name...";
            playerName.GetComponent<Button>().onClick.AddListener(() => {
                SceneManager.LoadScene("EnterName");
            });
            buttons.Add(playerName);
        }

        buttons.AddRange(new MenuButton[] {
            ResetSG.GetComponent<MenuButton>(),
            ResetPG.GetComponent<MenuButton>(),
            ClearSave.GetComponent<MenuButton>(),
            Retro.GetComponent<MenuButton>(),
            Scale.GetComponent<MenuButton>(),
            Discord.GetComponent<MenuButton>(),
            Keys.GetComponent<MenuButton>(),
            Exit.GetComponent<MenuButton>()
        });

        // add button functions

        // reset session globals
        ResetSG.GetComponent<Button>().onClick.AddListener(() => {
            if (SessionGlobalResetCooldown > 0) {
                LuaScriptBinder.ClearVariables();
                SessionGlobalResetCooldown = 60 * 2;
                ResetSG.GetComponentInChildren<Text>().text = "Session Globals Erased!";
            } else {
                SessionGlobalResetCooldown = 60 * 2;
                ResetSG.GetComponentInChildren<Text>().text = "Are you sure?";
            }
        });

        // reset permanent globals
        ResetPG.GetComponent<Button>().onClick.AddListener(() => {
            if (PermanentGlobalResetCooldown > 0) {
                LuaScriptBinder.ClearPermanentGlobals();
                PermanentGlobalResetCooldown = 60 * 2;
                ResetPG.GetComponentInChildren<Text>().text = "Permanent Globals Erased!";

                // Add useful permanent globals
                LuaScriptBinder.SetPermanentGlobal("CYFRetroMode", DynValue.NewBoolean(GlobalControls.retroMode));
                LuaScriptBinder.SetPermanentGlobal("CYFWindowScale", DynValue.NewNumber(ScreenResolution.windowScale));
            } else {
                PermanentGlobalResetCooldown = 60 * 2;
                ResetPG.GetComponentInChildren<Text>().text = "Are you sure?";
            }
        });

        // clear Save
        ClearSave.GetComponent<Button>().onClick.AddListener(() => {
            if (SaveCooldown > 0) {
                File.Delete(Application.persistentDataPath + "/save.gd");
                SaveCooldown = 60 * 2;
                ClearSave.GetComponentInChildren<Text>().text = "Save wiped!";
            } else {
                SaveCooldown = 60 * 2;
                ClearSave.GetComponentInChildren<Text>().text = "Are you sure?";
            }
        });

        // toggle retrocompatibility mode
        Retro.GetComponent<Button>().onClick.AddListener(() => {
            GlobalControls.retroMode =!GlobalControls.retroMode;

            // save RetroMode preferences to permanent globals
            LuaScriptBinder.SetPermanentGlobal("CYFRetroMode", DynValue.NewBoolean(GlobalControls.retroMode));

            Retro.GetComponentInChildren<Text>().text = ("Retrocompatibility Mode: " + (GlobalControls.retroMode ? "On" : "Off"));
        });
        Retro.GetComponentInChildren<Text>().text = ("Retrocompatibility Mode: " + (GlobalControls.retroMode ? "On" : "Off"));

        // change window scale
        Scale.GetComponent<Button>().onClick.AddListener(() => {
            #if UNITY_EDITOR
                int maxScale = 0;
            #else
                int maxScale = Mathf.FloorToInt(System.Math.Min(Screen.currentResolution.width / 640f, Screen.currentResolution.height / 480f));
            #endif
            if (ScreenResolution.windowScale < maxScale)
                ScreenResolution.windowScale += 1;
            else
                ScreenResolution.windowScale = 1;
            ScreenResolution.tempWindowScale = ScreenResolution.windowScale;
            ScreenResolution.SetFullScreen(Screen.fullScreen);

            // save RetroMode preferences to permanent globals
            LuaScriptBinder.SetPermanentGlobal("CYFWindowScale", DynValue.NewNumber(ScreenResolution.windowScale));

            Scale.GetComponentInChildren<Text>().text = "Window Scale: "  + ScreenResolution.windowScale + "x";
        });
        Scale.GetComponentInChildren<Text>().text = "Window Scale: " + ScreenResolution.windowScale  + "x";

        // Discord Rich Presence
        // Change Discord Status Visibility
        Discord.GetComponent<Button>().onClick.AddListener(() => {
            Discord.GetComponentInChildren<Text>().text = ("Discord Display: ") + DiscordControls.ChangeVisibilitySetting(1);
        });
        Discord.GetComponentInChildren<Text>().text = ("Discord Display: ") + DiscordControls.ChangeVisibilitySetting(0);

        Keys.GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("KeybindSettings");
        });
        Keys.GetComponentInChildren<Text>().text = "Keybinds...";

        // exit
        Exit.GetComponent<Button>().onClick.AddListener(() => {
            SceneManager.LoadScene("ModSelect");
        });

        HideRetiredButtons();
        StackButtons();
    }

    /// <summary>
    /// Takes over a row Options.unity still holds for an option that has been retired,
    /// renaming it so the hover description keys to what the row now does.
    ///
    /// Adding a genuinely new row needs someone with the Unity editor open, and the scene
    /// has spare rows sitting in it doing nothing, so reusing one costs nothing and leaves
    /// one less orphan behind. Give these proper objects when the menus are rebuilt for art.
    /// </summary>
    private MenuButton AdoptRetiredRow(string sceneName, string newName) {
        Transform row = ResetSG.transform.parent.Find(sceneName);
        if (!row)
            return null;
        row.gameObject.name = newName;
        row.GetComponent<Button>().onClick.RemoveAllListeners();
        return row.GetComponent<MenuButton>();
    }

    /// <summary>
    /// Options.unity still holds rows for options that have been retired, safe mode and
    /// Crate Your Frisk among them. The list is whatever is in `buttons`; anything else
    /// under the same parent is left over from a previous version and is hidden here.
    ///
    /// This is a stopgap. Those objects want deleting from the scene, which needs someone
    /// with the Unity editor open, and is worth doing when the menus are rebuilt for art.
    /// </summary>
    private void HideRetiredButtons() {
        foreach (Transform child in ResetSG.transform.parent)
            if (child.GetComponent<MenuButton>() && !buttons.Exists(b => b.transform == child))
                child.gameObject.SetActive(false);
    }

    /// <summary>
    /// Lays the visible options out from the top down, at the spacing Options.unity uses.
    ///
    /// Each button carries a fixed position from the scene, so hiding one left a gap where
    /// it used to be. Stacking them here means options can be added or retired without a
    /// scene edit, and without the list ever having a hole in it.
    /// </summary>
    private void StackButtons() {
        const float top = 160f, pitch = 40f;
        int row = 0;
        foreach (MenuButton button in buttons) {
            if (!button.gameObject.activeSelf)
                continue;
            RectTransform rt = button.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, top - pitch * row);
            row++;
        }
    }

    /// <summary>
    /// Which option the mouse is over, worked out from where the buttons actually are
    /// rather than from a ladder of hardcoded bands that has to be kept in step with them.
    /// </summary>
    private string ButtonAt(float mouseY) {
        foreach (MenuButton button in buttons) {
            if (!button.gameObject.activeSelf)
                continue;
            // anchoredPosition is measured from the middle of the 480 tall screen
            float centre = button.GetComponent<RectTransform>().anchoredPosition.y + 240f;
            if (mouseY <= centre + 20f && mouseY > centre - 20f)
                return button.name;
        }
        return null;
    }

    // Gets the text the description should use based on what button is currently being hovered over
    private string GetDescription(string buttonName) {
        string response;
        switch(buttonName) {
            case "ResetSG":
                response = "Resets all Session Global, also known as Real Globals.\n\n"
                         + "Session Globals are variables that persist through battles, but are deleted when the game is closed.";
                return response;
            case "ResetPG":
                response = "Resets all Permanent Globals, also known as AlMighty Globals.\n\n"
                         + "Permanent Globals are variables that are saved to a file, and stay even after you close the game.\n\n"
                         + "The options on this screen are stored as Permanent Globals.";
                return response;
            case "ClearSave":
                response = "Clears your save file.\n\n"
                         + "This holds your name, stats and inventory between sessions.\n\n"
                         + "Your save file is located at:\n\n";
                return response + "<b><size='14'>" + Application.persistentDataPath + "/save.gd</size></b>";
            case "Retro":
                response = "Toggles retrocompatibility mode.\n\n"
                         + "This mode is designed specifically to make encounters imported from Unitale v0.2.1a act as they did on the old engine.\n\n\n\n";
                return response + "<b>CAUTION!\nDISABLE</b> this unless you are running an encounter written for\n<b>Unitale v0.2.1a</b>.";
            case "Scale":
                response = "Scales the window in Windowed mode.\n\n"
                         + "This is useful for especially large screens (such as 4k monitors).\n\n"
                         + "Has no effect in Fullscreen mode.";
                return response;
            case "Discord":
                response = "Changes how much Discord Rich Presence should display on your profile regarding you playing Create Your Frisk.\n\n"
                         + "<b>Everything</b>: Everything is displayed: the mod you're playing, a timestamp and a description.\n\n"
                         + "<b>Game Only</b>: Only shows that you're playing Create Your Frisk.\n\n"
                         + "<b>Nothing</b>: Disables Discord Rich Presence entirely.\n\n"
                         + "If the connection to Discord is lost, you will have to restart Soulbound if you want your rich presence back.";
                return response;
            case "Keys":
                response = "Allows you to change the keys bound to the game's default keybinds, such as Confirm or Cancel.\n\n"
                         + "That way, your own keyboard scheme still works properly, and comfortably.";
                return response;
            case "PlayerName":
                response = "Changes your character's name.\n\n"
                         + "The name is stored as a Permanent Global, so it survives wiping your save "
                         + "file, and resetting Permanent Globals makes the game ask for it again.";
                return response;
            case "Exit":
                response = "Returns to the boss select screen.";
                return response;
            default:
                return "Hover over an option and its description will appear here!";
        }
    }

    private void SelectButton(int dir) {
        // Deselect the old button
        if (selectedButton >= 0) {
            buttons[selectedButton].lockAnimation = false;
            buttons[selectedButton].StartAnimation(-1);
        }

        selectedButton += dir;

        // Clamp the button number between existing buttons
        if (selectedButton < -10)                    selectedButton = 0;
        else if (selectedButton < 0)                 selectedButton = buttons.Count - 1;
        else if (selectedButton > buttons.Count - 1) selectedButton = 0;

        // Select the new button
        buttons[selectedButton].StartAnimation(1);
        buttons[selectedButton].lockAnimation = true;
    }

    // Used to animate scrolling left or right.
    private void Update() {
        // Button controls
        // Press the currently selected button
        if (GlobalControls.input.Confirm == ButtonState.PRESSED && selectedButton >= 0)
            buttons[selectedButton].GetComponent<Button>().onClick.Invoke();
        // Exit the Options menu
        if (GlobalControls.input.Cancel == ButtonState.PRESSED)
            buttons[buttons.Count - 1].GetComponent<Button>().onClick.Invoke();
        // Move up and down to navigate the buttons
        if (GlobalControls.input.Up == ButtonState.PRESSED)
            SelectButton(-1);
        if (GlobalControls.input.Down == ButtonState.PRESSED)
            SelectButton(1);

        // Update the description every 1/6th of a second
        if (DescriptionTimer > 0)
            DescriptionTimer--;
        else {
            DescriptionTimer = 10;

            // Try to find which button the player is hovering over
            string hoverItem = null;
            // If the player is within the range of the buttons
            int mousePosX = (int)((ScreenResolution.mousePosition.x / ScreenResolution.displayedSize.x) * 640);
            int mousePosY = (int)((Input.mousePosition.y / ScreenResolution.displayedSize.y) * 480);
            if (mousePosX >= 40 && mousePosX <= 290)
                hoverItem = ButtonAt(mousePosY);

            // Change the description to the current one
            if (hoverItem != null)        Description.GetComponent<Text>().text = GetDescription(hoverItem);
            // Else go back to the one selected by the inputs
            else if (selectedButton >= 0) Description.GetComponent<Text>().text = GetDescription(buttons[selectedButton].name);
            // Else pick the default description
            else                          Description.GetComponent<Text>().text = GetDescription("");
        }

        // Make the player click twice to reset RG or AG, or to wipe their save
        if (SessionGlobalResetCooldown > 0)
            SessionGlobalResetCooldown -= 1;
        else if (SessionGlobalResetCooldown == 0) {
            SessionGlobalResetCooldown = -1;
            ResetSG.GetComponentInChildren<Text>().text = "Reset Session Globals";
        }

        if (PermanentGlobalResetCooldown > 0)
            PermanentGlobalResetCooldown -= 1;
        else if (PermanentGlobalResetCooldown == 0) {
            PermanentGlobalResetCooldown = -1;
            ResetPG.GetComponentInChildren<Text>().text = "Reset Permanent Globals";
        }

        if (SaveCooldown > 0)
            SaveCooldown -= 1;
        else if (SaveCooldown == 0) {
            SaveCooldown = -1;
            ClearSave.GetComponentInChildren<Text>().text = "Wipe Save";
        }
    }
}

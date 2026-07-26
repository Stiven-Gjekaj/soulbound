using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectOMatic : MonoBehaviour {
    private static int currentBoss;
    private static List<BossEntry> bosses;
    private Dictionary<string, Sprite> portraits = new Dictionary<string, Sprite>();
    private bool animationDone = true;
    private float animationTimer;
    public EventSystem eventSystem;

    private static float bossListScroll;        // Used to keep track of the position of the boss list. Resets if you press escape

    private float ExitButtonAlpha = 5f;         // Used to fade the "Exit" button in and out
    private float OptionsButtonAlpha = 5f;      // Used to fade the "Options" button in and out

    private static int selectedItem;            // Used to let users navigate the boss list with the arrow keys!
    private bool ready;                         // False until Start finishes, so Update cannot drive a half set up screen

    // encounterBox is the scrolling overlay the boss list lives in. It keeps its old name
    // because it is bound by name to ModSelect.unity.
    public GameObject encounterBox, devMod, content, retromodeWarning;
    public GameObject btnList,              btnBack,              btnNext,              btnExit,              btnOptions;
    public Text       ListText, ListShadow, BackText, BackShadow, NextText, NextShadow, ExitText, ExitShadow, OptionsText, OptionsShadow;
    public GameObject ModContainer,  ModBackground,     ModTitle,     ModTitleShadow,     EncounterCount,     EncounterCountShadow,     FolderText,     FolderTextShadow;
    public GameObject AnimContainer, AnimModBackground, AnimModTitle, AnimModTitleShadow, AnimEncounterCount, AnimEncounterCountShadow, AnimFolderText, AnimFolderTextShadow;

    // Use this for initialization
    private void Start() {
        Destroy(GameObject.Find("Player"));
        UnitaleUtil.firstErrorShown = false;

        // There is one mod, and it is the game.
        StaticInits.MODFOLDER = StaticInits.GAME_MODFOLDER;

        // Name the character before the first fight. Both routes off the disclaimer end
        // here, and only one of them passes through the title screen, so this is the one
        // place that catches every player exactly once. Returning from a fight is not a
        // first run, and checking that also means a failed name entry cannot loop.
        if (StaticInits.ENCOUNTER == "" && !PlayerProfile.HasName) {
            SceneManager.LoadScene("EnterName");
            return;
        }

        // Re-read the registry every time, so editing bosses.lua does not need a restart.
        BossRegistry.Reload();
        bosses = BossRegistry.Entries;

        // BossRegistry has already sent the player to the error screen saying why.
        if (bosses.Count == 0)
            return;

        // Arriving from anywhere but a fight is a fresh run, so start at the first boss.
        // Coming back from a fight keeps the cursor on the boss just fought.
        if (StaticInits.ENCOUNTER == "") {
            currentBoss    = 0;
            selectedItem   = 0;
            bossListScroll = 0.0f;
        }

        // Keep the stored selection in range, in case the registry shrank since last time.
        if (currentBoss >= bosses.Count)
            currentBoss = 0;

        // Bind button functions
        btnBack.GetComponent<Button>().onClick.RemoveAllListeners();
        btnBack.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            if (!animationDone) return;
            bossSelection();
            ScrollBosses(-1);
        });
        btnNext.GetComponent<Button>().onClick.RemoveAllListeners();
        btnNext.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            if (!animationDone) return;
            bossSelection();
            ScrollBosses( 1);
        });

        // Give the boss list button a function
        btnList.GetComponent<Button>().onClick.RemoveAllListeners();
        btnList.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            if (animationDone)
                bossListMenu();
        });
        // Grab the exit button, and give it some functions
        btnExit.GetComponent<Button>().onClick.RemoveAllListeners();
        btnExit.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            SceneManager.LoadScene("Disclaimer");
            DiscordControls.StartTitle();
        });

        // Add devMod button functions
        if (GlobalControls.modDev) {
            btnOptions.GetComponent<Button>().onClick.RemoveAllListeners();
            btnOptions.GetComponent<Button>().onClick.AddListener(() => {
                eventSystem.SetSelectedGameObject(null);
                SceneManager.LoadScene("Options");
            });
        }

        // ModSelect.unity still labels this button from when it listed mod folders.
        ListText.text   = "Boss List";
        ListShadow.text = ListText.text;

        if (retromodeWarning)
            retromodeWarning.SetActive(GlobalControls.retroMode);

        bossSelection();

        // This check will be true if we just came back from a fight rather than from the Disclaimer screen
        if (StaticInits.ENCOUNTER != "") {
            // Start the Exit button at half transparency
            ExitButtonAlpha                       = 0.5f;
            ExitText.GetComponent<Text>().color   = new Color(1f, 1f, 1f, 0.5f);
            ExitShadow.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0.5f);

            // Start the Options button at half transparency
            if (GlobalControls.modDev) {
                OptionsButtonAlpha                       = 0.5f;
                OptionsText.GetComponent<Text>().color   = new Color(1f, 1f, 1f, 0.5f);
                OptionsShadow.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0.5f);
            }
        }

        // Reset it to let us accurately tell if the player just came here from the Disclaimer scene or the Battle scene
        StaticInits.ENCOUNTER = "";

        ready = true;
    }

    private IEnumerator LaunchBoss(BossEntry boss) {
        // First: make sure the encounter is still here and can be opened
        if (!File.Exists(Path.Combine(FileLoader.ModDataPath, "Lua/Encounters/" + boss.id + ".lua"))) {
            UnitaleUtil.DisplayLuaError(BossRegistry.FileName, "The boss \"" + boss.id + "\" had an encounter script when the list loaded, but it is gone now.");
            yield break;
        }

        // Dim the background to indicate loading
        ModBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.1875f);

        StaticInits.ENCOUNTER = boss.id;
        BossRecords.FightStarting(boss.id);

        yield return new WaitForEndOfFrame();
        try {
            StaticInits.InitAll(StaticInits.MODFOLDER, true);
            if (UnitaleUtil.firstErrorShown)
                throw new Exception();
            Debug.Log("Loading " + boss.id);
            GlobalControls.isInFight = true;
            DiscordControls.StartBattle(boss.name);
            SceneManager.LoadScene("Battle");
        } catch (Exception e) {
            ModBackground.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.25f);
            Debug.LogError("An error occured while starting a boss fight:\n" + e.Message + "\n\n" + e.StackTrace);
        }
    }

    // Shows one boss.
    private void ShowBoss(int id) {
        BossEntry boss = bosses[id];

        // Make clicking the background start the fight
        ModBackground.GetComponent<Button>().onClick.RemoveAllListeners();
        ModBackground.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            if (animationDone)
                StartCoroutine(LaunchBoss(boss));
        });

        // Update the portrait
        ModBackground.GetComponent<Image>().sprite = Portrait(boss);

        // Update the text
        ModTitle.GetComponent<Text>().text       = boss.name;
        ModTitleShadow.GetComponent<Text>().text = boss.name;

        EncounterCount.GetComponent<Text>().text       = boss.subtitle;
        EncounterCountShadow.GetComponent<Text>().text = boss.subtitle;

        // The cleared marker sits on the line the mod folder path used to occupy. Best
        // time and attempts are recorded too, but they wait for v0.4 to give them a place.
        string cleared = BossRecords.Cleared(boss.id) ? "CLEARED" : "";
        FolderText.GetComponent<Text>().text       = cleared;
        FolderTextShadow.GetComponent<Text>().text = FolderText.GetComponent<Text>().text;

        // Update the color of the arrows
        if (bosses.Count == 1) {
            BackText.color = new Color(0.25f, 0.25f, 0.25f, 1f);
            NextText.color = new Color(0.25f, 0.25f, 0.25f, 1f);
        } else {
            BackText.color = new Color(1f, 1f, 1f, 1f);
            NextText.color = new Color(1f, 1f, 1f, 1f);
        }
    }

    /// <summary>
    /// The image behind a boss's name. Bosses have no art yet, so this almost always
    /// falls through to the engine's black background. An artist only has to drop
    /// Sprites/Bosses/&lt;id&gt;.png into the mod for it to be picked up.
    /// </summary>
    private Sprite Portrait(BossEntry boss) {
        if (portraits.ContainsKey(boss.id))
            return portraits[boss.id];

        FileLoader.absoluteSanitizationDictionary.Clear();
        FileLoader.relativeSanitizationDictionary.Clear();

        Sprite portrait;
        try { portrait = SpriteUtil.FromFile("Bosses/" + boss.id + ".png"); }
        catch { portrait = SpriteUtil.FromFile("black.png"); }

        portraits.Add(boss.id, portrait);
        return portrait;
    }

    // Goes to the next or previous boss with a little scrolling animation.
    // -1 for left, 1 for right
    private void ScrollBosses(int dir) {
        // First, determine if the next boss should be shown
        if (bosses.Count <= 1) return;

        // If the new boss is being shown, start the animation!
        animationTimer = dir / 10f;
        animationDone  = false;

        // Enable the "ANIM" assets
        AnimContainer.SetActive(true);
        AnimContainer.transform.localPosition                 = new Vector2(0, 0);
        AnimModBackground       .GetComponent<Image>().sprite = ModBackground.GetComponent<Image>().sprite;
        AnimModTitleShadow      .GetComponent<Text>().text    = ModTitleShadow.GetComponent<Text>().text;
        AnimModTitle            .GetComponent<Text>().text    = ModTitle.GetComponent<Text>().text;
        AnimEncounterCountShadow.GetComponent<Text>().text    = EncounterCountShadow.GetComponent<Text>().text;
        AnimEncounterCount      .GetComponent<Text>().text    = EncounterCount.GetComponent<Text>().text;
        AnimFolderTextShadow    .GetComponent<Text>().text    = FolderTextShadow.GetComponent<Text>().text;
        AnimFolderText          .GetComponent<Text>().text    = FolderText.GetComponent<Text>().text;

        // Move all real assets to the side
        ModBackground.transform.Translate(640        * dir, 0, 0);
        ModTitleShadow.transform.Translate(640       * dir, 0, 0);
        ModTitle.transform.Translate(640             * dir, 0, 0);
        EncounterCountShadow.transform.Translate(640 * dir, 0, 0);
        EncounterCount.transform.Translate(640       * dir, 0, 0);
        FolderTextShadow.transform.Translate(640     * dir, 0, 0);
        FolderText.transform.Translate(640           * dir, 0, 0);

        // Actually choose the next boss
        currentBoss = Math.Mod(currentBoss + dir, bosses.Count);

        ShowBoss(currentBoss);
    }

    // Used to animate scrolling left or right.
    private void Update() {
        // Start bailed out: either it is loading another scene, or BossRegistry sent the
        // player to the error screen. Either way there is nothing here to drive yet.
        if (!ready)
            return;

        // Animation updating section
        if (AnimContainer.activeSelf) {
            animationTimer = animationTimer > 0 ? Mathf.Floor(animationTimer + 1) : Mathf.Ceil (animationTimer - 1);

            int distance = (int)((20 - Mathf.Abs(animationTimer)) * 3.4 * -Mathf.Sign(animationTimer));

            AnimContainer.transform.Translate(distance, 0, 0);
            ModContainer.transform.Translate(distance, 0, 0);

            if (Mathf.Abs(animationTimer) == 20) {
                AnimContainer.SetActive(false);

                // Manual movement because I can't change the movement multiplier to a precise enough value
                ModContainer.transform.Translate((int)(2 * -Mathf.Sign(animationTimer)), 0, 0);

                animationTimer = 0;
                animationDone = true;
            }
        }

        // Prevent scrolling too far in the encounter box
        if (encounterBox.activeSelf) {
            if (content.GetComponent<RectTransform>().anchoredPosition.y < -200)
                content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
            else if (content.GetComponent<RectTransform>().anchoredPosition.y > (content.transform.childCount - 1) * 30)
                content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (content.transform.childCount - 1) * 30);
        }

        // Detect hovering over the Exit button and handle fading
        if (ScreenResolution.mousePosition.x / ScreenResolution.displayedSize.x * 640 < 70 && Input.mousePosition.y / ScreenResolution.displayedSize.y * 480 > 450 && ExitButtonAlpha < 1f) {
            ExitButtonAlpha += 0.05f;
            ExitText.color   = new Color(1f, 1f, 1f, ExitButtonAlpha);
            ExitShadow.color = new Color(0f, 0f, 0f, ExitButtonAlpha);
        } else if (ExitButtonAlpha > 0.5f) {
            ExitButtonAlpha -= 0.05f;
            ExitText.color   = new Color(1f, 1f, 1f, ExitButtonAlpha);
            ExitShadow.color = new Color(0f, 0f, 0f, ExitButtonAlpha);
        }

        // Detect hovering over the Options button and handle fading
        if (GlobalControls.modDev) {
            if (ScreenResolution.mousePosition.x / ScreenResolution.displayedSize.x * 640 > 550 && Input.mousePosition.y / ScreenResolution.displayedSize.y * 480 > 450 && OptionsButtonAlpha < 1f) {
                OptionsButtonAlpha += 0.05f;
                OptionsText.color   = new Color(1f, 1f, 1f, OptionsButtonAlpha);
                OptionsShadow.color = new Color(0f, 0f, 0f, OptionsButtonAlpha);
            } else if (OptionsButtonAlpha > 0.5f) {
                OptionsButtonAlpha -= 0.05f;
                OptionsText.color   = new Color(1f, 1f, 1f, OptionsButtonAlpha);
                OptionsShadow.color = new Color(0f, 0f, 0f, OptionsButtonAlpha);
            }
        }

        // Controls:

        ////////////////// Main: ////////////////////////////////////
        //        Confirm: Fight the boss on screen                 //
        //         Cancel: Return to Disclaimer screen              //
        //             Up: Open the boss list                       //
        //           Menu: Open the options menu                    //
        //           Left: Scroll left                              //
        //          Right: Scroll right                             //
        ////////////////// Boss list: ///////////////////////////////
        //        Confirm: Jump to a boss                           //
        //         Cancel: Close the list                           //
        //             Up: Move up                                  //
        //           Down: Move down                                //
        /////////////////////////////////////////////////////////////

        if (!encounterBox.activeSelf) {
            // Main controls
            if (animationDone) {
                // Move left
                if (GlobalControls.input.Left == ButtonState.PRESSED)       ScrollBosses(-1);
                // Move right
                else if (GlobalControls.input.Right == ButtonState.PRESSED) ScrollBosses(1);
                // Open the boss list
                else if (GlobalControls.input.Up == ButtonState.PRESSED) {
                    bossListMenu();
                    content.transform.GetChild(selectedItem).GetComponent<MenuButton>().StartAnimation(1);
                // Fight the boss on screen
                } else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
                    ModBackground.GetComponent<Button>().onClick.Invoke();
            }

            // Access the Options menu
            if (GlobalControls.input.Menu == ButtonState.PRESSED)
                btnOptions.GetComponent<Button>().onClick.Invoke();
            // Return to the Disclaimer screen
            if (GlobalControls.input.Cancel == ButtonState.PRESSED)
                btnExit.GetComponent<Button>().onClick.Invoke();
        } else {
            // Boss list controls
            if (GlobalControls.input.Up == ButtonState.PRESSED || GlobalControls.input.Down == ButtonState.PRESSED) {
                // Store previous value of selectedItem
                int previousSelectedItem = selectedItem;

                // Move up or down the list
                selectedItem += GlobalControls.input.Up == ButtonState.PRESSED ? -1 : 1;

                // Keep the selector in-bounds
                if (selectedItem < 0)                                     selectedItem = content.transform.childCount - 1;
                else if (selectedItem > content.transform.childCount - 1) selectedItem = 0;

                // Animate the old button
                GameObject previousButton = content.transform.GetChild(previousSelectedItem).gameObject;
                previousButton.GetComponent<MenuButton>().StartAnimation(-1);

                // Animate the new button
                GameObject newButton = content.transform.GetChild(selectedItem).gameObject;
                newButton.GetComponent<MenuButton>().StartAnimation(1);

                // Scroll to the newly chosen button if it is hidden!
                float buttonTopEdge    = -newButton.GetComponent<RectTransform>().anchoredPosition.y + 100;
                float buttonBottomEdge = -newButton.GetComponent<RectTransform>().anchoredPosition.y + 100 + 30;

                float topEdge    = content.GetComponent<RectTransform>().anchoredPosition.y;
                float bottomEdge = content.GetComponent<RectTransform>().anchoredPosition.y + 230;

                // Button is above the top of the view
                if (topEdge > buttonTopEdge)
                    content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, buttonTopEdge);
                // Button is below the bottom of the view
                else if (bottomEdge < buttonBottomEdge)
                    content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, buttonBottomEdge - 230);
            }

            // Close the list
            if (GlobalControls.input.Cancel == ButtonState.PRESSED)
                ModBackground.GetComponent<Button>().onClick.Invoke();
            // Jump to the highlighted boss
            else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
                content.transform.GetChild(selectedItem).GetComponent<Button>().onClick.Invoke();
        }
    }

    // Shows the "boss page" screen.
    private void bossSelection() {
        eventSystem.SetSelectedGameObject(null);
        UnitaleUtil.printDebuggerBeforeInit = "";
        ShowBoss(currentBoss);

        // Hide the 4 buttons if needed
        if (!GlobalControls.modDev)
            devMod.SetActive(false);

        // Show the boss list button
        btnList.SetActive(true);

        // If the encounter box is visible, remove all its buttons before hiding
        if (encounterBox.activeSelf) {
            foreach (Transform b in content.transform) {
                if (b.gameObject.name != "Back")
                    Destroy(b.gameObject);
                else
                    b.GetComponent<MenuButton>().Reset();
            }
        }
        // Hide the selection box
        encounterBox.SetActive(false);
    }

    // Opens the scrolling interface and lets the user jump straight to any boss.
    private void bossListMenu() {
        // Hide the boss list button
        btnList.SetActive(false);

        // Automatically select the current boss when the list appears
        selectedItem = currentBoss + 1;

        // Give the back button its function
        GameObject back = content.transform.Find("Back").gameObject;
        back.GetComponent<Button>().onClick.RemoveAllListeners();
        back.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            // Reset the list's position
            bossListScroll = 0.0f;
            bossSelection();
        });

        // Make clicking the background exit this menu
        ModBackground.GetComponent<Button>().onClick.RemoveAllListeners();
        ModBackground.GetComponent<Button>().onClick.AddListener(() => {
            eventSystem.SetSelectedGameObject(null);
            if (!animationDone) return;
            // Store the list's position so it can be remembered
            bossListScroll = content.GetComponent<RectTransform>().anchoredPosition.y;
            bossSelection();
        });
        // Show the selection box
        encounterBox.SetActive(true);
        // Move the box to the stored position, for easier browsing
        content.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, bossListScroll);

        for (int i = 0; i < bosses.Count; i++) {
            BossEntry boss = bosses[i];

            // Create a button for each boss
            GameObject button = Instantiate(back);

            // Set parent and name
            button.transform.SetParent(content.transform);
            button.name = "BossButton";

            // Set position
            button.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 100 - (i + 1) * 30);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(430, 30);
            button.transform.Find("Fill").GetComponent<RectTransform>().sizeDelta = new Vector2(420, 20);

            // Set color
            button.GetComponent<Image>().color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
            button.GetComponent<MenuButton>().NormalColor = new Color(0.75f, 0.75f, 0.75f, 0.5f);
            button.GetComponent<MenuButton>().HoverColor  = new Color(0.75f, 0.75f, 0.75f, 1f);
            button.transform.Find("Fill").GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            // Set text
            string label = boss.name + (BossRecords.Cleared(boss.id) ? " - cleared" : "");
            button.transform.Find("Text").GetComponent<Text>().text = label;

            int tempCount = i;

            // Finally, set function!
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => {
                eventSystem.SetSelectedGameObject(null);
                // Store the list's position so it can be remembered
                bossListScroll = content.GetComponent<RectTransform>().anchoredPosition.y;

                currentBoss = tempCount;
                bossSelection();
            });
        }
    }
}

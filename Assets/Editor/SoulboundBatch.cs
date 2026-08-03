using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Working tool, not shipped. Drives the v0.6 menu rebuild from batch mode so the scenes are
/// written by the editor that owns the format rather than by hand in the YAML.
/// </summary>
public static class SoulboundBatch {
    private const string TitlePath  = "Assets/Scenes/TitleScreen.unity";
    private const string CreditPath = "Assets/Scenes/Credits.unity";
    private const string DisclaimerPath = "Assets/Scenes/Disclaimer.unity";
    private const string BossSelectPath = "Assets/Scenes/ModSelect.unity";
    private const string OptionsPath = "Assets/Scenes/Options.unity";
    private const string KeybindPath = "Assets/Scenes/KeybindSettings.unity";
    private const string NamePath = "Assets/Scenes/EnterName.unity";
    private const string BattlePath = "Assets/Scenes/Battle.unity";

    private const string TitleSprite = "Assets/Sprites/Soulbound_Title.png";
    private const string SoulSprite  = "Assets/Sprites/Soul_Cursor.png";
    private const string SilhouetteSprite = "Assets/Sprites/Boss_Silhouette.png";
    private const string ArenaBorderSprite = "Assets/Sprites/Arena_Border.png";
    private const string MenuFont    = "Assets/Fonts/PixelOperator/PixelOperator-Bold.ttf";

    /// <summary>
    /// Opens the rebuilt scenes and reports what is actually in them, including whether the
    /// inspector references resolved. A menu whose entries array came out empty still saves
    /// and still loads, and only fails when a player presses a key, so it is worth asserting
    /// here rather than finding out in a build.
    /// </summary>
    public static void Verify() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("SOULBOUND-BATCH-OK unity=" + Application.unityVersion);
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            sb.AppendLine("  buildscene " + (s.enabled ? "on  " : "off ") + s.path);

        int problems = 0;
        foreach (string path in new[] { DisclaimerPath, TitlePath, CreditPath, BossSelectPath, OptionsPath, KeybindPath, NamePath }) {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            sb.AppendLine("  --- " + path);

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) {
                sb.AppendLine("    " + root.name);
                foreach (Transform child in root.transform)
                    sb.AppendLine("      " + child.name
                                  + Describe(child.GetComponent<Image>())
                                  + Describe(child.GetComponent<Text>()));
            }

            BossSelect select = Object.FindObjectOfType<BossSelect>();
            if (select != null) {
                sb.AppendLine("    BossSelect.slots  = " + (select.slots == null ? "NULL" : select.slots.Length.ToString()));
                sb.AppendLine("    BossSelect.footer = " + (select.footer == null ? "NULL" : "bound"));
                if (select.slots == null || select.slots.Length != BossProgress.SlotsPerScreen) problems++;
                if (select.footer == null) problems++;
                foreach (BossSlot slot in select.slots ?? new BossSlot[0])
                    if (slot.silhouette == null || slot.bossName == null || slot.tries == null
                     || slot.clears == null || slot.deaths == null || slot.best == null || slot.nohit == null)
                        problems++;
            }

            OptionsScreen opts = Object.FindObjectOfType<OptionsScreen>();
            if (opts != null) {
                sb.AppendLine("    OptionsScreen rowRoot=" + (opts.rowRoot == null ? "NULL" : "bound")
                              + " description=" + (opts.description == null ? "NULL" : "bound")
                              + " cursor=" + (opts.cursor == null ? "NULL" : "bound")
                              + " font=" + (opts.font == null ? "NULL" : opts.font.name));
                if (opts.rowRoot == null || opts.description == null || opts.cursor == null || opts.font == null) problems++;
            }

            KeybindSettings binds = Object.FindObjectOfType<KeybindSettings>();
            if (binds != null) {
                KeybindEntry[] all = { binds.Confirm, binds.Cancel, binds.Menu, binds.Up, binds.Left, binds.Down, binds.Right };
                int bound = 0;
                foreach (KeybindEntry e in all)
                    if (e != null && e.Edit != null && e.Reset != null && e.Clear != null
                     && e.Text != null && e.KeyList != null && e.Image != null) bound++;
                sb.AppendLine("    KeybindSettings rows bound = " + bound + "/7"
                              + "  buttons=" + ((binds.Save && binds.ResetAll && binds.Restore && binds.Back) ? "ok" : "MISSING")
                              + "  listening=" + (binds.Listening == null ? "NULL" : "bound"));
                if (bound != 7 || binds.Listening == null
                 || !binds.Save || !binds.ResetAll || !binds.Restore || !binds.Back) problems++;
            }

            NameEntry name = Object.FindObjectOfType<NameEntry>();
            if (name != null) {
                bool ok = name.heading && name.nameText && name.hint && name.underline
                       && name.fade && name.quitLabel && name.doneLabel;
                sb.AppendLine("    NameEntry bindings = " + (ok ? "all bound" : "INCOMPLETE"));
                if (!ok) problems++;
            }

            MainMenu menu = Object.FindObjectOfType<MainMenu>();
            if (menu != null) {
                sb.AppendLine("    MainMenu.entries = " + (menu.entries == null ? "NULL" : menu.entries.Length.ToString()));
                sb.AppendLine("    MainMenu.cursor  = " + (menu.cursor == null ? "NULL" : menu.cursor.name));
                if (menu.entries == null || menu.entries.Length != Rows.Length) problems++;
                if (menu.cursor == null) problems++;
                foreach (Text t in menu.entries ?? new Text[0])
                    if (t == null || t.font == null) problems++;
            }
        }

        sb.AppendLine(problems == 0 ? "SOULBOUND-BATCH-VERIFY-OK" : "SOULBOUND-BATCH-VERIFY-PROBLEMS " + problems);
        Debug.Log(sb.ToString());
    }

    private static string Describe(Image image) {
        if (image == null) return "";
        return "  [Image sprite=" + (image.sprite == null ? "NULL" : image.sprite.name) + "]";
    }

    private static string Describe(Text text) {
        if (text == null) return "";
        return "  [Text \"" + text.text.Replace("\n", " / ") + "\" font=" + (text.font == null ? "NULL" : text.font.name) + "]";
    }

    // ---------------------------------------------------------------- building blocks

    private static Camera MakeCamera() {
        GameObject go = new GameObject("Main Camera", typeof(Camera), typeof(AudioSource));
        go.tag = "MainCamera";
        go.transform.position = new Vector3(320f, 240f, -10f);
        Camera cam = go.GetComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 240f;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = Color.black;
        AudioSource audio = go.GetComponent<AudioSource>();
        audio.playOnAwake = false;
        return cam;
    }

    private static Canvas MakeCanvas() {
        GameObject go = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(640f, 480f);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight  = 0.5f;
        // The game is 640x480 and pixel art, so a fractional scale would sample between
        // pixels. Whole multiples only.
        scaler.referencePixelsPerUnit = 100f;
        return canvas;
    }

    private static void MakeSupport() {
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        new GameObject("GlobalControls", typeof(GlobalControls));
    }

    private static RectTransform Place(GameObject go, Transform parent, Vector2 size, Vector2 at) {
        // A GameObject built around a plain MonoBehaviour gets a Transform, not a
        // RectTransform, and every canvas child needs the latter.
        RectTransform rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.sizeDelta        = size;
        rt.anchoredPosition = at;
        return rt;
    }

    private static Text MakeText(string name, Transform parent, string value, int size,
                                 TextAnchor align, Vector2 box, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Text));
        Text text = go.GetComponent<Text>();
        text.font                = AssetDatabase.LoadAssetAtPath<Font>(MenuFont);
        text.text                = value;
        text.fontSize            = size;
        text.alignment           = align;
        text.color               = Color.white;
        text.horizontalOverflow  = HorizontalWrapMode.Overflow;
        text.verticalOverflow    = VerticalWrapMode.Overflow;
        text.raycastTarget       = false;
        Place(go, parent, box, at);
        return text;
    }

    // ---------------------------------------------------------------- the menu

    private static readonly string[] Rows = { "BOSS SELECT", "OPTIONS", "CREDITS", "QUIT" };

    public static void BuildMenu() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        // The title, in the upper third, at the size it was drawn.
        GameObject logo = new GameObject("Title", typeof(Image));
        Image logoImage = logo.GetComponent<Image>();
        logoImage.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(TitleSprite);
        logoImage.raycastTarget = false;
        Place(logo, canvas.transform, new Vector2(528f, 96f), new Vector2(0f, 130f));

        // The rows sit in a left-aligned column so the soul can rest against the first
        // letter of whichever one is selected rather than floating at a fixed distance.
        List<Text> entries = new List<Text>();
        for (int i = 0; i < Rows.Length; i++) {
            GameObject go = new GameObject(Rows[i], typeof(Text), typeof(Button), typeof(EventTrigger));
            Text text = go.GetComponent<Text>();
            text.font               = AssetDatabase.LoadAssetAtPath<Font>(MenuFont);
            text.text               = Rows[i];
            text.fontSize           = 32;
            text.alignment          = TextAnchor.MiddleLeft;
            text.color              = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow   = VerticalWrapMode.Overflow;
            text.raycastTarget      = true;
            go.GetComponent<Button>().targetGraphic = text;
            Place(go, canvas.transform, new Vector2(300f, 40f), new Vector2(40f, 30f - i * 50f));
            entries.Add(text);
        }

        GameObject soul = new GameObject("Soul", typeof(Image));
        Image soulImage = soul.GetComponent<Image>();
        soulImage.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(SoulSprite);
        soulImage.raycastTarget = false;
        RectTransform soulRect = Place(soul, canvas.transform, new Vector2(32f, 32f), new Vector2(-134f, 30f));

        GameObject script = new GameObject("MenuScript", typeof(MainMenu));
        MainMenu menu = script.GetComponent<MainMenu>();
        menu.entries = entries.ToArray();
        menu.cursor  = soulRect;

        EditorSceneManager.SaveScene(scene, TitlePath);
        Debug.Log("SOULBOUND-BATCH-MENU saved " + TitlePath + " with " + entries.Count + " rows");
    }

    // ---------------------------------------------------------------- the credits

    public static void BuildCredits() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        MakeText("Heading", canvas.transform, "CREDITS", 32, TextAnchor.MiddleCenter,
                 new Vector2(600f, 40f), new Vector2(0f, 195f));

        // Soulbound's own line first, then the engine it stands on. The names below are the
        // attribution the GPLv3 requires and are carried over verbatim.
        string[,] blocks = {
            { "Soulbound",        "PaperTrail" },
            { "Undertale",        "Toby Fox" },
            { "Unitale",          "lvkuln" },
            { "Create Your Frisk", "RhenaudTheLukark" },
        };

        float y = 130f;
        for (int i = 0; i < blocks.GetLength(0); i++) {
            MakeText(blocks[i, 0], canvas.transform, blocks[i, 0], 16, TextAnchor.MiddleCenter,
                     new Vector2(600f, 20f), new Vector2(0f, y));
            MakeText(blocks[i, 0] + " Name", canvas.transform, blocks[i, 1], 24, TextAnchor.MiddleCenter,
                     new Vector2(600f, 28f), new Vector2(0f, y - 24f));
            y -= 62f;
        }

        MakeText("Contributors", canvas.transform, "CREATE YOUR FRISK CONTRIBUTORS", 16,
                 TextAnchor.MiddleCenter, new Vector2(620f, 20f), new Vector2(0f, y));
        MakeText("Contributor Names", canvas.transform,
                 "Eir_nya    Elytrafae    alexytomi\nNyakoFox    nanodesuologist    Shiftenas", 20,
                 TextAnchor.MiddleCenter, new Vector2(620f, 48f), new Vector2(0f, y - 30f));
        MakeText("More", canvas.transform,
                 "and everyone listed in the GitHub repository", 16,
                 TextAnchor.MiddleCenter, new Vector2(620f, 20f), new Vector2(0f, y - 66f));

        MakeText("Back", canvas.transform, "Press Cancel to go back", 16, TextAnchor.MiddleCenter,
                 new Vector2(620f, 20f), new Vector2(0f, -215f));

        new GameObject("CreditsScript", typeof(CreditsScreen));

        EditorSceneManager.SaveScene(scene, CreditPath);
        RegisterScene(CreditPath);
        Debug.Log("SOULBOUND-BATCH-CREDITS saved " + CreditPath);
    }

    // ---------------------------------------------------------------- shared widgets

    /// <summary>A clickable label. Returns the Button; its Text child carries the wording.</summary>
    private static Button MakeButton(string name, Transform parent, string label, int size,
                                     Vector2 box, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Image), typeof(Button));
        Image plate = go.GetComponent<Image>();
        plate.color = new Color(1f, 1f, 1f, 0.10f);
        Place(go, parent, box, at);

        Text text = MakeText(name + " Label", go.transform, label, size, TextAnchor.MiddleCenter, box, Vector2.zero);
        go.GetComponent<Button>().targetGraphic = plate;
        return go.GetComponent<Button>();
    }

    // ---------------------------------------------------------------- the battle box

    /// <summary>
    /// Points the arena's frame at Soulbound's own border sprite.
    ///
    /// Battle.unity is not rebuilt the way the menus were. It is the game rather than a menu:
    /// five and a half thousand lines driven by a dozen scripts that find their objects by
    /// name, and rebuilding it from an empty scene would be a rewrite of the battle system
    /// wearing a layout change's clothes. So the box is reskinned in place.
    /// </summary>
    public static void SkinBattleBox() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.OpenScene(BattlePath, OpenSceneMode.Single);

        Sprite border = AssetDatabase.LoadAssetAtPath<Sprite>(ArenaBorderSprite);
        if (border == null) {
            Debug.Log("SOULBOUND-BATCH-ARENA missing " + ArenaBorderSprite);
            return;
        }

        int changed = 0;
        foreach (GameObject root in scene.GetRootGameObjects()) {
            foreach (Transform t in root.GetComponentsInChildren<Transform>(true)) {
                if (t.name != "arena_border_outer")
                    continue;
                Image image = t.GetComponent<Image>();
                if (image == null)
                    continue;
                image.sprite = border;
                // Sliced with a hollow centre: the frame draws, the arena's own black
                // interior shows through it.
                image.type       = Image.Type.Sliced;
                image.fillCenter = false;
                changed++;
            }
        }

        if (changed > 0) {
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log("SOULBOUND-BATCH-ARENA reskinned " + changed + " arena border(s)");
    }

    // ---------------------------------------------------------------- name entry

    public static void BuildNameEntry() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        // Two lines tall, because a handful of names answer back over two.
        Text heading = MakeText("Heading", canvas.transform, "", 20, TextAnchor.MiddleCenter,
                                new Vector2(620f, 60f), new Vector2(0f, 140f));

        Text nameText = MakeText("Name", canvas.transform, "", 32, TextAnchor.MiddleCenter,
                                 new Vector2(400f, 44f), new Vector2(0f, 50f));

        GameObject rule = new GameObject("Underline", typeof(Image));
        Image underline = rule.GetComponent<Image>();
        underline.raycastTarget = false;
        Place(rule, canvas.transform, new Vector2(260f, 2f), new Vector2(0f, 22f));

        Text hint = MakeText("Hint", canvas.transform, "", 12, TextAnchor.MiddleCenter,
                             new Vector2(620f, 20f), new Vector2(0f, -10f));

        Text quit = MakeChoice("Quit", canvas.transform, new Vector2(-90f, -110f));
        Text done = MakeChoice("Done", canvas.transform, new Vector2(90f, -110f));

        // Last child, so it covers everything when the new game fades out.
        GameObject curtain = new GameObject("Fade", typeof(Image));
        Image fade = curtain.GetComponent<Image>();
        fade.color         = new Color(0f, 0f, 0f, 0f);
        fade.raycastTarget = false;
        Place(curtain, canvas.transform, new Vector2(640f, 480f), Vector2.zero);

        GameObject script = new GameObject("NameEntryScript", typeof(NameEntry));
        NameEntry entry = script.GetComponent<NameEntry>();
        entry.heading   = heading;
        entry.nameText  = nameText;
        entry.hint      = hint;
        entry.underline = underline;
        entry.fade      = fade;
        entry.quitLabel = quit;
        entry.doneLabel = done;

        EditorSceneManager.SaveScene(scene, NamePath);
        Debug.Log("SOULBOUND-BATCH-NAME saved " + NamePath);
    }

    /// <summary>One of the two buttons: a label the pointer can reach.</summary>
    private static Text MakeChoice(string name, Transform parent, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Text), typeof(Button), typeof(EventTrigger));
        Text text = go.GetComponent<Text>();
        text.font               = AssetDatabase.LoadAssetAtPath<Font>(MenuFont);
        text.text               = name;
        text.fontSize           = 18;
        text.alignment          = TextAnchor.MiddleCenter;
        text.color              = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow   = VerticalWrapMode.Overflow;
        text.raycastTarget      = true;
        go.GetComponent<Button>().targetGraphic = text;
        Place(go, parent, new Vector2(120f, 32f), at);
        return text;
    }

    // ---------------------------------------------------------------- the options screen

    public static void BuildOptionsScreen() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        MakeText("Heading", canvas.transform, "OPTIONS", 28, TextAnchor.MiddleCenter,
                 new Vector2(600f, 34f), new Vector2(0f, 212f));

        // The rows are made at runtime from OptionsScreen's own list, so the scene holds only
        // the box they go in. That is the whole reason this screen was rebuilt.
        GameObject root = new GameObject("Rows", typeof(RectTransform));
        RectTransform rowRoot = Place(root, canvas.transform, new Vector2(640f, 480f), Vector2.zero);

        Text description = MakeText("Description", canvas.transform, "", 13, TextAnchor.UpperLeft,
                                    new Vector2(308f, 330f), new Vector2(150f, 15f));
        description.horizontalOverflow = HorizontalWrapMode.Wrap;
        description.verticalOverflow   = VerticalWrapMode.Truncate;

        GameObject soul = new GameObject("Soul", typeof(Image));
        Image soulImage = soul.GetComponent<Image>();
        soulImage.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(SoulSprite);
        soulImage.raycastTarget = false;
        RectTransform soulRect = Place(soul, canvas.transform, new Vector2(24f, 24f), new Vector2(-298f, 160f));

        GameObject script = new GameObject("OptionsScript", typeof(OptionsScreen));
        OptionsScreen options = script.GetComponent<OptionsScreen>();
        options.rowRoot     = rowRoot;
        options.description = description;
        options.cursor      = soulRect;
        options.font        = AssetDatabase.LoadAssetAtPath<Font>(MenuFont);

        EditorSceneManager.SaveScene(scene, OptionsPath);
        Debug.Log("SOULBOUND-BATCH-OPTIONS saved " + OptionsPath);
    }

    // ---------------------------------------------------------------- the keybind screen

    private static readonly string[] Binds = { "Confirm", "Cancel", "Menu", "Up", "Left", "Down", "Right" };

    public static void BuildKeybinds() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        MakeText("Heading", canvas.transform, "KEYBINDS", 28, TextAnchor.MiddleCenter,
                 new Vector2(600f, 34f), new Vector2(0f, 212f));

        Text listening = MakeText("Listening", canvas.transform, "", 13, TextAnchor.MiddleCenter,
                                  new Vector2(620f, 20f), new Vector2(0f, 180f));

        GameObject script = new GameObject("KeybindScript", typeof(KeybindSettings));
        KeybindSettings keys = script.GetComponent<KeybindSettings>();
        keys.Listening = listening;

        KeybindEntry[] rows = new KeybindEntry[Binds.Length];
        for (int i = 0; i < Binds.Length; i++) {
            float y = 136f - i * 42f;

            // The object's name IS the keybind: KeybindEntry.Start reads gameObject.name and
            // looks it up in KeyboardInput.playerKeys, so these must match the dictionary.
            GameObject go = new GameObject(Binds[i], typeof(RectTransform), typeof(KeybindEntry));
            Place(go, canvas.transform, new Vector2(620f, 40f), new Vector2(0f, y));

            KeybindEntry entry = go.GetComponent<KeybindEntry>();

            GameObject rule = new GameObject("Rule", typeof(Image));
            Image ruleImage = rule.GetComponent<Image>();
            ruleImage.raycastTarget = false;
            Place(rule, go.transform, new Vector2(600f, 1f), new Vector2(0f, -18f));

            entry.Image   = ruleImage;
            entry.Text    = MakeText("Name",    go.transform, Binds[i], 16, TextAnchor.MiddleLeft, new Vector2(90f,  22f), new Vector2(-255f, 0f));
            entry.KeyList = MakeText("KeyList", go.transform, "",       13, TextAnchor.MiddleLeft, new Vector2(240f, 22f), new Vector2(-80f,  0f));
            entry.Edit    = MakeButton("Edit",  go.transform, "Edit",   13, new Vector2(70f, 24f), new Vector2(90f,  0f));
            entry.Reset   = MakeButton("Reset", go.transform, "Reset",  13, new Vector2(70f, 24f), new Vector2(170f, 0f));
            entry.Clear   = MakeButton("Clear", go.transform, "Clear",  13, new Vector2(70f, 24f), new Vector2(250f, 0f));

            rows[i] = entry;
        }

        keys.Confirm = rows[0];
        keys.Cancel  = rows[1];
        keys.Menu    = rows[2];
        keys.Up      = rows[3];
        keys.Left    = rows[4];
        keys.Down    = rows[5];
        keys.Right   = rows[6];

        keys.Save     = MakeButton("Save",     canvas.transform, "Save",      14, new Vector2(140f, 28f), new Vector2(-240f, -180f));
        keys.ResetAll = MakeButton("ResetAll", canvas.transform, "Reset All", 14, new Vector2(140f, 28f), new Vector2(-80f,  -180f));
        keys.Restore  = MakeButton("Restore",  canvas.transform, "Restore",   14, new Vector2(140f, 28f), new Vector2(80f,   -180f));
        keys.Back     = MakeButton("Back",     canvas.transform, "Back",      14, new Vector2(140f, 28f), new Vector2(240f,  -180f));

        MakeText("Hint", canvas.transform,
                 "Edit listens for a key. Press it again, or ESC, to stop.", 12,
                 TextAnchor.MiddleCenter, new Vector2(620f, 18f), new Vector2(0f, -215f));

        EditorSceneManager.SaveScene(scene, KeybindPath);
        Debug.Log("SOULBOUND-BATCH-KEYBINDS saved " + KeybindPath + " with " + rows.Length + " binds");
    }

    // ---------------------------------------------------------------- the boss select

    // Column centres, in canvas coordinates with the origin at the middle of a 640x480 screen.
    private const float ArtX    = -290f;
    private const float NameX   = -176f;
    private const float TriesX  = -52f;
    private const float ClearsX = 20f;
    private const float DeathsX = 92f;
    private const float BestX   = 180f;
    private const float NoHitX  = 270f;

    public static void BuildBossSelect() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        MakeText("Heading", canvas.transform, "BOSS SELECT", 28, TextAnchor.MiddleCenter,
                 new Vector2(600f, 34f), new Vector2(0f, 212f));

        // Column headings. The name column has none: the row's own name is its heading.
        MakeText("HeadTries",  canvas.transform, "TRIES",  12, TextAnchor.MiddleCenter, new Vector2(70f, 16f), new Vector2(TriesX,  178f));
        MakeText("HeadClears", canvas.transform, "CLEARS", 12, TextAnchor.MiddleCenter, new Vector2(70f, 16f), new Vector2(ClearsX, 178f));
        MakeText("HeadDeaths", canvas.transform, "DEATHS", 12, TextAnchor.MiddleCenter, new Vector2(70f, 16f), new Vector2(DeathsX, 178f));
        MakeText("HeadBest",   canvas.transform, "BEST",   12, TextAnchor.MiddleCenter, new Vector2(80f, 16f), new Vector2(BestX,   178f));
        MakeText("HeadNoHit",  canvas.transform, "NO HIT", 12, TextAnchor.MiddleCenter, new Vector2(80f, 16f), new Vector2(NoHitX,  178f));

        Sprite silhouette = AssetDatabase.LoadAssetAtPath<Sprite>(SilhouetteSprite);

        List<BossSlot> slots = new List<BossSlot>();
        for (int i = 0; i < BossProgress.SlotsPerScreen; i++) {
            float y = 146f - i * 48f;
            string tag = "Slot" + (i + 1);

            GameObject art = new GameObject(tag + " Art", typeof(Image));
            Image image = art.GetComponent<Image>();
            image.sprite        = silhouette;
            image.raycastTarget = false;
            Place(art, canvas.transform, new Vector2(40f, 40f), new Vector2(ArtX, y));

            slots.Add(new BossSlot {
                silhouette = image,
                bossName   = MakeText(tag + " Name",   canvas.transform, "", 16, TextAnchor.MiddleLeft,   new Vector2(172f, 22f), new Vector2(NameX,   y)),
                tries      = MakeText(tag + " Tries",  canvas.transform, "", 16, TextAnchor.MiddleCenter, new Vector2(70f,  22f), new Vector2(TriesX,  y)),
                clears     = MakeText(tag + " Clears", canvas.transform, "", 16, TextAnchor.MiddleCenter, new Vector2(70f,  22f), new Vector2(ClearsX, y)),
                deaths     = MakeText(tag + " Deaths", canvas.transform, "", 16, TextAnchor.MiddleCenter, new Vector2(70f,  22f), new Vector2(DeathsX, y)),
                best       = MakeText(tag + " Best",   canvas.transform, "", 16, TextAnchor.MiddleCenter, new Vector2(80f,  22f), new Vector2(BestX,   y)),
                nohit      = MakeText(tag + " NoHit",  canvas.transform, "", 16, TextAnchor.MiddleCenter, new Vector2(80f,  22f), new Vector2(NoHitX,  y))
            });
        }

        // The selected boss's one line. Seven rows cannot each carry a subtitle, so the screen
        // shows the one belonging to whatever is highlighted.
        Text footer = MakeText("Footer", canvas.transform, "", 14, TextAnchor.MiddleCenter,
                               new Vector2(620f, 20f), new Vector2(0f, -196f));

        GameObject script = new GameObject("BossSelectScript", typeof(BossSelect));
        BossSelect select = script.GetComponent<BossSelect>();
        select.slots  = slots.ToArray();
        select.footer = footer;

        EditorSceneManager.SaveScene(scene, BossSelectPath);
        Debug.Log("SOULBOUND-BATCH-BOSSSELECT saved " + BossSelectPath + " with " + slots.Count + " slots");
    }

    // ---------------------------------------------------------------- the disclaimer

    public static void BuildDisclaimer() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        MakeText("Heading", canvas.transform, "DISCLAIMER", 32, TextAnchor.MiddleCenter,
                 new Vector2(600f, 40f), new Vector2(0f, 160f));

        // The wording is the wording the screen already carried, moved out of C# and into the
        // scene. It is a legal notice, so it is not the place to be inventive.
        MakeText("Fanwork", canvas.transform,
                 "Soulbound is a fan work.\nIt is not owned by, endorsed by, or affiliated with\nToby Fox or Undertale.",
                 20, TextAnchor.MiddleCenter, new Vector2(620f, 80f), new Vector2(0f, 70f));

        MakeText("Licence", canvas.transform,
                 "It runs on Create Your Frisk, which is free software\nunder the GPLv3. This game is free.\nDo not sell it, or anything made with it.",
                 20, TextAnchor.MiddleCenter, new Vector2(620f, 80f), new Vector2(0f, -20f));

        MakeText("Build", canvas.transform, "This is an early build.", 20, TextAnchor.MiddleCenter,
                 new Vector2(620f, 24f), new Vector2(0f, -100f));

        Text version = MakeText("Version", canvas.transform, "v0.0.0", 16, TextAnchor.MiddleCenter,
                                new Vector2(620f, 20f), new Vector2(0f, -140f));

        MakeText("Prompt", canvas.transform, "Press any key to continue", 16, TextAnchor.MiddleCenter,
                 new Vector2(620f, 20f), new Vector2(0f, -200f));

        GameObject script = new GameObject("DisclaimerScript", typeof(DisclaimerScript));
        script.GetComponent<DisclaimerScript>().Version = version;

        EditorSceneManager.SaveScene(scene, DisclaimerPath);
        Debug.Log("SOULBOUND-BATCH-DISCLAIMER saved " + DisclaimerPath);
    }

    /// <summary>Adds a scene to the build list if it is not already in it.</summary>
    private static void RegisterScene(string path) {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach (EditorBuildSettingsScene s in scenes)
            if (s.path == path)
                return;
        scenes.Add(new EditorBuildSettingsScene(path, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    /// <summary>
    /// Builds a player, because there is no test suite here and the build is the check. Writes
    /// outside the repository so a verification run never leaves anything to clean up.
    /// </summary>
    public static void BuildPlayer() {
        List<string> scenes = new List<string>();
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            if (s.enabled)
                scenes.Add(s.path);

        string target = System.Environment.GetEnvironmentVariable("SOULBOUND_BUILD_PATH");
        if (string.IsNullOrEmpty(target))
            target = "/tmp/soulbound-verify/Soulbound.app";

        UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(
            scenes.ToArray(), target, BuildTarget.StandaloneOSX, BuildOptions.None);

        UnityEditor.Build.Reporting.BuildSummary summary = report.summary;
        Debug.Log("SOULBOUND-BATCH-BUILD " + summary.result
                  + " errors=" + summary.totalErrors
                  + " warnings=" + summary.totalWarnings
                  + " scenes=" + scenes.Count);
    }

    /// <summary>Everything the menu rebuild needs, in one run.</summary>
    public static void BuildAll() {
        BuildCredits();
        BuildMenu();
        BuildDisclaimer();
        BuildBossSelect();
        BuildOptionsScreen();
        BuildKeybinds();
        BuildNameEntry();
        AssetDatabase.SaveAssets();
        Debug.Log("SOULBOUND-BATCH-DONE");
    }
}

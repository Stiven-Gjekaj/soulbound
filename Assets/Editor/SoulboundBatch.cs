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

    private const string SoulSprite  = "Assets/Sprites/Soul_Cursor.png";

    // The menu logo, in three pieces rather than one picture, because the opening reveals
    // them one at a time.
    private const string LogoSoulSprite  = "Assets/Sprites/Soul.png";
    private const string LogoSwordSprite = "Assets/Sprites/Sword.png";
    private const string LogoTitleSprite = "Assets/Sprites/Soul_Title.png";
    private const string ScopeSprite     = "Assets/Sprites/Menu_Scope.png";
    private const string MoteSprite      = "Assets/Sprites/Menu_Mote.png";
    private const string LockSprite = "Assets/Sprites/Lock.png";
    private const string WheelSprite = "Assets/Sprites/Wheel_Rim.png";
    private const string ArenaBorderSprite = "Assets/Sprites/Arena_Border.png";
    private const string WallSprite   = "Assets/Sprites/Disclaimer_Wall.png";
    private const string CobwebSprite = "Assets/Sprites/Cobweb.png";
    private const string FisheyeShader   = "Assets/Shaders/MenuFisheye.shader";
    private const string FisheyeMaterial = "Assets/Shaders/MenuFisheye.mat";
    private const string MenuFont    = "Assets/Fonts/PixelOperator/PixelOperator-Bold.ttf";
    private const string CameraPrefab = "Assets/Resources/Prefabs/Main Camera.prefab";

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

            // Every screen needs these, and the ways they fail are all invisible until the
            // game runs: no ScreenResolution means keys are ignored, no AudioListener means
            // silence, no GlobalControls means no input at all.
            if (Object.FindObjectOfType<ScreenResolution>() == null) { sb.AppendLine("    NO ScreenResolution"); problems++; }
            if (Object.FindObjectOfType<AudioListener>()    == null) { sb.AppendLine("    NO AudioListener");    problems++; }
            if (Object.FindObjectOfType<GlobalControls>()   == null) { sb.AppendLine("    NO GlobalControls");   problems++; }

            BossSelect select = Object.FindObjectOfType<BossSelect>();
            if (select != null) {
                bool panel = select.portrait && select.bossName && select.subtitle && select.hint
                          && select.tries && select.clears && select.deaths && select.best && select.nohit;
                sb.AppendLine("    BossSelect slots = " + (select.slots == null ? "NULL" : select.slots.Length.ToString())
                              + "  rim=" + (select.wheelRim == null ? "NULL" : "bound")
                              + "  panel=" + (panel ? "bound" : "INCOMPLETE"));
                if (select.slots == null || select.slots.Length != BossProgress.SlotsPerScreen) problems++;
                if (select.wheelRim == null || !panel) problems++;
                foreach (BossSlot slot in select.slots ?? new BossSlot[0])
                    if (slot.icon == null || slot.padlock == null || slot.pivot == null)
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
                bool ok = name.heading && name.nameText && name.underline
                       && name.fade && name.quitLabel && name.doneLabel;
                sb.AppendLine("    NameEntry bindings = " + (ok ? "all bound" : "INCOMPLETE"));
                if (!ok) problems++;
            }

            MainMenu menu = Object.FindObjectOfType<MainMenu>();
            if (menu != null) {
                sb.AppendLine("    MainMenu.entries = " + (menu.entries == null ? "NULL" : menu.entries.Length.ToString()));
                sb.AppendLine("    MainMenu logo    = soul " + Bound(menu.soul) + ", sword " + Bound(menu.sword)
                              + ", title " + Bound(menu.title));
                sb.AppendLine("    MainMenu scope   = " + Bound(menu.scope) + ", ink " + Bound(menu.scopeInk)
                              + ", motes " + Bound(menu.motes) + ", fade " + Bound(menu.fade));
                if (menu.entries == null || menu.entries.Length != Rows.Length) problems++;
                // Every piece of the logo, because the opening reveals them one at a time and
                // a null one is a beat that silently does nothing rather than an error.
                if (!menu.soul || !menu.sword || !menu.title) problems++;
                if (!menu.scope || !menu.scopeInk || !menu.motes || !menu.fade) problems++;
                foreach (Text t in menu.entries ?? new Text[0])
                    if (t == null || t.font == null) problems++;

                foreach (Image piece in new[] { menu.soul, menu.sword, menu.title, menu.scopeInk })
                    if (piece != null && piece.sprite == null) {
                        sb.AppendLine("    NO sprite on " + piece.name);
                        problems++;
                    }
                if (menu.motes != null && menu.motes.mote == null) {
                    sb.AppendLine("    NO mote sprite");
                    problems++;
                }

                // The fisheye fails quietly in two ways that both leave a flat screen and no
                // error: a missing material, and a canvas left on Overlay so the frame never
                // reaches the effect at all.
                ScreenFisheye lens = Object.FindObjectOfType<ScreenFisheye>();
                Canvas menuCanvas = Object.FindObjectOfType<Canvas>();
                sb.AppendLine("    ScreenFisheye    = " + Bound(lens)
                              + ", material " + (lens == null ? "n/a" : Bound(lens.material))
                              + ", canvas " + (menuCanvas == null ? "NULL" : menuCanvas.renderMode.ToString()));
                if (lens == null || lens.material == null) problems++;
                if (menuCanvas == null || menuCanvas.renderMode != RenderMode.ScreenSpaceCamera
                 || menuCanvas.worldCamera == null) problems++;
            }
        }

        // Every clip key the menus ask for, and whether asking is safe.
        //
        // The registry raises a miss rather than returning one, so a key it cannot reach
        // throws out of whatever called it. That is not a hypothetical: mus_menu is on disk
        // inside the @Title mod, the registry only searches the loaded mod and Default, and
        // the resulting throw landed in front of the line that let the menu accept a keypress.
        // The menu came up looking finished and ignored the keyboard.
        //
        // MenuAudio is what turns those misses back into nulls. What matters is not whether a
        // key resolves, since a silent menu is fine, but that asking never throws.
        sb.AppendLine("  --- menu audio");
        foreach (string key in new[] { "mus_menu", "mus_barrier" }) {
            try {
                sb.AppendLine("    music " + key + " = " + (MenuAudio.Music(key) == null ? "absent, handled" : "resolves"));
            } catch (System.Exception e) {
                sb.AppendLine("    music " + key + " THREW " + e.GetType().Name);
                problems++;
            }
        }
        foreach (string key in new[] { "menumove", "menuconfirm", "slice", "intro_holdup" }) {
            try {
                sb.AppendLine("    sound " + key + " = " + (MenuAudio.Sound(key) == null ? "absent, handled" : "resolves"));
            } catch (System.Exception e) {
                sb.AppendLine("    sound " + key + " THREW " + e.GetType().Name);
                problems++;
            }
        }

        sb.AppendLine(problems == 0 ? "SOULBOUND-BATCH-VERIFY-OK" : "SOULBOUND-BATCH-VERIFY-PROBLEMS " + problems);
        Debug.Log(sb.ToString());
    }

    private static string Bound(Object o) { return o == null ? "NULL" : "ok"; }

    private static string Describe(Image image) {
        if (image == null) return "";
        return "  [Image sprite=" + (image.sprite == null ? "NULL" : image.sprite.name) + "]";
    }

    private static string Describe(Text text) {
        if (text == null) return "";
        return "  [Text \"" + text.text.Replace("\n", " / ") + "\" font=" + (text.font == null ? "NULL" : text.font.name) + "]";
    }

    // ---------------------------------------------------------------- building blocks

    /// <summary>
    /// The scene's camera, instanced from the prefab rather than built by hand.
    ///
    /// That prefab is not just a camera. It carries ScreenResolution, GlobalControls and the
    /// AudioListener, and building a bare camera instead cost two things that only show up when
    /// the game runs: with no ScreenResolution, hasInitialized never became true and the
    /// disclaimer ignored every key forever, and with no AudioListener nothing made a sound.
    /// </summary>
    private static Camera MakeCamera() {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(CameraPrefab);
        GameObject go = prefab != null ? (GameObject)PrefabUtility.InstantiatePrefab(prefab)
                                       : new GameObject("Main Camera", typeof(Camera), typeof(AudioSource), typeof(AudioListener));
        go.name = "Main Camera";
        go.tag  = "MainCamera";
        go.transform.position = new Vector3(320f, 240f, -10f);

        Camera cam = go.GetComponent<Camera>();
        cam.orthographic     = true;
        cam.orthographicSize = 240f;
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = Color.black;

        AudioSource audio = go.GetComponent<AudioSource>();
        if (audio != null)
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

    /// <summary>Only the event system. GlobalControls rides on the camera prefab.</summary>
    private static void MakeSupport() {
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
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

    /// <summary>
    /// Stretches an object to its parent plus a margin on every side, so it covers the canvas
    /// whatever size the canvas turns out to be and has that margin spare to move inside.
    ///
    /// A fixed size cannot do this. The canvas scales with the window, and in fullscreen on a
    /// wide monitor it reports something like 740 by 416 rather than the 640 by 480 the layout
    /// is authored against, which is wider than a backdrop sized for 640 and leaves a strip of
    /// nothing down each side.
    /// </summary>
    private static RectTransform PlaceStretched(GameObject go, Transform parent, float margin) {
        RectTransform rt = go.GetComponent<RectTransform>() ?? go.AddComponent<RectTransform>();
        rt.SetParent(parent, false);
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(-margin, -margin);
        rt.offsetMax = new Vector2(margin, margin);
        rt.anchoredPosition = Vector2.zero;
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

    /// <summary>
    /// The four entries, and where each sits. Left column first, so the order matches the
    /// order MainMenu.Activate reads and the grid it derives from the index.
    ///
    /// The lower row is pulled in toward the middle and set two points smaller than the upper
    /// one. That is the whole of the inward lean in the layout: a lower row nearer the centre
    /// and smaller reads as a row further away, and the parallax under the pointer does the
    /// rest at runtime.
    /// </summary>
    private struct Entry {
        public string Label;
        public Vector2 At;
        public int Size;
        public Entry(string label, Vector2 at, int size) { Label = label; At = at; Size = size; }
    }

    /// <summary>
    /// Pulled in from the edges further than the layout wants, because the fisheye pushes
    /// everything outward and the amount depends on where it started. The corners are pinned
    /// by the shader, but the middles of the left and right edges are not: content authored
    /// against the frame edge there ends up about eight per cent past it and is simply gone.
    /// The scope around the widest entry is the thing that runs out of room first, so the
    /// columns sit where its outer bracket still lands inside the frame after the bend.
    /// </summary>
    private static readonly Entry[] Rows = {
        new Entry("BOSS SELECT", new Vector2(-176f,  54f), 19),
        new Entry("OPTIONS",     new Vector2(-158f, -46f), 17),
        new Entry("CREDITS",     new Vector2( 176f,  54f), 19),
        new Entry("QUIT",        new Vector2( 158f, -46f), 17),
    };

    public static void BuildMenu() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Camera cam = MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        // Through the camera rather than over it. An Overlay canvas is drawn after the camera
        // has finished, so it never reaches an image effect, and the fisheye below would curve
        // an empty black frame while the menu sat flat on top of it.
        canvas.renderMode    = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera   = cam;
        canvas.planeDistance = 5f;

        ScreenFisheye lens = cam.gameObject.AddComponent<ScreenFisheye>();
        lens.material = LoadOrCreateMaterial(FisheyeShader, FisheyeMaterial);

        // The specks, first, so they fall behind everything. Stretched rather than sized: the
        // canvas is not always the 640x480 the layout is authored against, and in fullscreen
        // it reports whatever the window scaler makes of the monitor.
        // RectTransform up front. A GameObject built around a plain MonoBehaviour gets a
        // Transform, and adding the RectTransform afterwards is too late for anything that
        // has already asked for one.
        GameObject dust = new GameObject("Motes", typeof(RectTransform), typeof(MenuMotes));
        PlaceStretched(dust, canvas.transform, 0f);
        MenuMotes motes = dust.GetComponent<MenuMotes>();
        motes.mote = AssetDatabase.LoadAssetAtPath<Sprite>(MoteSprite);

        // The logo, three pieces in one group so the lean can move all of it at once. The
        // sword goes in first: it is driven down through the soul, and the soul has to cover
        // the part of the blade that is inside it.
        GameObject group = new GameObject("Logo", typeof(RectTransform));
        RectTransform logo = Place(group, canvas.transform, new Vector2(200f, 300f), new Vector2(0f, 8f));

        Image sword = MakeSprite("Sword", logo, LogoSwordSprite, new Vector2(80f, 240f), new Vector2(0f, 22f));
        Image soul  = MakeSprite("Soul",  logo, LogoSoulSprite,  new Vector2(160f, 160f), Vector2.zero);
        Image title = MakeSprite("Title", logo, LogoTitleSprite, new Vector2(160f, 160f), Vector2.zero);

        // The scope before the words, so a word is never drawn under its own highlight.
        Image scope = MakeSprite("Scope", canvas.transform, ScopeSprite,
                                 new Vector2(196f, 36f), Rows[0].At);

        List<Text> entries = new List<Text>();
        foreach (Entry row in Rows)
            entries.Add(MakeText(row.Label, canvas.transform, row.Label, row.Size,
                                 TextAnchor.MiddleCenter, new Vector2(196f, 36f), row.At));

        // Last, so it covers everything while the menu comes up out of black.
        GameObject cover = new GameObject("Fade", typeof(Image));
        Image menuFade = cover.GetComponent<Image>();
        menuFade.color         = Color.black;
        menuFade.raycastTarget = false;
        PlaceStretched(cover, canvas.transform, 0f);

        GameObject script = new GameObject("MenuScript", typeof(MainMenu));
        MainMenu menu = script.GetComponent<MainMenu>();
        menu.entries  = entries.ToArray();
        menu.soul     = soul;
        menu.sword    = sword;
        menu.title    = title;
        menu.scope    = scope.rectTransform;
        menu.scopeInk = scope;
        menu.fade     = menuFade;
        menu.motes    = motes;

        EditorSceneManager.SaveScene(scene, TitlePath);
        Debug.Log("SOULBOUND-BATCH-MENU saved " + TitlePath + " with " + entries.Count
                  + " entries two to a side of the logo");
    }

    /// <summary>
    /// The material the fisheye runs through, making it on first use.
    ///
    /// It has to exist as an asset rather than be built at runtime. A shader reaches a player
    /// build only if something in a scene refers to it, and the reference that carries it is a
    /// material: a component holding a shader by name would compile, ship without the shader,
    /// and fall through to an unbent screen with no error to explain why.
    /// </summary>
    private static Material LoadOrCreateMaterial(string shaderPath, string materialPath) {
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existing != null)
            return existing;

        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
        if (shader == null) {
            Debug.LogError("SOULBOUND-BATCH no shader at " + shaderPath);
            return null;
        }

        Material made = new Material(shader);
        AssetDatabase.CreateAsset(made, materialPath);
        AssetDatabase.SaveAssets();
        Debug.Log("SOULBOUND-BATCH-MATERIAL created " + materialPath);
        return made;
    }

    /// <summary>An Image on a sprite, placed. The menu builds enough of these to be worth one.</summary>
    private static Image MakeSprite(string name, Transform parent, string asset, Vector2 size, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Image));
        Image image = go.GetComponent<Image>();
        image.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(asset);
        image.raycastTarget = false;
        Place(go, parent, size, at);
        return image;
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

    /// <summary>
    /// A button the keyboard presses. It keeps its Button component, because that is what
    /// carries the click handler the selection invokes, but it is not a raycast target: the
    /// pointer does nothing anywhere in the menus.
    /// </summary>
    private static Button MakeButton(string name, Transform parent, string label, int size,
                                     Vector2 box, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Image), typeof(Button));
        Image plate = go.GetComponent<Image>();
        plate.color         = new Color(1f, 1f, 1f, 0.10f);
        plate.raycastTarget = false;
        Place(go, parent, box, at);

        Text text = MakeText(name + " Label", go.transform, label, size, TextAnchor.MiddleCenter, box, Vector2.zero);
        go.GetComponent<Button>().targetGraphic = plate;
        return go.GetComponent<Button>();
    }

    /// <summary>
    /// Renders a screen to a PNG so it can be looked at without running the game.
    ///
    /// The rows are filled here with what BossSelect would draw on a fresh save, because the
    /// scene stores them empty and the real values only arrive at runtime. Must run WITHOUT
    /// -nographics, and the canvas is switched off Overlay for the shot: an Overlay canvas is
    /// drawn after the camera rather than through it, so it never reaches a RenderTexture.
    /// The scene is not saved.
    /// </summary>
    public static void ShotBossSelect() {
        EditorSceneManager.OpenScene(BossSelectPath, OpenSceneMode.Single);

        BossSelect select = Object.FindObjectOfType<BossSelect>();

        // Which entry the shot is taken on. Everything below derives from this one number,
        // because a shot that filled the panel from one entry and the wheel from another would
        // show a screen the game cannot produce, and a picture that lies is worse than none.
        int shown = 0;
        string want = System.Environment.GetEnvironmentVariable("SOULBOUND_SHOT_SLOT");
        if (!string.IsNullOrEmpty(want))
            int.TryParse(want, out shown);

        // The registry as it stands: four playable entries, then the teased one, then nothing.
        string[] names    = { "Illia, The Tutor", "???", "???", "???", "???", "???", "???" };
        string[] arts     = { "illia", "teased", null, null, null, null, null };
        string[] subs     = { "Not built yet", "", "", "", "", "", "" };
        bool[]   playable = { true, false, false, false, false, false, false };
        bool[]   teased   = { false, true, false, false, false, false, false };

        Color dim  = new Color(0.42f, 0.42f, 0.42f, 1f);
        Color grey = new Color(0.5f, 0.5f, 0.5f, 1f);

        select.bossName.text  = names[shown];
        select.bossName.color = playable[shown] ? Color.white : grey;
        select.subtitle.text  = playable[shown] ? subs[shown] : "";
        select.subtitle.color = grey;
        select.hint.text      = playable[shown] ? "Confirm to fight" : (teased[shown] ? "Not in this build" : "");

        Text[] values  = { select.tries, select.clears, select.deaths, select.best, select.nohit };
        string[] real  = { "0", "0", "0", "-", "-" };
        for (int i = 0; i < values.Length; i++) {
            values[i].text  = playable[shown] ? real[i] : "?";
            values[i].color = playable[shown] ? new Color(1f, 1f, 0f, 1f) : grey;
        }

        Sprite shownArt = arts[shown] == null ? null
            : AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mods/Soulbound/Sprites/Bosses/" + arts[shown] + ".png");
        select.portrait.enabled = shownArt != null;
        if (shownArt != null) {
            select.portrait.sprite = shownArt;
            select.portrait.color  = playable[shown] ? Color.white : dim;
        }
        select.portraitLock.enabled = !playable[shown];

        // And the wheel, laid out the way TurnWheel would with that same entry selected and
        // nothing left to turn.
        int count = BossProgress.SlotsPerScreen, half = count / 2;
        for (int i = 0; i < select.slots.Length; i++) {
            BossSlot slot = select.slots[i];
            float offset = ((i - shown + half + count) % count) - half;
            float away   = Mathf.Abs(offset);
            if (away > select.wheelReach) {
                slot.icon.enabled = false;
                slot.padlock.enabled = false;
                continue;
            }

            float radians = offset * select.wheelStep * Mathf.Deg2Rad;
            slot.pivot.anchoredPosition = select.wheelCentre
                + new Vector2(Mathf.Cos(radians), -Mathf.Sin(radians)) * select.wheelRadius;
            float near = Mathf.Clamp01(1f - away / select.wheelReach);
            slot.pivot.localScale = Vector3.one * Mathf.Lerp(0.55f, 1.35f, near);

            float fade = Mathf.Lerp(0.15f, 1f, near);
            Sprite art = arts[i] == null ? null
                : AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Mods/Soulbound/Sprites/Bosses/" + arts[i] + ".png");
            slot.icon.enabled = art != null;
            if (art != null) {
                slot.icon.sprite = art;
                Color tint = playable[i] ? Color.white : dim;
                tint.a = fade;
                slot.icon.color = tint;
            }
            slot.padlock.enabled = !playable[i];
            slot.padlock.color   = new Color(1f, 1f, 1f, fade);
        }

        Camera cam = Camera.main;
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera  = cam;
        canvas.planeDistance = 5f;
        Canvas.ForceUpdateCanvases();

        RenderTexture rt = new RenderTexture(640, 480, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        Texture2D shot = new Texture2D(640, 480, TextureFormat.RGB24, false);
        shot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
        shot.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        string outPath = System.Environment.GetEnvironmentVariable("SOULBOUND_SHOT");
        if (string.IsNullOrEmpty(outPath))
            outPath = "/tmp/bossselect.png";
        System.IO.File.WriteAllBytes(outPath, shot.EncodeToPNG());
        Debug.Log("SOULBOUND-BATCH-SHOT wrote " + outPath);
    }

    /// <summary>
    /// Renders the disclaimer with its cover cleared. The cover sits opaque in the scene,
    /// because that is where the fade up starts from, so a shot taken as saved is black.
    /// </summary>
    public static void ShotDisclaimer() {
        EditorSceneManager.OpenScene(DisclaimerPath, OpenSceneMode.Single);
        DisclaimerScript screen = Object.FindObjectOfType<DisclaimerScript>();
        if (screen != null && screen.fade != null)
            screen.fade.color = new Color(0f, 0f, 0f, 0f);
        Shoot("/tmp/disclaimer.png");
    }

    /// <summary>
    /// Renders the menu as the opening leaves it, with the first entry selected.
    ///
    /// The scene is saved in the state the opening starts from, which is a black screen with
    /// an invisible logo on it, so a shot taken as saved shows nothing. Everything the opening
    /// would have brought up is brought up here instead.
    /// </summary>
    public static void ShotMenu() {
        EditorSceneManager.OpenScene(TitlePath, OpenSceneMode.Single);

        MainMenu menu = Object.FindObjectOfType<MainMenu>();
        for (int i = 0; i < menu.entries.Length; i++)
            menu.entries[i].color = i == 0 ? new Color(1f, 0.93f, 0.55f, 1f)
                                           : new Color(0.80f, 0.80f, 0.80f, 1f);
        if (menu.scope != null)
            menu.scope.anchoredPosition = menu.entries[0].rectTransform.anchoredPosition;
        if (menu.fade != null)
            menu.fade.color = new Color(0f, 0f, 0f, 0f);

        Shoot("/tmp/menu.png");
    }

    /// <summary>
    /// Renders whatever scene is open to a PNG. The canvas is switched off Overlay first,
    /// because an Overlay canvas is drawn after the camera rather than through it and so never
    /// reaches a RenderTexture. The scene is not saved.
    /// </summary>
    private static void Shoot(string fallbackPath) {
        Camera cam = Camera.main;
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        canvas.renderMode    = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera   = cam;
        canvas.planeDistance = 5f;
        Canvas.ForceUpdateCanvases();

        RenderTexture rt = new RenderTexture(640, 480, 24);
        cam.targetTexture = rt;
        cam.Render();
        RenderTexture.active = rt;

        Texture2D shot = new Texture2D(640, 480, TextureFormat.RGB24, false);
        shot.ReadPixels(new Rect(0, 0, 640, 480), 0, 0);
        shot.Apply();
        cam.targetTexture = null;
        RenderTexture.active = null;

        string outPath = System.Environment.GetEnvironmentVariable("SOULBOUND_SHOT");
        if (string.IsNullOrEmpty(outPath))
            outPath = fallbackPath;
        System.IO.File.WriteAllBytes(outPath, shot.EncodeToPNG());
        Debug.Log("SOULBOUND-BATCH-SHOT wrote " + outPath);
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

    /// <summary>
    /// Gives the in-fight timer an object in Battle.unity, which is the last thing the
    /// milestone lists for the battle screen. It used to build its own at runtime.
    ///
    /// Top right, clear of the arena in the middle and the stats along the bottom, and right
    /// aligned so the reading does not shift as the tenths tick over.
    /// </summary>
    public static void AddFightTimer() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.OpenScene(BattlePath, OpenSceneMode.Single);

        Canvas canvas = null;
        foreach (GameObject root in scene.GetRootGameObjects()) {
            foreach (Canvas c in root.GetComponentsInChildren<Canvas>(true)) {
                if (c.gameObject.name != "Canvas")
                    continue;
                canvas = c;
                break;
            }
            if (canvas != null)
                break;
        }
        if (canvas == null) {
            Debug.Log("SOULBOUND-BATCH-TIMER no Canvas in " + BattlePath);
            return;
        }

        // Idempotent: running this twice must not leave two timers behind.
        foreach (FightTimer old in Object.FindObjectsOfType<FightTimer>())
            Object.DestroyImmediate(old.gameObject);

        GameObject holder = new GameObject("FightTimer", typeof(RectTransform), typeof(FightTimer));
        Place(holder, canvas.transform, new Vector2(140f, 20f), new Vector2(230f, 212f));

        Text display = MakeText("Display", holder.transform, "0:00.0", 14, TextAnchor.MiddleRight,
                                new Vector2(140f, 20f), Vector2.zero);

        holder.GetComponent<FightTimer>().display = display;

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("SOULBOUND-BATCH-TIMER added the timer object to " + BattlePath);
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
        entry.underline = underline;
        entry.fade      = fade;
        entry.quitLabel = quit;
        entry.doneLabel = done;

        EditorSceneManager.SaveScene(scene, NamePath);
        Debug.Log("SOULBOUND-BATCH-NAME saved " + NamePath);
    }

    /// <summary>One of the two buttons: a label the pointer can reach.</summary>
    private static Text MakeChoice(string name, Transform parent, Vector2 at) {
        GameObject go = new GameObject(name, typeof(Text));
        Text text = go.GetComponent<Text>();
        text.font               = AssetDatabase.LoadAssetAtPath<Font>(MenuFont);
        text.text               = name;
        text.fontSize           = 18;
        text.alignment          = TextAnchor.MiddleCenter;
        text.color              = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow   = VerticalWrapMode.Overflow;
        text.raycastTarget      = false;
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
                 "Arrows move, Confirm presses, Cancel goes back. ESC stops listening.", 12,
                 TextAnchor.MiddleCenter, new Vector2(620f, 18f), new Vector2(0f, -215f));

        EditorSceneManager.SaveScene(scene, KeybindPath);
        Debug.Log("SOULBOUND-BATCH-KEYBINDS saved " + KeybindPath + " with " + rows.Length + " binds");
    }

    // ---------------------------------------------------------------- the boss select

    // The wheel sits off the left edge, so only its right hand side is on screen.
    // 360/13, so thirteen notches on the rim wrap exactly and an entry lands on each one.
    // At 27.5 the notches drifted against the entries a little more every turn.
    private const float WheelCx = -350f, WheelCy = 0f, WheelR = 230f, WheelStep = 360f / 13f;

    // The panel, to the right of the arc. Left edge of its text column.
    private const float PanelX = 130f;

    // Wheel_Rim.png is drawn at the radius the scene uses, so it goes in at its own size and
    // needs no scaling. Its track sits inside the circle the entries ride on rather than
    // underneath it: sharing a radius put dark icons on the rim and lost them.
    private const float RimSize = 400f;

    public static void BuildBossSelect() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        // The soul sits at the hub, which is off the left edge, so most of it is off screen.
        GameObject hub = new GameObject("Hub", typeof(Image));
        Image hubImage = hub.GetComponent<Image>();
        hubImage.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(SoulSprite);
        hubImage.raycastTarget = false;
        // Seven times the sprite's 32px. A whole multiple, because anything else samples
        // between pixels and a 32px sprite cannot afford that.
        Place(hub, canvas.transform, new Vector2(224f, 224f), new Vector2(WheelCx, WheelCy));

        // The rim next, so the entries draw over it.
        GameObject rim = new GameObject("WheelRim", typeof(Image));
        Image rimImage = rim.GetComponent<Image>();
        rimImage.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(WheelSprite);
        rimImage.raycastTarget = false;
        rimImage.color         = new Color(1f, 1f, 1f, 0.55f);
        RectTransform rimRect = Place(rim, canvas.transform, new Vector2(RimSize, RimSize), new Vector2(WheelCx, WheelCy));

        MakeText("Heading", canvas.transform, "BOSS SELECT", 24, TextAnchor.MiddleLeft,
                 new Vector2(340f, 30f), new Vector2(PanelX, 200f));

        // The entries. Positions are set at runtime as the wheel turns; these are only
        // starting values so the scene is not a pile of objects at the origin.
        List<BossSlot> slots = new List<BossSlot>();
        Sprite padlock = AssetDatabase.LoadAssetAtPath<Sprite>(LockSprite);
        for (int i = 0; i < BossProgress.SlotsPerScreen; i++) {
            GameObject go = new GameObject("Slot" + (i + 1), typeof(RectTransform));
            float radians = (i - BossProgress.SlotsPerScreen / 2) * WheelStep * Mathf.Deg2Rad;
            RectTransform pivot = Place(go, canvas.transform, new Vector2(64f, 64f),
                                        new Vector2(WheelCx + Mathf.Cos(radians) * WheelR,
                                                    WheelCy - Mathf.Sin(radians) * WheelR));

            // The icon itself, filled in at runtime from the mod by the boss's id.
            GameObject art = new GameObject("Icon", typeof(Image));
            Image icon = art.GetComponent<Image>();
            icon.raycastTarget = false;
            Place(art, pivot, new Vector2(64f, 64f), Vector2.zero);

            // The padlock over it. Half the icon's size and sat low and right, so it reads as
            // something laid on top rather than as the boss's own art.
            GameObject latch = new GameObject("Lock", typeof(Image));
            Image lockImage = latch.GetComponent<Image>();
            lockImage.sprite        = padlock;
            lockImage.raycastTarget = false;
            Place(latch, pivot, new Vector2(32f, 32f), new Vector2(12f, -12f));

            slots.Add(new BossSlot { icon = icon, padlock = lockImage, pivot = pivot });
        }

        // The panel: portrait, who it is, then the record it has against the player.
        GameObject port = new GameObject("Portrait", typeof(Image));
        Image portrait = port.GetComponent<Image>();
        portrait.raycastTarget = false;
        // Twice the 64px icon, a whole multiple, so the portrait stays crisp.
        Place(port, canvas.transform, new Vector2(128f, 128f), new Vector2(PanelX - 110f, 92f));

        GameObject portLatch = new GameObject("PortraitLock", typeof(Image));
        Image portraitLock = portLatch.GetComponent<Image>();
        portraitLock.sprite        = padlock;
        portraitLock.raycastTarget = false;
        Place(portLatch, canvas.transform, new Vector2(48f, 48f), new Vector2(PanelX - 76f, 58f));

        Text bossName = MakeText("Name",     canvas.transform, "", 22, TextAnchor.MiddleLeft, new Vector2(200f, 28f), new Vector2(PanelX + 60f, 118f));
        Text subtitle = MakeText("Subtitle", canvas.transform, "", 12, TextAnchor.UpperLeft,  new Vector2(200f, 46f), new Vector2(PanelX + 60f, 76f));
        subtitle.horizontalOverflow = HorizontalWrapMode.Wrap;

        string[] labels = { "TRIES", "CLEARS", "DEATHS", "BEST", "NO HIT" };
        Text[] values = new Text[5];
        for (int i = 0; i < labels.Length; i++) {
            float y = -20f - i * 30f;
            MakeText("Label" + i, canvas.transform, labels[i], 12, TextAnchor.MiddleLeft,
                     new Vector2(140f, 20f), new Vector2(PanelX - 100f, y));
            values[i] = MakeText("Value" + i, canvas.transform, "", 16, TextAnchor.MiddleRight,
                                 new Vector2(140f, 20f), new Vector2(PanelX + 100f, y));
        }

        // Full width and centred, because it names the way off the screen and that should not
        // be tucked into the panel's column.
        Text hint = MakeText("Hint", canvas.transform, "", 12, TextAnchor.MiddleCenter,
                             new Vector2(620f, 20f), new Vector2(0f, -205f));

        GameObject script = new GameObject("BossSelectScript", typeof(BossSelect));
        BossSelect select = script.GetComponent<BossSelect>();
        select.slots       = slots.ToArray();
        select.wheelRim    = rimRect;
        select.wheelCentre = new Vector2(WheelCx, WheelCy);
        select.wheelRadius = WheelR;
        select.wheelStep   = WheelStep;
        select.portrait     = portrait;
        select.portraitLock = portraitLock;
        select.bossName    = bossName;
        select.subtitle    = subtitle;
        select.tries       = values[0];
        select.clears      = values[1];
        select.deaths      = values[2];
        select.best        = values[3];
        select.nohit       = values[4];
        select.hint        = hint;

        EditorSceneManager.SaveScene(scene, BossSelectPath);
        Debug.Log("SOULBOUND-BATCH-BOSSSELECT saved " + BossSelectPath + " with " + slots.Count + " slots on the wheel");
    }

    // ---------------------------------------------------------------- the disclaimer

    public static void BuildDisclaimer() {
        UnityEngine.SceneManagement.Scene scene =
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        MakeCamera();
        MakeSupport();
        Canvas canvas = MakeCanvas();

        // A wall, barely there. It is meant to be felt rather than looked at: enough texture
        // that the screen is not a void, faint enough that nothing competes with the notice.
        GameObject art = new GameObject("Wall", typeof(Image));
        Image wall = art.GetComponent<Image>();
        wall.sprite        = AssetDatabase.LoadAssetAtPath<Sprite>(WallSprite);
        wall.color         = new Color(1f, 1f, 1f, 0.10f);
        wall.raycastTarget = false;
        PlaceStretched(art, canvas.transform, 0f);

        // Cobwebs, one per corner. The sprite is a whole web, so each is centred on its corner
        // and the screen edge crops it to the quarter that a corner web would be.
        Sprite silk = AssetDatabase.LoadAssetAtPath<Sprite>(CobwebSprite);
        Vector2[] corners = { new Vector2(-320f, 240f), new Vector2(320f, 240f),
                              new Vector2(-320f, -240f), new Vector2(320f, -240f) };
        float[] turns = { 90f, 0f, 180f, 270f };
        for (int i = 0; i < corners.Length; i++) {
            GameObject web = new GameObject("Cobweb" + (i + 1), typeof(Image));
            Image image = web.GetComponent<Image>();
            image.sprite        = silk;
            image.color         = new Color(1f, 1f, 1f, 0.22f);
            image.raycastTarget = false;
            RectTransform rt = Place(web, canvas.transform, new Vector2(150f, 150f), corners[i]);
            // Turned so the torn edge of the web faces into the screen rather than all four
            // sitting the same way up, which reads as one sprite pasted four times.
            rt.localRotation = Quaternion.Euler(0f, 0f, turns[i]);
        }

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

        // Last, so it covers everything at both ends of the screen's life.
        GameObject cover = new GameObject("Fade", typeof(Image));
        Image fade = cover.GetComponent<Image>();
        fade.color         = Color.black;
        fade.raycastTarget = false;
        PlaceStretched(cover, canvas.transform, 0f);

        GameObject script = new GameObject("DisclaimerScript", typeof(DisclaimerScript));
        DisclaimerScript disclaimer = script.GetComponent<DisclaimerScript>();
        disclaimer.Version = version;
        disclaimer.fade    = fade;

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

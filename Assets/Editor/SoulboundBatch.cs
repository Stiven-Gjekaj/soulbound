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

    private const string TitleSprite = "Assets/Sprites/Soulbound_Title.png";
    private const string SoulSprite  = "Assets/Sprites/Soul_Cursor.png";
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
        foreach (string path in new[] { DisclaimerPath, TitlePath, CreditPath }) {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            sb.AppendLine("  --- " + path);

            foreach (GameObject root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) {
                sb.AppendLine("    " + root.name);
                foreach (Transform child in root.transform)
                    sb.AppendLine("      " + child.name
                                  + Describe(child.GetComponent<Image>())
                                  + Describe(child.GetComponent<Text>()));
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
        RectTransform rt = go.GetComponent<RectTransform>();
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

    /// <summary>Everything the menu rebuild needs, in one run.</summary>
    public static void BuildAll() {
        BuildCredits();
        BuildMenu();
        BuildDisclaimer();
        AssetDatabase.SaveAssets();
        Debug.Log("SOULBOUND-BATCH-DONE");
    }
}

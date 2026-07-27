using System.Diagnostics;
using MoonSharp.Interpreter.CoreLib;
using UnityEngine;

public static class StaticInits {
    public static string _MODFOLDER;
    public static string MODFOLDER {
        get { return _MODFOLDER; }
        set {
            _MODFOLDER = value;
            LoadModule.ModFolder = value;
        }
    }

    public static string ENCOUNTER = "";
    public static string EDITOR_MODFOLDER = "@Title";
    /// <summary>The folder under Mods that holds the game. There is only ever one.</summary>
    public const string GAME_MODFOLDER = "Soulbound";
    private static bool firstInit;

    public static bool Initialized { get; set; }

    public delegate void LoadedAction();
    public static event LoadedAction Loaded;

    private static void OnEnable() {  UIController.SendToStaticInit += SendLoaded; }
    private static void OnDisable() { UIController.SendToStaticInit -= SendLoaded; }

    public static void Start() {
        // Settle where the game's files are before touching a registry. Every registry
        // path runs through Path.Combine(DataRoot, ...), so without a root they all throw,
        // and this runs from GlobalControls.Awake: Unity disables a component that throws
        // in Awake, which would take the Escape handler in GlobalControls.Update with it.
        // The error screen tells the player to press ESC to close the game, and that is
        // the one thing that has to keep working when there is nothing else left.
        if (FileLoader.DataRoot == null)
            return;

        if (!firstInit) {
            firstInit = true;
            SpriteRegistry.Start();
            AudioClipRegistry.Start();
            SpriteFontRegistry.Start();
            ShaderRegistry.Start();
        }
        if (string.IsNullOrEmpty(MODFOLDER))
            MODFOLDER = EDITOR_MODFOLDER;
        //if (CurrMODFOLDER != MODFOLDER || CurrENCOUNTER != ENCOUNTER)
        InitAll(MODFOLDER);
        Initialized = true;
    }

    public static void InitAll(string mod, bool shaders = false) {
        MODFOLDER = mod;
        Initialized = false;
        if (!GlobalControls.isInFight || GlobalControls.modDev) {
            FileLoader.absoluteSanitizationDictionary.Clear();
            FileLoader.relativeSanitizationDictionary.Clear();

            //UnitaleUtil.createFile();
            Stopwatch sw = new Stopwatch(); //benchmarking terrible loading times
            sw.Start();

            SpriteRegistry.Init();
            sw.Stop();
            UnityEngine.Debug.Log("Sprite registry loading time: " + sw.ElapsedMilliseconds + "ms");
            sw.Reset();

            sw.Start();
            AudioClipRegistry.Init();
            sw.Stop();
            UnityEngine.Debug.Log("Audio clip registry loading time: " + sw.ElapsedMilliseconds + "ms");
            sw.Reset();

            sw.Start();
            SpriteFontRegistry.Init();
            sw.Stop();
            UnityEngine.Debug.Log("Sprite font registry loading time: " + sw.ElapsedMilliseconds + "ms");
            sw.Reset();

            if (shaders) {
                sw.Start();
                ShaderRegistry.Init();
                sw.Stop();
                UnityEngine.Debug.Log("Shader registry loading time: " + sw.ElapsedMilliseconds + "ms");
                sw.Reset();
            }
        }
        LateUpdater.Init(); // must be last; lateupdater's initialization is for classes that depend on the above registries
        MusicManager.src = Camera.main.GetComponent<AudioSource>();
        SendLoaded();
        Initialized = true;
        //CurrENCOUNTER = ENCOUNTER;
        //CurrMODFOLDER = MODFOLDER;
    }

    public static void SendLoaded() {
        if (Loaded != null)
            Loaded();
    }

    /*public static void Reset() {
        Initialized = false;
        LuaScriptBinder.Clear();
        PlayerCharacter.instance.Reset();
    }*/
}
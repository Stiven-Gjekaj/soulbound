/// <summary>
/// This class will be used to store all customizable game variables, such as the Max HP Limit, the Max Level limit and such.
/// </summary>
[System.Serializable]public class ControlPanel {
    public static ControlPanel instance;
    public int LevelLimit = 99;
    public int HPLimit = 999;
    public int EXPLimit = 99999;
    public int GoldLimit = 99999;
    public float MaxDigitsAfterComma = 5;
    public float PlayerMovementPerSec = 120.0f;
    public float MinimumAlpha = 0.5f;
    // Fallback only. The boss select sends anyone with no stored name through name entry
    // first, so this should never reach the screen. See PlayerProfile.
    public string BasisName = "Soul";
    // The name of this game as the operating system and Discord see it. It read "Create Your
    // Frisk v0.6.6 LTS 4" until v0.5, having survived two sweeps for the fork's name because
    // it is assembled from three variables rather than written out, so grepping never found
    // it. It is the Windows title bar and, on every platform, the game name and icon tooltip
    // in Discord Rich Presence, which is to say it was in people's friends lists.
    //
    // Application.version is whatever the build was stamped with, so this follows a release
    // rather than needing an edit for each one.
    public string WindowBasisName = "Soulbound v" + UnityEngine.Application.version;
    #if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        public bool windows = true;
    #else
        public bool windows = false;
    #endif

    public ControlPanel() { instance = this; }
}

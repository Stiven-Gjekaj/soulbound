using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to contain a Lua error message, which is displayed as soon as the Error scene loads.
/// </summary>
public class ErrorDisplay : MonoBehaviour {
    public static string Message;

    private void Start() {
        UnitaleUtil.firstErrorShown = false;

        // A startup failure cannot be recovered from in place: leaving this screen returns
        // to the title, which runs the same lookup and lands right back here. Closing is
        // the only thing that helps, so say that instead of offering a restart.
        string mess;
        if (FileLoader.startupFailed) mess = "close Soulbound";
        else if (!GlobalControls.modDev) mess = "restart Soulbound";
        else                             mess = "reload";

        GetComponent<Text>().text = Message + "\n\nPress ESC to " + mess;
    }
}

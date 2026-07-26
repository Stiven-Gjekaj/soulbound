using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class to contain a Lua error message, which is displayed as soon as the Error scene loads.
/// </summary>
public class ErrorDisplay : MonoBehaviour {
    public static string Message;

    private void Start() {
        UnitaleUtil.firstErrorShown = false;
        string mess = !GlobalControls.modDev ? "restart CYF" : "reload";
        GetComponent<Text>().text = Message + "\n\nPress ESC to " + mess;
    }
}

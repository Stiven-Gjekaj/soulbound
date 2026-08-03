using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The credits, as a screen the player chooses rather than a camera pan hidden behind holding
/// a direction on the disclaimer.
///
/// The names on it are the attribution the GPLv3 requires, inherited from Create Your Frisk
/// and carried forward unchanged. They are not decoration and they are not ours to edit: the
/// engine this game runs on was written by the people listed here.
/// </summary>
public class CreditsScreen : MonoBehaviour {
    private void Update() {
        if (GlobalControls.input.Cancel == ButtonState.PRESSED
         || GlobalControls.input.Confirm == ButtonState.PRESSED
         || Input.GetMouseButtonDown(0))
            SceneManager.LoadScene("TitleScreen");
    }
}

using UnityEngine;

/// <summary>
/// Removes the retro mode warning banner, which is now unconditional.
///
/// Retro mode is gone, so the state this banner warned about cannot be reached. The prefab
/// that draws it is instanced in ModSelect, TitleScreen and Error, it defaults to active,
/// and no scene overrides that, so deleting this script outright would leave the warning
/// permanently visible on all three screens instead of never.
///
/// Taking the object out of those scenes needs the Unity editor, which is v0.6 work. Until
/// then this destroys it on load, which is what it already did whenever the flag was false.
/// Both this script and the prefab go with that scene pass.
/// </summary>
public class RetromodeWarning : MonoBehaviour {
    private void Start() {
        Destroy(gameObject);
    }
}

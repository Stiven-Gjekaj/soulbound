using UnityEngine;

/// <summary>
/// Asks the registry for a clip and answers null when there is not one.
///
/// This exists because <c>AudioClipRegistry</c> does not. Reading it suggests otherwise: the
/// miss path in <c>TryLoad</c> warns and returns null, which looks like the whole story and is
/// not. Underneath, <c>FileLoader.SanitizePath</c> raises a <c>CYFException</c> for a file that
/// is not there, so a missing clip arrives as a thrown exception rather than as a null, and
/// only some callers survive it.
///
/// The menus want the other behaviour. A screen asking for background music is asking for
/// something optional, and a missing track should leave it quiet, not abandon the rest of the
/// method. The cost of getting that wrong is not a silent menu: it is every line after the
/// call never running, which in a Start or a coroutine means a screen that is half built and
/// cannot say so.
///
/// That is not hypothetical. The menu asks for mus_menu, and the throw took out the line that
/// marked the menu ready for input: the screen came up looking finished and ignored the
/// keyboard forever.
///
/// mus_menu is worth understanding, because it looks present and is not. It ships inside the
/// @Title mod, and the registry searches the mod currently loaded and then Default, never the
/// other mods. The loaded mod is Soulbound, so the file is on disk, three folders away, and
/// unreachable. A key existing somewhere in Assets says nothing about whether a lookup for it
/// resolves.
/// </summary>
public static class MenuAudio {
    public static AudioClip Music(string key) {
        try {
            return AudioClipRegistry.GetMusic(key);
        } catch {
            // Missing background music is a fact about the project, not a fault to report. The
            // registry has already written the detail to the log on its way out.
            return null;
        }
    }

    public static AudioClip Sound(string key) {
        try {
            return AudioClipRegistry.GetSound(key);
        } catch {
            return null;
        }
    }

    /// <summary>Starts a looping track on the camera, or leaves it alone if there is none.</summary>
    public static void PlayMusic(string key, bool loop = true) {
        AudioSource source = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        if (source == null || source.isPlaying)
            return;

        AudioClip clip = Music(key);
        if (clip == null)
            return;

        source.clip = clip;
        source.loop = loop;
        source.Play();
    }

    /// <summary>Fires a one shot on the camera, or does nothing if the sound is missing.</summary>
    public static void PlaySound(string key) {
        AudioSource source = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        AudioClip clip = Sound(key);
        if (source != null && clip != null)
            source.PlayOneShot(clip);
    }
}

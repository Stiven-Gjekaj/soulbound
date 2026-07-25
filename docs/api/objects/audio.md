# The Audio object

### The Audio object [E/M/W]

The Audio object allows you to control music in the game and play sounds. Here are the
ways in which you can use it.

- **number** `Audio.playtime` - Get or set the current play position of the current music
  in seconds.
- **number** `Audio.totaltime` - Get the total length of the current music in seconds.
- `Audio.Play()` - Play the currently loaded music. Done automatically at the beginning of
  a fight.
- `Audio.Stop()` - Stops the music. If you want a battle not to have music, call this in
  `EncounterStarting()`.
- `Audio.Pause()` - Pause the music.
- `Audio.Unpause()` - Unpause the music if you previously paused it.
- `Audio.Volume(number value)` - Set music to given volume. `value` should be between
  `0.0` (muted) and `1.0` (full volume). This is `0.75` by default.
- `Audio.Pitch(number value)` - Set music pitch to given value. `1.0` is default, `2.0` is
  twice the regular speed. Negative values play the music backwards. `value` may be
  between `-3.0` and `3.0`.
- `Audio.LoadFile(string filename)` - Load music from the Audio folder titled
  `filename.ogg` or `filename.wav` and play it immediately. If you don't want immediate
  playback, call `Audio.Stop()` after this. Don't include the file extension.
- `Audio.PlaySound(string filename, <CYF> number volume = 0.65)` - Play the sound from the
  Sounds folder titled `filename.ogg` or `filename.wav`. Don't include the file extension.
- `<CYF>` **boolean** `Audio.isPlaying` - Returns true if the music is playing, or false
  if the music is stopped or paused.
- `<CYF>` `Audio.StopAll()` - Stops all playing audio.
- `<CYF>` `Audio.PauseAll()` - Pauses all the audio sources.
- `<CYF>` `Audio.UnpauseAll()` - Unpauses all the audio sources.
- `<CYF>` `Audio.SetSoundDictionary(string key, string value)` - Adds a sound to the sound
  dictionary. Doing so allows you to change the name of the sounds played by the engine,
  like for example `hurtsound` or `menumove`.

  Setting `key` to `"RESETDICTIONARY"` will reset the entire sound dictionary to its
  original state.
- `<CYF>` `Audio.GetSoundDictionary(string key)` - Returns the index of the sound in the
  sound dictionary. Returns the key itself if the key isn't in the dictionary.
- `<CYF>` `Audio[string key]`, `Audio[string key] = string value` - Fast way to use
  `Audio.SetSoundDictionary` / `GetSoundDictionary`.

As this is an object, you can't directly use it with `[func]`, but you can make your own
function if you want to, say, stop the music mid-dialogue:

```lua
currentdialogue = {"but then I realized...\n[w:30][func:drama]the butler did it!!!"}

function drama()
    Audio.Stop()
    Audio.PlaySound("dramatic_sound_effect")
end
```

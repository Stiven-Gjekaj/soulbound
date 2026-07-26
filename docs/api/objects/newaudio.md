# The NewAudio object

### The NewAudio object [E/M/W]

The NewAudio object allows you to create sound channels and has overall better music
management than the Audio object.

**Audio Channels** can play up to one sound at a time, and will stop any currently playing
sounds if you add a new sound to the channel.

Note: The default audio channel used by the Audio object, not this one, is called `"src"`.
It exists at the start of the encounter, and by default all music and sounds will be
played there. You may use it as the argument `"name"` in all of the below functions.

- `NewAudio.CreateChannel(string name)` - Creates the audio channel `name`.
- `NewAudio.DestroyChannel(string name)` - Destroys the audio channel `name`.
- **boolean** `NewAudio.Exists(string name)` - Checks if the audio channel `name` exists.
- **string** `NewAudio.GetAudioName(string name, boolean withPrefix = true)` - Gets the
  name and type of the audio clip currently playing in the channel `name` as a string. If
  no audio is playing in a channel, this function returns `"empty"`.

  If `withPrefix` is true, the string returned by this function will begin with `music:`
  for music clips, `sound:` for sound clips and `voice:` for voice clips.

  For example, playing the sound `dogsecret` in an audio channel and using this function
  on that channel would return the string `"sound:dogsecret"` (or `"dogsecret"`).
- **number** `NewAudio.GetTotalTime(string name)` - Gets the length of time in seconds that
  an audio channel `name`'s most recently played audio lasts for.
- **number** `NewAudio.GetPlayTime(string name)` or **number**
  `NewAudio.GetCurrentTime(string name)` - Get the current play position of the current
  audio of this channel in seconds.
- `NewAudio.PlayMusic(string name, string music, boolean loop = false, number volume = 1)` -
  Plays the music `music` on the channel `name`.

  `loop` tells the engine if it should loop the music or not; `volume` is the volume of
  the music.

  Songs played are loaded from your mod's `Audio` folder, as .ogg or .wav files. Don't
  include the file extension.
- `NewAudio.PlaySound(string name, string sound, boolean loop = false, number volume = 0.65)` -
  Plays the sound `sound` on the channel `name`.

  `loop` tells the engine if it should loop the sound or not; `volume` is the volume of
  the sound.

  Sounds played are loaded from your mod's `Sounds` folder, as .ogg or .wav files. Don't
  include the file extension.
- `NewAudio.PlayVoice(string name, string voice, boolean loop = false, number volume = 0.65)` -
  Plays the voice `voice` on the channel `name`.

  `loop` tells the engine if it should loop the voice or not; `volume` is the volume of
  the voice.

  Voices played are loaded from your mod's `Sounds/Voices` folder, as .ogg or .wav files.
  Don't include the file extension.
- `NewAudio.SetPitch(string name, number value)` - Sets the pitch (and speed) of the audio
  channel `name`. 1.0 is normal pitch and speed. If the value becomes negative, the audio
  will be played in reverse.
- **number** `NewAudio.GetPitch(string name)` - Gets the pitch of the audio channel `name`.
- `NewAudio.SetVolume(string name, number value)` - Sets the volume of the audio channel
  `name`. 0.75 is the default value.
- **number** `NewAudio.GetVolume(string name)` - Gets the volume of the audio channel
  `name`.
- `NewAudio.Play(string name)` - Plays the last sound played in the audio channel `name`.
- `NewAudio.Stop(string name)` - Stops the audio in the channel `name`.
- `NewAudio.Pause(string name)` - Pauses the audio in the channel `name`.
- `NewAudio.Unpause(string name)` - Unpauses the audio in the channel `name`.
- `NewAudio.SetPlayTime(string name, number value)` - Sets the track position of the audio
  channel `name`'s audio, in seconds.
- **boolean** `NewAudio.isStopped(string name)` - Checks if the audio channel `name` is
  stopped, or paused.
- `NewAudio.StopAll()` - Stops all audio channels' audio.
- `NewAudio.PauseAll()` - Pauses all the audio channels' audio.
- `NewAudio.UnpauseAll()` - Unpauses all the audio channels' audio.

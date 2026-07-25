# `<CYF>` The Map Overworld object

Each map has its own settings, which are determined by the mod it loads, the music it plays,
and a bunch of other stuff. Listed below are functions mainly used to access these variables.

### `Map.GetName()` returns **string**

Returns a string representing the name of the active scene. This is a valid name you may pass
to the other functions here.

### `Map.GetSaveName(string mapName)` returns **string**

Returns a string representing the text displayed in the save screen when the player has saved
on the scene `mapName`.

If there is no scene named `mapName`, or `mapName` does not have save point text set, this
function will return `nil`.

You can change these values by changing the function `AddKeysToMapCorrespondanceList` in the
file `Assets/Scripts/Util/UnitaleUtil.cs`.

### `Map.HasPlayerBeenInMap(string mapName)` returns **boolean**

Returns a boolean specifying whether the player has been in the map `mapName` before or not.
If they have, the function will return true, false otherwise.

### `Map.GetMusic()` returns **string**

Returns the name of the music the map plays when loaded.

### `Map.SetMusic(string value)`

Sets the name of the music the map plays when loaded.

### `Map.GetModToLoad()` returns **string**

Returns the name of the mod the map uses.

### `Map.SetModToLoad(string value)`

Sets the name of the mod the map uses. The player needs to enter the map again for this new
mod to be loaded.

### `Map.GetNoRandomEncounter()` returns **boolean**

Returns whether random encounters can occur on this map.

Important: Encounters with names starting with `#` will be ignored. Add `#` to the names of
Encounters you DON'T want to be chosen as random encounters. This way, you can have both
random encounters AND a unique encounter in the same map.

### `Map.SetNoRandomEncounter(boolean value)`

Sets whether random encounters can occur on this map.

### `Map.GetMusicKept()` returns **boolean**

Returns whether music will persist between the Overworld and battles, like in The Core.

### `Map.SetMusicKept(boolean value)`

Sets whether music will persist between the Overworld and battles, like in The Core.

The main Audio channel used for the map's bgm will be `StaticKeptAudio` if true, and `src`
otherwise.

### `Map.GetMusicMap(string mapName)` returns **string**

Returns the name of the music that the map `mapName` plays when loaded.

If `mapName` is the current map's name, this will return `Map.GetMusic()` instead.

Note: ONLY WORKS IF THE PLAYER HAS ENTERED THE MAP BEFORE. Use `Map.HasPlayerBeenInMap()` to
check if they have.

### `Map.SetMusicMap(string mapName, string value)`

Sets the name of the music that the map `mapName` plays when loaded. Works with maps the
player hasn't been in yet.

If `mapName` is the current map's name, this will call `Map.SetMusic()` instead.

### `Map.GetModToLoadMap(string mapName)` returns **string**

Returns the name of the mod that the map `mapName` uses.

If `mapName` is the current map's name, this will return `Map.GetModToLoad()` instead.

Note: ONLY WORKS IF THE PLAYER HAS ENTERED THE MAP BEFORE. Use `Map.HasPlayerBeenInMap()` to
check if they have.

### `Map.SetModToLoadMap(string mapName, string value)`

Sets the name of the mod that the map `mapName` uses. Works with maps the player hasn't been
in yet.

If `mapName` is the current map's name, this will call `Map.SetModToLoad()` instead.

### `Map.GetNoRandomEncounterMap(string mapName)` returns **string**

Returns whether random encounters can occur on the map `mapName`.

If `mapName` is the current map's name, this will return `Map.GetNoRandomEncounter()`
instead.

Note: ONLY WORKS IF THE PLAYER HAS ENTERED THE MAP BEFORE. Use `Map.HasPlayerBeenInMap()` to
check if they have.

### `Map.SetNoRandomEncounterMap(string mapName, boolean value)`

Sets whether random encounters can occur on the map `mapName`. Works on maps the player
hasn't been in yet.

If `mapName` is the current map's name, this will call `Map.SetNoRandomEncounter()` instead.

### `Map.GetMusicKeptMap(string mapName)`

Returns whether music will persist between the Overworld and battles, like in The Core, for
the map `mapName`.

If `mapName` is the current map's name, this will return `Map.GetMusicKept()` instead.

Note: ONLY WORKS IF THE PLAYER HAS ENTERED THE MAP BEFORE. Use `Map.HasPlayerBeenInMap()` to
check if they have.

### `Map.SetMusicKeptMap(string mapName, boolean value)`

Sets whether music will persist between the Overworld and battles, like in The Core, for the
map `mapName`. Works on maps the player hasn't been in yet.

If `mapName` is the current map's name, this will call `Map.SetMusicKept()` instead.

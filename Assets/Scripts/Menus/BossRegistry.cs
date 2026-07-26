using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;

/// <summary>
/// One boss, as listed in the mod's Lua/bosses.lua.
/// </summary>
public class BossEntry {
    /// <summary>Name of the encounter script under Lua/Encounters, without the extension.</summary>
    public string id;
    /// <summary>The boss name, shown large in the boss select.</summary>
    public string name;
    /// <summary>One line shown under the name.</summary>
    public string subtitle;
}

/// <summary>
/// Reads the boss registry: the ordered list of bosses the select screen shows.
///
/// The registry is a plain data file, so it runs in a bare sandbox rather than through
/// ScriptWrapper. ScriptWrapper binds the battle API, and most of that API does not
/// exist outside the Battle scene.
/// </summary>
public static class BossRegistry {
    public const string FileName = "bosses.lua";

    private static List<BossEntry> entries;

    /// <summary>The bosses, in the order the registry lists them. Read from disk on first use.</summary>
    public static List<BossEntry> Entries {
        get {
            if (entries == null)
                Reload();
            return entries;
        }
    }

    public static int Count { get { return Entries.Count; } }

    /// <summary>Full path to the registry file.</summary>
    public static string FilePath {
        get { return Path.Combine(FileLoader.DataRoot, "Mods/" + StaticInits.GAME_MODFOLDER + "/Lua/" + FileName); }
    }

    /// <summary>Re-reads the registry from disk, discarding whatever was loaded before.</summary>
    public static void Reload() {
        entries = new List<BossEntry>();

        string path = FilePath;
        if (!File.Exists(path)) {
            Fail("The boss registry is missing.\n\nIt should be at:\n" + path);
            return;
        }

        DynValue result;
        Script script = new Script(CoreModules.Preset_HardSandbox);
        try {
            result = script.DoString(File.ReadAllText(path), null, FileName);
        } catch (InterpreterException ex) {
            Fail("The boss registry could not be read.\n\n" + (ex.DecoratedMessage ?? ex.Message));
            return;
        } catch (Exception ex) {
            Fail("The boss registry could not be read.\n\n" + ex.Message);
            return;
        }

        if (result == null || result.Type != DataType.Table) {
            Fail("The boss registry must end by returning a list of bosses, like:\n\n"
               + "return {\n    { id = \"placeholder\", name = \"Placeholder\", subtitle = \"Not built yet\" },\n}");
            return;
        }

        Table list = result.Table;
        for (int i = 1; i <= list.Length; i++) {
            DynValue row = list.Get(i);
            if (row.Type != DataType.Table) {
                Fail("Boss " + i + " in the registry is a " + row.Type.ToString().ToLower() + ", but every entry must be a table.");
                return;
            }

            entries.Add(new BossEntry {
                id       = ReadString(row.Table, "id"),
                name     = ReadString(row.Table, "name"),
                subtitle = ReadString(row.Table, "subtitle")
            });
        }
    }

    private static string ReadString(Table row, string key) {
        DynValue value = row.Get(key);
        return value.Type == DataType.String ? value.String : "";
    }

    /// <summary>Drops whatever was loaded and sends the player to the error screen.</summary>
    private static void Fail(string message) {
        entries = new List<BossEntry>();
        UnitaleUtil.DisplayLuaError(FileName, message);
    }
}

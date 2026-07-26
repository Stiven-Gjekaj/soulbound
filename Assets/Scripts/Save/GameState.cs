using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;

/// <summary>
/// Class used as a database that is saved and loaded during the game.
/// Is used as the savefile in SaveLoad.
/// </summary>
[System.Serializable]
public class GameState {
    public static GameState current;
    public Hashtable soundDictionary;
    public ControlPanel controlpanel;
    public PlayerCharacter player;
    public Dictionary<string, string> playerVariablesStr = new Dictionary<string, string>();
    public Dictionary<string, double> playerVariablesNum = new Dictionary<string, double>();
    public Dictionary<string, bool> playerVariablesBool = new Dictionary<string, bool>();
    public List<string> inventory = new List<string>();
    public List<string> boxContents = new List<string>();
    public float playerTime;
    public string CYFversion = "";

    public void SaveGameVariables() {
        CYFversion = GlobalControls.CYFversion;

        try {
            GameObject Player = GameObject.Find("Player");
            LuaScriptBinder.SetSessionGlobal("PlayerPosX", DynValue.NewNumber(Player.transform.position.x));
            LuaScriptBinder.SetSessionGlobal("PlayerPosY", DynValue.NewNumber(Player.transform.position.y));
            LuaScriptBinder.SetSessionGlobal("PlayerPosZ", DynValue.NewNumber(Player.transform.position.z));
        } catch {
            LuaScriptBinder.SetSessionGlobal("PlayerPosX", DynValue.NewNumber(SaveLoad.savedGame.playerVariablesNum["PlayerPosX"]));
            LuaScriptBinder.SetSessionGlobal("PlayerPosY", DynValue.NewNumber(SaveLoad.savedGame.playerVariablesNum["PlayerPosY"]));
            LuaScriptBinder.SetSessionGlobal("PlayerPosZ", DynValue.NewNumber(SaveLoad.savedGame.playerVariablesNum["PlayerPosZ"]));
        }

        soundDictionary = MusicManager.hiddenDictionary;
        controlpanel = ControlPanel.instance;
        player = PlayerCharacter.instance;

        inventory.Clear();
        foreach (UnderItem item in Inventory.inventory)
            inventory.Add(item.Name);

        boxContents.Clear();
        foreach (UnderItem item in ItemBox.items)
            boxContents.Add(item.Name);

        playerTime = Time.time - GlobalControls.overworldTimestamp;

        try {
            foreach (string key in LuaScriptBinder.GetAllSessionGlobals().Keys) {
                DynValue dv;
                LuaScriptBinder.GetAllSessionGlobals().TryGetValue(key, out dv);
                switch (dv.Type) {
                    case DataType.Number:  playerVariablesNum.Add(key, dv.Number);   break;
                    case DataType.String:  playerVariablesStr.Add(key, dv.String);   break;
                    case DataType.Boolean: playerVariablesBool.Add(key, dv.Boolean); break;
                    case DataType.Nil:     LuaScriptBinder.RemoveSessionGlobal(key);              break;
                    default:
                        UnitaleUtil.WriteInLogAndDebugger("The saved value \"" + key + "\" is erroneous because a " + dv.Type.ToString().ToLower() + " can't be saved. Deleting it now.");
                        LuaScriptBinder.RemoveSessionGlobal(key);
                        break;
                }
            }
        } catch { /* ignored */ }
    }

    public void LoadGameVariables(bool loadGlobals = true) {
        foreach (string key in playerVariablesNum.Keys) {
            if (!loadGlobals && !key.Contains("PlayerPos")) continue;
            double a;
            playerVariablesNum.TryGetValue(key, out a);
            LuaScriptBinder.SetSessionGlobal(key, DynValue.NewNumber(a));
        }
        if (loadGlobals) {
            foreach (string key in playerVariablesStr.Keys) {
                string a;
                playerVariablesStr.TryGetValue(key, out a);
                LuaScriptBinder.SetSessionGlobal(key, DynValue.NewString(a));
            }

            foreach (string key in playerVariablesBool.Keys) {
                bool a;
                playerVariablesBool.TryGetValue(key, out a);
                LuaScriptBinder.SetSessionGlobal(key, DynValue.NewBoolean(a));
            }
        }

        Inventory.inventory.Clear();
        foreach (string str in inventory)
            Inventory.inventory.Add(new UnderItem(str));

        ItemBox.items.Clear();
        foreach (string str in boxContents)
            ItemBox.items.Add(new UnderItem(str));

        PlayerCharacter.instance = player;
        ControlPanel.instance = controlpanel;
        MusicManager.hiddenDictionary = soundDictionary;
    }
}


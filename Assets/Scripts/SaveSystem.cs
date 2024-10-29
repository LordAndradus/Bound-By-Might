using System.Collections.Generic;
using UnityEngine;

public static class SaveSystem
{
    
    public static void Save<T>(T Data, string FilePath) where T : Wrapper
    {
        string data = JsonUtility.ToJson(Data);
        data = Cryptographer.Encrypt(data, GlobalSettings.KeyPartOne + Cryptographer.KeyPartTwo);
        System.IO.File.WriteAllText(System.IO.Path.Combine(GlobalSettings.RootSaveDirectory, FilePath), data);
    }

    public static T Load<T>(string FilePath) where T : Wrapper
    {
        string data = System.IO.File.ReadAllText(System.IO.Path.Combine(GlobalSettings.RootSaveDirectory, FilePath));
        data = Cryptographer.Decrypt(data, GlobalSettings.KeyPartOne + Cryptographer.KeyPartTwo);
        T Information = JsonUtility.FromJson<T>(data);
        return Information;
    }

    [System.Serializable]
    public class Wrapper
    {

    }

    [System.Serializable]
    public class SaveFileWrapper : Wrapper
    {
        public bool NewGamePlus = false;
        public string SaveFileDateTime;
        public int ArmyCount;
        public int ArmyMax;

        public List<Unit> units;
        public List<Squad> squads;

        //Current Chapter Information


        UnitResourceManager.Wrapper resources = new();
    }

    [System.Serializable]
    public class QuickSaveInformation : Wrapper
    {
        public CombatGridSystem cgs;
        public PathFinder pf;
        public UnitPositionGrid upg;

        
    }
}
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour 
{
    public static TurnManager instance;
    public static Level CurrentLevel;
    [SerializeField] public List<Controller> controllers;
    [SerializeField] public Stack<Pair<Vector3, Vector3>> MoveHistory = new();
    //private protected Dictionary<Controller, Controller.Relationship> diplomacy = new();
    private SaveSystem.QuickSaveInformation qsi = new();
    private protected int ControllerTurn = 0;
    private protected int rounds = 0;

    void Awake()
    {
        instance = this;
        
        CurrentLevel = new Tutorial();

        //Get children controllers
        controllers.AddRange(transform.GetComponentsInChildren<Controller>());

        SpawnTile[] spawners = transform.GetComponentsInChildren<SpawnTile>();

        foreach (SpawnTile spawner in spawners) controllers[spawner.ControllerNum].spawners.Add(spawner);
    }

    private void Update()
    {
        controllers[ControllerTurn].Update();
        controllers[ControllerTurn].ControllerLogic();

        if(controllers[ControllerTurn].Finished) 
        {
            MoveHistory.Clear();
            controllers[ControllerTurn].Finished = false;
            controllers[ControllerTurn].SquadMoverList.ForEach(squad => squad.Reactivate());
            controllers[ControllerTurn].TurnNumber++;
            controllers[ControllerTurn].TurnHistory.Clear();
            ControllerTurn = (ControllerTurn + 1) % controllers.Count;
        }

        if(AutoSave()) return;

        AutoLoad();
    }

    private bool AutoSave()
    {
        if(Input.GetKeyDown(GlobalSettings.ControlMap[SettingKey.QuickSave]))
        {
            Debug.Log("Save information!");
            qsi = new();
            qsi.cgs = CombatGridSystem.instance;
            qsi.pf = CombatGridSystem.instance.PathGrid;
            qsi.upg = CombatGridSystem.instance.UnitGrid;
        }

        return false;
    }

    private void AutoLoad()
    {
        if(Input.GetKeyDown(GlobalSettings.ControlMap[SettingKey.QuickLoad]))
        {
            Debug.Log("Load information!");
        }
    }
}
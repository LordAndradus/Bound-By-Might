using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour 
{
    [Header("Turn Information")]
    public static TurnManager instance;
    public Level CurrentLevel;
    private CombatGridSystem cgs;
    [SerializeField] public List<Controller> controllers;
    [SerializeField] public Stack<Pair<Vector3, Vector3>> MoveHistory = new();
    //private protected Dictionary<Controller, Controller.Relationship> diplomacy = new();
    private SaveSystem.QuickSaveInformation qsi = new();
    private protected int ControllerTurn = 0;
    private protected int rounds = 0;

    [Header("User Interfaces")]
    public Button EndTurn;
    public GameObject confirm;
    public GameObject ResourceDropdown;
    public GameObject SquadPicker;
    public GameObject BattleSimulator;

    public void StartTurnManager(Level level)
    {
        //Remove prior instances of the level
        if(controllers != null)
        {
            foreach(Controller controller in controllers) controller.spawners.Clear();
            controllers.Clear();
        }

        //Add level instances
        instance = this;
        CurrentLevel = level;

        controllers.AddRange(transform.GetComponentsInChildren<Controller>());

        cgs = new(level.getWidth(), level.getHeight());

        SpawnTile[] spawners = transform.GetComponentsInChildren<SpawnTile>();

        foreach (SpawnTile spawner in spawners) controllers[spawner.ControllerNum].spawners.Add(spawner);

        foreach(Controller c in controllers)
        {
            if(c is PlayerController)
            {
                ((PlayerController) c).SetupPlayerController();
                break;
            }
        }
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
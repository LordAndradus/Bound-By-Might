using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0), Serializable]
public class Level : ScriptableObject
{
    public PathFinder pf;
    public CombatGridSystem cgs;
    public LevelFinishCondition lfc;

    //Bounds -> First = Negative boundary, Second = Positive boundary : The bounds are in terms of how many tiles
    public Pair<float, float> MapSize { get; set; }

    [SerializeField] int width = 18;
    [SerializeField] int height = 10;

    public Level()
    {
        if(width < 18)
        {
            Debug.LogError(String.Format("Level Width of {1}, minimum is 18", width));
            width = 18;
        }

        if(height < 10)
        {
            Debug.LogError(String.Format("Level Height of {1}, minimum is 10", height));
            height = 10;
        }

        MapSize = new(width, height);
    }

    public void SetMapSize(Level level)
    {
        MapSize = new(level.getWidth(), level.getHeight());
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}

[Serializable]
public enum LevelFinishCondition
{
    CaptureObjective,
    MoveToCoordinate,
    KillSquad,
    SurviveWaves,
    Annihilation,
    GatherResources,
    //TODO: Add more objectives if I work on this in the future
}
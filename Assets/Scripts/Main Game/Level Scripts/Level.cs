using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0), Serializable]
public class Level : ScriptableObject
{
    public PathFinder pf;
    public CombatGridSystem cgs;

    //Bounds -> First = Negative boundary, Second = Positive boundary : The bounds are in terms of how many tiles
    public Pair<float, float> MapSize { get; set; }

    [SerializeField] int width;
    [SerializeField] int height;

    public Level()
    {
        MapSize = new(width, height);
        
        if(MapSize == null) throw new System.Exception("You need to declare a Map Size for the level!");

        cgs = new(width, height);
    }

    public Level(Pair<float, float> MapSize)
    {

    }

    public void LoadLevelSquadlist()
    {
        //Load squad list from current Save File
    }
}
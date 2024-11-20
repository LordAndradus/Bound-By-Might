using UnityEngine;

/**
  * Takes care of picking up items or triggering special events during a specific level
 */
public class MapGridSystem
{
    public static MapGridSystem instance;

    private PathFinder pf;
    private UnitPositionGrid upg;

    public MapGridSystem(int width, int height) : this (width, height, new Vector3(-Mathf.Round(width/2f) * 2f,  -Mathf.Round(width/2f) * 2f)) {}

    public MapGridSystem(int width, int height, Vector3 origin)
    {
        instance = this;
    }

    public void MoveSquad(SquadMovementHandler smh, int destinationX, int destintaionY)
    {
        
    }
}
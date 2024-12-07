using System;
using UnityEngine;

public class Tutorial : Level
{
    
    public Tutorial()
    {
        MapSize = new(26, 26);
        cgs = new CombatGridSystem((int) MapSize.First, (int) MapSize.Second);

        foreach(Pair<int, int> position in UnwalkableSpaces)
        {
            PathNode pn = cgs.PathGrid.GetGridSystem().GetGridObject(position.First, position.Second);
            pn.SetIsWalkable(false);
        }

        cgs.PathGrid.GetGridSystem().drawDebugGrid();
    }
}
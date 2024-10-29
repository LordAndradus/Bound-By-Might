using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaFemale : Unit
{
    public MilitiaFemale()
    {
        UIFriendlyClassName = "Skirmisher";
        spriteView = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Skirmisher/Skirmisher");
        Icon = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Skirmisher/Skirmisher Icon");
        movement = MoveType.Light;

        GoldCost = 125;
    }

    private protected override void SetAttributes()
    {
        throw new System.NotImplementedException();
    }

    private protected override void SetCosts()
    {
        throw new System.NotImplementedException();
    }
}

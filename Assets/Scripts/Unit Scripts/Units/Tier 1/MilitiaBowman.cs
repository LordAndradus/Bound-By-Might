using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaBowman : Unit
{
    public MilitiaBowman()
    {
        UIFriendlyClassName = "Tyro Bowman";
        spriteView = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/MilitiaBowman/Militia Bowman");
        movement = MoveType.Standard;

        Icon = UtilityClass.Load<Sprite>(GlobalSettings.DefaultUnitSpriteIcon);

        GoldCost = 100;
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

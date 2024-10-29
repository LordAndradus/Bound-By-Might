using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NeophyteMagus : Unit
{
    public NeophyteMagus()
    {
        UIFriendlyClassName = "Neophyte Magus";
        spriteView = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Neophyte Magus/Neophyte Magus");
        Icon = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Neophyte Magus/Neophyte Magus Icon");
        movement = MoveType.Slow;

        GoldCost = 250;
        MagicGemCost = 1;
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

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Apothecary : Unit
{
    public Apothecary() : base()
    {
        UIFriendlyClassName = UtilityClass.UIFriendlyClassName(this.GetType().Name);
        spriteView = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Apothecary/Female Apothecary");
        Icon = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Apothecary/Apothecary Icon");
        movement = MoveType.Slow;

    }

    private protected override void SetAttributes()
    {
        TierLevel = 1;

        UnitAttributes = new()
        {
            {
                AttributeType.MaxHP,
                new(
                    80, Range(), 0, new(40, 60)
                )
            },
            {
                AttributeType.HP,
                new(
                    80, Range(), 0, new(40, 60)
                )
            },
            {
                AttributeType.Armor,
                new(
                    15, Range(), 0, new(0, 0)
                )
            },
            {
                AttributeType.Weapon,
                new(
                    30, Range(), 0, new(0, 0)
                )
            },
            {
                AttributeType.Strength,
                new(
                    10, Range(), 0, new(5, 10)
                )
            },
            {
                AttributeType.Agility,
                new(
                    10, Range(), 0, new(5, 15)
                )
            },
            {
                AttributeType.Magic,
                new(
                    25, Range(), 0, new(10, 20)
                )
            },
            {
                AttributeType.Leadership,
                new(
                    25, Range(), 0, new(10, 20)
                )
            }
        };
    }

    private protected override void SetCosts()
    {
        GoldCost = 150;
        IronCost = 0;
        MagicGemCost = 1;
        HorseCost = 0;
        HolyTearCost = 0;
        AdamntiumCost = 0;
    }
}

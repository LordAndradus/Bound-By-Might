using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaFemale : Unit
{
    public MilitiaFemale()
    {
        UIFriendlyClassName = "Skirmisher";
        Attack = AttackType.Melee;
        spriteView = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Skirmisher/Skirmisher");
        Icon = UtilityClass.Load<Sprite>("Sprites/Unit Sprites/Tier 1/Skirmisher/Skirmisher Icon");
        movement = MoveType.Light;

        information = UtilityClass.Load<UnitDataContainer>("Assets/Resources/Data Containers/Tier 1/MilitiaFemale.asset");
    }

    private protected override void SetAttributes()
    {
        TierLevel = 1;

        UnitAttributes = new()
        {
            {AttributeType.MaxHP, new(information.MaxHP)},
            {AttributeType.HP, new(information.HP)},
            {AttributeType.Armor, new(information.Armor)},
            {AttributeType.Weapon, new(information.Weapon)},
            {AttributeType.Strength, new(information.Strength)},
            {AttributeType.Agility, new(information.Agility)},
            {AttributeType.Magic, new(information.Magic)},
            {AttributeType.Leadership, new(information.Leadership)}
        };

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

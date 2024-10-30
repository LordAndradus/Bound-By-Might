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
        TierLevel = 1;

        HP = 80;
        MaxHP = 80;
        Armor = 15;
        WeaponPower = 30;
        Strength = 10;
        Agility = 10;
        Magic = 25;
        Leadership = 25;

        StrengthRequirement = 0;
        AgilityRequirement = 0;
        MagicRequirement = 0;

        HPGrowth = Random.Range(0, 501) / 100f;
        StrengthGrowth = Random.Range(0, 501) / 100f;
        AgilityGrowth = Random.Range(0, 501) / 100f;
        MagicGrowth = Random.Range(0, 501) / 100f;
        LeadershipGrowth = Random.Range(0, 501) / 100f;

        float GrowthTotal = HPGrowth + StrengthGrowth + AgilityGrowth + MagicGrowth + LeadershipGrowth;
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/* TODO
 * Figure out a system to where I keep stats, but everything else transfers. IE the unit is a knight and I demote them back to militia, it'll keep CP progression in the knight class.
 * Tier system is where with each iteration, the unit is 3 times as strong 3^tierlevel. Making the investment to upgrade appealing
*/

[Serializable]
public abstract class Unit
{
    public Sprite spriteView;
    public string Name;
    public string UIFriendlyClassName;
    public string Description = "Make sure to fill in the description, young game maker";
    public Sprite Icon;

    public SerializableDictionary<Type, Snapshot> CareerHistory = new();
    public List<Type> UpgradePath = new();



    [SerializeField] public MoveType movement;

    [Header("Base Attribute Scores")]
    //Base Threat = 100. Calculation goes as follows: Threat += 3 per normal stat, += 1 per armor & weapon; Threat += 1000 * tier level; Threat += 100 * Per Traits rarity
    [SerializeField] private protected int Threat = 100;
    [SerializeField] private protected int HP = 100, MaxHP = 100;
    [SerializeField] private protected int Armor = 30;
    [SerializeField] private protected int WeaponPower = 30;
    [SerializeField] private protected int Strength = 15;
    [SerializeField] private protected int Agility = 15;
    [SerializeField] private protected int Magic = 15;
    [SerializeField] private protected int Leadership = 25;
    [SerializeField] private protected int FieldCost = 10; //base cose = 10; Mercenary = 12 to loyal = 8

    [Header("Equipment, Trait, and Class Additions")]
    [SerializeField] private protected List<int> HPAdd = new();
    [SerializeField] private protected List<int> ArmorAdd = new();
    [SerializeField] private protected List<int> WeaponPowerAdd = new();
    [SerializeField] private protected List<int> StrengthAdd = new();
    [SerializeField] private protected List<int> AgilityAdd = new();
    [SerializeField] private protected List<int> MagicAdd = new();
    [SerializeField] private protected List<int> LeadershipAdd = new();

    [Header("Total Attributes - Calculated")]
    [SerializeField] public int TotalHP = 100;
    [SerializeField] public int TotalArmor = 100;
    [SerializeField] public int TotalWeapon = 100;
    [SerializeField] public int TotalStrength = 100;
    [SerializeField] public int TotalMagic = 100;
    [SerializeField] public int TotalAgility = 100;
    [SerializeField] public int TotalLeadership = 100;

    //This is only for promotion classes that require certain attributes
    [Header("Required Attribute Scores")]
    [SerializeField] private protected int StrengthRequirement;
    [SerializeField] private protected int AgilityRequirement;
    [SerializeField] private protected int MagicRequirement;

    [Header("Attribute Growth Factors")]
    //Range of growth is from 1.0f to 5.0f - This affects the BASE stats of a unit
    //Savant tier is if the total growth is around 22f, Genius is 15f, Above-Average is 12f, Average is 10f, Poor is 8f, and Farmer is 5f 
    [SerializeField] private protected GrowthType Growth; //Averaged based on growth scores. Tiers: Savant, Genius, Above-Average, Average, Below-Average, Poor, Farmer
    [SerializeField] private protected float HPGrowth;
    [SerializeField] private protected float StrengthGrowth;
    [SerializeField] private protected float AgilityGrowth;
    [SerializeField] private protected float MagicGrowth;
    [SerializeField] private protected float LeadershipGrowth;

    [Header("Attribute Generation Range")]
    [SerializeField] public Pair<float, float> HPRange;
    [SerializeField] public Pair<float, float> StrengthRange;
    [SerializeField] public Pair<float, float> AgilityRange;
    [SerializeField] public Pair<float, float> MagicRange;
    [SerializeField] public Pair<float, float> LeadershipRange;

    [Header("Progession Meters")]
    [SerializeField] private protected int Level;
    [SerializeField] public int TierLevel;
    [SerializeField] int PromotionPoints = 0;
    [SerializeField] int ExperiencePoints = 0;

    [Header("Progression Caps Per Level")]
    [SerializeField] private protected int PromotionCap; //Tier 1 = 500, Tier 2 = 3000, Tier 3 = 4500, MaxTier = 8000
    [SerializeField] private protected int ExperienceCap; //The higher the tier, the higher the experience cap, yet the growth rate is multiplied by the tier

    //Cost when adding to squad, and when trying to spawn it
    [Header("Material Cost")]
    [SerializeField] public int GoldCost; 
    [SerializeField] public int IronCost;
    [SerializeField] public int MagicGemCost;
    [SerializeField] public int HorseCost;

    [Header("WIP Material Cost")]
    [SerializeField] public int HolyTearCost;
    [SerializeField] public int AdamntiumCost;

    [Header("Traits")]
    private protected List<Trait> traits;
    public static readonly int AbsoluteMaxTraits = 6;
    [SerializeField] public int MaxTraits = 6; //How many traits the unit can learn in its lifetime. Absolute maxmium is 6

    public Unit() 
    {
        SetAttributes();
        SetCosts();
    }

    private protected abstract void SetAttributes();

    private protected abstract void SetCosts();

    public string displayQuickInfo()
    {
        return string.Format("{0}\nSTR: {1}\nAGI: {2}\nMAG: {3}\nLDR: {4}\nFCC: {5}\nGRO: {6}", Name, Strength, Agility, Magic, Leadership, FieldCost, Growth.ToString());
    }
    
    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append(string.Format("My nane is {0}; class = {1}; I have {2} traits\n", Name, GetType().Name, traits.Count));

        foreach(Trait trait in traits) sb.Append("Trait = " + trait.ToString() + "\n");

        return sb.ToString();
    }

    public class Snapshot
    {
        Type Career;
        public int PromotionPoints;
        public int PromotionCap;
    }

    public void LevelUp()
    {
        HP += (int) HPGrowth;
        CalculateTotals();
    }

    public void CalculateTotals()
    {
        TotalHP = HP + HPAdd.Sum();
        TotalArmor = Armor + ArmorAdd.Sum();
        TotalWeapon = WeaponPower + WeaponPowerAdd.Sum();
        TotalStrength = Strength + StrengthAdd.Sum();
        TotalMagic = Magic + MagicAdd.Sum();
        TotalAgility = Agility + AgilityAdd.Sum();
        TotalLeadership = Leadership + LeadershipAdd.Sum();
    }

    //Getters and Setters for Attribute Scores
    public int GetThreat() { return Threat; }
    public int GetHealth() { return HP; }
    public int GetMaxHealth() { return MaxHP; }
    public int GetArmor() { return Armor; }
    public int GetWeapon() { return WeaponPower; }
    public int GetStrength() { return Strength; }
    public int GetAgility() { return Agility; }
    public int GetMagic() { return Magic; }
    public int GetLeadership() { return Leadership; }

    public List<Trait> GetTraits() { return traits; }
    public void SetTraits(List<Trait> traits) { this.traits = traits; }

    public int GetLevel() { return Level; }
    public int GetFieldCost() { return FieldCost; }

    public int GetXPCap() { return ExperienceCap; }
    public int GetPPCap() { return PromotionCap; }
    public int GetXP() { return ExperiencePoints; }
    public int GetPP() { return PromotionPoints; }
}

public enum GrowthType
{
    Savant,
    Genius,
    Gifted,
    Avgerage,
    Subpar,
    Poor,
    Talentless
}

public enum MoveType
{
    Standard,
    Light,
    Slow,
    Cavalry,
    LightCavalry,
    Flying
}
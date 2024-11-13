using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/* TODO
 * Figure out a system to where I keep stats, but everything else transfers. IE the unit is a knight and I demote them back to militia, it'll keep CP progression in the knight class.
 * Tier system is where with each iteration, the unit is 3 times as strong 3^tierlevel. Making the investment to upgrade appealing
*/

[Serializable]
public class Unit
{
    //TEMP FIELDS - Will be taken up by a Data Container
    [Header("Temporary Fields")]
    public Sprite spriteView;
    public string UIFriendlyClassName;
    public string Description = "Make sure to fill in the description, young game maker";
    public Sprite Icon;
    public AttackType Attack;
    public Pair<int, int> AttackArea = new(1, 1); 
    [SerializeField] public MoveType movement;
    [SerializeField] readonly int MaxRange = (5 * 1000) + 1;
    [SerializeField] public List<Type> UpgradePath = new();
    public AttackPreference preference = AttackPreference.Front;
    [SerializeField] public int TierLevel;


    //Cost when adding to squad, and when trying to spawn it
    [Header("Material Cost")]
    [SerializeField] public int GoldCost; 
    [SerializeField] public int IronCost;
    [SerializeField] public int MagicGemCost;
    [SerializeField] public int HorseCost;

    [Header("WIP Material Cost")]
    [SerializeField] public int HolyTearCost;
    [SerializeField] public int AdamntiumCost;

    //END of temp fields, will be uniquely assigned to individual units
    [Header("Unit Information")]
    [SerializeField] private protected UnitDataContainer information;
    UnitDataContainer GetInformation() { return information; }
    public GrowthType GrowthDefintion;
    public string Name;

    [Header("Attributes")]
    [SerializeField] public SerializableDictionary<Type, Snapshot> CareerHistory = new();
    [SerializeField] float divisor = 1000;
    [SerializeField] public int FieldCost = 10; //Depends on Traits. 12 if Merc, to 8 if Loyal
    [SerializeField] public int threat = 256;
    [SerializeField] public bool DeathFlag = false;

    [SerializeField] public Pair<int, int> SquadPosition; //For when we reload the save file, we know what position to put the unit in

    [NonSerialized] private protected List<Pair<Unit, int>> DamageReport;

    [Header("Attribute Dictionary")]
    public SerializableDictionary<AttributeType, AttributeScore> UnitAttributes;

    [Header("Progession Meters")]
    [SerializeField] private protected int Level;
    [SerializeField] public int PromotionPoints = 0;
    [SerializeField] public int ExperiencePoints = 0;
    [SerializeField] public int ExperiencePointsDropped = 100;

    [Header("Progression Caps Per Level")]
    [SerializeField] private protected int PromotionCap; //Tier 1 = 500, Tier 2 = 3000, Tier 3 = 4500, MaxTier = 8000
    [SerializeField] private protected int ExperienceCap; //The higher the tier, the higher the experience cap, yet the growth rate is multiplied by the tier


    [Header("Traits")]
    private protected List<Trait> traits;
    public static readonly int AbsoluteMaxTraits = 6;
    [SerializeField] public int MaxTraits = 6; //How many traits the unit can learn in its lifetime. Absolute maxmium is 6

    public Unit() 
    {
        UnitAttributes = new();
        SetAttributes();
        SetCosts();
        SetGrowthType();
        SetUpgradePath();

        ExperienceCap = 500 * TierLevel;

        switch(TierLevel)
        {
            case 1:
                PromotionCap = 500;
                break;

            case 2:
                PromotionCap = 3000;
                break;

            case 3:
                PromotionCap = 4500;
                break;

            case 4:
                PromotionCap = 8000;
                break;
        }
    }

    private protected virtual void SetAttributes()
    {
        return;
    }

    private protected virtual void SetCosts()
    {
        return;
    }

    private protected virtual void SetUpgradePath()
    {
        return;
    }

    private protected float Range()
    {
        return UnityEngine.Random.Range(0, MaxRange) / (float) divisor;
    }

    private void SetGrowthType()
    {
        float MaxGrowth =  (float) UnitAttributes.Count * MaxRange / divisor;
        float GrowthSum = UnitAttributes.Sum(pair => pair.Value.GetGrowthValue());

        if(GrowthSum <= 0f || MaxGrowth <= 0f)
        {
            GrowthDefintion = GrowthType.Abysmal;
            return;
        }

        float GrowthRatio = GrowthSum / MaxGrowth;

        if(GrowthRatio >= 0.9f) GrowthDefintion = GrowthType.Savant;
        else if(GrowthRatio < 0.9f && GrowthRatio >= 0.75f) GrowthDefintion = GrowthType.Genius;
        else if(GrowthRatio < 0.75f && GrowthRatio >= 0.6f) GrowthDefintion = GrowthType.Gifted;
        else if(GrowthRatio < 0.6f && GrowthRatio >= 0.4f) GrowthDefintion = GrowthType.Avgerage;
        else if(GrowthRatio < 0.4f && GrowthRatio >= 0.3f) GrowthDefintion = GrowthType.Subpar;
        else if(GrowthRatio < 0.3f && GrowthRatio >= 0.1f) GrowthDefintion = GrowthType.Poor;
        else if(GrowthRatio < 0.1f && GrowthRatio >= 0.05f) GrowthDefintion = GrowthType.Talentless;
        else if(GrowthRatio < 0.05f && GrowthRatio >= 0.0f) GrowthDefintion = GrowthType.Abysmal;
    }

    public string displayQuickInfo()
    {
        return string.Format("{0}\nSTR: {1}\nAGI: {2}\nMAG: {3}\nLDR: {4}\nFCC: {5}\nGRO: {6}", Name, UnitAttributes[AttributeType.Strength], UnitAttributes[AttributeType.Agility], UnitAttributes[AttributeType.Magic], UnitAttributes[AttributeType.Leadership], FieldCost, GrowthDefintion.ToString());
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
        
        CalculateTotals();
    }

    public void CalculateTotals()
    {
        
    }

    //Getters and Setters for Attribute Scores
    public List<Trait> GetTraits() { return traits; }
    public void SetTraits(List<Trait> traits) { this.traits = traits; }

    public int GetLevel() { return Level; }
    public int GetFieldCost() { return FieldCost; }

    public int GetXPCap() { return ExperienceCap; }
    public int GetPPCap() { return PromotionCap; }
    public int GetXP() { return ExperiencePoints; }
    public int GetPP() { return PromotionPoints; }
}

[Serializable]
public class AttributeScore
{
    [SerializeField] int total;
    [SerializeField] int value, requirement;
    [SerializeField] float growth;
    [SerializeField] float leftover;
    [SerializeField] Pair<int, int> GenerationRange;
    public List<int> additions;

    public AttributeScore(int value, float growth, int requirement, Pair<int, int> GenerationRange)
    {
        this.value = value;
        this.growth = growth;
        this.requirement = requirement;
        this.GenerationRange = GenerationRange;
        additions = new();
        leftover = 0f;
        SetTotal();
    }

    public void SetBaseValue(int value)
    {
        if(value < 0)
        {
            Debug.LogError("There is a negative value being passed in attribute. What. The. Fuck.");
            return;
        }

        this.value = value;
    }

    public void LevelUp()
    {
        float val = (float) value + growth + leftover;
        value = (int) val;
        leftover = val - value;
    }

    public void SetTotal()
    {
        total = value + additions.Sum();
    }

    public int GetTotal()
    {
        return total;
    }

    public int GetRequirement()
    {
        return requirement;
    }

    public float GetGrowthValue()
    {
        return growth;
    }

    public Pair<int, int> GetGenerationRange()
    {
        return GenerationRange;
    }

    public override string ToString()
    {
        return value.ToString();
    }

    public static implicit operator int(AttributeScore a)
    {
        return a.value;
    }

    //Operations
    public static AttributeScore operator +(AttributeScore a1, AttributeScore a2)
    {
        a1.value += a2.value;
        return a1;
    }

    public static AttributeScore operator +(AttributeScore asVal, int val)
    {
        asVal.value += val;
        return asVal;
    }

    public static int operator +(int val, AttributeScore asVal)
    {
        val += asVal.value;
        return val;
    }

    public static AttributeScore operator -(AttributeScore a1, AttributeScore a2)
    {
        a1.value -= a2.value;
        return a1;
    }

    public static AttributeScore operator -(AttributeScore asVal, int val)
    {
        asVal.value -= val;
        return asVal;
    }

    public static int operator -(int val, AttributeScore asVal)
    {
        val -= asVal.value;
        return val;
    }

    public static AttributeScore operator *(AttributeScore a1, AttributeScore a2)
    {
        a1.value *= a2.value;
        return a1;
    }

    public static AttributeScore operator *(AttributeScore asVal, int val)
    {
        asVal.value *= val;
        return asVal;
    }

    public static int operator *(int val, AttributeScore asVal)
    {
        val *= asVal.value;
        return val;
    }

    public static AttributeScore operator /(AttributeScore a1, AttributeScore a2)
    {
        a1.value /= a2.value;
        return a1;
    }

    public static AttributeScore operator /(AttributeScore asVal, int val)
    {
        asVal.value /= val;
        return asVal;
    }

    public static int operator /(int val, AttributeScore asVal)
    {
        val /= asVal.value;
        return val;
    }

    public static bool operator ==(AttributeScore a1, AttributeScore a2)
    {
        return a1.value == a2.value;
    }

    public static bool operator ==(AttributeScore asVal, int val)
    {
        return asVal.value == val;
    }

    public static bool operator ==(int val, AttributeScore asVal)
    {
        return asVal.value == val;
    }

    public static bool operator !=(AttributeScore a1, AttributeScore a2)
    {
        return a1.value != a2.value;
    }

    public static bool operator !=(AttributeScore asVal, int val)
    {
        return asVal.value != val;
    }

    public static bool operator !=(int val, AttributeScore asVal)
    {
        return asVal.value != val;
    }

    public static bool operator <=(AttributeScore a1, AttributeScore a2)
    {
        return a1.value <= a2.value;
    }

    public static bool operator <=(AttributeScore asVal, int val)
    {
        return asVal.value <= val;
    }

    public static bool operator <=(int val, AttributeScore asVal)
    {
        return val <= asVal.value;
    }

    public static bool operator >=(AttributeScore a1, AttributeScore a2)
    {
        return a1.value >= a2.value;
    }

    public static bool operator >=(AttributeScore asVal, int val)
    {
        return asVal.value >= val;
    }

    public static bool operator >=(int val, AttributeScore asVal)
    {
        return val >= asVal.value;
    }

    public static bool operator <(AttributeScore a1, AttributeScore a2)
    {
        return a1.value < a2.value;
    }

    public static bool operator <(AttributeScore asVal, int val)
    {
        return asVal.value < val;
    }

    public static bool operator <(int val, AttributeScore asVal)
    {
        return val < asVal.value;
    }

    public static bool operator >(AttributeScore a1, AttributeScore a2)
    {
        return a1.value > a2.value;
    }

    public static bool operator >(AttributeScore asVal, int val)
    {
        return asVal.value > val;
    }

    public static bool operator >(int val, AttributeScore asVal)
    {
        return val > asVal.value;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

public enum AttributeType
{
    Armor,
    Weapon,
    HP, MaxHP,
    Strength,
    Agility,
    Magic,
    Leadership,
    NULL //For Unit attack AI in the battle simulator
}

public enum AttackType
{
    Melee,
    Archery,
    Magic,
    Healing,
    Firearms //Imagine a musket or something
}

public enum AttackPreference
{
    Front,
    Back,
    Middle,
    Most,
    Least,
    Leader,
}

public enum GrowthType
{
    Savant,
    Genius,
    Gifted,
    Avgerage,
    Subpar,
    Poor,
    Talentless,
    Abysmal
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
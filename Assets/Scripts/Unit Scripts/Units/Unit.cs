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
    [Header("Unit Information")]
    [SerializeField] private protected UnitDataContainer information;
    public UnitDataContainer GetInformation() { return information; }
    public GrowthType GrowthDefintion;
    public string Name;

    [Header("Attributes")]
    [SerializeField] public List<Snapshot> CareerHistory = new();
    private int numerator = 5001;
    private float divisor = 1000;
    [SerializeField] public int FieldCost = 10; //Depends on Traits. 12 if Merc, to 8 if Loyal
    [SerializeField] public int threat = 256;
    [SerializeField] public bool DeathFlag = false;

    [Header("Attribute Dictionary")]
    public SerializableDictionary<AttributeType, AttributeScore> ThisAttributes;

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

    [Header("Combat")]
    [NonSerialized] private protected List<Pair<Unit, int>> DamageReport; //Reports how much damage this unit sustained


    public delegate void PromotionInvoker(Type Class);
    public PromotionInvoker PromoteThis = (Type Class) => {
        Debug.Log("Make sure to override thsi delegate when promoting!");
    };

    public Unit() 
    {
        SetAttributes();

        ExperienceCap = 500 * information.TierLevel;

        switch(information.TierLevel)
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
        ThisAttributes = new()
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

        //Generate Attributes
        foreach(AttributeScore a in ThisAttributes.Values)
        {
            float GrowthValue = UnityEngine.Random.Range(0, numerator);
            a.SetGrowthValue(GrowthValue);
            
            Pair<int, int> range = a.GetGenerationRange();
            int value = UnityEngine.Random.Range(range.First, range.Second);
            a.SetBaseValue(value);
        }

        SetGrowthType();
    }

    private void SetGrowthType()
    {
        float MaxGrowth =  (float) ThisAttributes.Count * numerator / divisor;
        float GrowthSum = ThisAttributes.Sum(pair => pair.Value.GetGrowthValue());

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
        return string.Format("{0}\nSTR: {1}\nAGI: {2}\nMAG: {3}\nLDR: {4}\nFCC: {5}\nGRO: {6}", Name, ThisAttributes[AttributeType.Strength], ThisAttributes[AttributeType.Agility], ThisAttributes[AttributeType.Magic], ThisAttributes[AttributeType.Leadership], FieldCost, GrowthDefintion.ToString());
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
        public Snapshot(Unit type, int PromotionPoints, int PromotionCap)
        {
            Career = type.GetType();
            this.PromotionPoints = PromotionPoints;
            this.PromotionCap = PromotionCap;
        }

        public Type getCareer()
        {
            return Career;
        }

        Type Career;
        public int PromotionPoints;
        public int PromotionCap;
    }

    public void Promote(Unit UnitClass)
    {
        Snapshot ss = new(UnitClass, PromotionPoints, PromotionCap);
        CareerHistory.Add(ss);
        PromoteThis.Invoke(ss.getCareer());
        PromoteThis = (Type Class) => {
        Debug.Log("Make sure to override thsi delegate when promoting!");
        };
    }

    //Getters and Setters for Attribute Scores
    public List<Trait> GetTraits() { return traits; }
    public void SetTraits(List<Trait> traits) { this.traits = traits; }

    public int GetLevel() { return Level; }
    public int GetFieldCost() { return FieldCost; }

    public int GetIronCost() { return information.IronCost; }
    public int GetMagicGemCost() { return information.MagicGemCost; }
    public int GetHorseCost() { return information.HorseCost; }
    public int GetAdamantiumCost() { return information.AdamntiumCost; }
    public int GetHolyTearCost() { return information.HolyTearCost; }

    // [Header("Image Information")]
    // public Sprite spriteView;
    // public Sprite Icon;

    // [Header("Unit Information")]
    // public string UIFriendlyClassName;
    // public string Description = "Make sure to fill in the description, young game maker";
    // [SerializeField] public MoveType movement;

    // [Header("Attack Information")]
    // public AttackType Attack;
    // public Pair<int, int> AttackArea = new(1, 1);
    // public Pair<AttackPreference, AttributeType> AttackAI = new(AttackPreference.Front, AttributeType.NULL);

    // [Header("Generation Information")]
    // public int TierLevel = 1;
    // public List<UnitDataContainer> UpgradePath = new();

    public Pair<int,int> AttackArea() { return information.AttackArea; }
    public Sprite SpriteView() { return information.spriteView; }

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

    public AttributeScore(AttributeScore a) : this(a.value, a.growth, a.requirement, a.GenerationRange) {}

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

    public void SetGrowthValue(float value)
    {
        if(growth < 0f)
        {
            Debug.LogError("There was a negative value being passed for growth!");
            throw new SystemException("Negative value");
        }

        this.growth = value;
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
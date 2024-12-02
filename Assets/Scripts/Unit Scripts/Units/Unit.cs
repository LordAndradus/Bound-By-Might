using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
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
    public GrowthType GrowthDefintion;
    public string Name;

    [Header("Attributes")]
    [SerializeField] public List<Snapshot> CareerHistory = new();
    private int numerator = 5001;
    private float divisor = 1000;
    public int MarketCost = 0;
    public int FieldCost = 10; //Depends on Traits. 12 if Merc, to 8 if Loyal
    public int threat = 256;
    private int maxSpread = 17;
    private int minSpread = 17;
    public int getMinSpread() { return minSpread; }
    public int getMaxSpread() { return maxSpread; }
    public bool DeathFlag = false;
    public bool InvalidBit = true;

    [Header("Attribute Dictionary")]
    public SerializableDictionary<AttrType, AttrScore> ThisAttributes;

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
        //InitializeUnit();
    }

    public void InitializeUnit()
    {
        UnitLoader.AddBundles();
        GetInformation();
        ThisAttributes = new();
        SetAttributes();

        InvalidBit = false;

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

    private protected virtual void GetInformation() 
    {
        if(information == null) information = UnitLoader.AssetBundle[typeof(Apothecary)];
    }

    private protected virtual void SetAttributes()
    {
        if(information == null)
        {
            throw new SystemException("Information must not be empty!");
        }

        ThisAttributes = new()
        {
            {AttrType.MaxHP, new(information.MaxHP)},
            {AttrType.HP, new(information.HP)},
            {AttrType.Armor, new(information.Armor)},
            {AttrType.Weapon, new(information.Weapon)},
            {AttrType.Strength, new(information.Strength)},
            {AttrType.Agility, new(information.Agility)},
            {AttrType.Magic, new(information.Magic)},
            {AttrType.Leadership, new(information.Leadership)}
        };

        //Generate Attributes
        AttrScore[] atr = ThisAttributes.Values.ToArray();
        foreach(AttrScore a in atr)
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
        StringBuilder sb = new();
        sb.Append(Name + "\n");

        sb.Append("STR: " + ThisAttributes[AttrType.Strength] + "\n");
        sb.Append("AGI: " + ThisAttributes[AttrType.Agility] + "\n");
        sb.Append("MAG: " + ThisAttributes[AttrType.Magic] + "\n");
        sb.Append("LDR: " + ThisAttributes[AttrType.Leadership] + "\n");
        sb.Append("FCC: " + FieldCost + "\n");
        sb.Append("GRO: " + GrowthDefintion + "\n");

        return sb.ToString();
        //return string.Format("{0}\nSTR: {1}\nAGI: {2}\nMAG: {3}\nLDR: {4}\nFCC: {5}\nGRO: {6}", Name, ThisAttributes[AttributeType.Strength], ThisAttributes[AttributeType.Agility], ThisAttributes[AttributeType.Magic], ThisAttributes[AttributeType.Leadership], FieldCost, GrowthDefintion.ToString());
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

    //Get level and cost
    public int GetLevel() { return Level; }
    public int GetFieldCost() { return FieldCost; }

    //Get costs
    public int GoldCost() { return information.GoldCost; }
    public int IronCost() { return information.IronCost; }
    public int MagicGemCost() { return information.MagicGemCost; }
    public int HorseCost() { return information.HorseCost; }
    public int AdamantiumCost() { return information.AdamantiumCost; }
    public int HolyTearCost() { return information.HolyTearCost; }

    //DataUnitContainer information
    public Sprite spriteView() { return information.spriteView; }
    public Sprite Icon() { return information.Icon; }
    public string UIFriendlyClassName() { return information.UIFriendlyClassName; }
    public string Description() { return information.Description; }
    public MoveType movement() { return information.movement; }
    public AttackType Attack() { return information.Attack; }
    public Pair<int, int> AttackArea() { return information.AttackArea; }
    public Pair<AttackPreference, AttrType> AttackAI() { return information.AttackAI; }
    public int TierLevel() { return information.TierLevel; }
    public List<UnitDataContainer> UpgradePath = new();

    //Get progression stats
    public int GetXPCap() { return ExperienceCap; }
    public int GetPPCap() { return PromotionCap; }
    public int GetXP() { return ExperiencePoints; }
    public int GetPP() { return PromotionPoints; }

    //Battle specific
    public void damage(int damage)
    {
        ThisAttributes[AttrType.HP] -= damage;
        if(ThisAttributes[AttrType.HP] <= 0) DeathFlag = true;
    }
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
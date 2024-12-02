using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public enum AttrType
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

[Serializable]
public class AttrScore
{
    [SerializeField] int total;
    [SerializeField] int value, requirement;
    [SerializeField] float growth;
    [SerializeField] float leftover;
    [SerializeField] Pair<int, int> GenerationRange;
    public List<int> additions;

    public AttrScore(AttrScore a) : this(a.value, a.growth, a.requirement, a.GenerationRange) {}

    public AttrScore(int value, float growth, int requirement, Pair<int, int> GenerationRange)
    {
        this.value = value;
        this.growth = growth;
        this.requirement = requirement;
        this.GenerationRange = GenerationRange;
        additions = new();
        leftover = 0f;
        SetTotal();
    }

    public AttrScore() : this(100, 5.0f, 0, new(20, 40)) {}

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

    public static implicit operator int(AttrScore a)
    {
        return a.value;
    }

    //Operations
    public static AttrScore operator +(AttrScore a1, AttrScore a2)
    {
        a1.value += a2.value;
        return a1;
    }

    public static AttrScore operator +(AttrScore asVal, int val)
    {
        asVal.value += val;
        return asVal;
    }

    public static int operator +(int val, AttrScore asVal)
    {
        val += asVal.value;
        return val;
    }

    public static AttrScore operator -(AttrScore a1, AttrScore a2)
    {
        a1.value -= a2.value;
        return a1;
    }

    public static AttrScore operator -(AttrScore asVal, int val)
    {
        asVal.value -= val;
        return asVal;
    }

    public static int operator -(int val, AttrScore asVal)
    {
        val -= asVal.value;
        return val;
    }

    public static AttrScore operator *(AttrScore a1, AttrScore a2)
    {
        a1.value *= a2.value;
        return a1;
    }

    public static AttrScore operator *(AttrScore asVal, int val)
    {
        asVal.value *= val;
        return asVal;
    }

    public static int operator *(int val, AttrScore asVal)
    {
        val *= asVal.value;
        return val;
    }

    public static AttrScore operator /(AttrScore a1, AttrScore a2)
    {
        a1.value /= a2.value;
        return a1;
    }

    public static AttrScore operator /(AttrScore asVal, int val)
    {
        asVal.value /= val;
        return asVal;
    }

    public static int operator /(int val, AttrScore asVal)
    {
        val /= asVal.value;
        return val;
    }

    public static bool operator ==(AttrScore a1, AttrScore a2)
    {
        return a1.value == a2.value;
    }

    public static bool operator ==(AttrScore asVal, int val)
    {
        return asVal.value == val;
    }

    public static bool operator ==(int val, AttrScore asVal)
    {
        return asVal.value == val;
    }

    public static bool operator !=(AttrScore a1, AttrScore a2)
    {
        return a1.value != a2.value;
    }

    public static bool operator !=(AttrScore asVal, int val)
    {
        return asVal.value != val;
    }

    public static bool operator !=(int val, AttrScore asVal)
    {
        return asVal.value != val;
    }

    public static bool operator <=(AttrScore a1, AttrScore a2)
    {
        return a1.value <= a2.value;
    }

    public static bool operator <=(AttrScore asVal, int val)
    {
        return asVal.value <= val;
    }

    public static bool operator <=(int val, AttrScore asVal)
    {
        return val <= asVal.value;
    }

    public static bool operator >=(AttrScore a1, AttrScore a2)
    {
        return a1.value >= a2.value;
    }

    public static bool operator >=(AttrScore asVal, int val)
    {
        return asVal.value >= val;
    }

    public static bool operator >=(int val, AttrScore asVal)
    {
        return val >= asVal.value;
    }

    public static bool operator <(AttrScore a1, AttrScore a2)
    {
        return a1.value < a2.value;
    }

    public static bool operator <(AttrScore asVal, int val)
    {
        return asVal.value < val;
    }

    public static bool operator <(int val, AttrScore asVal)
    {
        return val < asVal.value;
    }

    public static bool operator >(AttrScore a1, AttrScore a2)
    {
        return a1.value > a2.value;
    }

    public static bool operator >(AttrScore asVal, int val)
    {
        return asVal.value > val;
    }

    public static bool operator >(int val, AttrScore asVal)
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

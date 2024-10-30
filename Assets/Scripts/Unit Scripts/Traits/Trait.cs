using System;
using UnityEngine;

[Serializable]
public class Trait
{
    public Sprite icon;

    public string UIFriendlyClassName;

    public string ShortDescription = "Learnable slot for a trait!";
    public string FlavorText = "A slot that can be used to learn a trait!";
    public string EffectText = "There is untapped potential to make this unit more effective...";

    public TraitRank rank = TraitRank.Normal;

    protected bool combat = false; //Indicates if it's a status changer, or if it's for combat

    // Start is called before the first frame update
    public Trait()
    {
        UIFriendlyClassName = UtilityClass.UIFriendlyClassName(GetType().Name);
    }

    public String getDescription()
    {
        return FlavorText + " " + EffectText;
    }

//Overriden functions========================================================================================================================
    public virtual void add(Unit u)
    {
        Debug.Log("Don't forget to add the trait!");
    }

    public virtual void remove(Unit u) //For non-combat effects
    {
        Debug.Log("Make sure to actually remove the trait!");
    }

    public virtual void trigger(Unit u)
    {
        Debug.LogError("Make sure to have a trigger for this trait!");
    }

    public virtual String toString()
    {
        return "Name = \"" + GetType().Name + "\", effect = \"" + EffectText + "\", Description = \"" + FlavorText + "\"";
    }

    public enum TraitRank
    {
        Legendary, //Massively boosts a unit, the rarest trait in the game
        Leader, //Effectively only when in a leadership position
        Epic, //Really good quality trait
        Rare, //Mid-tier trait
        Normal //Makes a unit more unique
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bulwark : Trait, TraitTrigger, AttributeBooster
{
    public Bulwark()
    {
        FlavorText = "Always seeking life at the frontlines, they are represented as the shield of their comrades";
        EffectText = "+25 Armor and Reduce damage by 25% on the first row";
    }

    Action<Unit> TraitTrigger.trigger { get; set; }

    public void Trigger(Unit u)
    {
        
    }

    Action<Unit> AttributeBooster.add { get; set; }
    Action<Unit> AttributeBooster.remove { get; set; }

    public void Add(Unit u)
    {
        
    }

    public void Remove(Unit u)
    {
        
    }
}

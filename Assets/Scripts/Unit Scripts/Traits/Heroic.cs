using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroic : Trait, TraitTrigger
{
    bool DeathDefied = false;
    int AddIndex = -1;

    public Heroic()
    {
        rank = TraitRank.Epic;
        FlavorText = "A soldier that will forge their path forward no matter how hopeless the situation. They exude a unique aura that inspires others to move forwards.";
        EffectText = "Defies a death blow once per battle. When defied, it increases this units damage for the whole battle by 100%.";
    }

    Action<Unit> TraitTrigger.trigger { get; set; }

    public void Trigger(Unit u)
    {
        DeathDefied = !DeathDefied;


        //Reset the trigger as the unit is dead dead.
        if(!DeathDefied)
        {
            if(AddIndex != -1) u.ThisAttributes[AttributeType.Weapon].additions.RemoveAt(AddIndex);
            return;
        }

        if(DeathDefied)
        {
            u.DeathFlag = false;

            int val = u.ThisAttributes[AttributeType.Weapon];
            AddIndex = u.ThisAttributes[AttributeType.Weapon].additions.Count;
            u.ThisAttributes[AttributeType.Weapon].additions.Insert(AddIndex - 1, val);
        }
    }
}

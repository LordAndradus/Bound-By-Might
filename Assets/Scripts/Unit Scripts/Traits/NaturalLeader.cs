using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalLeader : Trait, AttributeBooster
{
    int PreviousLeadership;
    int AddIndex = -1;

    public NaturalLeader()
    {
        rank = TraitRank.Rare;
        FlavorText = "Someone who is always at the center of their clique, making the tough decision no one else wants to.";
        EffectText = "When employed as a squad leader, they decreases the field cost of each unit by 10%";
    }

    public override void add(Unit u)
    {
        AttrScore leadership = u.ThisAttributes[AttrType.Leadership];
        PreviousLeadership = leadership;
        int score = leadership;
        leadership.additions.Add((int) (score * 0.1f));
    }

    Action<Unit> AttributeBooster.add { get; set; }
    Action<Unit> AttributeBooster.remove { get; set; }

    public void Add(Unit u)
    {
        AttrScore leadership = u.ThisAttributes[AttrType.Leadership];

        if(AddIndex != -1)
        {
            leadership.additions.RemoveAt(AddIndex);
        }

        PreviousLeadership = leadership;
        int score = leadership;
        AddIndex = leadership.additions.Count;
        leadership.additions.Insert(AddIndex - 1, (int) (score * 0.1f));
    }

    public void Remove(Unit u)
    {
        AttrScore leadership = u.ThisAttributes[AttrType.Leadership];
        
        if(AddIndex != -1)
        {
            leadership.additions.Remove(AddIndex);
        }
    }
}

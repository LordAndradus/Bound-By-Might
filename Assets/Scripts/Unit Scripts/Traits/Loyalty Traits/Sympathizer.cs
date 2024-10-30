using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sympathizer : Trait, Loyalty
{
    public Sympathizer()
    {
        FlavorText = "This soldier still fights for money, but is starting to see the importance of your cause.";
        EffectText = "+1 Field Cost and a %50 increase in Gold Cost to field";
    }
    
    public bool IsLoyaltyTrait()
    {
        return true;
    }
}

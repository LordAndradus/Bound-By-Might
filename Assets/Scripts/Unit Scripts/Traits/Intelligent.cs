using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intelligent : Trait, TraitTrigger
{
    int PreviousPP;
    int PreviousEP;

    public Intelligent()
    {
        FlavorText = "A practitioner of learning. They tend to acclimate quite easily to their surrounding, thanks to a laborious life of study";
        EffectText = "25% increase to experience and promotion point gain";
    }

    Action<Unit> TraitTrigger.trigger { get; set; }

    public void Trigger(Unit u)
    {
        //Record experience gain from battle
        int ppDifference = u.PromotionPoints - PreviousPP;
        int epDifference = u.ExperiencePoints - PreviousEP;

        if(ppDifference < 0 || epDifference < 0)
        {
            Debug.LogError("For some reason there is a negative difference, setting previous values to 0.");

            PreviousPP = 0;
            PreviousEP = 0;

            ppDifference = 0;
            epDifference = 0;
        }

        //Undo that change
        u.PromotionPoints -= PreviousPP;
        u.ExperiencePoints -= PreviousEP;
        
        //Apply Trait effect
        ppDifference = (int) (ppDifference * 0.25f);
        epDifference = (int) (epDifference * 0.25f);

        //Add trait effect back into Unit
        u.PromotionPoints += ppDifference;
        u.ExperiencePoints += epDifference;

        //Lastly, record the new Previous PP & EP
        PreviousPP = u.PromotionPoints;
        PreviousEP = u.ExperiencePoints;
    }
}

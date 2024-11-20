using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaFemale : Unit
{
    public MilitiaFemale()
    {
        information = UtilityClass.Load<UnitDataContainer>("Assets/Resources/Data Containers/Tier 1/MilitiaFemale.asset");
    }
}

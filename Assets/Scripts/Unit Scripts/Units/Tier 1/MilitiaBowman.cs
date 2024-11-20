using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaBowman : Unit
{
    public MilitiaBowman()
    {
        information = UtilityClass.Load<UnitDataContainer>("Assets/Resources/Data Containers/Tier 1/MilitiaBowman.asset");
    }
}

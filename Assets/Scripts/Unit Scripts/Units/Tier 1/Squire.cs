using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Squire : Unit
{
    public Squire()
    {
        information = UtilityClass.Load<UnitDataContainer>("Assets/Resources/Data Containers/Tier 1/Squire.asset");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaFemale : Unit
{
    public MilitiaFemale() : base() {}

    private protected override void GetInformation()
    {
        information = UnitLoader.AssetBundle[typeof(MilitiaFemale)];
    }
}

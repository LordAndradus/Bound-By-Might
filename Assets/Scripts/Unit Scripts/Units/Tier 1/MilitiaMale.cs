using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaMale : Unit
{
    public MilitiaMale() : base() {}

    private protected override void GetInformation()
    {
        information = UnitLoader.AssetBundle[typeof(MilitiaMale)];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MilitiaBowman : Unit
{
    public MilitiaBowman() : base() {}
    private protected override void GetInformation()
    {
        information = UnitLoader.AssetBundle[typeof(MilitiaBowman)];
    }
}

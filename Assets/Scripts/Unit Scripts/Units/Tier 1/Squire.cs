using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Squire : Unit
{
    public Squire() : base() {}

    private protected override void GetInformation()
    {
        information = UnitLoader.AssetBundle[typeof(Squire)];
    }
}

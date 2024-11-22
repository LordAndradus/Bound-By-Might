using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NeophyteMagus : Unit
{
    public NeophyteMagus() : base() {}

    private protected override void GetInformation()
    {
        information = UnitLoader.AssetBundle[typeof(NeophyteMagus)];
    }
}

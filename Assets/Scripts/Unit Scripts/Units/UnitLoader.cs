using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitLoader : MonoBehaviour
{
    public static Dictionary<Type, UnitDataContainer> AssetBundle = new();

    public void Start()
    {
        AddBundles();
        PrepopulateUnits();
    } 

    public static void AddBundles()
    {
        if(AssetBundle.Count != 0) return;
        AssetBundle.Add(typeof(Apothecary), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/Apothecary.asset"));
        AssetBundle.Add(typeof(MilitiaMale), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/MilitiaMale.asset"));
        AssetBundle.Add(typeof(MilitiaFemale), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/MilitiaFemale.asset"));
        AssetBundle.Add(typeof(MilitiaBowman), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/MilitiaBowman.asset"));
        AssetBundle.Add(typeof(NeophyteMagus), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/NeophyteMagus.asset"));
        AssetBundle.Add(typeof(Squire), UtilityClass.Load<UnitDataContainer>("Assets/Data Containers/Tier 1/Squire.asset"));
    }

    private static void PrepopulateUnits()
    {
        //Populate pre-placed units
        SquadMovementHandler[] squads = FindObjectsOfType<SquadMovementHandler>(false);

        foreach(SquadMovementHandler smh in squads)
        {
            foreach(Unit u in smh.GetSquad().RetrieveUnits())
            {
                UnitGenerator.GenerateQualities(u);
            }
        }
    }
}
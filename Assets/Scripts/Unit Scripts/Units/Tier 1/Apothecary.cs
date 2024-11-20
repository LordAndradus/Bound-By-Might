using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class Apothecary : Unit
{
    public Apothecary() : base()
    {
        information = UtilityClass.Load<UnitDataContainer>("Assets/Resources/Data Containers/Tier 1/Apothecary.asset");
    }

    // private protected override void SetAttributes()
    // {
    //     ThisAttributes = new()
    //     {
    //         {
    //             AttributeType.MaxHP,
    //             new(
    //                 80, Range(), 0, new(40, 60)
    //             )
    //         },
    //         {
    //             AttributeType.HP,
    //             new(
    //                 80, Range(), 0, new(40, 60)
    //             )
    //         },
    //         {
    //             AttributeType.Armor,
    //             new(
    //                 15, Range(), 0, new(0, 0)
    //             )
    //         },
    //         {
    //             AttributeType.Weapon,
    //             new(
    //                 30, Range(), 0, new(0, 0)
    //             )
    //         },
    //         {
    //             AttributeType.Strength,
    //             new(
    //                 10, Range(), 0, new(5, 10)
    //             )
    //         },
    //         {
    //             AttributeType.Agility,
    //             new(
    //                 10, Range(), 0, new(5, 15)
    //             )
    //         },
    //         {
    //             AttributeType.Magic,
    //             new(
    //                 25, Range(), 0, new(10, 20)
    //             )
    //         },
    //         {
    //             AttributeType.Leadership,
    //             new(
    //                 25, Range(), 0, new(10, 20)
    //             )
    //         }
    //     };
    // }
}

using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitData", menuName = "Unit Data", order = 0)]
public class UnitDataContainer : ScriptableObject 
{
    [Header("Image Information")]
    public Sprite spriteView;
    public Sprite Icon;

    [Header("Unit Information")]
    public string UIFriendlyClassName;
    public string Description = "Make sure to fill in the description, young game maker";
    [SerializeField] public MoveType movement;

    [Header("Attack Information")]
    public AttackType Attack;
    public Pair<int, int> AttackArea = new(1, 1);
    public Pair<AttackPreference, AttributeType> AttackAI = new(AttackPreference.Front, AttributeType.NULL);

    [Header("Generation Information")]
    public int TierLevel = 1;
    public List<UnitDataContainer> UpgradePath = new();

    [Header("Material Cost")]
    public int GoldCost = 150;
    public int IronCost = 0;
    public int MagicGemCost = 1;
    public int HorseCost = 0;
    public int HolyTearCost = 0;
    public int AdamntiumCost = 0;
    

    [Header("Attribute Scores")]
    public AttributeScore HP, MaxHP;
    public AttributeScore Armor, Weapon;
    public AttributeScore Strength;
    public AttributeScore Agility;
    public AttributeScore Magic;
    public AttributeScore Leadership;
}

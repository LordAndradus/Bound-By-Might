using UnityEngine;

/**
 * Contains static data for certain units, IE sprite view
 */
[CreateAssetMenu(fileName = "Unit", menuName = "Unit")]
public class UnitDataContainer : ScriptableObject
{
    [Header("Image Information")]
    public Sprite spriteView;
    public Sprite Icon;

    [Header("Text Information")]
    public string UIFriendlyClassName;
    public string Description = "Make sure to fill in the description, young game maker";
    
    [Header("Attack Information")]
    public Pair<int, int> AttackArea = new(1, 1);
    public AttackType Attack;
    public Pair<aPref, AttributeType> AttackBehavior;
    public AttackPreference preference = AttackPreference.Front;

}
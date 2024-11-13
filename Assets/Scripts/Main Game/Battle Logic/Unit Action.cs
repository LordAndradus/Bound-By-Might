using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitAction
{
    Unit Unit;
    Pair<int, int> Position;
    Squad Ally;
    Squad Enemy;

    int AttackRowLength;
    int AttackColLength;
    int TotalAttackCount;

    int SelectedIndex;
    List<Unit> UnitsPicked;
    List<List<Unit>> UnitPickerLists;

    public UnitAction(Unit Unit, Pair<int, int> Position, Squad Ally, Squad Enemy)
    {
        this.Unit = Unit;
        this.Position = Position;
        this.Ally = Ally;
        this.Enemy = Enemy;

        FindEnemies();
        ApplyAction();
    }

    private void ApplyAction()
    {

    }

    private void FindEnemies()
    {
        AttackPreference ap = Unit.preference;
        AttackRowLength = Unit.AttackArea.First;
        AttackColLength = Unit.AttackArea.Second;
        TotalAttackCount = AttackRowLength + AttackColLength;

        if(Unit.Attack == AttackType.Healing)
        {
            //Populate with allies
            return;
        }
        
        if(PopulateList((AttackPreference) (((int) ap + 0) % ((int) AttackPreference.Back + 1)))
        && PopulateList((AttackPreference) (((int) ap + 1) % ((int) AttackPreference.Back + 1)))
        && PopulateList((AttackPreference) (((int) ap + 2) % ((int) AttackPreference.Back + 1))))
        {
            Debug.LogError("Could not find any units in the enemy squad at all!");
            throw new SystemException("No units found in enemy squad");
        }
    }

    private void TargetColumn(Column column)
    {
        int col = (int) column;
        AddAttackArea(Position.First + 0, col);
        if(Position.First + 1 < Enemy.MaxHeight()) AddAttackArea(Position.First + 1, col);
        else UnitPickerLists.Add(new());
        if(Position.First - 1 > 0) AddAttackArea(Position.First - 1, col);
        else UnitPickerLists.Add(new());

        SelectedIndex = 0;

        for(int i = 1; i < UnitPickerLists.Count; i++)
        {
            SelectedIndex = UnitPickerLists[i - 1].Count < UnitPickerLists[i + 0].Count ? i : SelectedIndex;
        }

        RemoveEmptyLists();
    }

    private void TargetHighestAttribute(AttributeType aType, Squad squad = null)
    {
        if(squad == null) squad = Enemy;
        
        List<Pair<List<Unit>, int>> UnitListValues = new();
        int Value = 0;
        int rows = squad.MaxLength();
        int cols = squad.MaxHeight();

        int SearchLength = Unit.AttackArea.First;
        int SearchHeight = Unit.AttackArea.Second;

        for(int startY = 0; startY <= rows - SearchHeight; startY += SearchHeight)
        {
            for(int startX = 0; startX <= cols - SearchLength; startX += SearchLength)
            {
                List<Unit> ListOfUnits = new();

                for(int y = 0; y < startY + SearchHeight; y++)
                {
                    for(int x = 0; x < startX + SearchLength; x++)
                    {
                        ListOfUnits.Add(squad.RetrieveUnitFromPosition(x, y));
                        Value += ListOfUnits[x + y].UnitAttributes[aType];
                    }
                }

                UnitListValues.Add(new(ListOfUnits, Value));
                Value = 0;
            }
        }
        
        int MaxValue = int.MinValue;
        for(int i = 0, SelectedIndex = 0; i < UnitListValues.Count; i++)
        {
            if(UnitListValues[SelectedIndex].Second > MaxValue) SelectedIndex = i;
        }

        UnitsPicked = UnitListValues[SelectedIndex].First;
    }

    private void TargetLowestAttribute(AttributeType aType)
    {

    }

    private void TargetLeader()
    {
        AddAttackArea(Enemy.RetrievePositionFromUnit(Enemy.RetrieveLeader()));
    }

    private void AddAttackArea(int First, int Second)
    {
        AddAttackArea(new(First, Second));
    }

    private bool PopulateList(AttackPreference ap)
    {
        switch(ap)
        {
            case AttackPreference.Front:
                TargetColumn(Column.Front);
                break;
            case AttackPreference.Middle:
                TargetColumn(Column.Middle);
                break;
            case AttackPreference.Back:
                TargetColumn(Column.Back);
                break;
            case AttackPreference.Most:
                TargetHighestAttribute(AttributeType.HP);
                break;
            case AttackPreference.Least:
                TargetHighestAttribute(AttributeType.Armor);
                break;
            case AttackPreference.Leader:
                TargetLeader();
                break;
        }

        return UnitPickerLists.Count == 0;
    }

    private void AddAttackArea(Pair<int, int> InitialPosition, Squad squad = null)
    {
        if(squad == null) squad = Enemy;

        UnitsPicked = new();
        for(int x = 0; x < AttackRowLength; x++)
        {
            for(int y = 0; y < AttackColLength; y++)
            {
                if(InitialPosition.First + x > 2 || InitialPosition.Second + y > 2) continue;
                Unit u = squad.RetrieveUnitFromPosition(new(InitialPosition.First + x, InitialPosition.Second + y));
                if(u != null) UnitsPicked.Add(u);
            }
        }
        UnitPickerLists.Add(UnitsPicked);
    }

    private void RemoveEmptyLists()
    {
        foreach(List<Unit> list in UnitPickerLists)
        {
            if(list.Count == 0) UnitPickerLists.Remove(list);
        }
    }

    public enum ActionType
    {
        Heal,
        Damage,
        Defend
    }

    private enum Column
    {
        Front = 0,
        Middle = 1,
        Back = 2
    }
}
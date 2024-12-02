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
    }

    public List<Unit> affected()
    {
        return UnitsPicked;
    }

    private void FindEnemies()
    {
        AttackPreference ap = Unit.AttackAI().First;
        AttackRowLength = Unit.AttackArea().First;
        AttackColLength = Unit.AttackArea().Second;

        if(Unit.Attack() == AttackType.Healing)
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

    private void TargetAttributes(AttrType aType, bool highest = true, Squad squad = null)
    {
        if(squad == null) squad = Enemy;
        int SearchLength = Unit.AttackArea().First;
        int SearchHeight = Unit.AttackArea().Second;

        List<((int, int)[,], Unit[,])> UnitArrays = UtilityClass.getSubArrays(squad.getUnitGrid(), SearchLength, SearchHeight);

        int value = highest ? int.MinValue : int.MaxValue;
        (int, int)[,] SelectedIndices;
        Unit[,] SelectedUnits;
        foreach(((int, int)[,], Unit[,]) pairs in UnitArrays)
        {
            (int, int)[,] Indices = pairs.Item1;
            Unit[,] UnitGroups = pairs.Item2;

            int SetValue = 0;

            foreach(Unit unit in UnitGroups)
            {
                SetValue += unit.ThisAttributes[aType];
            }

            if(highest && value < SetValue)
            {
                value = SetValue;
                SelectedIndices = Indices;
                SelectedUnits = UnitGroups;
            }
            else if(!highest && value > SetValue)
            {
                value = SetValue;
                SelectedIndices = Indices;
                SelectedUnits = UnitGroups;
            }
        }
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
                TargetAttributes(AttrType.HP, true);
                break;
            case AttackPreference.Least:
                TargetAttributes(AttrType.Armor, true);
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
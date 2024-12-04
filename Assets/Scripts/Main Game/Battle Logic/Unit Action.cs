using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class UnitAction
{
    //Basic information
    Unit unit;
    Pair<int, int> Position;
    Squad Ally;
    Squad Enemy;

    //Unit data
    AttackPreference aPref;
    AttrType targetAttr;
    int AttackRowLength;
    int AttackColLength;

    //Selected units for carnage
    List<Unit> UnitsPicked;

    //Stats needed for a battle animator
    List<Pair<Unit, int>> DamageReport = new();

    public List<Pair<Unit, int>> Report() { return DamageReport; }
    public List<Unit> affected() { return UnitsPicked; }

    public UnitAction(Unit unit, Pair<int, int> Position, Squad Ally, Squad Enemy)
    {
        this.unit = unit;
        this.Position = Position;
        this.Ally = Ally;
        this.Enemy = Enemy;

        aPref = unit.AttackAI().First;
        targetAttr = unit.AttackAI().Second;
        AttackRowLength = unit.AttackArea().First;
        AttackColLength = unit.AttackArea().Second;

        UnitsPicked = new();

        FindEnemies();
        ApplyAction();
    }

    private void FindEnemies()
    {
        if(unit.Attack() == AttackType.Healing)
        {
            //Populate with allies
            TargetAttributes(false, Ally);
            return;
        }

        //TODO: If we couldn't populate the list, we will continue doing things until we reach a listable outcome
        int SizeOfAttackPreference = Enum.GetValues(typeof(AttackPreference)).Length;
        for(int i = 0; UnitsPicked.Count <= 0 && i <= SizeOfAttackPreference; i++)
        {
            AttackPreference ap = (AttackPreference) (((int) aPref + i) % SizeOfAttackPreference);
            PopulateList(ap);
        }

        //Remove NULL values
        for(int i = 0; i < UnitsPicked.Count; i++) if(UnitsPicked[i] == null) UnitsPicked.RemoveAt(i--);

        //If we couldn't find any units at all for whatever reason, pick a single unit and return it
        if(UnitsPicked.Count == 0)
        {
            foreach(Unit u in Enemy.RetrieveUnits()) if(u != null) 
            {
                Debug.LogError("Could not find units based on preferences");
                UnitsPicked.Add(u);
                break;
            }

            if(UnitsPicked.Count == 0) throw new SystemException("There are no units in the enemy squad???");
        }
    }

    private void ApplyAction()
    {
        int damage = BattleManager.DamageFormula(unit);

        foreach(Unit u in UnitsPicked)
        {
            int d = UnityEngine.Random.Range(damage - unit.getMinSpread(), damage + unit.getMaxSpread());
            
            if(unit.Attack() == AttackType.Healing) d = -d;

            u.damage(d);

            //Remove dead units
            if(u.DeathFlag) Enemy.UnitDied(u);

            DamageReport.Add(new(u, d));
        }
    }

    //TODO: Add Attack Area to this column
    private void TargetColumn(Column column, Squad squad = null)
    {
        if(squad == null) squad = Enemy;

        int col = (int) column;
        List<((int, int)[,], Unit[,])> SubArrs = UtilityClass.getSubArrays(squad.getUnitGrid(), AttackRowLength, AttackColLength);

        //Check to make sure the column has at least 1 unit
        
        //TODO: For now just return the first unit found in the squad
        foreach(Unit u in squad.getUnitGrid()) if(u != null) 
        {
            UnitsPicked.Add(u);
            break;
        }
    }

    private void TargetAttributes(bool highest = true, Squad squad = null)
    {    
        if(targetAttr == AttrType.NULL) throw new SystemException("You cannot pass NULL for this AttrType");
        if(squad == null) squad = Enemy;

        List<((int, int)[,], Unit[,])> UnitArrays = UtilityClass.getSubArrays(squad.getUnitGrid(), AttackRowLength, AttackColLength);

        List<Unit[,]> units = new();

        foreach(((int, int)[,], Unit[,]) Group in UnitArrays) units.Add(Group.Item2);

        Unit[,] SelectedUnits = GetTargets(units, highest);

        foreach(Unit u in SelectedUnits) UnitsPicked.Add(u);
    }

    private void TargetLeader(Squad squad = null)
    {
        if(targetAttr == AttrType.NULL) targetAttr = AttrType.HP;
        if(squad == null) squad = Enemy;

        List<((int, int)[,], Unit[,])> UnitArrays = UtilityClass.getSubArrays(squad.getUnitGrid(), AttackRowLength, AttackColLength);

        List<Unit[,]> LeaderArrays = new();

        Unit Leader = squad.RetrieveLeader();

        foreach(((int, int)[,], Unit[,]) pairs in UnitArrays)
        {
            Unit[,] units = pairs.Item2;
            if(SubArrayContains(units, Leader)) LeaderArrays.Add(units);
        }

        Unit[,] SelectedUnits = GetTargets(LeaderArrays, false);

        foreach(Unit u in SelectedUnits) UnitsPicked.Add(u);
    }

    private Unit[,] GetTargets(List<Unit[,]> units, bool highest = true)
    {
        int value = highest ? int.MinValue : int.MaxValue;
        Unit[,] SelectedUnits = null;
        foreach(Unit[,] UnitGroup in units)
        {
            int SetValue = 0;

            foreach(Unit unit in UnitGroup)
            {
                if(unit == null) continue;
                SetValue += unit.ThisAttributes[targetAttr];
            }

            if(highest && value < SetValue)
            {
                value = SetValue;
                SelectedUnits = UnitGroup;
            }
            else if(!highest && value > SetValue)
            {
                value = SetValue;
                SelectedUnits = UnitGroup;
            }
        }

        return SelectedUnits;
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
                TargetAttributes(true);
                break;
            case AttackPreference.Least:
                TargetAttributes();
                break;
            case AttackPreference.Leader:
                TargetLeader();
                break;
        }

        return UnitsPicked.Count == 0;
    }

    private void AddAttackArea(Pair<int, int> InitialPosition, Squad squad = null)
    {
        if(squad == null) squad = Enemy;

        for(int x = 0; x < AttackRowLength; x++)
        {
            for(int y = 0; y < AttackColLength; y++)
            {
                if(InitialPosition.First + x > 2 || InitialPosition.Second + y > 2) continue;
                Unit u = squad.RetrieveUnitFromPosition(new(InitialPosition.First + x, InitialPosition.Second + y));
                if(u != null) UnitsPicked.Add(u);
            }
        }
    }

    bool SubArrayContains(Unit[,] squad, Unit target)
    {
        HashSet<Unit> UnitSet = new();

        foreach(Unit u in squad) UnitSet.Add(u);

        return UnitSet.Contains(target);
    }

    private enum Column
    {
        Front = 0,
        Middle = 1,
        Back = 2
    }
}
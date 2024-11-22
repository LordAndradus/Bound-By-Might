using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System.Text;

[Serializable]
public class Squad
{
    public event Action FieldedUnitsChanged;
    static readonly Pair<int, int> SquadSize = new(3, 3); //First one is width, second one is height

    [Header("Parameters")]
    public string Name;
    public MoveType MovementType = default;
    public int MoveSpeed = 6;

    [Header("Squad Information")]
    [SerializeField] Unit Leader;
    [SerializeField] Equipment[] equipment = new Equipment[3];
    [SerializeField] private SerializableDictionary<Pair<int, int>, Unit> _FieldedUnits;
    [SerializeField] SerializableDictionary<Pair<int, int>, Unit> FieldedUnits = new(){
        {new(0, 0), null}, {new(0, 1), null}, {new(0, 2), null}, 
        {new(1, 0), null}, {new(1, 1), null}, {new(1, 2), null}, 
        {new(2, 0), null}, {new(2, 1), null}, {new(2, 2), null},
    };

    public int TranslateMovementType(MoveType movement)
    {
        switch(movement)
        {
            case MoveType.Standard:
                MoveSpeed = 6;
                break;
            case MoveType.Slow:
                MoveSpeed = 5;
                break;
            case MoveType.Light:
                MoveSpeed = 6;
                break;
            case MoveType.Cavalry:
                MoveSpeed = 7;
                break;
            case MoveType.LightCavalry:
                MoveSpeed = 7;
                break;
            case MoveType.Flying:
                MoveSpeed = 8;
                break;
        }

        return MoveSpeed;
    }

    public void DetermineMoveType()
    {
        List<Unit> units = RetrieveUnits();
        if(units.Count == 0) return;
        //Categorizes all Unit movement types and puts them in a dictionary with the MoveType as a key, and the sum of each unit's field cost if they're in that MoveType
        Dictionary<MoveType, int> sumByMovement = units.GroupBy(unit => unit.movement()).ToDictionary(movementType => movementType.Key, MovementType => MovementType.Sum(unit => unit.GetFieldCost()));
        //Reorder dictionary to pick the highest sum which has the most weight. Set it as this squads MovementType
        MovementType = sumByMovement.OrderByDescending(pair => pair.Value).First().Key;
        TranslateMovementType(MovementType);
    }

    public Squad CreateNewSquad(Unit leader)
    {
        return CreateNewSquad(leader, new(1, 1));
    }

    public Squad CreateNewSquad(Unit leader, Pair<int, int> position)
    {
        Leader = leader;
        FieldUnit(Leader, position);
        return this;
    }

    public Unit RetrieveLeader()
    {
        if(Leader == null)
        {
            foreach(var vp in FieldedUnits)
            {
                if(vp.Value != null) 
                {
                    Leader = vp.Value;
                    break;
                }
            }
        }
        return Leader;
    }

    public List<Unit> RetrieveUnits()
    {
        List<Unit> NonNullList = new();
        foreach(Unit u in FieldedUnits.Values.ToList()) if(u != null) NonNullList.Add(u);
        return NonNullList;
    }

    public List<Pair<Unit, Pair<int, int>>> RetrieveUnitPairs()
    {
        List<Pair<Pair<int, int>, Unit>> DictionaryPairs = FieldedUnits.ToPairedList();

        List<Pair<Unit, Pair<int, int>>> ReturnList = new();

        foreach(Pair<Pair<int, int>, Unit> DPair in DictionaryPairs)
        {
            if(DPair.Second == null) 
                continue;
            ReturnList.Add(new(DPair.Second, DPair.First));
        }


        return ReturnList;
    }

    public Unit RetrieveUnitFromPosition(int x, int y)
    {
        return RetrieveUnitFromPosition(new(x, y));
    }

    public Unit RetrieveUnitFromPosition(Pair<int, int> slot)
    {
        return FieldedUnits[slot];
    }

    public Pair<int, int> RetrievePositionFromUnit(Unit u)
    {
        if(!FieldedUnits.ContainsValue(u))
        {
            Debug.LogError("Unit does not exist in Squad!\n" + u.ToString());
            throw new SystemException("Unit not in squad");
        }

        return FieldedUnits.GetKey(u);
    }

    public Equipment[] RetrieveEquipment()
    {
        return equipment;
    }

    public void FieldUnit(Unit unit, Pair<int, int> slot)
    {
        if(slot.First > SquadSize.First || slot.First > SquadSize.Second || slot.First < 0 || slot.First < 0)
        {
            Debug.LogError("Slot passed in is out of the current squad bounds, slot passed = " + slot);
            throw new Exception("Out-of-bounds Exception: Unit Field");
        }

        FieldedUnits[slot] = unit;
        FieldedUnitsChanged?.Invoke();
    }

    public Unit UnfieldUnit(Pair<int, int> slot)
    {
        Unit unit = FieldedUnits[slot];
        FieldedUnits[slot] = null;
        FieldedUnitsChanged?.Invoke();

        return unit;
    }

    public void MoveFieldedUnit(Pair<int, int> slot1, Pair<int, int> slot2)
    {
        (FieldedUnits[slot2], FieldedUnits[slot1]) = (FieldedUnits[slot1], FieldedUnits[slot2]);
    }

    public int MaxLength()
    {
        return SquadSize.First;
    }

    public int MaxHeight()
    {
        return SquadSize.Second;
    }

    public Unit[,] getUnitGrid()
    {
        Unit[,] units = new Unit[3,3];

        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                units[i, j] = FieldedUnits[new(i, j)];
            }
        }

        return units;
    }

    public (int, int, int, int, int, int) GetResourceCost()
    {
        List<Unit> units = RetrieveUnits();
        
        (int, int, int, int, int, int) resources = units.Aggregate((0, 0, 0, 0, 0, 0), (resources, unit) => (
            resources.Item1 + unit.GoldCost(),
            resources.Item2 + unit.IronCost(),
            resources.Item3 + unit.MagicGemCost(),
            resources.Item4 + unit.HorseCost(),
            resources.Item5 + unit.HolyTearCost(),
            resources.Item6 + unit.AdamantiumCost()
        ));

        return resources;
    }

    public override string ToString()
    {
        StringBuilder sb = new();

        sb.Append("Leader: " + Leader.Name + "\n");

        int count = 0;
        foreach(Unit unit in RetrieveUnits())
        {
            if(unit != Leader) sb.Append(unit.Name).Append("\n");
            if(unit == null) sb.Append("Open unit slot");
            if(unit != null) count++;
        }

        sb.Append("Number of units: " + count + "/" + FieldedUnits.Count);

        return sb.ToString();
    }
}
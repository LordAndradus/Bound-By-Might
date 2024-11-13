using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using System.Linq;
using System.Text;

[Serializable]
public class Squad : ISerializationCallbackReceiver
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
    [SerializeField] SerializableDictionary<(int, int), Unit> FieldedUnits = new(){
        {(0, 0), null}, {(0, 1), null}, {(0, 2), null}, 
        {(1, 0), null}, {(1, 1), null}, {(1, 2), null}, 
        {(2, 0), null}, {(2, 1), null}, {(2, 2), null},
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
        Dictionary<MoveType, int> sumByMovement = units.GroupBy(unit => unit.movement).ToDictionary(movementType => movementType.Key, MovementType => MovementType.Sum(unit => unit.GetFieldCost()));
        //Reorder dictionary to pick the highest sum which has the most weight. Set it as this squads MovementType
        MovementType = sumByMovement.OrderByDescending(pair => pair.Value).First().Key;
        TranslateMovementType(MovementType);
    }

    public Squad CreateNewSquad(Unit leader)
    {
        Leader = leader;
        
        leader.SquadPosition = new(1, 1);

        FieldUnit(leader, leader.SquadPosition.ToTuple());

        return this;
    }

    public Unit RetrieveLeader()
    {
        return Leader;
    }

    public List<Unit> RetrieveUnits()
    {
        List<Unit> units = FieldedUnits.Values.ToList();
        foreach(Unit u in units) if(u == null) units.Remove(u);
        return units;
    }

    public List<Pair<Unit, (int, int)>> RetrieveUnitPairs()
    {
        List<Pair<(int, int), Unit>> DictionaryPairs = FieldedUnits.ToPairedList();

        List<Pair<Unit, (int, int)>> pairs = new();

        foreach(Pair<(int, int), Unit> DPair in DictionaryPairs)
        {
            pairs.Add(new(DPair.Second, DPair.First));
        }

        return pairs;
    }

    public Unit RetrieveUnitFromPosition(int x, int y)
    {
        return RetrieveUnitFromPosition((x, y));
    }

    public Unit RetrieveUnitFromPosition((int, int) slot)
    {
        return FieldedUnits[slot];
    }

    public (int, int) RetrievePositionSlotFromUnit(Unit u)
    {
        if(!FieldedUnits.ContainsValue(u))
        {
            Debug.LogError("Unit does not exist in Squad!\n" + u.ToString());
            throw new SystemException("Unit not in squad");
        }

        u.SquadPosition = new(FieldedUnits.GetKey(u));
        return u.SquadPosition.ToTuple();
    }

    public Pair<int, int> RetrievePositionFromUnit(Unit u)
    {
        return u.SquadPosition;
    }

    public Equipment[] RetrieveEquipment()
    {
        return equipment;
    }

    public void FieldUnit(Unit unit, (int, int) slot)
    {
        if(slot.Item1 > SquadSize.First || slot.Item1 > SquadSize.Second || slot.Item1 < 0 || slot.Item1 < 0)
        {
            Debug.LogError("Slot passed in is out of the current squad bounds, slot passed = " + slot);
            throw new Exception("Out-of-bounds Exception: Unit Field");
        }

        unit.SquadPosition = new(slot);
        FieldedUnits[slot] = unit;
        FieldedUnitsChanged?.Invoke();
    }

    public Unit UnfieldUnit((int, int) slot)
    {
        Unit unit = FieldedUnits[slot];
        FieldedUnits[slot] = null;
        unit.SquadPosition = null;
        FieldedUnitsChanged?.Invoke();

        return unit;
    }

    public void MoveFieldedUnit((int, int) slot1, (int, int) slot2)
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

    public (int, int, int, int, int, int) GetResourceCost()
    {
        List<Unit> units = RetrieveUnits();
        
        (int, int, int, int, int, int) resources = units.Aggregate((0, 0, 0, 0, 0, 0), (resources, unit) => (
            resources.Item1 + unit.GoldCost,
            resources.Item2 + unit.IronCost,
            resources.Item3 + unit.MagicGemCost,
            resources.Item4 + unit.HorseCost,
            resources.Item5 + unit.HolyTearCost,
            resources.Item6 + unit.AdamntiumCost
        ));

        return resources;
    }

    public void OnBeforeSerialize()
    {
        //Implement each pair inside the main readable dictionary (FieldedUnits) to the serialized dictionary (_FieldedUnits)
        _FieldedUnits.Clear();
        foreach(var kvp in FieldedUnits)
        {
            if(kvp.Value == null) continue;
            (int, int) TuplePair = kvp.Key;
            Unit unit = kvp.Value;
            unit.SquadPosition = new(TuplePair);
            _FieldedUnits.Add(new(TuplePair), unit);
        }
    }

    public void OnAfterDeserialize()
    {
        //Re-seat units into the main readable dictionary.
        foreach(var kvp in _FieldedUnits)
        {
            Pair<int, int> KeyPair = kvp.Key;
            Unit unit = kvp.Value;
            FieldedUnits[KeyPair.ToTuple()] = unit;
        }
    }
}
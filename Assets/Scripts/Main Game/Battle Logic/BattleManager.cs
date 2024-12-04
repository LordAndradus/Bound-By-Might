//For now it will simulate a battle.
//If we happen to animate it later, we will save a replay of what actions each unit took.

//Order of events: Magic -> Ranged -> Melee -> Healing; Magic will only have one action per battle
//Special rules

//I MIGHT add in different tactics: IE Aggressive. Units in the order of events target Highest HP first, Cautious: Increase Armor by 50, but decrease attack by 30

using System.Collections.Generic;

public class BattleManager
{
    Squad Attacking, Attacked;

    UnitPositionGrid upg;

    Queue<Unit> AttackOrder = new();
    
    //Create a replay for the animation
    List<UnitAction> actions = new();

    /// <summary>
    /// Bit 0 - Attacking destroyed
    /// Bit 1 - Attacked destroyed
    /// Bit 2 - 
    /// </summary>
    byte flags;

    public BattleManager(UnitPositionGrid UnitGrid, SquadMovementHandler Attacking, SquadMovementHandler Attacked)
    {
        upg = UnitGrid;
        this.Attacking = Attacking.GetSquad();
        this.Attacked = Attacked.GetSquad();
        LoadSquads(this.Attacking, this.Attacked);
        
        SimulateBattle();

        //If Attacking is empty, or Attacked is empty, then add a deletion callback.
        AddDeleteCallback(Attacking);
        AddDeleteCallback(Attacked);
    }

    private void AddDeleteCallback(SquadMovementHandler squad)
    {
        if(squad.GetSquad().Count() <= 0)
        {
            squad.DeleteSquadCallback += () => {
                upg.RemoveSquad(squad);
                Controller.ClearSelections();
            };
        }
    }

    public void SimulateBattle()
    {
        //Each battle has 2 rounds
        for(int i = 0; i < 2; i++) SimulateRound();
        
        //Now we report the actions to an animator
    }

    private void LoadSquads(Squad Attacking, Squad Attacked)
    {
        this.Attacking = Attacking;
        this.Attacked = Attacked;

        //Sequence of attacking => Magic -> Arhcery -> Melee -> Healing -> (Maybe) Firearms
        foreach(Unit u in Attacking.RetrieveUnits()) AttackOrder.Enqueue(u);
        foreach(Unit u in Attacked.RetrieveUnits()) AttackOrder.Enqueue(u);
    }

    private void SimulateRound()
    {
        Simulate(true); //Attack!
        Simulate(false); //Retaliate!
    }

    private void Simulate(bool Initiator)
    {
        Squad init = Initiator ? Attacking : Attacked;
        Squad take = Initiator ? Attacked : Attacking;

        for(int i = 0; i < AttackOrder.Count; i++)
        {
            Unit unit = AttackOrder.Dequeue();

            if(!init.ContainsUnit(unit)) break;

            //Skip dead units
            if(unit.DeathFlag) continue;

            UnitAction ua = new(unit, init.RetrievePositionFromUnit(unit), init, take);
            actions.Add(ua);

            //Requeue the unit so it's at the back
            AttackOrder.Enqueue(unit);
        }

        foreach(Unit unit in AttackOrder)
        {
            if(!init.ContainsUnit(unit)) break; //We reach the point in the attack order where 

            //Skip dead units and remove them from queue
            if(unit.DeathFlag)
            {
                AttackOrder.Dequeue();
                continue;
            }

            UnitAction ua = new(unit, Attacked.RetrievePositionFromUnit(unit), init, take);
            actions.Add(ua);
        }
    }

    public static int DamageFormula(Unit unit)
    {
        int damage = 0;

        //Basic formula. The main stat adds 1 points each, while the side-points add 0.1 points. The points can be truncated.
        //Another factor is the weapon. The weapon stat, unless the main stat, is a multiplicative property out of 255.
        //So if weapon is 20, then it's ((255 + 20) / 255) * damage, otherwise if it is the main stat, then add 2 points per.

        AttackType unitAttack = unit.Attack(); 

        float maxer = 1;
        float miner = 0.1f;

        //Strength based
        if(unitAttack == AttackType.Melee || unitAttack == AttackType.Melee)
        {
            damage += (int) (unit.ThisAttributes[AttrType.Strength] * maxer);
            damage += (int) (unit.ThisAttributes[AttrType.Magic] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Agility] * miner);
        }
        //Agility based
        else if(unitAttack == AttackType.Archery)
        {
            damage += (int) (unit.ThisAttributes[AttrType.Strength] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Magic] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Agility] * maxer);
        }
        //Magic based
        else if(unitAttack == AttackType.Magic || unitAttack == AttackType.Healing)
        {
            damage += (int) (unit.ThisAttributes[AttrType.Strength] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Magic] * maxer);
            damage += (int) (unit.ThisAttributes[AttrType.Agility] * miner);
        }
        //Weapon based
        else if(unitAttack == AttackType.Firearms)
        {
            damage += (int) (unit.ThisAttributes[AttrType.Strength] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Magic] * miner);
            damage += (int) (unit.ThisAttributes[AttrType.Agility] * miner);
            damage += (int) ((unit.ThisAttributes[AttrType.Weapon] + 1) * maxer);
        }

        //Damage = Damage * ((weapon + 255) / 255)
        if(unitAttack != AttackType.Firearms) damage = (int) ((float) damage * ((unit.ThisAttributes[AttrType.Weapon] + 255.0f) / 255.0f));
        return damage;
    }
}
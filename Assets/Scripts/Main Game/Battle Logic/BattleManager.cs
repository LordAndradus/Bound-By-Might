//For now it will simulate a battle.
//If we happen to animate it later, we will save a replay of what actions each unit took.

//Order of events: Magic -> Ranged -> Melee -> Healing; Magic will only have one action per battle
//Special rules

//I MIGHT add in different tactics: IE Aggressive. Units in the order of events target Highest HP first, Cautious: Increase Armor by 50, but decrease attack by 30

using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    public static BattleManager instance;

    SquadMovementHandler Attacking, Attacked;

    UnitPositionGrid upg;

    List<Unit> AttackOrder = new();

    //State of the battle for the animator
    public int round = 1;
    public bool retaliate = false;
    public bool finished = false;
    
    //Create a replay for the animation
    List<UnitAction> actions;

    public BattleManager(UnitPositionGrid UnitGrid, SquadMovementHandler Attacking, SquadMovementHandler Attacked)
    {
        instance = this;
        upg = UnitGrid;
        this.Attacking = Attacking;
        this.Attacked = Attacked;
        LoadSquads();
        
        BattleAnimator.instance.SetupAnimator(Attacking.GetSquad(), Attacked.GetSquad());

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

    private void LoadSquads()
    {
        Squad attacking = Attacking.GetSquad();
        Squad attacked = Attacked.GetSquad();

        EnqueueSquad(attacking);
        AttackOrder.Add(null); //Null value to separate the squads
        EnqueueSquad(attacked);
    }

    private void EnqueueSquad(Squad squad)
    {
        //Sequence of attacking => Magic -> Archery -> Melee -> Healing -> (Maybe) Firearms
        List<Unit> units = squad.RetrieveUnits();
        //Sort the current list by AttackType as prescribed above
        units.Sort((x, y) => {
            return x.Attack().CompareTo(y.Attack());
        });
        //Lastly, add it in the attack order
        foreach(Unit unit in units) AttackOrder.Add(unit);
    }

    public void Simulate()
    {
        actions = new();

        SquadMovementHandler init = retaliate ?  Attacked : Attacking;
        SquadMovementHandler take = retaliate ? Attacking : Attacked;

        for(int i = 0; i < AttackOrder.Count; i++)
        {
            Unit unit = AttackOrder[0];
            AttackOrder.RemoveAt(0);

            //We reached the break point to separate squads, enqueue to back and break out to report
            if(unit == null)
            {
                AttackOrder.Add(null);
                break;
            }

            if(!init.GetSquad().ContainsUnit(unit)) break;

            UnitAction ua = new(unit, init.GetSquad().RetrievePositionFromUnit(unit), init.GetSquad(), take.GetSquad());
            actions.Add(ua);

            //Remove dead units from queue
            for(int j = 0; j < AttackOrder.Count; j++)
            {
                Unit u = AttackOrder[j];

                if(u == null) continue;
                if(u.DeathFlag)
                {
                    AttackOrder.RemoveAt(j);
                }
            }

            //Check if there are any squads fully depleted
            if(take.Empty() || init.Empty())
            {
                if(take.Empty()) take.StartDeletionCallback();
                if(init.Empty()) init.StartDeletionCallback();
                finished = true;
                break;
            }

            //Requeue the unit so it's at the back
            AttackOrder.Add(unit);
        }

        //Revert actions going backwards
        for(int i = actions.Count - 1; i >= 0; i--) actions[i].RevertAction();

        //Now we report the actions to an animator
        BattleAnimator.instance.NotifyDataset(actions);

        retaliate = !retaliate;
        if(!retaliate) round++;
        if(round == 3) finished = true;
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
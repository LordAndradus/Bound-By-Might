//For now it will simulate a battle.
//If we happen to animate it later, we will save a replay of what actions each unit took.

//Order of events: Magic -> Ranged -> Melee -> Healing; Magic will only have one action per battle
//Special rules

//I MIGHT add in different tactics: IE Aggressive. Units in the order of events target Highest HP first, Cautious: Increase Armor by 50, but decrease attack by 30

using System.Collections.Generic;

public class BattleManager
{
    Squad Attacking, Attacked;

    List<Unit> AttackOrder = new();

    //Create a replay for the animation
    List<UnitAction> actions;

    public BattleManager(Squad Attacking, Squad Attacked)
    {
        LoadSquads(Attacking, Attacked);
        SimulateBattle();
    }

    public void SimulateBattle()
    {
        //Each battle has 2 rounds
        SimluateRound();
        SimluateRound();
    }

    private void LoadSquads(Squad Attacking, Squad Attacked)
    {
        this.Attacking = Attacking;
        this.Attacked = Attacked;

        //Sequence of attacking => Magic -> Arhcery -> Melee -> Healing -> (Maybe) Firearms
        foreach(Unit u in Attacking.RetrieveUnits()) AttackOrder.Add(u);
        foreach(Unit u in Attacked.RetrieveUnits()) AttackOrder.Add(u);
    }

    private void SimluateRound()
    {
        SimulateAttack();
        SimulateRetaliation();
    }

    private void SimulateAttack()
    {
        foreach(Unit unit in Attacking.RetrieveUnits())
        {
            UnitAction ua = new(unit, Attacking.RetrievePositionFromUnit(unit), Attacking, Attacked);
        }
    }

    private void SimulateRetaliation()
    {

    }
}
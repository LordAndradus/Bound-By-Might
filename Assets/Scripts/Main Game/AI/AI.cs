using System.Collections.Generic;

//Outline:
//Goal: Lower the attack power of the enemy

public class AI
{
    Stack<State> StateMachine = new();
    Dictionary<State, System.Action> StateMap = new();
    public static Difficulty difficulty = Difficulty.Peasant;
    
    public enum Difficulty
    {
        Peasant, //Uses random number generator for tactics. Always hires, always attacks within range, and always moves in a random direction
        Soldier, //Primitive, but it will attack the squads if it thinks it can hurt them subtantially, and it will move towards your squads if its squads are not entrenched. It also has some behaviors like moving towards a position, focusing on a particular squad, etc.
        Captain, //Somewhat complex. It does everything primitive can, but it also assigns each action a value for each squad.
        General //Complex, this is if I want to spend time outside of class programming this
    }

    public enum Mode
    {
        MoveToPosition,
        Follow,
        Defend,
        CaptureObjective,
        KillCertainSquad,
    }

    public enum State
    {
        Attacking,
    }
}
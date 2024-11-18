/// <summary>
/// The basic use of this AI is to assign an overall action to work towards. IE Capture a point, kill a certain squad,
/// Gather a certain resource amount, etc. It will share this directive to its lower-level AI counterparts.
/// 
/// It will calculate a value of how close it is to its progress. It will then allocate certain resources to each Sub-AI.
/// If we reach the GoalCompletionCap, then we simply continue in the direction we were going, because it's obviously working.\
/// Elsewise, we keep doing certain actions until we hit that cap.
/// </summary>
public class GrandStrategyAI : AI
{
    //int GoalCompletion;
    //static readonly int GoalCompletionCap = 10000;
    directive goal;

    public GrandStrategyAI(directive d)
    {
        goal = d;
    }
    
    public enum directive
    {
        CapturePosition,
        MoveToPosition,
        GatherResource,
        KillSpecificSquad,
        DefendPosition,
        KillNumberOfSquads,
        DefendCertainSquad,
    }
}
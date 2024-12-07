using UnityEngine;

public class LevelJumpstarter : MonoBehaviour
{
    [SerializeField] Level ToPlay;

    public PathFinder pf;
    public CombatGridSystem cgs;

    void Start()
    {
        if(ToPlay == null)
        {
            Debug.LogError("A level was not passed for initialization!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        ToPlay.SetMapSize(ToPlay);

        transform.parent.GetComponent<TurnManager>().StartTurnManager(ToPlay);

        foreach(Pair<int, int> position in ToPlay.UnwalkableSpaces)
        {
            PathNode pn = PathFinder.instance.GetGridSystem().GetGridObject(position.First, position.Second);
            pn.SetIsWalkable(false);
        }

        PathFinder.instance.GetGridSystem().SetDebugText();
    }
}

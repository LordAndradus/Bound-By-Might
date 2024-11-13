using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelJumpstarter : MonoBehaviour
{
    [SerializeField] Level ToPlay;
    [SerializeField] int Check;

    // Start is called before the first frame update
    void Start()
    {
        if(ToPlay == null)
        {
            throw new System.Exception("Did not pass level for initialization!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        transform.parent.GetComponent<TurnManager>().StartTurnManager(null);
        //Served its purpose, so get rid of it to save resources
        Destroy(this);
    }
}

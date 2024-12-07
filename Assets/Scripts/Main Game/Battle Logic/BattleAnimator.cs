using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BattleAnimator : MonoBehaviour
{
    public static BattleAnimator instance;
    List<UnitAction> data;
    int dataSize = 0;
    int index = 0;
    List<Pair<Unit, int>> damageReport;
    bool rush = false;
    bool close = false;

    /* bool auto = true;bool autoMode;
    Coroutine nextMove;
    Color autoEnabled = Color.green;
    Color autoDisabled = Color.red; */

    private float waitInSeconds = 0.33f;

    [Header("Visualizers")]
    [SerializeField] SquadCombatDisplay attacker;
    [SerializeField] SquadCombatDisplay attacked;

    [Header("Interatives")]
    [SerializeField] Button next;
    [SerializeField] Button Auto;
    [SerializeField] Button prev;
    [SerializeField] Button skip;
    [SerializeField] Button confirm;

    public void init()
    {
        instance = this;

        skip.onClick.AddListener(() => {
            RushSimulation();
        });

        confirm.onClick.AddListener(() => {
            gameObject.SetActive(false);
            close = true;
        });

        confirm.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public bool CloseAnimator()
    {
        if(close)
        {
            close = false;
            return true;
        }
        else 
        {
            return false;
        }
    }

    public void SetupAnimator(Squad attacker, Squad attacked)
    {
        gameObject.SetActive(true);
        confirm.gameObject.SetActive(false);
        this.attacker.displaySquad(attacker);
        this.attacked.displaySquad(attacked);
        BattleManager.instance.Simulate();
        StartCoroutine(DisplayNextAction());
    }

    public void FinishedAnimation()
    {
        rush = false;
        confirm.gameObject.SetActive(true);
        StopAllCoroutines();
        confirm.gameObject.SetActive(true);
    }

    public void NotifyDataset(List<UnitAction> data)
    {
        this.data = data;
        dataSize = data.Count;
        index = 0;

        if(dataSize == 0)
        {
            //The BattleManager hasn't detected that turns ran out, nor that one squad is destroyed. Something went wrong.
            if(!BattleManager.instance.finished)
            {
                Debug.LogError("Could not find dataset, something went wrong");
                Debug.Log(string.Format("Battle Manager State:\nRound: {0}\nRetaliate? {1}\nFinished? {2}", BattleManager.instance.round, BattleManager.instance.retaliate, BattleManager.instance.finished));
            }
        }
    }

    IEnumerator DisplayNextAction()
    {
        if(!rush) yield return new WaitForSeconds(waitInSeconds);

        //Show action
        data[index++].ApplyAction();
        attacked.UpdateDisplay();
        attacker.UpdateDisplay();

        //When finished with current data-set, get another one
        if(index == dataSize)
        {
            BattleManager.instance.Simulate();

            if(BattleManager.instance.finished)
            {
                FinishedAnimation();
                yield break;
            }
        }

        //Continue coroutine
        StartCoroutine(DisplayNextAction());
    }

    public void RushSimulation()
    {
        rush = true;
    }
}

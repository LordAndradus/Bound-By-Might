using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SquadCombatDisplay : MonoBehaviour
{
    Squad toShow;
    [SerializeField] Image[] UnitImage = new Image[9];
    [SerializeField] StatusBar[] UnitHealth = new StatusBar[9];

    public void displaySquad(Squad squad)
    {
        toShow = squad;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        for(int i = 0; i < 9; i++)
        {
            UnitImage[i].color = Color.white;
            UnitImage[i].gameObject.SetActive(false);
            UnitHealth[i].gameObject.SetActive(false);
        }

        List<Pair<Unit, Pair<int, int>>> UnitCoordinates = toShow.RetrieveUnitPairs();
        List<Pair<Unit, Pair<int, int>>> DeadCoordinates = toShow.RetrieveDeadPairs();
        foreach(Pair<Unit, Pair<int, int>> units in UnitCoordinates)
        {
            int idx = units.Second.First * 3 + units.Second.Second;
            UnitImage[idx].gameObject.SetActive(true);
            UnitHealth[idx].gameObject.SetActive(true);

            UnitImage[idx].sprite = units.First.Icon();
            UnitHealth[idx].SetValue(units.First.ThisAttributes[AttrType.HP], units.First.ThisAttributes[AttrType.MaxHP]);
        }

        foreach(Pair<Unit, Pair<int, int>> units in DeadCoordinates)
        {
            int idx = units.Second.First * 3 + units.Second.Second;
            UnitImage[idx].gameObject.SetActive(true);
            UnitHealth[idx].gameObject.SetActive(true);

            UnitImage[idx].sprite = units.First.Icon();
            UnitImage[idx].color = new(0.5f, 0.5f, 0.5f);
            UnitHealth[idx].SetValue(0f);
        }
    }
}

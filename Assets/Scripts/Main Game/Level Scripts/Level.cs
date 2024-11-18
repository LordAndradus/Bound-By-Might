using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level", order = 0), Serializable]
public class Level : ScriptableObject
{
    public Level instance;
    public PathFinder pf;
    public CombatGridSystem cgs;

    //Bounds -> First = Negative boundary, Second = Positive boundary : The bounds are in terms of how many tiles
    public Pair<float, float> MapSize { get; set; }

    public string thisName;

    [SerializeField] int width = 18;
    [SerializeField] int height = 10;

    public Level()
    {
        if(width < 18 || height < 10)
        {
            if(width < 18)
            {
                Debug.LogError(String.Format("Level Width of {1}, minimum is 18", width));
                width = 18;
            }

            if(height < 10)
            {
                Debug.LogError(String.Format("Level Height of {1}, minimum is 10", height));
                height = 10;
            }
        }

        instance = this;

        MapSize = new(width, height);
    }   

    public void setLevelInstance()
    {
        instance = this;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
/*     void Awake()
    {
        MouseHover = new GameObject("MosueHover", typeof(SpriteRenderer), typeof(MouseCursor));
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        sr.sprite = Resources.Load<Sprite>("Assets/Sprites/Tile Hover.png");

        sr.drawMode = SpriteDrawMode.Simple;
        sr.spriteSortPoint = SpriteSortPoint.Center;
        sr.material = new Material(Shader.Find("Sprites/Default"));

        sr.sortingLayerName = "GridOutline";
        sr.sortingOrder = 0;
    } */

    public static Pair<float, float> CursorPosition = new(1, 1);

    void Update()
    {
        Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        float xPos = camPos.x <= 0 ? Mathf.Floor(camPos.x) + 0.5f : Mathf.Ceil(camPos.x) - 0.5f;
        float yPos = camPos.y <= 0 ? Mathf.Floor(camPos.y) + 0.5f : Mathf.Ceil(camPos.y) - 0.5f;
        CursorPosition.set(xPos + 0.5f, yPos + 0.5f);
        transform.position = new Vector3(xPos, yPos, 0);
    }
}

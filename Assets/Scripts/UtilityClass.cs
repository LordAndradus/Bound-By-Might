using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UtilityClass
{
    public static Vector3 GetScreenMouseToWorldWithZ(Camera camera = null)
    {
        if(camera == null) camera = Camera.main;

        Vector3 position = camera.ScreenToWorldPoint(Input.mousePosition);
        
        return position;
    }

    /// <summary>
    /// Returns the Mouse position relative to where it is in the world coordinate system
    /// </summary>
    /// <param name="camera">The main Camera that Unity uses</param>
    /// <returns>A Vector3 Position</returns>
    public static Vector3 GetScreenMouseToWorld(Camera camera = null)
    {
        Vector3 Pos = GetScreenMouseToWorldWithZ(camera);
        Pos.z = 0f;
        return Pos;
    }

    public static Vector3 CopyVector(Vector3 Source)
    {
        return new Vector3(Source.x, Source.y, Source.z);
    }

    public static Vector3 CopyAbsVector(Vector3 source)
    {
        return new Vector3(Mathf.Abs(source.x), Mathf.Abs(source.y), Mathf.Abs(source.z));
    }

    /// <summary>
    /// Takes a Game Object and translates it local position to that on the world coordinate system
    /// </summary>
    /// <param name="child"></param>
    /// <param name="target"></param>
    /// <returns>A world coordinate from the specified child object</returns>
    public static Vector3 SetWorldPosition(GameObject parent, GameObject child, Vector3 target)
    {
        Transform transform = parent.transform;

        child.transform.localPosition = transform.InverseTransformPoint(target);

        return target;
    }

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default, int fontSize = 40, Color color = default, TextAnchor textAnchor = default, TextAlignment textAlignment = default, int sortingOrder = 5000)
    {
        if(color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, color, textAnchor, textAlignment, sortingOrder);
    }

    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject(text, typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns>A string with spaces between uppercase letters</returns>
    public static string UIFriendlyClassName(string name)
    {
        return Regex.Replace(name, "(?<!^)([A-Z])", " $1");
    }

    public static void CreateEmptyMeshArray(int QuadCount, out Vector3[] verticies, out int[] triangles, out Vector2[] uvs)
    {
        verticies = new Vector3[4 * QuadCount];
        triangles = new int[4 * QuadCount];
        uvs = new Vector2[6 * QuadCount];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <param name="Size"></param>
    /// <param name="Anchor"></param>
    /// <param name="colors"></param>
    /// <param name="Listener"></param>
    /// <returns>A Button GameObject</returns>
    public static GameObject CreateClickableObject(Transform parent, string name, Vector2 Size, Vector2 Anchor = default, ColorBlock colors = default, UnityEngine.Events.UnityAction Listener = default)
    {
        GameObject b = new(name);

        b.transform.SetParent(parent);

        Button button = b.AddComponent<Button>();

        Image i = b.AddComponent<Image>();

        button.targetGraphic = i;

        //Set button sprite image

        RectTransform rt = b.GetComponent<RectTransform>();
        rt.sizeDelta = Size;
        rt.anchoredPosition = Anchor;
        rt.localScale = Vector3.one;

        ColorBlock buttonColors = button.colors;

        if(colors == default)
        {
            buttonColors.normalColor = new Color(1f, 1f, 1f, 1f);
            buttonColors.highlightedColor = new Color(0.8f, 0.8f, 0.8f, 0.4f);
            buttonColors.pressedColor = new Color(0.8f, 0f, 0f, 0.8f);
            buttonColors.selectedColor = new Color(0.4f, 0f, 0f, 1f);
        }
        else
        {
            buttonColors.normalColor = colors.normalColor;
            buttonColors.highlightedColor = colors.highlightedColor;
            buttonColors.pressedColor = colors.pressedColor;
            buttonColors.selectedColor = colors.selectedColor;
        }

        button.colors = buttonColors;

        button.onClick.AddListener(Listener);

        return b;
    }

    public static GameObject UnanchoredPrefabObject(string PrefabPath, Transform parent, string name = null, Vector3 Position = default)
    {
        GameObject newObj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(PrefabPath), parent, false);
        newObj.name = name;
        newObj.transform.position = Position;
        return newObj;
    }

    public static GameObject CreatePrefabObject(string PrefabPath, Transform parent, string name = null, Vector3 Position = default)
    {
        GameObject Prefab = Load<GameObject>(PrefabPath);
        GameObject newObj = UnityEngine.Object.Instantiate(Prefab, Position, Quaternion.identity, parent);
        if(name != null) newObj.name = name;
        newObj.transform.position = Position;
        newObj.transform.localPosition = Position;
        return newObj;
    }

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        //Strip Resources and Assets from path
        string[] parts = path.Split(Path.DirectorySeparatorChar);
        if(parts.Length == 1) parts = path.Split(Path.AltDirectorySeparatorChar);

        if(parts[0] == "Assets") parts[0] = null;
        
        StringBuilder sb = new();
        bool SafeToAdd = !parts.Contains<String>("Resources");
        foreach(string part in parts)
        {
            if(part == null) continue;
            if(SafeToAdd) sb.Append(Path.GetFileNameWithoutExtension(part) + Path.DirectorySeparatorChar);
            if(part.Equals("Resources")) SafeToAdd = true;
        }

        path = sb.ToString();

        path = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));

        return Resources.Load<T>(path);
    }

    public static GameObject GetHoverObject(string tag = null)
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        GameObject HoveredObject = null;

        if(tag == null && raycastResults.Count > 0) return raycastResults[0].gameObject;

        if(raycastResults.Count > 0)
        {
            foreach(RaycastResult obj in raycastResults)
            {
                if(obj.gameObject.CompareTag("UnitGrid"))
                {
                    HoveredObject = obj.gameObject;
                    break;
                }
            }
        }

        return HoveredObject;
    }

    public static List<GameObject> GetAllChildObjectsWithComponent<T>(GameObject parent) where T : Component
    {
        List<GameObject> Children = new();

        foreach(Transform child in parent.transform)
        {
            T component = child.GetComponent<T>();
            if(component != null) Children.Add(child.gameObject);
            Children.AddRange(GetAllChildObjectsWithComponent<T>(child.gameObject));
        }

        return Children;
    }

    public static List<GameObject> GetAllChildObjects(GameObject parent)
    {
        List<GameObject> Children = new();

        foreach(Transform child in parent.transform)
        {
            Children.Add(child.gameObject);
            Children.AddRange(GetAllChildObjects(child.gameObject));
        }

        return Children;
    }

    public static void DeleteListContent(GameObject parent)
    {
        List<GameObject> children = GetAllChildObjects(parent);

        if(children.Count > 0)
        {
            foreach(GameObject child in children.ToList())
            {
                children.Remove(child);
                if(child != null) UnityEngine.Object.Destroy(child);
            }
        }
    }

    public static void DeleteListContent<T>(GameObject parent) where T : Component
    {
        List<GameObject> children = GetAllChildObjectsWithComponent<T>(parent);

        if(children.Count > 0)
        {
            foreach(GameObject child in children.ToList())
            {
                children.Remove(child);
                if(child != null) UnityEngine.Object.Destroy(child);
            }
        }
    }

    public static void DeleteListContentChildren<T>(GameObject parent) where T : Component
    {
        List<GameObject> ListContent = GetAllChildObjectsWithComponent<T>(parent);
    }

    public static List<((int, int)[,], T[,])> getSubArrays<T>(T[,] array, int searchWidth, int searchHeight) where T : UnityEngine.Object
    {
        int aHeight = array.GetLength(0);
        int aWidth = array.GetLength(1);

        List<((int, int)[,], T[,])> subArray = new();

        for(int y = 0; y < aHeight - searchHeight; y++)
        {
            for(int x = 0; x < aWidth - searchWidth; x++)
            {
                (int, int)[,] indices = new (int, int)[searchHeight, searchWidth];
                T[,] arr = new T[searchHeight, searchWidth];

                for(int row = 0; row < searchWidth; row++)
                {
                    for(int col = 0; col < searchHeight; col++)
                    {
                        indices[row, col] = new(y + row, x + col);
                        arr[row, col] = array[y + row, x + col];
                    }
                }

                subArray.Add(new(indices, arr));
            }
        }

        return subArray;
    }


    /// <summary>
    /// Prints a 2D Array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    public static void print2DArray<T>(T[,] array) where T : UnityEngine.Object
    {
        int width = array.GetLength(1);
        int height = array.GetLength(0);

        StringBuilder sb = new();

        sb.Append("Array = [");
        for(int i = 0; i < height - 1; i++)
        {
            T[] arr = new T[width];
            for(int j = 0; j < width; j++) arr[j] = array[i, j];

            for(int j = 0; j < width - 1; j++)
            {
                sb.Append(string.Format("{0}, ", arr[j].ToString()));
            }
            sb.Append(string.Format("{0}\n", arr[width - 1].ToString()));
        }

        for(int i = 0; i < width - 1; i++) sb.Append(string.Format("{0}, ", array[height - 1, i].ToString()));
        sb.Append(string.Format("{0}]\n", array[height - 1, width - 1].ToString()));

        Debug.Log(sb.ToString());
    }

    public static void printDeepArray(Array array)
    {
        int rank = array.Rank;

        
    }
}
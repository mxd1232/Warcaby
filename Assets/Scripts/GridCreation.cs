using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCreation : MonoBehaviour
{
    GameObject field;
    public static List<GameObject> Fields = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        float boardSize = GameObject.Find("Board").GetComponent<RectTransform>().sizeDelta.x;
        float boardSideStart = -boardSize / 2 + boardSize / 16;

        int j = 0, k = 0; ;
        for (int i = 0; i < 32; i++)
        {
            GameObject field = CreateField(i);
            field.GetComponent<RectTransform>().localPosition = new Vector3(boardSideStart + boardSize / 4 * j + boardSize / 8 * (k % 2), boardSideStart + boardSize / 8 * k, 0);

            field.GetComponent<Coordinates>().X = (j * 2) + k % 2;
            field.GetComponent<Coordinates>().Y = 7 - k;

            Fields.Add(field);

            j++;
            if (j >= 4)
            {
                j = 0;
                k++;
            }
        }
    }

    GameObject CreateField(int i)
    {
        field = new GameObject("Field" + i);

        field.transform.parent = GameObject.Find("Grid").transform;

        field.AddComponent<RectTransform>();
        field.GetComponent<RectTransform>().sizeDelta = new Vector2(64, 64);
      
        field.AddComponent<CanvasRenderer>();

        field.AddComponent<Image>();
        field.GetComponent<Image>().useSpriteMesh = true;
        field.GetComponent<Image>().color = new Color32(70, 70, 70, 255);

        field.AddComponent<FieldHandler>();

        field.AddComponent<Coordinates>();

        return field;
    }
}

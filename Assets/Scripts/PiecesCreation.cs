using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesCreation : MonoBehaviour
{
    GameObject piece;
    public static List<GameObject> Pieces = new List<GameObject>();
    public static BoardLogic BoardLogic = new BoardLogic();

    void Start()
    {     
        // GetComponent<BoardLogic>();

        float boardSize = GameObject.Find("Board").GetComponent<RectTransform>().sizeDelta.x;
        float boardSideStart = -boardSize / 2 + boardSize / 16;

        int j = 0, k = 0; ;
        for (int i = 0; i < 24; i++)
        {
            GameObject piece = createPiece(i);

            piece.GetComponent<Coordinates>().X = (j * 2)+k%2;
            

            if (i < 12)
            {
                

                piece.GetComponent<RectTransform>().localPosition = new Vector3(boardSideStart+boardSize/4*j+ boardSize / 8*(k%2), boardSideStart + boardSize / 8 * k, 0);
                piece.GetComponent<Image>().color = new Color32(200, 200, 200, 255);

                piece.GetComponent<Coordinates>().Y = 7 - k;
            }
            else
            {
                

                piece.GetComponent<RectTransform>().localPosition = new Vector3(boardSideStart + boardSize / 4 * j + boardSize / 8 * (k % 2), boardSideStart + boardSize / 8 * k, 0);
                piece.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

                piece.GetComponent<Coordinates>().Y = 7 - k;
            }
            //Debug.Log(BoardLogic.FindChecker(1,0));

            Pieces.Add(piece);

            j++;
            if(j>=4)
            {
                j = 0;
                k++;
            }                
            if(i==11)
            {
                k += 2;
            }
        }
    }

    GameObject createPiece(int i)
    {
        

        piece = new GameObject("Piece" + i);

        piece.transform.parent = GameObject.Find("Pieces").transform;
        //Add Components
        piece.AddComponent<RectTransform>();
        piece.GetComponent<RectTransform>().sizeDelta = new Vector2(55,55);

        piece.AddComponent<CanvasRenderer>();

        piece.AddComponent<Image>();
        piece.GetComponent<Image>().sprite = Resources.Load<Sprite>("Checker");
        piece.GetComponent<Image>().useSpriteMesh = true;

        piece.AddComponent<CanvasGroup>();

        piece.AddComponent<PieceHandler>();

        piece.AddComponent<Coordinates>();

        return piece;
    }
}

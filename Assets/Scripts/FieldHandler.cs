using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FieldHandler : MonoBehaviour, IDropHandler
{
    

    public void OnDrop(PointerEventData eventData)
    {
      if(eventData.pointerDrag != null)
        {
            if(eventData.pointerDrag != null)
            {
                //board Logic
                BoardLogic boardLogic = PiecesCreation.BoardLogic;
                char turn = boardLogic.Turn;


                int oldX = eventData.pointerDrag.GetComponent<Coordinates>().X;
                int oldY = eventData.pointerDrag.GetComponent<Coordinates>().Y;

                int newX = GetComponent<Coordinates>().X;
                int newY = GetComponent<Coordinates>().Y;

                Debug.Log(oldX+" "+oldY+" "+newX+" "+newY);

                if (boardLogic.MoveChecker(boardLogic.Checkers, boardLogic.FindChecker(boardLogic.Checkers,oldX, oldY), newX, newY) ==true)
                {        
                    eventData.pointerDrag.GetComponent<Coordinates>().X = newX;
                    eventData.pointerDrag.GetComponent<Coordinates>().Y = newY;

                    if(boardLogic.delX!=-1 && boardLogic.delY!=-1)
                    {

                        GameObject piece = FindCheckerToDestroy(boardLogic);  
                        if(piece!=null)
                        {
                            Destroy(piece);
                        }
                    }
                    if(boardLogic.FindChecker(boardLogic.Checkers,newX, newY).isQueen==true)
                    {
                        eventData.pointerDrag.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 45);
                    }

                    

                    MovePieceToField(eventData.pointerDrag,gameObject);

                    /*  Debug.Log(boardLogic.Turn);
                      Debug.Log(boardLogic.IsMoveAtack);
                      Debug.Log(boardLogic.FindChecker(boardLogic.Checkers, newX, newY).isQueen);
                      */
                    if(boardLogic.Turn!=turn && GameObject.Find("AI").GetComponent<AILogic>().IsAIOn==true)
                    {

                        StartCoroutine("UpdateTree");

                        //opoznienie
                      // StartCoroutine("UpdateTree");
                    }
 
                   // GameObject.Find("TreeBoard").GetComponent<TreeCreation>().UpdateTree();


                }
              
            }
        }
    }

    IEnumerator UpdateTree()
    {        
        yield return new WaitForEndOfFrame();
       
        GameObject.Find("TreeBoard").GetComponent<TreeCreation>().AIMove();
        GameObject.Find("TreeBoard").GetComponent<TreeCreation>().UpdateTree();
    }
    public static void MovePieceToField(GameObject piece,GameObject field)
    {
        piece.GetComponent<RectTransform>().anchoredPosition = field.GetComponent<RectTransform>().anchoredPosition;
        piece.GetComponent<PieceHandler>().droppedOnSlot = true;
    }

    public static GameObject FindCheckerToDestroy(BoardLogic boardLogic)
    {
        int x = boardLogic.delX;
        int y = boardLogic.delY;

        foreach (var piece in PiecesCreation.Pieces)
        {
          if(x==piece.GetComponent<Coordinates>().X && y== piece.GetComponent<Coordinates>().Y)
            {
                boardLogic.delX = -1;
                boardLogic.delY = -1;

                PiecesCreation.Pieces.Remove(piece);
                return piece;
            }

        }

        return null;
    }
}

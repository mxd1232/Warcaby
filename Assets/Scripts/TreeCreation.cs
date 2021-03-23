using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeCreation : MonoBehaviour
{
    // Start is called before the first frame update

    public MoveTree CurrentMoveTree = new MoveTree();
    public int TreeSize;

    void Start()
    {
        UpdateTree();
        //moveTree.PrintTree(moveTree);
        //moveTree.PrintValues(moveTree);


    }

    //ad ro


    public void CreatePhysicalTree(MoveTree moveTree, int row)
    {
        if (moveTree.Moves.Count == 0)
        {
            return;
        }
        int numberInList = 0;

        foreach (var move in moveTree.Moves)
        {

            CreatePhysicalTree(move, row + 1);

            if (move.Node == null)
            {
                move.Node = CreateNode(move, numberInList, row);
            }      
          
            numberInList++;
        }


    }
    public void SetNodeParent(MoveTree parentMoveTree, MoveTree moveTree)
    {
        if(TreeSize==1)
        {
            foreach (var move in moveTree.Moves)
            {
                move.Node.transform.SetParent(gameObject.transform, false);
            }
            return;
        }

        if(parentMoveTree!=null)
        {
            if (moveTree.CurrentBoardLogic != null && parentMoveTree.CurrentBoardLogic == null)
            {
                moveTree.Node.transform.SetParent(gameObject.transform, false);
            }
        }
        
       
        foreach (var move in moveTree.Moves)
        {
            if(moveTree.Node!=null)
            {
                move.Node.transform.SetParent(moveTree.Node.transform, false);
            }
            if (move.Moves.Count == 0)
            {
               
                continue;
            }

            SetNodeParent(moveTree, move);


        }
       



    }

    public void DrawTree(MoveTree parentMoveTree, MoveTree moveTree,int X,int Y)
    {
        Y++;   
        

        foreach (var move in moveTree.Moves)
        {
            if (move.Node != null)
            {
                move.Node.transform.localPosition += new Vector3(moveTree.Moves.Count*1*X, -1*Y, 0);
            }
            if (move.Moves.Count == 0)
            {

                continue;
            }
            X++;

            DrawTree(moveTree, move,X,Y);


        }

    }

    public GameObject CreateNode(MoveTree moveTree, int numberInRow, int row)
    {
        GameObject Node = new GameObject("TreeNode" + "R(" + row + ")" + "(" + numberInRow + ")");


        //Add Components
        Node.AddComponent<RectTransform>();
        Node.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);

        Node.AddComponent<CanvasRenderer>();

        Node.AddComponent<Image>();
        Node.GetComponent<Image>().sprite = Resources.Load<Sprite>("Checker");
        Node.GetComponent<Image>().useSpriteMesh = true;

        Node.AddComponent<CanvasGroup>();

        Node.AddComponent<NodeData>();

        Node.GetComponent<NodeData>().StartingCoordinates = moveTree.StartingCoordinates;
        Node.GetComponent<NodeData>().Moveset = moveTree.MovesInOneTurn;
        Node.GetComponent<NodeData>().NumberInRow = numberInRow;
        Node.GetComponent<NodeData>().Row = row;
        Node.GetComponent<NodeData>().ValueOfMovement = moveTree.ValueOfMovement;


        Node.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        Node.GetComponent<Image>().color = new Color32(0, 0, 0, 255);

        return Node;
    }

    public void DeleteNodes(MoveTree moveTree)
    {
        if(moveTree.Moves.Count==0)
        {
            Destroy(moveTree.Node);
            moveTree.Node = null;
            return;
        }
        foreach (var move in moveTree.Moves)
        {
            DeleteNodes(move);
        }
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    public void UpdateTree()
    {
        DeleteNodes(CurrentMoveTree);
        CurrentMoveTree = new MoveTree
        {
            MovesAhead = TreeSize
        };
        CurrentMoveTree.FindAllPossibleMoves(PiecesCreation.BoardLogic, TreeSize);
        CurrentMoveTree.FindTheBestMove(CurrentMoveTree, PiecesCreation.BoardLogic.Turn);

        CreatePhysicalTree(CurrentMoveTree, 0);
        SetNodeParent(null, CurrentMoveTree);
    }
 
    public void AIMove()
    {
        BoardLogic boardLogic = PiecesCreation.BoardLogic;
        char turn = boardLogic.Turn;

        UpdateTree();

        MoveTree nextMove = CurrentMoveTree;

        if (turn == 'B')
        {
            int value = -10000;

            foreach (var move in CurrentMoveTree.Moves)
            {
                if (move.ValueOfMovement > value)
                {
                    nextMove = move;
                    value = move.ValueOfMovement;
                }
            }
        }
        else
        {
            int value = 10000;
            foreach (var move in CurrentMoveTree.Moves)
            {
                if (move.ValueOfMovement < value)
                {
                    nextMove = move;
                    value = move.ValueOfMovement;
                }
            }
        }
        //find piece
        GameObject movingPiece = FindPieceByXY(nextMove.StartingCoordinates.x,nextMove.StartingCoordinates.y);
        //find field
        int numberOfLastMoveInTurn = nextMove.MovesInOneTurn.Count;
        if(numberOfLastMoveInTurn>0)
        {
            numberOfLastMoveInTurn--;
        }

        GameObject fieldForPiece = FindFieldByXY(nextMove.MovesInOneTurn[numberOfLastMoveInTurn].x, nextMove.MovesInOneTurn[numberOfLastMoveInTurn].y);

        Vector2Int previousMove = nextMove.StartingCoordinates;

        foreach (var singleMove in nextMove.MovesInOneTurn)
        {
            

            PiecesCreation.BoardLogic.MoveChecker(
                boardLogic.Checkers, 
                boardLogic.FindChecker(boardLogic.Checkers, previousMove.x, previousMove.y),
                singleMove.x,
                singleMove.y);

            if (boardLogic.delX != -1 && boardLogic.delY != -1)
            {
                GameObject checkerToDestroy = FieldHandler.FindCheckerToDestroy(boardLogic);
               if(checkerToDestroy!=null)
                {
                    Destroy(checkerToDestroy);
                }
            }
            previousMove = new Vector2Int(singleMove.x,singleMove.y);
        }

        //move piece to field
        FieldHandler.MovePieceToField(movingPiece, fieldForPiece);
        movingPiece.GetComponent<Coordinates>().X = fieldForPiece.GetComponent<Coordinates>().X;
        movingPiece.GetComponent<Coordinates>().Y = fieldForPiece.GetComponent<Coordinates>().Y;

        if (boardLogic.FindChecker(boardLogic.Checkers, movingPiece.GetComponent<Coordinates>().X, movingPiece.GetComponent<Coordinates>().Y).isQueen == true)
        {
            movingPiece.GetComponent<RectTransform>().sizeDelta = new Vector2(45, 45);
        }

      //  UpdateTree();

    }
    public static GameObject FindPieceByXY(int X, int Y)
    {

        foreach (var piece in PiecesCreation.Pieces)
        {
            if (X == piece.GetComponent<Coordinates>().X && Y == piece.GetComponent<Coordinates>().Y)
            {
                return piece;
            }

        }
        throw new Exception();

    }
    public static GameObject FindFieldByXY(int X, int Y)
    {

        foreach (var field in GridCreation.Fields)
        {
            if (X == field.GetComponent<Coordinates>().X && Y == field.GetComponent<Coordinates>().Y)
            {
                return field;
            }

        }
        throw new Exception();

    }



}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTree
{
    public GameObject Node;

    public List<MoveTree> Moves = new List<MoveTree>();
    public List<Vector2Int> MovesInOneTurn = new List<Vector2Int>();
    
    public BoardLogic CurrentBoardLogic = null;
    public Vector2Int StartingCoordinates;

    public int ValueOfMovement;

    public int MovesAhead;


    public void FindAllPossibleMoves(BoardLogic startingBoardLogic, int movesAhead)
    {
        BoardLogic boardLogic = startingBoardLogic.CopyBoard();

        foreach (var checker in boardLogic.Checkers)
        {
            if (checker.Color == boardLogic.Turn)
            {              
            FindAllPossibleMovesForChecker(boardLogic, checker, movesAhead);
            }
        }
    }
    public void FindTheBestMove(MoveTree moveTree, char turn)
    {
        MinMax(moveTree, turn);
    }

    public int MinMax(MoveTree moveTree, char turn)
    {
        if (moveTree.MovesAhead == 0)
        {
            return moveTree.ValueOfMovement;
        }
        if (turn == 'B')
        {
            int value = -10000;
            foreach (var move in moveTree.Moves)
            {
                value = Math.Max(value, MinMax(move, 'C'));
            }
            moveTree.ValueOfMovement = value;
            return value;
        }
        else
        {
            int value = 10000;
            foreach (var move in moveTree.Moves)
            {
                value = Math.Min(value, MinMax(move, 'B'));
            }
            moveTree.ValueOfMovement = value;
            return value;
        }

    }



    public void FindAllPossibleMovesForChecker(BoardLogic startingBoardLogic, Checker checker, int movesAhead)
    {
        Vector2Int startingCoords = new Vector2Int(checker.PositionX, checker.PositionY);

        int k;
        if (checker.isQueen == true)
        {
            k = 7;
        }
        else if (startingBoardLogic.SearchForNormalAtackForChecker(startingBoardLogic.Checkers,checker) == true)
        {
            k = 2;
        }
        else
        {
            k = 1;
        }


        BoardLogic boardLogic = startingBoardLogic.CopyBoard();
        checker = boardLogic.FindChecker(boardLogic.Checkers, checker.PositionX, checker.PositionY);

        for (int x = -k; x <= k; x += 1)
        {
            for (int y = - k; y <= k; y += 1)
            {
                if (x == 0 || y == 0)
                {
                    continue;
                }
                if (Math.Abs(x) - Math.Abs(y) != 0)
                {
                    continue;
                }
                int newX = checker.PositionX + x;
                int newY = checker.PositionY + y;
                if (newX < 0 || newX > 7 || newY < 0 || newY > 7)
                {
                    continue;
                }



                if (boardLogic.MoveChecker(boardLogic.Checkers, checker, newX, newY) == true)
                {
                    if(newX==5 && newY==2)
                    {

                    }

                    MoveTree movement = CreateMoveTreeNode(boardLogic,checker,movesAhead,startingCoords,k);

                    movement.EvaluateMovement();

                    Moves.Add(movement);

                    //next moves
                    if(movement.MovesAhead>0)
                    {
                        movement.FindAllPossibleMoves(boardLogic, movement.MovesAhead);
                    }
                    //cleanup
                    boardLogic = startingBoardLogic.CopyBoard();
                    checker = boardLogic.FindChecker(boardLogic.Checkers, startingCoords.x, startingCoords.y);
                }
            }
        }
    }
    public MoveTree CreateMoveTreeNode(BoardLogic boardLogic,Checker checker, int movesAhead,Vector2Int startingCoords,int k)
    {
        MoveTree movement = new MoveTree();
        movement.CurrentBoardLogic = boardLogic;
        movement.MovesInOneTurn.Add(new Vector2Int(checker.PositionX, checker.PositionY));
        movement.MovesAhead = movesAhead - 1;

        movement.StartingCoordinates = new Vector2Int(startingCoords.x, startingCoords.y);

        if (boardLogic.IsMoveAtack == true)
        {
            Vector2Int firstMove = new Vector2Int(checker.PositionX, checker.PositionY);

            movement.MovesInOneTurn = FindFullAtackMove(boardLogic, checker, k);
            movement.MovesInOneTurn.Insert(0, firstMove);
        }
        return movement;
    }

    public List<Vector2Int> FindFullAtackMove(BoardLogic startingBoardLogic, Checker checker, int k)
    {
        List<Vector2Int> atackMoves = new List<Vector2Int>();

        char startingTurn = startingBoardLogic.Turn;

        while (startingTurn == startingBoardLogic.Turn)
        {
            for (int x = -k; x <= k; x += 1)
            {
                for (int y = -k; y <= k; y += 1)
                {         
                    if(Math.Abs(x)!=Math.Abs(y))
                    {
                        continue;
                    }

                    if (startingBoardLogic.MoveChecker(startingBoardLogic.Checkers, checker, checker.PositionX + x, checker.PositionY + y) == true)
                    {
                        atackMoves.Add(new Vector2Int(checker.PositionX, checker.PositionY));

                        x = -k;
                        y = -k;
                    }
                }
            }
        }

        return atackMoves;
    }

    public void EvaluateMovement()
    {
        if(CurrentBoardLogic==null)
        {
            return;
        }

        int valueOfMovement=0;
        int k, q;

        foreach (var checker in CurrentBoardLogic.Checkers)
        {
            if(checker.Color=='B')
            {
                k = 1;
            }
            else
            {
                k = -1;
            }
            if(checker.isQueen==false)
            {
                q = 1;
            }
            else
            {
                q = 3;
            }
            valueOfMovement += k * q;

        }
        ValueOfMovement = valueOfMovement;
    }
    public static MoveTree PickChildMove(MoveTree moveTree, Vector2Int startingCoords, List<Vector2Int> movesInTurn)
    {
        //starting coords 
        //last coords
        foreach (var nextMove in moveTree.Moves)
        {
            if(startingCoords.x==nextMove.StartingCoordinates.x && startingCoords.y == nextMove.StartingCoordinates.y && CompareListOfVector2Int(movesInTurn,nextMove.MovesInOneTurn) ==true)
            {
                return nextMove;
            }
        }
        return null;
    }
    public static bool CompareListOfVector2Int(List<Vector2Int> List1, List<Vector2Int> List2)
    {
        int i = 0;
        if (List2.Count !=List1.Count)
        {
            return false;
        }

        foreach (var move in List1)
        {
            
            if (move.x != List2[i-1].x || move.y != List2[i-1].y)
            {
                return false;
            }
            i++;
        }
        return true;
    }
    /*
    public void PrintValues(MoveTree moveTree)
    {
        if(moveTree.ValueOfMovement!=0)
        Debug.Log("value:   " + moveTree.ValueOfMovement + "movesAhead: "+ moveTree.MovesAhead);

        if (moveTree.Moves.Count == 0)
            {
                return;
            }

        foreach (var move in moveTree.Moves)
        {
            PrintValues(move);
        }
    }

    public void PrintTree(MoveTree moveTree)
    {
        
        string str = "";
        str += "start ruch:" + moveTree.StartingCoordinates.x + " " + moveTree.StartingCoordinates.y;
        foreach (var move in moveTree.Moves)
        {
             
            str+=("pionek: " + move.StartingCoordinates.x + " " + move.StartingCoordinates.y + "ruchy: " + move.MovesInOneTurn[0].x + " " + move.MovesInOneTurn[0].y);         
        }
       
        Debug.Log(str);
        

        foreach (var move in moveTree.Moves)
        {
            if(move.Moves.Count==0)
            {
                continue;
            }
            foreach (var secondMove in move.Moves)
            {        
                PrintTree(secondMove);
            }
        }
        
    }*/
}

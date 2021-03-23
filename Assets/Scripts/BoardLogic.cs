using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoardLogic
{
    public List<Checker> Checkers = new List<Checker>();
    static int[,] EmptyBoard = new int[8, 8];
    public char Turn { get; private set; } = 'B';
  //public char Winner = ' ';

    public bool IsMoveAtack = false;
    public Checker AtackingChecker;

    public int delX =-1, delY =-1;
   
    public BoardLogic()
    {

        //8x8
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if ((x % 2 == 0 && y % 2 == 1) || (x % 2 == 1 && y % 2 == 0))
                {
                    EmptyBoard[x, y] = 1;

                    if (y < 3)
                    {
                        Checkers.Add(new Checker { Color = 'C', PositionX = x, PositionY = y, isQueen = false });
                        //  Console.Write("C" + " ");
                    }
                    else if (y > 4)
                    {
                        Checkers.Add(new Checker { Color = 'B', PositionX = x, PositionY = y, isQueen = false });
                        //   Console.Write("B" + " ");
                    }
                    else
                    {
                        //    Console.Write("1" + " ");
                    }

                }
                else
                {
                    EmptyBoard[x, y] = 0;
                    //  Console.Write("#" + " ");
                }
            }
            //   Console.WriteLine();
        }

       
    }
   
    public List<Checker> CopyCurrentCheckers()
    {
        List<Checker> newCheckers = new List<Checker>();

        foreach (var checker in Checkers)
        {
                newCheckers.Add(new Checker {
                    Color=checker.Color,
                    PositionX=checker.PositionX,
                    PositionY=checker.PositionY,
                    isQueen=checker.isQueen });
                
        }

        return newCheckers;
    }

    public BoardLogic CopyBoard()
    {
        BoardLogic boardLogic = new BoardLogic();
        boardLogic.Checkers = CopyCurrentCheckers();
        boardLogic.Turn = Turn;
        if (AtackingChecker != null) 
        {
            boardLogic.AtackingChecker = new Checker
            {
                Color = AtackingChecker.Color,
                PositionX = AtackingChecker.PositionX,
                PositionY = AtackingChecker.PositionY,
                isQueen = AtackingChecker.isQueen
            };
            boardLogic.IsMoveAtack = IsMoveAtack;
        }
        boardLogic.delX = delX;
        boardLogic.delY = delY;


        return boardLogic;
    }


    public bool MoveChecker(List<Checker> checkers,Checker checker, int newX, int newY)
    {

        if (checker == null)
        {
            //  Console.WriteLine("Brak figry, powtórz ruch");
            return false;
        }
        
        if(newX <0 || newX>7 || newY<0 || newY>7)
        {
            return false;
        }
   
        if(Math.Abs(newX%2-newY%2)!=1)
        {
            return false;
        }
        //TODO -add forced atack here

        if(AtackingChecker!=null & AtackingChecker!=checker)
        {
            return false;
        }
        if (checker.Color != Turn)
        {
            return false;
        }

        if (checker.isQueen == false)
        {
            if(MoveNormalChecker(checkers,checker, newX, newY)==true)
            {
                return true;
            }

        }
        else if (checker.isQueen == true)
        {          
            if (MoveQueenChecker(checkers,checker, newX, newY) ==true)
            {
                return true;
            }

        }
        return false;
    }

    private bool MoveQueenChecker(List<Checker> checkers, Checker checker, int newX, int newY)
    {
        bool isForcedAtack = SearchForAnyAtackForAllCheckers(checkers, checker);

        //jesli nowe miejsce jest puste
        if (IsQueenMovePossible(checkers,checker, newX, newY,isForcedAtack, out Checker killableChecker) == true)
        {
            //verify forced atack
         /*   Debug.Log(killableChecker);
            Debug.Log("atack" + IsMoveAtack);
            */

            if(killableChecker == null && IsMoveAtack == true)
            {
                return false;
            }
            if(killableChecker==null && IsMoveAtack==false)
            {
                MoveCheckerToNewCoords(checker, newX, newY);
                InvertTurn();
            }
            else if(killableChecker != null)
            {
                MoveAtackQueen(checkers,checker, newX, newY, killableChecker);
            }

           // Debug.Log("atack2" + IsMoveAtack);

            return true;


            //jesli pomiedzy nowym miejscem jest tylko jedna figura
        }
        return false;
    }

    private bool MoveAtackQueen(List<Checker> checkers, Checker checker, int newX, int newY, Checker killableChecker)
    {
        MoveCheckerToNewCoords(checker, newX, newY);

        // Checkers.Remove(killableChecker);     
        RemoveChecker(checkers,killableChecker.PositionX, killableChecker.PositionY);

        bool isForcedAtack = SearchForQueenAtackForChecker(checkers,checker);

        SetToAtack(isForcedAtack,checker);

        return true;

    }


    private bool SearchForQueenAtackForChecker(List<Checker> checkers, Checker checker)
    {
        //4 coords
        //++ az dojdzie do granicy
        int x = checker.PositionX;
        int y = checker.PositionY;

        for (int i = -1; i <= 1; i += 2)
        {
            for (int j = -1; j <= 1; j += 2)
            {
                while (x >= 0 && x <= 7 && y >= 0 && y <= 7)
                {

                    x += i;
                    y += j;

                    if (x < 0 || x > 7 || y < 0 || y > 7)
                    {
                        break;
                    }
                    if (IsQueenMovePossible(checkers, checker, x, y, false, out Checker killableChecker) == true)
                    {
                        if (killableChecker != null)
                        {
                            return true;
                        }

                    }
                }
                 x = checker.PositionX;
                 y = checker.PositionY;
            }
               
        }
            

        return false;
    }

    private bool SearchForQueenAtackForAllCheckers(List<Checker> checkers, Checker c)
    {
        foreach (var checker in checkers)
        {
            if (c.Color == checker.Color && checker.isQueen==true)
            {
                if (SearchForQueenAtackForChecker(checkers,checker) == true)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsQueenMovePossible(List<Checker> checkers, Checker checker, int newX, int newY, bool isForcedAtack, out Checker killableChecker)
    {
      
        killableChecker = null;

        if(newX<0 || newX>7 || newY<0 || newY>7)
        {
            return false;
        }

        if (FindChecker(checkers,newX, newY) != null)
        {
            //Console.WriteLine("Space is not empty");

            return false;
        }

        if (Math.Abs(checker.PositionX - newX) < 1 || Math.Abs(checker.PositionY - newY) < 1)
        {
            //Console.WriteLine("Not Far Enough");

            return false;
        }

       

        int inumX = Math.Sign(newX - checker.PositionX);
        int inumY = Math.Sign(newY - checker.PositionY);

        if (Math.Abs(inumX) != Math.Abs(inumY))
        {
            Console.WriteLine("Zly ruch ponow probe");
            return false;
        }

       
        

        int opositeCheckersInTheWay = 0;

        for (int x = checker.PositionX + inumX, y = checker.PositionY + inumY; (x != newX+inumX) || (y != newY+inumY); x += inumX, y += inumY)
        {
            Checker checkedChecker = FindChecker(checkers,x, y);
            if (checkedChecker == null)
            {
                continue;
            }
            if (checkedChecker.Color == checker.Color)
            {
                return false;
            }
            else if (checkedChecker.Color != checker.Color)
            {
                killableChecker = checkedChecker;
                opositeCheckersInTheWay++;
            }

        }
        if(opositeCheckersInTheWay == 0 && isForcedAtack ==false)
        {
            return true;
        }
        if (opositeCheckersInTheWay > 1 || (isForcedAtack == true && opositeCheckersInTheWay != 1))
        {
            killableChecker = null;
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool MoveNormalChecker(List<Checker> checkers, Checker checker, int newX, int newY)
    {
        int inumerator;
        if (checker.Color == 'C' && Turn == 'C')
        {
            inumerator = 1;
        }
        else if (checker.Color == 'B' && Turn == 'B')
        {
            inumerator = -1;
        }
        else
        {
           // Console.WriteLine("Blad, Rusz sie swoim kolorem");
            return false;
        }



        if (FindChecker(checkers,newX, newY) == null)
        {
            int Ydifference = newY - checker.PositionY;
            int Xdifference = newX - checker.PositionX;

            bool isForcedAtack = SearchForNormalAtackForChecker(checkers,checker);

            if ((Ydifference == +2 || Ydifference == -2) && (Xdifference == 2 || Xdifference == -2) && isForcedAtack == true)
            {
                if (MoveAtackNormal(checkers,checker, newX, newY) == true)
                {
                    //
                    if (checker.isQueen == false && IsMoveAtack==false &&(((checker.PositionY == 0 && checker.Color == 'B') || (checker.PositionY == 7 && checker.Color == 'C'))))
                    {
                        ChangeCheckerToQueen(checker);                       
                    }
                    
                    return true;
                }
            }
            else if ((Xdifference == 1 || Xdifference == -1) && newY == checker.PositionY + inumerator && SearchForAnyAtackForAllCheckers(checkers,checker) == false)
            {
                MoveCheckerToNewCoords(checker, newX, newY);

                if (checker.isQueen == false && (((checker.PositionY == 0 && checker.Color == 'B') || (checker.PositionY == 7 && checker.Color == 'C'))))
                {
                    ChangeCheckerToQueen(checker);
                }
                InvertTurn();

                return true;
            }
        }
        return false;
    }

    public void ChangeCheckerToQueen(Checker checker)
    {
        checker.isQueen = true;
    }
    public bool MoveAtackNormal(List<Checker> checkers, Checker checker, int newX, int newY)
    {

        int moveXFor = checker.PositionX + (newX - checker.PositionX) / 2;
        int moveYFor = checker.PositionY + (newY - checker.PositionY) / 2;

        if (FindOpositeChecker(checkers,moveXFor, moveYFor, checker.Color) != null)
        {
            RemoveChecker(checkers,moveXFor, moveYFor);

            MoveCheckerToNewCoords(checker, newX, newY);

            bool isForcedAtack = SearchForNormalAtackForChecker(checkers,checker);

            SetToAtack(isForcedAtack,checker);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveChecker(List<Checker> checkers, int x,int y)
    {
        delX = x;
        delY = y;

        checkers.Remove(FindChecker(checkers,x, y));
    }

    public bool SearchForNormalAtackForChecker(List<Checker> checkers, Checker checker)
    {
        int currX = checker.PositionX;
        int currY = checker.PositionY;

        for (int x = -1; x <= 1; x += 2)
        {
            for (int y = -1; y <= 1; y += 2)
            {
                if(currX + 2*x <0 || currX + 2 * x > 7 || currY + 2 * y < 0 || currY + 2 * y < 0)
                {
                    continue;
                }

                if (FindOpositeChecker(checkers,currX + x, currY + y, checker.Color) != null
                    && FindChecker(checkers,currX + 2 * x, currY + 2 * y) == null
                    && (currX + 2 * x >= 0 && currX + 2 * x < 8 && currY + 2 * y >= 0 && currY + 2 * y < 8) == true)
                {
                    return true;
                }
            }
        }

        return false;

    }
    public bool SearchForNormalAtackForAllCheckers(List<Checker> checkers, Checker c)
    {
        foreach (var checker in checkers)
        {
            if (c.Color == checker.Color)
            {
                if (SearchForNormalAtackForChecker(checkers,checker) == true)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private bool SearchForAnyAtackForAllCheckers(List<Checker> checkers, Checker checker)
    {
        if (SearchForNormalAtackForAllCheckers(checkers,checker) == true)
        {
            return true;
        }
        if (SearchForQueenAtackForAllCheckers(checkers,checker) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void SetToAtack(bool isForcedAtack, Checker checker)
    {
        if (isForcedAtack == true)
        {
            IsMoveAtack = true;
            AtackingChecker = checker;

        }
        else
        {
            IsMoveAtack = false;
            AtackingChecker = null;
            InvertTurn();
        }
    }
    public Checker FindChecker(List<Checker> checkers, int x, int y)
    {
        return checkers.Find(item => item.PositionX == x && item.PositionY == y);
    }

    public Checker FindOpositeChecker(List<Checker> checkers, int x, int y, int color)
    {
        //with different color
        return checkers.Find(item => item.PositionX == x && item.PositionY == y && item.Color != color);
    }

    void InvertTurn()
    {
        if (Turn == 'C')
        {
            Turn = 'B';
        }
        else if (Turn == 'B')
        {
            Turn = 'C';
        }
    }
    void MoveCheckerToNewCoords(Checker checker, int newX, int newY)
    {
        checker.PositionX = newX;
        checker.PositionY = newY;
    }

}
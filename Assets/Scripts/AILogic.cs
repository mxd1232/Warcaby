using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILogic : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsAIOn;
    public bool IsInstant;
    public bool IsFinished;
    public bool IsWhiteStarting;
    public float TimeDelay;

    public bool MakeAMove;

    public bool IsWhiteTurn = true;
    void Start()
    {
        if(IsAIOn==true && IsWhiteStarting==true)
        {
            Move();
        }
    }
    void Update()
    {
        if(MakeAMove==true)
        {
            Move();
            MakeAMove = false;
        }
    }
    void Move()
    {      
            if (IsInstant == true)
            {
                //todo -inner
                StartCoroutine(WholeGame());

            }
            else
            {
                StartCoroutine(SingleMove());
            }     
    }

    // Update is called once per frame
    IEnumerator SingleMove()
    {
        yield return new WaitForEndOfFrame();

        System.DateTime startTime = System.DateTime.UtcNow;

        if (IsWhiteTurn)
        {
            GameObject.Find("TreeBoard").GetComponent<TreeCreation>().AIMove();
        }
        else
        {
            GameObject.Find("TreeBoardWeaker").GetComponent<TreeCreation>().AIMove();           
        }

        System.TimeSpan ts = System.DateTime.UtcNow - startTime;
        Debug.Log(ts.ToString());

        UpdateTrees();
    }
    
    IEnumerator WholeGame()
    {
        yield return new WaitForEndOfFrame();


        while(IsFinished==false)
        {
            yield return new WaitForSeconds(TimeDelay);
            if (IsWhiteTurn==true)
            {
                GameObject.Find("TreeBoard").GetComponent<TreeCreation>().AIMove();
                IsWhiteTurn = false;
            }
            else
            {
                GameObject.Find("TreeBoardWeaker").GetComponent<TreeCreation>().AIMove();
                IsWhiteTurn = true;
            }

            UpdateTrees();
        }

    }
    public void UpdateTrees()
    {
        GameObject.Find("TreeBoard").GetComponent<TreeCreation>().UpdateTree();
        GameObject.Find("TreeBoardWeaker").GetComponent<TreeCreation>().UpdateTree();

        if (GameObject.Find("TreeBoardWeaker").GetComponent<TreeCreation>().CurrentMoveTree.Moves.Count == 0)
        {
            IsFinished = true;
        }
    }
}

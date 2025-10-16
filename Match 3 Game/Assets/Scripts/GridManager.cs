using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Collections;
using UnityEditor.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;

public class GridManager : MonoBehaviour
{
    public Transform TopLeftPivot;
    public float CellSize = 1f;

    //i is the row
    //j is the column
    public GameObject[,] TokenArray;
    public bool[,] CheckingArray;

    //Match List
    List<List<GameObject>> MatchList = new List<List<GameObject>>();

    //Array of token prefabs
    public GameObject[] TokenPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckingArray = new bool[8,8];

        TokenArray = new GameObject[8, 8];

        TopLeftPivot.position = new Vector3(-4*CellSize + CellSize/2, 4*CellSize - CellSize / 2, 0);

        ClearCheckingArray();

        FillWithTokens();

        SwapOutMatches();

        //Check for ability to make a move
    }

    //For testing
    IEnumerator TestingSwapOut()
    {
        yield return new WaitForSeconds(5F);
        SwapOutMatches();
    }

    void SwapOutMatches()
    {
        for (int i = 0; i < TokenArray.GetLength(0); i++)
        {
            for (int j = 0; j < TokenArray.GetLength(1); j++)
            {
                
                List<GameObject> tklist = new List<GameObject>(TokenPrefabs);

                //column check
                if (i - 1>= 0 && i + 1<= 7 && TokenArray[i - 1, j].name == TokenArray[i, j].name && TokenArray[i + 1, j].name == TokenArray[i, j].name)
                { 
                    RemovingTokenPrefabs(tklist,TokenArray[i,j]);
                    if (j-1>=0)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i, j - 1]);
                    }
                    if (j+1<=7)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i, j + 1]);
                    }

                    int ran_token = UnityEngine.Random.Range(-1, 2);
                    int ran_replace_token = UnityEngine.Random.Range(0,tklist.Count);
                    Destroy(TokenArray[i + ran_token, j]);
                    TokenArray[i + ran_token, j] = Instantiate(tklist[ran_replace_token], positionBasedOnPivot(i + ran_token, j), Quaternion.identity);
                    Debug.Log("Swapped " + (i + ran_token) + "," + j + " from "+ TokenArray[i, j].name + " to " + TokenArray[i + ran_token, j].name);
                }

                tklist = new List<GameObject>(TokenPrefabs);
                if (j - 1 >= 0 && j + 1 <= 7 && TokenArray[i, j - 1].name == TokenArray[i, j].name && TokenArray[i, j + 1].name == TokenArray[i, j].name)
                {
                    RemovingTokenPrefabs(tklist, TokenArray[i, j]);
                    if (i - 1 >= 0)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i - 1, j]);
                    }
                    if (i + 1 <= 7)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i + 1, j]);
                    }

                    int ran_token = UnityEngine.Random.Range(-1, 2);
                    int ran_replace_token = UnityEngine.Random.Range(0, tklist.Count);
                    Destroy(TokenArray[i, j + ran_token]);
                    TokenArray[i, j + ran_token] = Instantiate(tklist[ran_replace_token], positionBasedOnPivot(i , j + ran_token), Quaternion.identity);
                    Debug.Log("Swapped " + (i) + "," + (j + ran_token) + " from " + TokenArray[i, j].name + " to " + TokenArray[i , j + ran_token].name);
                }
            }
        }
    }

    private void RemovingTokenPrefabs(List<GameObject> tklist, GameObject Token)
    {
        for (int i = 0;i<tklist.Count ;i++)
        {
            if (tklist[i].GetComponent<TokenType>().color == Token.GetComponent<TokenType>().color)
            {
                tklist.RemoveAt(i);
                return;
            }
        }
    }

    void ClearCheckingArray()
    {
        for (int i = 0; i < CheckingArray.GetLength(0); i++)
        {
            for (int j = 0; j < CheckingArray.GetLength(1); j++)
            {
                CheckingArray[i,j] = false;
            }
        }
    }

    void FillWithTokens()
    {
        for (int i = 0; i < TokenArray.GetLength(0); i++)
        {
            for (int j = 0; j < TokenArray.GetLength(1); j++)
            {
                int random_num = UnityEngine.Random.Range(0, 5);
                TokenArray[i,j] = Instantiate(TokenPrefabs[random_num], positionBasedOnPivot(i,j), Quaternion.identity);
            }
        }
    }

    public Vector3 positionBasedOnPivot(int row, int col)
    {
        Vector3 TokenPOS = new Vector3(TopLeftPivot.position.x + col* CellSize, TopLeftPivot.position.y - row * CellSize, 0);

        return TokenPOS;
    }

    public int ColFromWorldPOS(float XCoord)
    {
        for (int i = 0; i < CheckingArray.GetLength(0); i++)
        {
            if ((i*CellSize + TopLeftPivot.position.x - CellSize / 2) <= XCoord && XCoord < (i*CellSize + TopLeftPivot.position.x + CellSize / 2))
            {
                return i;
            }
        }
        return -1;
    }

    public int RowFromWorldPOS(float YCoord)
    {
        for (int i = 0; i < CheckingArray.GetLength(1); i++)
        {
            if (( - i * CellSize + TopLeftPivot.position.y + CellSize / 2) > YCoord && YCoord >= (- i * CellSize + TopLeftPivot.position.y - CellSize / 2))
            {
                return i;
            }
        }
        return -1;
    }

    public GameObject getTokenAt(int row, int col)
    {
        return TokenArray[row,col];
    }

    public bool SwapToken(Vector2Int SelectedCoords, Vector2Int ClosestCoords, bool horiSwap)
    {

        //FindFirstObjectByType<InputManager>().SetAnimating(true);
        SwapInArray(SelectedCoords, ClosestCoords);

        //Check for valid swap, will return false if not
        if (CheckMatchesOnBoard())
        {
            //Do the destory

            //While loop until no more can be destroyed
            while (CheckMatchesOnBoard()){
                
            }
            
        }
        else
        {
            //Swap them back
            SwapInArray(SelectedCoords, ClosestCoords);
        }
        
        return true;
    }

    void SwapInArray(Vector2Int SelectedCoords, Vector2Int ClosestCoords)
    {
        //Swap in Token Array
        GameObject tempObj = TokenArray[SelectedCoords.y, SelectedCoords.x];
        TokenArray[SelectedCoords.y, SelectedCoords.x] = TokenArray[ClosestCoords.y, ClosestCoords.x];
        TokenArray[ClosestCoords.y, ClosestCoords.x] = tempObj;

        //Swap in Display Grid (snap in position)
        TokenArray[ClosestCoords.y, ClosestCoords.x].transform.position = positionBasedOnPivot(ClosestCoords.y, ClosestCoords.x);
        TokenArray[SelectedCoords.y, SelectedCoords.x].transform.position = positionBasedOnPivot(SelectedCoords.y, SelectedCoords.x);
    }

    bool CheckAbilityToMove()
    {
        return false;
    }

    bool CheckMatchesOnBoard()
    {
        //Column checks
        for (int i = 0; i < CheckingArray.GetLength(1); i++)
        {

        }

        //Row checks
        return false;
    }
}

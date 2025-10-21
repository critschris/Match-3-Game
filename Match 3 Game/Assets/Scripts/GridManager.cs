using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Collections;
using UnityEditor.Rendering;
using static UnityEngine.Rendering.DebugUI.Table;
using System.Linq;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public int scene = 1;

    public Transform TopLeftPivot;
    public float CellSize = 1f;

    //i is the row
    //j is the column
    public GameObject[,] TokenArray;

    //false is default
    public bool[,] CheckingArray;

    //Match List
    List<List<GameObject>> MatchList = new List<List<GameObject>>();

    //Array of token prefabs
    public GameObject[] TokenPrefabs;

    int points = 0;

    int points_buffer = 0;

    public TextMeshProUGUI Points_text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckingArray = new bool[8,8];

        TokenArray = new GameObject[8, 8];

        TopLeftPivot.position = new Vector3(-4*CellSize + CellSize/2, 4*CellSize - CellSize / 2, 0);

        //Clearing checking array
        ClearCheckingArray();

        //Fill board with random tokens
        FillWithTokens();

        //Swap out matching tokens on the board
        SwapOutMatches();

        //Check for ability to make a move

    }



    public void UpdatePoints()
    {
        points += points_buffer;
        Points_text.text = ""+points;
    }

    public void SwapOutMatches()
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

                    //Picking random token between the three
                    int ran_token = UnityEngine.Random.Range(-1, 2);

                    //Removing colors surrounding one of the three tokens
                    if (j-1>=0)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i + ran_token, j - 1]);
                    }
                    if (j+1<=7)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i + ran_token, j + 1]);
                    }
                    
                    int ran_replace_token = UnityEngine.Random.Range(0,tklist.Count);
                    Destroy(TokenArray[i + ran_token, j]);
                    TokenArray[i + ran_token, j] = Instantiate(tklist[ran_replace_token], positionBasedOnPivot(i + ran_token, j), Quaternion.identity);
                    Debug.Log("Swapped " + (i + ran_token) + "," + j + " from "+ TokenArray[i, j].name + " to " + TokenArray[i + ran_token, j].name);
                }

                tklist = new List<GameObject>(TokenPrefabs);
                if (j - 1 >= 0 && j + 1 <= 7 && TokenArray[i, j - 1].name == TokenArray[i, j].name && TokenArray[i, j + 1].name == TokenArray[i, j].name)
                {
                    RemovingTokenPrefabs(tklist, TokenArray[i, j]);

                    //Picking random token between the three
                    int ran_token = UnityEngine.Random.Range(-1, 2);

                    //Removing colors surrounding one of the three tokens
                    if (i - 1 >= 0)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i - 1, j + ran_token]);
                    }
                    if (i + 1 <= 7)
                    {
                        RemovingTokenPrefabs(tklist, TokenArray[i + 1, j + ran_token]);
                    }

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

    public void FillWithTokens()
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

    public void SwapToken(Vector2Int SelectedCoords, Vector2Int ClosestCoords, bool horiSwap)
    {
        
        //FindFirstObjectByType<InputManager>().SetAnimating(true);
        SwapInArray(SelectedCoords, ClosestCoords);

        StartCoroutine(SlowCheckmatch(SelectedCoords, ClosestCoords));
    }

    IEnumerator SlowCheckmatch(Vector2Int SelectedCoords, Vector2Int ClosestCoords)
    {
        yield return new WaitForSeconds(0.5F);
        //Check for valid swap, will return false if not
        bool match = CheckMatchesOnBoard();

        if (match)
        {
            FindFirstObjectByType<InputManager>().setWorkState(InputManager.GridWorkState.DestroyingMatches);

        }
        else
        {
            //PLay bad SFX
            FindFirstObjectByType<AudioManager>().Play("BadMove");
            //Swap them back
            SwapInArray(SelectedCoords, ClosestCoords);
            yield return new WaitForSeconds(0.5F);
            FindFirstObjectByType<InputManager>().SetAnimating(false);
        }
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

    public bool CheckMatchesOnBoard()
    {
        MatchList = new List<List<GameObject>>();
        bool matchfound = false;
        //Column checks
        for (int col = 0; col < TokenArray.GetLength(1); col++)
        {
            for (int row = 0; row < TokenArray.GetLength(0); row++)
            {
                if (row + 1 <= 6 && row + 2 <= 7 && TokenArray[row, col].name == TokenArray[row + 1, col].name && TokenArray[row + 1, col].name == TokenArray[row + 2, col].name)
                {
                    matchfound = true;
                    //Adding new matching list
                    MatchList.Add(new List<GameObject>());

                    //Add all three to the match list
                    for (int i = row; i <= 7; i++)
                    {
                        if (TokenArray[row, col].name == TokenArray[i, col].name)
                        {
                            AddingToNewMatchList(MatchList.Count - 1, i, col);
                        }
                        else
                        {
                            break;
                        }
                        //
                        row = i;
                    }
                    

                }
            }
        }

        //Row checks
        for (int row = 0; row < TokenArray.GetLength(0); row++)
        {
            for (int col = 0; col < TokenArray.GetLength(1); col++)
            {
                if (col + 1 <= 6 && col + 2 <= 7 && TokenArray[row, col].name == TokenArray[row, col + 1].name && TokenArray[row, col + 1].name == TokenArray[row, col + 2].name)
                {
                    matchfound = true;

                    //Adding new matching list
                    //MatchList.Add(new List<GameObject>());

                    List <GameObject> new_list = new List<GameObject>();

                    //Add all three to the match list
                    for (int i = col; i <= 7; i++)
                    {
                        if (TokenArray[row, col].name == TokenArray[row, i].name)
                        {
                            new_list.Add(TokenArray[row, i]);
                        }
                        else
                        {
                            AddToMatchListRow(new_list, row);
                            break;
                        }
                        //
                        col = i;
                    }
                }
            }
        }
        Debug.Log("Matchfound is "+matchfound);
        Debug.Log("MatchList is " + MatchList.Count);
        return matchfound;
    }

    void AddingToNewMatchList(int index, int row, int col)
    {
        MatchList[index].Add(TokenArray[row,col]);
        //Set CheckArray to true so no repeating
        CheckingArray[row, col] = true;
    }

    //Deciding to add to an existing list or a new list
    void AddToMatchListRow(List<GameObject> new_list, int row)
    {
        for (int i = 0; i < MatchList.Count; i++)
        {           
            if (MatchList[i][0].name == new_list[0].name && IntersectionCheck(i, new_list, row))
            {
                //Merge them in memory
                MatchList[i] = MatchList[i].Union(new_list).ToList();

                //end search
                return;
            }
        }

        MatchList.Add(new_list);
    }

   bool IntersectionCheck(int index, List<GameObject> new_list, int row)
    {
        for (int j = 0; j < MatchList[index].Count; j++)
        {
            GameObject chosenOBJ = MatchList[index][j];
            int yInGrid = RowFromWorldPOS(chosenOBJ.transform.position.y);
            if (yInGrid == row)
            {
                int xInGridOldList = RowFromWorldPOS(chosenOBJ.transform.position.x);
                for (int i = 0; i < new_list.Count; i++)
                {
                    int xInGridNewList = RowFromWorldPOS(TokenArray[row,i].transform.position.x);
                    if (xInGridOldList == xInGridNewList)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void DestroyTokensInList()
    {
        //Point system here
        //+50 for extra tokens
        foreach (List<GameObject> tokenmatch in MatchList)
        {
            points_buffer += 50*tokenmatch.Count;
            FindFirstObjectByType<AudioManager>().Play("DestroyToken");
            //Add point buffer here
            foreach (GameObject token in tokenmatch)
            {
                int xInGrid = ColFromWorldPOS(token.transform.position.x);
                int yInGrid = RowFromWorldPOS(token.transform.position.y);
                TokenArray[yInGrid,xInGrid] =null;
                Destroy(token);
            }
        }
        UpdatePoints();
        points_buffer = 0;
        
    }

    public void RefillEmptyCells()
    {
        //Bottom up filling
        for (int i = TokenArray.GetLength(0) - 1; i >= 0; i--)
        {
            for (int j = TokenArray.GetLength(1) - 1; j >= 0; j--)
            {
                if (TokenArray[i,j]==null)
                {
                    //Probability Logic
                    RandomToken(i, j);
                }
                
            }
        }
    }

    public int RandomToken(int row, int col)
    {
        //0 is blue
        //1 is green
        //2 is purple
        //3 is red
        //4 is yellow
        if (scene == 1)
        {

            if (row != 7 && row - 1 >= 0 && TokenArray[row-1,col] == null)
            {
                //First token has 40% chance of being the token below it
                int[] chances = { 15, 15, 15, 15, 15 };

                chances[TokenToTokenIndex(TokenArray[row + 1, col])] = 40;
                
                int Prefabindex = RandomTokenPrefabIndexWithProb(chances);

                TokenArray[row, col] = Instantiate(TokenPrefabs[Prefabindex], positionBasedOnPivot(row, col), Quaternion.identity);
                //For every subsequent token will be 60% to the the one underneath
                for (int i = row - 1; i >= 0 ; i--)
                {
                    if (TokenArray[i,col]==null)
                    {
                        int[] otherchances = { 10, 10, 10, 10, 10 };
                        chances[TokenToTokenIndex(TokenArray[i + 1, col])] = 60;

                        Prefabindex = RandomTokenPrefabIndexWithProb(chances);

                        TokenArray[i, col] = Instantiate(TokenPrefabs[Prefabindex], positionBasedOnPivot(i, col), Quaternion.identity);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                int[] evenchances = { 20, 20, 20, 20, 20 };

                int Prefabindex = RandomTokenPrefabIndexWithProb(evenchances);

                TokenArray[row, col] = Instantiate(TokenPrefabs[Prefabindex], positionBasedOnPivot(row, col), Quaternion.identity);
            }

        }
        else if (scene == 2)
        {
            int[] chances = { 1, 1, 1, 1, 1};

            if (row + 1 <= 7 && TokenArray[row + 1, col] != null)
            {
                chances[TokenToTokenIndex(TokenArray[row + 1, col])] += 1;
            }
            if (row + 1 <= 7 && col + 1 <= 7 && TokenArray[row + 1, col + 1]!= null)
            {
                chances[TokenToTokenIndex(TokenArray[row + 1, col + 1])] += 1;
            }
            if (col + 1 <= 7 && TokenArray[row, col + 1]!=null)
            {
                chances[TokenToTokenIndex(TokenArray[row, col + 1])] += 1;
            }
            if (row - 1 >= 0 && col + 1 <= 7 && TokenArray[row - 1, col + 1]!=null)
            {
                chances[TokenToTokenIndex(TokenArray[row - 1, col + 1])] += 1;
            }
            if (row - 1 >= 0 && TokenArray[row - 1, col]!=null)
            {
                chances[TokenToTokenIndex(TokenArray[row - 1, col])] += 1;
            }
            if (row - 1 >= 0 && col - 1 >= 0 && TokenArray[row - 1, col - 1]!=null)
            {
                chances[TokenToTokenIndex(TokenArray[row - 1, col - 1])] += 1;
            }
            if (col - 1 >= 0 && TokenArray[row, col - 1])
            {
                chances[TokenToTokenIndex(TokenArray[row, col - 1])] += 1;
            }
            if (col - 1 >= 0 && row + 1 <= 7 && TokenArray[row + 1, col - 1])
            {
                chances[TokenToTokenIndex(TokenArray[row + 1, col - 1])] += 1;
            }

            int Prefabindex = RandomTokenPrefabIndex(chances);

            TokenArray[row, col] = Instantiate(TokenPrefabs[Prefabindex], positionBasedOnPivot(row, col), Quaternion.identity);
        }

        return 0;
    }

    int RandomTokenPrefabIndexWithProb(int[] chances)
    {
        int Random_num = UnityEngine.Random.Range(0,100);
        int sum = 0;
        for (int i = 0; i < 5; i++)
        {
            if (Random_num >= sum && Random_num < sum + chances[i])
            {
                return i;
            }
            else
            {
                sum += chances[i];
            }
        }
        return -1;
    }

    int RandomTokenPrefabIndex(int[] chances)
    {
        int total = 0;
        for (int i = 0;i < chances.Length; i++)
        {
            total += chances[i];
        }
        int Random_num = UnityEngine.Random.Range(0, total);
        int sum = 0;
        for (int i = 0; i < 5; i++)
        {
            if (Random_num >= sum && Random_num < sum + chances[i])
            {
                return i;
            }
            else
            {
                sum += chances[i];
            }
        }
        return -1;
    }

    int TokenToTokenIndex(GameObject Token)
    {
        for (int i =0; i < TokenPrefabs.Length; i++)
        {
            if (Token.GetComponent<TokenType>().color == TokenPrefabs[i].GetComponent<TokenType>().color)
            {
                return i;
            }
        }
        return -1;
    }

    public bool PossibleMoveChecker()
    {
        for (int i = 0; i < TokenArray.GetLength(0); i++)
        {
            for (int j = 0; j < TokenArray.GetLength(1); j++)
            {
                //Column checks
                if (i + 1 < 8 && TokenArray[i,j].name == TokenArray[i+1,j].name)
                {
                    if (i + 3 < 8 && TokenArray[i,j].name == TokenArray[i+3,j].name)
                    {
                        return true;
                    }
                    else if (i + 2 < 8 && j + 1 < 8 && TokenArray[i, j].name == TokenArray[i + 2, j + 1].name)
                    {
                        return true;
                    }
                    else if (i + 2 < 8 && j - 1 >= 0 && TokenArray[i, j].name == TokenArray[i + 2, j - 1].name)
                    {
                        return true;
                    }
                    else if (i - 2 >= 0 && TokenArray[i, j].name == TokenArray[i - 2, j].name)
                    {
                        return true;
                    }
                    else if (i - 1 >= 0 && j - 1 >= 0 && TokenArray[i, j].name == TokenArray[i - 1, j - 1].name)
                    {
                        return true;
                    }
                    else if (i - 1 >= 0 && j + 1 < 8 && TokenArray[i, j].name == TokenArray[i - 1, j + 1].name)
                    {
                        return true;
                    }
                }

                //Row Checks
                if (j + 1 < 8 && TokenArray[i, j].name == TokenArray[i, j + 1].name)
                {
                    if (j + 3 < 8 && TokenArray[i, j].name == TokenArray[i, j + 3].name)
                    {
                        return true;
                    }
                    else if (j + 2 < 8 && i - 1 >= 0 && TokenArray[i, j].name == TokenArray[i - 1, j + 2].name)
                    {
                        return true;
                    }
                    else if (j + 2 < 8 && i + 1 < 8 && TokenArray[i, j].name == TokenArray[i + 1, j + 2].name)
                    {
                        return true;
                    }
                    else if (j - 2 >= 0 && TokenArray[i, j].name == TokenArray[i, j - 2].name)
                    {
                        return true;
                    }
                    else if (j - 1 >= 0 && i - 1 >= 0 && TokenArray[i, j].name == TokenArray[i - 1, j - 1].name)
                    {
                        return true;
                    }
                    else if (j - 1 >= 0 && i + 1 < 8 && TokenArray[i, j].name == TokenArray[i + 1, j - 1].name)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}

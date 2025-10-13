using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public Transform TopLeftPivot;
    public float CellSize = 1f;

    //i is the row
    //j is the column
    public GameObject[,] TokenArray;
    public bool[,] CheckingArray;

    //Match List
    List<List<GameObject>> MatchList;

    //Array of token prefabs
    public GameObject[] TokenPrefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CheckingArray = new bool[8,8];

        TokenArray = new GameObject[8, 8];

        TopLeftPivot.position = new Vector3(-4*CellSize + CellSize/2, 4*CellSize - CellSize / 2, 0);

        clearCheckingArray();

        fillWithTokens();

        Destroy(TokenArray[7, 5]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void clearCheckingArray()
    {
        for (int i = 0; i < CheckingArray.GetLength(0); i++)
        {
            for (int j = 0; j < CheckingArray.GetLength(1); j++)
            {
                CheckingArray[i,j] = false;
            }
        }
    }

    void fillWithTokens()
    {
        for (int i = 0; i < TokenArray.GetLength(0); i++)
        {
            for (int j = 0; j < TokenArray.GetLength(1); j++)
            {
                int random_num = Random.Range(0, 5);
                TokenArray[i,j] = Instantiate(TokenPrefabs[random_num], positionBasedOnPivot(i,j), Quaternion.identity);
            }
        }
    }

    Vector3 positionBasedOnPivot(int row, int col)
    {

        Vector3 TokenPOS = new Vector3(TopLeftPivot.position.x + col* CellSize, TopLeftPivot.position.y - row * CellSize, 0);

        return TokenPOS;
    }
}

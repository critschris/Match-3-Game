using UnityEngine;
using TMPro;
using System;

public class GameEndStats : MonoBehaviour
{
    
    public int TimeLeft = 0;

    public int Score = 0;

    public int Tokens_Left;

    public bool stars = false;

    public int starCount = 0;

    public GameObject[] stars_GO;

    public GameObject Stars;
    public void SetVariables(bool stars, int score)
    {

        if (stars)
        {
            if (score > 1500)
            {
                stars_GO[0].SetActive(true);
            }
            if (score > 3000)
            {
                stars_GO[1].SetActive(true);
            }
            if (score > 5000)
            {
                stars_GO[2].SetActive(true);
            } 
        }
    }

}

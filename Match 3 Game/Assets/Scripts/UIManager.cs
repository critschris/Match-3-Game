using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI Timer;

    public TextMeshProUGUI Moves;

    public GameObject NoMoreMovesPanel;

    public GameObject WINEndscreen;

    public GameObject LOSEEndscreen;

    public TextMeshProUGUI Points_text;

    public TextMeshProUGUI[] Token_point_text;

    public GameObject GameUI;

    public void RunStatsOnWin(int score)
    {
        WINEndscreen.GetComponentInChildren<GameEndStats>().SetVariables(true, score);
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class InputManager : MonoBehaviour
{
    Camera main_camera;

    GridManager gridman;

    Vector2Int SelectedCoords;
    public GameObject SelectedToken;
    Vector2Int ClosestCoords;  

    public bool horiSwap = false;

    GameObject[] SurroundingTokenArr;
    Vector3[] SurroundingTokenPositions;
    public int ClosestTokenIndex;

    bool animating = false;

    bool checking_lock = false;

    public GridWorkState gridWorkState = GridWorkState.None;

    public GameObject NoMoreMovesPanel;

    public int ComboCounter = 0;

    public enum GridWorkState {None, CheckingForMatches, DestroyingMatches, RefillingMatches, MovingAbilityCheck}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        main_camera = Camera.main;
        gridman = FindFirstObjectByType<GridManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!checking_lock && gridWorkState == GridWorkState.MovingAbilityCheck)
        {
            StartCoroutine(AbilityToMoveCheck());
        }
        if (!checking_lock && gridWorkState == GridWorkState.CheckingForMatches)
        {
            StartCoroutine(ChecknewTokens());
            return;
        }
        if (!checking_lock && gridWorkState == GridWorkState.RefillingMatches)
        {
            StartCoroutine(RefillTokens());
            return;
        }
        if (!checking_lock && gridWorkState == GridWorkState.DestroyingMatches)
        {
            StartCoroutine(DestroyTokens());
            return;
        }
        if (!animating)
        {
            MouseChecker();
        }
        if (!checking_lock && gridWorkState == GridWorkState.None)
        {
            
        }
    }

    IEnumerator AbilityToMoveCheck()
    {
        checking_lock = true;
        if (gridman.PossibleMoveChecker())
        {
            gridWorkState = GridWorkState.None;
            animating=false;
        }
        else
        {
            //Display no more moves screen
            NoMoreMovesPanel.SetActive(true);

            //Animation
            NoMoreMovesPanel.GetComponent<Animator>().SetBool("GoAway", false);
            NoMoreMovesPanel.GetComponent<Animator>().SetBool("TurnBlack", true);

            yield return new WaitForSeconds(0.25F);
            //Reshuffle board
            gridman.FillWithTokens();

            gridman.SwapOutMatches();

            yield return new WaitForSeconds(1F);
            //Turn off no more moves screen

            //Animation
            NoMoreMovesPanel.GetComponent<Animator>().SetBool("TurnBlack", false);
            NoMoreMovesPanel.GetComponent<Animator>().SetBool("GoAway", true);
            yield return new WaitForSeconds(0.25F);
            NoMoreMovesPanel.SetActive(false);

            gridWorkState = GridWorkState.MovingAbilityCheck;
        }

        checking_lock = false;
    }

    IEnumerator ChecknewTokens()
    {
        checking_lock = true;

        bool match = gridman.CheckMatchesOnBoard();
        yield return new WaitForSeconds(1F);
        if (match)
        {
            gridWorkState = GridWorkState.DestroyingMatches;
        }
        else
        {
            gridWorkState = GridWorkState.MovingAbilityCheck;
        }

        checking_lock = false;
    }

    IEnumerator RefillTokens()
    {
        checking_lock = true;

        gridman.RefillEmptyCells();
        yield return new WaitForSeconds(1F);
        gridWorkState = GridWorkState.CheckingForMatches;

        checking_lock = false;
    }

    IEnumerator DestroyTokens()
    {
        checking_lock = true;

        gridman.DestroyTokensInList();
        yield return new WaitForSeconds(1F);
        gridWorkState = GridWorkState.RefillingMatches;

        checking_lock = false;
    }

    public void setWorkState(GridWorkState state)
    {
        gridWorkState = state;
    }

    void MouseChecker()
    {
        if (Input.GetMouseButtonDown(0))
        {      
            Vector3 ScreenPos = Input.mousePosition;
            Vector3 WorldPos = main_camera.ScreenToWorldPoint(ScreenPos);
            int xInGrid = gridman.ColFromWorldPOS(WorldPos.x);
            int yInGrid = gridman.RowFromWorldPOS(WorldPos.y);
            if (xInGrid != -1 && yInGrid != -1)
            {
                SelectedCoords = new Vector2Int(xInGrid, yInGrid);
                SelectedToken = gridman.getTokenAt(yInGrid, xInGrid);
            }
            else
            {
                SelectedToken = null;
            }
            CalculateSurroundingTokens();
        }
        if (Input.GetMouseButton(0)&& SelectedToken != null)
        {
            Vector3 ScreenPos = Input.mousePosition;
            Vector3 MouseWorldPos = main_camera.ScreenToWorldPoint(ScreenPos);
            MouseWorldPos.z = 0;
            Vector3 TokenPos = gridman.positionBasedOnPivot(SelectedCoords.y, SelectedCoords.x);
            if (Vector3.Distance(MouseWorldPos, TokenPos) <= 1.25f)
            {
                SelectedToken.transform.position = MouseWorldPos;
            }
            else
            {
                SelectedToken.transform.position = TokenPos + Vector3.ClampMagnitude(MouseWorldPos-TokenPos, 1.25F);
            }

            FindingClosestToken();

            PutBackOldToken();

            if (horiSwap)
            {
                SurroundingTokenArr[ClosestTokenIndex].transform.position = new Vector3(SurroundingTokenPositions[ClosestTokenIndex].x + (TokenPos.x - SelectedToken.transform.position.x), SurroundingTokenPositions[ClosestTokenIndex].y, 0);
            }
            else
            {
                SurroundingTokenArr[ClosestTokenIndex].transform.position = new Vector3(SurroundingTokenPositions[ClosestTokenIndex].x, SurroundingTokenPositions[ClosestTokenIndex].y + (TokenPos.y - SelectedToken.transform.position.y), 0);
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            animating = true;

            //Do the swap (snap into position)
            gridman.SwapToken(SelectedCoords, ClosestCoords, horiSwap);

            //Reset selected variables
            SelectedToken = null;
        }
    }

    void CalculateSurroundingTokens()
    {
        SurroundingTokenArr = new GameObject[4];
        SurroundingTokenPositions = new Vector3[4];
        if (SelectedCoords.y + 1 < 8)
        {
            SurroundingTokenArr[0] = gridman.getTokenAt(SelectedCoords.y + 1, SelectedCoords.x);
            SurroundingTokenPositions[0] = gridman.positionBasedOnPivot(SelectedCoords.y + 1, SelectedCoords.x);
        }
        //Above
        if (SelectedCoords.y - 1 > -1)
        {
            SurroundingTokenArr[1] = gridman.getTokenAt(SelectedCoords.y - 1, SelectedCoords.x);
            SurroundingTokenPositions[1] = gridman.positionBasedOnPivot(SelectedCoords.y - 1, SelectedCoords.x);
        }
        //Left
        if (SelectedCoords.x - 1 > -1)
        {
            SurroundingTokenArr[2] = gridman.getTokenAt(SelectedCoords.y, SelectedCoords.x - 1);
            SurroundingTokenPositions[2] = gridman.positionBasedOnPivot(SelectedCoords.y, SelectedCoords.x - 1);
        }
        //Right
        if (SelectedCoords.x + 1 < 8)
        {
            SurroundingTokenArr[3] = gridman.getTokenAt(SelectedCoords.y, SelectedCoords.x + 1);
            SurroundingTokenPositions[3] = gridman.positionBasedOnPivot(SelectedCoords.y, SelectedCoords.x + 1);
        }
    }

    void FindingClosestToken()
    {
        float min = -1;
        for (int i = 0; i < SurroundingTokenPositions.Length; i++)
        {
            if (SurroundingTokenPositions[i] != null && (min == -1))
            {
                min = Vector3.Distance(SurroundingTokenPositions[i], SelectedToken.transform.position);
                ClosestTokenIndex = i;
                horiSwap = false;
                continue;
            }
            if (SurroundingTokenPositions[i] != null && (Vector3.Distance(SurroundingTokenPositions[i], SelectedToken.transform.position) < min))
            {
                min = Vector3.Distance(SurroundingTokenPositions[i], SelectedToken.transform.position);
                ClosestTokenIndex = i;
                if (i>=2)
                {
                    horiSwap = true;
                }
                else
                {
                    horiSwap = false;
                }
            }
        }

        if (ClosestTokenIndex == 0)
        {
            ClosestCoords = new Vector2Int (SelectedCoords.x, SelectedCoords.y + 1);
        }
        else if (ClosestTokenIndex == 1)
        {
            ClosestCoords = new Vector2Int(SelectedCoords.x, SelectedCoords.y - 1);
        }
        else if (ClosestTokenIndex == 2)
        {
            ClosestCoords = new Vector2Int(SelectedCoords.x - 1, SelectedCoords.y);
        }
        else if (ClosestTokenIndex == 3)
        {
            ClosestCoords = new Vector2Int(SelectedCoords.x + 1, SelectedCoords.y);
        }
    }

    void PutBackOldToken()
    {
        for (int i = 0; i < SurroundingTokenArr.Length; i++) {
            if (i!=ClosestTokenIndex && SurroundingTokenArr[i]!=null)
            {
                SurroundingTokenArr[i].transform.position = SurroundingTokenPositions[i];
            }
        }
    }

    public void SetAnimating(bool value)
    {
        animating = value;
    }
}

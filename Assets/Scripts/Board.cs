using System.Collections;
using UnityEngine;
public enum GameState
{
    wait,
    move
}
public class Board : MonoBehaviour
{
    public GameState CurrentState = GameState.move;

    public int Width;
    public int Height;
    public int OffSet;
    public GameObject TilePrefab;
    public GameObject[] Dots;
    public GameObject[,] AllDots;
    public GameObject destroyEffect;
    public Dot CurrentDot;

    private FindMatches findMatches;
    private BackgroundTile[,] allTiles;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[Width, Height];
        AllDots = new GameObject[Width, Height];
        SetUp();
    }
    private void SetUp()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Vector2 TempPosition = new(i, j + OffSet);
                GameObject BackgroundTile = Instantiate(TilePrefab, TempPosition, Quaternion.identity) as GameObject;
                BackgroundTile.transform.parent = this.transform;
                BackgroundTile.name = "( " + i + ", " + j + " )";

                int DotToUse = Random.Range(0, Dots.Length);
                int MaxIterations = 0;
                while (MatchesAt(i, j, Dots[DotToUse]) && MaxIterations < 100)
                {
                    DotToUse = Random.Range(0, Dots.Length);
                    MaxIterations++;
                    Debug.Log(MaxIterations);
                }
                MaxIterations = 0;

                GameObject dot = Instantiate(Dots[DotToUse], TempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().Row = j;
                dot.GetComponent<Dot>().Column = i;

                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                AllDots[i, j] = dot;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (AllDots[column, row - 1].tag == piece.tag && AllDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (AllDots[column - 1, row].tag == piece.tag && AllDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (AllDots[column, row].GetComponent<Dot>().IsMatched)
        {
            //How many elements are in the matched pieces list from findmatches?
            if (findMatches.CurrentMatches.Count == 4 || findMatches.CurrentMatches.Count ==7)
            {
                findMatches.CheckBombs();
            }
            findMatches.CurrentMatches.Remove(AllDots[column, row]);
            GameObject particle = Instantiate(destroyEffect, AllDots[column, row].transform.position, Quaternion.identity);
            Destroy(particle, .4f);
            Destroy(AllDots[column, row]);
            AllDots[column, row] = null;
        }
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        int _nullCount = 0;
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (AllDots[i, j] == null)
                {
                    _nullCount++;
                }
                else if (_nullCount > 0)
                {
                    AllDots[i, j].GetComponent<Dot>().Row -= _nullCount;
                    AllDots[i, j] = null;
                }
            }
            _nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (AllDots[i, j] == null)
                {
                    Vector2 TempPosition = new Vector2(i, j + OffSet);
                    int DotToUse = Random.Range(0, Dots.Length);
                    GameObject Piece = Instantiate(Dots[DotToUse], TempPosition, Quaternion.identity);
                    AllDots[i, j] = Piece;
                    Piece.GetComponent<Dot>().Row = j;
                    Piece.GetComponent<Dot>().Column = i;
                }
            }
        }
    }
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                if (AllDots[i, j] != null)
                {
                    if (AllDots[i, j].GetComponent<Dot>().IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.CurrentMatches.Clear();
        CurrentDot = null;
        yield return new WaitForSeconds(.5f);
        CurrentState = GameState.move;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FindMatches : MonoBehaviour
{
    public List<GameObject> CurrentMatches = new List<GameObject>();

    private Board board;

    void Start()
    {
        board = FindObjectOfType<Board>();
    }
    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCoroutine());
    }
    private IEnumerator FindAllMatchesCoroutine()
    {
        yield return new WaitForSeconds(.1f);
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                GameObject CurrentDot = board.AllDots[i, j];
                if (CurrentDot != null)
                {
                    if (i > 0 && i < board.Width - 1)
                    {
                        GameObject LeftDot = board.AllDots[i - 1, j];
                        GameObject RightDot = board.AllDots[i + 1, j];
                        if (LeftDot != null && RightDot != null)
                        {
                            if (LeftDot.tag == CurrentDot.tag && RightDot.tag == CurrentDot.tag)
                            {
                                // Bomb
                                if (CurrentDot.GetComponent<Dot>().IsRowBomb ||
                                    LeftDot.GetComponent<Dot>().IsRowBomb ||
                                    RightDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(j));
                                }
                                if (CurrentDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i));
                                }
                                if (LeftDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i - 1));
                                }
                                if (RightDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i + 1));
                                }
                                // End Bomb


                                if (!CurrentMatches.Contains(LeftDot))
                                {
                                    CurrentMatches.Add(LeftDot);
                                }
                                LeftDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(RightDot))
                                {
                                    CurrentMatches.Add(RightDot);
                                }
                                CurrentDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(CurrentDot))
                                {
                                    CurrentMatches.Add(CurrentDot);
                                }
                                CurrentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }
                    if (j > 0 && j < board.Height - 1)
                    {
                        GameObject UpDot = board.AllDots[i, j + 1];
                        GameObject DownDot = board.AllDots[i, j - 1];
                        if (UpDot != null && DownDot != null)
                        {
                            if (UpDot.tag == CurrentDot.tag && DownDot.tag == CurrentDot.tag)
                            {
                                // Bomb
                                if (CurrentDot.GetComponent<Dot>().IsColumnBomb ||
                                   UpDot.GetComponent<Dot>().IsColumnBomb ||
                                   DownDot.GetComponent<Dot>().IsColumnBomb)
                                {
                                    CurrentMatches.Union(GetColumnPieces(i));
                                }
                                if (CurrentDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(j));
                                }
                                if (UpDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(j - 1));
                                }
                                if (DownDot.GetComponent<Dot>().IsRowBomb)
                                {
                                    CurrentMatches.Union(GetRowPieces(j + 1));
                                }
                                // End Bomb
                                if (!CurrentMatches.Contains(UpDot))
                                {
                                    CurrentMatches.Add(UpDot);
                                }
                                UpDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(DownDot))
                                {
                                    CurrentMatches.Add(DownDot);
                                }
                                DownDot.GetComponent<Dot>().IsMatched = true;
                                if (!CurrentMatches.Contains(CurrentDot))
                                {
                                    CurrentMatches.Add(CurrentDot);
                                }
                                CurrentDot.GetComponent<Dot>().IsMatched = true;
                            }
                        }
                    }

                }
            }
        }
    }

    public void MatchePiesecOfColor(string Color)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                //Check piece exist
                if (board.AllDots[i,j] !=null)
                {
                    // Check TAG            
                    if (board.AllDots[i,j].tag == Color)
                    {
                        board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
    }
    public void MatchPiecesOfColor (string color)
    {
        for (int i = 0; i < board.Width; i++)
        {
            for (int j = 0; j < board.Height; j++)
            {
                // check if that piece exists
                if (board.AllDots[i,j] !=null)
                {
                    // check tag dot
                    if (board.AllDots[i,j].tag == color)
                    {
                        // set that dot to be match
                        board.AllDots[i, j].GetComponent<Dot>().IsMatched = true;
                    }
                }
            }
        }
    }

    List<GameObject> GetColumnPieces(int column)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.Height; i++)
        {
            if (board.AllDots[column, i] != null)
            {
                dots.Add(board.AllDots[column, i]);
                board.AllDots[column, i].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }
    List<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.Width; i++)
        {
            if (board.AllDots[i, row] != null)
            {
                dots.Add(board.AllDots[i, row]);
                board.AllDots[i, row].GetComponent<Dot>().IsMatched = true;
            }
        }
        return dots;
    }
    public void CheckBombs()
    {
        if (board.CurrentDot != null)
        {
            if (board.CurrentDot.IsMatched)
            {
                board.CurrentDot.IsMatched = false;
                /*int TypeOfBomb = Random.Range(0, 100);
                if (TypeOfBomb < 50)
                {
                    board.CurrentDot.MakeRowBomb();
                }
                else if (TypeOfBomb >= 50)
                {
                    board.CurrentDot.MakeColumnBomb();
                }
                */
                if ((board.CurrentDot.SwipeAngle > -45 && board.CurrentDot.SwipeAngle <= 45)
                  || board.CurrentDot.SwipeAngle < -135 || board.CurrentDot.SwipeAngle >= 135)
                {
                    board.CurrentDot.MakeRowBomb();
                }
                else
                {
                    board.CurrentDot.MakeColumnBomb();
                }
            }
            //Is the other piece matched ?
            else if (board.CurrentDot.OtherDot != null)
            {
                Dot otherDot = board.CurrentDot.OtherDot.GetComponent<Dot>();
                if (otherDot.IsMatched)
                {
                    otherDot.IsMatched = false;
                    /*
                    //Dicede what kind if bomb to make
                    int TypeOfBomb = Random.Range(0, 100);
                    if (TypeOfBomb < 50)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else if (TypeOfBomb >= 50)
                    {
                        otherDot.MakeColumnBomb();
                    }
                    */
                    if ((board.CurrentDot.SwipeAngle > -45 && board.CurrentDot.SwipeAngle <= 45)
                      || board.CurrentDot.SwipeAngle < -135 || board.CurrentDot.SwipeAngle >= 135)
                    {
                        otherDot.MakeRowBomb();
                    }
                    else
                    {
                        otherDot.MakeColumnBomb();
                    }
                }
            }
        }
    }
}
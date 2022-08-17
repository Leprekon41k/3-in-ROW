using System.Collections;
using System.Collections.Generic;
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
        StartCoroutine(FindAllMatchesCo());
    }
    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
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
    void FixedUpdate()
    {

    }
}

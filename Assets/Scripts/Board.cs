using UnityEngine;
public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;
    void Start()
    {
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height];
        SetUp();
    }
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 TempPosition = new(i, j);
                GameObject BackgroundTile = Instantiate(tilePrefab, TempPosition, Quaternion.identity) as GameObject;
                BackgroundTile.transform.parent = this.transform;
                BackgroundTile.name = "( " + i + ", " + j + " )";

                int DotToUse = Random.Range(0, dots.Length);
                int MaxIterations = 0;
                while (MatchesAt(i, j, dots[DotToUse]) && MaxIterations < 100)
                {
                    DotToUse = Random.Range(0, dots.Length);
                    MaxIterations++;
                    Debug.Log(MaxIterations);
                }
                MaxIterations = 0;

                GameObject dot = Instantiate(dots[DotToUse], TempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
}

using System.Collections;
using UnityEngine;
public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int Column;
    public int Row;
    public int PreviousColumn;
    public int PreviousRow;
    public int TargetX;
    public int TargetY;
    public bool IsMatched = false;
    public GameObject OtherDot;


    private FindMatches findMatches;
    private Vector2 tempPosition;
    private Board board;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    [Header("Swipe Stuff")]
    public float SwipeAngle = 0;
    public float SwipeResist = 1f;

    [Header("PowerUp Stuff")]
    public bool IsColorBomb;
    public bool IsColumnBomb;
    public bool IsRowBomb;
    public GameObject RowArrow;
    public GameObject ColumnArrow;
    public GameObject ColorBomb;


    void Start()
    {
        IsColumnBomb = false;
        IsRowBomb = false;
        IsColorBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        //TargetX = (int)transform.position.x;
        //TargetY = (int)transform.position.y;
        //Row = TargetY;
        //Column = TargetX;

        //PreviousRow = Row;
        //PreviousColumn = Column;

    }
    //For testing and debug   
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            IsColorBomb = true;
            GameObject Color = Instantiate(ColorBomb, transform.position, Quaternion.identity);
            Color.transform.parent = this.transform;            
        }
    }
    //
    void FixedUpdate()
    {
        FindMaches();

        if (IsMatched)
        {
            SpriteRenderer MySprite = GetComponent<SpriteRenderer>();
            Color CurentColor = MySprite.color;
            MySprite.color = new Color(CurentColor.r, CurentColor.g, CurentColor.b, 0.6f);
        }
        TargetX = Column;
        TargetY = Row;
        if (Mathf.Abs(TargetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.AllDots[Column, Row] != this.gameObject)
            {
                board.AllDots[Column, Row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(TargetX, transform.position.y);
            transform.position = tempPosition;
            board.AllDots[Column, Row] = this.gameObject;
        }
        if (Mathf.Abs(TargetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .6f);
            if (board.AllDots[Column, Row] != this.gameObject)
            {
                board.AllDots[Column, Row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = tempPosition;
        }
    }
    public IEnumerator CheckMoveCo()
    {
        if (IsColorBomb)
        {
            findMatches.MatchePiesecOfColor(OtherDot.tag);
            IsMatched = true;
        }
        else if (OtherDot.GetComponent<Dot>().IsColorBomb)
        {
            findMatches.MatchePiesecOfColor(this.gameObject.tag);
            IsMatched = true;
        }
        yield return new WaitForSeconds(.4f);
        if (OtherDot != null)
        {
            if (!IsMatched && !OtherDot.GetComponent<Dot>().IsMatched)
            {
                OtherDot.GetComponent<Dot>().Row = Row;
                OtherDot.GetComponent<Dot>().Column = Column;
                Row = PreviousRow;
                Column = PreviousColumn;
                yield return new WaitForSeconds(.5f);
                board.CurrentDot = null;
                board.CurrentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
        //otherDot = null;
    }
    private void OnMouseDown()
    {
        if (board.CurrentState == GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if (board.CurrentState == GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }
    public void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > SwipeResist ||
            Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > SwipeResist)
        {
            SwipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, 
                                     finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.CurrentState = GameState.wait;
            board.CurrentDot = this;
        }
        else
        {
            board.CurrentState = GameState.move;
        }
    }
    public void MovePieces()
    {
        if (SwipeAngle > -45 && SwipeAngle <= 45 && Column < board.Width - 1) // Right swipe 
        {
            OtherDot = board.AllDots[Column + 1, Row];
            PreviousRow = Row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Column -= 1;
            Column += 1;
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135 && Row < board.Height - 1) // Up swipe 
        {
            OtherDot = board.AllDots[Column, Row + 1];
            PreviousRow = Row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Row -= 1;
            Row += 1;
        }
        else if ((SwipeAngle > 135 && SwipeAngle <= 180 || SwipeAngle <= -135 && SwipeAngle >= -180) && Column > 0) // Left swipe 
        {
            OtherDot = board.AllDots[Column - 1, Row];
            PreviousRow = Row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Column += 1;
            Column -= 1;
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && Row > 0) // Down swipe 
        {
            OtherDot = board.AllDots[Column, Row - 1];
            PreviousRow = Row;
            PreviousColumn = Column;
            OtherDot.GetComponent<Dot>().Row += 1;
            Row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }
    public void FindMaches()
    {
        if (Column > 0 && Column < board.Width - 1)
        {
            GameObject LeftDot1 = board.AllDots[Column - 1, Row];
            GameObject RightDot1 = board.AllDots[Column + 1, Row];
            if (LeftDot1 != null && RightDot1 != null)
            {
                if (LeftDot1.tag == this.gameObject.tag && RightDot1.tag == this.gameObject.tag)
                {
                    LeftDot1.GetComponent<Dot>().IsMatched = true;
                    RightDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }
        }
        if (Row > 0 && Row < board.Height - 1)
        {
            GameObject UpDot1 = board.AllDots[Column, Row + 1];
            GameObject DownDot1 = board.AllDots[Column, Row - 1];
            if (UpDot1 != null && DownDot1 != null)
            {
                if (UpDot1.tag == this.gameObject.tag && DownDot1.tag == this.gameObject.tag)
                {
                    UpDot1.GetComponent<Dot>().IsMatched = true;
                    DownDot1.GetComponent<Dot>().IsMatched = true;
                    IsMatched = true;
                }
            }
        }
    }
    public void MakeRowBomb()
    {
        IsRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColumnBomb()
    {
        IsColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}

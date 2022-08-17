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
    public float SwipeAngle = 0;
    public bool IsMatched = false;
    public float SwipeResist = 1f;

    private Vector2 tempPosition;
    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    void Start()
    {
        board = FindObjectOfType<Board>();
        //TargetX = (int)transform.position.x;
        //TargetY = (int)transform.position.y;
        //Row = TargetY;
        //Column = TargetX;

        //PreviousRow = Row;
        //PreviousColumn = Column;
    }
    void FixedUpdate()
    {
        FindMaches();

        if (IsMatched)
        {
            SpriteRenderer MySprite = GetComponent<SpriteRenderer>();
            Color CurentColor = MySprite.color;
            MySprite.color = new Color(CurentColor.r, CurentColor.g, CurentColor.a, CurentColor.b);
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

        }
        else
        {
            tempPosition = new Vector2(transform.position.x, TargetY);
            transform.position = tempPosition;
        }
    }
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if (otherDot != null)
        {
            if (!IsMatched && !otherDot.GetComponent<Dot>().IsMatched)
            {
                otherDot.GetComponent<Dot>().Row = Row;
                otherDot.GetComponent<Dot>().Column = Column;
                Row = PreviousRow;
                Column = PreviousColumn;
                yield return new WaitForSeconds(.5f);
                board.CurrentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
        }
        otherDot = null;
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
            SwipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.CurrentState = GameState.wait;
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
            otherDot = board.AllDots[Column + 1, Row];
            PreviousRow = Row;
            PreviousColumn = Column;
            otherDot.GetComponent<Dot>().Column -= 1;
            Column += 1;
        }
        else if (SwipeAngle > 45 && SwipeAngle <= 135 && Row < board.Height - 1) // Up swipe 
        {
            otherDot = board.AllDots[Column, Row + 1];
            PreviousRow = Row;
            PreviousColumn = Column;
            otherDot.GetComponent<Dot>().Row -= 1;
            Row += 1;
        }
        else if ((SwipeAngle > 135 && SwipeAngle <= 180 || SwipeAngle <= -135 && SwipeAngle >= -180) && Column > 0) // Left swipe 
        {
            otherDot = board.AllDots[Column - 1, Row];
            PreviousRow = Row;
            PreviousColumn = Column;
            otherDot.GetComponent<Dot>().Column += 1;
            Column -= 1;
        }
        else if (SwipeAngle < -45 && SwipeAngle >= -135 && Row > 0) // Down swipe 
        {
            otherDot = board.AllDots[Column, Row - 1];
            PreviousRow = Row;
            PreviousColumn = Column;
            otherDot.GetComponent<Dot>().Row += 1;
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square : MonoBehaviour
{
    [SerializeField] private Color lightSquares, darkSquares;
    [SerializeField] private SpriteRenderer myRen;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject PossibleMoves;
    [SerializeField] private int rank, file;
    [SerializeField] private bool IsEmpty;
    [SerializeField] private Piece CurrentPiece = null;
    [SerializeField] private bool IsSelected = false;
    [SerializeField] private BoxCollider2D collider;
    private Vector3 MyPosition;
    // old blue color 2B37A6


    void Update()
    {
        if (CurrentPiece != null)
        {
            IsEmpty = false;
        }
        else
        {
            IsEmpty = true;
        }


        if (IsEmpty)
        {
            collider.size = new Vector2(0.9f, 0.9f);
        }
        else
        {
            collider.size = new Vector2(0.5f, 0.5f);
        }
    }

    public void Init(bool isLightColor, Vector3 v)
    {
        myRen.color = isLightColor ? lightSquares : darkSquares;
        MyPosition = v;
    }

    public void HighlightSquare()
    {
        _highlight.SetActive(true);
    }

    public void DontHighlightSquare()
    {
        _highlight.SetActive(false);
    }

    public void ShowMoves()
    {
        PossibleMoves.SetActive(true);
    }

    public void DontShowMoves()
    {
        PossibleMoves.SetActive(false);
    }

    public void SetRank(int num)
    {
        rank = num;
    }

    public void SetFile(int num)
    {
        file = num;
    }

    public int GetRank()
    {
        return rank;
    }

    public int GetFile()
    {
        return file;
    }

    public void SetIsEmpty(bool val)
    {
        IsEmpty = val;
    }

    public bool GetIsEmpty()
    {
        return IsEmpty;
    }

    public void SetPiece(Piece p)
    {
        CurrentPiece = p; ;
    }

    public Piece GetPiece()
    {
        return CurrentPiece;
    }

    public bool GetIsSelected()
    {
        return IsSelected;
    }

    public Vector3 GetPosition()
    {
        return MyPosition;
    }

    void OnMouseEnter()
    {
        IsSelected = true;
    }

    void OnMouseExit()
    {
        IsSelected = false;
    }

    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [SerializeField] private Color whitePiecesColor, blackPiecesColor;
    [SerializeField] private SpriteRenderer myRen;
    [SerializeField] private Transform thisPiece;
    private static float defaultSize = 1.3f, enlargedSize = 1.7f, SmallSize = 1f;
    [SerializeField] private Square currentSquare;
    public bool Moved = false;
    [SerializeField] private bool IsSelected = false;
    private static int WhiteCaptureCount;
    private static int BlackCaptureCount;
    public List<Square> possibleMoves;
    [SerializeField] public int value;

    public void Init(bool isLightColor, int val)
    {
        myRen.color = isLightColor ? whitePiecesColor : blackPiecesColor;
        WhiteCaptureCount = 0;
        BlackCaptureCount = 0;
        value = val;
    }

    void OnMouseEnter()
    {
        thisPiece.transform.localScale = new Vector3(enlargedSize, enlargedSize, 1);
    }

    void OnMouseExit()
    {
        thisPiece.transform.localScale = new Vector3(defaultSize, defaultSize, 1);
    }

    void OnMouseDown()
    {
        IsSelected = IsSelected ? false: true;
    }

    public void SetCurrentSquare(Square newSquare)
    {
        currentSquare = newSquare;
    }

    public Square GetCurrentSquare()
    {
        return currentSquare;
    }

    public Transform GetTransform()
    {
        return thisPiece;
    }



    public bool GetSelected()
    {
        return IsSelected;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void Move(Vector3 newPosition)
    {
        thisPiece.transform.position = newPosition;
        IsSelected = false;
    }


    public static void Capture(Piece p)
    {
        if(p == null)
        {
            return;
        }
        Transform t = p.GetTransform();
        if (p.tag == "White")
        {
            int col = BlackCaptureCount % 3, row = 0;
            for (int i = 3; i <= BlackCaptureCount; i += 3)
            {
                row++;
            }
            t.transform.position = new Vector3(8 + col, 7 - row);
            BlackCaptureCount++;
        }
        else
        {
            
            int col = WhiteCaptureCount % 3, row = 0;
            for (int i = 3; i <= WhiteCaptureCount; i+=3)
            {
                row++;
            }
            t.transform.position = new Vector3(-1-col, row);
            WhiteCaptureCount++;
        }
        t.transform.localScale = new Vector3(SmallSize, SmallSize, 1f);
    }

    public void Promote(string newName, Sprite newPiece)
    {
        this.name = newName;
        myRen.sprite = newPiece;
        this.value = this.tag == "White" ? 9 : -9;
    }

    public void Promote()
    {
        this.name = "Queen";
        this.value = this.tag == "White"? 9: -9;
    }

    public void CalculatePossibleMoves(Square[,] AllSquares)
    {
        possibleMoves = new List<Square>();
        Square s = GetCurrentSquare();
        int rank = s.GetRank(), file = s.GetFile();
        if (this.name == "Pawn")
        {
            if (!Moved)
            {
                if (this.tag == "White")
                {
                    if (AllSquares[rank + 1, file].GetIsEmpty())
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file]);
                        if (AllSquares[rank + 2, file].GetIsEmpty())
                        {
                            possibleMoves.Add(AllSquares[rank + 2, file]);
                        }
                    }
                    
                    if (file + 1 < 8 && !AllSquares[rank + 1, file +1].GetIsEmpty() && AllSquares[rank + 1, file + 1].GetPiece().tag == "Black")
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file + 1]);
                    }
                    if (file - 1 >= 0 && !AllSquares[rank + 1, file - 1].GetIsEmpty() && AllSquares[rank + 1, file - 1].GetPiece().tag == "Black")
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file - 1]);
                    }


                }
                else
                {
                    if (AllSquares[rank - 1, file].GetIsEmpty())
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file]);
                        if (AllSquares[rank - 2, file].GetIsEmpty())
                        {
                            possibleMoves.Add(AllSquares[rank - 2, file]);
                        }
                    }
                    
                    if (file + 1 < 8 && !AllSquares[rank - 1, file + 1].GetIsEmpty() && AllSquares[rank - 1, file + 1].GetPiece().tag == "White")
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file + 1]);
                    }
                    if (file - 1 >= 0 && !AllSquares[rank - 1, file - 1].GetIsEmpty() && AllSquares[rank - 1, file - 1].GetPiece().tag == "White")
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file - 1]);
                    }
                }
            }
            else
            {
                if (this.tag == "White")
                {
                    if (AllSquares[rank + 1, file].GetIsEmpty())
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file]);
                    }
                    if (file + 1 < 8 && !AllSquares[rank + 1, file + 1].GetIsEmpty() && AllSquares[rank + 1, file + 1].GetPiece().tag == "Black")
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file + 1]);
                    }
                    if (file - 1 >= 0 && !AllSquares[rank + 1, file - 1].GetIsEmpty() && AllSquares[rank + 1, file - 1].GetPiece().tag == "Black")
                    {
                        possibleMoves.Add(AllSquares[rank + 1, file - 1]);
                    }
                }
                else
                {
                    //Debug.Log($"I AM AT RANK{rank} AND FILE{file}");
                    if (AllSquares[rank - 1, file].GetIsEmpty())
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file]);
                    }
                    if (file + 1 < 8 && !AllSquares[rank - 1, file + 1].GetIsEmpty() && AllSquares[rank - 1, file + 1].GetPiece().tag == "White")
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file + 1]);
                    }
                    if (file - 1 >= 0 && !AllSquares[rank - 1, file - 1].GetIsEmpty() && AllSquares[rank - 1, file - 1].GetPiece().tag == "White")
                    {
                        possibleMoves.Add(AllSquares[rank - 1, file - 1]);
                    }
                }
            }

        }
        else if (this.name == "Knight")
        {
            int i = 2, j = 1;
            for (int count = 0; count < 2; count++)
            {
                if (rank + i < 8 && file + j < 8 )
                {
                    if(AllSquares[rank + i, file + j].GetIsEmpty() || AllSquares[rank + i, file + j].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file + j]);
                    }
                }
                if (rank + i < 8 && file - j >= 0)
                {
                    if (AllSquares[rank + i, file - j].GetIsEmpty() || AllSquares[rank + i, file - j].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file - j]);
                    }
                }
                if (rank - i >= 0 && file + j < 8)
                {
                    if (AllSquares[rank - i, file + j].GetIsEmpty() || AllSquares[rank - i, file + j].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file + j]);
                    }
                }
                if (rank - i >= 0 && file - j >= 0)
                {
                    if (AllSquares[rank - i, file - j].GetIsEmpty() || AllSquares[rank - i, file - j].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file - j]);
                    }
                }

                int temp = i;
                i = j;
                j = temp;
            }
        }
        else if (this.name == "Bishop")
        {
            int i = 1;
            while (rank + i < 8 && file + i < 8)
            {
                if (!AllSquares[rank + i, file + i].GetIsEmpty())
                {
                    if (AllSquares[rank + i, file + i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file+ i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + i, file + i]);
                i++;
            }
            i = 1;
            while (rank - i >= 0 && file - i >= 0)
            {
                if (!AllSquares[rank - i, file - i].GetIsEmpty())
                {
                    if (AllSquares[rank - i, file - i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file - i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - i, file - i]);
                i++;
            }
            i = 1;
            while (rank + i < 8 && file - i >= 0)
            {
                if (!AllSquares[rank + i, file - i].GetIsEmpty())
                {
                    if (AllSquares[rank + i, file - i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file - i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + i, file - i]);
                i++;
            }
            i = 1;
            while (rank - i >= 0 && file + i < 8)
            {
                if (!AllSquares[rank - i, file + i].GetIsEmpty())
                {
                    if (AllSquares[rank - i, file + i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file + i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - i, file + i]);
                i++;
            }
        }
        else if (this.name == "Queen")
        {
            int i = 1;
            int left = 1, right = 1, up = 1, down = 1;
            while (rank + i < 8 && file + i < 8)
            {
                if (!AllSquares[rank + i, file + i].GetIsEmpty())
                {
                    if (AllSquares[rank + i, file + i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file + i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + i, file + i]);
                i++;
            }
            i = 1;
            while (rank - i >= 0 && file - i >= 0)
            {
                if (!AllSquares[rank - i, file - i].GetIsEmpty())
                {
                    if (AllSquares[rank - i, file - i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file - i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - i, file - i]);
                i++;
            }
            i = 1;
            while (rank + i < 8 && file - i >= 0)
            {
                if (!AllSquares[rank + i, file - i].GetIsEmpty())
                {
                    if (AllSquares[rank + i, file - i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + i, file - i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + i, file - i]);
                i++;
            }
            i = 1;
            while (rank - i >= 0 && file + i < 8)
            {
                if (!AllSquares[rank - i, file + i].GetIsEmpty())
                {
                    if (AllSquares[rank - i, file + i].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - i, file + i]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - i, file + i]);
                i++;
            }
            while (rank - left >= 0)
            {
                if (!AllSquares[rank - left, file].GetIsEmpty())
                {
                    if (AllSquares[rank - left, file].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - left, file]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - left, file]);
                left++;
            }
            while (rank + right < 8)
            {
                if (!AllSquares[rank + right, file].GetIsEmpty())
                {
                    if (AllSquares[rank + right, file].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + right, file]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + right, file]);
                right++;
            }
            while (file + up < 8)
            {
                if (!AllSquares[rank, file + up].GetIsEmpty())
                {
                    if (AllSquares[rank, file + up].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank, file + up]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank, file + up]);
                up++;
            }
            while (file - down >= 0)
            {
                if (!AllSquares[rank, file - down].GetIsEmpty())
                {
                    if (AllSquares[rank, file - down].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank, file - down]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank, file - down]);
                down++;
            }
        }
        else if (this.name == "Rook")
        {
            int left = 1, right = 1, up = 1, down = 1;

            while (rank - left >= 0)
            {
                if (!AllSquares[rank - left, file].GetIsEmpty())
                {
                    if (AllSquares[rank - left, file].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank - left, file]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank - left, file]);
                left++;
            }
            while (rank + right < 8)
            {
                if (!AllSquares[rank + right, file].GetIsEmpty())
                {
                    if (AllSquares[rank + right, file].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank + right, file]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank + right, file]);
                right++;
            }
            while (file + up < 8)
            {
                if (!AllSquares[rank, file + up].GetIsEmpty())
                {
                    if (AllSquares[rank, file + up].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank, file + up]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank, file + up]);
                up++;
            }
            while (file - down >= 0)
            {
                if (!AllSquares[rank, file - down].GetIsEmpty())
                {
                    if (AllSquares[rank, file - down].GetPiece().tag != this.tag)
                    {
                        possibleMoves.Add(AllSquares[rank, file - down]);
                    }
                    break;
                }
                possibleMoves.Add(AllSquares[rank, file - down]);
                down++;
            }
            
        }
        else if (this.name == "King")
        {
            for (int i = -1; i < 2; i++)
            {
                if (rank + i >= 8 || rank + i < 0)
                {
                    continue;
                }
                for (int j = -1; j < 2; j++)
                {
                    if (file + j >= 8 || file + j < 0 || (rank + i == rank && file + j == file) || (!AllSquares[rank + i, file + j].GetIsEmpty() && AllSquares[rank + i, file + j].GetPiece().tag == this.tag))
                    {
                        continue;
                    }
                    possibleMoves.Add(AllSquares[rank + i, file + j]);
                }


            }

            if (!Moved)
            {

                if (AllSquares[rank, file + 1].GetIsEmpty() && AllSquares[rank, file + 2].GetIsEmpty() && !AllSquares[rank, file + 3].GetIsEmpty() && AllSquares[rank, file + 3].GetPiece().name == "Rook" && !AllSquares[rank, file + 3].GetPiece().Moved)
                {
                    possibleMoves.Add(AllSquares[rank, file + 2]);
                }
                if (AllSquares[rank, file - 1].GetIsEmpty() && AllSquares[rank, file - 2].GetIsEmpty() && AllSquares[rank, file - 3].GetIsEmpty() && !AllSquares[rank, file - 4].GetIsEmpty() && AllSquares[rank, file - 4].GetPiece().name == "Rook" && !AllSquares[rank, file - 4].GetPiece().Moved)
                {
                    possibleMoves.Add(AllSquares[rank, file - 2]);
                }

                
            }
        }
    }
}

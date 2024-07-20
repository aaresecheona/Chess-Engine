using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Engine
{
    int[,] Board;
    int p = 1, B = 3, N = 3, R = 5, Q = 9, K = 1000;
    int Bp = -1, BB = -3, BN = -3, BR = -5, BQ = -9, BK = -1000;
    List<Piece> allPieces;
    public Square[,] Squares;
    public static int count = 1;

    public Engine(List<Piece> pieces, Square[,] s)
    {
        Board = new int[8, 8] { { R,  N,  B,  Q,  K,  B,  N,  R},
                               { p,  p,  p,  p,  p,  p,  p,  p},
                               { 0,  0,  0,  0,  0,  0,  0,  0},
                               { 0,  0,  0,  0,  0,  0,  0,  0},
                               { 0,  0,  0,  0,  0,  0,  0,  0},
                               { 0,  0,  0,  0,  0,  0,  0,  0},
                               { Bp, Bp, Bp, Bp, Bp, Bp, Bp, Bp},
                               { BR, BN, BB, BQ, BK, BB, BN, BR} };
        allPieces = pieces;
        Squares = s; //.Clone() as Square[,];
    }

    public void UpdateBoard(int rank1, int file1, Piece p)
    {
        int rank2 = p.GetCurrentSquare().GetRank(), file2 = p.GetCurrentSquare().GetFile();
        Board[rank1, file1] = 0;
        Board[rank2, file2] = p.value;
    }

    public void UpdateBoard(Piece p, Square s)
    {
        p.Moved = true;
        int rank1 = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        int rank2 = s.GetRank(), file2 = s.GetFile();
        Piece target = s.GetPiece();
        Board[rank1, file1] = 0;
        Board[rank2, file2] = p.value;
        Squares[rank1, file1].SetPiece(null);
        Squares[rank1, file1].SetIsEmpty(true);
        Squares[rank2, file2].SetPiece(p);
        Squares[rank2, file2].SetIsEmpty(false);
        p.SetCurrentSquare(s);
        if (target != null)
        {
            target.SetCurrentSquare(null);
            allPieces.Remove(target);
        }

        foreach (Piece myPiece in allPieces)
        {
            myPiece.CalculatePossibleMoves(Squares);
        }
    }

    private void returnBoard(Piece p1, Square s1, Piece target, Square s2)
    {
        //p1.Moved = false;
        int rank1 = s1.GetRank(), file1 = s1.GetFile();
        int rank2 = s2.GetRank(), file2 = s2.GetFile();
        Board[rank1, file1] = p1.value;
        Board[rank2, file2] = target == null ? 0 : target.value;
        Squares[rank1, file1].SetPiece(p1);
        Squares[rank1, file1].SetIsEmpty(false);
        Squares[rank2, file2].SetPiece(target);
        p1.SetCurrentSquare(s1);
        if (target != null)
        {
            target.SetCurrentSquare(s2);
            Squares[rank2, file2].SetIsEmpty(false);
            allPieces.Add(target);
        }
        else
        {
            Squares[rank2, file2].SetIsEmpty(true);
        }

        foreach (Piece myPiece in allPieces)
        {
            myPiece.CalculatePossibleMoves(Squares);
        }
    }

    //castle short
    public void O_O(Piece p)
    {
        int rank = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        Piece rook = Squares[rank, file1 + 3].GetPiece();

        Board[rank, 5] = rook.value; // rook moved here
        Board[rank, 6] = p.value; // king moved here

        //move rook
        rook.Moved = true;
        rook.GetCurrentSquare().SetPiece(null);
        rook.GetCurrentSquare().SetIsEmpty(true);
        rook.SetCurrentSquare(Squares[rank, file1 + 1]);
        Squares[rank, file1 + 1].SetPiece(rook);
        Squares[rank, file1 + 1].SetIsEmpty(false);

        //move king
        p.Moved = true;
        p.GetCurrentSquare().SetPiece(null);
        p.GetCurrentSquare().SetIsEmpty(true);
        p.SetCurrentSquare(Squares[rank, file1 + 2]);
        Squares[rank, file1 + 2].SetPiece(p);
        Squares[rank, file1 + 2].SetIsEmpty(false);
    }

    //for when user castles
    public void updateBoardWithCastleShort(int rank)
    {
        //king
        Board[rank, 4] = 0;
        Board[rank, 6] = rank == 0 ? K : BK;

        //rook
        Board[rank, 5] = rank == 0 ? R : BR;
        Board[rank, 7] = 0;
    }

    //undo castle short
    public void UndoShortCastle(Piece p)
    {
        int rank = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        Piece rook = Squares[rank, file1 - 1].GetPiece();

        Board[rank, 7] = rook.value; // rook moved here
        Board[rank, 4] = p.value; // king moved here

        // rook
        rook.Moved = false;
        rook.GetCurrentSquare().SetPiece(null);
        rook.GetCurrentSquare().SetIsEmpty(true);
        rook.SetCurrentSquare(Squares[rank, file1 + 1]);
        Squares[rank, file1 + 1].SetPiece(rook);
        Squares[rank, file1 + 1].SetIsEmpty(false);

        // king
        p.Moved = false;
        p.GetCurrentSquare().SetPiece(null);
        p.GetCurrentSquare().SetIsEmpty(true);
        p.SetCurrentSquare(Squares[rank, file1 - 2]);
        Squares[rank, file1 - 2].SetPiece(p);
        Squares[rank, file1 - 2].SetIsEmpty(false);
    }

    //castle long
    public void O_O_O(Piece p)
    {
        int rank = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        Piece rook = Squares[rank, file1 - 4].GetPiece();

        Board[rank, 3] = rook.value; // rook moved here
        Board[rank, 2] = p.value; // king moved here

        //move rook
        rook.Moved = true;
        rook.GetCurrentSquare().SetPiece(null);
        rook.GetCurrentSquare().SetIsEmpty(true);
        rook.SetCurrentSquare(Squares[rank, file1 - 1]);
        Squares[rank, file1 - 1].SetPiece(rook);
        Squares[rank, file1 - 1].SetIsEmpty(false);

        //move king
        p.Moved = true;
        p.GetCurrentSquare().SetPiece(null);
        p.GetCurrentSquare().SetIsEmpty(true);
        p.SetCurrentSquare(Squares[rank, file1 - 2]);
        Squares[rank, file1 - 2].SetPiece(p);
        Squares[rank, file1 - 2].SetIsEmpty(false);
    }

    //for when user castles long
    public void updateBoardWithCastleLong(int rank)
    {
        //king
        Board[rank, 4] = 0;
        Board[rank, 2] = rank == 0 ? K : BK;

        //rook
        Board[rank, 3] = rank == 0 ? R : BR;
        Board[rank, 0] = 0;
    }

    //undo castle long
    public void UndoLongCastle(Piece p)
    {
        int rank = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        Piece rook = Squares[rank, file1 + 1].GetPiece();

        Board[rank, 0] = rook.value; // rook moved here
        Board[rank, 4] = p.value; // king moved here

        // rook
        rook.Moved = false;
        rook.GetCurrentSquare().SetPiece(null);
        rook.GetCurrentSquare().SetIsEmpty(true);
        rook.SetCurrentSquare(Squares[rank, file1 - 2]);
        Squares[rank, file1 - 2].SetPiece(rook);
        Squares[rank, file1 - 2].SetIsEmpty(false);

        // king
        p.Moved = false;
        p.GetCurrentSquare().SetPiece(null);
        p.GetCurrentSquare().SetIsEmpty(true);
        p.SetCurrentSquare(Squares[rank, file1 + 2]);
        Squares[rank, file1 + 2].SetPiece(p);
        Squares[rank, file1 + 2].SetIsEmpty(false);
    }

    //undo Promote
    void UndoPromote(Piece pawn)
    {
        pawn.name = "Pawn";
        pawn.value = 1;
    }


    // minimax function with alpha beta pruning
    public (float, Piece, Square) minimax(int depth, float alpha, float beta, bool WhiteToMove)
    {

        if (GameOver() || depth == 0)
        {
            return (EvaluatePosition(), null, null);
        }
        Piece bestPiece = null;
        Square bestSquare = null;
        float bestEval;


        if (WhiteToMove)
        {
            bestEval = -10000;
            Piece[] arr = allPieces.ToArray();
            foreach (Piece p in arr)
            {
                if (p.tag == "Black")
                    continue;

                List<Square> possibleMoves = p.possibleMoves;

                Square oldSquare = p.GetCurrentSquare();
                bool OldHasPieceMoved = p.Moved;
                int rank1 = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
                int piece = p.value;
                foreach (Square s in possibleMoves)
                {
                    int rank2 = s.GetRank(), file2 = s.GetFile();
                    Piece target = s.GetPiece();
                    int piece2 = target != null ? target.value : 0;
                    string oldName = p.name;

                    // make move and call new minimax function
                    if (p.name == "King" && file2 == file1 + 2)
                    {
                        O_O(p);
                    }
                    else if (p.name == "King" && file2 == file1 - 2)
                    {
                        O_O_O(p);
                    }
                    else
                    {
                        if (p.name == "Pawn" && p.tag == "White" && rank2 == 7)
                        {
                            p.Promote();
                        }
                        else if (p.name == "Pawn" && p.tag == "Black" && rank2 == 0)
                        {
                            p.Promote();
                        }
                        UpdateBoard(p, s);
                    }

                    var (val, asdfPiece, asdfSquare) = minimax(depth - 1, alpha, beta, !WhiteToMove);

                    if (val > bestEval)
                    {
                        bestEval = val;
                        bestPiece = p;
                        bestSquare = s;
                        alpha = val;
                    }


                    // return board back to original position
                    if (p.name == "King" && file2 == file1 + 2)
                    {
                        UndoShortCastle(p);
                    }
                    else if (p.name == "King" && file2 == file1 - 2)
                    {
                        UndoLongCastle(p);
                    }
                    else
                    {
                        if (oldName == "Pawn" && p.tag == "White" && rank2 == 7)
                        {
                            UndoPromote(p);
                            Board[rank2, file2] = 1;
                        }
                        else if (oldName == "Pawn" && p.tag == "Black" && rank2 == 0)
                        {
                            UndoPromote(p);
                            Board[rank2, file2] = -1;
                        }
                        returnBoard(p, oldSquare, target, s);
                        p.Moved = OldHasPieceMoved;
                    }


                    if (bestEval > beta)
                    {
                        return (bestEval, bestPiece, bestSquare);
                    }

                }

            }
        }
        else
        {
            bestEval = 10000;
            Piece[] arr = allPieces.ToArray();
            foreach (Piece p in arr)
            {
                if (p.tag == "White")
                    continue;

                List<Square> possibleMoves = p.possibleMoves;

                Square oldSquare = p.GetCurrentSquare();
                bool OldHasPieceMoved = p.Moved;
                int rank1 = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
                int piece = p.value;
                foreach (Square s in possibleMoves)
                {
                    int rank2 = s.GetRank(), file2 = s.GetFile();
                    Piece target = s.GetPiece();
                    int piece2 = target != null ? target.value : 0;
                    string oldName = p.name;

                    // make move and call new minimax function
                    if (p.name == "King" && file2 == file1 + 2)
                    {
                        O_O(p);
                    }
                    else if (p.name == "King" && file2 == file1 - 2)
                    {
                        O_O_O(p);
                    }
                    else
                    {
                        if (p.name == "Pawn" && p.tag == "White" && rank2 == 7)
                        {
                            p.Promote();
                            Board[rank2, file2] = 9;
                        }
                        else if (p.name == "Pawn" && p.tag == "Black" && rank2 == 0)
                        {
                            p.Promote();
                            Board[rank2, file2] = -9;
                        }
                        UpdateBoard(p, s);
                    }
                    var (val, asdfPiece, asdfSquare) = minimax(depth - 1, alpha, beta, !WhiteToMove);

                    if (val < bestEval)
                    {
                        bestEval = val;
                        bestPiece = p;
                        bestSquare = s;
                        beta = val;
                    }




                    // return board back to original position
                    if (p.name == "King" && file2 == file1 + 2)
                    {
                        UndoShortCastle(p);
                    }
                    else if (p.name == "King" && file2 == file1 - 2)
                    {
                        UndoLongCastle(p);
                    }
                    else
                    {
                        if (oldName == "Pawn" && p.tag == "White" && rank2 == 7)
                        {
                            UndoPromote(p);
                            Board[rank2, file2] = 1;
                        }
                        else if (oldName == "Pawn" && p.tag == "Black" && rank2 == 0)
                        {
                            UndoPromote(p);
                            Board[rank2, file2] = -1;
                        }
                        returnBoard(p, oldSquare, target, s);
                        p.Moved = OldHasPieceMoved;
                    }

                    if (bestEval < alpha)
                    {
                        return (bestEval, bestPiece, bestSquare);
                    }
                }

            }
        }


        return (bestEval, bestPiece, bestSquare);
    }

    public float EvaluatePosition()
    {
        float evaluation = 0;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                evaluation += Board[i, j];
            }
        }
        return evaluation;
    }

    bool GameOver()
    {
        bool whiteKing = false, blackKing = false;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (blackKing && whiteKing)
                {
                    return false;
                }
                if (Board[i, j] == 1000)
                    whiteKing = true;
                if (Board[i, j] == -1000)
                    blackKing = true;
            }
        }
        return true;
    }

    public void printBoard()
    {
        for (int i = 0; i < 8; i++)
        {
            string line = "";
            for (int j = 0; j < 8; j++)
            {
                line += $"{Board[i, j]} ";
            }
            Debug.Log(i + 1 + ": " + line);
        }

    }
}

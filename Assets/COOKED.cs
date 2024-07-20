using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class COOKED : MonoBehaviour
{
    [SerializeField] private Square tilePrefab;
    [SerializeField] private Piece piecePrefab;
    //[SerializeField] private Color lightSquares, darkSquares;

    // Camera
    [SerializeField] private Transform cam;


    // Sprites for pieces
    // Order: bishop, king, knight, pawn, queen, rook
    [SerializeField] private Sprite[] whitePieces;
    [SerializeField] private Sprite[] blackPieces;


    //Arrays containing all squares and pieces
    private Square[,] AllSquares;
    private List<Piece> AllPieces;
    private List<Piece> CapturedPieces;
    private List<Square> possibleMoves;


    [SerializeField] private bool PieceIsSelected = false;
    [SerializeField] private Piece SelectedPiece;
    [SerializeField] private Piece Target = null;


    [SerializeField] private bool PlayerIsWhite;

    [SerializeField] private bool WhiteToMove;

    private CookedEngine myChessEngine;
    public int depth;

    void Start()
    {
        int zero = 0, one = 1;
        int whatColor = Random.Range(zero, one + 1);
        if (whatColor == 1)
        {
            PlayerIsWhite = true;
        }
        else
        {
            PlayerIsWhite = false;
            cam.transform.Rotate(0, 0, 180);
        }

        CapturedPieces = new List<Piece>();
        AllPieces = new List<Piece>();
        whitePieces = Resources.LoadAll<Sprite>("chess pieces/white");
        blackPieces = Resources.LoadAll<Sprite>("chess pieces/black");
        createBoard();
        PieceIsSelected = false;
        SelectedPiece = null;
        WhiteToMove = true;
        possibleMoves = new List<Square>();
        myChessEngine = new CookedEngine(AllPieces, AllSquares);
        myChessEngine.printBoard();


        foreach (Piece myPiece in AllPieces)
        {
            myPiece.CalculatePossibleMoves(AllSquares);
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (PlayerIsWhite && WhiteToMove)
        {
            WhiteTurn();
        }
        else if (!PlayerIsWhite && !WhiteToMove)
        {
            BlackTurn();
        }
        else
        {
            Debug.Log("MYTURN");
            EngineTurn();
            myChessEngine.printBoard();

        }




    }

    void createBoard()
    {
        AllSquares = new Square[8, 8];
        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                bool IslightSquare = (file + rank) % 2 != 0;

                // Spawn and put square into 2d array
                Vector3 v = new Vector3(file, rank);
                AllSquares[rank, file] = (Square)Instantiate(tilePrefab, v, Quaternion.identity);

                //  Rename square and set parameters
                var spawnedTile = AllSquares[rank, file];
                spawnedTile.name = $"Tile {(char)('a' + file)} {rank + 1}";
                spawnedTile.Init(IslightSquare, v);
                spawnedTile.SetRank(rank);
                spawnedTile.SetFile(file);
                spawnedTile.SetIsEmpty(true);
                //spawnedTile.HighlightSquare();


                Piece pppp = null;
                // Will place the pieces into the correct starting squares
                if (rank + 1 == 1)
                {
                    var newPiece = Instantiate(piecePrefab, new Vector3(file, rank, -1f), Quaternion.identity);
                    pppp = newPiece;
                    var picture = newPiece.GetComponent<SpriteRenderer>();
                    AllPieces.Add(newPiece);
                    newPiece.SetCurrentSquare(spawnedTile);
                    spawnedTile.SetIsEmpty(false);
                    spawnedTile.SetPiece(newPiece);
                    if (file + 1 == 1 || file + 1 == 8)
                    {
                        //Sprite mySprite = Sprite.Create(whitePieces[0], new Rect(0, 0, whitePieces[0].width, whitePieces[0].height), new Vector2(whitePieces[0].width / 2, whitePieces[0].height / 2));
                        newPiece.name = $"Rook";
                        newPiece.tag = "White";
                        picture.sprite = whitePieces[5];
                        newPiece.Init(true, 5);
                    }
                    else if (file + 1 == 3 || file + 1 == 6)
                    {
                        newPiece.name = $"Bishop";
                        newPiece.tag = "White";
                        picture.sprite = whitePieces[0];
                        newPiece.Init(true, 3);
                    }
                    else if (file + 1 == 2 || file + 1 == 7)
                    {
                        newPiece.name = $"Knight";
                        newPiece.tag = "White";
                        picture.sprite = whitePieces[2];
                        newPiece.Init(true, 3);
                    }
                    else if (file + 1 == 4)
                    {
                        newPiece.name = $"Queen";
                        newPiece.tag = "White";
                        picture.sprite = whitePieces[4];
                        newPiece.Init(true, 9);
                    }
                    else if (file + 1 == 5)
                    {

                        newPiece.name = $"King";
                        newPiece.tag = "White";
                        picture.sprite = whitePieces[1];
                        newPiece.Init(true, 1000);
                    }
                }
                else if (rank + 1 == 2)
                {
                    var newPiece = Instantiate(piecePrefab, new Vector3(file, rank, -1f), Quaternion.identity);
                    pppp = newPiece;
                    var picture = newPiece.GetComponent<SpriteRenderer>();
                    AllPieces.Add(newPiece);
                    newPiece.SetCurrentSquare(spawnedTile);
                    newPiece.name = $"Pawn";
                    newPiece.tag = "White";
                    picture.sprite = whitePieces[3];
                    newPiece.Init(true, 1);
                    spawnedTile.SetIsEmpty(false);
                    spawnedTile.SetPiece(newPiece);
                }
                else if (rank + 1 == 8)
                {
                    var newPiece = Instantiate(piecePrefab, new Vector3(file, rank, -1f), Quaternion.identity);
                    pppp = newPiece;
                    var picture = newPiece.GetComponent<SpriteRenderer>();
                    AllPieces.Add(newPiece);
                    newPiece.SetCurrentSquare(spawnedTile);
                    spawnedTile.SetIsEmpty(false);
                    spawnedTile.SetPiece(newPiece);
                    if (file + 1 == 1 || file + 1 == 8)
                    {
                        newPiece.name = $"Rook";
                        newPiece.tag = "Black";
                        picture.sprite = blackPieces[5];
                        newPiece.Init(false, -5);
                    }
                    else if (file + 1 == 3 || file + 1 == 6)
                    {
                        newPiece.name = $"Bishop";
                        newPiece.tag = "Black";
                        picture.sprite = blackPieces[0];
                        newPiece.Init(false, -3);
                    }
                    else if (file + 1 == 2 || file + 1 == 7)
                    {
                        newPiece.name = $"Knight";
                        newPiece.tag = "Black";
                        picture.sprite = blackPieces[2];
                        newPiece.Init(false, -3);
                    }
                    else if (file + 1 == 4)
                    {
                        newPiece.name = $"Queen";
                        newPiece.tag = "Black";
                        picture.sprite = blackPieces[4];
                        newPiece.Init(false, -9);
                    }
                    else if (file + 1 == 5)
                    {

                        newPiece.name = $"King";
                        newPiece.tag = "Black";
                        picture.sprite = blackPieces[1];
                        newPiece.Init(false, -1000);
                    }
                }
                else if (rank + 1 == 7)
                {
                    var newPiece = Instantiate(piecePrefab, new Vector3(file, rank, -1f), Quaternion.identity);
                    pppp = newPiece;
                    var picture = newPiece.GetComponent<SpriteRenderer>();
                    AllPieces.Add(newPiece);
                    newPiece.SetCurrentSquare(spawnedTile);
                    newPiece.name = $"Pawn";
                    newPiece.tag = "Black";
                    picture.sprite = blackPieces[3];
                    newPiece.Init(false, -1);
                    spawnedTile.SetIsEmpty(false);
                    spawnedTile.SetPiece(newPiece);
                }
                if (pppp != null && !PlayerIsWhite)
                {
                    pppp.GetTransform().transform.Rotate(0, 0, 180);
                }
            }
        }

        cam.transform.position = new Vector3(4 - 0.5f, 4 - 0.5f, -10);
    }


    public void SelectPiece(Piece newPiece)
    {
        PieceIsSelected = true;
        SelectedPiece = newPiece;
    }

    public void DeselectPiece()
    {
        PieceIsSelected = false;
        SelectedPiece = null;
    }

    void MovePiece(Piece p, Square s)
    {
        Piece Target = s.GetPiece();
        if (Target != null)
        {
            CapturedPieces.Add(Target);
        }
        myChessEngine.UpdateBoard(p, s);
        var newLocation = new Vector3(s.GetPosition().x, s.GetPosition().y, -1f);
        p.Move(newLocation);
        Piece.Capture(Target);
    }

    // Moves peices p to where square s is
    void MovePiece(Piece p, Square s, List<Square> possibleMoves)
    {
        int rank1 = p.GetCurrentSquare().GetRank(), file1 = p.GetCurrentSquare().GetFile();
        if (possibleMoves.Contains(s))
        {
            if (p.name == "Pawn")
            {
                p.Moved = true;
            }
            p.GetCurrentSquare().DontHighlightSquare();
            p.GetCurrentSquare().SetPiece(null);
            p.GetCurrentSquare().SetIsEmpty(true);
            p.SetCurrentSquare(s);
            s.SetPiece(p);
            s.SetIsEmpty(false);
            var newLocation = new Vector3(s.GetPosition().x, s.GetPosition().y, -1f);
            p.Move(newLocation);
            WhiteToMove = WhiteToMove ? false : true;
        }

        p.Moved = true;
        SelectedPiece = null;
        PieceIsSelected = false;
        p.Deselect();
        Target = null;
        foreach (Square mySquare in possibleMoves)
        {
            mySquare.DontShowMoves();
        }
        myChessEngine.UpdateBoard(rank1, file1, p);
        foreach (Piece myPiece in AllPieces)
        {
            myPiece.CalculatePossibleMoves(AllSquares);
        }
    }

    // Moves piece p1 to where piece p2 is
    void MovePiece(Piece p1, Piece p2, List<Square> possibleMoves)
    {
        int rank1 = p1.GetCurrentSquare().GetRank(), file1 = p1.GetCurrentSquare().GetFile();
        Square s1 = p1.GetCurrentSquare(), s2 = p2.GetCurrentSquare();
        if (possibleMoves.Contains(s2))
        {
            if (p1.name == "Pawn")
            {
                p1.Moved = true;
            }
            AllPieces.Remove(p2);
            CapturedPieces.Add(p2);
            s1.SetPiece(null);
            s1.SetIsEmpty(true);
            s2.SetPiece(p1);
            s2.SetIsEmpty(false);
            p1.SetCurrentSquare(s2);
            var newLocation = new Vector3(s2.GetPosition().x, s2.GetPosition().y, -1f);
            p1.Move(newLocation);
            Piece.Capture(p2);
            WhiteToMove = WhiteToMove ? false : true;
        }

        p1.Moved = true;
        p1.Deselect();
        s1.DontHighlightSquare();
        s2.DontHighlightSquare();
        Target = null;
        SelectedPiece = null;
        PieceIsSelected = false;
        foreach (Square mySquare in possibleMoves)
        {
            mySquare.DontShowMoves();
        }
        myChessEngine.UpdateBoard(rank1, file1, p1);
        foreach (Piece myPiece in AllPieces)
        {
            myPiece.CalculatePossibleMoves(AllSquares);
        }
    }

    public void CastleShort(Piece p)
    {
        p.Moved = true;
        if (p.tag == "White")
        {
            MovePiece(p, AllSquares[0, 6], possibleMoves);
            MovePiece(AllSquares[0, 7].GetPiece(), AllSquares[0, 5], possibleMoves);
        }
        else
        {
            MovePiece(p, AllSquares[7, 6], possibleMoves);
            MovePiece(AllSquares[7, 7].GetPiece(), AllSquares[7, 5], possibleMoves);
        }

        WhiteToMove = WhiteToMove ? false : true;

    }

    public void CastleLong(Piece p)
    {
        p.Moved = true;
        if (p.tag == "White")
        {
            MovePiece(p, AllSquares[0, 2], possibleMoves);
            MovePiece(AllSquares[0, 0].GetPiece(), AllSquares[0, 3], possibleMoves);
        }
        else
        {
            MovePiece(p, AllSquares[7, 2], possibleMoves);
            MovePiece(AllSquares[7, 0].GetPiece(), AllSquares[7, 3], possibleMoves);
        }
        WhiteToMove = WhiteToMove ? false : true;
    }




    void WhiteTurn()
    {
        bool noOneSelected = true;

        foreach (Piece p in AllPieces)
        {
            if (p.tag == "Black" && p.GetSelected() && Target != p)
            {
                if (PieceIsSelected)
                {
                    Target = p;
                }
                p.Deselect();
                continue;
            }
            if (p == SelectedPiece && p.GetSelected())
            {
                noOneSelected = false;
            }
            else if (p.GetSelected() && p.tag == "White")
            {
                if (Target != null)
                {
                    Target.Deselect();
                    Target = null;
                }
                if (SelectedPiece != null)
                {
                    SelectedPiece.Deselect();
                    SelectedPiece.GetCurrentSquare().DontHighlightSquare();
                    foreach (Square s in possibleMoves)
                    {
                        s.DontShowMoves();
                    }
                }
                PieceIsSelected = true;
                noOneSelected = false;
                SelectedPiece = p;
            }
        }

        if (noOneSelected)
        {
            foreach (Square s in possibleMoves)
            {
                s.DontShowMoves();
            }
            if (SelectedPiece != null)
            {
                SelectedPiece.GetCurrentSquare().DontHighlightSquare();
            }
            PieceIsSelected = false;
            SelectedPiece = null;
        }

        if (PieceIsSelected)
        {
            SelectedPiece.GetCurrentSquare().HighlightSquare();
            possibleMoves = SelectedPiece.possibleMoves;
            foreach (Square sq in possibleMoves)
            {
                sq.ShowMoves();
            }
            Square s = null;
            foreach (Square sq in AllSquares)
            {
                if (sq.GetIsSelected())
                {
                    s = sq;
                    break;
                }
            }
            if (s != null && s.GetIsSelected() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (SelectedPiece.name == "King" && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() + 2])
                {
                    CastleShort(SelectedPiece);
                }
                else if (SelectedPiece.name == "King" && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() - 2])
                {
                    CastleLong(SelectedPiece);
                }
                else if (SelectedPiece.name == "Pawn" && s.GetRank() == 7)
                {
                    SelectedPiece.Promote("Queen", whitePieces[4]);
                    MovePiece(SelectedPiece, s, possibleMoves);
                }
                else
                {
                    MovePiece(SelectedPiece, s, possibleMoves);
                }

            }
            else
            {

                //foreach (Piece p in AllPieces)
                //{
                //if (p.tag == "Black" && p.GetSelected())
                //{
                //Target = p;
                //break;
                //}
                //}
                if (Target != null)
                {
                    if (SelectedPiece.name == "Pawn" && Target.GetCurrentSquare().GetRank() == 7)
                    {
                        SelectedPiece.Promote("Queen", whitePieces[4]);
                        MovePiece(SelectedPiece, Target, possibleMoves);
                    }
                    else
                    {
                        MovePiece(SelectedPiece, Target, possibleMoves);
                    }

                }

            }

        }
    }

    void BlackTurn()
    {
        bool noOneSelected = true;

        foreach (Piece p in AllPieces)
        {
            if (p.tag == "White" && p.GetSelected() && Target != p)
            {
                if (PieceIsSelected)
                {
                    Target = p; ;
                }
                p.Deselect();
                continue;
            }
            if (p == SelectedPiece && p.GetSelected())
            {
                noOneSelected = false;
            }
            else if (p.GetSelected() && p.tag == "Black")
            {
                if (Target != null)
                {
                    Target.Deselect();
                    Target = null;
                }
                if (SelectedPiece != null)
                {
                    SelectedPiece.Deselect();
                    SelectedPiece.GetCurrentSquare().DontHighlightSquare();
                    foreach (Square s in possibleMoves)
                    {
                        s.DontShowMoves();
                    }
                }
                PieceIsSelected = true;
                noOneSelected = false;
                SelectedPiece = p;
            }
        }

        if (noOneSelected)
        {
            foreach (Square s in possibleMoves)
            {
                s.DontShowMoves();
            }
            if (SelectedPiece != null)
            {
                SelectedPiece.GetCurrentSquare().DontHighlightSquare();
            }
            PieceIsSelected = false;
            SelectedPiece = null;
        }

        if (PieceIsSelected)
        {
            SelectedPiece.GetCurrentSquare().HighlightSquare();
            possibleMoves = SelectedPiece.possibleMoves;
            foreach (Square sq in possibleMoves)
            {
                sq.ShowMoves();
            }

            Square s = null;
            foreach (Square sq in AllSquares)
            {
                if (sq.GetIsSelected())
                {
                    s = sq;
                    break;
                }
            }
            if (s != null && s.GetIsSelected() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (SelectedPiece.name == "King" && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() + 2])
                {
                    CastleShort(SelectedPiece);
                }
                else if (SelectedPiece.name == "King" && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() - 2])
                {
                    CastleLong(SelectedPiece);
                }
                else if (SelectedPiece.name == "Pawn" && s.GetRank() == 0)
                {
                    SelectedPiece.Promote("Queen", blackPieces[4]);
                    MovePiece(SelectedPiece, s, possibleMoves);
                }
                else
                {
                    MovePiece(SelectedPiece, s, possibleMoves);
                }
            }
            else
            {

                //foreach (Piece p in AllPieces)
                //{
                //if (p.tag == "Black" && p.GetSelected())
                //{
                //Target = p;
                //break;
                //}
                //}
                if (Target != null)
                {
                    if (SelectedPiece.name == "Pawn" && Target.GetCurrentSquare().GetRank() == 0)
                    {
                        SelectedPiece.Promote("Queen", whitePieces[4]);
                        MovePiece(SelectedPiece, Target, possibleMoves);
                    }
                    else
                    {
                        MovePiece(SelectedPiece, Target, possibleMoves);
                    }
                }

            }

        }
    }

    void EngineTurn()
    {
        var (eval, bestPiece, bestSquare) = myChessEngine.minimax(depth, !PlayerIsWhite);
        MovePiece(bestPiece, bestSquare);
        WhiteToMove = WhiteToMove ? false : true;
    }

    List<Square> ShowLegalMoves(Piece p)
    {
        List<Square> allPossibleMoves = new List<Square>();
        Square s = p.GetCurrentSquare();
        int rank = s.GetRank(), file = s.GetFile();
        if (SelectedPiece.name == "Pawn")
        {
            if (!SelectedPiece.Moved)
            {
                if (SelectedPiece.tag == "White")
                {
                    AllSquares[rank + 1, file].ShowMoves();
                    AllSquares[rank + 2, file].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank + 1, file]);
                    allPossibleMoves.Add(AllSquares[rank + 2, file]);
                }
                else
                {
                    AllSquares[rank - 1, file].ShowMoves();
                    AllSquares[rank - 2, file].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank - 1, file]);
                    allPossibleMoves.Add(AllSquares[rank - 2, file]);
                }
            }
            else
            {
                if (SelectedPiece.tag == "White")
                {
                    AllSquares[rank + 1, file].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank + 1, file]);
                }
                else
                {
                    AllSquares[rank - 1, file].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank - 1, file]);
                }
            }

        }
        else if (SelectedPiece.name == "Knight")
        {
            int i = 2, j = 1;
            for (int count = 0; count < 2; count++)
            {
                if (rank + i < 8 && file + j < 8)
                {
                    AllSquares[rank + i, file + j].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank + i, file + j]);
                }
                if (rank + i < 8 && file - j >= 0)
                {
                    AllSquares[rank + i, file - j].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank + i, file - j]);
                }
                if (rank - i >= 0 && file + j < 8)
                {
                    AllSquares[rank - i, file + j].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank - i, file + j]);
                }
                if (rank - i >= 0 && file - j >= 0)
                {
                    AllSquares[rank - i, file - j].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank - i, file - j]);
                }

                int temp = i;
                i = j;
                j = temp;
            }
        }
        else if (SelectedPiece.name == "Bishop")
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (rank == i && file == j)
                    {
                        continue;
                    }
                    if (rank - file == i - j || rank + file == i + j)
                    {
                        AllSquares[i, j].ShowMoves();
                        allPossibleMoves.Add(AllSquares[i, j]);
                    }
                }
            }
        }
        else if (SelectedPiece.name == "Queen")
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (rank == i && file == j)
                    {
                        continue;
                    }
                    if (rank == i || file == j || rank - file == i - j || rank + file == i + j)
                    {
                        AllSquares[i, j].ShowMoves();
                        allPossibleMoves.Add(AllSquares[i, j]);
                    }
                }
            }
        }
        else if (SelectedPiece.name == "Rook")
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (rank == i && file == j)
                    {
                        continue;
                    }
                    if (rank == i || file == j)
                    {
                        AllSquares[i, j].ShowMoves();
                        allPossibleMoves.Add(AllSquares[i, j]);
                    }
                }
            }
        }
        else if (SelectedPiece.name == "King")
        {
            for (int i = -1; i < 2; i++)
            {
                if (rank + i >= 8 || rank + i < 0)
                {
                    continue;
                }
                for (int j = -1; j < 2; j++)
                {
                    if (file + j >= 8 || file + j < 0 || (rank + i == rank && file + j == file))
                    {
                        continue;
                    }
                    AllSquares[rank + i, file + j].ShowMoves();
                    allPossibleMoves.Add(AllSquares[rank + i, file + j]);
                }


            }
        }
        return allPossibleMoves;
    }


}















class CookedEngine
{
    int[,] Board;
    int p = 1, B = 3, N = 3, R = 5, Q = 9, K = 1000;
    int Bp = -1, BB = -3, BN = -3, BR = -5, BQ = -9, BK = -1000;
    List<Piece> allPieces;
    public Square[,] Squares;

    public CookedEngine(List<Piece> pieces, Square[,] s)
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
    }


    public (float, Piece, Square) minimax(int depth, bool WhiteToMove)
    {

        if (GameOver() || depth == 0)
        {
            return ((float)EvaluatePosition(), null, null);
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
                    int rank2 = s.GetRank(), file2 = s.GetRank();
                    Piece target = s.GetPiece();
                    int piece2 = target != null ? target.value : 0;

                    // make move and call new minimax function
                    UpdateBoard(p, s);
                    var (val, asdfPiece, asdfSquare) = minimax(depth - 1, !WhiteToMove);

                    if (val > bestEval)
                    {
                        bestEval = val;
                        bestPiece = p;
                        bestSquare = s;
                    }


                    // return board back to original position
                    returnBoard(p, oldSquare, target, s);
                    p.Moved = OldHasPieceMoved;

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

                    // make move and call new minimax function
                    UpdateBoard(p, s);
                    var (val, asdfPiece, asdfSquare) = minimax(depth - 1, !WhiteToMove);

                    if (val < bestEval)
                    {
                        bestEval = val;
                        bestPiece = p;
                        bestSquare = s;
                    }


                    // return board back to original position
                    returnBoard(p, oldSquare, target, s);
                    p.Moved = OldHasPieceMoved;
                }

            }
        }


        return (bestEval, bestPiece, bestSquare);
    }

    int EvaluatePosition()
    {
        int evaluation = 0;
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
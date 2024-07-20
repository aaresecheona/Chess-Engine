using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class ExploreBoard : MonoBehaviour
{
    [SerializeField] private Square tilePrefab;
    [SerializeField] private Piece piecePrefab;
    [SerializeField] private ChangeSide FLIPBOARD;
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



    [SerializeField] private bool WhiteToMove;

    public GameOverScreen gameOverScreen;
    bool gameover = false;


    void Start()
    {

        CapturedPieces = new List<Piece>();
        AllPieces = new List<Piece>();
        whitePieces = Resources.LoadAll<Sprite>("chess pieces/white");
        blackPieces = Resources.LoadAll<Sprite>("chess pieces/black");
        createBoard();
        PieceIsSelected = false;
        SelectedPiece = null;
        WhiteToMove = true;
        possibleMoves = new List<Square>();


        foreach (Piece myPiece in AllPieces)
        {
            myPiece.CalculatePossibleMoves(AllSquares);
        }
    }

    void Update()
    {

        foreach (Piece p in CapturedPieces)
        {
            if (p.name == "King")
            {
                GameOver(p.tag == "White" ? "Black" : "White");
                gameover = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (!gameover)
        {
            if (WhiteToMove)
            {
                WhiteTurn();
            }
            else
            {
                BlackTurn();
            }
        }

        if (FLIPBOARD.GetClicked())
        {
            if (FLIPBOARD.GetColor())
            {
                RotatePiecesForWhite();
            }
            else
            {
                RotatePiecesForBlack();
            }
            FLIPBOARD.UnClick();
        }



    }

    //game over
    void GameOver(string winner)
    {
        gameOverScreen.Setup(winner);
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
                if (SelectedPiece.name == "King" && !SelectedPiece.Moved && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() + 2])
                {
                    CastleShort(SelectedPiece);
                }
                else if (SelectedPiece.name == "King" && !SelectedPiece.Moved && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() - 2])
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
                if (SelectedPiece.name == "King" && !SelectedPiece.Moved && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() + 2])
                {
                    CastleShort(SelectedPiece);
                }
                else if (SelectedPiece.name == "King" && !SelectedPiece.Moved && s == AllSquares[SelectedPiece.GetCurrentSquare().GetRank(), SelectedPiece.GetCurrentSquare().GetFile() - 2])
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
                if (Target != null)
                {
                    if (SelectedPiece.name == "Pawn" && Target.GetCurrentSquare().GetRank() == 0)
                    {
                        SelectedPiece.Promote("Queen", blackPieces[4]);
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


    void RotatePiecesForBlack()
    {
        foreach (Piece p in AllPieces)
        {
            p.GetTransform().transform.Rotate(0, 0, 180);
        }
        foreach (Piece p in CapturedPieces)
        {
            p.GetTransform().transform.Rotate(0, 0, 180);
        }
        cam.transform.Rotate(0, 0, 180);
    }

    void RotatePiecesForWhite()
    {
        foreach (Piece p in AllPieces)
        {
            p.GetTransform().transform.Rotate(0, 0, -180);
        }

        foreach (Piece p in CapturedPieces)
        {
            p.GetTransform().transform.Rotate(0, 0, -180);
        }
        cam.transform.Rotate(0, 0, -180);
    }

}

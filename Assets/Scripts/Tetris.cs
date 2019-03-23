using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


// used to order pieces: top first than left first
public class PieceTopLeftHandComparer: IComparer<Transform>
{
    public int Compare(Transform t1, Transform t2)
    {
        if (t1.position.y == t2.position.y)
        {
            return t1.position.x < t2.position.x ? -1 : 1;
        }
        else
        {
            return t1.position.y > t2.position.y ? -1 : 1;
        }
    }
}

public class Tetris : MonoBehaviour
{
    public float pieceStartSpeed = 1.0f;
    public float pieceSpeed = 0.001f;
    public Vector3 spawnLeft;
    public Vector3 spawnRight;
    public float horizontalSpeed = 0.1f;

    public int score = 0;

    private Transform lastPiece = null;
    private PieceData lastPieceData = null;

    private List<Transform[]> mathModel;

    private Vector2Int playAreaSize;
    private Vector3 playAreaCenter;
    private Vector3 playAreaExtents;
    private Vector3 playAreaTopLeft;
    private float cellSize;

    public PieceDatabase pieceDatabase;

    public int spawnCount = 1;

    #region Tetris Initialization

    void Start()
    {
        playAreaSize = GetComponent<TetrisPlayArea>().playableAreaSize;
        cellSize = GetComponent<TetrisPlayArea>().cellSize;

        InitPieceDatabase();

        mathModel = new List<Transform[]>(playAreaSize.y);
        for (int i = 0; i < playAreaSize.y; i++)
        {
            mathModel.Add(new Transform[playAreaSize.x]);
        }

        playAreaCenter = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.center;
        playAreaExtents = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents;
        playAreaTopLeft = new Vector3(playAreaCenter.x - playAreaExtents.x, playAreaCenter.y + playAreaExtents.y, 0);
    }

    void InitPieceDatabase()
    {
        float fullRotation = 0;
        PieceTopLeftHandComparer comparer = new PieceTopLeftHandComparer();

        for (int i = 0; i < pieceDatabase.pieceTypes.Length; i++)
        {
            PieceType pt = pieceDatabase.pieceTypes[i];
            GameObject go = Instantiate(pt.prefab, new Vector3(-100, -100, 0), Quaternion.identity);

            if (pt.rotations.Length != 0)
            {
                pt.pieceRotations = new PieceRotation[pt.rotations.Length];
            }
            else
            {
                pt.pieceRotations = new PieceRotation[1];
            }
            
            SpriteRenderer area = go.transform.GetChild(go.transform.childCount - 2).GetComponent<SpriteRenderer>();

            Transform[] children = new Transform[go.transform.childCount - 2];
            for (int k = 0; k < go.transform.childCount - 2; k++)
            {
                children[k] = go.transform.GetChild(k);
            }

            fullRotation = 0;
            for (int j = 0; j < pt.pieceRotations.Length; j++)
            {
                Bounds bounds = area.bounds;

                pt.pieceRotations[j] = new PieceRotation();
                pt.pieceRotations[j].cells = new int[(int)(bounds.extents.y * 2 / cellSize), (int)(bounds.extents.x * 2 / cellSize)];
                pt.pieceRotations[j].rotation = fullRotation;

                System.Array.Sort(children, comparer);

                for (int k = 0; k < children.Length; k++)
                {
                    Vector3 localPos = children[k].position - GetPieceTopLeft(go.transform);
                    float centerX = localPos.x / cellSize;
                    float centerY = localPos.y / cellSize;
                    string name = children[k].name;
                    pt.pieceRotations[j].cells[
                        -Mathf.FloorToInt(Mathf.Abs(centerY)) * (int)Mathf.Sign(centerY), 
                        Mathf.FloorToInt(Mathf.Abs(centerX)) * (int)Mathf.Sign(centerX)
                        ] = System.Convert.ToInt32(name.Substring(name.Length - 1 , 1));
                }

                FindPieceRotationBottomCells(pt.pieceRotations[j]);
                FindPieceRotationLeftCells(pt.pieceRotations[j]);
                FindPieceRotationRightCells(pt.pieceRotations[j]);

                if (pt.rotations.Length != 0)
                {
                    fullRotation += pt.rotations[j];
                    go.transform.RotateAround(go.transform.GetChild(go.transform.childCount - 1).position, Vector3.forward, pt.rotations[j]);
                }
            }

            GameObject.Destroy(go);
        }
    }

    void FindPieceRotationBottomCells(PieceRotation pr)
    {
        pr.bottomCells = new Vector2Int[pr.cells.GetLength(1)];

        for (int i = 0; i < pr.cells.GetLength(1); i++)
        {
            for (int j = pr.cells.GetLength(0) - 1; j > -1; j--)
            {
                if (pr.cells[j,i] != 0)
                {
                    pr.bottomCells[i] = new Vector2Int(j, pr.cells[j, i]);
                    break;
                }
            }
        }
    }

    void FindPieceRotationLeftCells(PieceRotation pr)
    {
        pr.leftCells = new Vector2Int[pr.cells.GetLength(0)];

        for (int i = 0; i < pr.cells.GetLength(0); i++)
        {
            for (int j = 0; j < pr.cells.GetLength(1); j++)
            {
                if (pr.cells[i, j] != 0)
                {
                    pr.leftCells[i] = new Vector2Int(j, pr.cells[i, j]);
                    break;
                }
            }
        }
    }

    void FindPieceRotationRightCells(PieceRotation pr)
    {
        pr.rightCells = new Vector2Int[pr.cells.GetLength(0)];

        for (int i = 0; i < pr.cells.GetLength(0); i++)
        {
            for (int j = pr.cells.GetLength(1) - 1; j > -1; j--)
            {
                if (pr.cells[i, j] != 0)
                {
                    pr.rightCells[i] = new Vector2Int(j, pr.cells[i, j]);
                    break;
                }
            }
        }
    }

    #endregion

    #region Tetris Update

    void Update()
    {
        if (lastPiece == null)
        {
            // at the start or after last piece reached floor -> spawn another piece
            SpawnPiece();
            score += 100;
            GameObject.Find("UI").transform.GetChild(0).GetComponent<Text>().text = score.ToString();
        }
        else
        {
            // else move last piece down
            MovePieceVertically();
        }

        // move piece horizontally responding to player input
        if (Time.time % 0.1f < 0.016f)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(horizontalInput) > 0.0001f)
            {
                MovePieceHorizontally(horizontalInput);
            }
        }

        // rotate piece responding to player input
        if (Input.GetButtonDown("Rotate"))
        {
            RotatePiece();
        }
    }

    Transform SpawnPiece()
    {
        // determine piece type
        PieceType pt = pieceDatabase.pieceTypes[Random.Range(0, pieceDatabase.pieceTypes.Length)];
        //PieceType pt = pieceDatabase.pieceTypes[0];

        Transform pieceType = pt.prefab.transform;
        Bounds bounds = pieceType.GetChild(pieceType.childCount - 2).GetComponent<SpriteRenderer>().bounds;

        // determine spawn position
        Debug.Log(playAreaSize.x - 1 - bounds.extents.x * 2);
        Vector3 piecePos = new Vector3(spawnLeft.x + Random.Range(0, playAreaSize.x - (int)(bounds.extents.x / cellSize * 2) + 1) * cellSize, spawnLeft.y, spawnLeft.z);

        // instantiate piece
        lastPiece = Instantiate(pieceType, piecePos, Quaternion.identity);
        lastPieceData = lastPiece.GetComponent<PieceData>();

        return lastPiece;
    }

    void MovePieceVertically()
    {
        float currentSpeed = pieceSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput < -0.0001f)
        {
            currentSpeed *= 4;
        }

        MovePieceToFarthestFreePos(currentSpeed);
    }

    void MovePieceHorizontally(float horizontalInput)
    {
        Vector2 mathModelPos = GlobalPos2MathModelPos(GetPieceTopLeft(lastPiece));
        PieceType pt =  lastPieceData.type;
        PieceRotation pr = lastPieceData.type.pieceRotations[lastPieceData.rotationVariation];
        bool canMove = true;

        // left
        if (Mathf.Sign(horizontalInput) < 0)
        {
            canMove = mathModelPos.x > 0;

            for (int i = 0; i < pr.leftCells.Length; i++)
            {
                canMove = canMove && mathModel[Mathf.FloorToInt(mathModelPos.y) + i][(int)mathModelPos.x - pr.leftCells[i].x] == null;
                canMove = canMove && mathModel[Mathf.CeilToInt(mathModelPos.y) + i][(int)mathModelPos.x - pr.leftCells[i].x] == null;
            }
        }
        // right
        else
        {
            canMove = mathModelPos.x < playAreaSize.x;

            for (int i = 0; i < pr.rightCells.Length; i++)
            {
                canMove = canMove && mathModel[Mathf.FloorToInt(mathModelPos.y) + i][(int)mathModelPos.x - pr.rightCells[i].x] == null;
                canMove = canMove && mathModel[Mathf.CeilToInt(mathModelPos.y) + i][(int)mathModelPos.x - pr.rightCells[i].x] == null;
            }
        }

        //// left
        //if (Mathf.Sign(horizontalInput) < 0)
        //{
        //    switch (pieceData.pieceType)
        //    {
        //        case 0:
        //            canMove = canMove && mathModelPos.x > 0 && 
        //                mathModel[mathModelPos.y - 1][mathModelPos.x] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 1] == null;
        //            break;
        //        case 1:
        //            canMove = canMove && mathModelPos.x > 0 && 
        //                mathModel[mathModelPos.y - 1][mathModelPos.x] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 1] == null &&
        //                mathModel[mathModelPos.y - 1][mathModelPos.x + 2] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 3] == null;
        //            break;
        //        case 2:
        //            canMove = canMove && mathModelPos.x > 0 &&
        //                mathModel[mathModelPos.y][mathModelPos.x] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 1] == null;
        //            break;
        //        case 3:
        //            canMove = canMove && mathModelPos.x > 0 &&
        //                mathModel[mathModelPos.y][mathModelPos.x] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 1] == null;
        //            break;
        //        case 4:
        //            canMove = canMove && mathModelPos.x > 0 &&
        //                mathModel[mathModelPos.y - 1][mathModelPos.x] == null && mathModel[mathModelPos.y - 1][mathModelPos.x + 1] == null;
        //            break;
        //        case 5:
        //            canMove = canMove && mathModelPos.x > 0 &&
        //                mathModel[mathModelPos.y - 1][mathModelPos.x] == null && mathModel[mathModelPos.y][mathModelPos.x + 1] == null;
        //            break;
        //        default:
        //            Debug.Log($"MoveHorizontally left pieceType unknown {pieceData.pieceType}");
        //            break;
        //    }
        //}
        //// rigth
        //else
        //{
        //    switch (pieceData.pieceType)
        //    {
        //        case 0:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 1][mathModelPos.x] == null && mathModel[mathModelPos.y + 3][mathModelPos.x + 1] == null;
        //            break;
        //        case 1:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 1][mathModelPos.x] == null && mathModel[mathModelPos.y + 1][mathModelPos.x + 1] == null &&
        //                mathModel[mathModelPos.y + 1][mathModelPos.x + 2] == null && mathModel[mathModelPos.y + 1][mathModelPos.x + 3] == null;
        //            break;
        //        case 2:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 2][mathModelPos.x] == null && mathModel[mathModelPos.y + 3][mathModelPos.x + 1] == null;
        //            break;
        //        case 3:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 3][mathModelPos.x] == null && mathModel[mathModelPos.y + 2][mathModelPos.x + 1] == null;
        //            break;
        //        case 4:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 2][mathModelPos.x] == null && mathModel[mathModelPos.y + 2][mathModelPos.x + 1] == null;
        //            break;
        //        case 5:
        //            canMove = canMove && mathModelPos.x < playAreaSize.x &&
        //                mathModel[mathModelPos.y + 2][mathModelPos.x] == null && mathModel[mathModelPos.y + 3][mathModelPos.x + 1] == null;
        //            break;
        //        default:
        //            Debug.Log($"MoveHorizontally right pieceType unknown {pieceData.pieceType}");
        //            break;
        //    }
        //}

        if (canMove)
        {
            lastPiece.transform.Translate(Mathf.Sign(horizontalInput) * horizontalSpeed, 0, 0, Space.World);
        }
    }

    void RotatePiece()
    {
        bool canRotate = true;

        if (canRotate)
        {
            GetComponent<AudioSource>().Play();

            if (lastPieceData.type.rotations.Length != 0)
            {
                lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, lastPieceData.type.rotations[lastPieceData.rotationVariation]);
                lastPieceData.rotationVariation += 1;
                if (lastPieceData.rotationVariation % lastPieceData.type.rotations.Length == 0)
                {
                    lastPieceData.rotationVariation = 0;
                }
            }
        }
    }

    void MovePieceToFarthestFreePos(float speed)
    {
        int obstacleY = 10000;
        int closestObstacleY = obstacleY;
        Vector2 pieceMathPos;
        int bottomCellY = 0;

        pieceMathPos = GlobalPos2MathModelPos(GetPieceTopLeft(lastPiece));

        PieceRotation r = lastPieceData.type.pieceRotations[lastPieceData.rotationVariation];

        for (int i = 0; i < r.bottomCells.Length; i++)
        {
            obstacleY = FindFathestFreeCellY(pieceMathPos, r.bottomCells[i].x, i, speed);
            if (closestObstacleY > obstacleY)
            {
                closestObstacleY = obstacleY;
                bottomCellY = r.bottomCells[i].x;
            }
        }

        if (closestObstacleY != 10000)
        {
            lastPiece.transform.position = GetPieceTopLeftToPosCorrection() + MathModelPos2GlobalPos((int)pieceMathPos.x, closestObstacleY - bottomCellY);

            for (int i = 0; i < r.cells.GetLength(0); i++)
            {
                for (int j = 0; j < r.cells.GetLength(1); j++)
                {
                    if (r.cells[i, j] != 0)
                    {
                        mathModel[closestObstacleY - bottomCellY + i][(int)pieceMathPos.x + j] = lastPiece.GetChild(r.cells[i, j] - 1);
                    }
                }
            }

            lastPiece = null;
            lastPieceData = null;

            DestroyRows();
        }
        else
        {
            lastPiece.transform.Translate(0, -speed, 0, Space.World);
        }
    }

    void DestroyRows()
    {
        int count;
        for (int i = 0; i < playAreaSize.y; i++)
        {
            count = 0;
            for (int j = 0; j < playAreaSize.x; j++)
            {
                if (mathModel[i][j] != null)
                {
                    count++;
                }
            }

            if (count == playAreaSize.x)
            {
                for (int j = 0; j < playAreaSize.x; j++)
                {
                    if (mathModel[i][j].parent.childCount <= 3)
                    {
                        GameObject.Destroy(mathModel[i][j].parent.gameObject);
                    }
                    else
                    {
                        GameObject.Destroy(mathModel[i][j].gameObject);
                    }
                    mathModel[i][j] = null;
                }

                // move destroyed row to the top
                Transform[] row = mathModel[i];
                mathModel.RemoveAt(i);
                mathModel.Insert(0, row);

                MovePiecesDown(i - 1);
            }
        }
    }

    void MovePiecesDown(int untilRow)
    {
        for (int i = 0; i < untilRow; i++)
        {
            for (int j = 0; j < playAreaSize.x; j++)
            {
                if (mathModel[i][j] != null)
                {
                    mathModel[i][j].Translate(0, -cellSize, 0, Space.World);
                }
            }
        }
    }

    #endregion

    #region Tetris Math

    // returns closeset obstacle y in math model coordinates
    int FindFathestFreeCellY(Vector2 pieceMathPos, int bottomCellY, int xCellOffset, float speed)
    {
        int obstacleY = 10000;
        int i = 0;
        // does the lowest point of part of the piece (column j) crosses the virtual grid border?
        while (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellY < pieceMathPos.y + speed / cellSize + bottomCellY)
        {
            // is the lowest point of part of the piece (column j) lower than playable area boundary?
            if (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellY >= playAreaSize.y - 1 ||
                // is the cell beneath the cell we are trying to cross occupied?
                (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellY + 1 <= playAreaSize.y - 1 &&
                mathModel[Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellY + 1][(int)pieceMathPos.x + xCellOffset] != null))
            {
                // if so set closest obstacle y to this cell y
                obstacleY = Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellY;
                break;
            }
            i += 1;
        }

        return obstacleY;
    }

    Vector3 GetPieceTopLeft(Transform piece)
    {
        Bounds bounds = piece.GetChild(piece.childCount - 2).GetComponent<SpriteRenderer>().bounds;

        return new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, 0);
    }

    Vector3 GetPieceTopLeftToPosCorrection()
    {
        return lastPiece.transform.position - GetPieceTopLeft(lastPiece);
    }

    Vector2 GlobalPos2MathModelPos(Vector3 pos)
    {
        return new Vector2(Mathf.Round((pos.x - playAreaTopLeft.x) / cellSize), -(pos.y - playAreaTopLeft.y) / cellSize);
    }


    Vector3 MathModelPos2GlobalPos(int x, int y)
    {
        return new Vector3(x * cellSize + playAreaTopLeft.x, -y * cellSize + playAreaTopLeft.y, 0);
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * (B - A) + A;
        return P;
    }

    float RoundTo(float x, float factor)
    {
        return Mathf.Round(x / factor) * factor;
    }

    Vector3 RoundXTo(Vector3 v, float factor)
    {
        v.x = RoundTo(v.x, factor);
        return v;
    }

    #endregion
}

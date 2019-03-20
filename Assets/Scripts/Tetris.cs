using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;



public class Tetris : MonoBehaviour
{
    public Transform[] pieces;

    //public float piecePeriod = 10.0f;
    public float pieceStartSpeed = 1.0f;
    public float pieceSpeed = 0.001f;
    public float floorHeight = 0.0f;
    public Vector3 spawnLeft;
    public Vector3 spawnRight;
    public float horizontalSpeed = 0.1f;

    public int score = 0;

    private Transform lastPiece = null;
    private List<Transform[]> mathModel;

    private Vector2Int playAreaSize;
    private Vector3 playAreaCenter;
    private Vector3 playAreaExtents;
    private Vector3 playAreaTopLeft;
    private float cellSize;

    public PieceDatabase pd;

    public int spawnCount = 1;

    void Start()
    {
        InitPieceDatabase();

        playAreaSize = GetComponent<TetrisPlayArea>().playableAreaSize;
        cellSize = GetComponent<TetrisPlayArea>().cellSize;

        mathModel = new List<Transform[]>(playAreaSize.y);
        for (int i = 0; i < playAreaSize.y; i++)
        {
            mathModel.Add(new Transform[playAreaSize.x]);
        }

        playAreaCenter = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.center;
        playAreaExtents = transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents;
        playAreaTopLeft = new Vector3(playAreaCenter.x - playAreaExtents.x, playAreaCenter.y + playAreaExtents.y, 0);

        //Debug.Log("" + transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.center);
        //Debug.Log("" + transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents);

        //foreach (var v in transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.bounds.)
        //{
        //    Debug.Log("" + v.x + " " + v.y);
        //}
    }

    void InitPieceDatabase()
    {

    }

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
        if (Time.time % 0.3 < 0.016f)
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
        //Transform pieceType = pieces[Random.Range(0, pieces.Length)];
        Transform pieceType = pieces[0];
        Vector2Int pieceSize = pieceType.GetComponent<PieceData>().pieceSize;

        // determine spawn position at the top of the playable zone 
        Debug.Log(playAreaSize.x - 1 - pieceSize.x);
        Vector3 piecePos = new Vector3(spawnLeft.x + Random.Range(0, playAreaSize.x - pieceSize.x + 1) * cellSize, spawnLeft.y, spawnLeft.z);

        //Vector3 piecePos;
        //piecePos = new Vector3(spawnLeft.x + spawnCount * 2 * cellSize, spawnLeft.y, spawnLeft.z);
        //spawnCount++;


        //        Transform pieceType = piecspawnCountes[1];

        //mathModel[]

        // instantiate piece
        lastPiece = Instantiate(pieceType, piecePos, Quaternion.identity);

        // UpdateMathModel(piecePos, lastPiece.GetComponent<PieceData>().pieceType);

        return lastPiece;
    }

    void MovePieceVertically()
    {
        PieceData pieceData = lastPiece.GetComponent<PieceData>();

        float currentSpeed = pieceSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput < -0.0001f)
        {
            currentSpeed *= 4;
        }

        MoveToFarthestFreePos(currentSpeed);

        //if (MoveToFarthestFreePos(currentSpeed))
        //{
        //    // lastPiece.y = floorHeight + pieceSize.y * cellSize;
        //    Debug.Log("lower");
        //    lastPiece.transform.position = new Vector3(lastPiece.transform.position.x, floorHeight + pieceData.pieceSize.y * cellSize, lastPiece.transform.position.z);

        //    UpdateMathModel();

        //    lastPiece = null;
        //}
    }

    void MovePieceHorizontally(float horizontalInput)
    {
        Vector2 mathModelPos = GlobalPos2MathModelPos(lastPiece.transform.position);
        PieceData pieceData = lastPiece.GetComponent<PieceData>();
        bool canMove = true;

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
            UpdateMathModel();
        }
    }

    void RotatePiece()
    {
        PieceData pieceData = lastPiece.GetComponent<PieceData>();
        bool canRotate = true;

        switch (pieceData.pieceType)
        {
            case 0:
                canRotate = true;

                if (canRotate)
                {
                    GetComponent<AudioSource>().Play();
                    lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, -90);
                    pieceData.rotationVariation += 1;
                    if (pieceData.rotationVariation % 4 == 0)
                    {
                        pieceData.rotationVariation = 0;
                    }
                }
                break;
            case 1:
                canRotate = true;

                if (canRotate)
                {
                    GetComponent<AudioSource>().Play();
                    if (pieceData.rotationVariation == 0)
                    {
                        pieceData.rotationVariation = 1;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, -90);
                    }
                    else
                    {
                        pieceData.rotationVariation = 0;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, 90);
                    }
                }
                break;
            case 2:
                canRotate = true;

                if (canRotate)
                {
                    GetComponent<AudioSource>().Play();
                    lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, -90);
                    pieceData.rotationVariation += 1;
                    if (pieceData.rotationVariation % 4 == 0)
                    {
                        pieceData.rotationVariation = 0;
                    }
                }
                break;
            case 3:
                canRotate = true;

                if (canRotate)
                {
                    GetComponent<AudioSource>().Play();
                    if (pieceData.rotationVariation == 0)
                    {
                        pieceData.rotationVariation = 1;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, -90);
                    }
                    else
                    {
                        pieceData.rotationVariation = 0;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, 90);
                    }
                }
                break;
            case 4:
                canRotate = false;
                GetComponent<AudioSource>().Play();
                break;
            case 5:
                canRotate = true;

                if (canRotate)
                {
                    GetComponent<AudioSource>().Play();
                    if (pieceData.rotationVariation == 0)
                    {
                        pieceData.rotationVariation = 1;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, -90);
                    }
                    else
                    {
                        pieceData.rotationVariation = 0;
                        lastPiece.transform.RotateAround(lastPiece.GetChild(lastPiece.childCount - 1).position, Vector3.forward, 90);
                    }
                }
                break;
            default:
                Debug.Log($"RotatePiece pieceType unknown {pieceData.pieceType}");
                break;
        }
    }

    void MoveToFarthestFreePos(float speed)
    {
        PieceData pieceData = lastPiece.GetComponent<PieceData>();

        int type = pieceData.pieceType * 10 + pieceData.rotationVariation;
        int obstacleY = 10000;
        int closestObstacleY = obstacleY;
        int j = 0;
        Vector2 pieceMathPos;
        int bottomCellOffset;

        switch (type)
        {
            case 0:
                pieceMathPos = GlobalPos2MathModelPos(GetPieceTopLeft());
                bottomCellOffset = 1;

                while (j < 3)
                {
                    obstacleY = FindFathestFreeCellY(pieceMathPos, bottomCellOffset, j, speed);

                    // save the closest obstacle y of all found
                    if (closestObstacleY > obstacleY)
                    {
                        closestObstacleY = obstacleY;
                    }
                    j += 1;
                }

                if (closestObstacleY != 10000)
                {
                    lastPiece.transform.position = MathModelPos2GlobalPos((int)pieceMathPos.x, closestObstacleY - bottomCellOffset);

                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x] = lastPiece.GetChild(0);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x] = lastPiece.GetChild(1);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x + 1] = lastPiece.GetChild(2);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x + 2] = lastPiece.GetChild(3);

                    lastPiece = null;
                }
                else
                {
                    lastPiece.transform.Translate(0, -speed, 0, Space.World);
                }

                break;
            case 1:
                pieceMathPos = GlobalPos2MathModelPos(GetPieceTopLeft());
                bottomCellOffset = 2;

                obstacleY = FindFathestFreeCellY(pieceMathPos, 2, 0, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 2;
                }

                obstacleY = FindFathestFreeCellY(pieceMathPos, 0, 1, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 0;
                }

                if (closestObstacleY != 10000)
                {
                    lastPiece.transform.position = GetPieceTopLeftToPosCorrection() + MathModelPos2GlobalPos((int)pieceMathPos.x, closestObstacleY - bottomCellOffset);

                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x] = lastPiece.GetChild(0);
                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x + 1] = lastPiece.GetChild(1);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x] = lastPiece.GetChild(2);
                    mathModel[closestObstacleY - bottomCellOffset + 2][(int)pieceMathPos.x] = lastPiece.GetChild(3);

                    lastPiece = null;
                }
                else
                {
                    lastPiece.transform.Translate(0, -speed, 0, Space.World);
                }
                break;
            case 2:
                pieceMathPos = GlobalPos2MathModelPos(GetPieceTopLeft());
                bottomCellOffset = 0;

                obstacleY = FindFathestFreeCellY(pieceMathPos, 0, 0, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 0;
                }

                obstacleY = FindFathestFreeCellY(pieceMathPos, 0, 1, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 0;
                }

                obstacleY = FindFathestFreeCellY(pieceMathPos, 1, 2, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 1;
                }

                if (closestObstacleY != 10000)
                {
                    lastPiece.transform.position = GetPieceTopLeftToPosCorrection() + MathModelPos2GlobalPos((int)pieceMathPos.x, closestObstacleY - bottomCellOffset);

                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x] = lastPiece.GetChild(0);
                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x + 1] = lastPiece.GetChild(1);
                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x + 2] = lastPiece.GetChild(2);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x + 2] = lastPiece.GetChild(3);

                    lastPiece = null;
                }
                else
                {
                    lastPiece.transform.Translate(0, -speed, 0, Space.World);
                }
                break;
            case 3:
                pieceMathPos = GlobalPos2MathModelPos(GetPieceTopLeft());
                bottomCellOffset = 2;

                obstacleY = FindFathestFreeCellY(pieceMathPos, 2, 0, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 2;
                }

                obstacleY = FindFathestFreeCellY(pieceMathPos, 2, 1, speed);
                if (closestObstacleY > obstacleY)
                {
                    closestObstacleY = obstacleY;
                    bottomCellOffset = 2;
                }

                if (closestObstacleY != 10000)
                {
                    lastPiece.transform.position = GetPieceTopLeftToPosCorrection() + MathModelPos2GlobalPos((int)pieceMathPos.x, closestObstacleY - bottomCellOffset);

                    mathModel[closestObstacleY - bottomCellOffset][(int)pieceMathPos.x + 1] = lastPiece.GetChild(0);
                    mathModel[closestObstacleY - bottomCellOffset + 1][(int)pieceMathPos.x + 1] = lastPiece.GetChild(1);
                    mathModel[closestObstacleY - bottomCellOffset + 2][(int)pieceMathPos.x] = lastPiece.GetChild(2);
                    mathModel[closestObstacleY - bottomCellOffset + 2][(int)pieceMathPos.x + 1] = lastPiece.GetChild(3);

                    lastPiece = null;
                }
                else
                {
                    lastPiece.transform.Translate(0, -speed, 0, Space.World);
                }
                break;
            case 10:
                break;
            case 11:
                break;
            case 20:
                break;
            case 21:
                break;
            case 22:
                break;
            case 23:
                break;
            case 30:
                break;
            case 31:
                break;
            case 40:
                break;
            case 50:
                break;
            case 51:
                break;
            default:
                break;
        }
    }

    void UpdateMathModel()
    {
        //Vector3 pos = lastPiece.transform.position;
        //int pieceType = lastPiece.GetComponent<PieceData>().pieceType;

        //Vector2Int mathModelPos = GlobalPos2MathModelPos(pos);
        //switch (pieceType)
        //{
        //    case 0:
        //        mathModel[mathModelPos.x][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x][mathModelPos.y + 1] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y + 1] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x + 2][mathModelPos.y + 1] = lastPiece.GetChild(3);
        //        break;
        //    case 1:
        //        mathModel[mathModelPos.x][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x][mathModelPos.y + 1] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x][mathModelPos.y + 2] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x][mathModelPos.y + 3] = lastPiece.GetChild(3);
        //        break;
        //    case 2:
        //        mathModel[mathModelPos.x + 1][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x][mathModelPos.y + 1] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y + 1] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x + 2][mathModelPos.y + 1] = lastPiece.GetChild(3);
        //        break;
        //    case 3:
        //        mathModel[mathModelPos.x + 1][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x + 2][mathModelPos.y] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x][mathModelPos.y + 1] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y + 1] = lastPiece.GetChild(3);
        //        break;
        //    case 4:
        //        mathModel[mathModelPos.x][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x][mathModelPos.y + 1] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y + 1] = lastPiece.GetChild(3);
        //        break;
        //    case 5:
        //        mathModel[mathModelPos.x][mathModelPos.y] = lastPiece.GetChild(0);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y] = lastPiece.GetChild(1);
        //        mathModel[mathModelPos.x + 1][mathModelPos.y + 1] = lastPiece.GetChild(2);
        //        mathModel[mathModelPos.x + 2][mathModelPos.y + 1] = lastPiece.GetChild(3);
        //        break;
        //    default:
        //        Debug.Log($"PieceType {pieceType} unknown");
        //        break;
        //}
    }

    private void UpdateMathModelRowDestroyed(int rowNum)
    {
        mathModel.RemoveAt(rowNum);

        mathModel.Insert(0, new Transform[playAreaSize.x]);

        for (int i = 0; i < mathModel.Count; i++)
        {
            for (int j = 0; j < mathModel[i].Length; j++)
            {
                if (mathModel[i][j])
                {
                    mathModel[i][j].position = MathModelPos2GlobalPos(i, j);
                }
            }
        }
    }

    // returns closeset obstacle y in math model coordinates
    int FindFathestFreeCellY(Vector2 pieceMathPos, int bottomCellOffset, int xCellOffset, float speed)
    {
        int obstacleY = 10000;
        int i = 0;
        // does the lowest point of part of the piece (column j) crosses the virtual grid border?
        while (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellOffset < pieceMathPos.y + speed / cellSize + bottomCellOffset)
        {
            // is the lowest point of part of the piece (column j) lower than playable area boundary?
            if (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellOffset >= playAreaSize.y - 1 ||
                // is the cell beneath the cell we are trying to cross occupied?
                (Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellOffset + 1 <= playAreaSize.y - 1 &&
                mathModel[Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellOffset + 1][(int)pieceMathPos.x + xCellOffset] != null))
            {
                // if so set closest obstacle y to this cell y
                obstacleY = Mathf.CeilToInt(pieceMathPos.y) + i + bottomCellOffset;
                break;
            }
            i += 1;
        }

        return obstacleY;
    }

    Vector3 GetPieceTopLeft()
    {
        //Bounds bounds = lastPiece.GetComponent<BoxCollider2D>().bounds;

        //return new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, 0);

        Bounds bounds = lastPiece.transform.GetChild(lastPiece.childCount - 2).GetComponent<SpriteRenderer>().bounds;

        return new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, 0);
    }

    Vector3 GetPieceTopLeftToPosCorrection()
    {
        return lastPiece.transform.position - GetPieceTopLeft();
    }

    //Vector2Int GlobalPos2MathModelPos(Vector3 pos)
    //{
    //    return new Vector2Int( (int)Mathf.Round((pos.x - playAreaTopLeft.x) / cellSize), -(int)Mathf.Round((pos.y - playAreaTopLeft.y) / cellSize) );
    //}

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
}

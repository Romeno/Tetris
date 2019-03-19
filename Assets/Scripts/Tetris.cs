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

    void Start()
    {
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
        Transform pieceType = pieces[Random.Range(0, pieces.Length)];
        Vector2Int pieceSize = pieceType.GetComponent<PieceData>().pieceSize;

        // determine spawn position at the top of the playable zone 
        Debug.Log(playAreaSize.x - 1 - pieceSize.x);
        Vector3 piecePos = new Vector3(spawnLeft.x + Random.Range(0, playAreaSize.x - pieceSize.x + 1) * cellSize, spawnLeft.y, spawnLeft.z);
        piecePos = RoundXTo(piecePos, cellSize);

//        Transform pieceType = pieces[1];

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

        lastPiece.transform.Translate(0, -currentSpeed, 0, Space.World);

        if (CheckPieceColision())
        {
            // lastPiece.y = floorHeight + pieceSize.y * cellSize;
            Debug.Log("lower");
            lastPiece.transform.position = new Vector3(lastPiece.transform.position.x, floorHeight + pieceData.pieceSize.y * cellSize, lastPiece.transform.position.z);

            UpdateMathModel();

            lastPiece = null;
        }
    }

    void MovePieceHorizontally(float horizontalInput)
    {
        Vector2Int mathModelPos = GlobalPos2MathModelPos(lastPiece.transform.position);
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

    bool CheckPieceColision()
    {
        PieceData pieceData = lastPiece.GetComponent<PieceData>();

        switch (pieceData.pieceType)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            case 4:
                break;
            case 5:
                break;
            default:
                break;
        }

        return lastPiece.transform.position.y - pieceData.pieceSize.y * cellSize - floorHeight < 0.001f;
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

    Vector2Int GlobalPos2MathModelPos(Vector3 pos)
    {
        return new Vector2Int( (int)Mathf.Round((pos.x - playAreaTopLeft.x) / cellSize), -(int)Mathf.Round((pos.y - playAreaTopLeft.y) / cellSize) );
    }

    Vector3 MathModelPos2GlobalPos(int x, int y)
    {
        return new Vector3((x  + playAreaTopLeft.x) * cellSize, (-y + playAreaTopLeft.y) * cellSize, 0);
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

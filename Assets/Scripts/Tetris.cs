using System.Collections;
using System.Collections.Generic;
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
    private int[,] mathModel;

    private Vector2Int playAreaSize;
    private Vector3 playAreaCenter;
    private Vector3 playAreaExtents;
    private Vector3 playAreaTopLeft;
    private float cellSize;

    void Start()
    {
        playAreaSize = GetComponent<TetrisPlayArea>().playableAreaSize;
        cellSize = GetComponent<TetrisPlayArea>().cellSize;

        mathModel = new int[playAreaSize.x, playAreaSize.y];

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
                lastPiece.transform.Translate(Mathf.Sign(horizontalInput) * horizontalSpeed, 0, 0, Space.World);
            }
        }

        // rotate piece responding to player input
        if (Input.GetButtonDown("Rotate"))
        {
            lastPiece.transform.Rotate(0, 0, 90, Space.World);
        }
    }

    Transform SpawnPiece()
    {
        // determine piece type
        Transform pieceType = pieces[(int)Random.Range(0, pieces.Length - 0.001f)];
        Vector2Int pieceSize = pieceType.GetComponent<PieceData>().pieceSize;

        // determine spawn position at the top of the playable zone 
        Vector3 piecePos = LerpByDistance(spawnLeft, new Vector2(spawnRight.x - pieceSize.x * cellSize, spawnRight.y), Random.value);
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
        Vector2Int pieceSize = lastPiece.GetComponent<PieceData>().pieceSize;

        float currentSpeed = pieceSpeed;
        float verticalInput = Input.GetAxisRaw("Vertical");

        if (verticalInput < -0.0001f)
        {
            currentSpeed *= 4;
        }

        lastPiece.transform.Translate(0, -currentSpeed, 0, Space.World);

        if (lastPiece.transform.position.y - pieceSize.y * cellSize - floorHeight < 0.001f)
        {
            UpdateMathModel(lastPiece.transform.position, lastPiece.GetComponent<PieceData>().pieceType);
            lastPiece = null;
        }
    }

    void UpdateMathModel(Vector3 pos, int pieceType)
    {
        //Vector2Int mathModelPos = GlobalPos2MathModelPos(pos);
        //switch (pieceType)
        //{
        //    case 0:
        //        mathModel[mathModelPos.x, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 1, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 2, mathModelPos.y + 1] = 1;
        //        break;
        //    case 1:
        //        mathModel[mathModelPos.x, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 2] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 3] = 1;
        //        break;
        //    case 2:
        //        mathModel[mathModelPos.x + 1, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 1, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 2, mathModelPos.y + 1] = 1;
        //        break;
        //    case 3:
        //        mathModel[mathModelPos.x + 1, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x + 2, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 1, mathModelPos.y + 1] = 1;
        //        break;
        //    case 4:
        //        mathModel[mathModelPos.x, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x, mathModelPos.y + 1] = 1;
        //        mathModel[mathModelPos.x + 1, mathModelPos.y] = 1;
        //        mathModel[mathModelPos.x + 1, mathModelPos.y + 1] = 1;
        //        break;
        //    default:
        //        Debug.Log($"PieceType {pieceType} unknown");
        //        break;
        //}
    }

    Vector2Int GlobalPos2MathModelPos(Vector3 pos)
    {
        return new Vector2Int( (int)Mathf.Round((pos.x - playAreaTopLeft.x) / cellSize), -(int)Mathf.Round((pos.y - playAreaTopLeft.y) / cellSize) );
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

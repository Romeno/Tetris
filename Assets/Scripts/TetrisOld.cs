using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TetrisOld : MonoBehaviour
{
    public Transform[] pieces;

    //public float piecePeriod = 10.0f;
    public float pieceStartSpeed = 1.0f;
    public float pieceSpeed = 0.001f;
    public float floorHeight = 0.0f;
    public Vector3 spawnLeft;
    public Vector3 spawnRight;
    public float horizontalSpeed = 0.1f;

    private Transform lastPiece = null;

    

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if ((int)Time.time == spawnTime)
        //{
        //    spawnTime += spawnPeriod;
        //}
        if (lastPiece == null)
        {
            Vector3 piecePos = LerpByDistance(spawnLeft, spawnRight, Random.value);
            piecePos = RoundXTo(piecePos, 0.25f);

            Transform pieceType = pieces[(int)Random.Range(0, pieces.Length - 0.001f)];
            //Transform pieceType = pieces[1];
            PieceData pieceTypeData = pieceType.GetComponent<PieceData>();

            piecePos.x = piecePos.x + pieceTypeData.spawnOffset.x;
            piecePos.y = piecePos.y - pieceTypeData.spawnOffset.y;

            lastPiece = Instantiate(pieceType, piecePos, Quaternion.identity);
        }
        else
        {
            float currentSpeed = pieceSpeed;
            float verticalInput = Input.GetAxisRaw("Vertical");

            if (verticalInput < -0.0001f)
            {
                currentSpeed *= 4;
            }

            lastPiece.transform.Translate(0, -currentSpeed, 0, Space.World);

            if (lastPiece.transform.position.y - floorHeight < 0.001f )
            {
                lastPiece = null;
            }
        }

        if (Time.time % 0.3 < 0.016f)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");

            if (Mathf.Abs(horizontalInput) > 0.0001f)
            {
                lastPiece.transform.Translate(Mathf.Sign(horizontalInput) * horizontalSpeed, 0, 0, Space.World);
            }
        }

        if (Input.GetButtonDown("Rotate"))
        {
            Debug.Log("Rotate");
            lastPiece.transform.Rotate(0, 0, 90, Space.World);
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TetrisPeriodicSpawn : MonoBehaviour
{
    public Transform[] pieces;

    //public float piecePeriod = 10.0f;
    public float pieceStartSpeed = 1.0f;
    public float pieceSpeed = 1.0f;
    public float floorHeight = 0.0f;
    public Vector3 spawnLeft;
    public Vector3 spawnRight;

    private int spawnTime = 0;
    private int spawnPeriod = 3;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Time.time == spawnTime)
        {
            spawnTime += spawnPeriod;
            Instantiate(pieces[(int)Random.Range(0, pieces.Length - 0.001f)], LerpByDistance(spawnLeft, spawnRight, Random.value), Quaternion.identity);
        }
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }
}

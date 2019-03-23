using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceRotation
{
    public float rotation;
    public int[,] cells;
    public Vector2Int[] bottomCells;
    public Vector2Int[] leftCells;
    public Vector2Int[] rightCells;
}

[CreateAssetMenu(fileName = "Piece", menuName = "Tetris/Create Piece", order = 1)]
public class PieceType : ScriptableObject
{
    public int id;

    public float[] rotations;

    [System.NonSerialized]
    public PieceRotation[] pieceRotations;

    public GameObject prefab;
}

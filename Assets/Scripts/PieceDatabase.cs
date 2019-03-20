using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Piece Database", menuName = "Tetris/Create piece database", order = 1)]
public class PieceDatabase : ScriptableObject
{
    public int test;

    private void Awake()
    {
        Debug.Log("PieceDatabase awake");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Piece Database", menuName = "Tetris/Create Piece Database", order = 1)]
public class PieceDatabase : ScriptableObject
{
    // array where PieceType.id is a key
    public PieceType[] pieceTypes;
}

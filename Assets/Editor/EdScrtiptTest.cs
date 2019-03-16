using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//[CustomEditor(typeof(Tetris))]
//public class EdScrtiptTest : Editor
//{
//    SerializedProperty playableAreaProp;

//    void OnEnable()
//    {
//        // Setup the SerializedProperties.
//        playableAreaProp = serializedObject.FindProperty("pieceStartSpeed");
//    }

//    public override void OnInspectorGUI()
//    {
//        var tetris = target as Tetris;

//        DrawDefaultInspector();

//        EditorGUI.BeginChangeCheck();
//        EditorGUILayout.Slider(playableAreaProp, 0, 100, "Start Speed");
//        serializedObject.ApplyModifiedProperties();
//        if (EditorGUI.EndChangeCheck())
//        {
//            Debug.Log("Changed");
//        }

//        GUILayoutUtility.GetRect(18, 18, "TextField");

//        EditorGUILayout.LabelField("HUI", "Pizda");
//        EditorGUI.BeginChangeCheck();
//        EditorGUILayout.IntField("Hui!!!", 10);
//        if (EditorGUI.EndChangeCheck())
//        {
//            Transform pieceType = tetris.pieces[1];
//            //Instantiate(pieceType, new Vector3(0, 0, 0), Quaternion.identity);

//            //foreach (Transform child in tetris.transform)
//            //{
//            Transform bg = tetris.transform.GetChild(0);
//            SpriteRenderer grid = tetris.transform.GetChild(1).GetComponent<SpriteRenderer>();
//            bg.localScale = new Vector3(bg.localScale.x + 1, bg.localScale.y, bg.localScale.z);
//            grid.size = new Vector2(grid.size.x + 1, grid.size.y);
//            //}
//        }
//    }
//}

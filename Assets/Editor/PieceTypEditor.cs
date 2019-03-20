using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[CustomEditor(typeof(PieceType))]
//[CanEditMultipleObjects]
//public class PieceTypEditor : Editor
//{
//    SerializedProperty prefabProp;
//    SerializedProperty rotationsProp;
//    SerializedProperty typeProp;

//    void OnEnable()
//    {
//        // Setup the SerializedProperties.
//        prefabProp = serializedObject.FindProperty("prefab");
//        rotationsProp = serializedObject.FindProperty("rotationsProp");
//        typeProp = serializedObject.FindProperty("type");
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();

//        //EditorGUI.BeginChangeCheck();

//        EditorGUILayout.PropertyField(typeProp);
//        EditorGUILayout.PropertyField(rotationsProp);
//        EditorGUILayout.PropertyField(prefabProp);

//        //if (EditorGUI.EndChangeCheck())
//        //{
//        //    var tetris = target as TetrisPlayArea;

//        //    var bg = tetris.transform.GetChild(0);
//        //    var grid = tetris.transform.GetChild(1);

//        //    bg.transform.localScale = new Vector3(playableAreaProp.vector2IntValue.x * cellSizeProp.floatValue, playableAreaProp.vector2IntValue.y * cellSizeProp.floatValue, 1);
//        //    grid.GetComponent<SpriteRenderer>().size = new Vector2(playableAreaProp.vector2IntValue.x * cellSizeProp.floatValue, playableAreaProp.vector2IntValue.y * cellSizeProp.floatValue);
//        //}

//        serializedObject.ApplyModifiedProperties();
//    }
//}
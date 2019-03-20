using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TetrisPlayArea))]
[CanEditMultipleObjects]
public class TetrisPlayAreaEditor : Editor
{
    SerializedProperty playableAreaProp;
    SerializedProperty cellSizeProp;

    private void PrefabUpdated(GameObject go)
    {
        Debug.Log(go + " updated");
    }

    void OnEnable()
    {
        // Setup the SerializedProperties.
        playableAreaProp = serializedObject.FindProperty("playableAreaSize");
        cellSizeProp = serializedObject.FindProperty("cellSize");

        PrefabUtility.prefabInstanceUpdated = PrefabUpdated;

        Debug.Log("Enable");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(playableAreaProp);
        EditorGUILayout.PropertyField(cellSizeProp);
        if (EditorGUI.EndChangeCheck())
        {
            var tetris = target as TetrisPlayArea;

            var bg = tetris.transform.GetChild(0);
            var grid = tetris.transform.GetChild(1);

            bg.transform.localScale = new Vector3(playableAreaProp.vector2IntValue.x * cellSizeProp.floatValue, playableAreaProp.vector2IntValue.y * cellSizeProp.floatValue, 1);
            grid.GetComponent<SpriteRenderer>().size = new Vector2(playableAreaProp.vector2IntValue.x * cellSizeProp.floatValue, playableAreaProp.vector2IntValue.y * cellSizeProp.floatValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

}
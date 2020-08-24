using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

[CustomEditor(typeof(WheelController))]
public class EditorController : Editor
{
    private WheelController _wheelController;
    private void OnEnable()
    {
        _wheelController = (WheelController)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var sectors = _wheelController._sectors;
        if (sectors.Count > 0)
        {
            for (int i = 0; i < sectors.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("X", GUILayout.Width(15), GUILayout.Height(15)))
                {
                    _wheelController.DeleteSectoreObj(i);
                    break;
                }
                EditorGUILayout.EndHorizontal();
                sectors[i]._text = EditorGUILayout.TextField("UItext", sectors[i]._text);
                sectors[i]._amountWin = EditorGUILayout.IntField("WinAmount", sectors[i]._amountWin);
                sectors[i]._changeWin = EditorGUILayout.IntField("ChangeWin", sectors[i]._changeWin);
                sectors[i]._autoPos = EditorGUILayout.Toggle("AutoPos", sectors[i]._autoPos);
                sectors[i]._autoRot = EditorGUILayout.Toggle("AutoRot", sectors[i]._autoRot);
                sectors[i]._posXsprite = EditorGUILayout.FloatField("XposSprire", sectors[i]._posXsprite);
                sectors[i]._posYsprite = EditorGUILayout.FloatField("YposSprire", sectors[i]._posYsprite);
                sectors[i]._posXtext = EditorGUILayout.FloatField("XposText", sectors[i]._posXtext);
                sectors[i]._posYtext = EditorGUILayout.FloatField("YposText", sectors[i]._posYtext);
                sectors[i]._sectorOffset = EditorGUILayout.FloatField("sectorOffset", sectors[i]._sectorOffset);
                sectors[i]._spriteObj = (GameObject)EditorGUILayout.ObjectField("SpriteObj",sectors[i]._spriteObj,typeof(GameObject),true);
                sectors[i]._textObj = (GameObject)EditorGUILayout.ObjectField("TextObj",sectors[i]._textObj,typeof(GameObject),true);
                sectors[i]._enum = (MethodsEnumName) EditorGUILayout.EnumPopup("MethodWin", sectors[i]._enum);
                EditorGUILayout.EndVertical();
            }
        }
        
        if (GUILayout.Button("Add Sector"))
        {            
              _wheelController.SpawnSectorObj();
        }

        if (GUI.changed)
        {
            SetObjectsDirty(_wheelController.gameObject);
            _wheelController.OnGuiChange();
            serializedObject.ApplyModifiedProperties();
        }
    }

    public static void SetObjectsDirty(GameObject obj)
    {
        EditorUtility.SetDirty(obj);
        EditorSceneManager.MarkSceneDirty(obj.scene);
    }
}

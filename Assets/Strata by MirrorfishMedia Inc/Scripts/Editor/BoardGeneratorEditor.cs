using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace Strata
{

    //Simple Custom Editor for BoardGenerator that adds the button needed to generate in the scene view without entering playmode
    [CustomEditor(typeof(BoardGenerator))]
    public class BoardGeneratorEditor : Editor
    {

        private bool usingRandomSeed;
        BoardGenerator boardGenerator;
        SerializedObject boardGeneratorObject;
        SerializedProperty tilemapProperty;
        SerializedProperty boardGenerationProfileProperty;

        private void OnEnable()
        {
            boardGenerator = (BoardGenerator)target;
            boardGeneratorObject = new SerializedObject(boardGenerator);
            tilemapProperty = boardGeneratorObject.FindProperty("tilemap");
            boardGenerationProfileProperty = boardGeneratorObject.FindProperty("boardGenerationProfile");
        }

        public override void OnInspectorGUI()
        {

            

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            boardGenerator.buildOnStart = EditorGUILayout.Toggle("Build On Start", boardGenerator.buildOnStart);
            GUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();

            boardGenerator.randomSeed = EditorGUILayout.Toggle("Use Random Seed", boardGenerator.randomSeed);
            GUILayout.EndHorizontal();


            usingRandomSeed = boardGenerator.randomSeed;

            if (usingRandomSeed)
            {

            }
            else
            {
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();

                boardGenerator.useDailySeed = EditorGUILayout.Toggle("Use Daily Seed", boardGenerator.useDailySeed);
                GUILayout.EndHorizontal();
            }
            
            EditorGUILayout.PropertyField(tilemapProperty, new GUIContent("Tilemap"));
            EditorGUILayout.PropertyField(boardGenerationProfileProperty, new GUIContent("Board Generation Profile"));


            if (GUILayout.Button("Clear And Generate"))
            {
                boardGenerator.ClearAndRebuild();
            }

            if (GUILayout.Button("Clear"))
            {
                boardGenerator.ClearLevel();
            }

            boardGeneratorObject.ApplyModifiedProperties();
        }
    }

}


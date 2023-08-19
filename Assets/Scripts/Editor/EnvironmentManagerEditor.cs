using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(EnvironmentManager))]
public class EnvironmentManagerEditor : Editor
{
    SerializedProperty maxIterations;
    SerializedProperty currentIteration;
    SerializedProperty timeScaleMult;
    SerializedProperty maxTimer;
    SerializedProperty elapsedTime;
    SerializedProperty isSimulationActive;
    SerializedProperty teamManagers;

    SerializedProperty testListProperty;
    ReorderableList testList;

    private void OnEnable()
    {
        maxIterations = serializedObject.FindProperty("maxIterations");
        currentIteration = serializedObject.FindProperty("currentIteration");
        timeScaleMult = serializedObject.FindProperty("timeScaleMultiplier");
        maxTimer = serializedObject.FindProperty("maxTimer");
        elapsedTime = serializedObject.FindProperty("elapsedTime");
        isSimulationActive = serializedObject.FindProperty("isSimulationActive");
        teamManagers = serializedObject.FindProperty("teamManagers");


        testListProperty = serializedObject.FindProperty("automatedTests");

        testList = new ReorderableList(serializedObject, testListProperty, true, true, true, true);

        testList.drawElementCallback = DrawListItems;
        testList.drawHeaderCallback = DrawHeader;
    }

    void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = testList.serializedProperty.GetArrayElementAtIndex(index);

        var originalWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 15;
        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight),
            element,
            new GUIContent((index + 1).ToString() + ". ")
            );
        EditorGUIUtility.labelWidth = originalWidth;
        //EditorGUI.PropertyField(
        //    new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight),
        //    element,
        //    GUIContent.none
        //    );
    }

    void DrawHeader(Rect rect)
    {
        string name = "Automated Tests";
        EditorGUI.LabelField(rect, name);
    }

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.Space();

        maxIterations.intValue = EditorGUILayout.IntField("Max Iterations", maxIterations.intValue);
        EditorGUILayout.PropertyField(currentIteration);
        timeScaleMult.floatValue = EditorGUILayout.FloatField("Time Scale Multiplier", timeScaleMult.floatValue);
        EditorGUILayout.PropertyField(maxTimer);
        EditorGUILayout.PropertyField(elapsedTime);
        EditorGUILayout.PropertyField(isSimulationActive);
        EditorGUILayout.PropertyField(teamManagers);

        testList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestContainerEditor : Editor
{
    [MenuItem("Assets/Create/Create New Test List from Selection", priority = 0)]
    private static void CreateNewTestList()
    {
        TestContainer asset = ScriptableObject.CreateInstance<TestContainer>();
        string name = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/TestLists/NewScripableObject.asset");
        foreach (Object o in Selection.objects)
        {
            if (o.GetType() == typeof(TestSetup))
            {
                asset.testSetups.Add((TestSetup)o);
            }
        }

        AssetDatabase.CreateAsset(asset, name);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}

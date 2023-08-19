using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestContainer : ScriptableObject
{
    public string folderName = "";
    public List<TestSetup> testSetups = new List<TestSetup>();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvailableTestCategories : MonoBehaviour
{
    public AgentUIManager agentUIManager;
    public UIManager uiManager;

    public RectTransform contentList;
    public RectTransform testList;
    public GameObject testCardPrefab;
    public Button buttonPrefab;

    public List<TestContainer> testCategories = new List<TestContainer>();
    private void Awake()
    {
        foreach (var category in testCategories)
        {
            var newButton = Instantiate(buttonPrefab, contentList);
            newButton.onClick.AddListener(delegate { OnTestCategoryClick(category); });
            newButton.GetComponentInChildren<Text>().text = category.name;
        }
    }

    public void OnTestCategoryClick(TestContainer container)
    {
        Debug.Log(container);
        EnvironmentManager.instance.automatedTests = container.testSetups;

        uiManager.OnStartSimulation();

        for (int i = 0; i < testList.childCount; i++)
        {
            Destroy(testList.GetChild(i).gameObject);
        }
        foreach (var test in container.testSetups)
        {
            var newButton = Instantiate(testCardPrefab, testList).GetComponentInChildren<Button>();
            newButton.onClick.AddListener(delegate { agentUIManager.OnTestClick(test); });
            newButton.GetComponentInChildren<Text>().text = test.name;
        }
    }
}

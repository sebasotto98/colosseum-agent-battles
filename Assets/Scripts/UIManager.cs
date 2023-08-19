using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    EnvironmentManager envManager;
    public AgentUIManager agentUI;

    [SerializeField]
    RectTransform envSidePanelTransform;
    bool isEnvMenuExpanded = true;

    [SerializeField]
    RectTransform agentSidePanelTransform;
    bool isAgentMenuExpanded = true;

    [SerializeField]
    RectTransform testBottomPanelTransform;
    bool isTestMenuExpanded = true;

    [SerializeField]
    GameObject startButton;
    [SerializeField]
    GameObject restartButton;


    [SerializeField]
    Dropdown agentTypeDropdown;
    [SerializeField]
    InputField agentColorInputField;
    public List<AgentCharacteristics> agentTypes;
    [SerializeField]
    private AgentCharacteristics selectedAgentType;

    private void Awake()
    {
        selectedAgentType = agentTypes[0];

        ColorUtility.ToHtmlStringRGB(selectedAgentType.materialColor);
        agentColorInputField.text = ColorUtility.ToHtmlStringRGB(selectedAgentType.materialColor);
    }

    public void OnMaxIterationsChange(string input)
    {
        int value = int.Parse(input);

        if(value > 0)
            envManager.SetMaxIterations(value);
    }

    public void OnTimeScaleChange(string input)
    {
        float value = float.Parse(input);

        if (value > 0)
            envManager.SetTimeScaleMultiplier(value);
    }

    public void OnEnvExpand()
    {
        float widthFraction = Screen.width / envSidePanelTransform.rect.width;

        if(isEnvMenuExpanded)
        {
            isEnvMenuExpanded = false;
            envSidePanelTransform.anchoredPosition = new Vector2(-(Screen.width / widthFraction), 0);

        }
        else if(!isEnvMenuExpanded)
        {
            isEnvMenuExpanded = true;
            envSidePanelTransform.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void OnAgentExpand()
    {
        float widthFraction = Screen.width / agentSidePanelTransform.rect.width;

        if (isAgentMenuExpanded)
        {
            isAgentMenuExpanded = false;
            agentSidePanelTransform.anchoredPosition = new Vector2((Screen.width / widthFraction), 0);

        }
        else if (!isAgentMenuExpanded)
        {
            isAgentMenuExpanded = true;
            agentSidePanelTransform.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void OnTestContainersExpand()
    {
        float heigthFraction = Screen.height / testBottomPanelTransform.rect.height;

        if (isTestMenuExpanded)
        {
            isTestMenuExpanded = false;
            testBottomPanelTransform.anchoredPosition = new Vector2(0, -(Screen.height / heigthFraction));

        }
        else if (!isTestMenuExpanded)
        {
            isTestMenuExpanded = true;
            testBottomPanelTransform.anchoredPosition = new Vector2(0, 0);
        }
    }

    public void OnStartSimulation()
    {
        envManager.StartSimulation();
     
        if(envManager.isSimulationActive)
        {
            startButton.SetActive(false);
            restartButton.SetActive(true);
        }
    }

    public void OnSelectAgentType()
    {
        agentUI.teamID = agentTypeDropdown.value;
    }

    public void ChangeAgentColor(string input)
    {
        Regex r = new Regex("^#(([0-9a-fA-F]{2}){3}|([0-9a-fA-F]){3})$");

        Match m = r.Match(input);
        if(m.Success)
        {
            Debug.Log("Matched: " + input);
            Color newColor;
            bool parsed = ColorUtility.TryParseHtmlString(input, out newColor);

            Debug.Log("Parsed? " + parsed);
            selectedAgentType.materialColor = newColor;
        }

        envManager.ReaplyCharacteristics();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentUIManager : MonoBehaviour
{
    public Text currentTestName;

    public InputField agentType;
    public InputField agentCount;

    public InputField winRatio;
    public InputField avgTimeWin;
    public InputField avgInflictedDmg;
    public InputField avgSurvivalTime;
    public InputField avgSufferedLosses;

    private TestSetup selectedTest;
    public int teamID;

    private void Awake()
    {
        if(EnvironmentManager.instance.automatedTests.Count > 0)
            selectedTest = EnvironmentManager.instance.automatedTests[0];
    }
    // Update is called once per frame
    void Update()
    {
        currentTestName.text = selectedTest.name;

        agentType.text = selectedTest.teams[teamID].teamName;
        agentCount.text = selectedTest.teams[teamID].agentCount.ToString();

        winRatio.text = selectedTest.teams[teamID].stats.winLossRatio.ToString();
        avgTimeWin.text = selectedTest.teams[teamID].stats.averageTimeToWin.ToString();
        avgInflictedDmg.text = selectedTest.teams[teamID].stats.averateAmountOfInflictedDamage.ToString();
        avgSurvivalTime.text = selectedTest.teams[teamID].stats.averageSurvivalTime.ToString();
        avgSufferedLosses.text = selectedTest.teams[teamID].stats.averageSufferedAgentLosses.ToString();

    }

    public void OnTestClick(TestSetup testSetup)
    {
        selectedTest = testSetup;
    }
}

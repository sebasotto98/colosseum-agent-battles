using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "newTest", menuName = "New Test Configuration", order = 3)]
public class TestSetup : ScriptableObject
{
    public bool requiresTraining;
    public int trainingIterations;
    public List<TeamCharacteristics> teams;
}

[Serializable]
public class TeamCharacteristics
{
    public Vector3 spawnerPos;
    public AgentCharacteristics agentCharacteristics;
    public string teamName;
    public int agentCount;
    public TeamStatistics stats;
}

[Serializable]
public class TeamStatistics
{
    public float winLossRatio = 0;
    public float averageTimeToWin = 0;
    public float averateAmountOfInflictedDamage = 0;
    public float averageSurvivalTime = 0;
    public float averageSufferedAgentLosses = 0;
}

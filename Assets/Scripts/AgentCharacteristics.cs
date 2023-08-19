using UnityEngine;
using System;

public enum AgentActions
{
    NO_ACTION,
    UP,
    DOWN,
    RIGHT,
    LEFT,
    ATTACK,
}

public enum AgentType
{
    RANDOM,
    GREEDY,
    COOP_GREEDY,
    RATIONAL
}

public enum TargetingStrategy
{
    RANDOM,
    CLOSEST
}

[Serializable]
public class ActionUtility
{
    public AgentActions action;
    public float utility;

    public ActionUtility(AgentActions action, float util)
    {
        this.action = action;
        this.utility = util;
    }
}

[CreateAssetMenu(fileName = "agentType", menuName = "New Agent Type")]
public class AgentCharacteristics : ScriptableObject
{
    public AgentType agentType;
    public TargetingStrategy targetingStrat;

    public Color materialColor;
    public int healthPoints = 10;
    public int movementSpeedMultiplier = 1;
    public WeaponType weapon;

    [Range(0, 1)]
    public float learningRate;
    [Range(0, 1)]
    public float discountFactor;
    [Range(0, 1)]
    public float explorationRate;
}

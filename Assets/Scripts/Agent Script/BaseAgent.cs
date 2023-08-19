using System;

[Serializable]
public class BaseAgent
{
    protected AgentController controller;

    public BaseAgent(AgentController newController)
    {
        controller = newController;
    }

    public virtual AgentActions SelectAction()
    {
        return AgentActions.NO_ACTION;
    }

    public virtual void NextStep(AgentActions action)
    {
    }

    public virtual void AdjustReward(AgentActions action, float previousTargetHealthPoints, float previousDistanceToTarget)
    {
    }

    public virtual void ResetState()
    {
    }
}

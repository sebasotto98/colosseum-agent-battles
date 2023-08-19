using UnityEngine;

public class RandomAgent : BaseAgent
{
    public RandomAgent(AgentController newController) : base(newController)
    {
    }

    public override AgentActions SelectAction()
    {
        return (AgentActions)Random.Range(0, 6);
    }

}

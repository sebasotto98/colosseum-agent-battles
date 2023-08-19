using UnityEngine;

public class GreedyAgent : BaseAgent
{
    public GreedyAgent(AgentController newController) : base(newController)
    {
    }

    public override AgentActions SelectAction()
    {
        var currentTarget = controller.currentTarget;
        if (!currentTarget) return AgentActions.NO_ACTION;

        Vector3 distanceToTarget = currentTarget.transform.position - controller.transform.position;

        if (distanceToTarget.magnitude < controller.characteristics.weapon.weaponRange) return AgentActions.ATTACK;
        else
        {
            if (Mathf.Abs(distanceToTarget.x) > Mathf.Abs(distanceToTarget.y))
            {
                if (distanceToTarget.x > 0) return AgentActions.RIGHT;
                else if (distanceToTarget.x < 0) return AgentActions.LEFT;
                else return AgentActions.NO_ACTION;
            }
            else if (Mathf.Abs(distanceToTarget.x) < Mathf.Abs(distanceToTarget.y))
            {
                if (distanceToTarget.y > 0) return AgentActions.UP;
                else if (distanceToTarget.y < 0) return AgentActions.DOWN;
                else return AgentActions.NO_ACTION;
            }
            else
            {
                return (AgentActions)Random.Range(1, 5);
            }
        }
    }
}

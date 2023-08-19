using System.Linq;
using UnityEngine;

public class QLearningAgent : BaseAgent
{
    float[][] qValueMatrix;
    int currentState;
    protected float currentReward;

    public QLearningAgent(AgentController newController) : base(newController)
    {
        qValueMatrix = new float[1000][];
        for (int i = 0; i < 1000; i++)
        {
            qValueMatrix[i] = new float[6];
            for (int j = 0; j < 6; j++)
            {
                qValueMatrix[i][j] = 0.0f;
            }
        }
    }

    public override AgentActions SelectAction()
    {
        if (!controller.currentTarget) return AgentActions.NO_ACTION;

        //Adjust q_values
        var uniformVal = Random.Range(0.0f, 1.0f);
        if (!controller.isTraining || (controller.isTraining && uniformVal > controller.characteristics.explorationRate))
        {
            //Group of actions based on q_values
            return (AgentActions)qValueMatrix[currentState].ToList().IndexOf(qValueMatrix[currentState].Max());
        }
        else
        {
            var value = Random.Range(0, 6);
            return (AgentActions)value;
        }
    }

    public override void ResetState()
    {
        currentState = 0;
    }

    public override void AdjustReward(AgentActions action, float previousTargetHealthPoints, float previousDistanceToTarget)
    {
        if (controller.characteristics.agentType != AgentType.RATIONAL) return;

        if (!controller.currentTarget) return;

        var newDistanceToTarget = (controller.currentTarget.transform.position - controller.transform.position).sqrMagnitude;

        currentReward = -0.5f;
        if (controller.tookDamage)
        {
            currentReward += -1f;
            controller.tookDamage = false;
        }

        if (action == AgentActions.ATTACK && !controller.currentTarget.activeSelf)
            currentReward += 10;
        else if (action == AgentActions.ATTACK && previousTargetHealthPoints > controller.targetController.currentHealthPoints)
            currentReward += 2;
        else if (previousDistanceToTarget > newDistanceToTarget)
            currentReward += 1;

        if (controller.isTraining)
            UpdateQTable(action);
        else
            currentState++;
    }

    public void UpdateQTable(AgentActions action)
    {
        int nextState = currentState + 1;
        float alpha = controller.characteristics.learningRate;
        float gamma = controller.characteristics.discountFactor;

        qValueMatrix[currentState][(int)action] += alpha * (currentReward + gamma * qValueMatrix[nextState].Max() - qValueMatrix[currentState][(int)action]);

        currentState = nextState;
    }
}

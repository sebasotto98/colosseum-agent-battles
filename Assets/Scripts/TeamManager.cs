using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamManager : MonoBehaviour
{
    public int teamID = 0;
    public Color teamColor;

    [SerializeField]
    GameObject agentPrefab;
    public AgentCharacteristics agentCharacteristics;

    public List<GameObject> spawnedAgents = new List<GameObject>();
    public List<AgentController> spawnedAgentControllers = new List<AgentController>();
    public List<GameObject> deadAgents = new List<GameObject>();

    public string teamName = "";
    public int agentCount = 1;
    private int sufferedDamage = 0;
    private int inflictedDamage = 0;

    public bool readyToPlatoon = false;

    public List<float> survivalTimes = new List<float>();

    public string GetTeamName()
    {
        return teamName;
    }
    public int GetDeadAgentCount()
    {
        return deadAgents.Count;
    }
    public List<GameObject> GetSpawnedAgents()
    {
        return spawnedAgents;
    }

    public void AddDeadAgent(GameObject deadAgent)
    {
        deadAgents.Add(deadAgent);
        survivalTimes.Add(EnvironmentManager.instance.elapsedTime);
        if (deadAgents.Count == spawnedAgents.Count) EnvironmentManager.instance.NoAgentsAliveInTeam();
    }

    public float GetAverageSurvivalTime()
    {
        float totalTime = survivalTimes.Sum() + (spawnedAgents.Count - deadAgents.Count) * EnvironmentManager.instance.maxTimer;
        return totalTime / spawnedAgents.Count;
    }

    public int GetInflictedDamage()
    {
        return inflictedDamage;
    }
    public void IncrementInflictedDamage()
    {
        inflictedDamage++;
    }
    public void ResetInflictedDamage()
    {
        inflictedDamage = 0;
    }
    public int GetSufferedDamage()
    {
        return sufferedDamage;
    }

    public void IncrementSufferedDamage()
    {
        sufferedDamage++;
    }
    public void ResetSufferedDamage()
    {
        sufferedDamage = 0;
    }

    public void ResetAgents()
    {
        ResetInflictedDamage();
        ResetSufferedDamage();
        AdjustAgentsCount();

        Bounds bounds = GetComponent<BoxCollider2D>().bounds;
        spawnedAgentControllers.Clear();
        survivalTimes.Clear();

        foreach (GameObject agent in spawnedAgents)
        {
            AgentController controller = agent.GetComponent<AgentController>();
            if (!controller.characteristics) controller.characteristics = agentCharacteristics;

            if(EnvironmentManager.instance.requiresTraining && !controller.spawnLocation.Equals(Vector3.negativeInfinity))
            {
                agent.transform.position = controller.spawnLocation;
            }
            else
            {
                float offsetX = Random.Range(-bounds.extents.x, bounds.extents.x);
                float offsetY = Random.Range(-bounds.extents.y, bounds.extents.y);
                float offsetZ = Random.Range(-bounds.extents.z, bounds.extents.z);

                agent.transform.position = bounds.center + new Vector3(offsetX, offsetY, 0);

                controller.spawnLocation = agent.transform.position;
            }


            agent.SetActive(true);
            controller.StopAllCoroutines();
            controller.teamID = teamID;
            controller.platoonAgents.Clear();

            spawnedAgentControllers.Add(controller);
        }

        deadAgents.Clear();
    }

    public void PreSetup()
    {
        foreach (GameObject agent in spawnedAgents)
        {
            agent.GetComponent<AgentController>().SetupAgent(this);
        }
    }

    public void StartAgents()
    {
        foreach (GameObject agent in spawnedAgents)
        {
            agent.GetComponent<AgentController>().StartAgent(this);
        }
    }

    void AdjustAgentsCount()
    {
        if(spawnedAgents.Count < agentCount)
        {
            int difference = agentCount - spawnedAgents.Count;
            for (int i = 0; i < difference; i++)
            {
                GameObject newAgent = Instantiate(agentPrefab);
                newAgent.name = newAgent.name + i;
                spawnedAgents.Add(newAgent);

            }

        }
        else if(spawnedAgents.Count > agentCount)
        {
            int difference = spawnedAgents.Count - agentCount;
            List<GameObject> agentsToRemove = spawnedAgents.GetRange(agentCount, difference);
            spawnedAgents.RemoveRange(agentCount, difference);

            foreach (var item in agentsToRemove)
            {
                Destroy(item);
            }
        }

    }
}

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager instance { get; private set; }

    public int maxIterations = 20;
    public int trainingIterations = 0;
    [ReadOnly]
    public int currentIteration = 0;

    public float timeScaleMultiplier = 1;
    public float maxTimer = 20;
    [ReadOnly]
    public float elapsedTime = 0;

    [ReadOnly]
    public bool isSimulationActive = false;

    public List<TeamManager> teamManagers = new List<TeamManager>();
    private int incapacitatedTeams = 0;

    public List<TestSetup> automatedTests;
    private int currentAutomatedTest = 0;
    public int currentlySelectedTestIndex = 0;

    public bool requiresTraining = false;
    public bool isTraining = true;

    public int GetMaxIterations()
    {
        return maxIterations;
    }
    public void SetMaxIterations(int value)
    {
        maxIterations = value;
    }
    public void SetTimeScaleMultiplier(float value)
    {
        timeScaleMultiplier = value;
    
    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
            instance = this;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isSimulationActive) return;

        Time.timeScale = timeScaleMultiplier;

        elapsedTime += Time.deltaTime;

        if(elapsedTime >= maxTimer || incapacitatedTeams >= teamManagers.Count - 1)
        {
            currentIteration++;

            var minDeadAgents = teamManagers.Min(x => x.GetDeadAgentCount());
            //In case of a tie, the tying teams all win
            var minDeadTeams = teamManagers.Where(r => r.GetDeadAgentCount() == minDeadAgents).ToList();

            if(!isTraining)
            {
                foreach (TeamManager team in teamManagers)
                {
                    string teamName = team.GetTeamName();

                    BattleResult result = minDeadTeams.Contains(team) ? BattleResult._W : BattleResult._L;

                    Debug.Log("Team " + teamName + " got a " + result);

                    MetricManager.SetAverageSufferedAgentLosses(teamName, team.GetDeadAgentCount());

                    ComputeWL(team, result);

                    ComputeAmountOfInflictedDamage(team);

                    MetricManager.SetAverageSurvivalTime(team);
                    if(result == BattleResult._W)
                        MetricManager.SetAverageTimeToWin(teamName, (int)elapsedTime);
                }
            }

            elapsedTime = 0;

            RestartEnvironment();

            incapacitatedTeams = 0;

            Debug.Log("Battle iteration " + currentIteration + " over");

            if((!isTraining && currentIteration >= maxIterations) ||
                (isTraining && currentIteration >= trainingIterations))
            {
                if (isTraining)
                {
                    isTraining = false;
                    currentIteration = 0;
                }
                else if(automatedTests.Count > 0 && currentAutomatedTest < automatedTests.Count - 1)
                {
                    MetricManager.DisplayMetricsInTest(automatedTests[currentAutomatedTest]);
                    currentAutomatedTest++;
                    SetTestSetup(automatedTests[currentAutomatedTest]);
                    
                    RestartEnvironment();
                    ReaplyCharacteristics();

                    currentIteration = 0;
                }
                else
                {
                    MetricManager.DisplayMetricsInTest(automatedTests[currentAutomatedTest]);

                    Debug.Log("Simulation over");
                }
            }
        }
    }

    private void ComputeWL(TeamManager team, BattleResult result)
    {
        string teamName = team.GetTeamName();

        MetricManager.SetWLRatio(teamName, result);

        float wlRatio = MetricManager.GetWLRatio(teamName);

        Debug.Log("New team " + teamName + " W/L ratio is: " + wlRatio);
    }

    private void ComputeAmountOfInflictedDamage(TeamManager team)
    {
        string teamName = team.GetTeamName();

        int teamInflictedDamage = team.GetInflictedDamage();

        Debug.Log("Amount of inflicted damage of " + teamName + ": " + teamInflictedDamage);

        MetricManager.SetAverageAmountOfInflictedDamage(teamName, teamInflictedDamage);

        float averageID = MetricManager.GetAverageAmountOfInflictedDamage(teamName);

        Debug.Log("New average amount of inflicted damage of " + teamName + ": " + teamInflictedDamage);
    }

    public void StartSimulation()
    {
        isSimulationActive = true;

        currentIteration = 0;
        elapsedTime = 0;

        if (automatedTests.Count > 0)
        {
            SetTestSetup(automatedTests[currentAutomatedTest]);
            ReaplyCharacteristics();
        }

        foreach (var team in teamManagers)
        {
            team.ResetAgents();
        }
        ReaplyCharacteristics();
        RestartEnvironment();

    }

    private void RestartEnvironment()
    {
        foreach (var team in teamManagers)
        {
            team.ResetAgents();
        }

        foreach (var team in teamManagers)
        {
            team.PreSetup();
        }

        foreach (var team in teamManagers)
        {
            team.StartAgents();
        }
    }

    public void ReaplyCharacteristics()
    {
        foreach (TeamManager team in teamManagers)
        {
            foreach (var item in team.spawnedAgents)
            {
                var controller = item.GetComponent<AgentController>();
                controller.ApplyCharacteristics(team);
                item.SetActive(true);
            }
        }
    }

    public void NoAgentsAliveInTeam()
    {
        incapacitatedTeams++;
    }

    private void SetTestSetup(TestSetup test)
    {
        requiresTraining = test.requiresTraining;
        isTraining = test.requiresTraining;
        if (isTraining)
            trainingIterations = test.trainingIterations;
        
        int i = 0;
        foreach (var team in teamManagers)
        {
            TeamCharacteristics teamTestConfig = test.teams[i];
            team.transform.position = teamTestConfig.spawnerPos;
            team.agentCharacteristics = teamTestConfig.agentCharacteristics;
            team.teamName = test.name + "_" + teamTestConfig.teamName;
            team.agentCount = teamTestConfig.agentCount;
            i++;
        }
    }
}

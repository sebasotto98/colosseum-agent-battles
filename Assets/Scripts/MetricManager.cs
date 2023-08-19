using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum BattleResult
{
    _W, // win
    _L  // loss
}
public enum GenericMetricKeywords
{
    _Total,
    _Iteration
}
public enum MetricTypes
{
    _WLR, // win/loss ratio
    _TTW, // average time to win
    _ST,  // average survival time
    _ID,  // average amount of inflicted damage
    _SAL, // average number of suffered agent losses
}
// class to store metrics about every agent team to determine the most effective ones
public class MetricManager : MonoBehaviour
{
    private static List<MetricTypes> GetMetricTypes()
    {
        MetricTypes[] array = (MetricTypes[])MetricTypes.GetValues(typeof(MetricTypes));
        List<MetricTypes> list = new List<MetricTypes>(array);
        return list;
    }
    // called whenever a game ends to compute new W/L ratio for both teams
    public static void SetWLRatio(string teamName, BattleResult battleResult)
    {
        var metricName = teamName + MetricTypes._WLR;
        var winsStr = metricName + BattleResult._W;
        var lossesStr = metricName + BattleResult._L;
        var wins = PlayerPrefs.GetInt(winsStr);
        var losses = PlayerPrefs.GetInt(lossesStr);

        if(battleResult == BattleResult._W)
        {
            wins++;
            PlayerPrefs.SetInt(winsStr, wins);
        } else
        {
            losses++;
            PlayerPrefs.SetInt(lossesStr, losses);
        }

        float newRatio = (float)wins / (float)(wins + losses);

        PlayerPrefs.SetFloat(metricName, newRatio);

    }
    public static float GetWLRatio(string teamName)
    {
        var metricName = teamName + MetricTypes._WLR;

        float ratio = PlayerPrefs.GetFloat(metricName);

        return ratio;
    }
    // called whenever a game ends to compute new average time to win of the winning team
    public static void SetAverageTimeToWin(string teamName, int timeToWin)
    {
        SetMetricAverage(MetricTypes._TTW, teamName, timeToWin);
    }
    public static float GetAverageTimeToWin(string teamName)
    {
        var metricName = teamName + MetricTypes._TTW;

        var averageTTW = PlayerPrefs.GetFloat(metricName);

        return averageTTW;
    }
    // called whenever a game ends to compute the new average amount of inflicted damage of both teams
    public static void SetAverageAmountOfInflictedDamage(string teamName, int inflictedDamage)
    {
        SetMetricAverage(MetricTypes._ID, teamName, inflictedDamage);
    }
    public static float GetAverageAmountOfInflictedDamage(string teamName)
    {
        var metricName = teamName + MetricTypes._ID;

        var averageID = PlayerPrefs.GetFloat(metricName);
            
        return averageID;
    }
    // called whenever a game ends to compute the new average survival time of the losing team
    public static void SetAverageSurvivalTime(string teamName, int survivalTime)
    {
        SetMetricAverage(MetricTypes._ST, teamName, survivalTime);
    }

    public static void SetAverageSurvivalTime(TeamManager team)
    {
        SetMetricAverage(MetricTypes._ST, team.GetTeamName(), team.GetAverageSurvivalTime());
    }

    public static float GetAverageSurvivalTime(string teamName)
    {
        var metricName = teamName + MetricTypes._ST;

        var averageST = PlayerPrefs.GetFloat(metricName);

        return averageST;
    }
    // called whenever a game ends to compute the new average suffered agent losses of both teams
    public static void SetAverageSufferedAgentLosses(string teamName, int sufferedAgentLosses)
    {
        SetMetricAverage(MetricTypes._SAL, teamName, sufferedAgentLosses);
    }
    public static float GetAverageSufferedAgentLosses(string teamName)
    {
        var metricName = teamName + MetricTypes._SAL;

        var averageSAL = PlayerPrefs.GetFloat(metricName);

        return averageSAL;
    }
    // called by all average computing methods
    private static void SetMetricAverage(MetricTypes metricType, string teamName, float value)
    {

        var metricName = teamName + metricType;
        var metricTotalStr = metricName + GenericMetricKeywords._Total;
        var metricIterationStr = metricName + GenericMetricKeywords._Iteration;
        var metricTotal = PlayerPrefs.GetFloat(metricTotalStr);
        var metricIteration = PlayerPrefs.GetFloat(metricIterationStr);

        metricTotal += value;
        metricIteration++;

        PlayerPrefs.SetFloat(metricTotalStr, metricTotal);
        PlayerPrefs.SetFloat(metricIterationStr, metricIteration);

        float newAverage = (float)metricTotal / (float)metricIteration;

        PlayerPrefs.SetFloat(metricName, newAverage);

    }

    public static string GetMostEffectiveForMetric(TestSetup test, MetricTypes metric)
    {
        string metricName;
        Dictionary<string, float> dict = new Dictionary<string, float>();
        
        foreach(var team in test.teams)
        {
            metricName = team.teamName + metric;
            dict.Add(team.teamName, PlayerPrefs.GetFloat(metricName));
        }

        var ordered = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var first = dict.First();
        string mostEffectiveTeam = first.Key;

        return mostEffectiveTeam;
    }
    public static string GetMostEffectiveOverall(TestSetup test)
    {
        string metricName;
        Dictionary<string, float> dict = new Dictionary<string, float>();

        Dictionary<string, int> effectivenessDict = new Dictionary<string, int>();

        foreach (var team in test.teams)
        {
            effectivenessDict.Add(team.teamName, 0);
        }

        foreach (MetricTypes metric in GetMetricTypes())
        {
            foreach (var team in test.teams)
            {
                metricName = team.teamName + metric;
                dict.Add(team.teamName, PlayerPrefs.GetFloat(metricName));

                var ordered = dict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                var first = dict.First();

                effectivenessDict[first.Key] = effectivenessDict[first.Key]++;
            }
            dict.Clear();
        }
        

        var ordered2 = effectivenessDict.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
        var first2 = effectivenessDict.First();
        string mostEffectiveTeam = first2.Key;

        return mostEffectiveTeam;
    }
    public static void DisplayMetricsInTest(TestSetup test)
    {
        foreach (var team in test.teams)
        {
            TeamStatistics stats = team.stats;
            string teamName = test.name + "_" + team.teamName;

            stats.winLossRatio = GetWLRatio(teamName);
            stats.averageTimeToWin = GetAverageTimeToWin(teamName);
            stats.averateAmountOfInflictedDamage = GetAverageAmountOfInflictedDamage(teamName);
            stats.averageSurvivalTime = GetAverageSurvivalTime(teamName);
            stats.averageSufferedAgentLosses = GetAverageSufferedAgentLosses(teamName);
        }
    }
    public static void DisplayMostEffectiveTeamsOnScreen(TestSetup test)
    {

        foreach (MetricTypes metric in GetMetricTypes())
        {
            string mostEffectiveTeamForMetric = GetMostEffectiveForMetric(test, metric);
#if UNITY_EDITOR
                EditorUtility.DisplayDialog("Simulation finished", "Most effective team for " + metric + ": " + mostEffectiveTeamForMetric, "ok", "cancel");
#endif
        }

        string mostEffectiveTeamOverall = GetMostEffectiveOverall(test);

#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Simulation finished", "Most effective team overall: " + mostEffectiveTeamOverall, "ok", "cancel");
#endif
    }

}
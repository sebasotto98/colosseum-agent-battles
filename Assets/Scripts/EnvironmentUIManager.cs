using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnvironmentUIManager : MonoBehaviour
{
    public InputField maxIterations;
    public InputField currentIterations;
    public InputField timeScale;
    public InputField maxTimer;
    public InputField elapsedTime;

    private void Update()
    {
        maxIterations.text = EnvironmentManager.instance.maxIterations.ToString();
        currentIterations.text = EnvironmentManager.instance.currentIteration.ToString();
        timeScale.text = EnvironmentManager.instance.timeScaleMultiplier.ToString();
        maxTimer.text = EnvironmentManager.instance.maxTimer.ToString();
        elapsedTime.text = EnvironmentManager.instance.elapsedTime.ToString();
    }

    public void OnMaxTimerChange(string input)
    {
        float value = float.Parse(input);
        if (value > 0)
            EnvironmentManager.instance.maxTimer = value;
    }
}

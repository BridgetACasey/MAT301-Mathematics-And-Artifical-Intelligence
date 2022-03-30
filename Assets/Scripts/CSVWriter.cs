using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct AgentTestData
{
    public int generation;
    public int agentCount;
    public List<float> agentFitnesses;
    public List<float> agentTimes;
    public float fitnessSum;
    public float bestFitness;
    public int agentsCrashed;
    public int agentsTimedOut;
    public int agentsCompleted;
}

public class CSVWriter : MonoBehaviour
{
    [SerializeField] private string fileName;
    private string filePath;

    public List<AgentTestData> testLogs;

    void Start()
    {
        filePath = Application.dataPath + "/" + fileName;
    }

    //Prints every generation's stored data to a new .csv file
    public void WriteAgentDataToCSV()
    {
        TextWriter writer = new StreamWriter(filePath, false);

        writer.WriteLine("SUMMARY");
        writer.WriteLine("Gen., Agents, Fit. Sum, Best Fit., Crashed, TimedOut, Complete");
        writer.Close();

        writer = new StreamWriter(filePath, true);

        foreach(AgentTestData data in testLogs)
        {
            writer.WriteLine(data.generation + "," + data.agentCount + "," + data.fitnessSum + "," + data.bestFitness
                + "," + data.agentsCrashed + "," + data.agentsTimedOut + "," + data.agentsCompleted);
        }

        writer.WriteLine("FULL DATA");

        foreach (AgentTestData data in testLogs)
        {
            writer.WriteLine("Gen: " + data.generation);
            writer.WriteLine("Agents: " + data.agentCount);
            writer.WriteLine("Fitness, Times");

            for (int i = 0; i < data.agentCount; i++)
            {
                writer.WriteLine(data.agentFitnesses[i] + "," + data.agentTimes[i]);
            }
        }
    }
}
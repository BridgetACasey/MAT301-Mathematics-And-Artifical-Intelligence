using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    [SerializeField] private string fileName;
    private string filePath;

    [System.Serializable]
    public struct AgentTestData
    {
        public string generation;
        public int agentCount;
        public List<float> agentFitnesses;
        public List<float> agentTimes;
        public List<float> agentDistances;
        public float bestFitness;
        public float bestTime;
        public float bestDistance;
        public int agentsCrashed;
        public int agentsTimedOut;
        public int agentsCompleted;
    }

    void Start()
    {
        fileName += " " + System.DateTime.Now.ToString() + ".csv";
        filePath = Application.dataPath + "/" + fileName;
    }

    public void WriteToCSV()
    {
        TextWriter writer = new StreamWriter(filePath, false);

        writer.WriteLine("Gen., Count, Agents, Fit., Time, Dist., Best Fit., Best Time, Best Dist., Crashed, TimedOut, Complete");
        writer.Close();

        writer = new StreamWriter(filePath, true);


    }
}

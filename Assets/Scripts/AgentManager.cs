using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    public List<GameObject> agents;
    public bool completedGen = false;
    [SerializeField] private int populationSize = 32;

    public AgentManager(GameObject agentPrefab, Vector3 spawnPoint, Vector3 goalPosition)
    {
        agents = new List<GameObject>();

        for (int i = 0; i < populationSize; i++)
        {
            SpawnAgent(agentPrefab, spawnPoint);
        }

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().SetupAttributes();
            agent.GetComponent<Agent>().SetGoalPosition(goalPosition);
        }
    }

    public void CheckActiveAgents(bool running)
    {
        int agentsRemaining = 0;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().SetRunning(running);

            if (agent.GetComponent<Agent>().GetDriving())
                agentsRemaining++;
        }

        if (running)
        {
            if (agentsRemaining == 0)
                completedGen = true;
        }
    }

    public void UpdateAgentNetworks(List<DNA<AgentData>> genes)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            DNA<AgentData> dna = genes[i];
            GameObject agent = agents[i];

            for (int j = 0; j < dna.Genes.Length; j++)
            {
                agent.GetComponent<NeuralNetwork>().weights = dna.Genes[j].agentWeights;
                agent.GetComponent<NeuralNetwork>().biases = dna.Genes[j].agentBiases;
            }
        }
    }

    public void SpawnAgent(GameObject agentPrefab, Vector3 spawnPoint)
    {
        agents.Add(GameObject.Instantiate(agentPrefab, spawnPoint, Quaternion.identity));
    }

    public void ResetAgents()
    {
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().ResetAttributes();
        }
    }
}
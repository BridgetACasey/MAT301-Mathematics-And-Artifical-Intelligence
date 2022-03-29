using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    public List<GameObject> agents;
    public bool completedGen = false;
    public bool goalReached = false;
    public float bestDistance = 0.0f;

    public AgentManager(int population, GameObject agentPrefab, Vector3 spawnPoint)
    {
        agents = new List<GameObject>();

        for (int i = 0; i < population; i++)
        {
            SpawnAgent(agentPrefab, spawnPoint);
        }

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().SetupAttributes();
        }
    }

    public void CheckActiveAgents(bool running)
    {
        int agentsRemaining = 0;

        foreach (GameObject agent in agents)
        {
            agent.GetComponent<Agent>().SetRunning(running);

            if (agent.GetComponent<Agent>().GetReachedGoal())
                goalReached = true;

            if (agent.GetComponent<Agent>().GetDriving())
                agentsRemaining++;
        }

        if (running)
        {
            if (agentsRemaining == 0)
                completedGen = true;
        }
    }

    public void UpdateAgentNetworks(List<DNA> genes)
    {
        for (int i = 0; i < genes.Count; i++)
        {
            DNA dna = genes[i];
            GameObject agent = agents[i];

            agent.GetComponent<NeuralNetwork>().weights = dna.Genes[0].agentWeights;
            agent.GetComponent<NeuralNetwork>().biases = dna.Genes[0].agentBiases;
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
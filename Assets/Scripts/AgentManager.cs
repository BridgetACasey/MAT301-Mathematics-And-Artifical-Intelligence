using System.Collections.Generic;
using UnityEngine;

public class AgentManager
{
    public List<GameObject> agents;

    private bool completedGen = false;
    private bool goalReached = false;
    private int agentsCrashed = 0;
    private int agentsTimedOut = 0;
    private int agentsCompleted = 0;

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

    //Assesses the active agents and updates relevant data about the current generation
    public void CheckActiveAgents(bool running)
    {
        agentsCrashed = 0;
        agentsTimedOut = 0;
        agentsCompleted = 0;

        int agentsRemaining = 0;

        foreach (GameObject agent in agents)
        {
            Agent currentAgent = agent.GetComponent<Agent>();

            currentAgent.SetRunning(running);

            if (currentAgent.GetReachedGoal())
                goalReached = true;

            if (currentAgent.GetDriving())
                agentsRemaining++;

            switch(currentAgent.GetStatus())
            {
                case AgentStatus.CRASHED:
                    agentsCrashed++;
                    break;
                case AgentStatus.TIMEDOUT:
                    agentsTimedOut++;
                    break;
                case AgentStatus.COMPLETED:
                    agentsCompleted++;
                    break;
                default:
                    break;
            }
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

            //Only using one set of genes per agent network, hence hard-coding the first element
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

    public void SetCompletedGen(bool gen) { completedGen = gen; }

    public bool GetCompletedGen() { return completedGen; }

    public void SetGoalReached(bool goal) { goalReached = goal; }

    public bool GetGoalReached() { return goalReached; }

    public int GetAgentsCrashed() { return agentsCrashed; }

    public int GetAgentsTimedOut() { return agentsTimedOut; }

    public int GetAgentsCompleted() { return agentsCompleted; }
}
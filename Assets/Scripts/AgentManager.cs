using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public List<GameObject> agents;

    public AgentManager()
    {
        agents = new List<GameObject>();
    }

    public void CheckActiveAgents()
    {
        int agentsRemaining = 0;

        foreach (GameObject agent in agents)
        {
            if (agent.GetComponent<Agent>().GetDriving())
                agentsRemaining++;
        }

        if (agentsRemaining == 0)
            ResetAgents();
    }

    public void UpdateAgentAttributes(List<DNA<float>> genes)
    {

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

    public void EraseAgents()
    {
        foreach(GameObject agent in agents)
        {
            Destroy(agent);
        }

        agents.Clear();
    }
}
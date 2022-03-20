using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public List<GameObject> agents;

    public AgentManager()
    {
        agents = new List<GameObject>();
    }

    public void SpawnAgent(GameObject agentPrefab, Vector3 spawnPoint)
    {
        agents.Add(GameObject.Instantiate(agentPrefab, spawnPoint, Quaternion.identity));
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
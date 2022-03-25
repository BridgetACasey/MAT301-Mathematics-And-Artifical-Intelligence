using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;

public struct AgentData
{
	public List<Matrix<float>> agentWeights;
	public List<float> agentBiases;
}

public class Driving : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
	[SerializeField] private GameObject goal;

	[SerializeField] private Text runButtonText;
	[SerializeField] private Text genNumText;
	[SerializeField] private Text bestGenesText;
	[SerializeField] private Text bestFitnessText;
	[SerializeField] private Text elapsedTimeText;

	[SerializeField] private float mutationRate = 0.01f;
	[SerializeField] private int innerCount = 1;
	[SerializeField] private float innerScale = 400.0f;
	[SerializeField] private float timeScale = 1.0f;
	[SerializeField] private float bestFit = 0.0f;

	[SerializeField] private float elapsedTime = 0.0f;

	private GeneticAlgorithm<AgentData> geneticAlgorithm;
    private AgentManager agentManager;
    private System.Random random;
	private bool running = false;

	void Start()
    {
		random = new System.Random();
		agentManager = new AgentManager(agentPrefab, transform.position, goal.transform.position);
		geneticAlgorithm = new GeneticAlgorithm<AgentData>(agentManager.agents.Count, innerCount, random, GetRandomGene, FitnessFunction, mutationRate: mutationRate);
		agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
	}

	void Update()
    {
		Time.timeScale = timeScale;
		UpdateAlgorithmText();
		agentManager.CheckActiveAgents(running);

		if (running)
		{
			elapsedTime += Time.deltaTime;

			if (agentManager.completedGen)
			{
				geneticAlgorithm.NewGeneration();
				agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
				bestFit = geneticAlgorithm.BestFitness;
				agentManager.completedGen = false;
				agentManager.ResetAgents();
			}
		}
	}

    private AgentData GetRandomGene()
	{
		AgentData agentData = new AgentData();
	
		agentData.agentWeights = new List<Matrix<float>>();
		agentData.agentBiases = new List<float>();

		agentData.agentWeights.Add(Matrix<float>.Build.Dense(3, 3));
		agentData.agentWeights.Add(Matrix<float>.Build.Dense(3, 3));
		agentData.agentWeights.Add(Matrix<float>.Build.Dense(3, 3));
	
		for (int k = 0; k < agentData.agentWeights.Count; k++)
		{
			for (int j = 0; j < agentData.agentWeights[k].RowCount; j++)
			{
				for (int i = 0; i < agentData.agentWeights[k].ColumnCount; i++)
				{
					agentData.agentWeights[k][i, j] = Random.Range(-1.0f, 1.0f);
				}
			}
		}
	
		agentData.agentBiases.Add(Random.Range(-1.0f, 1.0f));
		agentData.agentBiases.Add(Random.Range(-1.0f, 1.0f));
	
		return agentData;
	}

	private float FitnessFunction(int index)
	{
		float fitness = agentManager.agents[index].GetComponent<Agent>().GetOverallFitness();
		return fitness;
	}

	private void UpdateAlgorithmText()
    {
		if (genNumText)
			genNumText.text = "Total Gens: " + geneticAlgorithm.Generation.ToString();

		if(bestGenesText)
        {
			float total = 0;

			for(int i = 0; i < geneticAlgorithm.BestGenes.Length; i++)
            {
				//total += geneticAlgorithm.BestGenes[i].agentBiases[0];
            }

			bestGenesText.text = "Best Genes: " + total.ToString();
        }

		if (bestFitnessText)
			bestFitnessText.text = "Best Fitness: " + geneticAlgorithm.BestFitness.ToString();

		if (elapsedTimeText)
			elapsedTimeText.text = "Time (s): " + elapsedTime.ToString();
	}

	public void RunAlgorithm()
    {
		//Start and stop the algorithm
		running = !running;

		if(running)
        {
			if (runButtonText)
				runButtonText.text = "STOP";
        }
        else
		{
			agentManager.ResetAgents();

			if (runButtonText)
				runButtonText.text = "RUN";
		}
    }
}
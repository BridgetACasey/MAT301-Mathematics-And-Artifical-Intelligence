using MathNet.Numerics.LinearAlgebra;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct AgentData
{
	public List<Matrix<float>> agentWeights;
	public List<float> agentBiases;
}

public class Driving : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;

	[SerializeField] private Text runButtonText;
	[SerializeField] private Text genNumText;
	[SerializeField] private Text fitnessSumText;
	[SerializeField] private Text bestFitnessText;
	[SerializeField] private Text bestDistanceText;
	[SerializeField] private Text currentTimeText;
	[SerializeField] private Text elapsedTimeText;

	[SerializeField] private int populationSize = 32;
	[SerializeField] private float mutationRate = 0.01f;
	[SerializeField] private int dnaLength = 4;
	[SerializeField] private float timeScale = 1.0f;
	[SerializeField] private float currentTime = 0.0f;
	[SerializeField] private float elapsedTime = 0.0f;

	private GeneticAlgorithm geneticAlgorithm;
    private AgentManager agentManager;
    private System.Random random;
	private bool running = false;

	void Start()
    {
		random = new System.Random();
		agentManager = new AgentManager(populationSize, agentPrefab, transform.position);
		geneticAlgorithm = new GeneticAlgorithm(agentManager.agents.Count, dnaLength, random, GetRandomGene, FitnessFunction, mutationRate: mutationRate);
		agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
	}

	void Update()
    {
		Time.timeScale = timeScale;
		UpdateAlgorithmText();
		
		agentManager.CheckActiveAgents(running);

		//if(agentManager.goalReached)
        //{
		//	RunAlgorithm();
		//	agentManager.ResetAgents();
		//	agentManager.goalReached = false;
        //}

		if (running)
		{
			elapsedTime += Time.deltaTime;
			currentTime += Time.deltaTime;

			if (agentManager.completedGen)
			{
				geneticAlgorithm.NewGeneration();
				agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
				agentManager.completedGen = false;
				agentManager.ResetAgents();
				currentTime = 0.0f;
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

		if(fitnessSumText)
			fitnessSumText.text = "Fitness Sum: " + geneticAlgorithm.fitnessSum;

		if (bestFitnessText)
			bestFitnessText.text = "Best Fitness: " + geneticAlgorithm.BestFitness.ToString();

		if (bestDistanceText)
			bestDistanceText.text = "Best Distance: ";

		if (currentTimeText)
			currentTimeText.text = "Time (s): " + currentTime.ToString();

		if (elapsedTimeText)
			elapsedTimeText.text = "Total Time (s): " + elapsedTime.ToString();
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
			if (runButtonText)
				runButtonText.text = "RUN";

			agentManager.ResetAgents();
		}
    }
}
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct AgentData
{
	public List<Matrix<float>> agentWeights;
	public List<float> agentBiases;
}

//The runnable script for the entire program
public class Driving : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;

	[SerializeField] private Text runButtonText;
	[SerializeField] private Text timeScaleText;
	[SerializeField] private Text genNumText;
	[SerializeField] private Text fitnessSumText;
	[SerializeField] private Text bestFitnessText;
	[SerializeField] private Text currentTimeText;
	[SerializeField] private Text elapsedTimeText;

	[SerializeField] private int populationSize = 32;
	[SerializeField] private float mutationRate = 0.01f;
	[SerializeField] private int dnaLength = 1;
	[SerializeField] private float timeScale = 1.0f;
	[SerializeField] private float currentTime = 0.0f;
	[SerializeField] private float elapsedTime = 0.0f;

	private GeneticAlgorithm geneticAlgorithm;
    private AgentManager agentManager;
	private CSVWriter csvWriter;
    private System.Random random;
	private bool running = false;

	void Start()
    {
		random = new System.Random();
		agentManager = new AgentManager(populationSize, agentPrefab, transform.position);
		csvWriter = GetComponent<CSVWriter>();
		geneticAlgorithm = new GeneticAlgorithm(agentManager.agents.Count, dnaLength, random, GetRandomGene, FitnessFunction, mutationRate: mutationRate);
		agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
	}

	void Update()
    {
		Time.timeScale = timeScale;
		UpdateAlgorithmText();
		
		agentManager.CheckActiveAgents(running);

		if(Input.GetKeyDown(KeyCode.Escape))
        {
			Application.Quit();
        }

		if (running)
		{
			elapsedTime += Time.deltaTime;
			currentTime += Time.deltaTime;

			if (agentManager.GetCompletedGen())
			{
				geneticAlgorithm.NewGeneration();
				WriteGenerationData();
				agentManager.UpdateAgentNetworks(geneticAlgorithm.Population);
				agentManager.SetCompletedGen(false);
				agentManager.ResetAgents();
				currentTime = 0.0f;
			}
		}

		if (agentManager.GetGoalReached())
		{
			RunAlgorithm();
			agentManager.SetGoalReached(false);
		}
	}

	private void WriteGenerationData()
	{
		AgentTestData testData = new AgentTestData();

		testData.generation = geneticAlgorithm.Generation - 1;
		testData.agentCount = populationSize;

		testData.agentFitnesses = new List<float>();
		testData.agentTimes = new List<float>();

		for (int i = 0; i < populationSize; i++)
        {
			Agent agent = agentManager.agents[i].GetComponent<Agent>();

			testData.agentFitnesses.Add(agent.GetOverallFitness());
			testData.agentTimes.Add(agent.GetElapsedTime());
        }

		testData.fitnessSum = geneticAlgorithm.fitnessSum;
		testData.bestFitness = geneticAlgorithm.BestFitness;
		testData.agentsCrashed = agentManager.GetAgentsCrashed();
		testData.agentsTimedOut = agentManager.GetAgentsTimedOut();
		testData.agentsCompleted = agentManager.GetAgentsCompleted();

		csvWriter.testLogs.Add(testData);
	}

	//Creating a totally new set of weights and biases for an agent network
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
		if (timeScaleText)
			timeScaleText.text = "T. Scale: " + RoundToFigures(timeScale, 2).ToString();

		if (genNumText)
			genNumText.text = "Current Gen: " + geneticAlgorithm.Generation.ToString();

		if(fitnessSumText)
			fitnessSumText.text = "Fitness Sum: " + RoundToFigures(geneticAlgorithm.fitnessSum, 3).ToString();

		if (bestFitnessText)
			bestFitnessText.text = "Best Fitness: " + RoundToFigures(geneticAlgorithm.BestFitness, 3).ToString();

		if (currentTimeText)
			currentTimeText.text = "Gen Time (s): " + RoundToFigures(currentTime, 5).ToString();

		if (elapsedTimeText)
			elapsedTimeText.text = "Total Time (s): " + RoundToFigures(elapsedTime, 5).ToString();
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

			csvWriter.WriteAgentDataToCSV();
			agentManager.ResetAgents();
		}
    }

	public void IncreaseTimescale() { timeScale += 0.15f; }

	public void DecreaseTimescale() { timeScale -= 0.15f; }

	//Helper function for rounding decimals in the UI
	public float RoundToFigures(float value, int digits)
	{
		float multiplier = Mathf.Pow(10.0f, (float)digits);
		return (Mathf.Round(value * multiplier) / multiplier);
	}
}
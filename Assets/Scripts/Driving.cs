using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Driving : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
	[SerializeField] private GameObject goal;

	[SerializeField] private Text runButtonText;
	[SerializeField] private Text genNumText;
	[SerializeField] private Text bestGenesText;
	[SerializeField] private Text bestFitnessText;

	[SerializeField] private float mutationRate = 0.01f;
	[SerializeField] private int innerCount = 40;
	[SerializeField] private float innerScale = 400.0f;
	[SerializeField] private float timeScale = 1.0f;
	[SerializeField] private float bestFit = 0.0f;

	private GeneticAlgorithm<float> geneticAlgorithm;
    private AgentManager agentManager;
    private System.Random random;
	private bool running = false;

	void Start()
    {
		random = new System.Random();
		agentManager = GetComponent<AgentManager>();
		geneticAlgorithm = new GeneticAlgorithm<float>(agentManager.agents.Count, innerCount, random, GetRandomGene, FitnessFunction, mutationRate: mutationRate);
	}

	void Update()
    {
		UpdateAlgorithmText();

		if (running)
		{
			agentManager.CheckActiveAgents();
			//agentManager.UpdateAgentJumpingStrength(geneticAlgorithm.Population);
			geneticAlgorithm.NewGeneration();
			bestFit = geneticAlgorithm.BestFitness;
		}
	}

	private float GetRandomGene()
	{
		float next = (float)random.NextDouble();

		float value = (innerScale + bestFit) / (float)innerCount;

		return (next * value);
	}

	private float FitnessFunction(int index)
	{
		float score = 0.0f;

		DNA<float> dna = geneticAlgorithm.Population[index];

		for (int i = 0; i < dna.Genes.Length; i++)
		{
			score += dna.Genes[i] - agentManager.agents[index].GetComponent<Agent>().GetOverallFitness();
		}

		return score;
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
				total += geneticAlgorithm.BestGenes[i];
            }

			bestGenesText.text = "Best Genes: " + total.ToString();
        }

		if (bestFitnessText)
			bestFitnessText.text = "Best Fitness: " + geneticAlgorithm.BestFitness.ToString();
	}

	public void RunAlgorithm()
    {
		//Start and stop the algorithm
		running = !running;

		if(running)
        {
			for (int i = 0; i < 32; i++)
			{
				agentManager.SpawnAgent(agentPrefab, transform.position);
			}

			foreach(GameObject agent in agentManager.agents)
			{
				agent.GetComponent<Agent>().SetupAttributes();
				agent.GetComponent<Agent>().SetGoalPosition(goal.transform.position);
			}

			if (runButtonText)
				runButtonText.text = "STOP";
        }
        else
		{
			agentManager.EraseAgents();

			if (runButtonText)
				runButtonText.text = "RUN";
		}
    }
}
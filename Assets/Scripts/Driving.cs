using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Driving : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
	[SerializeField] private GameObject goalPrefab;

	[SerializeField] private Text runButtonText;
	[SerializeField] private Text genNumText;
	[SerializeField] private Text bestGenesText;
	[SerializeField] private Text bestFitnessText;

	[SerializeField] private float mutationRate = 0.01f;
	[SerializeField] private int innerCount = 40;
	[SerializeField] private float innerScale = 400.0f;
	[SerializeField] private float timeScale = 1.0f;

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
    }

	private float GetRandomGene()
	{
		// Generate a new value based on the current best (the higher the best,
		// the greater the next random gene)

		return 0.0f;
	}

	private float FitnessFunction(int index)
	{
		// Go through each gene in a member of the population and make their fitness equal to
		// their strength
		
		return 0.0f;
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
			agentManager.SpawnAgent(agentPrefab, transform.position);

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
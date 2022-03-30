using System;

public class DNA
{
	public AgentData[] Genes { get; private set; }
	public float Fitness { get; private set; }

	private Func<AgentData> getRandomGene;
	private Func<int, float> fitnessFunction;

	private Random random;

	public DNA(int size, Random random, Func<AgentData> getRandomGene, Func<int, float> fitnessFunction, bool shouldInitGenes = true)
	{
		Genes = new AgentData[size];

		this.random = random;
		this.getRandomGene = getRandomGene;
		this.fitnessFunction = fitnessFunction;

		if (shouldInitGenes)
		{
			for (int i = 0; i < Genes.Length; i++)
			{
				Genes[i] = getRandomGene();
			}
		}
	}

	public float CalculateFitness(int index)
	{
		Fitness = fitnessFunction(index);

		return Fitness;
	}

	public DNA Crossover(DNA otherParent)
	{
		DNA child = new DNA(Genes.Length, random, getRandomGene, fitnessFunction, shouldInitGenes: false);

		for (int i = 0; i < Genes.Length; i++)
		{
			//Crossover will slightly favour the current set of genes, assuming it is the best from the previous generation
			child.Genes[i] = random.NextDouble() < 0.67 ? Genes[i] : otherParent.Genes[i];
			//child.Genes[i].agentWeights = random.NextDouble() < 0.67 ? Genes[i].agentWeights : otherParent.Genes[i].agentWeights;
			//child.Genes[i].agentBiases = random.NextDouble() < 0.67 ? Genes[i].agentBiases : otherParent.Genes[i].agentBiases;
		}
		
		return child;
	}

	public void Mutate(float mutationRate)
	{
		for (int i = 0; i < Genes.Length; i++)
		{
			if (random.NextDouble() < mutationRate)
			{
				Genes[i] = getRandomGene();
			}
		}
	}
}
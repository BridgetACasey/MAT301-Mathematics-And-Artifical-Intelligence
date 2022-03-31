using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;

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

		//Crossover will slightly favour the current set of genes, assuming it is the best from the previous generation
		for (int i = 0; i < Genes.Length; i++)
		{
			//Will mix and match the weights and biases between parents 80% of the time, otherwise will choose between whole sets of genes
			child.Genes[i].agentWeights = new List<Matrix<float>>();
			child.Genes[i].agentBiases = new List<float>();

			if (random.NextDouble() < 0.8)
			{
				child.Genes[i].agentWeights.Add(random.NextDouble() < 0.67 ? Genes[i].agentWeights[0] : otherParent.Genes[i].agentWeights[0]);
				child.Genes[i].agentWeights.Add(random.NextDouble() < 0.67 ? Genes[i].agentWeights[1] : otherParent.Genes[i].agentWeights[1]);
				child.Genes[i].agentWeights.Add(random.NextDouble() < 0.67 ? Genes[i].agentWeights[2] : otherParent.Genes[i].agentWeights[2]);

				child.Genes[i].agentBiases.Add(random.NextDouble() < 0.67 ? Genes[i].agentBiases[0] : otherParent.Genes[i].agentBiases[0]);
				child.Genes[i].agentBiases.Add(random.NextDouble() < 0.67 ? Genes[i].agentBiases[1] : otherParent.Genes[i].agentBiases[1]);
			}
			else
			{
				child.Genes[i] = random.NextDouble() < 0.67 ? Genes[i] : otherParent.Genes[i];
			}
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
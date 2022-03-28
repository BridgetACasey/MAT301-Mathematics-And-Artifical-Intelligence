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

		for (int i = 0; i < Genes.Length; i++)
		{
			//child.Genes[i] = random.NextDouble() < 0.5 ? Genes[i] : otherParent.Genes[i];
			child.Genes[i] = random.NextDouble() < 0.8 ? Genes[i] : otherParent.Genes[i];

			//child.Genes[i].agentWeights = new List<Matrix<float>>();
			//child.Genes[i].agentBiases = new List<float>();
			//
			//for (int j = 0; j < 3; j++)
			//{
			//	child.Genes[i].agentWeights.Add((Genes[i].agentWeights[j] + otherParent.Genes[i].agentWeights[j]) / 2);
			//}
			//
			//for(int k = 0; k < 2; k++)
			//{
			//	child.Genes[i].agentBiases.Add((Genes[i].agentBiases[k] + otherParent.Genes[i].agentBiases[k]) / 2);
			//}
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
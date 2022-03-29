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
			child.Genes[i] = random.NextDouble() < 0.75 ? Genes[i] : otherParent.Genes[i];
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
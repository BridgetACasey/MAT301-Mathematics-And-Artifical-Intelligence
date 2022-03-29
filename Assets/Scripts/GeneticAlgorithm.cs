using System.Collections.Generic;
using System;

public class GeneticAlgorithm
{
	public List<DNA> Population { get; private set; }
	public int Generation { get; private set; }
	public float BestFitness { get; private set; }
	public AgentData[] BestGenes { get; private set; }

	public DNA[] bestPerformers;

	public float mutationRate;
	private Random random;
	public float fitnessSum { get; private set; }

	public GeneticAlgorithm(int populationSize, int dnaSize, Random random, Func<AgentData> getRandomGene, Func<int, float> fitnessFunction, float mutationRate = 0.01f)
	{
		Generation = 1;
		this.mutationRate = mutationRate;
		Population = new List<DNA>();
		this.random = random;

		BestGenes = new AgentData[dnaSize];
		bestPerformers = new DNA[populationSize];

		for (int i = 0; i < populationSize; i++)
		{
			Population.Add(new DNA(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
		}
	}

	public void NewGeneration()
	{
		if (Population.Count <= 0)
		{
			return;
		}

		CalculateFitness();
		CalculateBestPerformers();

		List<DNA> newPopulation = new List<DNA>();

		DNA parent1 = ChooseBestParent();

		for (int i = 0; i < Population.Count; i++)
		{
			//DNA parent1 = ChooseParent();
			DNA parent2 = ChooseParent();

			DNA child = parent1.Crossover(parent2);

			child.Mutate(mutationRate);

			newPopulation.Add(child);
		}

		Population = newPopulation;

		Generation++;
	}

	public void CalculateFitness()
	{
		fitnessSum = 0;

		DNA best = Population[0];

		for (int i = 0; i < Population.Count; i++)
		{
			fitnessSum += Population[i].CalculateFitness(i);

			if (Population[i].Fitness > best.Fitness)
			{
				best = Population[i];
			}
		}

		BestFitness = best.Fitness;
		best.Genes.CopyTo(BestGenes, 0);
	}

	private void CalculateBestPerformers()
    {
		Population.CopyTo(bestPerformers, 0);

		System.Array.Sort(bestPerformers, delegate (DNA lhs, DNA rhs)
		{
			return rhs.Fitness.CompareTo(lhs.Fitness);
		});
    }

	private DNA ChooseParent()
	{
		double randomNumber = random.NextDouble() * fitnessSum;

		for (int i = 0; i < Population.Count; i++)
		{
			if (randomNumber < Population[i].Fitness)
			{
				return Population[i];
			}

			randomNumber -= Population[i].Fitness;
		}

		return Population[random.Next(0, Population.Count)];
	}

	private DNA ChooseBestParent()
    {
		DNA best = bestPerformers[0];

		double randomNumber = random.NextDouble();

		if (randomNumber > 0.2 && randomNumber < 0.3)
			best = bestPerformers[1];
		else if (randomNumber > 0.1 && randomNumber < 0.2)
			best = bestPerformers[2];
		else if (randomNumber < 0.1)
			best = bestPerformers[3];

		return best;
	}
}
using System.Collections.Generic;

using UnityEngine;

public class GeneticAlgorithm 
{
	#region PRIVATE_FIELDS
	private List<Genome> population = new List<Genome>();
	private List<Genome> newPopulation = new List<Genome>();

	private float totalFitness = 0f;

	private int eliteCount = 0;
	private float mutationChance = 0.0f;
	private float mutationRate = 0.0f;
	#endregion

	#region CONSTRUCTORS
	public GeneticAlgorithm(int eliteCount, float mutationChance, float mutationRate)
	{
		this.eliteCount = eliteCount;
		this.mutationChance = mutationChance;
		this.mutationRate = mutationRate;
	}
	#endregion

	#region PUBLIC_METHODS
	public Genome[] GetRandomGenomes(int count, int genesCount)
	{
		Genome[] genomes = new Genome[count];

		for (int i = 0; i < count; i++)
		{
			genomes[i] = new Genome(genesCount);
		}

		return genomes;
	}

	public Genome[] Epoch(Genome[] oldGenomes)
	{
		totalFitness = 0f;

		population.Clear();
		newPopulation.Clear();

		population.AddRange(oldGenomes);
		population.Sort(HandleComparison);

		foreach (Genome g in population)
		{
			totalFitness += g.fitness;
		}

		SelectElite();

		while (newPopulation.Count < population.Count)
		{
			Crossover();
		}

		return newPopulation.ToArray();
	}

	public Genome RouletteSelection()
	{
		float rnd = Random.Range(0, Mathf.Max(totalFitness, 0));

		float fitness = 0;

		for (int i = 0; i < population.Count; i++)
		{
			fitness += Mathf.Max(population[i].fitness, 0);

			if (fitness >= rnd) return population[i];
		}

		return null;
	}
	#endregion

	#region PRIVATE_METHODS
	private void SelectElite()
	{
		for (int i = 0; i < eliteCount && newPopulation.Count < population.Count; i++)
		{
			newPopulation.Add(population[i]);
		}
	}

	private void Crossover()
	{
		Genome mom = RouletteSelection();
		Genome dad = RouletteSelection();

		Crossover(mom, dad, out Genome child1, out Genome child2);

		newPopulation.Add(child1);
		newPopulation.Add(child2);
	}

	private void Crossover(Genome mom, Genome dad, out Genome child1, out Genome child2)
	{
		child1 = new Genome();
		child2 = new Genome();

		child1.genome = new float[mom.genome.Length];
		child2.genome = new float[mom.genome.Length];

		int pivot = Random.Range(0, mom.genome.Length);

		for (int i = 0; i < pivot; i++)
		{
			child1.genome[i] = mom.genome[i];

			if (ShouldMutate())
            {
				child1.genome[i] += Random.Range(-mutationRate, mutationRate);
			}

			child2.genome[i] = dad.genome[i];

			if (ShouldMutate())
            {
				child2.genome[i] += Random.Range(-mutationRate, mutationRate);
			}
		}

		for (int i = pivot; i < mom.genome.Length; i++)
		{
			child2.genome[i] = mom.genome[i];

			if (ShouldMutate())
            {
				child2.genome[i] += Random.Range(-mutationRate, mutationRate);
			}

			child1.genome[i] = dad.genome[i];

			if (ShouldMutate())
            {
				child1.genome[i] += Random.Range(-mutationRate, mutationRate);
			}
		}
	}

	private bool ShouldMutate()
	{
		return Random.Range(0.0f, 1.0f) < mutationChance;
	}

	private int HandleComparison(Genome x, Genome y)
	{
		return x.fitness > y.fitness ? 1 : x.fitness < y.fitness ? -1 : 0;
	}
	#endregion
}
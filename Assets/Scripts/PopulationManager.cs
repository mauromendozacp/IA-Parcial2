using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

public class PopulationManager : MonoBehaviourSingleton<PopulationManager>
{
    #region PUBLIC_FIELDS
    [SerializeField] private TextAsset brainDataJson = null;

    [HideInInspector] public int PopulationCount = 0;

    [HideInInspector] public int Turns = 0;
    [HideInInspector] public int IterationCount = 0;

    [HideInInspector] public int EliteCount = 0;
    [HideInInspector] public float MutationChance = 0f;
    [HideInInspector] public float MutationRate = 0f;

    [HideInInspector] public int InputsCount = 0;
    [HideInInspector] public int HiddenLayers = 0;
    [HideInInspector] public int OutputsCount = 0;
    [HideInInspector] public int NeuronsCountPerHL = 0;
    [HideInInspector] public float Bias = 0f;
    [HideInInspector] public float P = 0f;

    [HideInInspector] public int generation = 0;
    [HideInInspector] public int turnsLeft = 0;

    [HideInInspector] public float bestFitness = 0f;
    [HideInInspector] public float avgFitness = 0f;
    [HideInInspector] public float worstFitness = 0f;
    #endregion

    #region PRIVATE_FIELDS
    private GeneticAlgorithm genAlg = null;

    private List<Genome> populations = new List<Genome>();
    private List<NeuralNetwork> brains = new List<NeuralNetwork>();
    #endregion

    #region UNITY_CALLS
    public override void Awake()
    {
        base.Awake();

        PopulationCount = 100;

        Turns = 20;
        IterationCount = 1;

        EliteCount = 4;
        MutationChance = 0.10f;
        MutationRate = 0.01f;

        InputsCount = 6;
        HiddenLayers = 1;
        OutputsCount = 2;
        NeuronsCountPerHL = 7;
        Bias = 1f;
        P = 0.5f;

        generation = 0;
        turnsLeft = 0;

        bestFitness = 0f;
        avgFitness = 0f;
        worstFitness = 0f;
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartSimulation(List<Agent> agents)
    {
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        generation = 0;
        turnsLeft = Turns;

        foreach (Agent agent in agents)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            brains.Add(brain);
            populations.Add(genome);

            agent.SetBrain(genome, brain);
        }
    }

    public void Epoch(List<Agent> agents)
    {
        generation++;
        turnsLeft = Turns;

        bestFitness = GetBestFitness();
        avgFitness = GetAvgFitness();
        worstFitness = GetWorstFitness();

        Genome[] newGenomes = genAlg.Epoch(populations.ToArray());
        populations.Clear();
        populations.AddRange(newGenomes);

        for (int i = 0; i < PopulationCount; i++)
        {
            NeuralNetwork brain = brains[i];
            brain.SetWeights(newGenomes[i].genome);
            agents[i].SetBrain(newGenomes[i], brain);
        }
    }

    public void SaveData()
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.SaveFilePanel("Save Brain Data", "", "brain_data.json", "json");
#endif

        if (string.IsNullOrEmpty(path)) return;

        //File.WriteAllText(path, dataJson);
    }

    public void LoadData(Action onStartGame)
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Select Brain Data", "", "json");
#endif

        if (string.IsNullOrEmpty(path)) return;

        onStartGame?.Invoke();
    }
    #endregion

    #region PRIVATE_METHODS
    private NeuralNetwork CreateBrain()
    {
        NeuralNetwork brain = new NeuralNetwork();

        brain.AddFirstNeuronLayer(InputsCount, Bias, P);

        for (int i = 0; i < HiddenLayers; i++)
        {
            brain.AddNeuronLayer(NeuronsCountPerHL, Bias, P);
        }

        brain.AddNeuronLayer(OutputsCount, Bias, P);

        return brain;
    }

    private float GetBestFitness()
    {
        float fitness = 0f;
        foreach (Genome g in populations)
        {
            if (fitness < g.fitness)
            {
                fitness = g.fitness;
            }
        }

        return fitness;
    }

    private float GetAvgFitness()
    {
        float fitness = 0f;
        foreach (Genome g in populations)
        {
            fitness += g.fitness;
        }

        return fitness / populations.Count;
    }

    private float GetWorstFitness()
    {
        float fitness = float.MaxValue;
        foreach (Genome g in populations)
        {
            if (fitness > g.fitness)
            {
                fitness = g.fitness;
            }
        }

        return fitness;
    }
    #endregion
}
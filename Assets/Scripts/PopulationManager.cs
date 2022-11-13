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

    [HideInInspector] public int agentNro = 0;
    [HideInInspector] public string agentTeam = string.Empty;
    [HideInInspector] public float agentFitness = 0f;
    [HideInInspector] public int foodsConsumed = 0;
    [HideInInspector] public int row = 0;
    [HideInInspector] public int column = 0;
    #endregion

    #region PRIVATE_FIELDS
    private GeneticAlgorithm genAlg = null;

    private List<Genome> populations = new List<Genome>();
    private List<NeuralNetwork> brains = new List<NeuralNetwork>();

    private List<Genome> savePopulations = new List<Genome>();
    private bool dataLoaded = false;
    #endregion

    #region UNITY_CALLS
    public override void Awake()
    {
        base.Awake();

        PopulationCount = 100;

        Turns = 50;
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

        agentNro = 0;
        agentTeam = string.Empty;
        agentFitness = 0f;
        foodsConsumed = 0;
        row = 0;
        column = 0;
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartSimulation(List<Agent> agents, bool dataLoaded)
    {
        this.dataLoaded = dataLoaded;
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

        generation = dataLoaded ? generation : 0;
        turnsLeft = dataLoaded ? 0 : Turns;

        for (int i = 0; i < agents.Count; i++)
        {
            NeuralNetwork brain = CreateBrain();
            Genome genome = dataLoaded ? savePopulations[i] : new Genome(brain.GetTotalWeightsCount());

            brain.SetWeights(genome.genome);
            brains.Add(brain);
            populations.Add(genome);

            agents[i].SetBrain(genome, brain);
        }
    }

    public void Epoch(List<Agent> agents)
    {
        if (dataLoaded) return;

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

    public void UpdateFollowChaimbotData(Chaimbot chaimbot)
    {
        if (chaimbot != null)
        {
            foodsConsumed = chaimbot.FoodsConsumed;
            row = chaimbot.Index.x;
            column = chaimbot.Index.y;
            agentFitness = chaimbot.Fitness;
        }
        else
        {
            agentNro = 0;
            foodsConsumed = 0;
            row = 0;
            column = 0;
            agentTeam = TEAM.NONE.ToString();
            agentFitness = 0f;
        }
    }

    public void SaveData()
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.SaveFilePanel("Save Brain Data", "", "brain_data.json", "json");
#endif

        if (string.IsNullOrEmpty(path)) return;

        BrainData data = new BrainData();
        data.genomes = populations;

        data.GenerationCount = generation;
        data.PopulationCount = PopulationCount;

        data.Turns = Turns;
        data.IterationCount = IterationCount;

        data.EliteCount = EliteCount;
        data.MutationChance = MutationChance;
        data.MutationRate = MutationRate;

        data.InputsCount = InputsCount;
        data.HiddenLayers = HiddenLayers;
        data.OutputsCount = OutputsCount;
        data.NeuronsCountPerHL = NeuronsCountPerHL;
        data.Bias = Bias;
        data.P = P;

        string dataJson = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, dataJson);
    }

    public void LoadData(Action<bool> onStartGame)
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Select Brain Data", "", "json");
#endif

        BrainData data = null;
        string dataJson;
        if (string.IsNullOrEmpty(path))
        {
            dataJson = brainDataJson == null ? string.Empty : brainDataJson.text;
        }
        else
        {
            dataJson = File.ReadAllText(path);
        }
        data = JsonUtility.FromJson<BrainData>(dataJson);

        if (data == null) return;

        savePopulations = data.genomes;

        generation = data.GenerationCount;
        PopulationCount = data.PopulationCount;

        Turns = data.Turns;
        IterationCount = data.IterationCount;

        EliteCount = data.EliteCount;
        MutationChance = data.MutationChance;
        MutationRate = data.MutationRate;

        InputsCount = data.InputsCount;
        HiddenLayers = data.HiddenLayers;
        OutputsCount = data.OutputsCount;
        NeuronsCountPerHL = data.NeuronsCountPerHL;
        Bias = data.Bias;
        P = data.P;

        onStartGame?.Invoke(true);
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
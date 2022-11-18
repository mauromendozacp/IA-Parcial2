using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

public class PopulationManager : MonoBehaviourSingleton<PopulationManager>
{
    #region PUBLIC_FIELDS    
    [Header("General Settings")]
    [SerializeField] private TextAsset brainDataJson = null;
    [Range(100, 150)] public int PopulationCount = 0;

    [Range(1, 500)] public int Turns = 0;
    [Range(1, 100)] public int IterationCount = 0;
    [Range(1, 100)] public int AgentMaxGeneration = 0;

    [Header("Team A Settings")]
    public int A_EliteCount = 0;
    public float A_MutationChance = 0f;
    public float A_MutationRate = 0f;

    public int A_InputsCount = 0;
    public int A_HiddenLayers = 0;
    public int A_OutputsCount = 0;
    public int A_NeuronsCountPerHL = 0;
    public float A_Bias = 0f;
    public float A_P = 0f;

    [Header("Team B Settings")]
    public int B_EliteCount = 0;
    public float B_MutationChance = 0f;
    public float B_MutationRate = 0f;

    public int B_InputsCount = 0;
    public int B_HiddenLayers = 0;
    public int B_OutputsCount = 0;
    public int B_NeuronsCountPerHL = 0;
    public float B_Bias = 0f;
    public float B_P = 0f;

    [HideInInspector] public int generation = 0;
    [HideInInspector] public int turnsLeft = 0;

    [HideInInspector] public float bestFitness = 0f;
    [HideInInspector] public float avgFitness = 0f;
    [HideInInspector] public float worstFitness = 0f;

    [HideInInspector] public int agentNro = 0;
    [HideInInspector] public string agentTeam = string.Empty;
    [HideInInspector] public float agentFitness = 0f;
    [HideInInspector] public int agentFoodsConsumed = 0;
    [HideInInspector] public int agentGeneration = 0;
    [HideInInspector] public int row = 0;
    [HideInInspector] public int column = 0;

    [HideInInspector] public int totalChaimbots = 0;
    [HideInInspector] public int totalFoodsConsumed = 0;
    [HideInInspector] public int totalDeaths = 0;

    [HideInInspector] public int chaimbotsA = 0;
    [HideInInspector] public int foodsA = 0;
    [HideInInspector] public int deathsA = 0;

    [HideInInspector] public int chaimbotsB = 0;
    [HideInInspector] public int foodsB = 0;
    [HideInInspector] public int deathsB = 0;
    #endregion

    #region PRIVATE_FIELDS
    private GeneticAlgorithm genAlgA = null;
    private GeneticAlgorithm genAlgB = null;

    private List<Genome> populations = new List<Genome>();
    private List<Genome> populationsA = new List<Genome>();
    private List<Genome> populationsB = new List<Genome>();

    private bool dataLoaded = false;
    private string dataPath = string.Empty;
    private string fileName = "/Data/brain_data.json";
    #endregion

    #region UNITY_CALLS
    public override void Awake()
    {
        base.Awake();

        generation = 0;
        turnsLeft = 0;

        bestFitness = 0f;
        avgFitness = 0f;
        worstFitness = 0f;

        agentNro = 0;
        agentTeam = string.Empty;
        agentFitness = 0f;
        agentFoodsConsumed = 0;
        agentGeneration = 0;
        row = 0;
        column = 0;

        totalChaimbots = 0;
        totalFoodsConsumed = 0;
        totalDeaths = 0;

        chaimbotsA = 0;
        foodsA = 0;
        deathsA = 0;

        chaimbotsB = 0;
        foodsB = 0;
        deathsB = 0;

        dataPath = Application.dataPath;
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartSimulation(List<Chaimbot> chaimbots, bool dataLoaded)
    {
        this.dataLoaded = dataLoaded;

        genAlgA = new GeneticAlgorithm(A_EliteCount, A_MutationChance, A_MutationRate);
        genAlgB = new GeneticAlgorithm(B_EliteCount, B_MutationChance, B_MutationRate);

        generation = dataLoaded ? generation : 0;
        turnsLeft = dataLoaded ? 0 : Turns;

        bestFitness = 0f;
        avgFitness = 0f;
        worstFitness = 0f;

        if (dataLoaded)
        {
            List<Chaimbot> chaimbotsA = chaimbots.FindAll(c => c.Team == TEAM.A);
            List<Chaimbot> chaimbotsB = chaimbots.FindAll(c => c.Team == TEAM.B);

            for (int i = 0; i < chaimbotsA.Count; i++)
            {
                NeuralNetwork brain = CreateBrainA();
                Genome genome = populationsA[i];

                brain.SetWeights(genome.genome);

                chaimbotsA[i].SetBrain(genome, brain);
            }

            for (int i = 0; i < chaimbotsB.Count; i++)
            {
                NeuralNetwork brain = CreateBrainB();
                Genome genome = populationsB[i];

                brain.SetWeights(genome.genome);

                chaimbotsB[i].SetBrain(genome, brain);
            }
        }
        else
        {
            populationsA.Clear();
            populationsB.Clear();

            for (int i = 0; i < chaimbots.Count; i++)
            {
                NeuralNetwork brain = null;
                Genome genome = null;

                if (chaimbots[i].Team == TEAM.A)
                {
                    brain = CreateBrainA();
                    genome = new Genome(brain.GetTotalWeightsCount());

                    brain.SetWeights(genome.genome);
                    populationsA.Add(genome);
                }
                else if (chaimbots[i].Team == TEAM.B)
                {
                    brain = CreateBrainB();
                    genome = new Genome(brain.GetTotalWeightsCount());

                    brain.SetWeights(genome.genome);
                    populationsB.Add(genome);
                }

                chaimbots[i].SetBrain(genome, brain);
            }
        }

        populations.AddRange(populationsA);
        populations.AddRange(populationsB);

        totalChaimbots = populations.Count;
        chaimbotsA = populationsA.Count;
        chaimbotsB = populationsB.Count;

        totalFoodsConsumed = 0;
        foodsA = 0;
        foodsB = 0;

        totalDeaths = 0;
        deathsA = 0;
        deathsB = 0;
    }

    public void Epoch(List<Chaimbot> chaimbots, Action<Genome[], NeuralNetwork[], TEAM> onCreateNewChaimbots)
    {
        if (dataLoaded) return;

        generation++;
        turnsLeft = Turns;

        bestFitness = GetBestFitness();
        avgFitness = GetAvgFitness();
        worstFitness = GetWorstFitness();

        populations.Clear();
        populationsA.Clear();
        populationsB.Clear();

        ExtinctChaimbots(chaimbots);
        SurvivingChaimbots(chaimbots);
        BreeadingChaimbots(chaimbots, onCreateNewChaimbots);
        
        populations.AddRange(populationsA);
        populations.AddRange(populationsB);

        totalChaimbots = populations.Count;
        chaimbotsA = populationsA.Count;
        chaimbotsB = populationsB.Count;

        foodsA = 0;
        foodsB = 0;
        deathsA = 0;
        deathsB = 0;
    }

    public void UpdateFollowChaimbotData(Chaimbot chaimbot)
    {
        if (chaimbot != null)
        {
            agentFoodsConsumed = chaimbot.FoodsConsumed;
            row = chaimbot.Index.x;
            column = chaimbot.Index.y;
            agentFitness = chaimbot.Fitness;
        }
        else
        {
            agentNro = 0;
            agentFoodsConsumed = 0;
            agentGeneration = 0;
            row = 0;
            column = 0;
            agentTeam = TEAM.NONE.ToString();
            agentFitness = 0f;
        }
    }

    public void AddFoodsConsumed(TEAM team)
    {
        totalFoodsConsumed++;

        if (team == TEAM.A)
        {
            foodsA++;
        }
        else
        {
            foodsB++;
        }
    }

    public void AddDeaths(TEAM team)
    {
        totalDeaths++;

        if (team == TEAM.A)
        {
            deathsA++;
        }
        else
        {
            deathsB++;
        }
    }

    public int GetPopulationACount()
    {
        return populationsA.Count;
    }
    public int GetPopulationBCount()
    {
        return populationsB.Count;
    }

    #region DATA
    public void SaveData()
    {
        string path = null;

#if UNITY_EDITOR
        path = EditorUtility.SaveFilePanel("Save Brain Data", "", "brain_data.json", "json");
        if (string.IsNullOrEmpty(path)) return;
#endif  

        if (string.IsNullOrEmpty(path))
        {
            path = dataPath + fileName;
        }

        BrainData data = new BrainData();
        data.genomesA = populationsA;
        data.genomesB = populationsB;

        data.GenerationCount = generation;
        data.PopulationCount = PopulationCount;

        data.Turns = Turns;

        data.A_EliteCount = A_EliteCount;
        data.A_MutationChance = A_MutationChance;
        data.A_MutationRate = A_MutationRate;

        data.A_InputsCount = A_InputsCount;
        data.A_HiddenLayers = A_HiddenLayers;
        data.A_OutputsCount = A_OutputsCount;
        data.A_NeuronsCountPerHL = A_NeuronsCountPerHL;
        data.A_Bias = A_Bias;
        data.A_P = A_P;

        data.B_EliteCount = B_EliteCount;
        data.B_MutationChance = B_MutationChance;
        data.B_MutationRate = B_MutationRate;

        data.B_InputsCount = B_InputsCount;
        data.B_HiddenLayers = B_HiddenLayers;
        data.B_OutputsCount = B_OutputsCount;
        data.B_NeuronsCountPerHL = B_NeuronsCountPerHL;
        data.B_Bias = B_Bias;
        data.B_P = B_P;

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

        populations.Clear();
        populationsA.Clear();
        populationsB.Clear();

        populationsA = data.genomesA;
        populationsB = data.genomesB;

        generation = data.GenerationCount;
        PopulationCount = data.PopulationCount;

        Turns = data.Turns;

        A_EliteCount = data.A_EliteCount;
        A_MutationChance = data.A_MutationChance;
        A_MutationRate = data.A_MutationRate;

        A_InputsCount = data.A_InputsCount;
        A_HiddenLayers = data.A_HiddenLayers;
        A_OutputsCount = data.A_OutputsCount;
        A_NeuronsCountPerHL = data.A_NeuronsCountPerHL;
        A_Bias = data.A_Bias;
        A_P = data.A_P;

        B_EliteCount = data.B_EliteCount;
        B_MutationChance = data.B_MutationChance;
        B_MutationRate = data.B_MutationRate;

        B_InputsCount = data.B_InputsCount;
        B_HiddenLayers = data.B_HiddenLayers;
        B_OutputsCount = data.B_OutputsCount;
        B_NeuronsCountPerHL = data.B_NeuronsCountPerHL;
        B_Bias = data.B_Bias;
        B_P = data.B_P;

        onStartGame?.Invoke(true);
    }
    #endregion

    #endregion

    #region PRIVATE_METHODS
    private NeuralNetwork CreateBrainA()
    {
        NeuralNetwork brain = new NeuralNetwork();

        brain.AddFirstNeuronLayer(A_InputsCount, A_Bias, A_P);

        for (int i = 0; i < A_HiddenLayers; i++)
        {
            brain.AddNeuronLayer(A_NeuronsCountPerHL, A_Bias, A_P);
        }

        brain.AddNeuronLayer(A_OutputsCount, A_Bias, A_P);

        return brain;
    }

    private NeuralNetwork CreateBrainB()
    {
        NeuralNetwork brain = new NeuralNetwork();

        brain.AddFirstNeuronLayer(B_InputsCount, B_Bias, B_P);

        for (int i = 0; i < B_HiddenLayers; i++)
        {
            brain.AddNeuronLayer(B_NeuronsCountPerHL, B_Bias, B_P);
        }

        brain.AddNeuronLayer(B_OutputsCount, B_Bias, B_P);

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
        fitness = populations.Count == 0 ? 0f : fitness / populations.Count;

        return fitness;
    }

    private float GetWorstFitness()
    {
        float fitness = populations.Count == 0 ? 0f : float.MaxValue;
        foreach (Genome g in populations)
        {
            if (fitness > g.fitness)
            {
                fitness = g.fitness;
            }
        }

        return fitness;
    }

    private void ExtinctChaimbots(List<Chaimbot> chaimbots)
    {
        List<Chaimbot> extinctChaimbots = chaimbots.FindAll(c => c.FoodsConsumed == 0 || c.Dead || c.GenerationCount > AgentMaxGeneration);

        for (int i = 0; i < extinctChaimbots.Count; i++)
        {
            Destroy(extinctChaimbots[i].gameObject);
            chaimbots.Remove(extinctChaimbots[i]);
        }
    }

    private void SurvivingChaimbots(List<Chaimbot> chaimbots)
    {
        List<Chaimbot> survivingChaimbots = chaimbots.FindAll(c => c.FoodsConsumed >= 1);

        List<Chaimbot> survivingChaimbotsA = survivingChaimbots.FindAll(c => c.Team == TEAM.A);
        List<Chaimbot> survivingChaimbotsB = survivingChaimbots.FindAll(c => c.Team == TEAM.B);

        List<Genome> survivingGenomesA = new List<Genome>();
        for (int i = 0; i < survivingChaimbotsA.Count; i++)
        {
            survivingGenomesA.Add(survivingChaimbotsA[i].Genome);
        }

        List<Genome> survivingGenomesB = new List<Genome>();
        for (int i = 0; i < survivingChaimbotsB.Count; i++)
        {
            survivingGenomesB.Add(survivingChaimbotsB[i].Genome);
        }

        populationsA.AddRange(survivingGenomesA);
        populationsB.AddRange(survivingGenomesB);
    }

    private void BreeadingChaimbots(List<Chaimbot> chaimbots, Action<Genome[], NeuralNetwork[], TEAM> onCreateNewChaimbots)
    {
        List<Chaimbot> breedingChaimbots = chaimbots.FindAll(c => c.FoodsConsumed >= 2);

        List<Chaimbot> breedingChaimbotsA = breedingChaimbots.FindAll(c => c.Team == TEAM.A);
        List<Chaimbot> breedingChaimbotsB = breedingChaimbots.FindAll(c => c.Team == TEAM.B);

        List<Genome> breedingGenomesA = new List<Genome>();
        for (int i = 0; i < breedingChaimbotsA.Count; i++)
        {
            breedingGenomesA.Add(breedingChaimbotsA[i].Genome);
        }

        List<Genome> breedingGenomesB = new List<Genome>();
        for (int i = 0; i < breedingChaimbotsB.Count; i++)
        {
            breedingGenomesB.Add(breedingChaimbotsB[i].Genome);
        }

        Genome[] newGenomesA = null;
        if (breedingGenomesA.Count >= 2)
        {
            if (breedingChaimbotsA.Count % 2 != 0)
            {
                breedingChaimbotsA.RemoveAt(breedingChaimbotsA.Count - 1);
            }

            newGenomesA = genAlgA.Epoch(breedingGenomesA.ToArray());
        }

        Genome[] newGenomesB = null;
        if (breedingGenomesB.Count >= 2)
        {
            if (breedingGenomesB.Count % 2 != 0)
            {
                breedingGenomesB.RemoveAt(breedingGenomesB.Count - 1);
            }

            newGenomesB = genAlgB.Epoch(breedingGenomesB.ToArray());
        }

        if (newGenomesA != null)
        {
            NeuralNetwork[] brainsA = new NeuralNetwork[newGenomesA.Length];
            for (int i = 0; i < brainsA.Length; i++)
            {
                brainsA[i] = CreateBrainA();
            }
            onCreateNewChaimbots?.Invoke(newGenomesA, brainsA, TEAM.A);
            populationsA.AddRange(newGenomesA);
        }

        if (newGenomesB != null)
        {
            NeuralNetwork[] brainsB = new NeuralNetwork[newGenomesB.Length];
            for (int i = 0; i < brainsB.Length; i++)
            {
                brainsB[i] = CreateBrainB();
            }
            onCreateNewChaimbots?.Invoke(newGenomesB, brainsB, TEAM.B);
            populationsB.AddRange(newGenomesB);
        }
    }
    #endregion
}
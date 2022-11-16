using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

public class PopulationManager : MonoBehaviourSingleton<PopulationManager>
{
    #region PUBLIC_FIELDS
    [SerializeField] private TextAsset brainDataJson = null;

    [Range(100, 150)] public int PopulationCount = 0;

    [Range(1, 500)] public int Turns = 0;
    [Range(1, 100)] public int IterationCount = 0;

    public int EliteCount = 0;
    public float MutationChance = 0f;
    public float MutationRate = 0f;

    public int InputsCount = 0;
    public int HiddenLayers = 0;
    public int OutputsCount = 0;
    public int NeuronsCountPerHL = 0;
    public float Bias = 0f;
    public float P = 0f;

    [HideInInspector] public int generation = 0;
    [HideInInspector] public int turnsLeft = 0;

    [HideInInspector] public float bestFitness = 0f;
    [HideInInspector] public float avgFitness = 0f;
    [HideInInspector] public float worstFitness = 0f;

    [HideInInspector] public int agentNro = 0;
    [HideInInspector] public string agentTeam = string.Empty;
    [HideInInspector] public float agentFitness = 0f;
    [HideInInspector] public int agentFoodsConsumed = 0;
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
    private GeneticAlgorithm genAlg = null;

    private List<Genome> populations = new List<Genome>();
    private List<Genome> populationsA = new List<Genome>();
    private List<Genome> populationsB = new List<Genome>();

    private bool dataLoaded = false;
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
    }
    #endregion

    #region PUBLIC_METHODS
    public void StartSimulation(List<Chaimbot> chaimbots, bool dataLoaded)
    {
        this.dataLoaded = dataLoaded;
        genAlg = new GeneticAlgorithm(EliteCount, MutationChance, MutationRate);

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
                NeuralNetwork brain = CreateBrain();
                Genome genome = populationsA[i];

                brain.SetWeights(genome.genome);

                chaimbotsA[i].SetBrain(genome, brain);
            }

            for (int i = 0; i < chaimbotsB.Count; i++)
            {
                NeuralNetwork brain = CreateBrain();
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
                NeuralNetwork brain = CreateBrain();
                Genome genome = new Genome(brain.GetTotalWeightsCount());

                brain.SetWeights(genome.genome);

                if (chaimbots[i].Team == TEAM.A)
                {
                    populationsA.Add(genome);
                }
                else if (chaimbots[i].Team == TEAM.B)
                {
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
#endif

        if (string.IsNullOrEmpty(path)) return;

        BrainData data = new BrainData();
        data.genomesA = populationsA;
        data.genomesB = populationsB;

        data.GenerationCount = generation;
        data.PopulationCount = PopulationCount;

        data.Turns = Turns;

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

        populations.Clear();
        populationsA.Clear();
        populationsB.Clear();

        populationsA = data.genomesA;
        populationsB = data.genomesB;

        generation = data.GenerationCount;
        PopulationCount = data.PopulationCount;

        Turns = data.Turns;

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
        List<Chaimbot> extinctChaimbots = chaimbots.FindAll(c => c.FoodsConsumed == 0 || c.Dead);

        for (int i = 0; i < extinctChaimbots.Count; i++)
        {
            Destroy(extinctChaimbots[i].gameObject);
            chaimbots.Remove(extinctChaimbots[i]);
        }
    }

    private void SurvivingChaimbots(List<Chaimbot> chaimbots)
    {
        List<Chaimbot> survivingChaimbots = chaimbots.FindAll(c => c.FoodsConsumed == 1);

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

        Genome[] newGenomesA = genAlg.Epoch(breedingGenomesA.ToArray());
        Genome[] newGenomesB = genAlg.Epoch(breedingGenomesB.ToArray());

        NeuralNetwork[] brainsA = new NeuralNetwork[newGenomesA.Length];
        for (int i = 0; i < brainsA.Length; i++)
        {
            brainsA[i] = CreateBrain();
        }

        NeuralNetwork[] brainsB = new NeuralNetwork[newGenomesB.Length];
        for (int i = 0; i < brainsB.Length; i++)
        {
            brainsB[i] = CreateBrain();
        }

        onCreateNewChaimbots?.Invoke(newGenomesA, brainsA, TEAM.A);
        onCreateNewChaimbots?.Invoke(newGenomesB, brainsB, TEAM.B);

        populationsA.AddRange(newGenomesA);
        populationsB.AddRange(newGenomesB);
    }
    #endregion
}
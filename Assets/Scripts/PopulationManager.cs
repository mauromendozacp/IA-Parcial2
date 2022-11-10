using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PopulationManager : MonoBehaviourSingleton<PopulationManager>
{
    #region PUBLIC_FIELDS
    [SerializeField] private TextAsset brainDataJson = null;

    [HideInInspector] public int PopulationCount = 40;

    [HideInInspector] public float GenerationDuration = 20.0f;
    [HideInInspector] public int IterationCount = 1;

    [HideInInspector] public int EliteCount = 4;
    [HideInInspector] public float MutationChance = 0.10f;
    [HideInInspector] public float MutationRate = 0.01f;

    [HideInInspector] public int InputsCount = 6;
    [HideInInspector] public int HiddenLayers = 1;
    [HideInInspector] public int OutputsCount = 2;
    [HideInInspector] public int NeuronsCountPerHL = 7;
    [HideInInspector] public float Bias = 1f;
    [HideInInspector] public float P = 0.5f;

    [HideInInspector] public int generation = 0;
    #endregion

    #region PRIVATE_FIELDS
    private GeneticAlgorithm genAlg = null;

    private List<Genome> populations = new List<Genome>();
    private List<NeuralNetwork> brains = new List<NeuralNetwork>();
    #endregion

    #region PUBLIC_METHODS
    public float GetBestFitness()
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

    public float GetAvgFitness()
    {
        float fitness = 0f;
        foreach (Genome g in populations)
        {
            fitness += g.fitness;
        }

        return fitness / populations.Count;
    }

    public float GetWorstFitness()
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
}
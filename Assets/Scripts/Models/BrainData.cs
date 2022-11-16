using System.Collections.Generic;

[System.Serializable]
public class BrainData
{
    public List<Genome> genomesA = null;
    public List<Genome> genomesB = null;

    public int GenerationCount = 0;
    public int PopulationCount = 0;
    public int Turns = 0;
    public int EliteCount = 0;
    public float MutationChance = 0f;
    public float MutationRate = 0f;
    public int InputsCount = 0;
    public int HiddenLayers = 0;
    public int OutputsCount = 0;
    public int NeuronsCountPerHL = 0;
    public float Bias = 0;
    public float P = 0f;
}
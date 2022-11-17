using System.Collections.Generic;

[System.Serializable]
public class BrainData
{
    public List<Genome> genomesA = null;
    public List<Genome> genomesB = null;

    public int GenerationCount = 0;
    public int PopulationCount = 0;
    public int Turns = 0;

    public int A_EliteCount = 0;
    public float A_MutationChance = 0f;
    public float A_MutationRate = 0f;

    public int A_InputsCount = 0;
    public int A_HiddenLayers = 0;
    public int A_OutputsCount = 0;
    public int A_NeuronsCountPerHL = 0;
    public float A_Bias = 0;
    public float A_P = 0f;

    public int B_EliteCount = 0;
    public float B_MutationChance = 0f;
    public float B_MutationRate = 0f;

    public int B_InputsCount = 0;
    public int B_HiddenLayers = 0;
    public int B_OutputsCount = 0;
    public int B_NeuronsCountPerHL = 0;
    public float B_Bias = 0;
    public float B_P = 0f;
}
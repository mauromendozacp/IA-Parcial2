using UnityEngine;

public enum TEAM
{
    NONE,
    A,
    B
}

public abstract class Agent : MonoBehaviour
{
    #region PROTECTED_FIELDS
    protected Genome genome = null;
    protected NeuralNetwork brain = null;

    protected TEAM team = TEAM.NONE;
    protected float[] inputs = null;
    protected float fitness = 1f;
    #endregion

    #region PROPERTIES
    public float Fitness { get => fitness; }
    #endregion

    #region PUBLIC_METHODS
    public void SetBrain(Genome genome, NeuralNetwork brain)
    {
        this.genome = genome;
        this.brain = brain;
        inputs = new float[brain.InputsCount];

        OnReset();
    }

    public void OnThink()
    {
        ProcessInputs();

        float[] outputs = brain.Synapsis(inputs);

        ProcessOutputs(outputs);
    }

    public void SetGoodFitness()
    {
        fitness *= 2f;
        genome.fitness = fitness;
    }
    #endregion

    #region PROTECTED_METHODS
    protected virtual void OnReset()
    {
        fitness = 1f;
    }

    protected abstract void ProcessInputs();

    protected abstract void ProcessOutputs(float[] outputs);
    #endregion
}
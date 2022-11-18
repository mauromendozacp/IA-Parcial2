using UnityEngine;

public enum TEAM
{
    NONE,
    A,
    B,

    COUNT
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
    public Genome Genome { get => genome; }
    public NeuralNetwork Brain { get => brain; }
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

    public void UpdateFitness(float fitness)
    {
        this.fitness *= fitness;

        if (this.fitness < 1f)
        {
            this.fitness = 1f;
        }

        genome.fitness = this.fitness;
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
public enum TEAM
{
    NONE,
    A,
    B,

    COUNT
}

public abstract class Agent : BehaviourTree
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
    public float[] Inputs { get => inputs; }
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

    public void UpdateFitness(float fitness)
    {
        this.fitness *= fitness;
        SetGenomeFitness();
    }

    public void SetFitness(float fitness)
    {
        this.fitness = fitness;
        SetGenomeFitness();
    }

    public void SetInput(int index, float value)
    {
        if (index >= 0 && index < inputs.Length)
        {
            inputs[index] = value;
        }
    }
    #endregion

    #region PROTECTED_METHODS
    protected virtual void OnReset()
    {
        fitness = 1f;
    }
    #endregion

    #region PRIVATE_METHODS
    private void SetGenomeFitness()
    {
        genome.fitness = fitness;
    }
    #endregion
}
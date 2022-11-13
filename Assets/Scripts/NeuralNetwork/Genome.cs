using UnityEngine;

[System.Serializable]
public class Genome
{
	#region PUBLIC_FIELDS
	public float[] genome = null;
	[System.NonSerialized] public float fitness = 0;
	#endregion

	#region CONSTRUCTORS
	public Genome()
	{
		genome = null;
		fitness = 0;
	}

	public Genome(float[] genome)
	{
		this.genome = genome;
		fitness = 0;
	}

	public Genome(int genesCount)
	{
		genome = new float[genesCount];

		for (int i = 0; i < genesCount; i++)
        {
			genome[i] = Random.Range(-1.0f, 1.0f);
		}

		fitness = 0;
	}
	#endregion
}
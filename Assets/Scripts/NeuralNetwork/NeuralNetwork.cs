using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class NeuralNetwork
{
	#region PUBLIC_FIELDS
	public List<NeuronLayer> layers = new List<NeuronLayer>();
	public int totalWeightsCount = 0;
	public int inputsCount = 0;
	#endregion

	#region PROPERTIES
	public int InputsCount
	{
		get { return inputsCount; }
	}
	#endregion

	#region PUBLIC_METHODS
	public bool AddNeuronLayer(int neuronsCount, float bias, float p)
	{
		if (layers.Count == 0)
		{
			Debug.LogError("Call AddFirstNeuronLayer(int inputsCount, float bias, float p) for the first layer.");
			return false;
		}

		return AddNeuronLayer(layers[layers.Count - 1].OutputsCount, neuronsCount, bias, p);
	}

	public bool AddFirstNeuronLayer(int inputsCount, float bias, float p)
	{
		if (layers.Count != 0)
		{
			Debug.LogError("Call AddNeuronLayer(int neuronCount, float bias, float p) for the rest of the layers.");
			return false;
		}

		this.inputsCount = inputsCount;

		return AddNeuronLayer(inputsCount, inputsCount, bias, p);
	}

	private bool AddNeuronLayer(int inputsCount, int neuronsCount, float bias, float p)
	{
		if (layers.Count > 0 && layers[layers.Count - 1].OutputsCount != inputsCount)
		{
			Debug.LogError("Inputs Count must match outputs from previous layer.");
			return false;
		}

		NeuronLayer layer = new NeuronLayer(inputsCount, neuronsCount, bias, p);

		totalWeightsCount += (inputsCount + 1) * neuronsCount;

		layers.Add(layer);

		return true;
	}

	public int GetTotalWeightsCount()
	{
		return totalWeightsCount;
	}

	public void SetWeights(float[] newWeights)
	{
		int fromId = 0;

		for (int i = 0; i < layers.Count; i++)
		{
			fromId = layers[i].SetWeights(newWeights, fromId);
		}
	}

	public float[] GetWeights()
	{
		float[] weights = new float[totalWeightsCount];
		int id = 0;

		for (int i = 0; i < layers.Count; i++)
		{
			float[] ws = layers[i].GetWeights();

			for (int j = 0; j < ws.Length; j++)
			{
				weights[id] = ws[j];
				id++;
			}
		}

		return weights;
	}

	public float[] Synapsis(float[] inputs)
	{
		float[] outputs = null;

		for (int i = 0; i < layers.Count; i++)
		{
			outputs = layers[i].Synapsis(inputs);
			inputs = outputs;
		}

		return outputs;
	}
	#endregion
}
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField] private Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 3);
    [SerializeField] private Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);
    [SerializeField] private List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    [SerializeField] public List<Matrix<float>> weights = new List<Matrix<float>>();                     
    [SerializeField] public List<float> biases = new List<float>();

    public (float, float) RunNeuralNetwork(float right, float forward, float left)
    {
        inputLayer[0, 0] = right;
        inputLayer[0, 1] = forward;
        inputLayer[0, 2] = left;

        //Using PointwiseTanh to get the hyperbolic tangent of each distance
        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)    //If using more than 1 hidden layer, apply the corresponding weights and biases
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        //Using the sigmoid function to get a normalised result for acceleration, as it should never be greater than 1.0
        //Applying the tangent function again to get the true steering value, which should always be between -1.0 and 1.0
        return (SigmoidFunction(outputLayer[0, 0]), (float)Math.Tanh(outputLayer[0, 1]));
    }

    public void SetupNeuralNetwork()
    {
        ResetNeuralNetwork();
        //Initialising weights and biases to be random at launch
        RandomiseBiases();
        RandomiseWeights();
    }

    public void ResetNeuralNetwork()
    {
        inputLayer.Clear();
        outputLayer.Clear();
        hiddenLayers.Clear();
        weights.Clear();
        biases.Clear();

        hiddenLayers.Add(Matrix<float>.Build.Dense(1, 3));

        weights.Add(Matrix<float>.Build.Dense(3, 3));
        weights.Add(Matrix<float>.Build.Dense(3, 3));
        weights.Add(Matrix<float>.Build.Dense(3, 3));
    }

    private void RandomiseWeights()
    {
        for(int k = 0; k < weights.Count; k++)
        {
            for(int j = 0; j < weights[k].RowCount; j++)
            {
                for(int i = 0; i < weights[k].ColumnCount; i++)
                {
                    weights[k][i, j] = Random.Range(-1.0f, 1.0f);
                }
            }
        }
    }

    private void RandomiseBiases()
    {
        biases.Add(Random.Range(-1.0f, 1.0f));
        biases.Add(Random.Range(-1.0f, 1.0f));
    }

    private float SigmoidFunction(float s) { return (1.0f / (1.0f + Mathf.Exp(-s))); }
}
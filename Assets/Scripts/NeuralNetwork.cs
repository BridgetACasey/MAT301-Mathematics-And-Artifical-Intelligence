using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class NeuralNetwork
{
    private Matrix<float> inputLayer;
    private Matrix<float> outputLayer;
    private List<Matrix<float>> hiddenLayers;
}
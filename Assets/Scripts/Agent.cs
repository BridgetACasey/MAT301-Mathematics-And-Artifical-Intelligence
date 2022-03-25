using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 previousPosition;
    [SerializeField] private Vector3 startRotation;

    [SerializeField] private Vector3 goalPosition;

    [SerializeField] private float elapsedTime;
    [SerializeField] private float distanceToGoal;
    [SerializeField] private float distanceFromStart;
    [SerializeField] private float distanceTravelled;
    [SerializeField] private float overallFitness;

    [SerializeField] private float moveSpeed = 12.0f;
    [SerializeField] private float turnScale = 0.05f;

    [Range(-1.0f, 1.0f)]
    [SerializeField] private float acceleration = 1.0f;
    [Range(-1.0f, 1.0f)]
    [SerializeField] private float turnRate;

    [SerializeField] private Vector3 input;

    [SerializeField] private bool running = false;
    [SerializeField] private bool driving = false;

    [SerializeField] private NeuralNetwork neuralNetwork;

    private float distanceLeft, distanceForward, distanceRight; //The distance in each direction to the nearest wall

    void Update()
    {
        if (running)
        {
            if (elapsedTime > 6.0f)    //Cuts the agent off if they're taking too long or driving in circles
                StopDriving();

            if (driving)
            {
                elapsedTime += Time.deltaTime;
                CalculateOverallFitness();
                Drive();
            }
        }
    }

    private void FixedUpdate()
    {
        CalculateDirectionalDistance();

        (acceleration, turnRate) = neuralNetwork.RunNeuralNetwork(distanceRight, distanceForward, distanceLeft);
    }

    public void SetupAttributes()
    {
        driving = true;
        startPosition = transform.position;
        previousPosition = startPosition;
        startRotation = transform.eulerAngles;
        neuralNetwork.SetupNeuralNetwork();
    }

    public void ResetAttributes()
    {
        elapsedTime = 0.0f;
        distanceToGoal = 0.0f;
        distanceFromStart = 0.0f;
        distanceTravelled = 0.0f;
        transform.position = startPosition;
        transform.Rotate(startRotation);
        overallFitness = 0.0f;
        driving = true;
    }

    private void CalculateOverallFitness()
    {
        overallFitness = distanceFromStart - elapsedTime;
    }

    private void CalculateDirectionalDistance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
            distanceForward = hit.distance / 12.0f;

        ray.direction = transform.forward + transform.right;

        if (Physics.Raycast(ray, out hit))
            distanceRight = hit.distance / 12.0f;

        ray.direction = transform.forward - transform.right;

        if (Physics.Raycast(ray, out hit))
            distanceLeft = hit.distance / 12.0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
            StopDriving();
    }

    public void Drive()
    {
        previousPosition = transform.position;

        input = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, acceleration * moveSpeed), 0.02f);
        input = transform.TransformDirection(input);
        transform.position += input;

        transform.eulerAngles += new Vector3(0, (turnRate * 90.0f) * turnScale, 0);

        distanceTravelled += (previousPosition - transform.position).magnitude;
        distanceFromStart = (startPosition - transform.position).magnitude;
    }

    public void StopDriving()
    {
        driving = false;
        distanceTravelled += (previousPosition - transform.position).magnitude;
        distanceFromStart = (startPosition - transform.position).magnitude;
        distanceToGoal = (goalPosition - transform.position).magnitude;
        CalculateOverallFitness();
        //Debug.Log("Fitness: " + overallFitness);
        //Debug.Log("Elapsed Time: " + elapsedTime + " Distance to Goal: " + distanceToGoal);
        //Debug.Log("Forward: " + distanceForward + " Left: " + distanceLeft + " Right: " + distanceRight);
    }

    public void SetGoalPosition(Vector3 position) { goalPosition = position; }

    public void SetRunning(bool run) { running = run; }

    public bool GetDriving() { return driving; }

    public float GetOverallFitness() { return overallFitness; }
}
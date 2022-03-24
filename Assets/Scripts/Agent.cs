using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 startRotation;

    [SerializeField] private Vector3 goalPosition;

    [SerializeField] private float elapsedTime;
    [SerializeField] private float distanceToGoal;
    [SerializeField] private float overallFitness;

    [Range(-1.0f, 1.0f)]
    [SerializeField] private float acceleration = 1.0f;
    [Range(-1.0f, 1.0f)]
    [SerializeField] private float turnRate;

    [SerializeField] private float moveSpeed = 8.0f;
    [SerializeField] private float turnSpeed = 1.0f;

    [SerializeField] bool driving;

    private float distanceLeft, distanceForward, distanceRight; //The distance in each direction to the nearest wall

    void Update()
    {
        if (driving)
        {
            elapsedTime += Time.deltaTime;
            Drive();
        }
    }

    private void FixedUpdate()
    {
        CalculateDirectionalDistance();
    }

    public void SetupAttributes()
    {
        driving = true;
        startPosition = transform.position;
        startRotation = transform.eulerAngles;
    }

    public void ResetAttributes()
    {
        elapsedTime = 0.0f;
        distanceToGoal = 0.0f;
        transform.position = startPosition;
        transform.Rotate(startRotation);
        driving = true;
    }

    private void CalculateOverallFitness()
    {
        overallFitness = distanceToGoal + elapsedTime + ((distanceForward + distanceLeft + distanceRight) / 3.0f);
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
        {
            driving = false;
            distanceToGoal = (goalPosition - transform.position).magnitude;
            CalculateOverallFitness();
            Debug.Log("Elapsed Time: " + elapsedTime + " Distance to Goal: " + distanceToGoal);
            Debug.Log("Forward: " + distanceForward + " Left: " + distanceLeft + " Right: " + distanceRight);
        }
    }

    public void Drive()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
        transform.eulerAngles += new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void SetGoalPosition(Vector3 position) { goalPosition = position; }

    public bool GetDriving() { return driving; }

    public float GetOverallFitness() { return overallFitness; }
}
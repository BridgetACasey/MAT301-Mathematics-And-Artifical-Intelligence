using UnityEngine;

public enum AgentStatus
{
    NONE = 0,
    CRASHED,
    TIMEDOUT,
    COMPLETED
}

public class Agent : MonoBehaviour
{
    [SerializeField] private AgentStatus agentStatus;
    
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 previousPosition;
    [SerializeField] private Vector3 startRotation;

    [SerializeField] private float elapsedTime;
    [SerializeField] private float distanceFromStart;
    [SerializeField] private float distanceTravelled;
    [SerializeField] private float overallFitness;

    [SerializeField] private float moveSpeed = 24.0f;
    [SerializeField] private float turnScale = 6.0f;

    [Range(-1.0f, 1.0f)]
    [SerializeField] private float acceleration = 1.0f;
    [Range(-1.0f, 1.0f)]
    [SerializeField] private float steering = 0.0f;

    [SerializeField] private Vector3 input;

    [SerializeField] private bool running = false;
    [SerializeField] private bool driving = false;
    [SerializeField] private bool reachedGoal = false;

    [SerializeField] private NeuralNetwork neuralNetwork;

    private float distanceLeft, distanceForward, distanceRight; //The distance in each direction to the nearest wall

    void Update()
    {
        if (running)
        {
            if (elapsedTime > 30.0f || (distanceFromStart < 3.0f && elapsedTime > 8.0f))    //Cuts the agent off if they're taking too long or just driving in circles
            {
                agentStatus = AgentStatus.TIMEDOUT;
                StopDriving();
            }

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

        (acceleration, steering) = neuralNetwork.RunNeuralNetwork(distanceRight, distanceForward, distanceLeft);
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
        agentStatus = AgentStatus.NONE;
        elapsedTime = 0.0f;
        acceleration = 1.0f;
        steering = 0.0f;
        input = new Vector3(0.0f, 0.0f, 0.0f);
        distanceLeft = 0.0f;
        distanceForward = 0.0f;
        distanceRight = 0.0f;
        distanceFromStart = 0.0f;
        distanceTravelled = 0.0f;
        transform.position = startPosition;
        previousPosition = startPosition;
        transform.eulerAngles = startRotation;
        overallFitness = 0.0f;
        reachedGoal = false;
        driving = true;
    }

    private void CalculateOverallFitness()
    {
        overallFitness = distanceFromStart + (distanceTravelled * 0.05f) - elapsedTime;
        //overallFitness = distanceFromStart - elapsedTime;
    }

    private void CalculateDirectionalDistance()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        float trackWidth = 12.0f;

        if (Physics.Raycast(ray, out hit))
            distanceForward = hit.distance / trackWidth;

        ray.direction = transform.forward + transform.right;
        //ray.direction = transform.right;

        if (Physics.Raycast(ray, out hit))
            distanceRight = hit.distance / trackWidth;

        ray.direction = transform.forward - transform.right;
        //ray.direction = -transform.right;

        if (Physics.Raycast(ray, out hit))
            distanceLeft = hit.distance / trackWidth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            agentStatus = AgentStatus.CRASHED;
            StopDriving();
        }
        else if (collision.gameObject.tag == "Goal")
        {
            agentStatus = AgentStatus.COMPLETED;
            reachedGoal = true;
            StopDriving();
        }
    }

    public void Drive()
    {
        previousPosition = transform.position;

        input = Vector3.Lerp(Vector3.zero, new Vector3(0.0f, 0.0f, acceleration * moveSpeed), 0.95f);
        input = transform.TransformDirection(input);
        transform.position += (input * Time.deltaTime);

        transform.eulerAngles += (new Vector3(0.0f, (steering * 90.0f) * turnScale, 0.0f) * Time.deltaTime);

        distanceTravelled += (previousPosition - transform.position).magnitude;
        distanceFromStart = (startPosition - transform.position).magnitude;
    }

    public void StopDriving()
    {
        driving = false;
        distanceTravelled += (previousPosition - transform.position).magnitude;
        distanceFromStart = (startPosition - transform.position).magnitude;
        CalculateOverallFitness();

        if (reachedGoal)
            overallFitness += 50.0f;
    }

    public void SetStatus(AgentStatus status) { agentStatus = status; }

    public AgentStatus GetStatus() { return agentStatus; }

    public void SetRunning(bool run) { running = run; }

    public bool GetDriving() { return driving; }

    public bool GetReachedGoal() { return reachedGoal; }

    public float GetOverallFitness() { return overallFitness; }

    public float GetElapsedTime() { return elapsedTime; }

    public float GetDistanceFromStart() { return distanceFromStart; }

    public float GetDistanceTravelled() { return distanceTravelled; }
}
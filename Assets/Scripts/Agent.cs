using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public float distanceToGoal { get; private set; }

    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField]private float turnSpeed = 1.0f;

    void Start()
    {
    }

    void Update()
    {
        MoveForward();
        //MoveBackward();
        //TurnLeft();
        //TurnRight();
    }

    public void MoveForward()
    {
        transform.position += transform.forward * Time.deltaTime * moveSpeed;
    }

    public void MoveBackward()
    {
        transform.position -= transform.forward * Time.deltaTime * moveSpeed;
    }

    public void TurnLeft()
    {
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -turnSpeed);
    }

    public void TurnRight()
    {
        transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), turnSpeed);
    }
}
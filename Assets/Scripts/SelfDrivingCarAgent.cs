using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class SelfDrivingCarAgent : Agent
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;
    [SerializeField] private bool ignoreBrake;
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private CarController carController;

    [SerializeField] private float timeToCheckpoint;
    public float timer;
    public float timerStopPenalty;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        carController = transform.GetComponent<CarController>();

        trackCheckpoints.OnAgentCorrectCheckpoint += TrackCheckpoints_OnAgentCorrectCheckpoint;
        trackCheckpoints.OnAgentWrongCheckpoint += TrackCheckpoints_OnAgentWrongCheckpoint;

        timer = 0f;
        timerStopPenalty = 0f;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer >= timeToCheckpoint)
        {
            if (timer >= timeToCheckpoint * 2)
            {
                EndEpisode();
            } else {
                timerStopPenalty += Time.deltaTime;
                if (timerStopPenalty >= 2f)
                {
                    timerStopPenalty = 0f;
                    AddReward(-0.05f);
                    print("Reward -0.05 - Not moving to checkpoint");
                }
            }
        }
        
    }

    private void TrackCheckpoints_OnAgentWrongCheckpoint(object sender, System.EventArgs e)
    {
        if(trackCheckpoints.agentTransform == transform)
        {
            AddReward(-1f);
            print("Reward -1");
        }
    }

    private void TrackCheckpoints_OnAgentCorrectCheckpoint(object sender, System.EventArgs e)
    {
        if (trackCheckpoints.agentTransform == transform)
        {
            timer = 0f;
            timerStopPenalty = 0f;
            AddReward(+1f);
            print("Reward +1");

            if(trackCheckpoints.bestTimeThanPreviousCheck)
            {
                AddReward(+0.5f);
                print("Reward +0.5 | Best Time Check");
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 checkpointForward = trackCheckpoints.GetNextCheckpoint().transform.forward;
        float directionDot = Vector3.Dot(transform.forward, checkpointForward);
        sensor.AddObservation(directionDot);
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        carController.StopCar();
        trackCheckpoints.Reset();
        timer = 0f;
        timerStopPenalty = 0f;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int horizontal = actions.DiscreteActions[0];
        int vertical = actions.DiscreteActions[1];
        int brake = actions.DiscreteActions[2];

        switch (horizontal)
        {
            case 0: carController.horizontal = -1; break;
            case 1: carController.horizontal = 0; break;
            case 2: carController.horizontal = 1; break;
        }
        switch (vertical)
        {
            case 0: carController.vertical = -1; break;
            case 1: carController.vertical = 0; break;
            case 2: carController.vertical = 1; break;
        }

        if (!ignoreBrake)
        {
            switch (brake)
            {
                case 0: carController.brake = false; break;
                case 1: carController.brake = true; break;
            }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        discreteActions[0] = (int) Input.GetAxisRaw("Horizontal") + 1;
        discreteActions[1] = (int) Input.GetAxisRaw("Vertical") + 1;
        discreteActions[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;

        carController.horizontal = discreteActions[0];
        carController.vertical = discreteActions[1];
        carController.brake = System.Convert.ToBoolean(discreteActions[2]);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-2.0f);
            print("Bateu na parede!");
            EndEpisode();
        }

        if(collision.gameObject.tag == "Agent")
        {
            Physics.IgnoreCollision(collision.gameObject.GetComponentInChildren<Collider>(), GetComponentInChildren<Collider>());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.5f);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.5f);
            print("Bateu na parede!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-0.1f);
        }
    }*/

}

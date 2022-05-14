using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnAgentCorrectCheckpoint;
    public event EventHandler OnAgentWrongCheckpoint;

    private List<Checkpoint> checkpointList;
    public List<float> checkpointListTime;
    private int nextCheckpointIndex;

    public Transform agentTransform;
    public float lapTime;
    public bool bestTimeThanPreviousCheck;

    public CheckpointVisibility checkpointVisibility;

    private void Awake()
    {
        Transform checkpointsTransform = transform.Find("Checkpoints");
        checkpointVisibility = checkpointVisibility.GetComponent<CheckpointVisibility>();

        checkpointList = new List<Checkpoint>();
        foreach (Transform checkpointSingleTransform in checkpointsTransform)
        {
            Checkpoint checkpoint = checkpointSingleTransform.GetComponent<Checkpoint>();
            checkpoint.setTrackCheckpoints(this);

            checkpointList.Add(checkpoint);
            checkpointListTime.Add(float.MaxValue);
        }

        nextCheckpointIndex = 0;
        lapTime = 0f;
        bestTimeThanPreviousCheck = false;
    }

    private void FixedUpdate()
    {
        lapTime += Time.deltaTime;
    }

    public void AgentThroughCheckpoint(Checkpoint checkpoint, Transform agent)
    {
        agentTransform = agent;

        if (checkpointList.IndexOf(checkpoint) == nextCheckpointIndex)
        {
            // Correct Checkpoint
            Debug.Log("Correct!");
            if (nextCheckpointIndex >= 2)
            {
                checkpointList[nextCheckpointIndex - 2].GetComponent<Collider>().enabled = true;
                if (checkpointVisibility.isVisible)
                {
                    checkpointList[nextCheckpointIndex - 2].GetComponent<MeshRenderer>().enabled = true;
                }
            }

            if (lapTime != 0 && lapTime < checkpointListTime[nextCheckpointIndex])
            {
                checkpointListTime[nextCheckpointIndex] = lapTime;
                bestTimeThanPreviousCheck = true;
            }
            else
            {
                bestTimeThanPreviousCheck = false;
            }

            nextCheckpointIndex = (nextCheckpointIndex + 1) % checkpointList.Count;
            OnAgentCorrectCheckpoint?.Invoke(this, EventArgs.Empty);

            checkpoint.GetComponent<MeshRenderer>().enabled = false;
            checkpoint.GetComponent<Checkpoint>().carGoThrough = true;
            checkpoint.GetComponent<Collider>().enabled = false;


            if (nextCheckpointIndex == 0)
            {
                Reset();
            }
        }
        else
        {
            // Wrong Checkpoint
            Debug.Log("Wrong!");
            OnAgentWrongCheckpoint?.Invoke(this, EventArgs.Empty);
        }
    }

    public Checkpoint GetNextCheckpoint()
    {
        return checkpointList[nextCheckpointIndex];
    }

    public void Reset()
    {
        nextCheckpointIndex = 0;
        lapTime = 0f;
        foreach (Checkpoint checkpoint in checkpointList)
        {
            if (checkpointVisibility.isVisible)
            {
                checkpoint.GetComponent<MeshRenderer>().enabled = checkpointVisibility.isVisible;
                checkpoint.GetComponent<Collider>().enabled = true;
            }
            checkpoint.GetComponent<Checkpoint>().carGoThrough = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointVisibility : MonoBehaviour
{
    public bool isVisible;
    public GameObject[] checkpoints;

    private void Awake()
    {
        checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            isVisible = !isVisible;

            foreach (GameObject checkpoint in checkpoints)
            {
                Checkpoint checkpointScript = checkpoint.GetComponent<Checkpoint>();
                checkpointScript.mesh.enabled = isVisible;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public TrackCheckpoints trackCheckpoints;
    public MeshRenderer mesh;

    public bool carGoThrough;

    private void Start()
    {
        trackCheckpoints = trackCheckpoints.GetComponent<TrackCheckpoints>();
        mesh = transform.gameObject.GetComponent<MeshRenderer>();

        mesh.enabled = trackCheckpoints.checkpointVisibility.isVisible ? true : false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if(other.TryGetComponent<SelfDrivingCarAgent>(out SelfDrivingCarAgent agent))
        if(other.transform.parent.TryGetComponent<SelfDrivingCarAgent>(out SelfDrivingCarAgent agent))
        {
            trackCheckpoints.AgentThroughCheckpoint(this, other.transform.parent);
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.TryGetComponent<SelfDrivingCarAgent>(out SelfDrivingCarAgent agent))
        {
            transform.GetComponent<Collider>().enabled = true;
        }
    }*/

    public void setTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
}

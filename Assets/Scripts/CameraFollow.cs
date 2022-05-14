using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private List<Transform> targets;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;

    private int actualTarget;
    private int nextTarget;

    private void Start()
    {
        actualTarget = 0;
        nextTarget = 0;

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Agent"))
        {
            targets.Add(obj.transform);
        }
    }

    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            actualTarget = nextTarget;
            nextTarget = (nextTarget + 1) % targets.Count;
        }
    }

    private void HandleTranslation()
    {
        var targetPosition = targets[actualTarget].TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }
    private void HandleRotation()
    {
        var direction = targets[actualTarget].position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}

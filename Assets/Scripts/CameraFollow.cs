using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private Transform target;
    [SerializeField] private float translateSpeed;
    [SerializeField] private float rotationSpeed;


    private void FixedUpdate()
    {
        HandleTranslation();
        HandleRotation();
    }

    // Update is called once per frame
    
    private void HandleTranslation()
    {
        Vector3 targetPosition;
        if (Input.GetKey(KeyCode.Q)) targetPosition = target.TransformPoint(new Vector3(offset.x, offset.y, offset.z * -1)) ;
        else targetPosition = target.TransformPoint(offset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, translateSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        var direction = target.position - transform.position;
        var rotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}

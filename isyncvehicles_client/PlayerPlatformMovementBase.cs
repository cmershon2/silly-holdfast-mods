using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformMovementBase : MonoBehaviour
{
    public Transform rbody;
    public bool isOnPlatform;
    public Transform platformRBody;
    public Vector3 lastPlatformPosition;

    void Awake()
    {
        rbody = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if(isOnPlatform)
        {
            Vector3 deltaPosition = platformRBody.position - lastPlatformPosition;
            rbody.position = rbody.position + deltaPosition;
            lastPlatformPosition = platformRBody.position;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<isPlatformBase>() != null)
        {
            platformRBody = other.gameObject.GetComponent<Transform>();
            lastPlatformPosition = platformRBody.position;
            isOnPlatform = true;
        }
    }

    public void OnTriggerExit (Collider other)
    {
        if(other.gameObject.GetComponent<isPlatformBase>() != null)
        {
            isOnPlatform = false;
            platformRBody = null;
        }
    }
}
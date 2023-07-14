using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMechanic : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entity Entered");
        other.transform.parent = transform;
    }

    public void OnTriggerExit (Collider other)
    {
        Debug.Log("Entity Exited");
        other.transform.parent = null;
    }
}

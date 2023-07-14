using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDebugger : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("------------------------------");
        Debug.Log("Trigger Debugger");
        Debug.Log($"Entity Name: {other.gameObject.name}");
        Debug.Log($"Entity Parent: {other.gameObject.transform.parent}");
        Debug.Log($"Entity Tag: {other.gameObject.tag}");
        Debug.Log($"Entity Layer: {other.gameObject.layer}");
        if(other.gameObject.GetComponent<PlayerPlatformMovement>() != null)
        {
            Debug.Log($"Entity has PlayerPlatformMovement script");
        } else {
            Debug.Log($"Entity missing PlayerPlatformMovement script");
        }
        Debug.Log("------------------------------");
    }
}

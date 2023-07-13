using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanInteract_Trigger : MonoBehaviour
{
    public bool canInteract;
    public Collider interactableCollider;
    public bool isDev;

    public void OnTriggerStay(Collider other)
    {
        if(other.gameObject.name.Contains("Owner"))
        {
            canInteract = true;
            interactableCollider = other;
        } 
        else if(isDev)
        {
            canInteract = true;
            interactableCollider = other;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name.Contains("Owner"))
        {
            canInteract = false;
            interactableCollider = null;
        } else if(isDev){
            canInteract = false;
            interactableCollider = null;
        }
    }
}

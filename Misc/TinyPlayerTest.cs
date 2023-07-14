using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyPlayerTest : MonoBehaviour
{
    public GameObject teleporter_enter_target;
    public GameObject teleporter_exit_target;
    public Vector3 playerTargetScale;
    public Vector3 playerOriginalScale;

    public void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            tinyTime(other);
        }
    }

    public void tinyTime(Collider player)
    {
        if(player.transform.localScale == playerTargetScale)
        {
            player.transform.localScale = playerOriginalScale;
            player.transform.position = teleporter_exit_target.transform.position;
        }
        else
        {   
            playerOriginalScale = player.transform.localScale;
            player.transform.localScale = playerTargetScale;
            player.transform.position = teleporter_enter_target.transform.position;
        }
    }
}

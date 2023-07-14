using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoldfastSharedMethods;

public class Teleporter : MonoBehaviour
{
    public GameObject teleporter_target;
    public AudioSource teleporter_audio;
    public float teleporter_audio_delay = 0.3f;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entity Entered");
        other.transform.position = teleporter_target.transform.position;
        teleporter_audio.PlayDelayed(teleporter_audio_delay);
    }
}

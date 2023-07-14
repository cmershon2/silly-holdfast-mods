using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaNetworkActor : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 75f;
    public float idleAudioFrequency = 25f;
    public float despawnAfterDeath = 5f;
    public AudioSource goombaAudioSource;
    public AudioClip goombaDeathSound;
    public AudioClip[] goombaIdleSounds;
    public bool isWandering = false;
    public bool isRotatingLeft = false;
    public bool isRotatingRight = false;
    public bool isWalking = false;
    public bool isDead = false;
    public float idleAudioTimer;
    public Animator anim;

    // handle death on jump
    public void OnTriggerEnter(Collider other)
    {
        if(!isDead){
            isDead = true;
            goombaAudioSource.PlayOneShot(goombaDeathSound);
        }
    }
}

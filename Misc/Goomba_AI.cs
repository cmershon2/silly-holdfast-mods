using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goomba_AI : MonoBehaviour
{
    public float moveSpeed = 1.5f;
    public float rotationSpeed = 75f;
    public float idleAudioFrequency = 25f;
    public float despawnAfterDeath = 5f;
    public AudioSource goombaAudioSource;
    public AudioClip goombaDeathSound;
    public AudioClip[] goombaIdleSounds;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;
    private bool isDead = false;
    private float idleAudioTimer;

    private Animator anim;

    void Start()
    {
        // Fetch local gameObject's animator
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            anim.SetBool("isWalking", isWalking);

            if (isWandering == false)
            {
                StartCoroutine(Wander());
            }
            if (isRotatingRight == true)
            {
                // handle right rotation of gameObject
                transform.Rotate(transform.up * Time.deltaTime * rotationSpeed);
            }
            if (isRotatingLeft == true)
            {
                // handle left rotation of gameObject
                transform.Rotate(transform.up * Time.deltaTime * -rotationSpeed);
            }
            if (isWalking == true)
            {
                // handle forward of gameObject
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }

            // play random idle audio clip if Goomba is alive
            idleAudioTimer += Time.deltaTime;
            if (idleAudioTimer > idleAudioFrequency)
            {
                goombaAudioSource.PlayOneShot(RandomIdleAudioClip());
                idleAudioTimer = 0;
            }

        } else {
            anim.SetBool("isDead", true);
        }
    }

    IEnumerator Wander()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(1, 4);
        int rotateLorR = Random.Range(1, 2);
        int walkWait = Random.Range(1, 5);
        int walkTime = Random.Range(1, 6);

        isWandering = true;
        isWalking = false;
        yield return new WaitForSeconds(walkWait);
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        yield return new WaitForSeconds(rotateWait);
        if (rotateLorR == 1)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        if (rotateLorR == 2)
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        isWandering = false;
    }

    // handle death on jump
    public void OnTriggerEnter(Collider other)
    {
        if(!isDead){
            isDead = true;
            goombaAudioSource.PlayOneShot(goombaDeathSound);

            StartCoroutine(DestroyCorpse());
        }
    }

    IEnumerator DestroyCorpse(){
        yield return new WaitForSeconds(despawnAfterDeath);
        Destroy(gameObject);
    }

    // return a random idle audio clip
    AudioClip RandomIdleAudioClip(){
        return goombaIdleSounds[Random.Range(0, goombaIdleSounds.Length)];
    }
}

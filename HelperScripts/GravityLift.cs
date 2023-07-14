using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLift : MonoBehaviour
{
    public float liftForce = 10f; // the maximum force applied to lift the player or object
    public float transitionTime = 1f; // the duration of the transition
    public float exitDelay = 1f; // the delay before the player can re-enter the lift after exiting

    public bool isInsideLift = false;
    public bool canEnterLift = true;
    public float currentForce = 0f;
	private Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (!canEnterLift) return; // exit if the player is still in the exit delay period

        rb = other.gameObject.GetComponent<Rigidbody>();

        if (rb != null) // check if the object has a Rigidbody component
        {
            isInsideLift = true;
            StartCoroutine(TransitionForce(rb, liftForce, transitionTime));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        rb = other.gameObject.GetComponent<Rigidbody>();

        if (rb != null) // check if the object has a Rigidbody component
        {
            isInsideLift = false;
            StartCoroutine(TransitionForce(rb, 0f, transitionTime));
            StartCoroutine(EnterDelay());
        }
    }

    private IEnumerator TransitionForce(Rigidbody rb, float targetForce, float duration)
    {
        float startTime = Time.time;
        float startForce = currentForce;

        while (Time.time - startTime < duration)
        {
            currentForce = Mathf.Lerp(startForce, targetForce, (Time.time - startTime) / duration);
            yield return null;
        }

        currentForce = targetForce;
    }

    private IEnumerator EnterDelay()
    {
        canEnterLift = false;
        yield return new WaitForSeconds(exitDelay);
        canEnterLift = true;
    }

    private void FixedUpdate()
    {
        if (isInsideLift)
        {
            rb.AddForce(Vector3.up * currentForce, ForceMode.Force); // apply gradual upward force to lift the object
        }
    }
}
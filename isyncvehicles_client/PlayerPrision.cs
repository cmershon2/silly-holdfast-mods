using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class PlayerPrision : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject prisonGuide;
    [SerializeField] private float guideOffset;
    [SerializeField] private float translateSpeed;

    void Start()
    {
        var ppg = (PlayerPrisionGuide)FindObjectOfType(typeof(PlayerPrisionGuide));
        prisonGuide = ppg.gameObject;
    }

    private void FixedUpdate()
    {
        HandleTranslation();
    }

    private void HandleTranslation()
    {
        RaycastHit hit;
        Ray upTown = new Ray(transform.position, Vector3.up);
        // Debug.DrawRay(transform.position, Vector3.up);

        if (Physics.Raycast(upTown, out hit))
        {
            if (hit.collider.tag == "IgnoreCameraCollision")
            {
                var positionAdjustment = new Vector3(target.position.x, hit.point.y + guideOffset, target.position.z);
                transform.position = Vector3.Lerp(transform.position, positionAdjustment, translateSpeed * Time.deltaTime);
            }
            else 
            {
                var positionAdjustment = new Vector3(target.position.x, target.position.y-69 , target.position.z);
                transform.position = Vector3.Lerp(transform.position, positionAdjustment, translateSpeed * Time.deltaTime);
            }
        } 
        else 
        {
            var positionAdjustment = new Vector3(target.position.x, target.position.y-69 , target.position.z);
            transform.position = Vector3.Lerp(transform.position, positionAdjustment, translateSpeed * Time.deltaTime);
        }
    }
}
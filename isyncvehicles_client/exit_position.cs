using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exit_position : MonoBehaviour
{
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.up, out hit,  5))
        {
            if(hit.transform.gameObject.layer == 13)
            {
                Debug.Log("Above");
            }
        }
    }
}
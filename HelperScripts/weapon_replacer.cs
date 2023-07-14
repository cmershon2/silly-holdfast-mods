using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// version 1
public class weapon_replacer : MonoBehaviour
{
    [Header("Weapon Replacer")]

    public float swapTime = 3.0f;

    [Serializable]
    public class WeaponInfo
    {
        public GameObject WeaponPrefab;
        public string searchByName;
        public Vector3 rotationalOffset;
    }
    [SerializeField]
    public WeaponInfo[] WeaponDetails;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SwapAllByName", 0.0f, swapTime);
    }

    public void SwapAllByArray(List<GameObject> oldGameObjects)
    {
        // Store a boolean to detect if we intend to swap this game object.
        bool swappingSelf = false;

        // For each game object in the oldGameObjects array, 
        for (int i = 0; i < oldGameObjects.Count; i++)
        {
            // If the current game object is this game object, 
            if (oldGameObjects[i] == gameObject)
            {
                // Enable the flag to swap this game object at the end, so we
                // do not destroy it before the script an complete its task.
                swappingSelf = true;
            }
            else
            {
                // Else, we are not dealing with the game object local to this 
                // script, so we can swap the prefabs, immediately. 
                SwapPrefabs(oldGameObjects[i]);
            }
        }

        // If we have flagged the local game object to be swapped, 
        if (swappingSelf)
        {
            // Swap the local game object.
            SwapPrefabs(gameObject);
        }
    }

    public void SwapAllByName()
    {
        List<GameObject> oldGameObjects = new List<GameObject>();
        
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            foreach(WeaponInfo wi in WeaponDetails){
                if(gameObj.name == wi.searchByName)
                {
                    oldGameObjects.Add(gameObj);
                }
            }
        }

        if(oldGameObjects.Count > 0)
        {
            SwapAllByArray(oldGameObjects);
        }
    }

    void SwapPrefabs(GameObject oldGameObject)
    {
        // Instantiate the new game object at the old game objects position and rotation.
        foreach(WeaponInfo wi in WeaponDetails){
            if(oldGameObject.name == wi.searchByName){
                // Determine the rotation and position values of the old game object.
                // Replace rotation with Quaternion.identity if you do not wish to keep rotation.
                Debug.Log($"Parent {oldGameObject.transform.parent.name}");
                oldGameObject.transform.Rotate(wi.rotationalOffset);
                Quaternion rotation = oldGameObject.transform.rotation;
                Vector3 position = oldGameObject.transform.position;
                GameObject swapModel = wi.WeaponPrefab;

                GameObject newGameObject = Instantiate(swapModel, position, rotation);

                // If the old game object has a valid parent transform,
                // (You can remove this entire if statement if you do not wish to ensure your
                // new game object does not keep the parent of the old game object.
                if (oldGameObject.transform.parent != null)
                {
                    // Set the new game object parent as the old game objects parent.
                    newGameObject.transform.SetParent(oldGameObject.transform.parent);
                }

                // Hide the old game object, immediately, so it takes effect in the editor.
                oldGameObject.SetActive(false);
            }
        }
    }
}

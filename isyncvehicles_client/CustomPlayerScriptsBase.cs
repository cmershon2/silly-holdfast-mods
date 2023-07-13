using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomPlayerScriptsBase : MonoBehaviour
{
    public int playerLayerIndex;
    public float refreshTime = 10.0f;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("getAllByLayer", 0.0f, refreshTime);
    }

    public void getAllByLayer()
    {
        List<GameObject> oldGameObjects = new List<GameObject>();
        
        foreach(GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
        {
            // filter on only player layer
            if(gameObj.layer == playerLayerIndex && gameObj.GetComponent<PlayerPlatformMovementBase>() == null)
            {
                Debug.Log($"Found player: {gameObj.name}, attempting to add scripts");
                oldGameObjects.Add(gameObj);
            }
        }

        if(oldGameObjects.Count > 0)
        {
            AddComponentToArray(oldGameObjects);
        }
    }

    public void AddComponentToArray(List<GameObject> oldGameObjects)
    {
        for (int i = 0; i < oldGameObjects.Count; i++)
        {
            PlayerPlatformMovementBase newInstance = oldGameObjects[i].AddComponent<PlayerPlatformMovementBase>();
            Debug.Log($"Script successfully added to {oldGameObjects[i].name}");
        }
    }
}
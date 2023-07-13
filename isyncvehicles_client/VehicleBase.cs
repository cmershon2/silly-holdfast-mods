using System.Collections;
using System.Collections.Generic;
using HoldfastSharedMethods;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

// Script responsible for the following:
// - Vehicle Slots 
// - Vehicle Type
// - Vehicle Exit Points
// - Vehicle Enter Points

public class VehicleBase : MonoBehaviour
{   
    public enum vehicleDirections{Forward,Back}

    [Header("Vehicle Configuration")]
    [Space(10)]

    public bool isDevMode;
    public KeyCode vehicleInteractKey = KeyCode.E;
    public KeyCode vehicleBrakeKey = KeyCode.Space;
    public AudioSource vehicleAudioMaster;
    public GameObject vehicleClient;
    public Camera vehicleCam;
    [Space(10)]
    public float vehicleSpeed;

    [Header("Vehicle Slots")]
    [Space(10)]
    public bool[] isOwned;
    public bool[] isPilot;
    public int[] playerId;
    public GameObject[] actualPlayer;
    public GameObject[] dummyPlayer;
    public CanInteract_Trigger[] slotEnterTrigger;
    public GameObject[] slotExitPosition;
    public GameObject[] slotPlayerPrision;
    public Text[] slotUIText;
    public string[] slotEnterText;
    public Color[] slotEnterTextColor; 
    public string[] slotFilledText;
    public Color[] slotExitTextColor;

    [Header("Vehicle Killboxes")]
    public float killSpeed;
    public GameObject[] killBoxes;
    public vehicleDirections[] killDirection;

    [Header("Server Configuration")]
    [Space(10)]
    public float interpolationSpeed;
    public int vehicleIndex;

    private Camera initialPlayerMainCam;
    private int localPlayerId;
    private CarControllerBase carController;
    private InputField f1MenuInputField;
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private Rigidbody carControllerRigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        for (int i = 0; i < canvases.Length; i++) {
            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0) {
                //Inside this, now we need to find the input field where the player types messages.
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (f1MenuInputField != null) {
                    Debug.Log("Vehicle Base : Found the Game Console Panel");
                } else {
                    Debug.Log("Vehicle Base : Game Console Panel not found. This mod may not work correctly!");
                }
                break;
            }
        }

        if(isDevMode){Debug.LogWarning($"Vehicle Base set to Developer Mode on vehicle: {gameObject.name}");}
        if(vehicleClient.GetComponent<CarControllerBase>() != null)
        { 
            carController = vehicleClient.GetComponent<CarControllerBase>(); 
            carControllerRigidbody = vehicleClient.GetComponent<Rigidbody>(); 
        }
        
        carControllerRigidbody.isKinematic = true;
        // Debug.Log($"Vehicle Base : New vehicle spawned, {gameObject.name}");
        vehicleIndex = getVehicleIndex(gameObject);
        vehicleCam.gameObject.SetActive(false);
        // Debug.Log($"Vehicle Base : {gameObject.name} has {isOwned.Length} slots");

        if(isOwned.Length > 0)
        {
            foreach (Text text in slotUIText)
            {
                text.gameObject.SetActive(false);
            }
            foreach (GameObject dummy in dummyPlayer)
            {
                dummy.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        vehicleOwnerHandler();
        vehiclePlayerInteractions();
        playerUpdateMovement();
    }

    public void vehicleOwnerHandler()
    {
        int index = 0;
        foreach (bool o in isOwned)
        {
            if(slotEnterTrigger[index].canInteract && (slotEnterTrigger[index].interactableCollider != null && slotEnterTrigger[index].interactableCollider.gameObject.GetComponent<PlayerPlatformMovementBase>() != null))
            {
                if(!o)
                {
                    slotUIText[index].text = slotEnterText[index];
                    slotUIText[index].color = slotEnterTextColor[index];
                    // enter vehicle
                    if(Input.GetKeyDown(vehicleInteractKey))
                    {
                        initialPlayerMainCam = Camera.main;
                        playerEnter(index);
                    }
                } 
                else
                {
                    slotUIText[index].text = slotFilledText[index];
                    slotUIText[index].color = slotExitTextColor[index];
                } 

                slotUIText[index].gameObject.SetActive(true);
            }
            else if((Input.GetKeyDown(vehicleInteractKey) || Input.GetKeyDown(KeyCode.Escape))&&(actualPlayer[index] != null)&&(o && isLocalPlayer(actualPlayer[index])))
            {
                // exit vehicle
                playerExit(index);
            }
            else if(slotEnterTrigger[index].interactableCollider != null && slotEnterTrigger[index].interactableCollider.gameObject.GetComponent<PlayerPlatformMovementBase>() == null)
            {
                // handle players who are missing platform movement script
                slotUIText[index].text = "Try again in a sec.";
                slotUIText[index].color = slotExitTextColor[index];
                slotUIText[index].gameObject.SetActive(true);
            }
            else
            {
                slotUIText[index].gameObject.SetActive(false);
            }

            index++;
        }
    }

    public void vehiclePlayerInteractions()
    {
        vehicleSpeed = carControllerRigidbody.velocity.magnitude;
        Vector3 vehicleDirection = carController.transform.InverseTransformDirection(carControllerRigidbody.velocity);
        var dir = Quaternion.Inverse(carController.transform.rotation) * carControllerRigidbody.velocity;
        
        if(vehicleSpeed > killSpeed)
        {
            if(dir.z > 0)
            {
                // moving foward
                var index = 0;
                foreach(GameObject kb in killBoxes)
                {
                    if(killDirection[index] == vehicleDirections.Forward){kb.SetActive(true);}
                    else {kb.SetActive(false);}
                    index++;
                }
            }
            else if(dir.z < 0)
            {
                // moving back
                var index = 0;
                foreach(GameObject kb in killBoxes)
                {
                    if(killDirection[index] == vehicleDirections.Back){kb.SetActive(true);}
                    else {kb.SetActive(false);}
                    index++;
                }
            }
        }
        else
        {
            foreach(GameObject kb in killBoxes)
            {
                kb.SetActive(false);
            }
        }
    }

    public void sendBrake(bool braking)
    {
        if(braking)
        {
            string f1command = $"rc login isyncvehicles:brake:{vehicleIndex}|{localPlayerId}";
            
            if(!isDevMode)
            {
                f1MenuInputField.onEndEdit.Invoke(f1command);
            }
            else
            {
                Debug.Log(f1command);
            }
        }
    }

    public void playerEnter(int index)
    {
        actualPlayer[index] = slotEnterTrigger[index].interactableCollider.gameObject;
        playerId[index] = getPlayerId(actualPlayer[index]);
        localPlayerId = playerId[index];
        if(playerId[index] == -1 && !isDevMode)
        {
            slotEnterText[index] = slotEnterText[index]+":Invalid Player";
            return;
        }

        onlyOneCamera(0);
        isOwned[index] = true;
        dummyPlayer[index].SetActive(true);

        // update car controller's local player if player is owner & slot is pilot
        if (isLocalPlayer(actualPlayer[index]) && isPilot[index])
        {
            carController.localPlayer = actualPlayer[index];
            carControllerRigidbody.isKinematic = false;
        }
        else
        {
            carControllerRigidbody.isKinematic = true;
        }

        actualPlayer[index].transform.position = slotPlayerPrision[index].transform.position;

        string f1command = $"rc login isyncvehicles:enter:{vehicleIndex}|{playerId[index]}|{index}";
        if(!isDevMode)
        {
            f1MenuInputField.onEndEdit.Invoke(f1command);
        }
        else
        {
            Debug.Log(f1command);
        }
    }

    public void playerExit(int index)
    {
        onlyOneCamera(1);
        actualPlayer[index].transform.position = slotExitPosition[index].transform.position;
        isOwned[index] = false;
        actualPlayer[index] = null;
        dummyPlayer[index].SetActive(false);
        carControllerRigidbody.isKinematic = true;
        
        string f1command = $"rc login isyncvehicles:exit:{vehicleIndex}|{playerId[index]}|{index}";
        if(!isDevMode)
        {
            f1MenuInputField.onEndEdit.Invoke(f1command);
        }
        else
        {
            Debug.Log(f1command);
        }
    }

    public void playerUpdateMovement()
    {
        int bucketOwner = findBucketOwnerIndex();

        if(localPlayerId != null && bucketOwner == localPlayerId)
        {
            if(Vector3.Distance(vehicleClient.transform.position, lastPosition) >= 0.001)
            {
                // Capture the player's position before the F1 command is fired off and use this
                // to determine if the player has moved in the if statement above.
            
                lastPosition = vehicleClient.transform.position;
                // clean up position & rotation to remove spaces
                var cleanPosition = Regex.Replace( vehicleClient.transform.position.ToString("F3")," ","" );
                var cleanRotation = Regex.Replace( vehicleClient.transform.rotation.ToString("F3")," ","" );
                string f1command = $"rc login isyncvehicles:move:{vehicleIndex}|{localPlayerId}|{cleanPosition}|{cleanRotation}|{carController.horizontalInput}|{carController.verticalInput}";
                
                if(!isDevMode)
                {
                    f1MenuInputField.onEndEdit.Invoke(f1command);
                }
                else
                {
                    Debug.Log(f1command);
                }
            }
        
            if(Quaternion.Angle(vehicleClient.transform.rotation, lastRotation) >= 0.001)
            {
                // Capture the player's rotation before the F1 command is fired off and use this
                // to determine if the player has turned in the if statement above. 
            
                lastRotation = vehicleClient.transform.rotation;
                // clean up position & rotation to remove spaces
                var cleanPosition = Regex.Replace( vehicleClient.transform.position.ToString("F3")," ","" );
                var cleanRotation = Regex.Replace( vehicleClient.transform.rotation.ToString("F3")," ","" );
                string f1command = $"rc login isyncvehicles:move:{vehicleIndex}|{localPlayerId}|{cleanPosition}|{cleanRotation}|{carController.horizontalInput}|{carController.verticalInput}";

                if(!isDevMode)
                {
                    f1MenuInputField.onEndEdit.Invoke(f1command);
                }
                else
                {
                    Debug.Log(f1command);
                }
            }
        }
    }

    public void serverSyncVehiclePositionCache(Vector3 _newPosition, Quaternion _newRotation, float _horizontalInput, float _verticalInput)
    {
        // Debug.Log($"Vehicle Base - Got that cache");
        // Debug.Log($"Vehicle Base - vehiclePosition:{vehicleClient.transform.position}, new position:{_newPosition}, vehicleRotation:{vehicleClient.transform.rotation}, new rotation:{_newRotation}");

        var interpolationTime = Time.deltaTime * interpolationSpeed;
        Debug.Log($"Step 1: {interpolationTime}");
        vehicleClient.transform.position = Vector3.Lerp(vehicleClient.transform.position, _newPosition, interpolationTime);
        Debug.Log($"Step 2: {vehicleClient.transform.position}");
        vehicleClient.transform.rotation = Quaternion.Slerp(vehicleClient.transform.rotation, _newRotation, interpolationTime);
        Debug.Log($"Step 3: {vehicleClient.transform.rotation}");
    }

    public void serverSyncVehicleOwnerCache(int _cacheOwnerPlayerId, int _cacheSeatIndex)
    {
        // Debug.Log($"Vehicle Base - Server owner cache recieved for {gameObject.name}");

        playerId[_cacheSeatIndex] = _cacheOwnerPlayerId;
        isOwned[_cacheSeatIndex] = true;
        dummyPlayer[_cacheSeatIndex].SetActive(true);
    }

    public void serverUpdateMovement(int localPlayerId, int serverPlayerId, Vector3 _newPosition, Quaternion _newRotation, float _horizontalInput, float _verticalInput)
    {
        int bucketOwner = findBucketOwnerIndex();

        if(bucketOwner != localPlayerId)
        {
            // only update vehicle if it is on a client that is not the pilot
            var interpolationTime = Time.deltaTime * interpolationSpeed;
            carController.horizontalInput = Mathf.Lerp(carController.horizontalInput, _horizontalInput, interpolationTime);
            carController.verticalInput = Mathf.Lerp(carController.verticalInput, _verticalInput, interpolationTime);
            vehicleClient.transform.position = Vector3.Lerp(vehicleClient.transform.position, _newPosition, interpolationTime);
            vehicleClient.transform.rotation = Quaternion.Slerp(vehicleClient.transform.rotation, _newRotation, interpolationTime);
        }
    }

    public void serverPlayerEnter(int _playerId, int _slotIndex)
    {
        playerId[_slotIndex] = _playerId;
        isOwned[_slotIndex] = true;
        dummyPlayer[_slotIndex].SetActive(true);
    }

    public void serverPlayerExit(int _playerId, int _slotIndex)
    {
        playerId[_slotIndex] = -10;
        isOwned[_slotIndex] = false;
        dummyPlayer[_slotIndex].SetActive(false);
    }
    
    public void serverPlayerBrake()
    {
        int bucketOwner = findBucketOwnerIndex();

        // Debug.Log($"serverPlayerBrake: Bucket: {bucketOwner}, Player: {serverPlayerId}");

        /*
        if(bucketOwner != serverPlayerId)
        {
            
        }
        */
    }

    public void onlyOneCamera(int actionType)
    {
        // 0 = enter
        // 1 = exit
        Camera[] sceneCameras = GameObject.FindObjectsOfType<Camera>();
        foreach (Camera cam in sceneCameras)
        {
            cam.enabled = false;
        }

        if(actionType == 0)
        {
            vehicleCam.gameObject.SetActive(true);
            vehicleCam.enabled=true;

        }
        else if(actionType == 1)
        {
            vehicleCam.gameObject.SetActive(false);
            vehicleCam.enabled=false;
            initialPlayerMainCam.enabled = true;
        }
    }

    // check if a player is in a given vehicle
    public bool inVehicle(GameObject localPlayer)
    {
        if(localPlayer == null){return false;}

        if(actualPlayer.Length > 0)
        {
            foreach (GameObject ap in actualPlayer)
            {
                if(localPlayer == ap)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // check if a player is a pilot of vehicle
    public bool isVehiclePilot(GameObject localPlayer)
    {
        if(localPlayer == null){return false;}

        if(actualPlayer.Length > 0)
        {
            var index = 0;
            foreach (GameObject ap in actualPlayer)
            {
                if(isPilot[index] && localPlayer == ap)
                {
                    return true;
                }

                index++;
            }
        }
        return false;
    }

    // check if local player is owner of instance
    public bool isLocalPlayer(GameObject localPlayer)
    {
        if(localPlayer.name.Contains("Owner") || isDevMode)
        {
            return true;
        }

        return false;
    }

    // extract player id from object name
    // input: Owner - Player (#23)
    // output: 23
    public int getPlayerId(GameObject localPlayer)
    {
        try
        {
            int match = Int32.Parse( Regex.Match(localPlayer.name, @"\(#([^)]*)\)").Groups[1].Value );
            if(match != null){ return match; }
            return -1;
        }
        catch (FormatException e)
        {
            Debug.Log($"Vehicle Base - getPlayerId: error {e}");
            return -1;
        }
    }

    // extract vehicle index from object name
    // input: 0_isyncvehicles_vehicleName
    // output: 0
    public int getVehicleIndex(GameObject vehicle)
    {
        try
        {
            int match = Int32.Parse( Regex.Match(vehicle.name, @"^\d+").Value );
            if(match != null){ return match; }
            return -1;
        }
        catch (FormatException e)
        {
            Debug.Log($"Vehicle Base - getVehicleIndex: error {e}");
            return -1;
        }
    }

    public int findBucketOwnerIndex()
    {
        if(isOwned.Length > 0)
        {
            int index=0;
            foreach (bool s in isOwned)
            {
                // return first instance of owner
                if(s){ return playerId[index]; }
                index++;
            }
            // no owner found, return -1
            return -1;
        }
        // no slots found, return -1
        return -1;
    }

}
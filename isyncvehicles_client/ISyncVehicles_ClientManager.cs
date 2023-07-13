using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoldfastSharedMethods;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;


// Script responsible for the following:
// - vehicle init
// - vehicle caching

public class ISyncVehicles_ClientManager : MonoBehaviour
{
    public GameObject vehiclePrefab;
    public string vehicleName;
    public int vehicleSlots;
    public Transform[] spawnLocations;
    public List<GameObject> vehicles;
    public GameObject vehicleParent;

    private InputField f1MenuInputField;

    // Start is called before the first frame update
    void Start()
    {

        // init scene with vehicles at default positions
        // this will later be synced
        
        /*
        int index = 0;
        foreach (Transform spawn in spawnLocations)
        {
            var currLocalVehicle = Instantiate(vehiclePrefab, spawn.position, spawn.rotation) as GameObject;
            currLocalVehicle.name = $"{index}_ISyncVehicles_{vehicleName}";
            currLocalVehicle.transform.parent = vehicleParent.transform;
            
            vehicles.Add(currLocalVehicle);

            index++;
        }
        */
    }

    public void initVehiclesFromCache(string positionCache, string ownerCache)
    {
        int index = 0;
        string[] splitPositionCache = positionCache.Split('&');
        string[] splitOwnerCache = ownerCache.Split('&');

        foreach (Transform spawn in spawnLocations)
        {
            if(splitPositionCache[index] != "none")
            {
                string[] positionFull = splitPositionCache[index].Split('_');
                string[] ownerFull = splitOwnerCache[index].Split('#');

                // example cache for position: 1_2_(266.6,35.9,283.6)_(0.0,0.4,0.0,0.9)_0_0
                // example cache for owner: enter_5_2_0,enter_5_4_1,exit_5_5_2,none,none,none,...

                // TODO: Init vehicle based on cache data
                GameObject vehicleToSpawn = vehiclePrefab;
                GameObject currLocalVehicle = Instantiate(vehiclePrefab, spawn.position, spawn.rotation);
                currLocalVehicle.name = $"{index}_ISyncVehicles_{vehicleName}";
                currLocalVehicle.transform.parent = vehicleParent.transform;
                vehicles.Add(currLocalVehicle);

                var vb = currLocalVehicle.GetComponent<VehicleBase>();

                foreach (string oc in ownerFull)
                {
                    var ownerCacheSplit = oc.Split('_');
                    var cacheOwnerAction = ownerCacheSplit[0];
                    // ignore if "n"
                     if(cacheOwnerAction == "n"){ continue; }

                    // check if player is in slot
                    if(cacheOwnerAction == "enter")
                    {
                        var cacheOwnerPlayerId = IntParseFast(ownerCacheSplit[2]);
                        var cacheOwnerSlotIndex = IntParseFast(ownerCacheSplit[3]);
                        vb.serverSyncVehicleOwnerCache(cacheOwnerPlayerId, cacheOwnerSlotIndex);
                    }
                }

                var cachePosition = StringToVector3(positionFull[2]);
                var cacheRotation = StringToQuaternion(positionFull[3]);
                var cacheHorizontalInput = float.Parse(positionFull[4]);
                var cacheVerticalInput = float.Parse(positionFull[5]);

                vb.serverSyncVehiclePositionCache(cachePosition, cacheRotation, cacheHorizontalInput, cacheVerticalInput);
            } 
            else
            {
                GameObject vehicleToSpawn = vehiclePrefab;
                GameObject currLocalVehicle = Instantiate(vehicleToSpawn, spawn.position, spawn.rotation);
                currLocalVehicle.name = $"{index}_ISyncVehicles_{vehicleName}";
                currLocalVehicle.transform.parent = vehicleParent.transform;
                vehicles.Add(currLocalVehicle);
            } 

            index++;
        }
    }

    // Helper function to convert string to Vector3
    public static Vector3 StringToVector3(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
            sVector = sVector.Substring(1, sVector.Length-2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector3 result = new Vector3(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]));

        return result;
    }

    // Helper function to convert string to Quaternion
    public static Quaternion StringToQuaternion(string sQuaternion)
    {
        // Remove the parentheses
        if (sQuaternion.StartsWith("(") && sQuaternion.EndsWith(")"))
        {
            sQuaternion = sQuaternion.Substring(1, sQuaternion.Length - 2);
        }

        // split the items
        string[] sArray = sQuaternion.Split(',');

        // store as a Vector3
        Quaternion result = new Quaternion(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }

    // Helper function to quickly convert string to int
    public static int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }
}
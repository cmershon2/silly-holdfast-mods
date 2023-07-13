using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections;
using System;
using HoldfastSharedMethods;
using UnityEngine.UI;
using UnityEngine;

public class ISyncVehicles_Client : IHoldfastSharedMethods
{
    private ISyncVehicles_ClientManager vehicleManager;
    private InputField f1MenuInputField;

    public bool isLocalPlayer = false;
    public bool isClient = false;

    // interpolation variables
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;
    public string localPlayerId;
    public List<string> tempCacheHolder;

    public void OnSyncValueState(int value) {
    }

    public void OnUpdateSyncedTime(double time) {
    }

    public void OnUpdateElapsedTime(float time) {
    }

    public void OnUpdateTimeRemaining(float time) {
    }

    public void OnIsServer(bool server) {
    }

    public void OnIsClient(bool client, ulong steamId) 
    {
        // init is on client var
        isClient = client;

        // only listen if on client
        if(!isClient){return;}
        localPlayerId = $"{steamId}";

        var actors = Resources.FindObjectsOfTypeAll<ISyncVehicles_ClientManager>();
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();

        for (int i = 0; i < canvases.Length; i++) {
            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0) {
                //Inside this, now we need to find the input field where the player types messages.
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (f1MenuInputField != null) {
                    Debug.Log("ISyncVehicleController - Client : Found the Game Console Panel");

                    if(actors.Length == 1) {
                        Debug.Log($"ISyncVehicleController - Client : Found vehicle manager.");
                        vehicleManager = actors[0];

                    } else if(actors.Length > 1) {
                        Debug.LogWarning($"ISyncVehicleController - Client : Found too many vehicle managers, {actors.Length} found.");
                    } 
                    else {
                        Debug.Log("ISyncVehicleController - Client : No actors found.");
                    }
                } else {
                    Debug.Log("ISyncVehicleController - Client : Game Console Panel not found. This mod may not work correctly!");
                }
                break;
            }
        }
    }

    public void OnDamageableObjectDamaged(GameObject damageableObject, int damageableObjectId, int shipId, int oldHp, int newHp) {
    }

    public void OnPlayerHurt(int playerId, byte oldHp, byte newHp, EntityHealthChangedReason reason) {
    }

    public void OnPlayerKilledPlayer(int killerPlayerId, int victimPlayerId, EntityHealthChangedReason reason, string additionalDetails) {
    }

    public void OnPlayerShoot(int playerId, bool dryShot) {
    }

    public void OnPlayerJoined(int playerId, ulong steamId, string playerName, string regimentTag, bool isBot) {
        
        // only listen if on client
        if(!isClient){return;}
        if($"{steamId}" != localPlayerId){return;}

        // Request server cache
        
        Debug.Log($"ISyncVehicles - Client: Player {localPlayerId} joined.");

        string f1command = $"rc login isyncvehicles:getCache:{localPlayerId}|{vehicleManager.spawnLocations.Length}|{vehicleManager.vehicleSlots}";
        f1MenuInputField.onEndEdit.Invoke(f1command);
    }

    public void OnPlayerLeft(int playerId) {
    }

    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject) {
    }

    public void OnScorableAction(int playerId, int score, ScorableActionType reason) {
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text) 
    {
        // only listen if on client
        if(!isClient){return;}

        // only list if message from server
        if (playerId != -1){return;}

        // listen if message has correct command
        string[] command = text.Split(':');
        if (command[0] != "isyncvehicles"){ return; }

        string[] inputVariables = command[2].Split('|');

        // Debug.Log($"Client Manager : {command[1]}, {text}");

        if(command[1] == "enter")
        {
            if(inputVariables.Length != 3){return;}
            var _vehicleIndex = IntParseFast(inputVariables[0]);
            var _playerId = IntParseFast(inputVariables[1]);
            var _slotIndex = IntParseFast(inputVariables[2]);
            var _vehicleBase = vehicleManager.vehicles[_vehicleIndex].GetComponent<VehicleBase>();

            _vehicleBase.serverPlayerEnter(_playerId, _slotIndex);
        }
        else if(command[1] == "exit")
        {
            if(inputVariables.Length != 3){return;}
            var _vehicleIndex = IntParseFast(inputVariables[0]);
            var _playerId = IntParseFast(inputVariables[1]);
            var _slotIndex = IntParseFast(inputVariables[2]);
            var _vehicleBase = vehicleManager.vehicles[_vehicleIndex].GetComponent<VehicleBase>();

            // Debug.Log($"Exit: {command[1]}, {_vehicleIndex}, {_playerId}, {_slotIndex}");
            _vehicleBase.serverPlayerExit(_playerId, _slotIndex);
        }
        else if(command[1] == "move")
        {
            // return if input variable less than, return
            if(inputVariables.Length < 4){return;}
            var _vehicleIndex = IntParseFast(inputVariables[0]);
            var _playerId = IntParseFast(inputVariables[1]);
            var _position = StringToVector3(inputVariables[2]);
            var _rotation = StringToQuaternion(inputVariables[3]);
            var _horizontalInput = float.Parse(inputVariables[4]);
            var _verticalInput = float.Parse(inputVariables[5]);
            var _vehicleBase = vehicleManager.vehicles[_vehicleIndex].GetComponent<VehicleBase>();

            //Debug.Log($"Move: {command[1]}, {_vehicleIndex}, {_playerId}, {_position}, {_rotation}, {_horizontalInput}, {_verticalInput}");

            _vehicleBase.serverUpdateMovement(playerId, _playerId, _position, _rotation, _horizontalInput, _verticalInput);
        }
        else if(command[1] == "brake")
        {
            // TODO: handle braking
        }
        else if(command[1] == "init")
        {
            var _playerId = inputVariables[0];
            // Debug.Log($"time to init {_playerId}, {localPlayerId}");
            // only init if I was the player who joined.
            if(_playerId != localPlayerId && _playerId.Length > 15){ return; }
            // Debug.Log($"Get Cache: {_playerId} got cache of postions: {inputVariables[1]}, owners: {inputVariables[2]}");

            vehicleManager.initVehiclesFromCache(inputVariables[1], inputVariables[2]);
        }

        return;
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

    public void OnRoundDetails(int roundId, string serverName, string mapName, FactionCountry attackingFaction, FactionCountry defendingFaction, GameplayMode gameplayMode, GameType gameType) {
    }

    public void OnPlayerBlock(int attackingPlayerId, int defendingPlayerId) {
    }

    public void OnPlayerMeleeStartSecondaryAttack(int playerId) {
    }

    public void OnPlayerWeaponSwitch(int playerId, string weapon) {
    }

    public void OnCapturePointCaptured(int capturePoint) {
    }

    public void OnCapturePointOwnerChanged(int capturePoint, FactionCountry factionCountry) {
    }

    public void OnCapturePointDataUpdated(int capturePoint, int defendingPlayerCount, int attackingPlayerCount) {
    }

    public void OnRoundEndFactionWinner(FactionCountry factionCountry, FactionRoundWinnerReason reason) {
    }

    public void OnRoundEndPlayerWinner(int playerId) {
    }

    public void OnPlayerStartCarry(int playerId, CarryableObjectType carryableObject) {
    }

    public void OnPlayerEndCarry(int playerId) {
    }

    public void OnPlayerShout(int playerId, CharacterVoicePhrase voicePhrase) {
    }

    public void OnInteractableObjectInteraction(int playerId, int interactableObjectId, GameObject interactableObject, InteractionActivationType interactionActivationType, int nextActivationStateTransitionIndex) {
    }

    public void PassConfigVariables(string[] value) {
    }

    public void OnEmplacementPlaced(int itemId, GameObject objectBuilt, EmplacementType emplacementType) {
    }

    public void OnEmplacementConstructed(int itemId) {
    }

    public void OnBuffStart(int playerId, BuffType buff) {
    }

    public void OnBuffStop(int playerId, BuffType buff) {
    }

    public void OnShotInfo(int playerId, int shotCount, Vector3[][] shotsPointsPositions, float[] trajectileDistances, float[] distanceFromFiringPositions, float[] horizontalDeviationAngles, float[] maxHorizontalDeviationAngles, float[] muzzleVelocities, float[] gravities, float[] damageHitBaseDamages, float[] damageRangeUnitValues, float[] damagePostTraitAndBuffValues, float[] totalDamages, Vector3[] hitPositions, Vector3[] hitDirections, int[] hitPlayerIds, int[] hitDamageableObjectIds, int[] hitShipIds, int[] hitVehicleIds) {
    }

    public void OnVehicleSpawned(int vehicleId, FactionCountry vehicleFaction, PlayerClass vehicleClass, GameObject vehicleObject, int ownerPlayerId) {
    }

    public void OnVehicleHurt(int vehicleId, byte oldHp, byte newHp, EntityHealthChangedReason reason) {
    }

    public void OnPlayerKilledVehicle(int killerPlayerId, int victimVehicleId, EntityHealthChangedReason reason, string details) {
    }

    public void OnShipSpawned(int shipId, GameObject shipObject, FactionCountry shipfaction, ShipType shipType, int shipNameId) {
    }

    public void OnShipDamaged(int shipId, int oldHp, int newHp) {
    }

    public void OnAdminPlayerAction(int playerId, int adminId, ServerAdminAction action, string reason) {
    }

    public void OnConsoleCommand(string input, string output, bool success) {
    }

    public void OnRCLogin(int playerId, string inputPassword, bool isLoggedIn) {
    }

    public void OnRCCommand(int playerId, string input, string output, bool success) {
    }

    public void OnPlayerPacket(int playerId, byte? instance, Vector3? ownerPosition, double? packetTimestamp, Vector2? ownerInputAxis, float? ownerRotationY, float? ownerPitch, float? ownerYaw, PlayerActions[] actionCollection, Vector3? cameraPosition, Vector3? cameraForward, ushort? shipID, bool swimming) {
    }

    public void OnVehiclePacket(int vehicleId, Vector2 inputAxis, bool shift, bool strafe, PlayerVehicleActions[] actionCollection) {
    }

    public void OnOfficerOrderStart(int officerPlayerId, HighCommandOrderType highCommandOrderType, Vector3 orderPosition, float orderRotationY, int voicePhraseRandomIndex) {
    }

    public void OnOfficerOrderStop(int officerPlayerId, HighCommandOrderType highCommandOrderType) {
    }
}

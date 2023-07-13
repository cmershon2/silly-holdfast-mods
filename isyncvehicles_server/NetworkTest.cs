using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HoldfastSharedMethods;
using System;
using UnityEngine.UI;

public class NetworkTest : IHoldfastSharedMethods
{
    private InputField f1MenuInputField;
    private ulong player;

    // Init caches with default values of none
    public List<string> vehiclePositionCache = Enumerable.Repeat("none", 100).ToList();

    // 2D array for cache, init with values of none
    // total of 100 rows & 10 columns
    // rows = vehicle index
    // columns = vehicle slots/seats
    public string[,] vehicleOwnerCache = new string[100,10]{
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"},
			{"n","n","n","n","n","n","n","n","n","n"}
	};

    public void OnSyncValueState(int value) {
    }

    public void OnUpdateSyncedTime(double time) {
    }

    public void OnUpdateElapsedTime(float time) {
    }

    public void OnUpdateTimeRemaining(float time) {
    }

    public void OnIsServer(bool server) 
    {
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();

        for (int i = 0; i < canvases.Length; i++) {
            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0) {
                //Inside this, now we need to find the input field where the player types messages.
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (f1MenuInputField != null) {
                    Debug.Log("ISyncVehicles - Server : Found the Game Console Panel");
                } else {
                    Debug.Log("ISyncVehicles - Server : Game Console Panel not found. This mod may not work correctly!");
                }
                break;
            }
        }
    }

    public void OnIsClient(bool client, ulong steamId){
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
        // TODO : Sync existing data to newly joined players 
    }

    public void OnPlayerLeft(int playerId) {
    }

    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject) {
    }

    public void OnScorableAction(int playerId, int score, ScorableActionType reason) {
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text) {
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
        if(playerId == -1){ return; } // bad server, not allowed to drive vehicles >:[
        
        // verify correct command
        string[] command = inputPassword.Split(':');
        if (command[0] != "isyncvehicles"){ return; }

        // verify there are correct number of input variables
        string[] inputVariables = command[2].Split('|');
        if(inputVariables.Length < 0)
        { 
            Debug.Log($"ISyncVehicles - Invalid variables. Input was: {inputPassword}");
            return; 
        }

        // verify the action is valid
        string[] validActions = {"enter","exit","brake","move","getCache"};
        int index = Array.FindIndex(validActions, str => str == command[1]);
        if(index == -1)
        {
            Debug.Log($"ISyncVehicles - Invalid action. Input was: {inputPassword}");
            return;
        }

        // handle server caching for vehicle movement & ownership.
        if(command[1] == "move"){
            var moveCache = command[2].Replace('|','_');
            vehiclePositionCache[IntParseFast(inputVariables[0])] = $"{moveCache}";
            // Debug.Log($"ISyncVehicles - New movement cache: {vehiclePositionCache[IntParseFast(inputVariables[0])]}");
        }

        if(command[1] == "enter" || command[1] == "exit"){
            var ownerCache = command[2].Replace('|','_');
            vehicleOwnerCache[IntParseFast(inputVariables[0]),IntParseFast(inputVariables[2])] = $"{command[1]}_{ownerCache}";
            // Debug.Log($"ISyncVehicles - New owner cache: {vehicleOwnerCache[IntParseFast(inputVariables[0])]}");
        }

        if(command[1] == "getCache")
        {
            // new player joined, sync them with the server.
            var playerSteamId = inputVariables[0];
            var numOfVehicles = IntParseFast(inputVariables[1]);
            var positionCache = string.Join("&",vehiclePositionCache.GetRange(0, numOfVehicles));
            var slotCount = IntParseFast(inputVariables[2]);
            string[,] ownerCacheWindow = new string[numOfVehicles,slotCount];

            // Disgusting preformance but damn, it gets a window of data
            for (int i = 0; i < numOfVehicles; i++)
            {
                for (int j = 0; j < slotCount; j++)
                {
                    ownerCacheWindow[i, j] = vehicleOwnerCache[i, j];
                }
            }
            var ownerCache = string.Join("&", ownerCacheWindow.OfType<string>()
                .Select((value, index) => new {value, index})
                .GroupBy(x => x.index / ownerCacheWindow.GetLength(1))
                .Select(x => $"{string.Join("#", x.Select(y => y.value))}"));
            
            f1MenuInputField.onEndEdit.Invoke($"serverAdmin quietBroadcastMessage isyncvehicles:init:{playerSteamId}|{positionCache}|{ownerCache}");

            return;
        }
        else
        {
            // broadcast to client
            f1MenuInputField.onEndEdit.Invoke($"serverAdmin quietBroadcastMessage {inputPassword}");
        }

        return;
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
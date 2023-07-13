using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using HoldfastSharedMethods;

namespace CodeDucky.Syncing.VideoPlayers
{
    public class ISyncVideoPlayer_NetworkController : IHoldfastSharedMethods
    {
        private ISyncVideoPlayer[] videoSync; // array of video players found and will be synced

        private string brandingColor = "#ff5e00"; // used to make our logs look pretty
        private bool _videosFound = false; // private flag to note when all video players have been found
        private bool _videosReady = false; // private flag to note when all video players are ready/prepared

        public void OnUpdateSyncedTime(double time) {
            if(_videosFound && !_videosReady)
            {
                List<ISyncVideoPlayer> readyVids = new List <ISyncVideoPlayer>();

                foreach (ISyncVideoPlayer vid in videoSync)
                {
                    if(vid.isVideoReady)
                    {
                        readyVids.Add(vid);
                    }
                }

                // check if all the videos are ready, if not gotta check on the next frame.
                if(readyVids.Count == videoSync.Length)
                {
                    Debug.Log($"<color={brandingColor}>ISyncVideoPlayers Network Controller</color> : Videos ready for sync.");
                    _videosReady = true;
                }
            }

            if(_videosReady)
            {
                foreach (ISyncVideoPlayer vid in videoSync)
                {
                    vid.syncVideoByServerTime(time);
                }

                Debug.Log($"<color={brandingColor}>ISyncVideoPlayers Network Controller</color> : Videos are synced, let the movie night begin!");

                // only need to update the frame once when the player initially joins.
                // updating frame-by-frame based on player join causes studdering & streaming latency
                _videosReady = false;
                _videosFound = false;
            }
        }

        public void OnIsClient(bool client, ulong steamId) {
            var actors = Resources.FindObjectsOfTypeAll<ISyncVideoPlayer>();

            if(actors.Length > 0) {
                Debug.Log($"<color={brandingColor}>ISyncVideoPlayers Network Controller</color> : Found {actors.Length} VideoPlayers.");
                videoSync = actors;
                _videosFound = true;
            } else {
                Debug.Log($"<color={brandingColor}>ISyncVideoPlayers Network Controller</color> : No VideoPlayers found.");
            }
        }

        #region Unused Methods
        
        public void OnSyncValueState(int value) {
        }
        
        public void OnUpdateElapsedTime(float time) {
        }

        public void OnUpdateTimeRemaining(float time) {
        }

        public void OnIsServer(bool server) {
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
        #endregion
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoldfastSharedMethods;

public class SendVoting : MonoBehaviour
{
    [Serializable]
    public class buttonVote
    {
        public Button voting;
        public int memeId;
    }

    [Serializable]
    public class votingResults
    {
        public GameObject graphics;
        public string submitter;
        public int memeId;
    }


    [Header("User Interface Config")]
    [Space(10)]
    public Button closeUI;
    public GameObject votingPanel;
    public GameObject settingsText;
    public bool menuOpenState = false;
    public GameObject memePanel;
    public GameObject votedPanel;
    public GameObject resultPanel;
    public Text resultVoteCount;
    public Text resultWinnerName;

    [Header("Voting Config")]
    [Space(10)]
    public string playerSteamId;
    public buttonVote[] votingInput;
    public votingResults[] winnerGraphic;
    public bool isDev = false;
    public bool testWinner = false;

    [Header("Discord Config")]
    [Space(10)]
    public Button discordButton;
    public string discordInviteLink = "https://discord.gg/ZM8S8zNZ3C";

    private InputField f1MenuInputField;

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
                    Debug.Log("Voting System - Client : Found the Game Console Panel");
                } else {
                    Debug.LogWarning("Voting System - Client : Game Console Panel not found. This mod may not work correctly!");
                }
                break;
            }
        }

        discordButton.onClick.AddListener(openDiscordServer);
        closeUI.onClick.AddListener(setMenuState);

        foreach (buttonVote item in votingInput)
        {
            item.voting.onClick.AddListener(() => SendVote(item.memeId));
        }

        Invoke("serverSync", 5.0f);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            setMenuState();
        }

        if(testWinner)
        {
            setResultsPanel(6, 69);
        }
    }

    public void serverSync()
    {
        string f1command = $"rc login event:syncVotes:{playerSteamId}";

        if(isDev){
            Debug.Log(f1command);
        }

        f1MenuInputField.onEndEdit.Invoke(f1command);
    }

    void setMenuState(){
        menuOpenState = !menuOpenState;
        settingsText.SetActive(!menuOpenState);
        votingPanel.SetActive(menuOpenState);

        string f1command = $"set shouldUnlockMouse {menuOpenState}";

        if(isDev){
            Debug.Log(f1command);
        }

        f1MenuInputField.onEndEdit.Invoke(f1command);
    }

    public void setCenterContent(int state)
    {
        if(state == 0){
            memePanel.SetActive(true);
            votedPanel.SetActive(false);
            resultPanel.SetActive(false);
        }
        else if(state == 1){
            memePanel.SetActive(false);
            votedPanel.SetActive(true);
            resultPanel.SetActive(false);
        }
        else if(state == 2){
            memePanel.SetActive(false);
            votedPanel.SetActive(false);
            resultPanel.SetActive(true);
        }
    }

    public void setResultsPanel(int winner, int votes)
    {
        // TODO: set result panel to winning meme graphic & votes
        setCenterContent(2);
        winnerGraphic[winner].graphics.SetActive(true);
        resultWinnerName.text = $"{winnerGraphic[winner].submitter} has won!";
        resultVoteCount.text = $"They received {votes} votes!";
    }

    void SendVote(int meme_id)
    {
        string f1command = $"rc login event:vote:{playerSteamId}|{meme_id}";

        if(isDev){
            Debug.Log(f1command);
        }

        f1MenuInputField.onEndEdit.Invoke(f1command);

        setCenterContent(1);
    }

    public void openDiscordServer()
    {
        Application.OpenURL(discordInviteLink);
    }
}

public class SendVoting_Client : IHoldfastSharedMethods
{
    public bool isLocalPlayer = false;
    public bool isClient = false;
    public string localPlayerId;
    public SendVoting voteManager;

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

        var votingManager = Resources.FindObjectsOfTypeAll<SendVoting>();

        if(votingManager.Length == 1) {
            Debug.Log($"Voting System - Client : Player initialized");
            votingManager[0].playerSteamId = localPlayerId;
            voteManager = votingManager[0];
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
    }

    public void OnPlayerLeft(int playerId) {
    }

    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject) {
    }

    public void OnScorableAction(int playerId, int score, ScorableActionType reason) {
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text) {

        // only listen if on client
        if(!isClient){return;}

        // only list if message from server
        if (playerId != -1){return;}

        // Log: -1, sem:winner:6|69
        // Debug.Log($"{playerId}, {localPlayerId}, {text}");

        // listen if message has correct command
        string[] command = text.Split(':');
        if (command[0] != "sem"){ return; }

        string[] inputVariables = command[2].Split('|');

        if(command[1] == "winner")
        {
            var memeWinner =  IntParseFast(inputVariables[0]);
            var totalVotes = IntParseFast(inputVariables[1]);
            voteManager.setResultsPanel(memeWinner, totalVotes);
        }

        // only listen to state events if it is sent to local player
        if(inputVariables[0] != localPlayerId){return;}

        if(command[1] == "state")
        {
            // sem:state:<player steam id>|voted
            if(inputVariables[1] == "voted"){
                // player has already voted
                // Debug.Log($"Player");
                voteManager.setCenterContent(1);
            }

            // sem:state:<player steam id>|results|<winning vote id>|<total votes>
            if(inputVariables[1] == "results"){
                // Results have been sent

                var memeWinner =  IntParseFast(inputVariables[2]);
                var totalVotes = IntParseFast(inputVariables[3]);
                voteManager.setResultsPanel(memeWinner, totalVotes);
            }
        }

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

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoldfastSharedMethods;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class IReplaceWeapons : MonoBehaviour
{

    [Serializable]
    public class weaponPackData
    {
        public string packName;
        public int[] replacementWeaponIndex;
    }

    [Header("Weapon Settings")]
    [SerializeField]
    public List<weaponPackData> weaponPacks; // TODO: implement better weapon packs instead of shit contains logic
    public List<GameObject> Weapons = new List<GameObject>();
    public List<GameObject> requestedWeapons = new List<GameObject>();
    public float replacementInterval = 10f; // default interval replacement
    public bool isDev = false; // in-editor dev bool
    public bool isDevCleanRound = false; // in-editor clear round bool
    public bool isServerDev = false; // server dev bool, for debug mode feature flag
    public bool swapInterval = false;

    private Scene scene;

    public List<GameObject> _spawnedWeapons = new List<GameObject>();
    public List<GameObject> _originalWeapons = new List<GameObject>();
    public List<Quaternion> _originalWeaponRotation = new List<Quaternion>();

    [Header("Debugger Settings")]
    public bool isDevShowUI = false;
    public GameObject debuggerUI;
    public GameObject scopeUIPanel;
    public Dropdown weaponSelector;
    public Button minimizeDebugger;
    public Button updateWeaponData;
    public Button findHandData;
    public InputField[] weaponTransformationData;

    private Animator _debuggerAnimator;
    private bool _isDebuggerMinimized = false;

    [Serializable]
    public class debuggerPositionData
    {
        public Transform weaponTransform;
        public Transform scopeTransform;

        public debuggerPositionData(Transform weapon, Transform scope)
        {
            weaponTransform = weapon;
            if(scope != null){
                scopeTransform = scope;
            } else {
                scopeTransform = null;
            }
        }
    }

    [HideInInspector, SerializeField]
    private List<debuggerPositionData> _debuggerFormData = new List<debuggerPositionData>();

    void Start()
    {
        scene = SceneManager.GetActiveScene();
        _debuggerAnimator = debuggerUI.GetComponent<Animator>();

        if(isDev){
            Debug.LogError($"🔫 IReplaceWeapons : Error Code #69 : IF YOU ARE SEEING THIS CODE DUCKY MADE AN OOPSIE, PLEASE YELL AT HIM!");
            UpdatePlayerList();
        }

        if(isDevShowUI) {
            initDebugger();
        } else {
            debuggerUI.SetActive(false);
        }
    }

    void Update()
    {
        if(isDev && isDevCleanRound){
            cleanRound();
        }
    }

    public void swapByInterval(float intervalSwapTime)
    {
        Debug.Log("IReplaceWeapons : Init interval swap");
        swapInterval = true;
        InvokeRepeating("GetSmoothed", 0.0f, intervalSwapTime);
    }

    public void initRequestedWeapons(List<int> weaponRequest)
    {
        if(weaponRequest.Count <= 0)
        {
            Debug.LogError($"IReplaceWeapons : No weapons requested");
            return;
        }

        foreach (int w in weaponRequest)
        {
            if(w >= 0 && w < Weapons.Count)
            {
                requestedWeapons.Add(Weapons[w]);
            }
            else
            {
                Debug.LogError($"IReplaceWeapons : Invalid weapon index of {w}, must be less than {Weapons.Count}");
            }
        }
    }

    // reverts weapons for next round
    public void cleanRound()
    {
        if(swapInterval){
            CancelInvoke();
        }

        var index = 0;
        if(_originalWeapons.Count > 0)
        {
            foreach (GameObject og in _originalWeapons)
            {
                if(og != null )
                { 
                    og.SetActive(true);
                    // og.transform.rotation = _originalWeaponRotation[index];
                }

                index++;
            }
        }

        if(_spawnedWeapons.Count > 0)
        {
            foreach (GameObject spawned in _spawnedWeapons)
            {
                if( spawned != null )
                { 
                     Destroy(spawned);
                }
            }
        }
    }

    // debugger funtion to list children in hand
    public void getHandData()
    {
        var actors = scene.GetRootGameObjects();
        List<GameObject> _charModelHandler = new List<GameObject>();
        List<GameObject> _playerRig = new List<GameObject>();

        foreach (GameObject gameObj in actors)
        {
            if(gameObj.name.Contains("Smoothed"))
            {
                Transform[] allKids = gameObj.GetComponentsInChildren<Transform>(true);
                
                foreach (Transform kid in allKids)
                {
                    if(kid.name.Contains("Character Model Handler(Clone)"))
                    {
                        _charModelHandler.Add(kid.gameObject);
                    }
                }
            }
        }
        
        foreach(GameObject cmh in _charModelHandler)
        {
            Transform cmhRig = cmh.transform.GetChild(0);
            Transform itemRoot = cmhRig.transform.Find("Root Item Bone");
            if(itemRoot != null)
            {
                _playerRig.Add(itemRoot.gameObject);

            }
        }

        foreach (GameObject pr in _playerRig)
        {
            Debug.Log("##############################");
            Debug.Log($"Starting {pr.name}");
            Debug.Log("##############################");
            PrintAllChildren(pr, "");
            Debug.Log("##############################");
        }
    }

    // recursive function for printing hand children
    void PrintAllChildren(GameObject obj, string path)
    {
        // Get the object's name
        string objName = obj.name;

        // Add this object's name to the path
        path += "/" + objName;

        // Print the full path if the object has no children
        if (obj.transform.childCount == 0)
        {
            Debug.Log(path);
            return;
        }

        // Recursively print the children
        foreach (Transform child in obj.transform)
        {
            PrintAllChildren(child.gameObject, path);
        }
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

    public void SwapChildByName(List<GameObject> playerRigs)
    {
        List<GameObject> oldGameObjects = new List<GameObject>();
        
        foreach (GameObject item in playerRigs)
        {
            Transform[] allData = item.GetComponentsInChildren<Transform>(true);

            foreach(Transform g in allData)
            {
                var gameObj = g.gameObject;
                foreach(GameObject w in requestedWeapons){
                    var wi = w.GetComponent<WeaponData>();
                    var logic = wi.replaceLogicContains;

                    if((gameObj.name.Contains(wi.WeaponHierarchy[0]) && logic == true) || (isDev && gameObj.name == wi.WeaponHierarchy[0] && logic == false))
                    {
                        if(wi.hierarchySearch)
                        {
                            var childPath = String.Join("/",wi.WeaponHierarchy.Skip(1));
                            var child = GetChildWithName(gameObj, childPath);

                            if(child != null)
                            {
                                oldGameObjects.Add(child);
                            }

                            if(wi.hideRamRod && wi.ramRodObjectName != null)
                            {
                                // remove last item from array as this should be the ramrod
                                var ramChildSub = wi.WeaponHierarchy.Take(wi.WeaponHierarchy.Length - 1);
                                var ramChildPath = String.Join("/",ramChildSub.Skip(1)) + $"/{wi.ramRodObjectName}";
                                var ramRod = GetChildWithName(gameObj, ramChildPath);

                                if(ramRod != null)
                                {
                                    oldGameObjects.Add(ramRod);
                                }
                                else
                                {
                                    if(isServerDev)
                                    {
                                        Debug.LogWarning($"IReplaceWeapons : Ramrod not found for {gameObj.name}");
                                    }
                                }
                            }

                            if(wi.hideBayonet && wi.bayonetObjectName != null)
                            {
                                // remove last item from array as this should be the bayonet
                                var bayChildSub = wi.WeaponHierarchy.Take(wi.WeaponHierarchy.Length - 1);
                                var bayChildPath = String.Join("/",bayChildSub.Skip(1)) + $"/{wi.bayonetObjectName}";
                                var bay = GetChildWithName(gameObj, bayChildPath);

                                if(bay != null)
                                {
                                    oldGameObjects.Add(bay);
                                }
                                else
                                {
                                    if(isServerDev)
                                    {
                                        Debug.LogWarning($"IReplaceWeapons : Bayonet not found for {gameObj.name}");
                                    }
                                }
                            }
                        }
                        else if(isDev)
                        {
                            oldGameObjects.Add(gameObj);
                        }
                    }
                }
            }
        }
        

        if(oldGameObjects.Count > 0)
        {
            SwapAllByArray(oldGameObjects);
        }
    }

    GameObject GetChildWithName(GameObject obj, string name) 
    {
        Transform trans = obj.transform;
        Transform childTrans = trans.Find(name);

        if (childTrans != null && childTrans.gameObject.activeSelf) {
            return childTrans.gameObject;
        } else {
            return null;
        }
    }

    void SwapPrefabs(GameObject oldGameObject)
    {
        // Instantiate the new game object at the old game objects position and rotation.
        foreach(GameObject wi in requestedWeapons){
            var weaponInfo = wi.GetComponent<WeaponData>();
            var logic = weaponInfo.replaceLogicContains;
            var parentGameObject = GetParentAtLevel(oldGameObject,weaponInfo.WeaponHierarchy.Length - 1);
            var childName = weaponInfo.WeaponHierarchy.Last();

            if(isServerDev)
            {
                Debug.Log($"IReplace Weapons : Debug Mode : I found a parent of {parentGameObject.name} this right?");
            }

            // handle hiding ram rod
            if(weaponInfo.hideRamRod && (weaponInfo.ramRodObjectName == oldGameObject.name) )
            {
                oldGameObject.SetActive(false);
            }

            // handle hiding bayonet
            if(weaponInfo.hideBayonet && (weaponInfo.bayonetObjectName == oldGameObject.name) )
            {
                oldGameObject.SetActive(false);
            }

            if((oldGameObject.name.Contains(weaponInfo.WeaponHierarchy[0]) && logic == true) || (isDev && oldGameObject.name == weaponInfo.WeaponHierarchy[0] && logic == false) || (weaponInfo.hierarchySearch && oldGameObject.name == childName && parentGameObject.name.Contains(weaponInfo.WeaponHierarchy[0]) && logic == true))
            {

                _originalWeapons.Add(oldGameObject);
                Quaternion rotation = oldGameObject.transform.rotation;
                Vector3 position = oldGameObject.transform.position;
                GameObject swapModel = wi;

                if(isServerDev){
                    Debug.Log("<color=#00FFFF>--------------------------------------------------</color>");
                    Debug.Log($"Old Model: {oldGameObject.name}");
                    Debug.Log($"New Model: {swapModel.name}");
                    Debug.Log("--------------------------------------------------");
                    Debug.Log("------------ <color=#00FFFF>HIERARCHY OF REPLACEMENT</color> ------------");
                    Debug.Log("--------------------------------------------------");
                    Debug.Log($"{oldGameObject.transform.parent.name}");
                    Debug.Log($"{oldGameObject.transform.parent.transform.parent.name}");
                    Debug.Log($"{oldGameObject.transform.parent.transform.parent.transform.parent.name}");
                    Debug.Log($"{oldGameObject.transform.parent.transform.parent.transform.parent.transform.parent.name}");
                    Debug.Log($"{oldGameObject.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.name}");
                    Debug.Log($"{oldGameObject.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.transform.parent.name}");
                    Debug.Log("<color=#00FFFF>--------------------------------------------------</color>");
                }

                GameObject newGameObject = Instantiate(swapModel, position, rotation);
                newGameObject.transform.Rotate(weaponInfo.WeaponPrefabRotationalOffset);

                _spawnedWeapons.Add(newGameObject);

                if (oldGameObject.transform.parent != null)
                {
                    newGameObject.transform.SetParent(oldGameObject.transform.parent);
                }

                oldGameObject.SetActive(false);
            }
        }

        GameObject GetParentAtLevel(GameObject gameObject, int level)
        {
            Transform currentTransform = gameObject.transform;

            for (int i = 0; i < level; i++)
            {
                if (currentTransform.parent == null)
                {
                    // Reached the top before reaching the desired level
                    return null;
                }

                currentTransform = currentTransform.parent;
            }

            return currentTransform.gameObject;
        }
    }

    #region PlayerManager

    public void UpdatePlayerList(float delay = 5.0f)
    {
        Invoke("GetSmoothed", delay);
    }

    public void GetSmoothed()
    {
        var actors = scene.GetRootGameObjects();
        List<GameObject> _charModelHandler = new List<GameObject>();
        List<GameObject> _playerRig = new List<GameObject>();

        if(swapInterval && isServerDev)
        {
            Debug.Log("IReplaceWeapons : Swap interval invoked.");
        }

        foreach (GameObject gameObj in actors)
        {
            if(gameObj.name.Contains("Smoothed"))
            {
                Transform[] allKids = gameObj.GetComponentsInChildren<Transform>(true);
                
                foreach (Transform kid in allKids)
                {
                    if(kid.name.Contains("Character Model Handler(Clone)"))
                    {
                        _charModelHandler.Add(kid.gameObject);
                    }
                }
            }
        }
        
        foreach(GameObject cmh in _charModelHandler)
        {
            Transform cmhRig = cmh.transform.GetChild(0);
            Transform itemRoot = cmhRig.transform.Find("Root Item Bone");
            if(itemRoot != null)
            {
                _playerRig.Add(itemRoot.gameObject);

            }
        }

        SwapChildByName(_playerRig);
    }

    #endregion

    #region Debugger

    public void initDebugger(){

        debuggerUI.SetActive(true);
        Debug.Log($"🔫 IReplaceWeapons : Adding Debugger to round. Cheer-o good sport!");

        weaponSelector.ClearOptions();
        List<string> weaponSelectorOptions = new List<string>();

        foreach (GameObject w in Weapons)
        {
            var data = w.GetComponent<WeaponData>();
            debuggerPositionData formData;
            if(data.scopeCamera != null) {
                formData = new debuggerPositionData(w.transform.GetChild(0), data.scopeCamera.transform);
            }else{
                formData = new debuggerPositionData(w.transform.GetChild(0), null);
            }
            weaponSelectorOptions.Add(data.WeaponName);
            _debuggerFormData.Add(formData);
        }

        updateWeaponData.interactable = true;
        weaponSelector.AddOptions(weaponSelectorOptions); 
        debuggerSetOptions(0);

        weaponSelector.onValueChanged.AddListener(delegate {
            DropdownValueChanged(weaponSelector);
        });

        findHandData.onClick.AddListener(getHandData);
        updateWeaponData.onClick.AddListener(updateWeaponPlacement);
        minimizeDebugger.onClick.AddListener(minimizeHandler);
    }

    void minimizeHandler(){
        _isDebuggerMinimized = !_isDebuggerMinimized;

        _debuggerAnimator.SetBool("isClosed",_isDebuggerMinimized);

        if(_isDebuggerMinimized){
            minimizeDebugger.gameObject.GetComponentInChildren<Text>().text = "+";
        } else {
            minimizeDebugger.gameObject.GetComponentInChildren<Text>().text = "-";
        }
    }

    void DropdownValueChanged(Dropdown change)
    {
        debuggerSetOptions(change.value);
    }

    public void debuggerSetOptions(int index){
        var data = _debuggerFormData[index];

        weaponTransformationData[0].text = data.weaponTransform.position.x.ToString("F6");
        weaponTransformationData[0].readOnly = false;
        weaponTransformationData[1].text = data.weaponTransform.position.y.ToString("F6");
        weaponTransformationData[1].readOnly = false;
        weaponTransformationData[2].text = data.weaponTransform.position.z.ToString("F6");
        weaponTransformationData[2].readOnly = false;

        weaponTransformationData[3].text = data.weaponTransform.localEulerAngles.x.ToString("F6");
        weaponTransformationData[3].readOnly = false;
        weaponTransformationData[4].text = data.weaponTransform.localEulerAngles.y.ToString("F6");
        weaponTransformationData[4].readOnly = false;
        weaponTransformationData[5].text = data.weaponTransform.localEulerAngles.z.ToString("F6");
        weaponTransformationData[5].readOnly = false;

        if(data.scopeTransform != null){
            scopeUIPanel.SetActive(true);

            weaponTransformationData[6].text = data.scopeTransform.localPosition.x.ToString("F6");
            weaponTransformationData[6].readOnly = false;
            weaponTransformationData[7].text = data.scopeTransform.localPosition.y.ToString("F6");
            weaponTransformationData[7].readOnly = false;
            weaponTransformationData[8].text = data.scopeTransform.localPosition.z.ToString("F6");
            weaponTransformationData[8].readOnly = false;

            weaponTransformationData[9].text = data.scopeTransform.localEulerAngles.x.ToString("F6");
            weaponTransformationData[9].readOnly = false;
            weaponTransformationData[10].text = data.scopeTransform.localEulerAngles.y.ToString("F6");
            weaponTransformationData[10].readOnly = false;
            weaponTransformationData[11].text = data.scopeTransform.localEulerAngles.z.ToString("F6");
            weaponTransformationData[11].readOnly = false;

        } else {
            scopeUIPanel.SetActive(false);

            weaponTransformationData[6].text = "0";
            weaponTransformationData[6].readOnly = true;
            weaponTransformationData[7].text = "0";
            weaponTransformationData[7].readOnly = true;
            weaponTransformationData[8].text = "0";
            weaponTransformationData[8].readOnly = true;

            weaponTransformationData[9].text = "0";
            weaponTransformationData[9].readOnly = true;
            weaponTransformationData[10].text = "0";
            weaponTransformationData[10].readOnly = true;
            weaponTransformationData[11].text = "0";
            weaponTransformationData[11].readOnly = true;
        }
    }

    public void updateWeaponPlacement(){
        var weaponIndex = weaponSelector.value;
        var weaponActors = Resources.FindObjectsOfTypeAll<WeaponData>();
        var data = _debuggerFormData[weaponIndex];

        foreach (WeaponData w in weaponActors)
        {
            if(w.WeaponName == weaponSelector.options[weaponSelector.value].text && w.gameObject.name.Contains("(Clone)"))
            {
                var curr = w.transform.GetChild(0);
                // get position values
                float wp_x = float.Parse(weaponTransformationData[0].text);
                float wp_y = float.Parse(weaponTransformationData[1].text);
                float wp_z = float.Parse(weaponTransformationData[2].text);
                // get rotation values
                float wr_x = float.Parse(weaponTransformationData[3].text);
                float wr_y = float.Parse(weaponTransformationData[4].text);
                float wr_z = float.Parse(weaponTransformationData[5].text);

                w.transform.GetChild(0).localPosition = new Vector3(wp_x,wp_y,wp_z);
                w.transform.GetChild(0).localRotation = Quaternion.Euler(wr_x,wr_y,wr_z);

                if(scopeUIPanel.activeSelf){
                    // get position values
                    float cp_x = float.Parse(weaponTransformationData[6].text);
                    float cp_y = float.Parse(weaponTransformationData[7].text);
                    float cp_z = float.Parse(weaponTransformationData[8].text);
                    // get rotation values
                    float cr_x = float.Parse(weaponTransformationData[9].text);
                    float cr_y = float.Parse(weaponTransformationData[10].text);
                    float cr_z = float.Parse(weaponTransformationData[11].text);

                    w.scopeCamera.transform.localPosition = new Vector3(cp_x,cp_y,cp_z);
                    w.scopeCamera.transform.localRotation = Quaternion.Euler(cr_x,cr_y,cr_z);
                }
            }
        }
    }
    #endregion
}

public class IReplaceWeaponsInterface : IHoldfastSharedMethods
{
    private IReplaceWeapons WeaponManager;
    private InputField f1MenuInputField;
    public GameObject Smoothed;
    public GameObject TempPlayer;
    public bool spawnedWithNoSmoothed = false;
    
    public List<int> requestedWeapons = new List<int>();
    public bool updatePlayerList = false;
    public float delay = 5.0f;
    public bool roundEnd = false;

    public double lastUpdateTime;

    public bool isClient = false;
    public string localPlayerSteamId;
    public string localPlayerId;

    public string defaultLogColor = "#00FFFF";
    public string debugLogColor = "#ff6f00";
    public string errorLogColor = "#ff5e5e";

    // feature flags
    public bool debugMode = false;
    public bool swapInterval = false;
    public float swapIntervalTime = 5.0f;

    public void OnSyncValueState(int value) {
    }

    public void OnUpdateSyncedTime(double time) {

        // swapInterval is garbage, use interval swap FF
        if(!swapInterval && updatePlayerList == true && (time - lastUpdateTime) > 10)
        {
            if(debugMode)
            {
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Recieved Player Weapon Update</color>");
            }
            updatePlayerList = false;
            lastUpdateTime = time;
            WeaponManager.UpdatePlayerList(delay);
        }

        if(roundEnd == true)
        {
            if(debugMode)
            {
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Recieved Round End Update</color>");
            }

            roundEnd = false;
            WeaponManager.cleanRound();
        }
    }

    public void OnUpdateElapsedTime(float time) {
    }

    public void OnUpdateTimeRemaining(float time) {
    }

    public void OnIsServer(bool server) {
    }

    public void OnIsClient(bool client, ulong steamId) {

        // init is on client var
        isClient = client;

        // only listen if on client
        if(!isClient){return;}
        localPlayerSteamId = $"{steamId}";

        var weaponActors = Resources.FindObjectsOfTypeAll<IReplaceWeapons>();
        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        

        // Get the "Game Console Panel"
        for (int i = 0; i < canvases.Length; i++) {
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0) {
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (f1MenuInputField != null) {
                    if(debugMode)
                    {
                        Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons : Found the Game Console Panel</color>");
                    }
                } else {
                    Debug.LogError($"<color={errorLogColor}>🔫 IReplaceWeapons : ❗❗Game Console Panel not found. This mod may not work correctly!❗❗</color>");
                }
                break;
            }
        }

        if(weaponActors.Length > 0) {
            if(debugMode)
            {
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons : Found Weapon Manager.</color>");
            }

            WeaponManager = weaponActors[0];

            WeaponManager.initRequestedWeapons(requestedWeapons);

            if(debugMode){
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons : Init Debugger</color>");
                WeaponManager.initDebugger();
                WeaponManager.isServerDev = true;
            }

            if(swapInterval){
                if(debugMode)
                {
                    Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons : Feature Flag : Swap by interval enabled. Weapons will be swapped every {swapIntervalTime} seconds.</color>");
                }
                WeaponManager.swapByInterval(swapIntervalTime);
            }

        }
        else {
            Debug.LogError($"<color={errorLogColor}>🔫 IReplaceWeapons : ❗❗No Weapon Manager found. This mod may not work correctly❗❗</color>");
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
        if(!isClient){return;}
        if($"{steamId}" == localPlayerSteamId){ localPlayerId = $"{playerId}"; }
    }

    public void OnPlayerLeft(int playerId) {
    }

    public void OnPlayerSpawned(int playerId, int spawnSectionId, FactionCountry playerFaction, PlayerClass playerClass, int uniformId, GameObject playerObject) {
        if(!swapInterval){
            if(debugMode)
            {
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Player {playerId} Spawned, Send Request For Player Weapon Update</color>");
            }

            updatePlayerList = true;
            // give it some time for the player to spawn
            delay = 5.0f;
        }
    }

    public void OnScorableAction(int playerId, int score, ScorableActionType reason) {
    }

    public void OnTextMessage(int playerId, TextChatChannel channel, string text) {
        // listen if message has correct command
        string[] command = text.Split(':');
        if (command[0] != "ireplaceweapons"){ return; }

        if(command[1] == "cleanround")
        {
            WeaponManager.cleanRound();
        }
    }

    public void OnRoundDetails(int roundId, string serverName, string mapName, FactionCountry attackingFaction, FactionCountry defendingFaction, GameplayMode gameplayMode, GameType gameType) {
    }

    public void OnPlayerBlock(int attackingPlayerId, int defendingPlayerId) {
    }

    public void OnPlayerMeleeStartSecondaryAttack(int playerId) {
    }

    public void OnPlayerWeaponSwitch(int playerId, string weapon) {
        // swapInterval is garbage, use interval swap FF
        if(!swapInterval){
            if(debugMode)
            {
                Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Player {playerId} Switched Weapon, Send Request For Player Weapon Update</color>");
            }

            updatePlayerList = true;
            // player already exists, no need for delay
            delay = 0.0f;
        }
    }

    public void OnCapturePointCaptured(int capturePoint) {
    }

    public void OnCapturePointOwnerChanged(int capturePoint, FactionCountry factionCountry) {
    }

    public void OnCapturePointDataUpdated(int capturePoint, int defendingPlayerCount, int attackingPlayerCount) {
    }

    public void OnRoundEndFactionWinner(FactionCountry factionCountry, FactionRoundWinnerReason reason) {
        if(debugMode)
        {
            Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Round End. Remove replacement weapons</color>");
        }

        roundEnd = true;
    }

    public void OnRoundEndPlayerWinner(int playerId) {
        if(debugMode)
        {
            Debug.Log($"<color={defaultLogColor}>🔫 IReplaceWeapons :</color><color={debugLogColor}> DEBUG MODE</color><color={defaultLogColor}> : Round End. Remove replacement weapons</color>");
        }

        roundEnd = true;
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
        foreach (string val in value)
        {
            string[] command = val.Split(':');
            if (command[0] != "IReplaceWeapons"){ continue; }

            // Handle feature flags
            if(command[1]=="feature_flag"){
                if(command[2]=="debug_mode")
                {
                    debugMode = true;
                }

                if(command[2]=="swap_interval")
                {
                    swapInterval = true;
                    
                    if(3 < command.Length)
                    {
                        swapIntervalTime = float.Parse(command[3]);
                    }
                }
            }

            // TODO: make iterative logic to get rid of if/else tower of terror

            if(command[1]=="pack")
            {
                // dear god this is bad.
                // make an iterative solution at some point
                if(command[2]=="meme" && (!requestedWeapons.Contains(0) || !requestedWeapons.Contains(1) || !requestedWeapons.Contains(2) || !requestedWeapons.Contains(3) || !requestedWeapons.Contains(4) || !requestedWeapons.Contains(5)|| !requestedWeapons.Contains(6)|| !requestedWeapons.Contains(7)|| !requestedWeapons.Contains(8)|| !requestedWeapons.Contains(9)|| !requestedWeapons.Contains(10)|| !requestedWeapons.Contains(11)|| !requestedWeapons.Contains(12)|| !requestedWeapons.Contains(13)|| !requestedWeapons.Contains(14)) )
                {
                    requestedWeapons.Add(0);
                    requestedWeapons.Add(1);
                    requestedWeapons.Add(2);
                    requestedWeapons.Add(3);
                    requestedWeapons.Add(4);
                    requestedWeapons.Add(5);
                    requestedWeapons.Add(6);
                    requestedWeapons.Add(7);
                    requestedWeapons.Add(8);
                    requestedWeapons.Add(9);
                    requestedWeapons.Add(10);
                    requestedWeapons.Add(11);
                    requestedWeapons.Add(12);
                    requestedWeapons.Add(13);
                    requestedWeapons.Add(14);
                    requestedWeapons.Add(15);
                    requestedWeapons.Add(16);
                    requestedWeapons.Add(17);
                    requestedWeapons.Add(18);
                    requestedWeapons.Add(19);
                    requestedWeapons.Add(20);
                    requestedWeapons.Add(21);
                    requestedWeapons.Add(22);
                    requestedWeapons.Add(23);
                    requestedWeapons.Add(24);
                    requestedWeapons.Add(25);
                }
                else if(command[2]=="halo")
                {
                    // TODO add halo configs
                    Debug.LogError($"<color={errorLogColor}>🔫 IReplaceWeapons : Halo weapon pack is currently not avaliable.</color>");
                }
                else if(command[2]=="caveman")
                {
                    requestedWeapons.Add(26);
                    requestedWeapons.Add(27);
                }
                else
                {
                    Debug.LogError($"<color={errorLogColor}>🔫 IReplaceWeapons : {command[2]} is not a valid weapon pack.</color>");
                }
            } 
            else if(!requestedWeapons.Contains(0) && command[1]=="intervention" && command[2]=="enfield_no_bayonet")
            {
                requestedWeapons.Add(0);
            }
            else if(!requestedWeapons.Contains(1) && command[1]=="master_sword" && command[2]=="pattern_1803_flank_officer")
            {
                requestedWeapons.Add(1);
            }
            else if(!requestedWeapons.Contains(2) && command[1]=="anakin_lightsaber" && command[2]=="1796_pattern_light_sabre")
            {
                requestedWeapons.Add(2);
            }
            else if(!requestedWeapons.Contains(3) && command[1]=="nerf_gun" && command[2]=="pistol_xii")
            {
                requestedWeapons.Add(3);
            }
            else if(!requestedWeapons.Contains(4) && command[1]=="spas12_normal" && command[2]=="british_blunderbuss")
            {
                requestedWeapons.Add(4);
            }
            else if(!requestedWeapons.Contains(5) && command[1]=="spas12_gold" && command[2]=="french_blunderbuss")
            {
                requestedWeapons.Add(5);
            }
            else if(!requestedWeapons.Contains(6) && command[1]=="m16_rifle" && command[2]=="charleville_ixxii")
            {
                requestedWeapons.Add(6);
            }
            else if(!requestedWeapons.Contains(7) && command[1]=="glamdring" && command[2]=="russian_hussar")
            {
                requestedWeapons.Add(7);
            }
            else if(!requestedWeapons.Contains(8) && command[1]=="red_lightsaber" && command[2]=="1796_epee")
            {
                requestedWeapons.Add(8);
            }
        }
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
        if(success != true){ return; }

        if(input.Contains("mapRotation")){
            WeaponManager.cleanRound();

            Debug.LogError($"<color={errorLogColor}>🔫 IReplaceWeapons : ❗❗Manually setting the mapRotation may cause issues❗❗</color>");
        }

        // this doesn't work obviously
        if(input == "rc ireplaceweapons forceClean"){
            // WeaponManager.cleanRound(); 
            f1MenuInputField.onEndEdit.Invoke($"rc serverAdmin quietBroadcastMessage ireplaceweapons:cleanRound");
        }
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
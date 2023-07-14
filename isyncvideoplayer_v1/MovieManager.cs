using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HoldfastSharedMethods;

public class MovieManager : MonoBehaviour
{
    public AudioSource movieAudio;
    public GameObject theaterSettingsPanel;
    public GameObject settingsText;
    public Slider volumeSlider;
    public bool menuOpenState = false;
    public Button discordButton;
    public string discordInviteLink = "https://discord.gg/ZM8S8zNZ3C";

    private InputField f1MenuInputField; // Game Console input
    private string brandingColor = "#34eb49"; // used to make our logs look pretty

    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.onValueChanged.AddListener (delegate {ValueChangeCheck();});
        discordButton.onClick.AddListener(openDiscordServer);

        var canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        for (int i = 0; i < canvases.Length; i++) {
            //Find the one that's called "Game Console Panel"
            if (string.Compare(canvases[i].name, "Game Console Panel", true) == 0) {
                //Inside this, now we need to find the input field where the player types messages.
                f1MenuInputField = canvases[i].GetComponentInChildren<InputField>(true);
                if (f1MenuInputField != null) {
                    Debug.Log($"<color={brandingColor}>Movie Manager</color> : Found the Game Console Panel");
                } else {
                    Debug.Log($"<color={brandingColor}>Movie Manager</color> : Game Console Panel not found. This mod may not work correctly!");
                }
                break;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            menuOpenState = !menuOpenState;
            settingsText.SetActive(!menuOpenState);
            theaterSettingsPanel.SetActive(menuOpenState);

            string f1command = $"set shouldUnlockMouse {menuOpenState}";
            f1MenuInputField.onEndEdit.Invoke(f1command);
        }
    }

    public void openDiscordServer()
    {
        Application.OpenURL(discordInviteLink);
    }

    public void ValueChangeCheck()
	{
		movieAudio.volume = volumeSlider.value;
	}
}
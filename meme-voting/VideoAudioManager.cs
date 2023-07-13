using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class VideoAudioManager : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler
{
    public AudioSource audioSource;
    public GameObject audioPlay;
    public GameObject audioMute;
    private bool mouse_over = false;

    void Update()
    {
        if (mouse_over)
        {
            audioSource.mute = false;
            audioPlay.SetActive(true);
            audioMute.SetActive(false);
        } 
        else
        {
            audioSource.mute = true;
            audioPlay.SetActive(false);
            audioMute.SetActive(true);
        } 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_over = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_over = false;
    }
}
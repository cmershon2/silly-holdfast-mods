using System.Collections;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(VideoPlayer))]
public class VideoLoader : MonoBehaviour
{
    public string videoURL;
    public bool usingCDN = false;
    public bool isVideoReady = false;
    public RawImage[] output;

    private VideoPlayer _myVideoPlayer;

    void Start()
    {
        //rend = GetComponent<Renderer>();
        _myVideoPlayer = GetComponent<VideoPlayer>();

        if(usingCDN)
        {
            _myVideoPlayer.playOnAwake = false;
            _myVideoPlayer.source = VideoSource.Url;
            _myVideoPlayer.url = videoURL;

            _myVideoPlayer.Prepare();

            _myVideoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
        }
        else
        {
            StartCoroutine(LoadExternalVideo(videoURL));
        }
    }

    private void VideoPlayerPrepareCompleted(VideoPlayer source)
    {
        _myVideoPlayer.Play();
    }

    void Update()
    {
        if(_myVideoPlayer.isPrepared)
        {
            isVideoReady = true;

            foreach (RawImage item in output)
            {
                item.texture = _myVideoPlayer.texture;
            }
        }
    }

    IEnumerator LoadExternalVideo(string url){
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            //Fetches a page and displays the number of characters of the response.
            yield return request.SendWebRequest();
            _myVideoPlayer.url = request.url;
            _myVideoPlayer.isLooping = true;
            _myVideoPlayer.Play();
        }
    }

}
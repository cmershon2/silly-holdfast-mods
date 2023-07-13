using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;
using System.Collections;

namespace CodeDucky.Syncing.VideoPlayers
{
    [RequireComponent(typeof(VideoPlayer))]
    public class ISyncVideoPlayer : MonoBehaviour
    {
        public string videoURL;
        public GameObject loadingCanvas;
        public float videoSpeedModifier = 0.001f;
        public bool isDev = false;
        public bool usingCDN = false;

        [HideInInspector]
        public bool isVideoReady = false;
        private VideoPlayer _myVideoPlayer;
        private double _currentFrame = 0;
        private double _videoStartTime;
        private bool _serverInit = false;

        void Start()
        {
            _myVideoPlayer = GetComponent<VideoPlayer>();

            if(usingCDN)
            {
                _myVideoPlayer.playOnAwake = false;
                _myVideoPlayer.source = VideoSource.Url;
                _myVideoPlayer.url = videoURL;

                ShowLoadingIcon();
                _myVideoPlayer.Prepare();

                _myVideoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
            }
            else
            {
                ShowLoadingIcon();
                StartCoroutine(LoadExternalVideo(videoURL));
            }
        }

        private void VideoPlayerPrepareCompleted(VideoPlayer source)
        {
            loadingCanvas.SetActive(false);
            _myVideoPlayer.Play();
        }

        private void ShowLoadingIcon()
        {
            loadingCanvas.SetActive(true);
        }

        private void HideLoadingIcon()
        {
            loadingCanvas.SetActive(false);
        }

        void Update()
        {
            if(_myVideoPlayer.isPlaying)
            {
                isVideoReady = true;
                HideLoadingIcon();
            }
        }

        public void syncVideoByServerTime(double time)
        {
            double calcTime = time % _myVideoPlayer.length;
            _myVideoPlayer.time = calcTime;
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

        /// Gets the duration of the video in seconds.
        public ulong Duration
        {
            get { return _myVideoPlayer.frameCount / (ulong)_myVideoPlayer.frameRate; }
        }

        /// Jumps to the specified time in the video.
        public void Seek(float time)
        {
            time = Mathf.Clamp(time, 0, 1);
            _myVideoPlayer.time = time * Duration;
        }
    }
}
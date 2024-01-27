using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Hahaha {
    public class StreamVideo : MonoBehaviour
    {
        public RawImage rawImage;
        public VideoPlayer videoPlayer;
        public AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(PlayVideo());
        }

        IEnumerator PlayVideo()
        {
            videoPlayer.prepareCompleted += VideoPrepared;
            videoPlayer.Prepare();

            WaitForSeconds waitForSeconds = new WaitForSeconds(1);
            while (!videoPlayer.isPrepared)
            {
                yield return waitForSeconds;
                break;
            }

            rawImage.texture = videoPlayer.texture;
            videoPlayer.Play();
            audioSource.Play();
        }

        void VideoPrepared(VideoPlayer vp)
        {
            float videoRatio = (float)vp.width / (float)vp.height;
            float screenRatio = (float)Screen.width / (float)Screen.height;

            if (videoRatio > screenRatio)
            {
                // screen is narrower than video
                rawImage.rectTransform.sizeDelta =
                    new Vector2(Screen.width, Screen.width / videoRatio);
            }
            else
            {
                // screen is wider than video
                rawImage.rectTransform.sizeDelta =
                    new Vector2(Screen.height * videoRatio, Screen.height);
            }

        }
    }
}

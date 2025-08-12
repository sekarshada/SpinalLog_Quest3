using System.Data.Common;
using UnityEngine;
using UnityEngine.Video;
public class VideoUIManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void Start()
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(false);
        }

        // Ensure the video starts paused
        videoPlayer.Pause();
    }

    public void ShowVideo()
    {
        if (videoPlayer != null)
        {
            videoPlayer.gameObject.SetActive(true);
        }
    }

    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }
    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
    }
    public void ReplayVideo()
    {
        videoPlayer.Stop();
        videoPlayer.Play();
    }
}
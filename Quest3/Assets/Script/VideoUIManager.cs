using UnityEngine;
using UnityEngine.Video;
public class VideoUIManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
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
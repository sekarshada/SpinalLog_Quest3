using UnityEngine;
public class CueManager : MonoBehaviour
{
    public GameObject glowL4;
    public GameObject arrowCue;
    public GameObject countdownArc;
    public void ShowGlowZone(string id)
    {
        HideAllCues();
        if (id == "L4" && glowL4 != null)
            glowL4.SetActive(true);
    }
    public void ShowArrow(Vector3 from, Vector3 to)
    {
        HideAllCues();
        if (arrowCue != null)
        {
            arrowCue.SetActive(true);
            arrowCue.transform.position = from;
            arrowCue.transform.LookAt(to);
        }
    }
    public void ShowCountdown(float seconds)
    {
        HideAllCues();
        if (countdownArc != null)
        {
            countdownArc.SetActive(true);
            // Optional: animate countdown using a coroutine
        }
    }
    public void HideAllCues()
    {
        if (glowL4 != null) glowL4.SetActive(false);
        if (arrowCue != null) arrowCue.SetActive(false);
        if (countdownArc != null) countdownArc.SetActive(false);
    }
}
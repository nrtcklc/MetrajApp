using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SplashInputBlocker : MonoBehaviour
{
    public GameObject inputBlocker; // InputBlocker objesi

    [SerializeField]
    float blockDuration; // Splash süresi

    private void Start()
    {
        StartCoroutine(DisableAfterDelay());
    }

    IEnumerator DisableAfterDelay()
    {
        if (inputBlocker != null)
            inputBlocker.SetActive(true);

        yield return new WaitForSeconds(blockDuration);

        if (inputBlocker != null)
            inputBlocker.SetActive(false);
    }
}
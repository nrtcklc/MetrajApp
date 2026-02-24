using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro; // EKLENDİ

public class LoadingScreen : MonoBehaviour
{
    public Slider loadingSlider;
    public TMP_Text percentText;     // Text → TMP_Text
    public TMP_Text loadingText;     // Text → TMP_Text
    public float minLoadTime = 3f;

    private void Start()
    {
        StartCoroutine(LoadMainSceneAsync());
        StartCoroutine(AnimateLoadingText());
    }

    IEnumerator LoadMainSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("MainScene");
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / minLoadTime);

            if (loadingSlider != null)
                loadingSlider.value = progress;

            if (percentText != null)
            {
                int displayPercent = Mathf.RoundToInt(progress * 100f / 5f) * 5;
                percentText.text = displayPercent + "%";
            }

            if (op.progress >= 0.9f && elapsed >= minLoadTime)
                op.allowSceneActivation = true;

            yield return null;
        }
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "Yükleniyor";
        int dotCount = 0;

        while (true)
        {
            dotCount = (dotCount % 3) + 1;

            if (loadingText != null)
                loadingText.text = baseText + new string('.', dotCount);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
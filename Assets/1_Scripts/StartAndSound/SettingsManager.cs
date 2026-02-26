using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("UI")]
    public Slider volumeSlider;
    public TMP_Text volumeText;

    public GameObject volumeOff, volume1, volume2, volume3;

    private string packageName = "com.NurtacKilic.MetrajHakedis";
    private string playStoreUrl = "https://play.google.com/store/apps/details?id=com.NurtacKilic.MetrajHakedis";

    private void Start()
    {
        // Slider sýnýrlarý
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 10f;
        }

        // PlayerPrefs yükleme
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 5f);


        ApplyVolume();

        // Dinleyiciler
        volumeSlider.onValueChanged.AddListener(delegate { ApplyVolume(); });
    }
    void Update()
    {
        float vol = AudioListener.volume;

        if (vol == 0f)
        {
            volumeOff.SetActive(true);
            volume1.SetActive(false);
            volume2.SetActive(false);
            volume3.SetActive(false);
        }
        else if (vol > 0f && vol <= 0.4f)
        {
            volume1.SetActive(true);
            volumeOff.SetActive(false);
            volume2.SetActive(false);
            volume3.SetActive(false);
        }
        else if (vol > 0.4f && vol <= 0.7f)
        {
            volume2.SetActive(true);
            volumeOff.SetActive(false);
            volume1.SetActive(false);
            volume3.SetActive(false);
        }
        else
        {
            volume3.SetActive(true);
            volumeOff.SetActive(false);
            volume1.SetActive(false);
            volume2.SetActive(false);
        }
    }
    public void ApplyVolume()
    {
        AudioListener.volume = volumeSlider.value / 10f;
        PlayerPrefs.SetFloat("volume", volumeSlider.value);

        if (volumeText != null)
            volumeText.text = volumeSlider.value.ToString("F0");
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void PrivacyPolicy()
    {
        Application.OpenURL("https://docs.google.com/document/d/14cr5XIC-qVY4K5JQFn922fuLfzwxp8Vx/edit?usp=drive_link&ouid=115843884881959542974&rtpof=true&sd=true");
    }
    public void TermOfUse()
    {
        Application.OpenURL("https://docs.google.com/document/d/1nGZ3jWRIr2h97mD9MYoNlT4hDS9xrAbU/edit?usp=drive_link&ouid=115843884881959542974&rtpof=true&sd=true");
    }
    public void Facebook()
    {
        Application.OpenURL("https://www.facebook.com/nrtcklc");
    }
    public void Instagram()
    {
        Application.OpenURL("https://www.instagram.com/nrtcklc/");
    }
    public void Twitter()
    {
        Application.OpenURL("https://twitter.com/nrtcklc");
    }
    public void VKontakte()
    {
        Application.OpenURL("https://vk.com/nrtcklc");
    }
    public void Telegram()
    {
        Application.OpenURL("https://t.me/nrtcklc");
    }
    public void Skype()
    {
        Application.OpenURL("https://join.skype.com/invite/nxQe1Tgz3PFE");
    }
    public void SendEmail()
    {
        string email = "nurtac85@gmail.com";
        string mailto = $"mailto:{email}";
        Application.OpenURL(mailto);
    }

    public void OpenStoreForRating()
    {
#if UNITY_ANDROID
        Application.OpenURL("market://details?id=" + packageName);
#else
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + packageName);
#endif
    }
    public void ShareApplication()
    {
#if UNITY_ANDROID
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", "android.intent.action.SEND");

        intentObject.Call<AndroidJavaObject>("putExtra",
            "android.intent.extra.TEXT",
            "Metraj & Hakediþ hesabý için kullandýðým uygulama:\n" + playStoreUrl);

        intentObject.Call<AndroidJavaObject>("setType", "text/plain");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

        currentActivity.Call("startActivity", intentObject);
#endif
    }

}